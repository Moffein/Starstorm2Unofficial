# Starstorm 2

## Overview

Starstorm 2 is a work-in-progress RoR2 port of Starstorm, a diverse gameplay and content mod for Risk of Rain 1. It features two new survivors, over 20 new items and equipment, and a new, dangerous event to shake up your runs.

More information on Starstorm, including features and content to look forward to in SS2, can be found in its info section at https://rainfusion.ml/ or its wiki at https://starstormmod.fandom.com/wiki/Starstorm_Mod_Wiki. For more information on SS2, including development updates and an outlet for feedback and bug reports, visit our Discord server at https://discord.gg/starstorm2.

## Features

![](https://i.imgur.com/eLUJ3Z6.png)

* **Executioner** - An aggressive, versatile survivor with an arsenal made for picking off targets and chaining kills. His powerful ion gun gains a shot for every enemy you take down.

* **???** - A survivor of unknown origin, familiar yet unfamiliar. Under the right conditions, you may meet him on the planet, but he certainly won't be on your side.

![](https://i.imgur.com/c0GoQOf.png)

* **New items** - 18 new items and 4 new equipment to support new and existing playstyles. Strengthen your crits with the Erratic Gadget, become a stationary powerhouse with the Hunter's Sigil, or gain some vertical thrust on demand with the Pressurized Canister - and keep an eye out for the elusive Sibylline tier of items!

![](https://i.imgur.com/vGTL8l9.png)

* **Typhoon** - The planet's ultimate challenge awaits those who dare...

* **Storms** - When the skies cloud over, be on your guard. These occasional bouts of bad weather provide enemies with dangerous stat boosts.

* **The Void** - Those who find themselves travelling to the planet's hidden realms may accidentally leak some of those realms back onto Petrichor V...

* **Void Elites** - These otherworldly elites gradually dampen your movement speed and jump height if you linger inside their spheres of influence. Ordinarily they begin spawning after looping back to stage 1, but beware - if you have a propensity for dimension-hopping, you may meet them earlier, alongside another unwelcome visitor...

## Credits

* Code -  Anreol, Flan, KevinFromHPCustomerService, Moffein, Nebby, rob, swuff★, Shared, TheTimesweeper, xpcybic, Xubas
* Art/Modelling/Animation - Bolty, bruh, Cexeub, chugu, JestAnotherAnimator, Gem, LucidInceptor, Plexus, PapaZach, QuietAnon, redacted, rob, Spenny, swuff★, xpcybic, Reithierion, UnknownGlaze
* Writing - Blimblam, Lyrical Endymion, QandQuestion, swuff★, T_Dimensional
* Sound - Neik, UnknownGlaze
* Additional support/special thanks - Shooty, don, Draymarc, Gnome, KomradeSpectre, MinimalEffort, Moshinate, Neik, Riskka, Altzeus, JesusPxP, Vale-X

## Known Issues

The most critical issues, and their workarounds if any, will be added to this list as they are discovered. Other issues can be found on our Discord server in #ss2-known-issues.

* Typhoon icon is a white square.
* SS2 survivors do not have item displays for new vanilla items (e.g. Planula).
* Visual effects relating to Void Fields are not visible to non-hosts in multiplayer.
* Stirring Soul counts as a boss-tier item and can be used in overgrown 3d printers.
* SS2 survivors get stuck mid-animation when using some skills while getting frozen. This is cosmetic and will usually fix itself.

## Changelog

**Warning: content spoilers below!**

### 0.3.24
* General
	* Ported the mod to ThunderKit (Dev Note: This is mostly an internal change, but sadly, will reset all unlocks. This shouldn't happen again - we promise. On a side note, some details may have been lost in transition - let us know if something feels off and isn't listen in the patch notes)
	* Updated Typhoon's icon
* Events
	* Rewrote the system that handles storms - now allowing for multiple events at once, events of differing rarity, and an API for other modders to add their own events
	* Updated the post procesing for storms to fade in / out nicely, and overall look better. There's wind and other cool fancy shit. Crazy
	* The intensity of storms changes visually based on difficulty. Expect Drizzles or Typhoons based on your gameplay settings
	* Fixed an issue where storms overwrote enemy stats entirely (this fixes enemies being unslowable, notable with REX's rooting)
* Executioner
	* Touched up base model to address anatomy issues and add detail
	* Service Pistol: Slightly increased fire rate
	* Service Pistol: Removed damage falloff
	* Ion Burst: Now has a passive recharge, on top of gaining stocks on kill
	* Execution: Rewritten entirely. When leaping, Executioner's velocity now increases / decreases, rather than snapping up and down
	* Execution: Damage scales based on velocity while falling - H3AD-5T v2 fans rejoice!
	* Execution: Removed secondary hitbox during the latter half of the attack that was attached to the axe and didn't trigger the slam
	* Execution: Cooldown increased to 10 seconds
	* Fear debuff now makes it harder to aim at the inflictor, applies to both players and enemies
	* Redid visual effects - keep an eye out for his shiny new axe!
* Nemesis Commando
	* Single Tap: Now fully reloads, rather than reloading six shots at a time
	* Updated debuff icon for Gouge
* Items (Dev Note: This is the first half of item reworks, with the rest planned to come with 0.4.0. If an item hasn't been touched up, expect it then)
	* Coffee Bag: Updated textures
	* Detritive Trematode: Updated model
	* Dormant Fungus: Health gained is now flat rather than % based
	* Dormant Fungus: Updated textures and added new visual effect of trailing fungus while active
	* Malice: Damage now stacks hyperbolically rather than linearlly
	* Hottest Sauce: Updated initial damage burst from 150% to 200%
	* Hunter's Sigil: Effect now lingers for a short moment when beginning to move after activating the effects. Length of this period is based on stacks of the item held
	* Hunter's Sigil: Updad visual effects. Sorry, Sekiro fans
	* Prototype Jet Boots: Now stuns on hit
	* Prototype Jet Boots: Stacking now increases radius
	* Watch Metronome: Buff now builds slowly when walking, and builds quickly when standing still
	* Watch Metronome: Speed boost is only spent while sprinting
	* Droid Head: Drones now scale damage and attack speed with stacks
	* Droid Head: Drones now inherit the elite type from slain elites
	* Droid Head: Gave the spawned drones a unique identity - meet the Security Drone!
	* Erratic Gadget: Extra crit damage is now applied to the crit itself, rather than as a separate hit (crits now do 150% rather than 100% + 50%, the old behavior)
	* Nkota's Heritage: Now only gives Uncommon (Green) or Rare (Red) items past Level 13, and only gives Rare (Red) items past Level 26
	* Nkota's Heritage: Earned items are held until the next stage if the teleporter is fully charged, to prevent them from being lost while changing stages
	* Cloaking Headband: Updated model
	* Greater Warbanner: Cooldown reduction now stacks with other CDR such as Alien Head or Purity
	* Greater Warbanner: Updated model and visual effects
	* Relic of Mass: Now increases movement speed cap to double your maximum movespeed to help emulate Starstorm 1 behavior
	* Stirring Soul: Odds of receiving an item now start at 0%, with every unsuccesful roll adding 0.5% to the odds of gaining an item, up to 20%. Odds reset to 0% on successful roll.
	* Stirring Soul: No longer affected by luck increasing / decreasing effects 

### 0.3.23
* General
	* Added challenge: "MUL-T: Grand Mastery"
	* I don't know why 0.4.0 released either
* Executioner 
	* Fixed massive visual effects when using Execution
	* Fixed gaining permanent supercharge when killing AWU/Direseeker with Execution
	* Killing Aurelionite and Brother Glass now grants temporary supercharge
	* Updated description texts to reflect recent changes to Secondary
* Nemesis Commando
	* Updated various animations
	* Fixed typo in character description
	* Sword swing visuals are now blue when using the Commando skin
	* Single Tap: Reduced damage on hit (320% -> 210%)
	* Single Tap: Now has six stocks by default (previously one)
	* Single Tap: Added slick new reload animation. Very fancy.
	* Decisive Strike (Scepter): Now stuns all affected enemies
	* Decisive Strike (Scepter): Updated visuals
	* Gouge debuff is no longer applied if the hit is blocked (e.g. Tougher Times)
	* Gouge damage decreased by 75% when applied to players (Dev note: Getting hit by Decisive Strike felt like a bit too much of a death sentence)
* Items
	* Detritive Trematode: Now no longer applies debuff when hitting an enemy with an attack that has a proc coefficient of 0
	* Hottest Sauce: Updated sound effects
	* Prototype Jet Boots: Updated sound effects
* Void Event
	* Nemesis boss theme now loops properly
* ss_test
	* Cyborg: Added "Cyberstream" skin
	* Cyborg: Added "Metamorphic" skin (Dev note: these skins will eventually become rewards for Cyborg's Mastery and Grand Mastery challenges, respectively. They've both been left unlocked by default for testing reasons.}

### 0.3.22
* General
	* Fix Shrines of the Mountain not spawning additional bosses
* Executioner
	* Added animations for Heresy set of items
* Nemesis Commando
	* Added animations for Heresy set of items
* Void Event
	* Fixed new soundtrack not playing for clients in multiplayer

### 0.3.21
* Void Event
	* Nemesis survivors now have a unique boss theme
	* Shrine of the Mountain no longer spawns multiple Nemeses, instead it buffs the stats of the one that spawns
	* Nemesis survivors now have a much larger item blacklist
	* Greatly increased the spawn weight of Nemesis survivors
* ss_test
	* Cyborg: Fixed missing text strings
	* Chirr: Fixed missing text strings
* Cursed Config
	* Now disabled by default

### 0.3.20
* General
	* Updated mod icon - now in HD!
	* Added another new config option, the "cursed" config. Behind this config are various comedic skins, shitposts of features, or other things that we felt were worth making the time to make, despite them not fitting in within the core mod. Here be dragons!
* Nemesis Commando
	* Single Tap and Commando skins are now locked by default
	* Updated appearance of Commando skin
	* Added new challenge: Nemesis Nemesis
	* Decisive Strike is now locked by default
	* Added new challenge: Zandatsu
* ss_test
	* Added new survivors: Chirr and Cyborg
	* Nucleator: Added complete item displays for Vanilla items
* Cursed Config
	* Added new "Motivator" skin for Nemesis Commando
	* Old Nemesis Commando "Commando" skin is now behind this config option

### 0.3.19
* Executioner
	* Fixed softlock when dying on Glass
	* Added new survivor icon
	* Improved cloth physics for skirt
	* Overhauled all visual effects
	* Updated icon for Fear debuff
	* Fixed aim animation being off center
	* Added landing animation
* Nemesis Commando
	* Lowered recoil on Single Tap
	* Added new survivor icon
	* Overhauled sword swing visual effect

### 0.3.18
* General
	* Fixed various descriptions and name tokens being incorrect
	* Fixed a typo in the config
	* Tweaked the volume of various sound effects to prevent future hearing loss lawsuits
	* Fixed massive item displays... Again
	* Fixed the whole mod breaking when a survivor was disabled
* Executioner
	* Landing kills with Execution now gives double the charges of Ion Burst (was previously one extra)
	* Execution now has a maximum height, to prevent teleporting out of the top of stages
	* Properly networked bonus secondary charges from Execution
	* Fixed buggy overshield visuals
	* Fixed being able to cancel any skill with Crowd Dispersion
	* Fixed Forgive Me Please not working
* Items
	* Equipment items exist again
* ss_test
	* Executioner: Properly networked Deadly Voltage primary
	* Executioner: Raised proc coefficient of Deadly Voltage (.3 -> .75)
	* Executioner: Increased range of Deadly Voltage (20 -> 28)
	* Nucleator: Properly networked all skills
	* Nucleator: Fixed charge meter vanishing when sprinting or using certain skills
	* Nucleator: Word-for-word from Rob: "updated vfx i think, not sure"

### 0.3.17
* General
	* Added "ss_test" config option to enable certain work in progress features. As of now, this enables the Nucleator, a new survivor who can risk his own health for massive damage output, and the "Deadly Voltage" primary skill for Executioner. Note that these features, as well as future features locked behind this config, may be buggy or notably unpolished compared to other content in the mod, and as such, are recommended for experienced users only.
	* Fixed unlockables
* Executioner
	* Added a new visual effect when gaining charges of Secondary
	* Damaging an enemy within five seconds of its death grants a kill assist, adding a charge of secondary, regardless of who landed the finishing blow
	* Executioner now gains bonus charges of Secondary when defeating tougher foes
	* Secondary now becomes "supercharged" for ten seconds after defeating the Alloy Worship Unit or Direseeker (added by "Direseeker" mod), and permanently after defeating Mithrix. While supercharged, the ability has unlimited stocks. (Dev note: There was no need for this other than it's cool)
* Nemesis Commando
	* Added sound effect on character select screen
	* Updated sound effects for Distant Gash
	* Single Tap (alternate secondary) no longer interrupts Primary skill
* Items
	* Most items can now have various numerical values changed via config file
	* Hunter's Sigil: Updated sound effect
	* Malice: Properly cleans up VFX, should help with stuttering / lag issues
	* Pressurized Can: No longer breaks the console if given to an equipment drone
	* Prototype Jet Boots: Now have a config setting to reduce or disable visual effects

### 0.3.16
* General
	* Disable Void elites while issues with custom elite spawns are being fixed
	* Fix character select screen being empty if any SS2 survivors are disabled via config
* Nemesis Commando
	* Updated sprint animation
	* Added out-of-combat sprint animation (only applies to Minuano skin for the moment)
	* Lowered the recoil from Distant Gash
	* Fixed Distant Gash having inconsistent damage
	* Doubled the hit count and increased the frequency of hits for Decisive Strike with Ancient Scepter
	* Tweaked Decisive Strike for the boss version of Nemesis Commando: now has a visual indicator, now always fully charges the attack, stays stationary for the entire attack
* Items
	* Fix giant item displays on Aetherium items and Ancient Scepter
	* Greater Warbanner: fix missing buff icon, change indicator material to be distinct from regular warbanners
	* Hunter's Sigil: fix networking on effects
	* Malice: fix effects appearing on player instead of enemies
	* Diary: fix Engineer turrets gaining exp from the item

### 0.3.15
* General
	* Fix Void elites not spawning correctly, creating "fake elites" with health boost
	* Fix Void elites spawning on Commencement
* Executioner
	* Enemies afflicted with Fear now take 50% more damage (was 20%)

### 0.3.14
* General
	* Compatibility with the Anniversary Update
	* Text strings are no longer in a separate language file
	* Adjust SS2 survivor/item sorting order in character select and logbook
* Items
	* Cloaking Headband: reduce cooldown (90sec -> 45sec)

### 0.3.13
* General
	* Fix enemy damage not scaling with level during storms
* Items
	* Coffee Bag: adjusted stats to be in line with Risky 2's balancing (10% -> 7% move speed, 7% -> 7.5% attack speed)
	* Dormant Fungus: reduce stacking behavior (1.5% per stack -> 2% + 0.5% per stack)
	* Hottest Sauce: change stacking behavior to increase damage instead of duration, increase initial burst damage (100% -> 150%)
	* Hunter's Sigil: hide buff effect when near the camera, add new sound for buff
	* Pressurized Canister: only applies upward force if you hold jump instead of constantly for the whole duration. Config option added to make it behave like prior releases if you, like Rob, thought that was hilarious.

### 0.3.12
* General
	* Fix a certain unfinished elite type being able to spawn when using some modded artifacts
* Nemesis Commando
	* Added Ancient Scepter support
* Items
	* Publicize item/equipment classes so other modded characters can add display rules for SS2 items
	* Hunter's Sigil: make buff visual smaller
	* Green Chocolate: add a damage reduction upon taking heavy damage
	* Green Chocolate: fix 20% max HP threshold not factoring in shield
	* Green Chocolate: update model/icon
	* Stirring Soul: increase lifetime on souls before they despawn (10s -> 20s)
	* Stirring Soul: blacklist item from being given to late joiners if DropinMultiplayer is in use

### 0.3.11
* Executioner
	* Increased Primary damage (280% -> 300%)
	* Added new skin inspired by Altzeus and JesusPxP's Wastelander mod
	* Added new unlock for Wastelander skin
	* Fix being able to unlock Executioner by destroying rocks on Sky Meadow
	* Updated skill icons
* Nemesis Commando
	* Full visual effects pass
	* Altered visual effects on sword for Grand Mastery skin
	* Added unique sound effects for Grand Mastery skin
	* Added unlock condition for Commando skin
	* Updated Commando skin
	* Added unique visual & sound effects for Commando skin
	* Significantly reduced Submission's knockback
	* Changed scaling for Nemesis Commando as a boss
	* Blacklisted certain unfair / unfun items from boss Nemesis Commando
	* Tweaked hitstop to help with mobility issues
	* Networked sound effects
* Items
	* Stirring Soul: reduced odds of item drops (7% -> 5%)
	* Stirring Soul: fix Sky Meadow rocks dropping souls
	* Stirring Soul: added item displays
	* Broken Blood Tester: added item displays
	* Coffee Bag: added item displays
	* Dormant Fungus: added item displays
	* Droid Head: added item displays
	* Fork: added item displays
	* Hottest Sauce: added item displays
	* Hunter's Sigil: tweaked sound effects
* Other
	* Fixed storm ambience volume / looping
	* Refactored code to allow other mods to add their own Nemesis bosses to void event
	
### 0.3.10
* Executioner
	* Added "Host Only" to unlock description
	* Fixed special immediately snapping to ground
	* Updated visuals while carrying Visions of Heresy
	* Axe is now invisible while carrying Shattering Justice
* Nemesis Commando
	* Nerfed Distant Gash damage (200% min - 800% max -> 160% min - 500% max)
	* Cleaned up visual effects
	* Gun is now holstered and crosshair is changed when running an all-sword loadout
	* Fixed compatibility issue with Phase Round Lightning mod
	* Changed gunshot sound effect
	* Tactical Roll has a shorter cooldown (6s -> 5s), and cooldown begins immediately, rather than after the skill is performed in full
	* Fixed some sound effects being played globally
* Items
	* Molten Coin: added sound effects
	* Molten Coin: now properly scales with proc coefficient
	* Diary: added sound effects
	* Hottest Sauce: added sound effects
	* Detritive Trematode: debuff length is now based on proc coefficient
	* Prototype Jet Boots: updated visual and sound effects
	* Hunter's Sigil: visual effects now properly networked
	* Stirring Soul: added sound effects
* Void Elites
	* Fixed Void Elite debuff permanently applying in multiplayer widepeepoHappy
* Other
	* Added storm ambience during storms

### 0.3.9
* Executioner
	* Execution's drop can no longer be overridden by Milky Chrysalis
* Nemesis Commando
	* Add unique visual effect to boss variant
	* Lower base damage on primary (200% -> 160%)
	* Tweak Decisive Strike visuals
	* Increase base damage on Decisive Strike (375% -> 380%)
	* Fix incorrect damage values on Submission
	* Change new secondary name to Single Tap
	* Fix Single Tap not working in multiplayer
	* Fix core position being too high up
	* Fix camera angle being locked on spawn
* Items
	* Hunter's Sigil buff now lingers for 0.75s after you start moving
	* Add visual efffect and sound for Hunter's Sigil activation
	* Raised Cloaking Headband's cooldown (45s -> 90s)

### 0.3.8
* Executioner
    * Tweaked sound volume
    * Lower Execution jump height
    * Add crit sound to primary
    * Remove proc coefficient from utility
    * Lower utility fear radius (25m -> 20m)
    * Fix cloth physics on mastery skin
* Nemesis Commando
    * Gouge stacks now refresh duration like bleed
    * Lower base damage on primary (220% -> 200%)
    * Rework Submission to a chargeable shotgun blast
    * Added an alternate secondary, Phase Shot
    * Update overview text to change secondary name
    * Fix keywords on Submission and Decisive Strike
    * Fix excessive hitstop on primary
    * Fix primary not dealing damage with too much attack speed
    * Update sprint animation
    * Added and tweaked sounds for everything
    * Boss variant now sprints
    * Adjusted camera position on spawn

### 0.3.7
* General
	* Fix no monsters spawning on any run after the first
* Items
	* Remove incomplete Baby's Toys

### 0.3.6
* General
	* Fixed custom sounds not working
	* Registered some skill states that may or may not have been causing networking issues
	* Made Typhoon's text about increasing enemy cap only display if setting is enabled in config
* Executioner
	* Added an umbra for the Artifact of Vengeance
	* Added work in progress rest emote
	* Added taunt emote (bound to 2 by default)
	* Tweaked Execution animation
	* Adjusted cloth physics, no linger clips through body
	* Execution is now agile and moves faster while ascending
* Nemesis Commando
	* Added an umbra for the Artifact of Vengeance
	* Added taunt emote
	* Added footstep sounds to sprint
	* Increase base damage on primary (160% -> 220%)
	* Increase max damage on secondary (60% -> 800%)
	* Reduce base charge time on secondary (1.5s -> 1.25s)
	* Increase cooldown on secondary (3s -> 4s)
	* Secondary cooldown now starts when the skill starts rather than ends
	* Updated jump animations

### 0.3.5
* General
	* Fixed config not properly disabling survivors

### 0.3.4
* Executioner
	* Updated description of Fear for accuracy (enemies take 20% more damage while afflicted)
	* Primary bullet size slightly increased
	* Secondary can now be held down to fire more than 10 shots
	* Using Special doesn't cut velocity as abruptly
	* Update sound on primary
	* Fixed sounds not playing for some players
	* Fixed strange shield overlays
* Nemesis Commando
	* Retimed sitting emote to not play at hyperspeed
	* Fixed sounds not playing for some players
	* Fixed strange shield overlays
	* Updated description of Submission for accuracy
	* Updated description of Decisive Strike for accuracy
* Enemies
	* Add sound for spawning Void Elites

### 0.3.3
* General
	* Fixed issue with Nemesis Commando automatically unlocking for some players

### 0.3.2
* General
	* Fix an issue causing an ESO-related error and possibly preventing Void elites from spawning in some cases
* Enemies
	* Add item blacklisting to Nemesis Commando boss
	* Nemesis Commando boss is given roughly half as many items as before and is no longer given unique boss damage boost (he is still very dangerous to be near)
* Executioner
	* Increase base damage on primary (240% -> 280%)
	* Reduce base fire rate on primary (0.5s -> 0.65s)
	* Secondary fires a maximum of 10 shots at a time, even if more are stored via Backup Magazines
	* Increase base/maximum damage on special (1000%/1600% -> 1100%/2200%)
	* Special grants additional secondary restock on kill instead of reducing cooldowns
	* Adjust health and damage scaling very slightly
	* Update animations and VFX
* Nemesis Commando
	* Secondary projectile speed scales with charge
	* Update description on Submission (default special) to better reflect what it does
	* Update animations
	* New sword sounds
* Items
	* Re-enable Green Chocolate. Buff lasts longer and can stack.
	* Add Relic of Mass lore
	* Dormant Fungus: reduce healing (2% -> 1.5%)
	* Fork: reduce damage bonus (10% -> 7%)
	* Diary: adjust scaling to give less exp at low levels and a bit more at higher levels
	* Prototype Jet Boots: enemies no longer hurt themselves when using the item
	* Malice: reduce damage scaling on stack (each additional enemy hit takes 55% of the previous enemy's damage)
	* Droid Head: increase drone damage and movespeed; stacking now scales drone damage instead of duration

### 0.3.1
* Fix missing ESO dependency

### 0.3.0
* *First Thunderstore release*
* Gameplay
	* Add storms and run-wide event system. Storms are more frequent and have shorter forewarning at higher difficulties.
	* Add Void elites (tier 2)
	* Visiting Void Fields at any point during a run makes Void Elites start appearing early and releases Commando's vestige into our reality...
	* Removed Herobrine
* Executioner
	* Add unlock condition
	* Add mastery skin
	* Add grand mastery skin, obtainable by winning/obliterating on Typhoon difficulty
	* New model and animations, based more closely on Starstorm's Executioner
	* Gun glow now reflects number of stored secondary charges
	* Update item displays to fit new model
	* Reworked base/maximum damage on special (1200%/2400% -> 1000%/1600%)
	* Fix special causing self-damage/utility causing self-fear when Artifact of Chaos is active
	* Add remaining item displays (if any are missing, yell at me -swuff★)
* Nemesis Commando
	* Add unlock condition
	* New mastery skin (old one is available by default)
	* Add grand mastery skin, obtainable by winning/obliterating on Typhoon difficulty
	* Update sounds and visuals, especially on Decisive Strike
	* Update description
	* Add remaining item displays (same as with Exe -★)
* Commando
	* Add grand mastery skin
* Items
	* Add Relic of Mass (lunar) and Stirring Soul
	* Internal item code rewrite to support item displays for our items
	* Add per-item on/off toggles to config
	* Diary: scale up exp gain with level
	* Molten Coin: update icon
	* Hottest Sauce: deals 100% base damage before applying burn; fix lighting yourself on fire when Artifact of Chaos is active
	* Nkota's Heritage: adjust rarity scaling to work similarly to RoR1 Starstorm, add chance for red items, prevent Engineer turrets from dropping items
	* Erratic Gadget: add non-stacking 10% crit chance
	* Greater Warbanner: only one banner may be placed at a time (previous banner is removed when a new one is placed)

### 0.2.4.3
* hotfix for nemmando networking for real this time

### 0.2.4.2
* hotfix for networking
* fix for strides of heresy breaking on nemmando

### 0.2.4.1
* Nemesis Commando
	* Fixed networking issues
	* Lowered Decisive Strike (Special) charge time from 2s to 1.75s and buffed damage per hit from 350% to 375%
	* Removed auto-aim and Awareness buff on Submission, added proper attack speed sclaing.

### 0.2.4
* Executioner
	* Primary is slower but more powerful
	* Special now deals 1200% flat on groups, and 2400% to solo targets (from 1500% to solo target, split between # of targets)
* Nemesis Commando
	* New animations for pretty much everything
	* Primary has had damage / attack speed adjustments, hitbox changes, is now a one-two combo like Loader's
	* Secondary is now a proper sword beam, visually
	* Secondary now applies Gouge
	* Adjusted interrupt priorities for abilities to make them flow together better
	* New alternate special ability, WIP.

### 0.2.3
* Nemesis Commando
	* Fix sword combo not resetting when not using primary
	* Fix slightly bad aim on special
	* Fix gouge applying twice
	* Adjust skill priorities so other skills can be used while holding primary attack
* Executioner
	* Add some more item displays
* Items
	* Diary: increase exp gain rate (3 per 3sec -> 3 per 2sec)
	* Molten Coin: scale earnings by number of stages cleared ($1 per stage)
	* Hottest Sauce: increase burn damage (50% -> 100% damage/sec)
	* Nkota's Heritage: chance of uncommon item scales with level
	* Pressurized Canister: fix poor thrust force when used with heavy characters e.g. Acrid
	* Update item descriptions to match item behavior
* Gameplay
	* Typhoon: increase soft cap on enemy spawns to 80. This is an experimental feature and can be disabled in config.

### 0.2.2
* Executioner
	* Fix bad aim on secondary
	* Add some item displays

### 0.2.1
* Executioner
	* Increase aim snapping on secondary again
	* Add display rules for equipment that needs them to work
* Nemesis Commando
	* Primary is now agile (can be used while sprinting)
	* All hits of primary apply gouge status
	* Gouge duration resets when reapplied
	* New secondary: a chargeable sword beam which behaves like Commando's Phase Round
	* Add more item display rules
* Items
	* Broken Jet Boots: change vfx to something less visually intrusive
	* Watch Metronome: increase base decay duration (2 -> 4sec)
	* Fix some typos in item logbook entries
	* Broken Blood Tester: disable proc while run timer is paused
	* Molten Coin: fix missing texture
* Other
	* Fix known multiplayer incompatibilities (e.g. Executioner's secondary not storing charges)

### 0.2.0
* Items
	* Add Malice, Fork, Coffee Bag, Watch Metronome, Hunter's Sigil, Green Chocolate, Droid Head, Pressurized Canister, and Cloaking Headband
	* Disable Nucleator until he is more finished
	* New icons for items that were missing them
	* Add logbook entries for items
	* Fix Molten Coin not working
	* Hottest Sauce: Increase range, fix missing stacking behavior, fix proc on unusable equipment e.g. fuel array
	* Change Dormant Fungus healing (10% per 5sec -> 2% per sec)
	* Fix oddities on some item models
	* Add debuff icons for Trematode, Strange Can
* Executioner
	* Utility procs fear for its entire duration
	* Activating utility causes stun and slight pushback
	* Slightly decrease fear proc radius on utility
	* Enemies under fear debuff take 20% more damage from all sources
	* Tone down particles/VFX on utility
	* Slightly increase aim snapping while using secondary (easier to land shots)
* Nemesis Commando
	* Add mastery achievement + skin
	* Adjust base stats to more closely match Commando's
	* Fix attacks having 0 proc coefficient
	* Add skill icons
	* Add descriptions for survivor and skills
	* Add logbook entry
	* Add sounds
	* Ragdoll, animation tweaks, some item displays, and lots of other quality-of-life details
	* Fix jogging in place on character select screen

### 0.1.0
* Initial release - includes Executioner, Nemesis Commando, Nucleator (WIP), 11 items, and typhoon difficulty
