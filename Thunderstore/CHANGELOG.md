# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [3.2.0] - 2025-10-15 - "Synchronization"
### Added
- Added video playback synchronization. Hosts video playback time is synced with all clients.
- Added posters synchronization. Host now selects the active poster pack or randomization seed and syncs it across all clients.
- Added toggleable *Networking* option to config.
- Added *LethalNetworkAPI* as dependency.

## [3.1.0] - 2025-10-12 - "Compatibility"
### Added
- Added BiggerShip compatibility.

### Fixed
- Fixed posters not being affected by lights and shadows.

## [3.0.0] - 2025-10-11 - "The Overhaul"
### Added
- Added .mp4 support.
  - Added a config option to enable or disable the audio of the poster *mp4* *[ default is false ]* 
  - Added a config option for each .mp4 poster to change the max volume distance of the video *[ default is 4 ]* 
  - Added a config option for each .mp4 poster to change the aspect ratio of the video *[ default is Stretch ]* 
  - Added a config option for each .mp4 poster to change the volume of the video *[ default is false ]* 

### Changed
- Replaced creating posters via code with AssetBundle.
- Config overhaul.
- Refactored and reorganized the entire codebase.

### Fixed
- Fixed poster positions with *2StoryShipMod*.
- Fixed poster packs disabled in a mod manager still showing up in the config.

## [2.0.0] - 2025-06-09 - "Major update"
### Added
- Added a setting to all posters in poster pack to disabled/enable them individually.
- Added a setting `PerSession`, which allows user to choose between randomizing posters on every lobby reload or on every game restart.
- Added `Chance` config options for `PerPack` and `PerPosters` randomization modes.
  - PerPack: if any pack has a `Chance > 0`, use weighted selection.
  - PerPoster: if poster has a `Chance > 0`, use weighted selection.
  - 0% chance means equal probability *[ standard behavior ]*.
- Added support for `.jpg`, `.jpeg`, and `.bmp`.
- Added texture caching
  - Stores loaded textures in memory, reloading only when packs change. Clear cache on mod reload or game exit.

### Changed
- Little more texture loading optimization.
- Improved configs.
- Improved and removed some logs.

### Removed
- Removed `ShipWindowsBeta` compatibility as it was merged into the main `ShipWindows` mod.

### Fixed
- Fixed vanilla posters not re-enabling if there were custom posters loaded.

## [1.4.0] - 2025-05-13 - "Improvements"
### Changed
- Improved compatibility with other mods.
- Changed recommended sizes in README.

## [1.3.5] - 2025-02-18 - "Minor update"
### Removed
- Removed 1 second delay before creating custom posters, as it was not fixing the intended issue.

## [1.3.4] - 2025-02-18 "Compatibility"
### Added
- Temporarily added `ShipWindowsBeta` compatibility.

## [1.3.3] - 2025-02-15 "Optimization"
### Changed
- Optimized custom poster creation, the game no longer freezes when loading posters.
- Removed unused code.

### Fixed
- Fixed a texture memory leak.
- Added error handling for more robustness.

## [1.3.2] - 2025- "Release"
### Added
- Initial release!
- ShipWindows compatibility!
- WiderShipMod compatibility!
- 2 Story Ship compatibility!
