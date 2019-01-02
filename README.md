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
* Windows 10 SDK (Tested with `10.0.17134.0`)
* Android SDK (test with `android-28`)
* XCode or whatever you iOS folk use

## Usage

* Clone this repo
* Open Unity Hub
* Click "Open"
* Navigate to `.\XR_Starter` and open
* Choose `2018.3.0f2` version when opening
* Wait for things to compile
* Select the `Sharing` GameObject
* Enter a unique value for your own `Room Id` - if you're in a team, use the same Room Id.

### Targeting your platform

In Unity

* `File`, `Build Settings`
* Select:
  * Mixed Reality: `Universal Windows Platform`
  * Android: Andorid
  * iOS: iOS
* Select `Switch Platform`

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

### Vuforia

Vuforia is configured entirely in the `Vuforia` prefab, along with the standard `VuforiaConfiguration` asset.

A couple scripts are involved to get it all going

* `StartVuforia` - This configures Vuforia appropriately for the target platform (See through for hololens, hand held otherwise), and disabled it in the editor.
* `EstablishOrigin` - This is the main workhorse for the scene placement - all it really does though is sets the position and rotation of the scene root with whatever is reported by Vuforia, and then (optionally) turns off Vuforia.  This also shows and hides the instructions.
* `TurnOff Behaviour` - Simply turns off the renderer for the image target.  Out of the box Vuforia, why deviate from the norm?

### Sharing

This project uses [PUN](https://www.photonengine.com/en/pun) for sharing - Photon Unity Networking.

The `RoomManager` script is responsible for auto joining the room and spawning player prefabs as other players join.

The `Player` prefab contains a visual display of the remote user's position, and uses a `ShareLocalTransform` script to ensure the positions are sent and received to and from peers.

See the spinning cube for another example of sharing.