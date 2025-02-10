# MoreBrutalLethalCompanyPlus *v5.1.0*, *"???"*

## 🔧 FIXES

- 

## ✨ NOTABLE CHANGES

- 

### 🛠️ Overall Added Mods:

- 

## 🗑️ REMOVALS

### 🚪 Removed Interiors:

- 

### 🌑 Removed Moons:

- 

### 🚫 Overall Removed Mods:

- 

## 📦 MOD UPDATES
*(Old version → New version)*

- Castellum Carnis: 1.0.3 → 1.0.4


# MoreBrutalLethalCompanyPlus *v5.0.1*, *"Fixes"*

## 🔧 FIXES

- Forgot to update some mods in manifest.
- Forgot to remove CoilHeadMod from manifest.
- Forgot to change LC FastStartup from Lan > Online mode.
- sowwy D: 

## 📦 MOD UPDATES
*(Old version → New version)*

- FacilityMeltdown: 2.7.1 → 2.6.20
- PathfindingLagFix: 1.6.0 → 2.1.0
- Aquatis: 1.6.0 → 2.1.0

# MoreBrutalLethalCompanyPlus *v5.0.0*
### *"Optimization, Balance, Fixes, Features"*

## 🔧 FIXES

- Terminal & Scanning:
  - Fixed terminal scan command including dead bodies in scrap counts.
  - Fixed scanning collected story logs and exploded landmines.
  - Fixed shotgun UI showing on everyone’s screen.
  - Fixed turning on the ammo indicator breaking the shotgun safety.
  - Fixed upper-case commands not being recognized.
  - Fixed shop item price de-sync.
- Enemies & AI:
  - Fixed maneater clicking its mandibles when freshly spawning.
  - Fixed giants not stomping when ignited.
  - Fixed collision bugs with cruisers and players.
  - Fixed a strange bug where players regenerated to 100% health after taking non-lethal damage.
  - Fixed NeedyCats conflicting with TerminalFormatter.
  - Fixed a Vanilla bug where Masked go invisible when mimicking a player who dies.
- Weather & Environment:
  - Fixed uncertain weather generation on Gordion.
  - Fixed floodlight brightness not reverting after Blackout weather.
  - Fixed dropship lights turning off during Blackout weather.
  - Fixed stormy weather re-targeting items already inside the ship.
- Performance & Sounds:
  - Fixed store node lagging the game.
  - Fixed music not playing more than once per session.
  - Fixed cullfactory sometimes not unhiding cruiser items.
  - Fixed items emitting light and effects while in inventory but not held.
  - Fixed critical audio from **Jetpack Warning** not playing.
  - Fixed mineshaft doors muffling their own audio as if they were behind a wall.
- Cruiser & Ship:
  - Fixed cruiser seat bugs:
    - Players can now use the scanner while seated.
    - Players are no longer immune to death pits while seated.
  - Fixed cruiser magnet lever being off initially if the ship lever was on when spawned.
  - Fixed terminal model not being visible on the ship’s internal security camera.
  - Fixed soft-locks after pulling the lever.
- Other:
  - Fixed Night Vision not working as expected due to mistake between initial and incremental values.
  - Fixed **DoorBreach** networking.
  - Corpse fixes:
    - Meteors no longer spawn Tragedy-masked bodies.
    - Fixed bee/bunny suit parts being visible on webbed bodies.
  - And more...

## ✨ NOTABLE CHANGES

- General Improvements:
  - Major optimization improvements.
  - Hot-Reload feature:
    - A new feature that dynamically loads and unloads custom moon AssetBundles as needed.
	- This significantly reduces RAM usage.
  - Enabled Distance Culling.
    - Objects that are 400 units far from the player will be culled.
	  - Small performance boost.
  - Increased games resolution from 540p > 720p.
    - Shouldn't really decrease games performance.
  - Increased the default chance for stingers to play.
  - Phones:
    - New ship upgrade called *Personal Phones*:
      - After buying it, everyone will get a phone.
	  - Player can take out his phone by clicking keybind **[ Y ]**
	  - Player needs to hold keybind **[ Z ]** and click on the number on the phone and drag the number to the stopper to dial.
	  - Phones ringtone can be customized in starting menu.
	  - Phones will not take up an inventory slot, and do not require battery.
      - Phones are 'immersive' to use and may not be reliable under pressure.
      - Phone calls will transmit all noises around the player you are talking to.
   	  - Switch your phone to vibrate if you don't want monsters to hear how you are talking or switch it to the silent to ignore all your friends calls.
      - Player can receive calls from local fauna.
	    - Hoarding Bugs have a small chance using their phones and calling a player
	    - Maskeds have a chance that they will use their phones and call a player and copy the voice of the player they're mimicking.
	- New ship item called *Phonebook Clipboard*:
	  - When loading a new save, *Phonebook Clipboard* will be next to the lever.
	  - *Phonebook Clipboard* has all player contacts.
	  - If player somehow lost it or sold it, it can be bought in the shop.
	- New ship upgrade called *Switchboard*:
	  - Call your friends from the safety of the ship using The Company's patented new "Non-Rotary" dialing method!
	  - Wear a headset to communicate hands-free.
	  - Tired of the friend you are talking to? Transfer it to someone else.
	  - *Switchboard* has it's own contact list.
  - Added 9 rare funny scrap, one of them are weapon. 4 rare vanilla weapon scrap and 31 basic vanilla themed scrap.
  - Added improved flashlight version called *Elite Flashlight*.
    - It's wider and has greater light distance and has a longer battery life than the Pro's flashlight.
  - Increased maximum number of simultaneously audible sounds from 256 > 512.
  - Added new menu music.
  - Reduced the noise/"graininess" visible in fog, and improves the definition of light shapes.
  - Removed Christmas scrap, suits and hazard, also reduced snowfall weather chance.
  - Bunkbed will be now stored at the start.
  - Added some alternate scrap skins:
    - Gift box *[ purple & blue ]*
    - Candy *[ red w/ yellow marbling ]*
    - Perfume bottle *[ red, blue, black; +5 bottle shapes ]*
    - Pill bottle *[ red-orange ]*
    - Mug *[ blue steel ]*
    - Control pad *[ green ]*
    - Plastic fish *[ yellow & red ]*
  - Players can now see how much ammo they hold in the shotgun.
  - If player will speak loudly enough while spectating, the person you are watching will hear a spooky ghost noise.
  - After pulling ship lever to land on the moon, tip HUD message will show up with random *tips*.
  - Balanced scrap spawn weights on each moon.
    - Some dungeons will have special scrap with increased or rare weight.
  - Balanced enemy spawn weights on each moon.
  - Ship doors can no longer kill enemies.
  - The game will now begin waiting for the dungeon to spawn sooner.
    - This should hopefully help reduce potential de-syncs.
  - Changed Wheelbarrow and Shopping Cart restrictions from *ItemCount* to *TotalWeight*:
    - 100 weight a Wheelbarrow & Shopping Cart can carry in items before it is considered full.
  - Replaced the Action Ambience audio with the variant from v67, the Halloween patch.
  - Increased range and intensity scan vision in the dark.
  - Optimized pathfinding.
  - Improved under-snow detection and blocked bees from spawning during snowy weather.
  - Interiors will have custom scrap assigned by them theme with a higher spawn chance.
  - Supercharger:
    - Increased supercharge chance from 10% > 25%.
	- Increased chance of explosion to 50%.
  - Disabled Mimic fire exit several possible imperfections.
  - Increased timer of how often does it try to become a zombie.
  - Ghost can't anymore call a drop ship.
  - Reduced the scale scrap value max value from 1.5 > 1.3.
    - Scrap value when playing with less than 4 people in the lobby maximum was reduced.
  - Reduced lag spikes:
    - When a new Barber spawns.
    - When Barbers accelerates hourly.
  - Re-implementation of *WindowVariants*:
    - Added several new varieties of windows for modded moons.
    - Windows no longer turn solid black when powered off.
  - Fall damage will now scale with carried weight.
- Cosmetics & Visuals:
  - Added 4 cosmetics to maneaters:
    - Silly Outfit, Fancy Outfit, Evil Horns, Pacifier.
  - Added charred skin for giants burning to death.
  - Replaced vanilla dropship with Starship.
  - Added a map to the cruiser's control center.
  - When breaking a door it will just open instead of destroying.
  - Forest moons now have gray boulders instead of sandstone.
  - Added some funny suits.
- Terminal & UI:
  - Modified the descriptions of vanilla moons and added a video preview to them.
  - Moons page was replaced with interactive catalogue menu.
  - Store page was replaced with interactive catalogue menu.
  - Enabled Minimap command.
  - Added bestiary and storage nodes.
  - Added decorations to the store page.
  - Spectator voting tip notification has been hidden.
  - Reworked performance report and fines animations.
  - Disabled restart command.
  - Terminal screen will be disabled if player is not in the ship.
  - Scan command will won't show the exact total value.
- Weather:
  - Slightly reduced Snowfall weather happening chance.
  - Weather scrap multipliers were reworked.
  - Weathers no longer multiply scrap values, only amounts.
  - Increased scrap amount multipliers for Blackout, Cloudy, Eclipsed, Flooded, Foggy, Rainy, Stormy, and combined weathers.
  - Added new combined weather types:
    - Stormy + Rainy + Flooded + Eclipsed.
    - Stormy + Rainy + Flooded.
    - Stormy + Rainy + Eclipsed.
    - Stormy + Flooded.
- Enemies & AI:
  - Circuit bees now defend their hive from anything too close and can set giants on fire.
  - Hygrodere damages creatures on contact (except Maneater) and consumes enemy corpses.
  - Removed enemy Kidnapper Fox:
    - Causing performance issues.
  - Earth Leviathan now targets creatures near it and can eat players inside the ship.

### 🛠️ Systems Overhaul:

- Reworked all systems and their moons, introducing a refined structure:
  - Orion: (Starting System):
    - Includes 5 beginner-friendly moons with Wasteland, Rocky, and Valley biomes.
  - Andromeda
    - Includes 6 moons with Canyon, Ocean and Valley biomes.
	- Price > 200.
  - Harmonia
    - Includes 6 moons with a with of Tundra, Rocky and Canyon biomes.
	- Price > 500.
  - Pegasus
    - Includes 7 moons with on Company, Ocean and Canyon biomes.
	- Price > 1000.
	- Each moon price > 50.
  - Scorpius
    - Includes 7 moons with Volcanic, Rocky and Argon biomes.
	- Price > 1800.
	- Each moon price > 100.
  - Gemini
    - Includes 7 moons with Ocean, Canyon and Tundra biomes.
	- Price > 3000.
	- Each moon price > 150.
  - Taurus
    - Includes 8 moons with on Wasteland, Argon and Valley biomes.
	- Price > 4500.
	- Each moon price > 200.
  - Aquarius
    - Includes 7 moons with Volcanic, Ocean and Company biomes.
	- Price > 6000.
	- Each moon price > 300.
  - Cygnus
    - Includes 9 moons with Argon, Canyon, Rocky and Tundra biomes.
	- Price > 8000.
	- Each moon price > 400.
  - CORRUPTION DETECTED
    - Includes 8 moons with all types of biomes.
	- Price > 9999.
	- Each moon price > 500.

### 📈 Quota Overhaul:

- Enabled partial rollover to reward players for exceeding quotas while preventing too fast progression.
- Reduced deadline from 5 days > 4 days.
- Reduced starting quota from 400 > 300.
- Increased base increase from 200 > 250.
- Increased curve sharpness from 2 > 4.

### 📜 Contracts:

- Added *Contracts* interactive terminal page:
  - Defusal Contract:
    - Look around the facility for a ticking bomb. Defuse it before the timer runs out or anyone near it might meet a gruesome fate.
    - A serial is shown on it. You will need this for the command `lookup <serial>` in the terminal which it will show the right sequence of wires you need to cut to defuse it.
  - Exterminator Contract:
    - A nest of hoarding bugs has been detected on this facility. Clear them out and destroy the nest.
	  - A loot object will spawn after destroying it.
  - Extraction Contract:
    - A fellow employee has been reported missing in the facility. You will need to find him and escort them back to the ship for safety (and disciplinary action by The Company).

### 💀 Enemy Improvements:

- Circuit bees defend their hive from anything *(excluding Manticoil and Eyeless Dogs)* that gets too close. They're now deadly to every living being.
  - Small chance to set giants on fire.
- Hygrodere cannot open doors.
- Earth Leviathan targets creatures that come near it. Ignores small creatures.
  - Earth Leviathan eat players inside ship leaving the moon.
- Forest Giant:
  - Stays on fire for a while after death.
  - Has a chance to extinguish itself.

### 🛡️ FairAI:

- Can be targeted by turrets:
  - Masked.
  - Butler.
  - Singed Maneater.
- Can be blown up on the mines:
  - Masked.
  - Hoarding bug.
  - Flowerman.
  - Butler.
  - Singed Maneater.

### 🌌 New Outside Objects:

- Crystals:
  - Embrion
  - Crystallum
  - Olympus
  - Seichi
- Ribcage:
  - Experimentation
  - March
  - Auralis
  - Flicker
  - Fremist
  - Frostfall
  - Collateral
  - Vertigo
  - EGypt
  - Ichor
  - Sector-0
  - Aquatis
  - Arcadia
  - Atlas Abyss
- Snowman:
  - Snowy moons.

### ⚖ Overall Rebalance:

- The knife will deal damage faster.
- Barbers will spawn in pairs, up to 8 total:
  - In mineshafts, they are limited to 1 total.
- Butler:
  - Butlers have an increased chance to spawn in manor interiors.
  - When triggering a Butler to attack by bumping into it (rare chance), it will no longer "berserk", unless the offending player is alone.
  - Butlers will deal damage slower, as long as they haven't been attacked before. In singleplayer, this setting will also increase their HP from 2 to 3.
  - Randomized knife price:
    - On average, it will be significantly more valuable than $35, the vanilla price.
- Coil-heads will begin "recharging" when they are stunned by stun grenades, radar boosters, or homemade flashbangs.
- Jester:
  - Player can walk through Jesters once they begin winding up.
  - Slightly increased the average time before a Jester winds, in multiplayer.
- Maneater:
  - Prevented the Maneater's ability to transform before it has been encountered by a player.
  - Eating scrap will permanently reduces the speed at which a Maneater transforms from crying.
  - Adult Maneaters will have far worse turning speed when turning.

### 🌕 Rebalanced Moons:

- Added new HDRI sky volumes to snowy moons.
- Adamance's layout has been edited to be more Cruiser friendly.
- Added custom interior lighting to Embrion and Titan.
- Added slightly purple tinted fog to Embrion, and colder interior lighting on Titan.
- Slightly increased starting moons enemy spawning speed.
- Slightly increased min and max scrap values in starting systems.
- Experimentation:
  - Filled the hole in the back of the alleyway with a fire exit.
- Embrion:
  - Added large boulders for more "stealth" gameplay.
  - Added new sunset HDRI sky volume to Embrion.
- March:
  - Added a rickety bridge. Stats are inbetween Adamance and Vow.
  - Has a water tower hinting at the back fire exit's location.
  - Left fire exit was moved to the right of the ship to create a more evenly distanced triangle of fire exits around the moon. It's also a slightly shorter walk.
- Artifice: 
  - Allowed access to the 4th warehouse.
  - Adjusted the dropship position.
  - Added in more nodes for AI pathfinding.
- Assurance:
  - Modified the Main Entrance buidling, allowing access to the pipe to the Fire Exit through a bit of parkour.
- Gordion:
  - Added more shipments, with some of them being open.
  - Limited the area with walls.
    - There is a chance 50% that it will choose vanilla Gordion or Improved.
- Dine:
  - Replaced Rainy with Foggy.
  - Has two fire exits, one requiring an extension ladder or a jetpack.
  - Terrain was slightly edited to allow for an alternate route from the right fire exit.
  - Has guide light poles that go towards main, and the fence in the middle of the map was moved back so the moon is more cruiser friendly.

### 🚪 Entrance Control System:

- **Entrance Control System:**
  - Players must **enter through the main entrance** and **exit through fire exits** when active.
  - Activation chance - 15%.
- **Dynamic Mode:**
  - Activates on certain days when Entrance Controls are active.
  - Players can **enter using any door** but must **exit through the opposite type** of door used for entry.
  - Activation chance - 60%.
- **Corruption Mode:**
  - Reverses default behavior. Players **enter through fire exits** and **exit through the main entrance**.
  - Activation chance - 5%.
- Last player alive is always able to leave using any door.
- Entrance controls are disabled for the first **10 days** of new saves, allowing time to gather keys.
- Entrance controls will turn off automatically if:
  - The moon is flooded.
  - The **Apparatus** has been taken.
  - The facility's power is turned off.

### 🛒 Shop Changes:

- Reduced *Shovel* price from 40 > 30 credits.
- Reduced *Zap Gun* price from 400 > 300 credits.
- Reduced *Extension Ladder* price from 115 > 100 credits.
- Reduced *Cruiser* price from 400 > 370 credits.
- Added *Firework Rocket* item.
  - Price - 25 credits.
- Added *Explosive* *[ M67 Grenade ]* item.
  - Price - 180 credits.
- Added *Switchboard* upgrade.
  - Price - 450 credits.
- Added *Personal Phones* item.
  - Price - 200 credits.
- Added *Elite Flashlight* item.
  - Price - 90 credits.
- Removed *Handlamp* from the store.

### ⚡ Zap Gun Changes:

- Zap Gun can set Forest Giants on fire.
- Zap Gun will evaporate Hydrogare.

### 🔫 Shotgun Changes:

- If the shotgun slips out of your hands, it will not fire.
- Reduced drop chance from 10% > 7%.

### 🩸 Castellum Carnis Interior Changes:

- Added a unique ground light prop to the "void" room.
- Added veins to the fire exit metal walls.
- Fixed the normal map on some of the veins.
- Fixed the main entrance and fire exit walls not having collision.
- The "two handed" room skylight now changes with the time of day and weather.

### 🏮 Siabudabu Moon Changes:

- Reduced the verticies of the terrain to be more performance friendly.
- Fixed the NavMesh issues and removed NavMesh in the non-playable areas.

### 🌀 Vacuity Moon Changes:

- There will be only Desert season instead of Winter or Desert choosed randomly. 

### 🌟 Starlancer Moons Changes:

- Performance improvements on Auralis and StarlancerZero, small performance improvement on Triskelion.

### 🕳️ Atlas Abyss Moon Changes:

- Fixed Navmesh.
- Removed AI jumping across pillars.
- Fixed spawns being stuck under pallets.
- Fixed unintended hiding spot.
- Added more spots for enemies to climb on the ship.
- Added moss spawns.

### 👑 Olympus Moon Changes:

- Increased the quality for some terrain textures.
- Removed free additional loot at the easter egg place.
- Added *ReverbTriggers* to Main Hall.

### 🏚️ Storehouse Interior Changes:

- Added Pedro *(whatever that means.)*
- Reduced silly poster spawn chance.
- Removed long shutter hallway tile.
- Added chance for elevator shaft room to be a dead end.
- Fixed incorrect stair and upper level audio for server room.
- Finally fixed the elevator falling.

### 🗻 Seichi Moon Changes:

- Removed Christmas stuff.
- Temporarily removed Shisha.

## 🌌 ADDITIONS

### 🏠 New Interiors:

- Slaughterhouse
- Cabin
- *[CORRUPTION DETECTED]*
- Mental Hospital
- Rubber Rooms
- Atlantean Citadel Aquatic
- Art Gallery

### 🌙 Added Moons:

- Added new moon: Landslide.
  - Which system: Gemini.
  - Risk Level: B+.
  
- Added new moon: Brutality.
  - Which system: Eridanus.
  - Risk Level: C+.
  
- Added new moon: Frostfall.
  - Which system: Cygnus.
  - Risk Level: D. 
  
- Added new moon: Sanguine.
  - Which system: Andromeda.
  - Risk Level: C++.  
  
- Added new moon: EchoReach.
  - Which system: Lyra.
  - Risk Level: C. 
  
- Added new moon: Retinue.
  - Which system: Cepheus.
  - Risk Level: B.
    
- Added new moon: Aquar.
  - Which system: Eridanus.
  - Risk Level: C+.
      
- Added new moon: Scallg.
  - Which system: Orion.
  - Risk Level: A.
        
- Added new moon: Quasara.
  - Which system: Centaurus.
  - Risk Level: A+.
          
- Added new moon: Valunarion.
  - Which system: Draco.
  - Risk Level: S.
            
- Added new moon: Ferangdalion.
  - Which system: Centaurus.
  - Risk Level: A+.
            
- Added new moon: Aethon.
  - Which system: Orion.
  - Risk Level: D.
            
- Added new moon: Prominance.
  - Which system: CORRUPTION DETECTED.
  - Risk Level: [ CORRUPTION DETECTED ].
  
- Added new moon: Thallasic.
  - Which system: Aquarius.
  - Risk Level: A++.
  
- Added new moon: Reign.
  - Which system: Harmonia.
  - Risk Level: C+.
  
- Added new moon: Symbiosis.
  - Which system: Pegasus.
  - Risk Level: C++.
  
- Added new moon: Veld.
  - Which system: CORRUPTION DETECTED.
  - Risk Level: [ CORRUPTION DETECTED ].
  
- Added new moon: Desperation.
  - Which system: Cygnus.
  - Risk Level: S.

### 🪓 Added Weapons:

- Axe:
  - A classic metal axe with a wooden frame, can be found in the facility with with the value of 20 or more.
- Metal Pipe:
  - Solid, heavy and rare, can be found in the facility with with the value of 100 or more.
- Crowbar:
  - Slim, generic but iconic, can be found in the facility with the value of 60 or more.
- M67 Grenade:
  - Tiny, deadly, and expensive, a buyable item with the price of 180.

### 🪙 Added Scrap:

- Smoke Detector
- LethalBeacon
- **Premium Scrap**:
  - Chocobo
  - The King
  - Crystal
  - Puppy Shark
  - Rupee
  - Ea nasir
  - Spoon
  - Stick
  - Square Steel
- **WiseWilderness**:
  - Butler's Broom
  - Defective Headlight
  - Decapitated Head
  - Bloody Coil
  - Wooden Shako
- **fiufkis Scrap**:
  - Book
  - Bottle
  - Cardboard Box
  - Car Tire
  - Chair
  - Despair
  - Picture
  - Pillow
  - Dr.Lethal
  - Magnet
  - Vent Cover
  - Meteorite
  - Cake
  - Candle
  - Moon Dew
  - Keyboard
  - Skull
  - Pumpkin
  - Cartridge
  - Desk Lamp
  - Ship Speaker
  - Stool
- **SlaughterhouseScraps**:
  - Barbed wire.
  - Flamingo floatie.
  - Meat scale.
  - Pig painting.
  - Ritual mask.

### 👾 Added Enemies:

- TheThing
  - Creature that lurks in the darkness.
- Smallfry
  - Spawns outside the facility.

### 🛠️ Overall Added Mods:

- TerraMesh
- Smoke Detector
- Do We Need To Go Deeper
- RebalancedMoonBeta
- Piggys Reanimation
- BabyManeater
- SpookyCompany
- fiufkis Scrap
- WiseWilderness
- WiseWeapons
- Slaughterhouse
- SlaughterhouseScraps
- SmallFryEnemy
- CruiserMap
- Sanguine
- EchoReach
- FairAI
- AutoMessage
- HideNotifications
- MapImprovements
- TheThing
- 17 Frostfall
- WeatherTweaks
- FunnySuit
- Moon Day Speed Multiplier Patcher
- PoweredMoons
- Aquar
- Natural Selection
- NaturalSelectionLib
- ItemWeights
- HalloweenAction
- StarshipDeliveryMod
- Beanies Moons
- Item Lights And Effects Fix
- Orbits
- Matty Fixes
- PathfindingLib
- ScaledFallDamage
- UpturnedVariety
- ZortMenuTheme
- PremiumScraps
- EliteFlashlight
- LethalPhones
- LethalBeacon
- MaskedFixes
  - MaskedInvisFix
- MaskFixes
- ButteRyBalance

## 🗑️ REMOVALS

### 🚪 Removed Interiors:

- Gray Apartments
- USC Vortex
- Tomb
- Xen Interior

### 🌑 Removed Moons:

- altMoons:
  - Phaedra
  - Dirge
  - Pelagia
- Infernis
- Gratar
- Asteroid-13
- Oldred
- Polarus
- Cosmocos
- Bomenoren
- Celest

### 🚫 Overall Removed Mods:

- LessPopups
- USC Vortex
- USC Vortex Interiors
- BetaWeatherTweaksBeta
- Horrifying Bracken
- JesterScreamRevamped
- CustomSounds
- LCSoundTool
- PintoBoy
- CoilHeadMod
- TurretSettings
- SnowyHolidayDropship
- altMoons
- Tomb
- Tauralis *[ in PoweredMoons pack ]*
- Gray Apartments Interior
- Xen Interior
- SnowPlaygrounds
- SCP-966
- SCP-500
- ChristmasSuits
- YesFox
- KidnapperFoxSettings
- SnowyWeeds
- PriceTweaker
- Motion Tracker V3
- CreditCalculator
- TerminalApi
- UsualScrap
- ScanRecolor
- Wheelbarrow
- Infernis
- Gratar
- Asteroid-13
- Polarus
- Oldred
- Cosmocos
- ChristmasSuits
- 97 Bomenoren
- ScarletDevilMansion
- TonightWeDine
- Celest
- SaveItemRotations
- Matty Fixes Experimental
- BunkbedRevive
- WeedKillerFixes
- CruiserTerminal
- CookieKnife

## 📦 MOD UPDATES
*(Old version → New version)*

- Castellum Carnis: 1.0.3 → 1.0.4
- WinterLodge: 1.0.6 → 1.0.7
- Generic Interiors: 1.6.0 → 1.6.3
- Tomb: 0.2.9 → 0.3.0
- Seichi: 0.9.11 → 0.9.14
- Generic Moons: 6.2.0 → 7.0.1
- MrovWeathers: 0.0.1 → 0.0.3
- LethalElementsBeta: 1.2.60 → 1.2.71
- TestAccountVariety: 1.37.0 → 1.38.0
- LoadstoneUnstableBeta: 0.1.19 → 0.1.22
- Useful Zap Gun: 0.3.2 → 0.4.2
- SlipperyShotgun: 1.2.0 → 1.3.0
- FacelessStalker: 1.1.9 → 1.2.0
- Better Shotgun Tooltip: 1.4.0 → 1.5.0
- EnemySoundFixes: 1.6.0 → 1.6.4
- UsualScrap: 1.7.6 → 1.7.7
- ScanRecolor: 1.1.2 → 1.1.5
- GoodItemScan: 1.12.0 → 1.13.0
- TestAccountCore: 1.13.1 → 1.14.0
- ButteryFixes: 1.10.13 → 1.11.0
- WeatherRegistry: 0.3.13 → 0.3.16
- LethalFixes: 1.2.4 → 1.2.5
- CullFactory: 1.6.2 → 1.7.0
- Atlas Abyss: 1.2.3 → 1.2.6
- NightOfTheLivingMimic: 1.0.3 → 1.0.16
- DoorBreach: 1.2.1 → 1.4.0
- MrovLib: 0.2.13 → 0.2.21
- TerminalFormatter: 0.2.23 → 0.2.28
- MalfunctioningDoors: 1.12.0 → 1.13.1
- StoreRotationConfig: 2.5.0 → 2.5.1
- Better Shotgun Tooltip: 1.5.0 → 1.5.2
- FinallyCorrectKeys: 1.3.0 → 1.5.0
- Tauralis: 1.0.6 → 1.0.7
- Chameleon: 1.4.2 → 2.1.1
- JetpackFixes: 1.5.0 → 1.5.2
- DistinctMoonVariety: 1.5.0 → 1.6.1
- CruiserTerminal: 1.1.1 → 1.1.2
- NeedyCats: 1.2.2 → 1.2.3
- DoorBreach: 1.3.0 → 1.3.1
- SpiderPositionFix: 1.2.0 → 1.2.1
- LethalLevelLoader: 1.3.13 → 1.4.8
- DungeonGenerationPlus: 1.3.3 → 1.3.4
- ScarletDevilMansion: 2.1.0 → 2.2.2
- StarlancerMoons: 2.4.0 → 3.1.0
- SnowyWeeds: 1.3.3 → 1.3.4
- Mirage: 1.15.5 → 1.17.0
- YesFox: 1.0.9 → 1.1.0
- HexiBetterShotgunFixed: 1.0.0 → 1.0.1
- OpenLib: 0.2.12 → 0.2.14
- AdditionalNetworking: 2.1.2 → 2.1.3
- ReservedItemSlotCore: 2.0.39 → 2.0.42
- TransmitPunctuation: 1.0.2 → 1.0.3
- HotbarPlus: 1.8.0 → 1.8.1
- Lategame Company Cruiser Upgrades: 1.1.2 → 1.1.3
- LobbyImprovements: 1.0.7 → 1.0.8
- Cruiser Additions: 1.4.2 → 1.4.3
- Lategame Upgrades: 3.11.0 → 3.11.4
- JLL: 1.8.0 → 1.9.0
- Celest: 1.1.4 → 1.1.5
- WesleyInteriors: 1.6.2 → 2.0.11
- Aquatis: 2.2.3 → 2.2.4
- Olympus: 1.0.5 → 1.0.6
- FixPluginTypesSerialization: 1.1.1 → 1.1.2
- LethalConfig: 1.4.3 → 1.4.5
- LethalPerformance: 0.5.0 → 0.5.1
- LethalPipeRemoval: 1.7.6 → 1.8.0
- BarberFixes: 1.2.2 → 1.3.0
- fiufkis Moons: 1.1.2 → 1.1.3
- CustomStoryLogs: 1.4.5 → 1.4.6
- PathfindingLagFix: 1.4.0 → 2.1.0
- Spectralis: 0.9.8 → 0.9.9
- DarmuhsTerminallStuff: 3.7.7 → 3.8.4
- Mirage: 1.17.0 → 1.18.0
- EGypt: 2.0.21 → 2.0.22
- BetterEXP: 2.5.2 → 2.6.0
- ReservedWalkieSlot: 2.0.6 → 2.0.7
- ReservedUtilitySlot: 1.0.6 → 1.0.7
- ReservedSprayPaintSlot: 1.1.2 → 1.1.3
- RevisitStingers: 1.2.1 → 1.2.2
- Cruiser Additions: 1.4.3 → 1.4.4
- Lethal Doors Fixed: 1.4.3 → 1.4.4

# MoreBrutalLethalCompanyPlus *v4.4.0*, *"Small Fixes, Big Impact"*

## 🔧 FIXES

- Fixed a compatibility issue with *FacilityMeltdown* where the scrap multiplier was applied twice.
- Fixed a bug with the pricing of hives.
- Fixed some soft-locks on certain custom moons/interiors.
- Fixed Clay Surgeons trick snipping players.
- Fixed an error that could occur when a mob explodes using Zap Gun.
- Fixed an issue where Artifice's global volume behaved inversely compared to Titan's.
- Fixed spider never reaching the floor position.
- Fixed spider not climbing the wall.
- Fixed particle lights emitted from items being invisible in the interior.
- Fixed ship fog.
- Fixes to doors:
  - The same random clip now plays on both sides of the entrance door.
  - Entrance door sounds now play over walkie-talkies when players enter/exit the building. (like mimics)
  - Mimics now play entrance door sounds on both sides when entering/exiting the building. (like players)
  - Fixed factory doors, locker doors, and breaker box having backwards open/closed sounds.
  - Fixed cabin doors on Rend and Adamance still using the steel door sounds.

## ✨ NOTABLE CHANGES

- Disabled Spooky mode in Toy Store interior.
- Old Birds won't spawn inside Grand Armory interior. anymore.
- The light behind entrance doors now adjusts dynamically based on your landing location, weather, and time of day.
- Optimized *Bunker Spider* behavior and performance for smoother gameplay.
- Balanced gambling machines.
- Reduced spawn frequency for the Christmas hazard.
- Performance improvements have been done on the Siabudabu moon.
- Removed *VileVendingMachine* enemy.
- Corrected the most of the UI elements to display the keybind the user has set for an action.
- Clock can be visible only in spectator mode.
- Player can't no more handle 2 two-handed items.
- Customized some death notes.
- Reduced maximum seconds for the worm to emerge from 3 > 2 seconds.
- Updated *Snowy/Foggy* Planets list for Forest Giants.
- Increased regular Teleporter cooldown from 10 > 30 seconds.
- Taking shower won't help with stopping Ghost Girl to chase player.
- Terminal can be moved.
- Balanced ship malfunctions.
- Reduced Meteor Shower happening chance.
- Masked enemies can't get back to life after dying.
- Removed Cough sound when getting poisoned by a Puffer.

### 📈 Buy Rate Changes:

- Disabled jackpot buy rate alert.
- Reduced jackpot maximum rate from 3 > 2. (2 = 200%)
- Reduced last day maximum rate from 1.2 > 1.1.
- Reduced Guaranteed Company maximum buy rate from 1.2 > 1.1.

### 🚗 Cruiser Changes:

- Gear will not be automatically switched to drive/reverse from parked.
- Gear will not be automatically switched to parked when the key is taken from the ignition.
- Adjusted ignition chances.

### 🪜 Ladder Changes:

- Increased sprinting climb speed multiplier from 1.3 > 1.5.
- One-Handed items are not hidden no more when climbing a ladder.

### 🛒 Shop Changes:

- Removed *Ammo Tin* shop item.
- Reduced Shotgun shell price from 25 > 20 credits.
- Reduced Extend Deadline price from 800 > 500 credits.
- Reduced Belt Bag price from 160 > 125 credits.

### 🖥️ Terminal Upgrades Changes:

- Increased chance of upgrades going on sale.
- After quota failure upgrades won't be keeped.
- Removed upgrades:
  - Silver Bullets.
  - Fusion Matter.
  - Effective Bandaids.
  - Better Scanner.
- Increased Scrap Keeper price from 1500 > 2000 credits.
- Reduced Scavanger Instincts price from 1600 > 1000 credits.
- Increased Night Vision upgrade price from 550 > 800 credits.
- Reduced Midas Touch upgrade price from 2200 > 1800 credits.
- Balanced Bigger Lungs upgrade.

### 🎒 Belt Bag Changes:

- Reduced capacity from 15 > 6.
- One-Handed scrap can be placed inside.
- Belt Bag will be hidden when pocketed.

### 📡 Signal Translator Improvements:

- The Signal Translator turret deactivation may fail, resulting in you being filled with lead.
- Increased chance of Signal Translator teleporting player when being eaten by a Giant.
- Increased chance of Signal Translator teleporting player when being torched by an Old Bird.
- Increased chance of Signal Translator opening a blast door for player.

### 🕷️ Bunker Spider Changes:

- Reduced minimum webs that Bunker Spider can place from 20 > 18.
- Increased minimum distance between webs from 0.5 > 0.7.
- Increased web placement interval variation from 0.5 > 0.8.

### 🌨️ Weather Updates:

- Snowfall now has a lower chance of occurring on moons.
- Improved performance and behavior during Snowfall weather.
- Snowfall weather type is now exclusive to snowy moons.
- Solar Flare, Blizzard, and Windy weather have been removed.

### 🏠 Storehouse Interior Changes:

- Added hallway junction tile.
- Added thin storage room.
- Added Elevator storage room.
- Added Server room.
- Added Break room.
- Added Taller stairs.
- Added Elevator shaft.
- Added Steam leaks.
- Added more posters.
- Added custom stinger.
- Added randomized lighting.
- Added props that make sound to add a bit more variety to the soundscape.
- Elevator will fall before apparatus has been pulled.
- Stairs and storage rooms are less common.
- Easter egg door chance lowered by half.
- Patched a small hole in the slanty downward tile place with the door sometimes.
- Fixed a scrap spawn sometimes placing scrap in an unobtainable area.

## 🌌 ADDITIONS

### 🛠️ Added Mods:

- ImprovedClock
- FinallyCorrectKeys
- MrovWeathers

## 🗑️ REMOVALS

### 🛠️ Removed Mods:

- CodeRebirth
- VileVendingMachine
- Ammo Tin
- LCBetterClock
- HandsNotFull
- CustomWormTiming

## 📦 MOD UPDATES
*(Old version → New version)*

- WeatherRegistry: 0.3.12 → 0.3.13
- ButterFixes: 1.10.10 → 1.10.13
- Chameleon: 1.3.0 → 1.4.2
- ClaySurgeonOverhaul: 1.3.4 → 1.3.5
- BetaWeatherTweaksBeta: 0.24.6 → 0.25.0
- Useful Zap Gun: 0.3.1 → 0.3.2
- LethalElementsBeta: 1.2.50 → 1.2.60
- ArtificeBlizzard: 1.0.4 → 1.0.5
- Generic Interiors: 1.1.1 → 1.6.0
- SpiderPositionFix: 1.1.2 → 1.2.0
- CullFactory: 1.5.0 → 1.6.2
- CentralConfig: 0.16.0 → 0.16.1
- TestAccountCore: 1.12.0 → 0.13.1
- TestAccountVariety: 1.36.2 → 1.37.0
- DoorBreach: 1.1.0 → 1.2.1
- LethalNetworkAPI: 3.3.1 → 3.3.2
- EnemySoundFixes: 1.5.10 → 1.6.0

# MoreBrutalLethalCompanyPlus *v4.3.0*, *"Merry Christmas!"*

## 🔧 FIXES

- Fixed an issue where WeatherTweaks would override WeatherRegistry's configured scrap multipliers.

## ✨ NOTABLE CHANGES

- Days on moons were slightly increased.
- Balanced dynamically adjusting dungeon size:
  - Increased *Scale Dungeon Size Percent Change* from 2.5 > 8.
- Increased Gift spawn chance.
- Added new Christmas suits.

### 🎄 New Christmas Hazard:

- Introduced a new hazard that spawns inside the facility on all moons.
- The hazard resembles scrap, adding an element of surprise for players.

### ❄️ Weather Changes:

- **New weather: Blizzard:**
  - A relentless storm that tests your survival skills to the limit.
  - **Features:**
    - Increased challenges such as snow accumulation, frozen trails, and icy terrain.
    - Howling winds constantly change direction, making navigation treacherous and pushing players off course.
    - Prolonged exposure drains health from frostbite damage - seek shelter to recover.
    - Periodic waves of extreme cold deal instant frostbite and can sweep players away if caught.
- **New weather: Snowfall:**
  - A quiet but persistent snowfall of snow that reshapes the environment.
  - **Features:**
    - Snow accumulates over time, changing thee terrain and reducing movement speed based on depth.
    - Players leave tracks in the snow, allowing for faster movement along established paths. Strategic digging can increase mobility.
    - Shovels can be used to clear snow from critical areas or to escape traps.
    - Staying too long in deep snow increases the risk of freezing solid - find warmth or shelter to survive.
    - Open water freezes, creating new traversal opportunities and hazards.
- The weather *Heatwave* has been removed.
- Blackout's display color has been updated to improve visibility.
- Blizzard and Snowfall weather has a higher chance of occurring on moons.
- The chances of Stormy and Rainy Weather have been drastically reduced.

### ⚖️ Enemy Balance:

- **Locker:**
  - Reduced the chance of the Locker reactivating after a chase from 50% > 35%.
  - The Locker *no longer* automatically lunges at players in its line of sight and within reach.
  - Increased Lockers spawn power.
  - Reduced Lockers maximum spawn on moons.
  - Adjusted Lockers volume.
- **Peepers:**
  - Reduced Peepers weight from 7 > 6 lb.
  - Increased minimum Peeper group size from 2 > 3 Peepers.
  - Increased maximum Peeper group size from 4 > 5 Peepers.
- **BigMouth:**
  - Reduced players detection distance from 4.65 > 4.3.
  - Increased chase duration from 2 > 4 seconds.
  - Increased attack damage from 5 > 10.
- **Shy Guys:**
  - Reduced trigger time from 66.4 > 30 seconds. *(how long the Shy Guy must remain in the Triggered state to become fully enraged)*
- **Ogopogo:**
  - Increased the range that Ogopogo will lose player from 70 > 75.
- **Slenderman:**
  - Slenderman will shut off the facility lights after being seen the first time.
- **Siren-Head:**
  - After spawning will act like a tree.
  - Adjusted on what moons he can spawn:
    - *Vow, March, Fray, Celest, Gratar, Spectralis, Pelagia, Bomenoren, Eden.*
- **Nutcracker:**
  - Nutcrackers at 2-3 HP will shoot slightly faster.
  - Increased chance for Nutcracker to spawn on moons.
- **Eyeless Dog:**
  - Reduced Eyeless dogs HP from 12 > 10.

### 🔧 New Scrap:

- **Added New Scrap Items:**
  - *???* Radio
  - YIPPEE
  - *???* YIPPEE
  - Tree Cookie
  - Toy Car
  - Shiba Plush
  - Monitor
  - Gingerbread Man.

## 🌌 ADDITIONS

### 🛠️ Added Mods:

- LethalElementsBeta
- TestAccountVariety
- ChristmasSuits

## 🗑️ REMOVALS

### 🛠️ Removed Mods:

- YippeeScrap
- LethalElements

## 📦 MOD UPDATES
*(Old version → New version)*

- TerrasScrap: 1.0.1 → 1.0.2
- MrovLib: 0.2.12 → 0.2.13
- WeatherRegistry: 0.3.11 → 0.3.12
- BetaWeatherTweaksBeta: 0.24.5 → 0.24.6

# MoreBrutalLethalCompanyPlus *v4.2.4*, *"Fixes, Balance"*

## 🔧 FIXES

- Fixed LobbyImprovements menu color.
- Fixed systems having wrong moons.
- Fixed problem with enemies getting stuck inside on Tolian moons.
- Fixed game freezing for host. (at least didn't had that issue when did some solo tests. Needs further testing)

## ✨ NOTABLE CHANGES

- Removed strange items from the store.
- Clay Surgeon Changes:
  - Added a sleepiness screen filter when in proximity with a Clay Surgeon.
  - Remastered the proximity ambience audio.
  - Performance improvements.
- Systems page is now interactable.
- Removed Backrooms, it was causing some issues.
- Removed Rats, sometimes they wouldn't do any damage to the player.
- Removed the DarkMist enemy that was causing some NRE's (NullReferenceException).
  - [ *thanks* **Itekso** ]
- Slightly balanced dead bodies turning into masked enemies.
- Added Holiday themed skins to various entities.
- Zap Gun can cause damage to a player.
- Balanced weather scrap multipliers.
- Adjusted the enemy spawn rate to match the new enemy powers.

### 🌪️ Tornado Changes:
- Tornados **can't** appear on these moons:
  - *Experimentation, Adamance, Triskelion, StarlancerZero, Corrosion, Submersion, Hydro, USC Vortex, Kast, Infernis, Asteroid-13, Seichi, Detritus, Cosmocos, Atlas Abyss, Nyx, Crystallum, Ichor, Sector-0*.
    - Reason: they don't go well with these moons.

## 🌌 ADDITIONS

### 🛠️ Added Mods:

- LoadstoneUnstableBeta

## 🗑️ REMOVALS

### 🛠️ Removed Mods:

- LoadstoneNightly
- RealBackroomsPatch
- Backrooms
- Rats
- DiscountSync
- DarkMist
- ShovelInputBuffer

### 👾 Removed Enemies:

- StandaloneBlindPup

## 📦 MOD UPDATES
*(Old version → New version)*

- LethalConstellations: 0.2.8 → 0.3.8
- SnowyHolidayDropship: 1.1.2 → 1.1.3
- NightOfTheLivingMimic: 1.0.0 → 1.0.3
- Useful Zap Gun: 0.2.1 → 0.3.1
- ClaySurgeonOverhaul: 1.3.3 → 1.3.4
- OpenLib: 0.2.11 → 0.2.12
- Emblem: 1.6.2 → 1.6.4
- CodeRebirth: 0.9.6 → 0.9.7
- WeatherRegistry: 0.3.9 → 0.3.11
- SlipperyShotgun: 1.1.0 → 1.2.0
- SnowPlaygrounds: 1.0.5 → 1.0.6
- MrovLib: 0.2.11 → 0.2.12
- Flicker: 1.1.2 → 1.1.3
- altMoons: 0.3.4 → 0.3.5
- Spectralis: 0.9.7 → 0.9.8
- 13Kast: 1.0.8 → 1.0.9
- BetaWeatherTweaksBeta: 0.24.4 → 0.24.5
- ExtraEnemyVariety: 1.8.1 → 1.9.0

# MoreBrutalLethalCompanyPlus *v4.2.3*, *"Manifest"*

## 🔧 FIXES

- Fixed manifest file.

# MoreBrutalLethalCompanyPlus *v4.2.2*, *"Fixes"*

## 🔧 FIXES

- Fixed a bug where more than one player enter the same snowman at the same time.
- Fixed a bug that failed to destroy snowballs when a player entered a snowman.
- Fixed a bug that made the player unkillable when someone forced them out of the snowman with a snowball.
- Fixed a bug that caused some enemies that were disabled in Config to still mimic voices.
- Fixed an issue where routing to Eridanus (or any other system) would incorrectly direct the player to a different system.
- Fixed vanilla bug where the star and decorative lights are visible in the sky before the dropship appears.

## ✨ NOTABLE CHANGES

- Replaced ImmersiveScrap with TerrasScrap.
- Snow pile & Snowman will spawn only on snowy moons:
  - *Titan, Dine, Rend, Icebound, Auralis, Temper, Pinnacle, Polarus, Dirge, Tauralis, Feign, Summit, Tunere, WinterLodge, Siabudabu.*
- Balanced weather chance and scrap multiplier.

### 🏔️ WinterLodge Moon Changes:

- Added Festive lights for Christmas 🎄.
- Fixed the remaining audio reverb NullReferenceException.

### 🌘 Flicker Moon Changes:

- Film grain intensity begins slightly softer, but strengthens at night falls.

## 🌌 ADDITIONS

### 🛠️ Added Mods:

- TerrasScrap

## 🗑️ REMOVALS

### 🛠️ Removed Mods:

- ImmersiveScrap

## 📦 MOD UPDATES
*(Old version → New version)*

- Flicker moon: 1.1.1 → 1.1.2
- WinterLodge: 1.0.5 → 1.0.6
- DungeonGenerationPlus: 1.3.2 → 1.3.3
- BetaWeatherTweaksBeta: 0.24.2 → 0.24.4
- SnowPlaygrounds: 1.0.3 → 1.0.5
- Mirage: 1.15.4 → 1.15.4
- ClaySurgeonOverhaul: 1.3.2 → 1.3.3
- WeatherRegistry: 0.3.7 → 0.3.9
- CodeRebirth: 0.9.2 → 0.9.6
- MrovLib: 0.2.10 → 0.2.11
- SnowyHolidayDropship: 1.1.1 → 1.1.2
- Useful Zap Gun: 0.2.0 → 0.2.1

# MoreBrutalLethalCompanyPlus *v4.2.1*, *"Optimization, Balance & Fixes"*

## 🔧 FIXES

- Fixed Cruisers terminal 'Quit' command.
- Fixed NRE (Null Reference Exception) on re-host.
- Fixed interior shuffler picking the same random number for every interior on day passing.
- Hopefully fixed dungeon de-sync.
- Fixed hotbar being soft-locked by disabling quick drop, increasing swap interval to default.
- Fixed adding Scan Nodes to items that already have a Scan Node.
- Hopefully fixed soft-lock in orbit for host.
- Fixed dead bodies not being automatically collected as scrap when being teleported to the ship.
- Fixed the issue, when discounts are removed on server relaunch.

## ✨ NOTABLE CHANGES

- V69 compatibility.
- Changed logo for Christmas (again...)
- Improved optimization.
- Balanced Mineshaft interior:
  - Reduced generation size a half.
  - Loot is more distributed across the interior rather than it being super cave-focused.
- Reduced enemy spawning speed even more on starting moons.
- Reduced chance of shotgun falling from the hands.
  - Added *Slip* sound effect.
- Disabled modded Meteor Shower.
  - Increased chance of vanilla Meteor Shower happening.
- Balanced Map Hazards Min and Max values.
- Removed Centipede from Storehouse interior and replaced with other enemy because of Centipede bugging.
- Reduced chance of Supercharge happening.
- Reduced chance of Crawler (Thumper) spawning on starting moons.
- Improved Noise Suppression.
- Speaker no more plays intro voice.
- Enabled the left/right arrow keys to quickly cycle through radar cameras while using the terminal.
- Balanced enemy Butler.
- Turret balance:
  - Reduced damage per hit: 50 > 25.
  - Reduced warmup: 1.5 > 1 seconds.
  - Reduced rotation range: 45 > 40.
- Bringed back Vain Shrouds and the outside enemy **Kidnapper Fox**:
  - Player will drop all items when the Kidnapper Fox grabs player with their tongue.
  - Kidnapper Fox will not insta-kill player when they are taken to their nest.
  - Reduced amount of damage the Kidnapper Fox will deal to players per bite.
  - Vain Shrouds are snowy on snowy moons.
- Player can hold 2 two-handed items at a time.
- Made Zap Gun useful:
  - Red Locust Bees, Butler Bees and Docile Locust Bees can be killed within 3 seconds using Zap Gun.
- Moons with active weather will contain scrap with higher value.
  
### 🌩️ Enhanced Weather System:

- Advanced Weather Selection:
  - Implemented a more advanced system for choosing weather conditions.
  - On a new save, weather conditions now closely mimic the default vanilla behavior, even when modded moons are present.
- Dynamic Weather Enhancements:
  - **Combined Weathers**: Multiple weather conditions can now occur simultaneously, adding depth to gameplay.
  - **Progressing Weathers**: Weather conditions can now shift dynamically throughout the day, creating a more immersive experience.
- New Weather Condition: *Blackout*
  - All lights on the planet and in the dungeon are disabled, plunging players into total darkness.

### 🎄 New Christmas Features:

- Replaced Starship dropship with Holiday dropship from V45.
- Snow piles:
  - Snow piles now spawn outdoors, offering a festive hazard.
  - Players can collect 3 snowballs per stack from these piles.
- Snowballs:
  - A new throwable item with unique interactions:
    - **Ground Marks**: Dropping or throwing snowballs can leave marks on the ground, serving as hints or markers for other players.
    - **Player Interaction**: Hitting another player pushes them back and temporarily obstructs their camera view.
    - **Enemy Slowdown**: Snowballs hitting enemies slow them down.
    - **Snowman Explosion**: If a snowball hits a Snowman, the Snowman explodes.
- Snowman:
  - Snowmen spawn outdoors as a new hazard.
  - **Building Snowmen**:
    - Crouch and drop a snowball to create a small snowman.
	- Feed it additional snowballs to grow it into a fully formed snowman.
  - Player Hiding Mechanic:
    - Fully grown snowmen can be used as a hiding spot.
	- Use the secondary interaction key to exit, causing the snowman to explode.
	- Enemy Interaction:
      - If being chased, enemies will continue advancing toward the snowman.
	  - If an enemy collides with the snowman while a player is hiding, the snowman explodes, slowing the enemy for a few seconds to allow a player to escape.
      - **Invisibility**: If the player hasn’t been spotted and hides in a snowman, they remain invisible to enemies.

### ✨ Systems Balance:

- Refined system difficulty ranking for more balanced progression.
- Moons have been reassigned to the systems they fit into.
- Players can use command 'info <system>' to get information about any system.
- Renamed System *Orionis* > *Orion*.
  
### 🏰 Dynamically Adjusted Dungeon Size:

- Added a new feature that dynamically adjusts dungeon size based on the number of players in the lobby.
  - Default lobby size: 4
  - Example:
    - 1 Player: Dungeon size decreased to approximately 75.1% of the default.
    - 3 Players: Dungeon size decreased to approximately 90.9% of the default.
    - 5 Players: Dungeon size increased to approximately 110% of the default.
    - 7 Players: Dungeon size increased to approximately 133.1% of the default.
    - 12 Players: Dungeon size increased to approximately 214.3% of the default (capped at the Maximum Size Multiplier of 1.4).
    - *(The same formula used to calculate the scrap value adjustment is used again here)*
  - Thresholds and limits ensure balanced gameplay:
    - Min Size Multiplier: 0.65.
	- Max Size Multiplier: 1.5.
	
### 💰 Adjusted Penalty and Reward Systems:

 - **Penalty for Fallen Players:**
   - Reduced the penalty for each fallen player to **15%** (previously 20%).
   - Increased the penalty reduction for retrieving fallen players to **50%** (previously 40%).
 - **Penalty for Fallen Players (on Gordion):**
   - Introduced a small penalty of **5%** for each fallen player on Gordion (previously 0%).
   - Increased penalty reduction for retrieving fallen players on Gordion to **75%** (previously 0%).
 - **Proportional Fine System:**
   - Enabled **Proportional Fine** to ensure penalties scale fairly with the ratio of fallen players to total players.
 - **Rank Rewards:**
   - Adjusted rank rewards for better progression.
 - **Boost Credit Threshold:**
   - Lowered the Boost Credit Threshold to **100** (previously 120) to improve accessibility for players with lower credit balances.

### 🫧 Oxygen changes:

- Player will consume oxygen a lot faster in water.
- Running will consume oxygen faster.
  - Running on snowy moons will consume oxygen even faster.
- When player has no oxygen, player will be getting more damage *10 > 15*.
- Updated Green Planets list: Fray, Vow, March, Adamance, Celest, Gratar, Aquatis, Bomenoren, Volition, Seichi, Submersion, Humidity, Spectralis.
- Moons in the Scorpius system will consume more oxygen.
- Snowy moons will consume slightly more oxygen outside.
- Oxygen consumption inside the factory will be slightly lower than outside.
- Oxygen consumption is much higher when the player is in fear.
- Oxygen on Eclipsed moons will be limited.

### 🌙 Seichi changes:

- Optimization.
- Mostly unnoticable skybox changes.
- Fixed issues with *Katana*.
- Added Christmas tree and lights.

## 🌌 ADDITIONS

### 🌙 New Moons:

- Added new moon: Eden.
  - Which system: Draco.
  - Risk Level: S.
  
- Added new moon: 26 Feign.
  - Which system: Draco.
  - Risk Level: S.  
  
- Added new moon: 93 Ichor.
  - Which system: Gemini.
  - Risk Level: B+.  
  
- Added new moon: Gordion Sector-0.
  - Which system: Scorpius.
  - Risk Level: CORRUPTION DETECTED.  
  
- Added new moon: 290 Summit.
  - Which system: Andromeda.
  - Risk Level: C++.  
  
- Added new moon: 607 Tunere.
  - Which system: Scorpius.
  - Risk Level: CORRUPTION DETECTED.  
  
- Added new moon: WinterLodge.
  - Which system: Draco.
  - Risk Level: S.  
  
- Added new moon: Flicker.
  - Which system: Centaurus.
  - Risk Level: A+.  
  
- Added new moons pack: fiufkis Moons
  - Fremist:
    - Which system: Orion.
    - Risk Level: A.
  - Siabudabu:
    - Already in the pack.
  
### 🏠 New Interiors:

- Added new interior: Sector-0.

- Added new interior: Gray Apartments.

- Added new interior: Castellum Carnis.

### 🛠️ Added Mods:

- Mirage
- MirageCore
- RebalancedMineshaft
- CookieKnife
- NightOfTheLivingMimic
- Useful Zap Gun
- HandsNotFull
- KidnapperFoxSettings
- SnowyWeeds
- YesFox
- BetaWeatherTweaksBeta
- TurretSettings
- DiscountSync
- ButlerSettings
- SnowPlaygrounds
- SnowyHolidayDropship

## 🗑️ REMOVALS

### 🛠️ Removed Mods:

- MirageCoreExperimental
- MirageExperimental
- Zombies
- CustomDeathPenalty (replaced with CC)
- FastKnife (Replaced with CookieKnife)
- CozyImprovements
- FeedFish
- TeleportDecline
- MaskRemover
- TakeThatMaskOff
- LetTheDeadRest
- EnumUtils
- BetterLightning
- QuickerVote
- StopBeesOpeningDoors
- No Console Spam
- StarshipDeliveryMod

### 🏠 Removed Interiors:

- Removed interior: LC Office.

### 🌙 Removed Moons:

- Orion
- Gloom
- ZeimaltMod
- Teyn
- Aerona
- Harloth
- Azure
- SchalttafelMod
- Acidir
- Siabudabu (replaced with fiufkis Moons)

## 📦 MOD UPDATES
*(Old version → New version)*

- SlipperyShotgun: 1.0.8 → 1.1.0
- CentralConfig: 0.15.6 → 0.16.0
- UsualScrap: 1.7.4 → 1.7.6
- jLL: 1.7.7 → 1.8.0
- OpenLib: 0.2.10 → 0.2.11
- TestAccountCore: 1.10.0 → 1.11.0
- WeatherRegistry: 0.3.5 → 0.3.7
- ShipWindows: 1.10.2 → 1.11.0
- CodeRebirth: 0.8.11 → 0.9.2
- darmuhsTerminalStuff: 3.7.2 → 3.7.7
- Siabudabu: 6.0.0 → 6.0.1
- RuntimeIcons: 0.2.0 → 0.3.0
- LethalLib: 0.16.1 → 0.16.2
- CozyOffice: 1.3.5 → 1.3.6
- XuMiscTools: 1.3.0 → 1.3.1
- LethalConstellations: 0.2.7 → 0.2.8
- ExtraEnemyVariety: 1.8.0 → 1.8.1
- LoadstoneNightly: 0.1.17 → 0.1.18
- TooManyEmotes: 2.2.13 → 2.2.14
- Seichi: 0.9.10 → 0.9.11
- Olympus: 1.0.3 → 1.0.4
- Malfunctions: 1.10.0 → 1.10.1
- Locker: 1.6.0 → 1.6.1
- ReservedItemSlotCore: 2.0.38 → 2.0.39
- ghostCodes: 2.5.1 → 2.5.2
- LobbyImprovements: 1.0.6 → 1.0.7
- ScannableTools: 1.1.3 → 1.1.4
- DungeonGenerationPlus: 1.3.0 → 1.3.2

# MoreBrutalLethalCompanyPlus *v4.2.0* Release Notes

### _FIXES:_

- SpiderPositionFix:
  - Revamped how spider moves to wall position. Spider agent now path finds below the wall position before mesh container and agent separates.
  - Changed formula of how much agent speed is reduced.
  - Spider now tilts when moving on slopes.
  - Tweaked thresholds.
- Fixed non-masked voice playback delays.
- Fixed a issue that caused some players to not be able to use Mirage.
- Fixed a bug, where recordings are created even if the push-to-talk button isn't pressed.
- Fixed a bug that caused some Mirage recordings to be choppy.
- Fixed items going under the ship after rejoining.
- Fixed mineshaft echo persisting forever if you die in the caves.
- Fixed buggy keyboard interactions with the belt bag UI.
- Fixed audio issues with sandworm.
- Fixed the sun disappearing after you get fired.
- Weather effects are now correctly synced between clients.
- Weather effects are now correctly re-enabled when leaving dungeon.
- Hopefully fixed the player animator breaking after an charging explosion.
- Fixed a bug where game could completely lockup when you crouch in LOS of a Forest Giant.
- Hopefully fixed Sandworm being in the Facility.
- Fixed an error that occurred when returning to orbit without a previously purchased cruiser.
- Fixed a bug that locked both terminals after the first use of the terminal each day.
- Fixed compatibility bugs with Malfunctions.

### _NOTABLE CHANGES:_

- Balance:
  - Tweaked Min and Max scrap amount on some moons.
  - Enemies on starting moons will spawn slower.
  - Enemies will spawn hourly instead of every other hour.
  - Enemies will instantly emerge from their vent when spawned.
  - After enemy spawn lists are updated by various means, any time there are 2 or more of the same enemy, only the entry for the enemy with the highest rarity will be kept.
  - Balanced scrap.
  - Balanced enemies on moons.
- Removed a lot of mods that were broken or were making small changes.
  - Maybe improved optimization.
- Updated all logos.
- Updated slightly posters.
- Removed crates from spawning outdoors.
- Increased starting credits to *120*.
- If all crew will stay alive after a day, crew will get 10 bonus credits.
- Reduced visibility in dark.
- Balanced interiors weight for moons.
- Reduced spider health *6 > 5*.
- Artifice moon changes:
  - Artifice will be always snowy.
  - Daytime enemies will not spawn. (Manticoils, circuit bees, tulip snakes)
- Players cannot place scrap in Belt Bag, only tools.
- Lobby will be automatically reopen after reaching orbit.
- Added Backrooms:
  - There is 1% chance that player will be teleported to Backrooms with lethal damage.
  - There is 2% chance that player will be teleported to Backrooms with non-lethal damage.
  - There is 3% chance that player will be teleported to Backrooms if they will be annoying.
  - Player will drop everything after teleporting.
  - There are 2 smilers.
- Removed Bear traps.
- Tweaked weather weight.
- Decreased tornados moving speed.
- Decreased tornados pull strength.
- Added a Handlamp to the store.
  - Price - 50 credits.
- Facility doors cannot open players anymore.
- Cruiser terminal health system:
  - Cruiser terminal can be damaged
  - If it's destroyed, consequences will be bad...
- Added new skins to enemies.
- Reduced monster spawn amount during meltdown sequence.
- Removed all interior fog for better visibility.
- Toilet Paper is no more covering the whole screen when carried.
- When Meteor drops, it can sometimes spawn sapphire, ruby and emerald which costs big money.
- Oxygen will consume faster if player falls in to the water.
- Replaced Noot airhorn sound with Moan.
- Redwood giant when spawning can sometimes break trees.
- Removed scrap deposit notification:
  - The HUD element notifying players when scrap is deposited into the ship has been removed.
- Completely replaced the Barber's model with a new design.
- There is a small chance that player will drop gun after shooting.
- Replaced Enemies Scanner tool with Motion Tracker V3:
  - Costs - 300 credits.
  - Detects enemies in 40 radius.
  - Tracks movement near the player.
  - Battery - 4 minutes.
- Added new terminal command *'calc help'*:
  - 'calc [ *wanted money* ]' to calculate the money.
  - 'calc quota' to subtract the already earned money from your quota.
  - Standard calculating, for example 'calc 100+100'.
- Added Rats:
  - **Rat Nest:** A sewer grate can spawn on any map as a map hazard which will spawn rats every 8 - 20 seconds.
  - **Rat Types:** Each rat will either defend the nest or scout the dungeon. If a player gets too close to the nest, they will damage the player.
  - **Enemy Vents:** If a scout rat can't get to a desired location, it will try to crawl through the vents to get to that location.
  - **Lost Rats:** If a rat cannot get to the nest, even by vents, it will roam at random. These rats will never attack the player or enemies. They are only aggressive when part of a colony.
  - **Colony Threat System:** Each time a rat sees a player or enemy, they will add 1 to a threat counter. When that reaches a threshold they will start swarming the threat.
  - **Swarming:** When enough rats are swarming a target, they will begin attacking the target, dealing 2 damage each bite.
  - **Rat Food:** If a rat finds a player corpse it will attempt to drag the corpse back to the nest. They will rip food off of enemies depending on the enemies HP.
  - **Rat Control:** You can stop rats from spawning at a nest by finding the terminal code located on the nest. Inputing this code into the terminal will open/close the grate.
- Removed interiors:
  - Removed modded interior Warehouse.
  - Removed modded interior SCPFoundation.
- Removed *DynamicInteriorVariety* and replace with CC feature *Dungeon Shuffler*. (Info about that lower)
- Removed enemies:
  - The Walker.
  - The Watcher.
- Replaced *VoiceHud* mod push-to-talk icon with *LethalFixes* feature.
- No more music playing when ship leave alert appears.
- Maneater no more says *Yippee*.
- Removed Colorful Jar Of Pickles scrap.
- Hoarding Bug no more stomps with funny sound, only *Yippee*.
- Enemies cannot no more escape or enter the facility.
- Removed Gokus on some Generic Moons.
- Some changes for Cruiser:
  - Everyone can use eject driver button, instead of only driver.
  - Reduced seat boost slightly.
  - Some fixes.
- Removed moon Jabiua due to it bugs and being deprecated and moved Siabudabu from Cygnus to Cepheus.
- Terminal Upgrades changes:
  - Disabled upgrade Stimpack.
  - Sleght Of Hand price increased from 150 > 250 credits.
    - Increased price of each additional upgrade.
  - Silver Bullets price increased from 500 > 1000 credits.
  - Shutter Batteries price increased from 200 > 300 credits.
  - Scrap Keeper price increased from 1000 > 1500 credits.
  - Scavenger Instincts price increased from 1200 > 1600 credits.
    - Initial amount of scrap that can spawn in a given level when first purchased increased from 4 > 5.
  - Night Vision price increased from 380 > 550 credits.
  - Efficient Engines upgrade now works for systems.
  - Removed Oxygen Canisters upgrade.
  - Removed Long Barrel upgrade.
  - Removed Hollow Point upgrade.
  - Added new upgrade Midas Touch which increases the value of the scrap found in the moons.
    - Price - 2200 credits.
  - Lighting Rod price increased from 350 > 800 credits.
- Increased the amount of charge an item can get after supercharge.
- Item price changes:
  - Belt Bag - 45 > 160 credits.
  - Boombox - 60 > 80 credits.
  - Cat Food - 5 > 8 credits.
  - Extension Ladder - 60 > 115 credits.
  - Jetpack - 700 > 600 credits.
  - Lockpicker - 20 > 40 credits.
  - Pro-flashlight - 25 > 30 credits.
  - Radar-booster - 60 > 90 credits.
  - Shell - 20 > 25 credits.
  - Shotgun - 850 > 900 credits.
  - Shovel - 30 > 40 credits.
  - Stun grenade - 30 > 35 credits.
  - Walkie-talkie - 12 > 13 credits.
- New item - Battery:
  - Can spawn on all moons with small chance.
  - Can be bought in store for 10 credits.
  - Maximum that can be spawned on moon - 2.
  - Charge item with keybind - [ *C* ]
  
---

### _NEW FEATURES:_

- Added Scrap Shuffler:
  - Introduced a new mechanic that increases the likelihood of scrap spawning based on previous days it was not selected, enhancing the variety of scrap encountered during gameplay.
- Added Dungeon Shuffler:
  - Implemented a Dungeon Shuffler system that boosts the chances of dungeons being selected if they were not chosen on previous days, diversifying dungeon encounters.
- Adjusted Scrap Value for Player Count:
  - Modified the scrap value system to scale based on the number of players in the game, ensuring a balanced experience that rewards players with appropriate scrap amounts relative to their party size.
    - If in the crew there are playing less than 3 players, scrap value will be higher.
	- If in the crew there are playing more than 3 players, scrap value will be slightly smaller.
	
---
	
###  ● Added new interior: Storehouse.

---

### _WESLEYS INTERIORS CHANGES:_

- Explosive chests are easier to distinguish on spooky store.
- Lowered the damage of explosive chests slightly.
- Delightful hats have been added to nutcracker statues.
- Wooden doors will no longer push you away.

### _SIABUDABU MOON CHANGES:_

- Added new ambience, for a more dynamic gameplay.
- Improved terrain visual.

---

### _ADDED MODS:_

- CentralConfig
- True Darkness client
- YippeeMod
- CozyOffice
- Backrooms
- RealBackroomsPatch
- ToiletPaperNormalizer
- HornMoan
- LethalRichPresenceExperimental
- LessPopups
- Rats
- ClaySurgeonOverhaul
- SlipperyShotgun
- TerminalApi
- CreditCalculator
- Motion Tracker V3
- PriceTweaker
- Batteries
- MirageExperimental
- MirageCoreExperimental *[ Mirage dependency ]*
- LethalSettings *[ Mirage dependency ]*
- OpusDotNet *[ Mirage dependency ]*
- Concentus *[ Mirage dependency ]*

### _REMOVED MODS:_

- BlahajPlush
- ColorfulJarOfPickles
- StarlancerWarehouse
- ExtensionLadderFix
- UnityDebuggerAssistant
- DoomEternalSuperShotgunSounds ALT
- YippeeDweller
- YippeeTwo
- DynamicInteriorVariety
- GhostGirlFearReset
- LobbyCompatibility
- PizzaTowerEscapeMusic
- LETHAL RESONANCE MELTDOWN
- True Darkness
- StarlancerEnemyEscape
- EnhancedTZP
- VoiceHUD [using LethalFixes feature instead]
- Diversity
- SkinnedRendererPatch
- Forest Giant Motionsense
- GraphicsAPI
- TheGiantSpecimens
- SCPFoundationDungeon
- NAudioLame
- Mirage
- MirageCore
- LCCutscene
- LethalLevelLoaderAOORE Fix
- ExperimentalEnemyInteractions
- EpicNutcracker
- LethalQuantities
- NootHorn
- LethalRichPresence
- BarberMaterialTweaks
- Enemies Scanner
- LightsOut
- Jabiua

### _UPDATED MODS:_

- CullFactory *1.4.3 > 1.5.0*
- Generic Moons *6.1.1 > 6.2.0*
- WesleysInteriors *1.5.16 > 1.6.2*
- Moved Magnet Switch *1.2.0 > 1.4.0*
- GoodItemScan *1.10.0 > 1.12.0*
- FurnitureLock *1.3.3 > 1.3.6*
- RuntimeIcons *0.1.6 > 0.2.0*
- Mirage *1.11.1 > 1.12.3*
- Cruiser Additions *1.4.0 > 1.4.1*
- LostEnemyFix *1.0.0 > 1.1.0* 
- BarberFixes *1.2.0 > 1.2.2*
- LobbyControl *2.4.9 > 2.4.10*
- WeatherRegistry *0.2.7 > 0.3.5*
- BarberMaterialTweaks *1.1.0 > 1.1.1*
- SpiderPositionFix *1.1.0 > 1.1.2*
- Chameleon *1.2.1 > 1.2.4*
- ZeimaltMod *1.1.0 > 1.1.1*
- altMoons *0.3.3 > 0.3.4*
- Biodiversity *0.1.3 > 0.1.4*
- UsualScrap *1.6.9 > 1.7.4*
- TestAccountCore *1.9.1 > 1.10.0*
- RemoveInteriorFog *1.0.1 > 1.0.2*
- CodeRebirth *0.7.18 > 0.8.11*
- SchalttafelMod *1.2.1 > 1.2.2*
- LCUltrawide Community *1.1.2 > 1.1.3*
- LoadstoneNightly *0.1.16 > 0.1.17*
- FashionableCompany *0.3.0 > 0.4.0*
- ButteryFixes *1.10.8 > 1.10.9*
- Tauralis *1.0.4 > 1.0.6*
- Seichi *0.9.8 > 0.9.10*
- LethalConstellations *0.2.5 > 0.2.6*
- ExtraEnemyVariety *1.7.0 > 1.8.0*
- OpenLib *0.2.8 > 0.2.10*
- MrovLib *0.2.8 > 0.2.10
- StandaloneBlindPup *1.1.0 > 2.0.0*
- LethalLevelLoader *1.3.10 > 1.3.13*
- LethalPerformance *0.4.2 > 0.5.0*
- TooManyEmotes *2.2.11 > 2.2.13*
- darmuhsTerminallStuff *3.6.8 > 3.7.3*
- CruiserTerminal *1.0.4 > 1.1.1*
- ColorfulEnemyVariety *1.4.1 > 2.0.1*
- Lategame Upgrades *3.10.11 > 3.11.0*
- LethalConstellations *0.2.6 > 0.2.7*
- Supercharger *1.1.0 > 1.2.0*
- Vacuity *1.0.4 > 1.0.5*
- ButterFixes *1.10.9 > 1.10.10*
- Cruiser Additions *1.4.1 > 1.4.2*
- Siabudabu *5.1.0 > 6.0.0*
- TonightWeDine *1.0.1 > 1.0.2*
- Matty Fixes Experimental *1.1.32 > 1.1.33*
- Chameleon *1.2.4 > 1.3.0*

# MoreBrutalLethalCompanyPlus *v4.1.0* Release Notes

### _FIXES:_

- Fixed some visual issues with the "sun" during the blizzard.
- Malfunctions:
  - Fixed Power Malfunction not triggering on Experimentation.
  - Fixed disabled Spark VFXs still triggering light.
- Fixed wooden crates having health similar to metal crates.
- Fixed scan nodes with wooden and metal crates.
- Fixed issues metal crates.
- Fixed some redwood sounds playing only on host.
- Fixed raindrops on helmet during Flooded weather.
- Fixed grouped enemies sometimes adding to power count without actually spawning.
- Fixed clients not being able to place items on a Jester if it spawned inside while they were outside.
- The external security camera is now a bit busier when in orbit:
  - Now you can see the planet and moon the ship is parked at, like earlier versions of the game.
  - Fixed the particle effect when the ship is moving through space, so it now works every time you reroute (not just the first time).
- Fixed bug with getting fired.
- Fixed loud horn position when buying.
- Fixed players being unable to join on rehost.
- Fixed Barbers being unable to open doors.
- Fixed issues with wooden crates.
- Fixed LAN-related issues.
- Fixed Emblem error.
- Fixed helmet condensation not working on Gordion.
- Fixed an error in the Dungen Optimizations which would cause errors with certain interiors.
- Fixed item rotations on drop for VoidLeak scrap.
- Fixed glowing eyes on Tragedy mimics.
- Fixed an error that would occur when landing on a moon with no sun.
- Fixed the spider getting stuck and desync between agent and mesh container.
- Fixed Barber's weird errors with UVs as well as restoring his broken normal map that was cut from v60.
- Fixed EnemyAI that fails to locate NavMesh by killing them.

### _NOTABLE CHANGES:_

- Compatibility for V68.
- Removed pumpkins from logo.
- Small poster changes.
- Bodycam:
  - Sky effects are changed based on the camera perspective. When a body cam target is looking down a long hallway in the interior, the end of the hallway will now be dark.
  - Bodycams be unaffected by the game's gamma setting and the tonemapping pass. This should help with visibility in dark areas, as well as prevent the screen from getting excessively bright when gamma is above normal.
  - Fixed a vanilla bug that prevents players converted by a masked enemy from being targeted by the map.
  - The sun always visible on the body cam, regardless of the render distance.
  - Adjusted head cam attachment points to be more consistent.
  - Switched to a more accurate method of tracking the environmental visuals that should be visible on body cams.
- Locker enemy:
  - Reworked blood effects.
  - Adjusted hitboxes and speed.
  - Imporved player path finding.
  - Added destruction of turrets during chases.
  - Added destruction of landmines during chases.
  - Fixed not being able to traverse slopes and stairs.
- BetterEXP:
  - Added a new note - "Team Backbone" - given to the player(s) with the most found scrap that made it to ship.
    - The scrap does not have to be returned by the player who found it, any player will count for it.
  - Your player's badge that appears to teammates will now be based on your BetterEXP rank instead of vanilla rank.
  - Some adjustments have been made to the Death Penalty reduction and contribution & scrap found bonuses.
  - Vanilla EXP will now show up on the pause menu, underneath your BetterEXP rank.
- PjonkGoose can now use fire exits and elevators to chase the player outside.
- Added new mechanics to the Walker.
- Changed the Watcher spawn logic.
- Tweaked darkness intensity for better visibility.
- Balanced enemies escaping facility.
- Balanced weather weight.
- Shopping Cart is more rarer.
- Added Bear Traps:
  - Can only spawn outside.
- Balanced metal & wooden crate spawning outside.
- Slightly updated metal crate textures.
- If crates are pulled up with fists, the player gets damaged a little bit.
- Balanced enemies and scrap spawning for some moons.
- Meteor Shower now can be as weather.
  - They are rare.
- Disabled Halloween fog.
- Added new themed cosmetics.
- Added cruiser upgrades:
  - Brake Fluid.
  - Ignition Coil.
  - Improved Steering.
  - Rapid Motors.
  - Supercharged Pistons.
  - Turbo Tank.
  - Vehicle Plating.
- Removed Shrimp enemy due to his some bugs.
- Lobby:
  - Added an option to specify a server password.
  - Added a warning when trying to host a lobby where the name contains a word that is blacklisted in vanilla.
  - Replaced the public, friends-only & invite-only buttons with a dropdown.
- Leviathan can target other creatures.
- Red bees will defend nest from mobs and kill everything in rampage.
- Added a Ammo Tin item that can be bought in the terminal shop:
  - Price - 500.
  - If no shells are found in hotbar, shotgun will use a % of the charge to reload.
  - Can be recharged at the ship.
  - Total Shotgun Reloads from Full Ammo Tin 8.
- Added terminal in the cruisers back.
- Dust pan is now conductive.
- There's a ghost in the terminal and it's sending random broadcast codes to mess with the facility:
  - Instanity Mode Bonuses:
    - If player dies then there will be added percentage to insanity.
	- If Ghost Girl has been spawned then there will be added percentage to insanity.
  - Counter Play:
	- To stop Ghost Girl from chasing, player try can use shower (there's a small chance).
	- Player can try to delay ghostCodes by rebooting terminal (65% chance).
  - Cruiser Interaction:
    - Ghost have a chance to change cruiser radio station.
	- Ghost have a chance to eject player from the driver seat.
	- Ghost have a chance to flicker cruiser lights.
	- Ghost have a chance to break the cruiser windshield.
	- Ghost have a chance to push cruiser.
	- Ghost have a chance to open/close cruiser doors.
  - Door Interaction:
    - Ghost have a chance to unlock single facility door.
	- Ghost have a chance to lock single facility door.
	- Ghost have a chance to haunt all facility doors (rapidly open/close).
  - Ghost Girl:
    - Ghost Girl have a chance to breathe on walkie-talkie.
	- Ghost Girl have a chance to garble walkiie-talkie.
	- Ghost Girl have a chance to affect haunted players batteries.
  - Messages:
    - Ghost can send some unique messages to the signal translator and ships monitor.
  - Main Interaction:
    - Ghost can:
	  - Start turret berserk.
	  - Bit blast doors.
	  - Blow a mine.
	  - Flicker a player's light.
	  - Flip breakers.
	  - Interact with noisy scrap items.
	  - Interact with any noisy scrap that is held by a player.
	  - Affect all batteries.
	  - Affect random player batteries.
	  - Disable Toil-Head turret.
	  - Start Toil-Head turret berserk.
  - Ship Interactions:
    - Ghost can:
	  - Teleport a player.
	  - Inverse teleport a player.
	  - Flicker lights on the ship.
	  - Open/close ship doors.
	  - Shock a player that is interaction with terminal.
	  - Change lever's weight.
	  - Call a drop ship with a random item from the store for free.
  - __**If you have __epilepsy__, disable *Rapid Lights* in config ghostCodes.Setup - Effects/Sound!**__
  
---  

###  ● Added new moon: Integrity.
  - Which system: Cepheus.
  - Risk Level: ?.
  
###  ● Added new moon: Humidity.
  - Which system: Taurus.
  - Risk Level: ?.
  
###  ● Added new moon: Olympus.
  - Which system: Eridanus.
  - Risk Level: ?.
  
###  ● Added new moon: Detritus.
  - Which system: Eridanus.
  - Risk Level: ?.

###  ● Added new moon: ???.
  - Which system: ErI##*#3.
  - Risk Level: ?.
  - Name can be found on one of the Generic Moons.
  
---
  
###  ● Added new interior: Storehouse.

--- 

###  ● Added new enemy: Faceless Stalker (Slenderman).
**Spoiler:**

<details>
  <summary>Faceless Stalker SPOILER</summary>  
  <img src="https://i.imgur.com/jLTNNjM.png" alt="bruh">

- Mysterious pages have appeared inside the facility... They depict what seems like a tall, faceless humanoid entity.
- The employer has stated that these pages are of high value, but warns employees to "regularly check behind their backs" while collecting them.
- Why? No one knows, but it seems there's more to these pages than meets the eye.

Anyways, what could possibly go wrong?

</details>
  
---
  
### _GENERIC MOONS CHANGES:_

- Allowed creatures to walk onto the ship's catwalk/into the ship (except for Hydro).
- Fixed the shine on some waters.

- Collateral:
  - Fixed Maneater door.
- Icebound:
  - Snowflakes no longer spawn at **all** if its rainy or stormy.
- Hydro:
  - Visual touchups.
- Corrosion:
  - Acid now counts as water.
  - Acid now blows up the car.
  - Acid no longer instantly kills you upon being fully submerged.
  - Acid damage is properly reoccurring.
- Vertigo:
  - The pit now blows up the car.
  - Fixed brutally and mercilessly executing the player upon attempting to play the game.
  
### _WESLEYS INTERIORS CHANGES:_

- Fixed some errors.
- Entrances are now networked properly.
- Fixed some visual jank in Grand Armory.

### _DISTINCT MOONS VARIETY CHANGES:_

- Rebaked Occlusion Culling.
- Improving terrains shading and lighting significantly to be more in line with Vanilla.

- 16 Attenuation:
  - Terrain was remade.
  - Removed Flooded as a possible random weather.
- 77 Volition:
  - Terrain improvements.
  - Fixed Ground Fog not covering the entire map.
  - Added Foggy as a possible random weather.
  - Removed Halloween decorations.
  - Main Entrance Building Area & Houses Area have been expanded.
  - Added new routes for the side buildings at Main Entrance.
  - Leveled some pits that would get the cruiser stuck guaranteed to incentivize more cruiser travel.
  
### _SEICHI MOON CHANGES:_

- Lantern:
  - Light stays on when dropped.
  - Light stays "on" when pocketed.
  - Custom turn on and off SFXs.
- Added Kanabo and Uchiwa sound.
- Removed Katana's mesh collider (Bodies won't fling out of existence).
- Fixed Volcanic and SnowyAmbience not playing.

### _SDM INTERIOR CHANGES:_

- Two new outside rooms with an open sunroof.
- Added visual effects to the dungeon if the moon has certain weather effects.
- Dungeon "sunlight" will decrease over time.
- The v62 enemies should destroy Scarlet doors now.
- Undefined enemies (future or modded) should open Scarlet doors by default now.
- Updated the red anger effect to be less red.
- Made the mayor pillars float higher in the air.

### _ADDED MODS:_

- XuMiscTools
- Matty Fixes
- AutoScroll
- Generic Interiors
- ShyGuySettings
- FastKnife
- FashionableCompany
- Olympus
- SpiderPositionFix
- LobbyControl
- BarberMaterialTweaks
- ExperimentalEnemyInteractions
- CruiserTerminal
- Lategame Company Cruiser Upgrades
- Ammo Tin
- FacelessStalker
- ghostCodes
- Moon Scrap Limits Patch
- RemoveInteriorFog
- LandFromOrbit
- RuntimeIcons
- LostEnemyFix

### _REMOVED MODS:_

- LobbyControl Experimental
- HalloweenTheme
- BetterSpectate
- SmartItemSaving
- YesFox
- BetaWeatherTweaksBeta
- AutoKnifeAttack
- EnhancedIcons

### _UPDATED MODS:_

- darmuhsTerminallStuff *3.6.2 > 3.6.8*
- MrovLib *0.2.4 > 0.2.8*
- Football *1.1.10 > 1.1.11*
- CodeRebirth *0.5.2 > 0.7.18*
- EnemySkinKit *1.3.1 > 1.3.4*
- WeatherRegistry *0.1.25 > 0.2.7*
- ReservedItemSlotCore *2.0.37 > 2.0.38*
- OpenLib *0.2.2 > 0.2.8*
- GoodItemScan *1.9.0 > 1.10.0*
- jLL *1.6.5 > 1.7.3*
- UsualScrap *1.6.8 > 1.6.9*
- LethalSettings *1.4.0 > 1.4.1*
- Better Shoothgun Tooltip *1.2.0 > 1.3.0*
- VertexLibrary *1.0.0 > 1.1.0*
- DistinctMoonVariety *1.2.0 > 1.5.0*
- Tauralis *1.0.2 > 1.0.4*
- Azure *0.6.2 > 0.6.4*
- 13Kast *1.0.7 > 1.0.8*
- FurnitureLock *1.3.2 > 1.3.3*
- ExtraEnemyVariety *1.6.0 > 1.7.0*
- Better Shootgun Tooltip *1.3.0 > 1.4.0*
- More Suits *1.4.3 > 1.4.5*
- TooManyEmotes *2.2.7 > 2.2.11*
- OpenBodyCams *2.4.3 > 3.0.3*
- CullFactory *1.4.2 > 1.4.3*
- Lategame Upgrades *3.10.4 > 3.10.11*
- BetterEXP *2.4.0 > 2.5.2*
- ButteryFixes *1.10.3 > 1.10.8*
- AdditionalNetworking *2.1.1 > 2.1.2*
- CIBL *1.2.5 > 1.2.6*
- VentSpawnFix *1.2.1 > 1.2.2*
- jLL *1.7.3 > 1.7.7*
- NiceChat *1.2.6 > 1.2.7*
- MoreCompany *1.10.1 > 1.11.0*
- WesleysInteriors *1.5.10 > 1.5.16*
- Generic Moons *5.3.1 > 6.1.1*
- Harloth *0.5.4 > 0.5.6*
- ArtificeBlizzard *1.0.3 > 1.0.4*
- Seichi *0.9.2 > 0.9.8*
- LethalConstellations *0.2.4 > 0.2.5*
- AdditionalNetworking *2.1.2 > 2.1.3*
- ButterFixes *1.10.3 > 1.10.7*
- BepInEx-FLABP *0.6.4 > 0.6.5*
- Locker *1.2.6 > 1.6.0*
- Mirage *1.11.0 > 1.11.1*
- Malfunctions *1.9.1 > 1.10.0*
- Groan Tube Scrap *1.0.2 > 1.0.4*
- TestAccountCore *1.8.2 > 1.9.1*
- Zombies *0.3.10 > 0.3.12*
- PjonkGoose *1.4.1 > 1.5.0*
- ImmersiveScrap *1.3.1 > 1.4.0*
- MoreCounterplay *1.4.0 > 1.4.1*
- Cruiser Additions *1.3.0 > 1.4.0*
- Diversity *2.1.2 > 3.0.2*
- Emblem *1.5.3. > 1.6.2*
- LobbyImprovements *1.0.4 > 1.0.6*
- Spectralis *0.9.6 > 0.9.7*
- LC Office *1.2.4 > 1.2.5*
- LobbyCompatibility *1.2.0 > 1.3.0*
- BarberFixes *1.1.0 > 1.2.0*
- ScarletDevilMansion *1.3.27 > 2.0.0*
- DungeonGenerationPlus *1.1.2 > 1.2.0*
- EGypt Moon *2.0.20 > 2.0.21*
- VoidLeak *1.12.1 > 1.13.1*
- LoadstoneNightly *0.1.15 > 0.1.16*
- SpikeTrapFixes *1.1.1 > 1.1.2*
- Chameleon *1.1.3 > 1.2.1*

# MoreBrutalLethalCompanyPlus *v4.0.1* Release Notes

### _FIXES:_

- Fixed custom poster not loading up.
- Fixed when navigation malfunctioning happens then you are getting to the other system.
- Fixed spider webs.
- Fixed item rotations.
- Fixed an issue where some items would be visible off-screen after spawning.
- Fixed softlock by SignalTranslatorUpgrade mod.
- Fixed Night Vision not working in the dark.
- Fixed an issue with lobby codes that caused the 4th player to be unable to join.
- Fixed 6th player having their camera stuck when looking at siren head.
- And more.

### _NOTABLE CHANGES:_

- Halloween!
- Changed modpacks logo.
- Balanced interior chances on the moons.
- Oxygen Changes:
  - Added new moons to green oxygen list.
  - Snowy moons will drain more oxygen when player is running.
- Theres a small chance that bracken with insta kill player instead of taking player.
- Signal Transmiter Changes:
  - Disabled Signal Transmiter enemy warning chance because they can be very annoying.
  - Reduced Blast Doors opening and closing chance when looked at it.
  - Enabled teleporting a player that is being torched by Old Birds.
- Reduced Belt Bag capacity *15 > 5*.
- Increased climbing speed while sprinting slightly.
- Increased ladder entrance animation slightly.
- Allowed using ladders while carrying a two-handed object.
- Big Mouth enemy now is spawning on more moons.
- Balanced scrap price and spawn slightly.
- Added revive limit, 1 revive per player.
- Increased Shotgun price *650 > 850* credits.
- Increased jackpot alert display delay to prevent overlapping.
- Added tags to some of the moons that didn't had it.
- Reverted exterior door change.
- Increased Coil-Head teleporting timer when not encountering a player *60 > 80* seconds.
- Increased Coil-Head chance to chase the player after passive teleporting.
- Death Penalty Changes:
  - Decreased quota increasing percent per dead player that is not retrived *5% > 3%*.
  - Reduced fine percentage for each dead player *5% > 4%*
  - Reduced fine percentage reduction for retrieving the players body *40% > 30%*.
- Increased scanning light range and intensity slightly.
- DarkMist Changes:
  - Limited max doors closed times by DarkMist enemy *∞ > 10*
  - Increased minimum distance from entrance to DarkMist to start targeting the player *25 > 50*.
  - Increased timer in seconds after getting targeted by DarkMist.
- If player exited and left terminal on page 'moons', after entering terminal it will load home page.
- Extend Deadline Changes:
  - Increased Extend Deadline price *555 > 800* credits.
  - *50* credits additional cost will be added to the Extend Deadline per every quota completed.
  - *5* credits additional cost will be added to the Extend Deadline command per every day extended.
- Reduced apparatus value *250 > 180*.
- Slightly reduced full darkness intensity.
- Gambling Changes:
  - Increased Gambling Machine uses *100 > 500*.
  - Added 1 more Gambling Machine.
- Increased Wheelbarrow's and Shopping Cart's grab prompt size to reduce the amount of situations where it can get stuck.
- Replaced Dropship with Starship:
  - It's much bigger.
  - It has custom VFXs and SFXs.
  - It's louder.
  - It has custom animations.
  - Has increased render distance when outside.
- Jetpack will explode if player is flying too fast, while you are also *extremely* high above the terrain.
- Enemy Scanner Changes:
  - Increased Enemy Scanner price *200 > 350* credits.
  - Reduced Scanner cooldown timer *50 > 35* seconds.
  - Increased count of the nearest enemies *4 > 5*.
  - Increased battery capacity *135 > 160*.
- Reserved Slot Changes:
  - Decreased Bracken walkie-talkie jamming distance *70 > 65*.
  - Decreased Office elevator music volume *100 > 75*.
  - Increased Walkie-Talkie reserved slot price *350 > 500* credits.
  - Increased Utility reserved slot price *250 > 300* credits.
  - Increased Spray Paint reserved slot price *90 > 130* credits.
  - Increased Scrap Insurance price *500 > 650* credits.

### _WESLEYS INTERIORS:_

- **Grand Armory:**
  - Overhauled generation to fix non-spawning issues, now features different paths with acid sections and varied loot rooms.
  - Replaced acid tiles with modular ones, added stairs and jump variations to acid pool transitions.
  - Added second elevator, lava pools in Canyon/Wasteland tags, and ball pits to the Fun tag.
  - Platforms above lava are now safe from burning.
  - Synced loot room switches and introduced alarm system in loot rooms when power is down.
  - More fire exits in deeper areas, additional loot spot in cold storage server room.
  - Fixed missing parkour platform, added new ambient sounds, and apparatus spot in cold storage.
- **Toy Store:**
  - Optimized gameplay, made shotgun doors more dangerous.
  - Introduced unused nutcracker crawling vent sound as ambience and removed wall turret.
  
### _HARVEST MOONS CHANGES:_

- Fray:
  - Visual adjustments.
- Cambrian:
  - Fixed main entrance scan node being behind them main entrance.
- Temper:
  - Various improvements.

### _ALTMOONS CHANGES:_

- Dirge:
  - Fixed the eclipsed sun and hopefully the path back to ship is more obvious.
  - Fixed fire exit rotation.
  - Small interior spawn tweaks.
- Phaedra:
  - Sun is larger.
  - Fixed fire exit rotation.
- Pelagia:
  - Large outside rework.
  - Day/night spawns have been lowered.
  - Scrap spawns are changed.
  - Culling and LODs have been properly added to the swamp trees.
  
### _ADDED MODS:_

- VertexLibrary
- True Darkness
- StarshipDeliveryMod
- Wesleys Skins
- HalloweenTheme

### _REMOVED MODS:_

- BodyRemover
- Synthesis Dropship Replacer
- ScannableCodes
- SignalTranslatorUpgrade
- Full Darkness
- CorrectDeathPenalty

### _UPDATED MODS:_

- darmuhsTerminallStuff *3.6.0 > 3.6.7*
- CullFactory *1.3.15 > 1.4.2*
- LethalFixes *1.2.3 > 1.2.4*
- LethalPerformance *0.4.0 > 0.4.2*
- MattyFixes Experimental *1.1.24 > 1.1.28*
- StoreRotationConfig *2.4.1 > 1.5.0*
- jLL *1.6.0 > 1.6.5*
- Zombies *0.3.8 > 0.3.10*
- Better Shotgun Tooltip *1.1.1 > 1.2.0*
- WesleysInteriors *1.4.6 > 1.5.9*
- Harloth *0.5.3 > 0.5.4*
- 13Kast *1.0.6 > 1.0.7*
- Lategame Upgrades *3.10.3 > 3.10.4*
- TeleportDecline *1.0.2 > 1.1.0*
- StarlancerAIFix *3.8.3 > 3.8.4*
- LoadstoneNightly *0.1.14 > 0.1.15*
- Shopping Cart *1.0.1 > 1.0.2*
- Wheelbarrow *1.0.2 > 1.0.3*
- Custom Item Behaviour Library *1.2.4 > 1.2.5*
- LobbyImprovements *1.0.3 > 1.0.4*
- WesleysInteriors *1.5.9 > 1.5.10*
- HarvestMoons *1.4.10 > 1.4.11*
- Azure *0.6.1 > 0.6.2*
- SirenHead *2.0.1 > 2.0.3*
- altMoons *0.3.1 > 0.3.3*
- LethalResonance *4.7.4 > 4.7.5*
- Interactive Terminal Api
- LobbyControl Experimental *4.4.1 > 4.5.0*
- ArtificeBlizzard 1.0.2 > 1.0.3*
- TooManySuits *2.0.0 > 2.0.1*
- WesleysInteriorsAddon *1.0.1 > 1.1.0*
- Xen Interiors *0.3.3 > 0.4.0*
- Mirage *1.10.0 > 1.11.0*
- EnemySkinKit *1.2.5 > 1.3.1*
- No Console Spam *1.5.0 > 1.6.0*
- Cruiser Additions *1.2.2 > 1.3.0*
- ButteryFixes *1.10.2 > 1.10.3*

# MoreBrutalLethalCompanyPlus *v4.0.0* Release Notes

### _FIXES:_

- Fixed some de-syncs.
- Fixed the cabin doors using the wrong sounds on Rend and Adamance.
- Fixed Teeth scrap not being collected when magnetizing the Cruiser.
- Old Birds and Forest Keepers no longer interfere with items (soccer balls & whoopee cushions) when they are inside the ship.
- Fixed throwables (stun grenades, easter eggs, etc.) falling through catwalks and other metal gratings when thrown.
- Fixed some enemy line-of-sight issues on Dine.
- Spike traps no longer spawn inside of the elevator.
- Fixed the jetpack's broken proximity warning (the beeping) logic.
- Hopefully fixed the bug where the jetpack "drops inputs" when clicking.
- Fixed Old Birds erroring if the enemy they are targeting gets destroyed by an earth leviathan.
- Fixed error occuring when disconnecting from a lobby when you were previously a host in the game session.
- Fixed furniture moving sound during lobby creation.
- Fixed issue with clients purchasing the deadline extensions.
- Fixed mimics not playing hit sounds when the death animation occurs.
- Fixed old birds erroring if the enemy they are targeting gets destroyed by an earth leviathan.
- Fixed Signal Transmiter.
- Fixed issue of causing errors when typing nothing but spaces in the terminal.
- Fixed extension ladder clipping through the map.
- Fixed issues when players were still heavy even without carrying anything.

### _NOTABLE CHANGES:_

- Improved optimisations.
- Added a new "Systems" command to the terminal. Typing **Systems** will display all 14 systems, each containing a set of moons.
  - Expansive systems indicate higher difficulty levels.
  - Moons within the same system have similar difficulty, with slight variations in scrap and monster spawn rates.
- Added a lot of new moons.
- Changed crosshair slightly.
- Eyeless Dogs have 10 HP now instead of 12.
- Lowered Hoarding Bug picking player up chance to minimum.
  - Hold time was reduced *7 > 3* seconds.
- Removed suit pages keybind changers.
  - Player can click on the arrow to change page.
- New ambience sounds and more.
- Added an overlay that indicates the reason the body cam is not visible.
- Disabled all other tornado types and enabled only *smoke* tornado type.
- When scanning in the dark it will get little brighter for better visibility.
  - Changed scan color to a subtle dark white.
- Added an overlay to the bodycam.
- Removed random emote keybind.
- Remade posters.
- Remade game menu logo.
- Imitate players voices can only: Oni, Masked and Flowerman.
- Balanced gambling chances on Company moon.
  - Player can only use gambling machine 100 times.
- Tweaked LGU upgrades price and more.
- Increased bonus credits.
- Made jester scream loader and horryfier.
- Added new suits.
- Corrected the penalty shown when dead bodies are recovered.
- Dead enemies bodies will despawn after a time.
- Improved shotgun safety tooltip.
- In spectator screen players can see cause of the death.
- Improved spray paint.
- Player can purchase scrap insurance which saves their currently stored scrap in the ship from disappearing when all players get wiped in the moon landing after the purchase.
  - Terminal command 'scrap insurance'.
- Balanced meteor shower chances.
- Players can see a red message show up on screen with the jackpot buy rate.
- Tweaked buy rate chances.
- Added storage lights.
- Added light switch glowing in the dark.
- Added charge station glowing in the dark.
- Popped Jester with built up enough speed, will now slam open doors instead of just normally opening them.
- Moved magnet switch.
- Added buyable reserved spray paint slot.
- The company monster will now kill everyone in its path, not just one employee.
- Added bunch of new scrap.
- Bracken will snatch player instead of killing player instantly.
- Added back new interior *Office*.
- Changed Dines layout.
- Replaced the dropship theme.
- and mooooooree.

###  ● Added new enemy **PjonkGoose**.
**Spoiler:**

<details>
  <summary>PjonkGoose SPOILER</summary>  
  <img src="https://i.postimg.cc/ZKQtCSLg/image-2.png" alt="popo">

- PjonkGoose sits on his expensive golden egg, if you take it, you are ded...

</details>

###  ● Added new enemy **BigMouth**.
**Spoiler:**

<details>
  <summary>BigMouth SPOILER</summary>  
  <img src="https://gcdn.thunderstore.io/live/repository/icons/Wexop-BigMouth-1.1.1.png.128x128_q95.png" alt="BigMouth">

- It's just a teeth in the distance, but if you come closer, it will get much bigger...

</details>

### _SEICHI MOON CHANGES:_

- Performance & Optimizations:
  - New occlusion culling for better performance.
  - Added LODs to most objects, optimized materials (Hachiman, bamboo, masks), and tweaked textures.
- Adjustments:
  - Moved BoundWalls, AI nodes, and interact triggers.
  - Changed audio intervals and chance percentages.
  - Terrain and texture updates for Seichi, Snowichi, and Scorchi maps.
  - Adjusted ambience and fog effects on Foggy weather.
- Fixes & Features:
  - Fixed animations (Oni, OniMask), material bugs, and offsets.
  - Readded custom sound effects and fixed various audio issues.
  - Shisha will now run faster when hit.
  - Players can sit at the Crypt fire exit.
  
### _HARVEST MOONS CHANGES:_

- Rebalanced moon spawn curves.
- Fixed missing textures on Sierra.
- Significant changes to Sierra's layout and visuals.
- Added more foliage to Fray.

### _WESLEY INTERIORS CHANGES:_

- **General:**
  - Fixed custom apparatuses becoming regular apparatuses after reloading the lobby.
- **Toy Store:**
  - Fixed enemy pathing issues on entry tiles in specific areas.
  - Shotgun traps now fling bodies and can damage enemies.
  - Chest traps:
    - Increased damage and blast radius.
    - Now use a burnt body type and can hurt enemies.
  - Apparatus pull event now deals 100 damage (up from 99).
  - "Welcome home" trap wire made harder to detect.
  - Adjusted damage ranges:
    - Shotgun traps: 5-8 damage.
    - Welcome home traps: 2-4 damage.
  - Fixed inverse softlock zones.
- **Grand Armory:**
- Ceiling crushers now cause head explosions.
- Elevator can crush players.
- Hooks deal 25 damage.
- Lurkers:
  - Inflict recurring damage (10 dmg/0.2s).
  - Can now harm enemies.
- Fixed persistent recharge station particles.
- Prevented instant deaths from teleporting onto acid pools.
- Teleportation exploits around acid pools fixed for enemies.
- Acid falls no longer have body types.
- Off-mesh links added to unreachable areas.
- Acid pools now have variations based on level tags.
- Updated water color.
- Introduced baked beans as an acid variant.

### _XEN INTERIOR CHANGES:_

- Hopefully fixed a bug with monsters getting stuck.
- Fixed a bug with floating landmines.

### _STARLANCER MOONS CHANGES:_

- **General:**
  - Updated moons to version 64, incorporating new vanilla content for Auralis, Triskelion, and StarlancerZero.
  - Breakable trees mechanic discovered but will require a future update due to the need to manually replace terrain-painted trees.
- **StarlancerZero:**
  - Removed errant navmesh under rocks to prevent enemies from phasing through.
  - Adjusted reverb effects in the special area for a smoother audio experience.
  - Relocated doors for a fresh layout.

### _ADDED MODS:_

- Synthesis Dropship Replacer
- LevelMusicLib
- JesterScreamRevamped
- Fashion Company
- CorrectDeathPenalty
- BodyRemover
- NiceChat
- Better Shotgun_Tooltip
- ItemTertiaryUse Conflict_Solver
- SpectateDeathCause
- BetterSprayPaint
- ExtensionLadderFix
- SmartItemSaving
- Scrap Insurance
- MeteorShowerChance
- BuyRateSettings
- CozyImprovements
- JesterDoorSlam
- Moved Magnet_Switch
- ExtraEnemyVariety
- ReservedSprayPaintSlot
- LethalSettings
- NAudioLame
- MirageCore
- HungryCompany
- Lethal Weight_Fix
- SCP500
- UsualScrap
- PjonkGoose
- SnatchinBracken
- BetterScanVision
- BetterScanVision
- LCCutscene
- GraphicsAPI
- Diversity
- Zombies
- BigMouth
- LC Office
- LethalLevelLoaderAOORE Fix
- Chocolate Moons
- Aerona
- Teyn
- Jabiua
- Tomb
- EGypt Moon
- Crystallum
- Azure
- Nyx
- Orion
- Aquatis
- 97 Bomenoren
- SchalttafelMod
- Tauralis
- Vacuity Moon
- StandaloneBlindPup
- altMoons
- ZeimaltMod
- Atlas Abyss
- Cosmocos
- Oldred
- Polarus
- DistinctMoonVariety
- TonightWeDine
- LethalConstellations

### _REMOVED MODS:_

- XuMiscTools *[ Deprecated ]*
- StarshipDeliveryMod
- SaveOurLoot
- GorgonzolaEnemyVariety
- MoonUnlockUnhide
- BarchLib
- Selenes Choice
- SoberShip
- KickWithoutBan
- HideChat
- Haunted
- NuclearLibrary
- Symbiosis
- True Darkness
- ScaledFallDamage
- JetpacksCarryBigItems
- SignalTranslatorAligner
- ut99 Interiors
- ShipColors
- LethalStats
- FreeBirdTotemRemixJester
- ShockwaveDroneEnemy
- Remnants
- Junic *[ M ]*
- Synthesis *[ M ]*
- Gorgonzola *[ M ]*
 
### _UPDATED MODS:_

- TooManyEmotes *2.2.6 > 2.2.7*
- TooManyEmotesScrap *1.0.7 > 1.0.8*
- LethalNetworkAPI *3.3.0 > 3.3.1*
- MoreCounterplay *1.3.1 > 1.4.0*
- LethalFixes *1.2.1 > 1.2.3*
- StarlancerEnemyEscape *2.5.3 > 2.5.5*
- ReservedItemSlotCore *2.0.36 > 2.0.37*
- StarlancerAIFix *3.8.2 > 3.8.3*
- Matty Fixes Experimental *1.1.21 > 1.1.24*
- CountryRoadCreature *1.0.6 > 1.0.7*
- TerrasRebalance *1.1.0 > 1.2.0*
- WesleysInteriors *1.3.11 > 1.4.6*
- Generic Moons *5.3.0 > 5.3.1*
- Seichi *0.8.7 > 0.9.0*
- CullFactory *1.3.12 > 1.3.15*
- Lategame Upgrades *3.10.l > 3.10.3*
- JetpackFixes *1.4.5 > 1.5.0*
- ButteryFixes *1.10.1 > 1.10.2*
- FurnitureLock *1.3.1 > 1.3.2*
- Seichi *0.9.0 > 0.9.2*
- StarlancerEnemyEscape *2.5.5 > 2.5.7*
- WeatherRegistry *0.1.23 > 0.1.25*
- HarvestMoons *1.4.8 > 1.4.9*
- LethalResonance *4.7.1 > 4.7.4*
- Interactive Terminal API *1.1.3 > 1.1.4*
- Football *1.1.8 > 1.1.10*
- EnemySkinRegistry *1.4.4 > 1.4.6*
- Extend Deadline *1.0.1 > 1.0.2*
- TooManySuits *1.1.2 > 2.0.0*
- OpenBodyCams *2.3.1 > 2.4.3*
- EnemySoundFixes *1.5.9 > 1.5.10*
- VileVendingMachine *1.1.1 > 1.1.2*
- HarvestMoons *1.4.9 > 1.4.10*
- YesFox *1.0.7 > 1.0.9*
- LethalConfigh *1.4.2 > 1.4.3*
- NestFix *1.0.2 > 1.1.0*
- OpenLib *0.1.8 > 0.2.2*
- DarmuhsTerminallStuff *3.5.10 > 3.6.0*
- FacilityMeltdown *2.6.19* > *2.6.20*
- Mirage *1.8.2 > 1.10.0*
- Oxygen *1.6.1 > 1.6.3*
- ShipColors *0.2.1 > 0.2.3*
- StarlancerMoons *2.3.3 > 2.4.0*
- Xen Interiors *0.3.2 > 0.3.3*
- BlahajPlush *1.0.3 > 1.1.0*