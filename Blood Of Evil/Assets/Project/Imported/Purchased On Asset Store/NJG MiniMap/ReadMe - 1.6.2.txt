----------------------------------------------
		  NJG Mini/World Map
    Copyright © 2013 - 2015 Ninjutsu Games LTD.
             Version 1.6.2
       http://www.ninjutsugames.com
         hjupter@ninjutsugames.com
----------------------------------------------

Thank you for buying NJG MiniMap!

If you have any questions, suggestions, comments or feature requests, please
email me at: hjupter@ninjutsugames.com

NJG MiniMap its a professional easy to use minimap and map solution powered by Unity GUI which helps it make it fully customizeable.

---------------------------------------
 Documentation
---------------------------------------
1. Click on Create > UI > Minimap
2. Assign your player target by using a layer or assign it manually into the UIMiniMap inspector.
3. Attach the "MapItem" component to the gameObject do you want to track.

https://docs.google.com/document/d/11CtGGOQjnT58W7whT5cj1HqrDwlU1ZRNAvS-Y2AJchY

If you have Tasharen FOW System installed you can simply define this "TasharenFOW" 
in the scripting define symbols field within the player settings

-----------------
 Version History
----------------- 

1.6.2
- NEW: Huge performance improvements overall
- NEW: Added new smooth position options for icons on Settings
- NEW: Updated LeanTween. Stripped down everything leaving the DLL only.
- FIX: Fixed issues causing Unity crash
- FIX: Changing a marker type on map item inspector now works properly

1.6.1
- NEW: Easier setup, minimap ready in 2 Clicks!!. Just go to Create (or Right Click) > UI > Minimap and your minimap is ready to use and customize.
- NEW: Editor preview. No need to hit play to see how the map texture looks.
- NEW: Revamped and simplified demo.
- NEW: Map now use image mask for icons.
- NEW: Icon sprite is now displayed on Map Item inspector.
- FIX: Player icon movement now looks smooth.
- FIX: Fixed issues with arrows, they now works as it should.
- FIX: Removed some unnesessary UI options from minimap inspector.
- FIX: Fixed a lot of issues with rendering and positions.
- FIX: The package no longer requires Photon.

NOTE: Examples scenes are being worked on.


1.6.0
- NEW: Unity 5 ready.
- NEW: Unity UI version! Yay!
- NEW: WebGL, Windows Phone support (including the FOW system).
- NEW: Network demo with photon, soon with UNET
- NEW: Zones can now render the map on trigger making this very useful for cases like if you enter a house and only want to display the inside etc.
	   You can set custom render layers and minimap/worldmap zoom.
- NEW: Map.instance.MapToWorld() works as expected now, it even gives you the exact position on the ground. This can be used to move the camera around or spawn things using the map. New demo for this is coming soon!
- NEW: NJGMap settings is now saved on a Resources folder for global use, no need to have NJGMap script on the scene anymore
- NEW: Added new Map class which will handle minimap and worldmap old MiniMap and WorldMap classes has been deleted
- NEW: Revamped code to eliminate innecessary classes and methods
- NEW: Revamped NGUI version
- NEW: NGUI version prefab now use the new NGUI anchor system
- NEW: Is now possible to disable "keys" for shortcuts (Toggle map, zoom in/out, etc)
- NEW: Added an onTargetChanged event which will be triggered every time the Map target is changed
- NEW: Removed shader type there's only 2 shaders one for FOW and another for regular map
- NEW: Added onFOWEnabled and onFOWDisabled events
- NEW: Renamed onWorldNameChange to onWorldZoneChange and its now static. This can be useful to toggle objects by comparing the zone name, check out the roof of the house on the demo.
- NEW: Added always visible icon and arrow on the icon editor.
- FIX: Reduced memory allocation and some minor performance optimizations
- FIX: Fixed an issue with KeySelector
- FIX: Fixed an issue where map wasnt being displayed with the current zoom at the begining

1.5.7
- NEW: The map system wont break anymore if it doesnt have a target.
- FIX: Bug fixes

1.5.6
- NEW: MapItem now reacts properly with FOW showing and hiding if its covered by NJGFOW.
- NEW: Added "Dont Destroy" option if you want to make NJGMap instance persistant.
- NEW: Added "Render OnLevelLoad" option which will make map to render automatically when a level loads.
	   This will only work if "Dont Destroy" option is enabled.
- NEW: Added a new prefab standalone prefab for NGUI version if you want to use outside of your UI.
- NEW: Added a new scene for standalone prefab.
- FIX: Fixed an issue with NJGTools.
- FIX: Fixed a small issue with NJGZones

1.5.5
- NEW: Tested with NGUI 3.6.7
- NEW: Added "exploredRatio" property to get the range of explored ratio of the map.
- NEW: Added a new example scene showing how to make transparent roads for minimap.
- NEW: Added a new property to set "custom bounds" center.
- NEW: Bounds Offset now affects custom bounds too.
- NEW: Switching to FOW shader type on UIMinimap and UIWorldMap inspectors will turn on FOW on NJGMap inspector automatically.
- FIX: Fixed a small issue with Tasharen FOW System
- FIX: Fixed an issue when switching of scene using the same NJGMinimap system.
- FIX: Renamed "collider" property to "zoneCollider" on MapZone to avoid some issues with collider being duplicated.
- FIX: Few improvements small improvements to the NJGMap inspector.
- FIX: Fixed panning and some other issues with the Mesh Version.

1.5.4
- NEW: Map Zones can now generate the texture map on trigger. Added toggle to the MapZone component.
- NEW: NGUI 3.6.4 support.
- NEW: Updated Demo scenes.
- NEW: Optimized shaders for mobiles.
- NEW: Performance optimizations.
- NEW: Switched to LeanTween for better performance and more platforms supported.
- NEW: Simplified the prefab. All you have to do now is drop it inside your UIRoot.
- DEL: Removed "Layer" option from NJGMap inspector since its no longer needed.
- FIX: Fixed some issues with inspectors.
- FIX: Fixed a bunch of issues with the prefabs.
- FIX: Render camera will work fine on unity 4.3 with occlusion culling.
- FIX: FOW shaders will change properly when FOW its turned on.

1.5.3
- NEW: There is now 2 packages for each version. (NGUI, Mesh)
- NEW: NGUI 3.0.8 support
- FIX: Fullscreen functionality has been improved (animated fullscreen, icons position and masking etc.) <- NGUI Only!
- FIX: Some tweaks to both versions.

1.5.2
- NEW: NGUI 3.0.7 f2 support
- NEW: Added a new prefab with a resizable and dragable World Map.
- NEW: Added a new fullscreen button for the world map. (See the scene "Example - 2D Resizable")
- FIX: Zoom keys won't affect minimap while world map is open anymore.
- FIX: Fixed other bugs that was not letting the Mesh Version work alone.

1.5.1
- NEW: Unity 4.3 support
- NEW: NGUI 3.0.6 f7 support
- FIX: Fixed a bug with Dynamic Render mode for scenes modified at run-time.
- FIX: Selection sprites should display fine when is assigned under icons settings.
- FIX: The package should now works fine without NGUI.

1.5.0
- NEW: NGUI 3.x support added.
- NEW: Maps are now rendered on Quad's instead of UITextures due to a glitch with NGUI 3.x.
- NEW: Added depth property for maps.
- NEW: Revamped inspectors make it them easier.
- NEW: Updated demo skin.
- NEW: Scene bounds are now calculated automatically if there is any change on the current scene.
- NEW: Its now possible to preview scene bounds on scene view.
- NEW: Now minmimap inspector will round size and margin. 
- NEW: Map icons interaction can be now enabled or disabled.
- NEW: Added double resolution rendering.
- NEW: Map icons now can be selected. Added a new option on the editor so is now possible to select which sprite use for selection display of any specific icon type.
- NEW: Its now possible to render multiple terrains.
- NEW: Added default empty icon type so MapItem can be assigned to an object using this type to just reveal NJGFOW.
- NEW: UILabelCoords now have 'format' property so it can be cusomizable.
- NEW: Toggle keys (rotation lock, world map etc) will not be triggered when an input has focus.
- NEW: Added a prefab ready to drop it into your own UI.
- FIX: Scenes that are not positioned at 0,0,0 can be rendered without issues.
- FIX: Minimap and Worldmap inspectors now save properties as they should.
- FIX: Simplified prefabs hierarchy.
- FIX: Few more bug fixes.
- DEL: Removed Update Bounds button since its no longer needed.

1.4.1
- NEW: Backwards compatibility with Unity 3.5.7
- NEW: Zoom now use floats instead of ints.
- NEW: Added zoom amount property to the inspectors.
- NEW: There is now 2 NGUI distribution packages one for Unity 3.x and Unity 4
- FIX: More code optimizations, types are now defined as ints instead of strings.
- FIX: Deleted imported NGUI free.
- FIX: Fixed a minor issue with bounds calculation.
- FIX: FOW Demo should work fine now.

1.4.0
- NEW: Added new alternate way to render the maps (NGUI is not required anymore but its still supported)
- NEW: Added a texture packer for non-NGUI system.
- NEW: Restructured the whole code in order to support both render engines. 
- NEW: OnGUI: Added new IconSelector class to select icons and arrows from a specific folder. 
- NEW: Panning added for both Mini map and World map with configurable settings: easing, speed, sensitivity and more.
- NEW: Added Render Modes now you can specify between these modes:
	   - Once: Render the map only once
	   - ScreenChange: Render the map once and re-render it again if the screen size changes
	   - Dynamic: Render the map every specified times or by calling the method NJGMap.instance.GenerateMap()
- NEW: Removed TweenZoom and replaced with HOTTween.
- NEW: Added super fast Fog of War rendered by GPU and calculated on a another thread for performance.
- NEW: Added big terrain example.
- NEW: Pivot option for MiniMap, its now easier to change the alignment of the minimap.
- NEW: Mouse Scroll Wheel zoom with configurable settings: easing, speed, zoom keys and more.
- NEW: WorldMap can use zoom now.
- NEW: MiniMap size inspector option.
- FIX: Lots of performance optimizations.
- FIX: Fix a leak memory issue.
- FIX: Few more bug fixes.

1.3.2
- NEW: Its now possible to generate automatically transparent images by removing its background color.
- NEW: New Shader for transparent maps added.
- NEW: Map orientation option now you can generate maps for 2D Games. Demo scene has been added.
	   Note: Arrows doesn't work for now but this will be fixed on the next updates.
- NEW: UIMinimap and UIWorldMap has the option to choose between texture mask and color mask shader.
- NEW: Its now possible to re-create map texture at runtime.
- NEW: Added new mask textures.
- NEW: Rotate option for arrows, now its possible lock arrows rotation.
- FIX: Bounds calculation has been improved.
- FIX: Few minor fix and code optimizations.
- FIX: Fixed a bug with world name label.
- FIX: Fixed a bug with MapItem not pulling new map items properly.

1.3.0
- NEW: Map orientation option now you can generate maps for 2D Games too.
- NEW: New TweenZoom making possible to tween the zoom with easing.
- NEW: New properties to the Minimap inspector so you can edit easing and speed of the zoom.
- NEW: Keys option zoomIn/Out the Minimap.
- NEW: Key option for lock the Minimap rotation.
- NEW: Key selector wizard editor for easy keycode selection.
- NEW: Improved Tooltip now follows the mouse position.
- NEW: Added an option to calculate minimap border radius automatically or set it manually.
- NEW: Rewrited UIWorldMap class.
- FIX: Bounds calculation has been improved.
- FIX: Now minimap updates correctly the mask when is saved as prefab and you use the same prefab on different scenes.
- FIX: Few minor fix and code optimizations.

1.2.0
- NEW: Its now possible to generate a texture manually during edit time and save it. 
	   Giving you freedom to grab a photo of your scene and make a nice map on photoshop or whatever tool you want.
	   A demo scene has been added.
- NEW: Some modifications to the inspectors.
- NEW: Added a key option to the Minimap class to toggle the world map.
- NEW: Minimap Icons root now will follow uiTexture rotation.
- NEW: Minimap inspector now have exposed the border radius limit, icons will get culled if they get farther this radius.
- FIX: Icons no longer make scaling animation on the world map. 
- FIX: Minor fix where UILabelCoords was displaying the letter Z instead of Y.

1.1.0
- NEW: Added direction arrows
- NEW: Revamped NJGMap editor
- NEW: Added an option to choose if an icon have arrow
- NEW: Map Zones
- NEW: Map Zones editor
- FIX: Updated Mask Shader to support mobiles
- FIX: Bunch of editor modifications and bug fixes
- FIX: Bunch of glitches and some code optimizations
- FIX: Icons scale bug where they appeared scaled instead of the global icon size

1.0.0
- Initial Release
