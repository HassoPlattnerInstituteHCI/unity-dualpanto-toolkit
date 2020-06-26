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
If you already have a Unity project, that's great. If not, create a new Unity 3D project, initialize a git repo with `git init` and add the [Unity .gitignore](https://github.com/github/gitignore/blob/master/Unity.gitignore).

Add this framework as a submodule into the Assets folder:
```
cd path/to/repo
cd Assets
git submodule add https://github.com/HassoPlattnerInstituteHCI/unity-dualpanto-framework
```

## Creating a Panto Application in Unity
### Adding the right components
Drag the Panto Prefab into your scene. You can find it at `Assets -> unity-dualpanto-framework -> Resources`. The Panto game object has different components attached to it: the DualPantoSync, the lower handle, the upper handle and a level. It also has a child component: the Panto Working Area. This is the area the DualPanto can reach.
![Panto Prefab in Scene](https://github.com/HassoPlattnerInstituteHCI/unity-dualpanto-framework/blob/readme_with_images/readme_images/panto_prefab_with_highlights.png)

### Setting up the camera
Prepare your scene by making sure the camera is facing straight down onto your scene. `Main Camera -> Projection` should be `orthographic`. Rotate it to `90` on the x axis, so it's facing downwards. Adjust the position, height and size so that you can see the entire area of the level.
![Camera Settings](https://github.com/HassoPlattnerInstituteHCI/unity-dualpanto-framework/blob/readme_with_images/readme_images/camera_with_highlights.png)

### Reducing the rendering quality
It is good practice to reduce the rendering quality of your application, you can do this via `Edit -> Project Settings -> Quality`. Select `Very Low` in the `Default` dropdown.
![Reduce Rendering Quality](https://github.com/HassoPlattnerInstituteHCI/unity-dualpanto-framework/blob/readme_with_images/readme_images/quality_highlights.png)

### Find out the serial port of your device 
At this point we still need to manually update the serial port of our panto before running the application.
Therefore we have to find the used serial port on our computer and replace the string "//.//COM3" in the Panto prefab under DualPantoSync _Port Name_.

On Windows open the _Device Manager_ and go to _Ports (COM & LPT)_. Under that tab you will find a device called "Silicon Labs CP..." with the usb serial port in brackets (e.g. "COM6"). 
The _Port Name_ in your Panto Object would hence after updating be "//.//COM6".

On Unix you can list your usb devices by using the command `ls /dev | grep cu.`
To find out which device your Panto is one easy way is to plug the device out and in again and to check in between which serial port disappeared. That's the one we want to use.
Copy the path of the port (e.g. "/dev/cu.SLAB_USBtoUART") into the _Port Name_ on the Panto Object.
![Setting Port Name](https://github.com/HassoPlattnerInstituteHCI/unity-dualpanto-framework/blob/readme_with_images/readme_images/portname_with_highlight.png)


### Your first Panto demo
In your Unity Scene, add a cube using `GameObject -> 3D Object -> Cube`. Attach the `MeHandle` component to this cube. It should now follow the movement of the Upper Panto Handle.
![Adding the player script to the cube](https://github.com/HassoPlattnerInstituteHCI/unity-dualpanto-framework/blob/readme_with_images/readme_images/adding_script_to_object.png)
  
**You can find more sample scenes to get inspired in `ExampleScenes`, the relevant scripts can be found in `ExampleScripts`**

### Testing your app
There are two ways to test your app:
* Using the emulator mode (default): For this you do not need a DualPanto, the device will be emulated. You should see two game objects that represent the two handles. The blue objects represents the lower handle, the green one the upper handle. When the handles are controlled by the user, both will follow the mouse. You emulate rotation input with `a` and `d`.
* Using a DualPanto: If you want to run the application on the Panto, make sure the Debug mode is disabled in the DualPantoSync component and the panto is connected to your computer. If you have no device connect, it will fall back to the emulator mode.
![First App](https://github.com/HassoPlattnerInstituteHCI/unity-dualpanto-framework/blob/readme_with_images/readme_images/first_app.gif)

### Using the Blind emulator
To get a better sense of what your game will feel to blind people, there is a small emulator for blind vision. By default, pressing `b` during game play will toggle this mode.
If you are using a panto, it will simply hide the game. If you are using the emulator, you should only see the two handles and a small area surrounding them.
This will work best if you disable environment lighting in the scene first: Open `Window -> Rendering -> Lighting Settings`, then set `Environment Setting -> Source` to `Color` and choose that color to be black. In addition, set `Environment Reflections -> Source` to `Custom`. You will need to do this for each scene.
![Blind mode emulator](https://github.com/HassoPlattnerInstituteHCI/unity-dualpanto-framework/blob/readme_with_images/readme_images/blind_mode.png)

## Troubleshooting

### Updating Submodules
if a function does not seem to exist (unity throws an error like "missing assembly reference") or if you try to use content that we released at a later stage than when the framework was released, try to update your submodules first before reaching out to us (we will always use the latest state of the submodules when we try to debug your code). 

git submodule update --remote

### How do I turn my dualPanto device on?
On the back of your dualPanto device is a power switch. Push so that it turns to **On** and make sure the battery is charged.

### How do I reset my dualPanto device?
On the back of your dualPanto device is a button next to the cable connection. Move the linkages back in the closing position, turn the handles so they point to the right, press the button and wait 3 seconds.

### dualPanto handles not moving inside the game/Message _Revision id not matching. Try resetting the panto._ appears.
Try to reset the dualPanto device using the button on the back. For this see _How do I reset my dualPanto device._

### dualPanto handles not moving physically.
Have you turned the device on? For this see [How do I turn my dualPanto device on?](https://github.com/HassoPlattnerInstituteHCI/unity-dualpanto-framework/blob/master/README.md#how-do-i-turn-my-dualpanto-device-on)

### dualPanto works sometimes/Message _Skipping god object_ appears.
Don't hold the handles too hard or push against the motors too hard.

### Debugging
If you keep having troubles, you can enable `show raw values` in the DualPantoSync component. A Popup will show you the raw position and rotation values Unity receives for each handle, how much time has passed since t last received a heartbeat from the device, the name of the port and the current protocol revision id.