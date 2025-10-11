---
# CustomPosters
### **A mod for Lethal Company that replaces the vanilla posters in the ship with custom posters added by user.**

---

## Features

- Overrides the vanilla posters in the ship with your own images.
- Supports multiple poster packs. 
- Ability to enable/disable packs and posters, change chances of the packs and posters, change volume, max distance and aspect ratio of `.mp4` posters via config file.
- Compatible with **ShipWindows**, **2 sToRy ShIp** and **WiderShipMod**.
- Optimized to prevent texture leaking.
- [**PosterCropperTool**](https://github.com/se3ya/PosterCropperTool) allows to crop posters that are made for **LethalPosters** so they can be compatible with **CustomPosters**.
  - *Sizes of the posters will be slightly incorrect after cropping!*

---

## Adding Custom Posters
### Supported formats
- `.png`, `.jpg`, `.jpeg`, `.bmp`, `.mp4`.
### Steps
1. Create a folder structure for your custom poster pack in the `BepInEx/plugins` directory as shown below.
2. Place your poster images in the `posters` and `tips` folders, ensuring filenames match exactly - *`Poster1.png`, `Poster2.mp4`, `Poster3.bmp`, `Poster4.jpeg`, `Poster5.png`* and *`CustomTips.jpg`*.

**Folder Structure**:
_<p><small>Poster image names must match the structure below.</small></p>_



   
       BepInEx/
        ├── plugins/
        │   └──── YourPosterPackModName/
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

## Configuration Options

- **Randomier mode**:
  - *PerPack (default)*: Selects one pack randomly for all posters.
  - *PerPoster*: Randomizes textures for each poster from all enabled packs.
- **Per session**:
  - *False (default)*: Randomizes posters only when the lobby reloads.
  - *True*: Randomized posters only when restarting the game.
- **Enable/Disable Packs and Posters**:
  - Each poster pack has an `Enabled` setting for poster pack and posters. Set to `false` to disable a pack or a poster.
- **Global Chance**:
  - Assign a `Global Chance` value (0–100) for each pack. If any pack has a `Global Chance > 0`, weighted random selection is used.
  - A `Chance = 0` excludes the pack from selection, reverting to equal probability among enabled packs with non-zero chances.
- **Per-Poster Chance**:
  - For each poster in a pack, set a probability (0–100). If any poster has a `Chance > 0`, weighted selection applies.
  - Having 2 and more poster packs and one of the poster pack has (e.g. Poster2) `Chance = 70` and second poster pack with same poster has `Chance = 0` means that second poster pack poster is excluded from selection within that pack.
- **Volume**:
  - Configure volume of `.mp4` posters.
- **Max distance**:
  - Configure maximum audio distance of `.mp4` posters.
- **Aspect ratio**:
  - Choose aspect ratio of `.mp4` posters *[ Stretch 'X', FitInside, FitOutside, NoScaling ]*.
- **TextureCaching**:
  - *Enabled*: Stores textures in memory for faster access, reducing disk reads.
  - *Disabled (Default)*: Loads textures from disk each time, which may slightly increase load times (based on image size).

---

## FAQ

### **Q: Can I use multiple poster packs at the same time?**
Yes! The mod supports multiple poster packs. You can configure poster packs in the config.

### **Q: Is this mod compatible with other ship mods?**
Yes, the mod is compatible with **ShipWindows**, **2 sToRy ShIp**, and **WiderShipMod**. Poster positions are automatically adjusted based on the installed mods and configs.

### **Q: Can I use custom sizes for posters?**
Yes, but for best results, use the recommended sizes listed in the [Adding Custom Posters](https://github.com/se3ya/CustomPosters?tab=readme-ov-file#recommended-poster-sizes--in-pixels-) section.

### **Q: What happens if a poster fails to load?**
If a specific poster file (like a .png or .mp4) can't be loaded, the mod will log an error in the game's console and that poster simply won't appear.
In PerPack mode, if a file from the chosen pack fails, its spot will be empty.
In PerPoster mode, the mod will just pick another working poster from the available pool.

---

## Troubleshooting
### My Posters Aren't Showing Up
1. Ensure your images are named correctly (e.g., `Poster1.png`, `CustomTips.png`).
2. Check the `BepInEx/LogOutput.log` file for errors related to texture loading.
3. Make sure the posters and tips folders are in the correct location as shown in *[Adding Custom Posters](https://github.com/se3ya/CustomPosters?tab=readme-ov-file#adding-custom-posterscreating-custom-posters-mod)*.
4. Confirm images are in supported formats (`.png`, `.jpg`, `.jpeg`, `.bmp`, `.mp4`) and not corrupted.

### Default Poster (Plane.001) is Still Visible
- This happens if the mod fails to load any custom poster pack. Check the config and make sure you have at least 1 poster pack enabled.

---

## Credits

- Developed by **[seeya](https://thunderstore.io/c/lethal-company/p/seechela/)**.
- Was inspired by LethalPosters mod.

---

## License
Distributed under the GPL v3 License.

---

### 💖 Support
If you enjoy my work, consider [supporting](https://www.buymeacoffee.com/see_ya) me. Donations are optional but greatly appreciated.

---
