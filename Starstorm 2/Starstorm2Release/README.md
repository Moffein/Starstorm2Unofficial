# Starstorm 2 Unofficial

Unofficial update to the Pre-SotV version of Starstorm 2. **[Get the official version here!](https://thunderstore.io/package/TeamMoonstorm/Starstorm2/)**

Executioner and Nemesis Commando are based on the version from the pre-SotV build with some tweaks, while Cyborg/Chirr/Pyro have been re-coded from the ground up due to the old code being unworkable.

This mod can be used alongside the official version of Starstorm 2. Be sure to go through your configs to disable duplicated features.

I've gotten permission to work on this project and host it here, and permission to upload it to Thunderstore as a hidden mod.

The SS2 team is free to use any of the new code from this mod as long as this repo is allowed to stay up and credit is given.

## Features

- New Survivors:
  - Executioner
  - Cyborg
  - Chirr
  - Pyro
  - Nemesis Commando
  
 - Storms
 
 - Nemesis Invasion Event (triggered after completing Void Fields)
 
 *Items/Equipments are currently disabled.*
 
 *If unlock achievements aren't working, you can force unlock them in the config.*

## Changelog

`0.13.0`

- Chirr
	- General
		- Fixed secret ending text being broken.
		- Adding ending text for Mastery skin.
		
	- Sanative Aura
		- Increased heal over time from 30% -> 75% to match SS1.
		- Reduced cooldown from 15s -> 12s
		
	- Natural Link
		- Allies no longer passively absorb damage.
		- Increased cooldown from 3s -> 10s
		- Leashing allies now causes all non-Champion (boss) enemies to target them temporarily.
		
	- Allies
		- Significantly reduced aim delay.
		- Removed old stat scaling.
		- New stat scaling:
			- HP scales with AmbientLevel
			- Skill damage scales with AmbientLevel
			- Base damage scales with Player Level
			- 1.25x HP/Skill Damage multiplier on top of this.
		- Reduced knockback taken by 75%.
		- Added a 0.5s grace period between damage taken. (Excludes DoTs)

- Chirr (todo)
	- Ping stuff.
	- Balance.

`0.12.9`

- Nemesis Invasion
	- Added Move Speed Cap config option to prevent boss move speed from passing a specified threshold. (Disabled by default)

`0.12.8`

- Executioner
	- Fixed giant Royal Capacitor display.

`0.12.7`

- Nemesis Invasion
	- Fixed internal Nemesis stat items getting removed by the item blacklist.