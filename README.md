---
# CustomPosters
### **A mod for Lethal Company that replaces the default posters in the ship with custom posters. You can add your own images to create a personalized experience!**

---

### Table of Contents

1. [Quick Start](https://github.com/se3ya/CustomPosters/tree/main#quick-start)
2. [Features](https://github.com/se3ya/CustomPosters?tab=readme-ov-file#features)
3. [Adding Custom Posters](https://github.com/se3ya/CustomPosters?tab=readme-ov-file#adding-custom-posterscreating-custom-posters-mod)
4. [Configuration](https://github.com/se3ya/CustomPosters?tab=readme-ov-file#configuration)
5. [FAQ](https://github.com/se3ya/CustomPosters/blob/main/README.md#faq)
6. [Troubleshooting](https://github.com/se3ya/CustomPosters?tab=readme-ov-file#troubleshooting)
7. [License](https://github.com/se3ya/CustomPosters?tab=readme-ov-file#license)
8. [Credits](https://github.com/se3ya/CustomPosters?tab=readme-ov-file#credits)
   
---

## Quick Start

1. Download and install [GaleModManager](https://thunderstore.io/c/lethal-company/p/Kesomannen/GaleModManager/).
2. Download the latest version of **CustomPosters** from [Thunderstore](https://thunderstore.io/c/lethal-company/p/seechela/CustomPosters/).
3. Add your custom posters to the `posters` and `tips` folders as described in the [Adding Custom Posters](#adding-custom-posterscreating-custom-posters-mod) section.
4. Configure poster packs and probabilities in the `CustomPosters.cfg` file (optional).
5. Launch the game and enjoy your custom posters!

---

## Features

- Override the default posters in the ship with your own custom images.
- Support for multiple poster packs, with the ability to enable/disable poster packs or posters itself, change chances of the packs and posters via configuration.
- If any custom poster fails to load, the mod will disable that poster at all.
  - If you have more than 2 poster mods and first poster don't load, it will try to load second poster instead.
- Compatible with **ShipWindows**, **2 sToRy ShIp** and **WiderShipMod**.
- Optimized to prevent texture leaking.
- Tool called [**PosterCropperTool**](https://github.com/se3ya/PosterCropperTool) which allows to crop posters from **LethalPosters** mod to be compatible with **CustomPosters**.
  - **Sizes of the posters might be slightly be incorrect after cropping!**

---

## Adding Custom Posters
### Requirements
- CustomPosters
- Supported formats - `.png`, `.jpg`, `.jpeg`, `.bmp`.
### Steps
1. Create a folder structure for your custom poster pack in the `BepInEx/plugins` directory as shown below.
2. Place your poster images in the `posters` and `tips` folders, ensuring filenames match exactly (e.g., `Poster1.png`, `CustomTips.jpg`).

**Folder Structure**:
_<p><small>Poster image names must match the structure below.</small></p>_



   
       BepInEx/
        ├── plugins/
        │   └──── YourModName/
        │          └── CustomPosters/
        │                ├── posters/
        │                │    └── Poster1.png, Poster2.png, Poster3.png, Poster4.png, Poster5.png
        │                └── tips/
        │                     └── CustomTips.png
        └── ...                  

### Recommended Poster Sizes *[ In pixels ]*
- Poster1 - 639 Width, 488 Height
- Poster2 - 730 Width, 490 Height
- Poster3 - 749 Width, 1054 Height
- Poster4 - 729 Width, 999 Height
- Poster5 - 552 Width, 769 Height
- CustomTips - 860 Width, 1219 Height
---

## Configuration
The mod automatically generates a configuration file (`CustomPosters.cfg`) in the `BepInEx/config` folder. You can use this file to customize the behavior of the mod.

### Configuration Options

- **Randomier mode**:
  - *PerPack (default)*: Selects one pack randomly for all posters.
  - *PerPoster*: Randomizes textures for each poster from all enabled packs.
- **Per session**:
  - *False (default)*: Randomizes posters only when the lobby reloads.
  - *True*: Randomized posters only when restarting the game.
- **Enable/Disable Packs and Posters**:
  - Each poster pack has an `Enabled` setting for poster pack and posters. Set to `false` to disable a pack or a poster.
- **Per-Pack Chance**:
  - Assign a `Chance` value (0–100) for each pack. If any pack has a `Chance > 0`, weighted random selection is used.
  - A `Chance = 0` excludes the pack from selection, reverting to equal probability among enabled packs with non-zero chances.
- **Per-Poster Chance**:
  - For each poster in a pack, set a probability (0–100). If any poster has a `Chance > 0`, weighted selection applies.
  - Having 2 and more poster packs and one of the poster pack has (e.g. Poster2) `Chance = 70` and second poster pack with same poster has `Chance = 0` means that second poster pack poster is excluded from selection within that pack.
- **TextureCaching**:
  - *Enabled (Default)*: Stores textures in memory for faster access, reducing disk reads.
  - *Disabled*: Loads textures from disk each time, which may slightly increase load times (based on image size).

---

## FAQ

### **Q: Can I use multiple poster packs at the same time?**
Yes! The mod supports multiple poster packs. You can enable or disable packs in the configuration file.

### **Q: Is this mod compatible with other ship mods?**
Yes, the mod is compatible with **ShipWindows**, **2 sToRy ShIp**, and **WiderShipMod**. Poster positions are automatically adjusted based on the installed mods.

### **Q: Can I use custom sizes for posters?**
Yes, but for best results, use the recommended sizes listed in the [Adding Custom Posters](https://github.com/se3ya/CustomPosters?tab=readme-ov-file#recommended-poster-sizes--in-pixels-) section.

### **Q: What happens if a poster fails to load?**
The mod disables that poster and falls back to the vanilla poster or another valid poster from an enabled pack.

---

## Troubleshooting
### My Posters Aren't Showing Up
1. Ensure your images are named correctly (e.g., `Poster1.png`, `CustomTips.png`).
2. Check the `BepInEx/LogOutput.log` file for errors related to texture loading.
3. Make sure the posters and tips folders are in the correct location as shown in *[Adding Custom Posters](https://github.com/se3ya/CustomPosters?tab=readme-ov-file#adding-custom-posterscreating-custom-posters-mod)*.
4. Confirm images are in supported formats (`.png`, `.jpg`, `.jpeg`, `.bmp`) and not corrupted.

### Default Poster (Plane.001) is Still Visible
- This happens if the mod fails to load any custom poster pack. Check the config and make sure you have at least 1 poster pack enabled, check log file for errors and ensure your images are valid.

---

## Credits

- Developed by **[seeya](https://thunderstore.io/c/lethal-company/p/seechela/)**.
- Was inspired by LethalPosters mod.

---

## License
Distributed under the GPL v3 License. See LICENSE.txt for more information

---

### 💖 Support
If you enjoy my work, consider [supporting](https://www.buymeacoffee.com/see_ya) us. Donations are optional but greatly appreciated.

---
