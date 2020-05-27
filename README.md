# unity-dualpanto-framework

## Installation Guide (Using UnityPanto)

### Unity
Install [Unity Version 2019.2.12](https://unity3d.com/de/get-unity/download/archive) (best 2019.2.12f1).

#### VisualStudio & Git
You will need something to edit code (e.g. the VisualStudio IDE or VisualStudio Code).
For version control you will need git.

### Install the ESP32 driver

- [Download](https://www.silabs.com/products/development-tools/software/usb-to-uart-bridge-vcp-drivers) the installer for your OS-Version.
- Run the installer.

### SpeechIO
You will also need the [UnitySpeechIO Project](https://github.com/HassoPlattnerInstituteHCI/SpeechIOForUnity)

## Running your application on a dualpanto device

After adding the Panto object to your scene check it in the inspector and make sure that the debug mode is disabled

### Find out the serial port of your device 

At this point we still need to manually update the serial port of our panto before running the application.
Therefore we have to find the used serial port on our computer and replace the string "//.//COM3" in the file _Assets -> PantoScripts -> DualPantoSync.cs_ with our serial port.

On Windows open the _Device Manager_ and go to _Ports (COM & LPT)_. Under that tab you will find a device called "Silicon Labs CP..." with the usb serial port in brackets (e.g. "COM6"). 
The string in your _DualPantoSync.cs_ would hence after updating be "//.//COM6".

On Unix you can list your usb devices by using the command `ls /dev | grep cu.`
To find out which device your Panto is one easy way is to plug the device out and in again and to check in between which serial port disappeared. That's the one we want to use.
Copy the path of the port (e.g. "/dev/cu.SLAB_USBtoUART") into the _DualPantoSync.cs_.

Now you're ready to run the application by clicking the play button. :)