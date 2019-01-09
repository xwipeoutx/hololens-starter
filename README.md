# XR Starter

Little starter project for XR development.

Designed for Hololens (MRTK), but should be fine for other platforms too.

Includes

* MRTK (helpful utils for MR development)
* Photon (sharing)
* Vuforia (marker-based tracking)
* Initial tracking marker

## Prerequisites

Install the following - you can skip Android/IOS/UWP things depending on what platform you're targetting

* Unity 2018.3.0f2 (Tip: Use the [Unity Hub](https://unity3d.com/get-unity/download))
* Include the following Unity components when installing (if relevant for your platform)
  * Vuforia
  * Android Build Support
  * iOS Build Support
  * UWP Build Support (IL2CPP)
  * UWP Build Support (.NET)
  * Windows Build Support (IL2CPP)
* Visual Studio 2017 
* Include the following workloads (not 100% sure on these...)
  * Universal Windows Platform development
  * .NET desktop development
  * .NET Core cross-platform development
* Windows 10 SDK (tested with `10.0.17134.0`)
* Android SDK (tested with `android-28`)
* XCode or whatever you iOS folk use

## Usage

### Cloning and opening

* Clone this repo
* Open Unity Hub
* Click "Open"
* Navigate to `.\XR_Starter` and open
* Choose `2018.3.0f2` version when opening
* Wait for things to compile

### Use a better layout

The default Unity layout is bad.  Go to `Window` -> `Layouts` -> `Tall` for a better one.

I'd also suggest adjusting the following

* Change the `Project` tab to be `One Column Layout` using the view menu (hamburger) in the top right
![](https://i.imgur.com/UUWqy6b.png)
* Seperate the `Scene` tab from the `Game` tab so you can see them both at once
![](https://i.imgur.com/gwFan1H.png)

### Open the right scene

When you first launch, there's a good chance it will just be an empty scene

* In the inspector, navigate to `_game`, `Scene`, `Main` and double click it 

![](https://i.imgur.com/JSSxQMJ.png)

* Look around the scene by double-clicking game objects in the inspector to focus them, or holding down your `RMB` in the scene window and using `WASD` for movement whilst it is held down

### Set up a unique room id for sharing

* Select the `Sharing` GameObject

![](https://i.imgur.com/xmYy2RA.png)

* Enter a unique value for your own `Room Id` - if you're in a team, use the same Room Id.

### Targeting your platform

Unity will almost certainly be targeting PC standalone initially - which won't work for the sort of app we're building (Hololens or phone or whatever)

Switch to the appropriate platform:

* `File`, `Build Settings`
* Select:
  * Mixed Reality: `Universal Windows Platform`
  * Android: `Android`
  * iOS: `iOS`
* Select `Switch Platform`

### Test in the editor

Hit the `Play` button to start running it in the editor.  This uses MRTK's camera movement and interaction shims to simulate the Hololens and hands.

Use the following controls

* Right-Click drag: Look around
* WASD: While RMB is down, flies around
* Shift/Space: Bring virtual hands into view with finger up
* Click: Bring finger down

TLDR: hold down RMB and use WASD.  When you want to air tap, shift+click LMB

Regarding gameplay

* Air-tapping the cube will toggle it on or off (shared to all clients)
* All players in the same room will be visible as small avatars that move around

## Check your project builds

### Mixed Reality (Hololens or WMR occluded devices)

* Ensure your platform is set to UWP in Build Settings
* In the menu, choose `Mixed Reality Toolkit`, `Build Window`
* Click "Build All"

**Note**: If you have issues here, ensure you are building in `Release` mode for `x86` and targeting `Direct3D` (not `XAML`)

Your console should report success

### Android

* Ensure your platform is set to Android in Build Settings
* In build settings, select `Build`

This should give you an `.apk` file where you specify.

Let me know if you get "Build and Run" working, but I had to `adb install foo.apk` to get it installed on my device.

**Note**: Ensure ARCore is installed on your phone/tablet, to get much better tracking after the marker is found

### IOS

* Ensure your platform is set to Android in Build Settings
* In build settings, select `Build`

This will make an xcode project in the folder you specify.

You're on your own now, because iOS is not my wheelhouse

## Project structure

This project probably shouldn't be used for inspiration - but it'll do for a hacky workshop :)

### Scene Organisation

The scene is organised in the following structure

* Vuforia - _Everything needed to detect a marker and anchor the scene there_
* Sharing - _Connects to a Photon server and auto joins a room.  Click-free sharing._
* Mixed Reality Bits - _A few things to handle mixed reality - the cursor and input manager_
* MixedRealityCameraParent - _Prefab from MRTK with the same name._
  * Vuforia Instructions - _Guiding text and occlusion plane to identify Vuforia marker_
* SceneRoot - _The origin for the scene.  Will be placed dead in the middle of the tracking marker._
  * Togglable Spinning Cube - _Sample game object sharing state with all other devices_

### Assets

All app-specific assets should be put in the `_game` folder - this reduces clutter when it comes to importing store assets, which always end up in the root (annoyingly).

There are a couple exceptions, otherwise it would be too easy.

* Vuforia configuration is in `Assets/Resources/VuforiaConfiguration`
* Remote spawned prefabs (eg. the player in this case) need to go in `Assets/Resources`, as they are loaded by path
* Photon configuration is in `Assets/Photon/PhotonUnityNetworking/Resources/PhotonServerSettings`

## Overview

The initial scene is simple - a spinning cube (toggleable by clicking it) 

### Vuforia

Vuforia is configured entirely in the `Vuforia` prefab, along with the standard `VuforiaConfiguration` asset.

A couple scripts are involved to get it all going

* `VuforiaConfigurator` - This configures Vuforia appropriately for the target platform options, and handles setting the scene root based on the marker.
* `TurnOff Behaviour` - Simply turns off the renderer for the image target.  Out of the box Vuforia, why deviate from the norm?

### Sharing

This project uses [PUN](https://www.photonengine.com/en/pun) for sharing - Photon Unity Networking.

The `RoomManager` script is responsible for auto joining the room and spawning player prefabs as other players join.

The `Player` prefab contains a visual display of the remote user's position, and uses a `ShareLocalTransform` script to ensure the positions are sent and received to and from peers.

See the spinning cube for another example of sharing.
