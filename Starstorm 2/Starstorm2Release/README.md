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

`0.14.2`

- Added skill icon for Cyborg's Shock Core (Thanks Littera!)

`0.14.1`

- Chirr
	- Added config option to disable Minion ping targeting in case you want to use other mods that add this.

`0.14.0`

- Cyborg
	- Unmaker
		- Damage increased from 270% -> 300%
		
	- Rising Star
		- Damage increased from 240%-720% -> 300%-900%
		
	- Defense Matrix
		- Reduced base recharge time from 8s -> 7s
		- Increased meter increase from Backup Mags from +50% -> +100%
		- Increased base shield duration from 2s -> 3s
		- Removed cooldown reduction on block.
		
	- Shock Core
		- Reduced damage from 800%-1600% -> 600%-1200%
		- Reduced cooldown from 12s -> 7s
		- Moved to Secondary slot so that there's an option for people who don't want to use the shield.
		- Disabled self-knockback.
		- Can now instantly fire your primary after releasing the button.
		
	- Recall
		- Reduced cooldown from 12s -> 10s
		
	- Flight Mode
		- Reduced cooldown from 12s -> 10s
		
	- Overheat Redress
		- Reduced cooldown from 12s -> 10s
		- Hits per second increased from 5 -> 8
		- DPS increased from 700% -> 1200%
		- Full DPS time increased from 0s -> 1s
		- Max DPS falloff reduced from -90% -> -80%
		- Increased AoE range from 12m -> 15m
		- Increased pull force from 300 -> 600

- Executioner
	- New Primary: Standard Issue Pistol
		- It's the same as his default primary, except it fires 1 shot instead of 2 (still is 4 shots per second)
	- Special
		- Increased damage from 800%-2400% -> 1000%-3000%
		
- Nemmando
	- Decisive Strike
		- Increased slash damage from 300% -> 360%

`0.13.1`

- Nemesis Commando
	- Fixed camera getting permanently changed when using Decisive Strike Scepter.
		- Fixed other potential areas where this could happen with his specials.
	
- Chirr
	- Re-enabled void death immunity on allies.

`0.13.0`

- Chirr
	- General
		- Fixed secret ending text being broken.
		- Adding ending text for Mastery skin.
		
	- Sanative Aura
		- Increased heal from 25% + 30% -> 30% + 70% to match SS1's total healing amount.
		- Reduced cooldown from 15s -> 12s
		
	- Natural Link
		- Leashing allies now causes all non-Champion (boss) enemies to target them temporarily.
			- Ally periodically heals for every enemy distracted.
		- Leash now teleports your ally to where your crosshair is pointing.
		- Increased cooldown from 3s -> 12s
		- Allies no longer passively absorb damage.
		
	- Allies
		- When pinging enemies, maintains attention until it dies or you ping something else.
		- Significantly improved aim speed.
		- Redid stat scaling since the old system was incredibly convoluted.
		- New stat scaling:
			- HP/Damage scales with player level. (used to scale with Ambient Level)
			- +50% HP and +100% Skill damage for non-elites.
			- +100% HP and +200% Skill damage for elites (overrides elite stats)
		- Disabled Void Implosion immunity.
		- Enabled Grandparent Overheat immunity.
		- Reduced knockback taken by 90%.
		
		*Stats need more testing. Did some lategame runs and Golems seemed to be surviving fine even with the lower stat scaling. Earlygame allies seem to be a lot squishier.*
	
- Nemesis Invasion
	- Bosses no longer spawn in the bazaar (can be re-enabled in config).

`0.12.9`

- Nemesis Invasion
	- Added Move Speed Cap config option to prevent boss move speed from passing a specified threshold. (Disabled by default)

`0.12.8`

- Executioner
	- Fixed giant Royal Capacitor display.

`0.12.7`

- Nemesis Invasion
	- Fixed internal Nemesis stat items getting removed by the item blacklist.