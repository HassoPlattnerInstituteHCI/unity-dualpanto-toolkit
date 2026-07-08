# unity-dualpanto-toolkit
## Installation Guide

### 1. Install Unity
- Install [Unity Hub](https://unity.com/download).
- Install editor version 6000.3.17f1 (alternatively 6000.3.18f1) from the [Unity Download Archive](https://unity.com/de/releases/editor/archive).
### 2. Install Code Editor & Git
- You will need something to edit C# code (VSCode or Rider work great, VisualStudio IDE is fine too).
- For version control you will need git.

### 3. Install the ESP32 driver
If you haven't already, get the driver needed to communicate with the dualpanto microcontroller
- [Download](https://www.silabs.com/products/development-tools/software/usb-to-uart-bridge-vcp-drivers) the installer for your OS-Version.
- Run the installer.

### 4. Adding the framework to your project
1. Open a blank 3D Unity project.
2. Initialize a git repo by running `git init` in your project directory. If you want, [add a remote](https://docs.github.com/en/get-started/git-basics/managing-remote-repositories)
3. Add the [Unity .gitignore](https://github.com/github/gitignore/blob/master/Unity.gitignore) at the root of your project directory
4. Add this framework as a submodule into the Assets folder:
```
cd path/to/repo
cd Assets
git submodule add git@github.com:HassoPlattnerInstituteHCI/unity-dualpanto-toolkit
git submodule update --init --recursive
```
You can find the installation instructions for SpeechIO [here](https://github.com/HassoPlattnerInstituteHCI/SpeechIOForUnity#installation). 

## Creating a Panto Application in Unity
### 1. Adding the right components
Drag the Panto Prefab into your scene. You can find it at `Assets -> Resources` or `Assets -> unity-dualpanto-toolkit -> Assets -> Resources`, depending on how you imported the package. The Panto game object has different components attached to it: the DualPantoSync, the lower handle, the upper handle and a level. It also has a few child objects, including Panto Working Areas for different device versions. This is the area the DualPanto can reach.
#### Don't worry if your scene looks odd, this will be taken care of in the next step.
![Panto Prefab in Scene](/Documentation/readme_images/panto_prefab_with_highlights.png)

### 2. Setting up the camera and lighting
After adding the Panto Prefab, the scene might look overly bright, or be shown from a weird camera angle. The Panto Prefab already contains it's own light source and a fittingly positioned camera. You can therefore delete the `Main Camera` object that is created with every new scene in Unity. If the scene is very bright (white panto-area), also delete the directional light that is part of any new scene. If the light is still to bright, expand the "Panto" game object in the object tree, select its "Directional Light" object and lower its "Intensity" property.

### 3. Find out the serial port of your device, if needed
You may be able to skip this step. The Panto will by default use the most common port name on your OS: `//.//COM3` (Windows), `/dev/cu.SLAB_USBtoUART` (MacOS) or `/dev/ttyUSB0` (Linux). Check if your Panto is already being found by connecting it and hitting Play (make sure Debug is disabled). If it works, you can skip this part!

If it doesn't work, you need to manually update the serial port of your panto before running the application.
Therefore you have to find the correct port name and enter it in DualPantoSync's _Overwrite Default Port_.

**On Windows:**
- Open the _Device Manager_ and go to _Ports (COM & LPT)_.
- Under that tab you will find a device called "Silicon Labs CP..." with the usb serial port in brackets (e.g. "COM6"). 
The _Port Name_ in your Panto Object would hence be needed to be changed to "//.//COM6".

**On Unix:**
- With dualpanto disconnected, run `ls /dev/cu.*` to list your USB devices
- Repeat with dualpanto attached and check which of the listed serial ports is new
- Copy the path of the port (e.g. "/dev/cu.SLAB_USBtoUART") into the _Port Name_ on the Panto Object.
![Setting Port Name](/Documentation/readme_images/portname_with_highlight.png)


### 4. Creating a simple interaction with dualpanto
- In your Unity Scene, add a cube using the global menu `GameObject -> 3D Object -> Cube`, or through the right-click menu in the game object hierarchy.
- Select the cube and attach the `Me Handle` component to it.
When you run your application in the next section, the cube should follow the movement of the Upper Panto Handle.
![Adding the player script to the cube](/Documentation/readme_images/adding-script-to-object-2.jpg)
- Create obstacles by adding a cube using the global menu GameObject -> 3D Object -> Cube and attaching the Panto Box Collider component to it.
- Create an empty GameObject in the scene and attach the Obstacle Manager component to it. This will find all Panto Colliders in the scene and create obstacles for them on start. You can toggle obstacles on/off at runtime with the E and D keys.
  
**You can find more sample scenes to get inspired in `ExampleScenes`, the relevant scripts can be found in `ExampleScripts`**

### 5. Testing your app
- run your Application by pressing the play button on top of Unity
- **mac users** might have to allow executing `libserial.dylib` in settings
  - refer to [Troubleshooting](#troubleshooting) if this or other problems arise

#### NOTE: If your don't see your game objects while running, press `b`to toggle visibility modes (see [Using the Blind emulator](#using-the-blind-emulator))

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

## Multi-Scene Applications
If your application spans multiple Unity scenes, see the [Scene Manager documentation](Documentation/documentation_scenes.md) for setting up scene navigation while keeping the DualPanto connection alive.

## Troubleshooting

### MacOS Error "libserial.dylib not opened"
If you recieve a popup like this upon first launch of you application, force quit Unity with `cmd+option+esc` to be able to close the popup. Then go to `Settings -> Privacy & Security`, scroll to the bottom and allow opening the library.

### Updating Submodules
if a function does not seem to exist (unity throws an error like "missing assembly reference") or if you try to use content that we released at a later stage than when the framework was released, try to update your submodules first before reaching out to us (we will always use the latest state of the submodules when we try to debug your code). 

`git submodule update --remote`

### How do I turn my dualPanto device on?
On the back of your dualPanto device is a power switch. Push so that it turns to **On** and make sure the battery is charged.

### How do I reset/calibrate my dualPanto device?
On the back of your dualPanto device is a button next to the USB port. Move the linkages back in the closing position, turn the handles so they point to the right, press the button and wait 3 seconds.

### Unity freezes upon starting the dualpanto application
The toolkit resets the device over its serial control lines before connecting, so the firmware boots fresh and completes the SYNC handshake. If the device still does not answer within the configured timeout, connecting aborts with a Console error instead of freezing Unity. If you see that error, power-cycle the Panto or press the reset button next to the USB port, then press Play again.

### dualPanto handles not moving inside the game/Message _Revision id not matching. Try resetting the panto._ appears.
Try to reset the dualPanto device using the button on the back. For this see _How do I reset my dualPanto device._

### dualPanto handles not moving physically.
Have you turned the device on? For this see [How do I turn my dualPanto device on?](https://github.com/HassoPlattnerInstituteHCI/unity-dualpanto-toolkit/blob/master/README.md#how-do-i-turn-my-dualpanto-device-on)

### dualPanto works sometimes/Message _Skipping god object_ appears.
Don't hold the handles too hard or push against the motors too hard.

### Game objects do not collide with obstacles in debug mode.
Make sure you use `HandlePosition()` instead of `GetPosition()`. See the [documentation](https://github.com/HassoPlattnerInstituteHCI/unity-dualpanto-toolkit/blob/develop/Documentation/documentation.md)  for more info on usage.

### No input or output from the DualPanto is arriving, the Console is showing *Received sync*, but no *Received heartbeat*.  
This might be due to obstacles registering before the device is ready. Insert a `Task.Delay(1000)` to wait 1 second before registering an obstacle.  

### The device keeps crashing
You might be adding too many obstacles at once. The Panto has a limited capacity for the amount and size for obstacles it can store at any time. A large obstacle takes up as much capacity as many small ones. If you only need obstacles on one handle, it is good practice to only only toggle `onUpper` or `onLower`.

### Debugging
If you keep having troubles, you can enable `show raw values` in the DualPantoSync component. A Popup will show you the raw position and rotation values Unity receives for each handle, how much time has passed since t last received a heartbeat from the device, the name of the port and the current protocol revision id.

### I'm having issues with Unity
Please refer to the Wiki.
