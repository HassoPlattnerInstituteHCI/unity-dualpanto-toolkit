using System;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

namespace DualPantoToolkit
{
    /// <summary>
    /// Best-effort hard reset of the Panto through the serial control lines (DTR/RTS),
    /// used to make the firmware's SYNC handshake deterministic on every connect.
    ///
    /// Background: the firmware only initiates the SYNC handshake right after booting.
    /// Whether the ESP32 reboots when the serial port is (re)opened depends on driver
    /// side effects, which is why reconnecting (e.g. re-entering Play mode) sometimes
    /// hangs waiting for a SYNC that never comes. Pulsing the lines explicitly removes
    /// that dependency on driver behavior.
    ///
    /// The pulse follows the standard ESP32 auto-reset sequence (same as esptool and
    /// the Arduino IDE): assert RTS (EN low) while DTR is deasserted (IO0 high), wait,
    /// release RTS. Keeping IO0 high during the EN rising edge is what boots the chip
    /// into the firmware instead of the serial bootloader. On boards without the
    /// standard auto-reset wiring the pulse is a harmless no-op on the chip.
    ///
    /// Implemented via P/Invoke because System.IO.Ports is not available at Unity's
    /// .NET Standard API compatibility level.
    /// </summary>
    public static class SerialPortReset
    {
        /// <summary>
        /// Pulses the reset line of the device behind <paramref name="portName"/>.
        /// Returns true if the pulse was sent, false if the port could not be opened
        /// or the platform call failed. Never throws.
        /// </summary>
        public static bool TryHardReset(string portName)
        {
            try
            {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
                return HardResetWindows(portName);
#else
                return HardResetPosix(portName);
#endif
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[DualPanto] Could not reset device on {portName}: {e.Message}");
                return false;
            }
        }

        private const int ResetPulseMs = 100;

#if !UNITY_EDITOR_WIN && !UNITY_STANDALONE_WIN
        // --- POSIX (Linux / macOS): toggle the modem control lines via ioctl(2) ---

        [DllImport("libc", EntryPoint = "open", SetLastError = true)]
        private static extern int PosixOpen(string pathname, int flags);
        [DllImport("libc", EntryPoint = "close")]
        private static extern int PosixClose(int fd);
        [DllImport("libc", EntryPoint = "ioctl", SetLastError = true)]
        private static extern int PosixIoctl(int fd, UIntPtr request, ref int argp);

        private const int TIOCM_DTR = 0x002;
        private const int TIOCM_RTS = 0x004;

        private static bool HardResetPosix(string portName)
        {
            bool isLinux = Application.platform == RuntimePlatform.LinuxEditor
                || Application.platform == RuntimePlatform.LinuxPlayer;

            // open(2) flags and ioctl(2) request numbers differ between Linux and Darwin
            int O_RDWR = 2;
            int O_NOCTTY = isLinux ? 0x100 : 0x20000;
            int O_NONBLOCK = isLinux ? 0x800 : 0x4;
            UIntPtr TIOCMBIS = (UIntPtr)(isLinux ? 0x5416u : 0x8004746Cu); // set line bits
            UIntPtr TIOCMBIC = (UIntPtr)(isLinux ? 0x5417u : 0x8004746Bu); // clear line bits

            int fd = PosixOpen(portName, O_RDWR | O_NOCTTY | O_NONBLOCK);
            if (fd < 0) return false;
            try
            {
                int dtr = TIOCM_DTR;
                int rts = TIOCM_RTS;
                if (PosixIoctl(fd, TIOCMBIC, ref dtr) < 0) return false; // clear DTR: IO0 high, normal boot
                if (PosixIoctl(fd, TIOCMBIS, ref rts) < 0) return false; // set RTS: EN low, hold chip in reset
                Thread.Sleep(ResetPulseMs);
                if (PosixIoctl(fd, TIOCMBIC, ref rts) < 0) return false; // clear RTS: EN high, chip boots
                return true;
            }
            finally
            {
                PosixClose(fd);
            }
        }
#else
        // --- Windows: EscapeCommFunction on a CreateFile handle ---

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr CreateFileW(string fileName, uint access, uint shareMode,
            IntPtr security, uint disposition, uint flags, IntPtr template);
        [DllImport("kernel32.dll")]
        private static extern bool EscapeCommFunction(IntPtr handle, uint func);
        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr handle);

        private const uint GENERIC_READ = 0x80000000;
        private const uint GENERIC_WRITE = 0x40000000;
        private const uint OPEN_EXISTING = 3;
        private const uint SETRTS = 3;
        private const uint CLRRTS = 4;
        private const uint CLRDTR = 6;
        private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        private static bool HardResetWindows(string portName)
        {
            IntPtr handle = CreateFileW(portName.Replace('/', '\\'),
                GENERIC_READ | GENERIC_WRITE, 0, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
            if (handle == INVALID_HANDLE_VALUE) return false;
            try
            {
                if (!EscapeCommFunction(handle, CLRDTR)) return false; // clear DTR: IO0 high, normal boot
                if (!EscapeCommFunction(handle, SETRTS)) return false; // set RTS: EN low, hold chip in reset
                Thread.Sleep(ResetPulseMs);
                if (!EscapeCommFunction(handle, CLRRTS)) return false; // clear RTS: EN high, chip boots
                return true;
            }
            finally
            {
                CloseHandle(handle);
            }
        }
#endif
    }
}
