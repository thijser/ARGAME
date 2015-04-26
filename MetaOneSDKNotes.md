Meta One SDK Notes
==================
This file contains observation notes regarding the Meta One SDK. This file 
serves as a reference for solving issues and recording important information
about the workings of the Meta One SDK that might help to use the various 
features of the SDK.

Note: The Meta One (probably) needs support from the native MetaVision library,
deployed as a DLL in `Plugins/MetaVisionDLL_1_5_0_1184.dll`. However, as this is 
a native library (not a C# library), Unity can only link to it on Windows... As 
such it may not work on Linux or Mac OS X. On Mac OS X, at least, Unity logs an 
error message stating failure to load the abovementioned library. However, basic 
functionality such as the Meta One camera and display seem to work on Mac OS X.

Setting up the Meta SDK
-----------------------
The Meta SDK does not compile out of the box on Unity 5. The steps in this 
section should be followed after importing the Meta SDK in Unity to fix issues 
and optimise workflow. Note that these issues may or may not occur, and if the 
issue does not occur, you should leave everything as-is to prevent accidentally
breaking something else.

First, manually run the MSVC Redist in the Libraries directory of the 
Meta SDK Program Files directory if you haven't already done so. You may need 
to restart Unity for it to find the MSVC libraries required by the Meta SDK.

The Meta SDK uses a class that was in Unity 4 but is deprecated and has been 
removed from Unity 5. As such, to use the SDK, you have to navigate to 
`Plugins/HighlighingSystem/Scripts/` and open the `HighlightableObject` script. 
Find the use of the class `ClothRenderer` and observe that this section is 
surrounded by a preprocessor directive `#if !UNITY_FLASH … #endif`. Replace the 
clause of the directive to `UNITY_4 && !UNITY_FLASH` to only enable the feature 
on Unity 4. Disabling this part of the script has no noticeable effect on the 
operation of the SDK unless the Cloth class is used (which is the single 
replacement for a few classes dedicated to rendering clothing).

The Console may report repeated Exceptions originating from 
`SyntaxTree.VisualStudio.Unity.Bridge.*`. As this clutters Console output and 
is caused by an Editor plugin included in the Meta SDK, it is possible to remove
the plugin from the Assets directory to prevent these errors. Removing the 
Editor plugin does not affect the operation of the SDK, so it is safe to remove. 
To do so, simply remove the UnityVS directory in the SDK, and restart Unity (to 
force Unity to reload the Editor plugins). The exception reports should then 
have stopped.

The `_MetaWorld` Prefab may contain an invalid reference to a script. Look for
`_MetaWorld/MetaFrame/MetaDisplay/HeatDisplay` and set the Behaviour script to
`IRDisplay`. Then go to the MetaDisplay GameObject and set the value of the
field `IR Display` to `HeatDisplay`.

Using the Meta SDK
------------------
Because the Meta SDK is still in Beta, documentation is sometimes unclear, 
incorrect or even missing. An overview of main features of the SDK is collected
here for reference.

Documentation for the Meta SDK (as far as available) can be found under 
Window > Meta. However, documentation of some rather important parts of the SDK
(such as the 'Meta Input' and 'Meta Gesture Manager' entries) are 'Coming Soon'
and there's only a limited set of examples available.

It is possible to verify if the libraries and drivers are properly installed and 
can be used. See the "Meta Setup Guide" topic of the Meta documentation for 
details.

Main Prefab to use is `_Meta/_MetaCore/_Prefabs/_MetaWorld` (including 
underscores). This includes the MetaCamera and a directional light for 
projecting the scene through the Meta glasses.

The Meta SDK supports a Full and Rectified device profile. The Rectified profile 
translates real-world coordinates to screen coordinates 1 to 1, while the Full 
profile shows everything the camera can see (effectively providing a zoomed-out 
view). To switch, find the Camera Profile setting in the `_MetaWorld` prefab and 
choose either `meta1_full` or `meta1_rectified`.

Objects can be made to respond to interactions by adding the MetaBody behaviour 
script to it. The MetaCore script can be found in 
`_Meta/_MetaCore/Scripts/MetaBehaviour/`.

`MonoBehaviour` scripts that use Meta interactions and events extend from 
`MetaBehaviour` instead (this adds event handlers for Meta gestures). 
Available events: OnClick, OnHover, OnGrab, OnRelease, OnMove and 
OnMoveRelease.

MGUI is based on NGUI and adds Meta-controlled buttons and other user-interface 
elements. Use the MGUIRoot prefab as a starting point, and add user interface 
elements as desired (such as MetaPanel, MetaButton, ...)

`MetaTarget` is a script that identifies a marker target. `MetaTracker` is the 
script that tracks the target.
