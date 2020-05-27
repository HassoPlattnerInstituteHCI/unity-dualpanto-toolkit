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

### Adding the framework to your project
Create a Unity project in a git repo, then add this framework as a submodule into the Assets folder:
```
cd path/to/repo
cd Assets
git submodule add https://github.com/HassoPlattnerInstituteHCI/unity-dualpanto-framework
```
You will also need the [UnitySpeechIO Project](https://github.com/HassoPlattnerInstituteHCI/SpeechIOForUnity).
```
cd path/to/repo
cd Assets
git submodule add https://github.com/HassoPlattnerInstituteHCI/SpeechIOForUnity
```

## Creating a new Panto Application in Unity
Prepare your scene by making sure the camera is facing straight down onto your scene.
Drag the Panto Prefab into your scene. The Panto game object has four components attached to it: the DualPantoSync, the lower handle, the upper handle and a level.

There are two ways to test your app:
* Using the emulator mode (default): For this you do not need a DualPanto, the device will be emulated. You should see two game objects that represent the two handles. The blue objects represents the lower handle, the green one the upper handle.
* Using a DualPanto: If you want to run the application on the Panto, make sure the Debug mode is disabled in the DualPantoSync component and the panto is connected to your computer.

### Find out the serial port of your device 

At this point we still need to manually update the serial port of our panto before running the application.
Therefore we have to find the used serial port on our computer and replace the string "//.//COM3" in the file _Assets -> PantoScripts -> DualPantoSync.cs_ with our serial port.

On Windows open the _Device Manager_ and go to _Ports (COM & LPT)_. Under that tab you will find a device called "Silicon Labs CP..." with the usb serial port in brackets (e.g. "COM6"). 
The string in your _DualPantoSync.cs_ would hence after updating be "//.//COM6".

On Unix you can list your usb devices by using the command `ls /dev | grep cu.`
To find out which device your Panto is one easy way is to plug the device out and in again and to check in between which serial port disappeared. That's the one we want to use.
Copy the path of the port (e.g. "/dev/cu.SLAB_USBtoUART") into the _DualPantoSync.cs_.

Now you're ready to run the application by clicking the play button. :)
