# CustomPosters *v2.1.0*, *"Video Support"*

- Added .mp4 support.
  - Added a config option to enable or disable the audio of the poster *mp4* *[ default is false ]*.
  - Added a config option for each .mp4 poster to change the volume of the video *[ default is false ]*.
- Poster caching is false by default.
Todo:
- Improve poster disabling system.
- Spawn poster planes even faster:
  - Use asset bundles and other improvements.

# CustomPosters *v2.0.0*, *"Major update"*

- Removed ShipWindowsBeta compatibility as ShipWindowsBeta was pushed to ShipWindows.
- Fixed vanilla posters not re-enabling if there were no posters loaded.
- Added Texture caching
  - Stores loaded textures in memory, reloading only when packs change. Clear cache on mod reload or game exit. (eh, uh, yeah)
- Little more texture loading optimization.
- Added support for:
  - .jpg
  - .jpeg
  - .bmp
- Improved configs.
- Added a setting to all posters in poster pack to disabled/enable poster.
- Added a setting *PerSession* which allows user to choose from randomizing posters on every lobby reload or game restart.
- Added chances config for PerPack and PerPosters
  - PerPack:
    - If any pack has a Chance > 0, use weighted selection.
  - PerPoster:
    - If poster (e.g., Poster1.jpg) has a Chance > 0, use weighted selection.
  - 0% chance means equal probability *[ standard behavior ]*.
- Improved and removed some logs.

# CustomPosters *v1.4.0*, *"Improvements"*

- Improved other mods compatibility.
- Changed recommended sizes in pixels for each poster in README.

# CustomPosters *v1.3.5*, *"huh?"*

- Removed 1 second delay before creating custom posters.
  - Was meant to fix something but it didn't do anything...

# CustomPosters *v1.3.4*, *"Compatibility"*

- Temporarily added ShipWindowsBeta compatibility.

# CustomPosters *v1.3.3*, *"Optimization"*

- Fixed texture leaking.
- Optimized CustomPosters creation.
  - The game no longer freezes when loading posters.
- Removed unused code.
- Added error handling for robustness.

# CustomPosters *v1.3.2*, *"Positions"*

- Improved poster positions.

# CustomPosters *v1.3.1*, *"Soft Dependency"*

- Made compatibility mods load first and then CustomPosters so it can detect them.

# CustomPosters *v1.3.0*, *"Compatibility"*

- 2 Story Ship compatibility!

# CustomPosters *v1.2.0*, *"Compatibility"*

- WiderShipMod compatibility!

# CustomPosters *v1.1.0*, *"Compatibility"*

- ShipWindows compatibility!
- Handling improvements.

# CustomPosters *v1.0.0*, *"Release"*

- Release!
