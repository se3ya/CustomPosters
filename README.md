---
# CustomPosters
### **A mod for Lethal Company that replaces the default posters in the ship with custom posters. You can add your own images to create a personalized experience!**

---

### Table of Contents

1. [Features](https://github.com/se3ya/CustomPosters?tab=readme-ov-file#features)
2. [Adding Custom Posters/Creating Custom Posters Mod](https://github.com/se3ya/CustomPosters?tab=readme-ov-file#adding-custom-posterscreating-custom-posters-mod)
3. [Configuration](https://github.com/se3ya/CustomPosters?tab=readme-ov-file#configuration)
4. [Troubleshooting](https://github.com/se3ya/CustomPosters?tab=readme-ov-file#troubleshooting)
5. [License](https://github.com/se3ya/CustomPosters?tab=readme-ov-file#license)
6. [Credits](https://github.com/se3ya/CustomPosters?tab=readme-ov-file#credits)
   
---

## Features

- Override the default posters in the ship with your own custom images.
- Simply drop your images into the `posters` and `tips` folders.
- Adding multiple poster mods that are compatible with this mod will __work__, as said in *Randomization Options*.
- If any custom poster fails to load, the mod will disable that poster at all.
  - If you have more than 2 poster mods and first poster don't load, it will try to load second poster instead.
- Randomization Options:
  - *Poster Randomizer*: Randomly select one pack for all posters or mix textures from multiple packs.
- Compatible with **ShipWindows** and **WiderShipMod**.
- Incompatibility with **2 sToRy ShIp**. *[ planned ]*

---

## Adding Custom Posters/Creating Custom Posters Mod
### Requirements
- CustomPosters
- Supported formats - .png
- Lethal Company (obviously).
### Steps
This is how your mod folder structure should look like to work with CustomPosters:
_<p><small>Names of the poster image should be exactly like shown in structure</small></p>_



   
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
- Poster1 - 341 Width, 559 Height
- Poster2 - 285 Width, 559 Height
- Poster3 - 274 Width, 243 Height
- Poster4 - 372 Width, 672 Height
- Poster5 - 411 Width, 364 Height
- CustomTips - 1024 Width, 1020 Height
---

## Configuration
The mod automatically generates a configuration file (`CustomPosters.cfg`) in the `BepInEx/config` folder. You can use this file to customize the behavior of the mod

### Configuration Options

**PosterRandomizer**:
- *Enabled (Default)*: Randomly select one pack and use it for all posters.
- *Disabled*: Randomly select a pack for each poster individually.
  
**Enable/Disable Packs**:
- Each pack has an `Enabled` setting in the configuration file. Set it to true or false to enable or disable the pack.

---

## Troubleshooting
### My Posters Aren't Showing Up
1. Ensure your images are named correctly (e.g., `Poster1.png`, `CustomTips.png`).
2. Check the `BepInEx/LogOutput.log` file for errors related to texture loading.
3. Make sure the posters and tips folders are in the correct location as shown in *[Adding Custom Posters/Creating Custom Posters Mod](https://github.com/se3ya/CustomPosters?tab=readme-ov-file#adding-custom-posterscreating-custom-posters-mod)*.

### The Default Poster (Plane.001) is Still Visible
- This happens if the mod fails to load any custom poster pack. Check the config and make sure you have at least 1 poster pack enabled, check log file for errors and ensure your images are valid.

---

## License
Distributed under the GPL v3 License. See LICENSE.txt for more information

---

## Credits

- Developed by **[seeya](https://thunderstore.io/c/lethal-company/p/seechela/)**.
- Was inspired by LethalPosters mod.

---
### 💖 Support
If you enjoy my work, consider [supporting](https://www.buymeacoffee.com/see_ya) us. Donations are optional but greatly appreciated.

---
