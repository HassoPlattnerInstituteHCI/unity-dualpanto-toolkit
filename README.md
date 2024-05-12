# unity-dualpanto-toolkit

## Installation Guide (Using UnityPanto)

### Unity
Install 2021.3.0f1 (best 2020.1.6f1).

#### VisualStudio & Git
You will need something to edit code (e.g. the VisualStudio IDE or VisualStudio Code).
For version control you will need git.

### Install the ESP32 driver
- [Download](https://www.silabs.com/products/development-tools/software/usb-to-uart-bridge-vcp-drivers) the installer for your OS-Version.
- Run the installer.

### Adding the framework to your project
If you already have a Unity project, that's great. If not, create a new Unity 3D project, initialize a git repo with `git init` and add the [Unity .gitignore](https://github.com/github/gitignore/blob/master/Unity.gitignore).

[Download the latest release](https://github.com/HassoPlattnerInstituteHCI/unity-dualpanto-toolkit/releases/tag/v1.2) of this repository and drag it into Unity. 

Alternatively you can add this framework as a submodule into the Assets folder. This is recommended if you wish to make changes to the framework:
```
cd path/to/repo
cd Assets
git submodule add git@github.com:HassoPlattnerInstituteHCI/unity-dualpanto-toolkit
git submodule update --init --recursive
```
You can find the installation instructions for SpeechIO [here](https://github.com/HassoPlattnerInstituteHCI/SpeechIOForUnity#installation). 

## Creating a Panto Application in Unity
### Adding the right components
Drag the Panto Prefab into your scene. You can find it at `Assets -> unity-dualpanto-toolkit -> Assets -> Resources`. The Panto game object has different components attached to it: the DualPantoSync, the lower handle, the upper handle and a level. It also has a few child objects, including Panto Working Areas for different device versions. This is the area the DualPanto can reach.
![Panto Prefab in Scene](/Documentation/readme_images/panto_prefab_with_highlights.png)

### Setting up the camera
Prepare your scene by deleting the `Main Camera` object that is created with every new scene in Unity. The Panto Prefab, that you just added to the scene, already contains a camera that is adjusted to show a top-down perspective of the entire area the Panto can reach.
If the scene is very bright (white panto-area), also delete the directional light that is part of any new scene. The Panto Prefab also contains one of those.

### Find out the serial port of your device, if needed
You may be able to skip this step. The Panto will by default use the most common port name on your OS: `//.//COM3` (Windows), `/dev/cu.SLAB_USBtoUART` (MacOS) or `/dev/ttyUSB0` (Linux). Check if your Panto is already being found by connecting it and hitting Play (make sure Debug is disabled). If it works, you can skip this part!

If it doesn't work, you need to manually update the serial port of your panto before running the application.
Therefore you have to find the correct port name and enter it in DualPantoSync's _Overwrite Default Port_.

On Windows open the _Device Manager_ and go to _Ports (COM & LPT)_. Under that tab you will find a device called "Silicon Labs CP..." with the usb serial port in brackets (e.g. "COM6"). 
The _Port Name_ in your Panto Object would hence after updating be "//.//COM6".

On Unix you can list your usb devices by using the command `ls /dev | grep cu.`
To find out which device your Panto is one easy way is to plug the device out and in again and to check in between which serial port disappeared. That's the one we want to use.
Copy the path of the port (e.g. "/dev/cu.SLAB_USBtoUART") into the _Port Name_ on the Panto Object.
![Setting Port Name](/Documentation/readme_images/portname_with_highlight.png)


### Your first Panto demo
In your Unity Scene, add a cube using `GameObject -> 3D Object -> Cube`. Attach the `MeHandle` component to this cube. It should now follow the movement of the Upper Panto Handle.
![Adding the player script to the cube](/Documentation/readme_images/adding_script_to_object.png)
  
**You can find more sample scenes to get inspired in `ExampleScenes`, the relevant scripts can be found in `ExampleScripts`**

### Testing your app
There are two ways to test your app:
* Using the emulator mode (default): For this you do not need a DualPanto, the device will be emulated. You should see two game objects that represent the two handles. The blue objects represents the lower handle, the green one the upper handle. When the handles are controlled by the user, both will follow the mouse. You emulate rotation input with `a` and `d`.
* Using a DualPanto: If you want to run the application on the Panto, make sure the Debug mode is disabled in the DualPantoSync component and the panto is connected to your computer. If you have no device connect, it will fall back to the emulator mode.
**If you do not see the cube in Game View:** Refer to the next section.
![First App](/Documentation/readme_images/first_app.gif)

### Using the Blind emulator
To get a better sense of what your game will feel to blind people, there is a small emulator for blind vision. You can use `b` to toggle between blind view (only the handles and the area is displayed), mixed mode (the outlines of collider will also be displayed) and development mode (everything is displayed).
Blind Mode                 | Mixed Mode                | Develop Mode
:-------------------------:|:-------------------------:|:--------------------:
![Blind Mode](/Documentation/readme_images/blind_mode.jpg)            |  ![Mixed Mode](/Documentation/readme_images/mixed_mode.jpg)          | ![Develop Mode](/Documentation/readme_images/develop_mode.jpg)

## Troubleshooting

### Updating Submodules
if a function does not seem to exist (unity throws an error like "missing assembly reference") or if you try to use content that we released at a later stage than when the framework was released, try to update your submodules first before reaching out to us (we will always use the latest state of the submodules when we try to debug your code). 

`git submodule update --remote`

### How do I turn my dualPanto device on?
On the back of your dualPanto device is a power switch. Push so that it turns to **On** and make sure the battery is charged.

### How do I reset my dualPanto device?
On the back of your dualPanto device is a button next to the cable connection. Move the linkages back in the closing position, turn the handles so they point to the right, press the button and wait 3 seconds.

### dualPanto handles not moving inside the game/Message _Revision id not matching. Try resetting the panto._ appears.
Try to reset the dualPanto device using the button on the back. For this see _How do I reset my dualPanto device._

### dualPanto handles not moving physically.
Have you turned the device on? For this see [How do I turn my dualPanto device on?](https://github.com/HassoPlattnerInstituteHCI/unity-dualpanto-toolkit/blob/master/README.md#how-do-i-turn-my-dualpanto-device-on)

### dualPanto works sometimes/Message _Skipping god object_ appears.
Don't hold the handles too hard or push against the motors too hard.

### Game objects do not collide with obstacles in debug mode.
Make sure you use `HandlePosition()` instead of `GetPosition()`. See the [documentation](https://github.com/HassoPlattnerInstituteHCI/unity-dualpanto-toolkit/blob/master/Assets/documentation/documentation.md) for more info on usage.

### No input or output from the DualPanto is arriving, the Console is showing *Received sync*, but no *Received heartbeat*.  
This might be due to obstacles registering before the device is ready. Insert a `Task.Delay(1000)` to wait 1 second before registering an obstacle.  

### The device keeps crashing
You might be adding too many obstacles at once. The Panto has a limited capacity for the amount and size for obstacles it can store at any time. A large obstacle takes up as much capacity as many small ones. If you only need obstacles on one handle, it is good practice to only only toggle `onUpper` or `onLower`.

### Debugging
If you keep having troubles, you can enable `show raw values` in the DualPantoSync component. A Popup will show you the raw position and rotation values Unity receives for each handle, how much time has passed since t last received a heartbeat from the device, the name of the port and the current protocol revision id.

### I'm having issues with Unity
Please refer to the Wiki.
