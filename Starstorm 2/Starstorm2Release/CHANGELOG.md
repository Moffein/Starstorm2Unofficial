`0.22.0`

- Fixed Chirr/Cyborg skins for MemOp.

`0.21.23`

- Added another final boss to Chirr's befriend blacklist.

`0.21.22`

- Fixed Chirr being able to kill befriended final bosses.

`0.21.21`

- Fixed some Autoconfig stuff not disabling properly due to MemOp?

`0.21.20`

- Fixed Cursed config being broken due to MemOp.

`0.21.19`

- Attempted to fix MemOp config issue.

`0.21.18`

- Fixed for Memop, kinda.
	- Vanilla skins currently broken. I probably won't fix unless someone sends a PR.

`0.21.17`

- Chirr
	- Minions from her Secret get swapped for better-working versions when entering new stages.

- Watch Metronome
	- Reduced chargeup time from 6s -> 4s

`0.21.16`

- Fix for SotS Phase 3.

`0.21.15`

- Cyborg is now listed as a variant for SS2 Beta Cyborg.
- Added RU TL (Thanks N3xT!)

`0.21.14`

- Updated R2API DamageType implementation, removed deprecated components.

`0.21.13`

- Fixed TakeDamage nullrefs.

`0.21.12`

- Fixed for latest update.
	- Custom damagetypes will be broken until DamageTypeAPI updates.

`0.21.11`

- Added extra nullchecking to a niche part of Nemmando.

`0.21.10`

- Attempted to fix Storms erroring and permanently increasing enemy stats due to the last game update. Let me know if this issue persists.

`0.21.9`

- Recompiled for latest update.
	- Let me know if Chirr's thingy broke.

`0.21.8`

- Buffed Fork from +2.4 base damage -> +5% base damage

`0.21.7`

- Cyborg Defense Matrix now guards against Vagrant Novas.

`0.21.6`

- Attempted to fix incompat with SS2 Official music.
	- This is done by not loading SS2U's music soundbank if SS2O is loaded.

`0.21.5`

- Updated autoconfig to match latest SS2 Official plugin GUID.

`0.21.4`

- Nemmando is now automatically unlocked if Autoconfig is active and SS2 Official is loaded.

`0.21.3`

- Updated Autoconfig to use the latest SS2 Official plugin GUID.

`0.21.2`

- Fixed JSON error with Portuguese translation.

`0.21.1`

- Fixed Autoconfig always being active regardless of whether or not SS2 Official is installed.

`0.21.0`

- SS2 Official Autoconfig (Enabled by default, Untested)
	- If it detects SS2 Official is loaded, it'll automatically turn off Storms/Events, Void Invasion, Typhoon, and duplicated Items/Equipments that exist in the Official version.
		- The list of features disabled is manually set, and won't automatically update when SS2 Official updates.
	- Does not affect Survivors since they're already nested under Variants.
	- If you want to manually configure which features to keep, disable this option.

- Re-enabled Dormant Fungus by default. (Existing config unaffected)
	- It's a nice item to have in the pool, and can be considered different from Vanilla Wungus by having lower numbers but being usable alongside Bungus.
	
- Re-enabled Relic of Mass by default. (Existing config unaffected)
	- Acceleration reduction is funny, different enough from Stone Flux Pauldron to warrant keeping.
	

`0.20.9`

- Fixed TemporaryOverlay nullrefs on Executioner and other survivors.
- Helminth Roost now uses Ash Storm VFX.

`0.20.8`

- Reverted previous update since Vanilla is fixed now.

`0.20.7`

- Temporary fix for the mod breaking Maximum_Cope and similar Framerate fix mods.
	- Chirr's Minions now get a base damage multiplier instead of a skill damage multiplier as a result of this. This will be reverted once Vanilla is fixed.

`0.20.6`

- Chirr
	- Peak fiction.

`0.20.5`

- Chirr
	- Added False Son to Befriend blacklist.

`0.20.4`

- Storms
	- Reformed Altar now uses Dust Storm VFX.

`0.20.3`

- Update for DLC2
	- Needs testing.
	
- Executioner
	- Single shot is now default, double shot is alt.
- Chirr and Cyborg now use Artificer's hover setting.

- Added PT-BR TL (Thanks kauzok!)

`0.20.2`

- Fixed material settings on Commando's grandmastery.

`0.20.1`

- Fixed adjusted materials not fading at close-range.

`0.20.0`

- Now depends on ShaderSwapper.
- A bunch of material adjustments.
- Added dependency on Loader Slam Audio Fix until the bug is fixed in Vanilla.

- Fixed Executioner's selected primary not saving when restarting the game.

- Cyborg
	- Colossus skin now uses red lasers.
	- Increased Rising Star proc coefficient from 0.5 -> 0.7

`0.19.4`

- Survariants compatibility
	- Can be toggled in config.
	- Nemmando is listed under Commando.
	- Duplicated survivors with SS2 Official are listed as a variant. (Currently Executioner/Chirr)

- Updated Icons
	- Updated Strange Can and Greater Warbanner logbook icons.
	- New Strange Can buff icon.
	- Added transparency to Radiation buff icon.
	
- Cloaking Headband
	- Now gives a speed boost as well.

`0.19.3`

- Fixed Chirr's ally health modifier not applying earlygame (when player level > ambient level)

`0.19.2`

- Recompiled with new DLLs from the Devotion update.
- Fixed extra space after Shipping Method field in lore.

`0.19.1`

- Items
	- Strange Can
		- Increased minimum damage to follow a similar damage formula to Acrid's Poison. (Minimum is set at 100% x HP Percent base damage, so 2.5%HP = 250% base damage, 1%HP = 100% base damage, etc.)

`0.19.0`

*Credits to Thingw for the new icons!*

- Now uses language files.
- Item/Equipment lore now uses the original SS1 lore.

- Cyborg
	- New Passive: Energy Core
		- Cyborgs skills all draw from a shared Energy Pool instead of using cooldowns.
			- Takes 15s to recharge, scales off of cooldown reduction to each skill.
			- Extra skill stocks increase the max cap of the energy pool.
				- Secondary: +11.1%
				- Utility/Special: +33.3%
		- Cyborg can still hover by holding jump.
		
		*Hoping this new passive will help Cyborg feel more unique. Skills are mostly mechanically the same, but he now has a lot more flexibility in how/when he can use his skills.*
	
	- Primaries no longer slow on hit.
	- Special Skills can now be fired while Primary is being fired.
	
	- Alt Primary: Renamed to Piercing Shot
	
	- New Default Secondary: Rising Star
		- 30% Energy/s
		- Rapidly fire slowing shots for 3x140% damage. (0.5 proc)
			- Fires 5x per second.
			
		*The highest raw single target DPS skill in the kit, at the cost of consuming Energy very quickly.*
	
	- Defense Matrix
		- 7s cooldown -> 15% Energy/s
			- Increased shield uptime from 3s -> 6.67s
			
		*Shield uptime was balanced around frame-perfect defenses in solo, but ended up being too short to be useful for protecting teammates in multiplayer.*
		
	- Recall
		- 8s cooldown -> 50% Energy
		- Removed shock explosion.
		- Now stuns in a very small radius for 1000% damage when teleporting. (Hidden Stat)
		
		*No longer shocks, but now can be potentially used 2 times in a row.*
		
	- Flight Mode
		- 8s cooldown -> 60% Energy/s
		- Skill now lasts for however long you're holding the button.
		
	- Overheat Redress
		- 10s cooldown -> 60% Energy
	
	- Shock Core
		- Moved from Secondary slot to Special slot. (Reverting earlier update)
		- 7s cooldown -> 40% Energy
		- Reduced damage from 600% (was mislabeled as 800%) -> 400%
			- Implosion damage remains the same, 1200% (was mislabeled as 1600%)
		- Increased pull radius from 20m -> 30m
		
- Chirr
	- Friend loadout is now saved between stages. (Relevant for Hunted and other mods that let you befriend survivors)
	- Fixed TP boss befriending being locked behind Scepter due to an earlier update. (You'll still need Scepter to befriend Champions, but you should now be able to befriend Horde bosses)

- Executioner
	- Service Pistol now has a unique icon.
	
- Items
	- Strange Can
		- Minimum damage is now capped at 1.

`0.18.13` to `0.18.14`

- Attempted to fix a conflict with SS2 Official's Nemesis music. (Didn't work)

`0.18.12`

- Fixed Nucleator Radation damagetype being unregistered. Should fix certain attacks causing radiation to be applied.

`0.18.11`

- Fixed Nucleator Sit emote.

`0.18.10`

- Updated mod icon (Thanks DestroyedClone!)

`0.18.9`

- Nucleator
	- Updated anims (Thanks Spanish Space Inquisition!)
	- Increased health regen from 1.0 -> 2.5
	- Radiation Poisoning VFX now scales with body size.
	- Irradiate
		- Description now mentions the radius scaling.
		- Overcharge projectile now has prediction disabled, so its position won't be desynced for clients anymore.
		- Changed Overcharge VFX to a cleaner effect.
		
	- Quarantine
		- Reduced damage from 400%-1200% -> 200%-1080%
		- Increased root duration from 3s -> 5s
		
		*Now deals same damage as primary, since having this be a power skill felt unfitting.*
		
	- Fission Impulse
		- No longer stops movement. (Old version can be re-enabled in config)
		- Adjusted leap hitbox.
			- Enemy hitbox detection radius reduced from 4m -> 2.5m, now can detect non-flying enemies.
			
		*Goal is to make this more consistent at direct-hitting enemies when used offensively.*
	
- Nemmando
	- Gouge
		- Reduced proc coefficient from 0.7 -> 0.5
		- Reduced damage from 400% -> 300%

`0.18.8`

- Fixed some Nucleator sounds playing globally.
- Fixed most sounds not factoring in orientation for real this time.

`0.18.7`

- Enabled nemesis VFX on Nemforcer Invader.

`0.18.6`

- Fixed Nucleator's unfinished item display code causing double item pickups on certain mod setups.
- Fixed Nemmando modifying Executioner's itemdisplays.
- Achievements for disabled content are now hidden from the logbook. (They might still show up in other places though?)
	- This feature can be toggled in the config, in case it happens to break anything.

`0.18.5`

- Nucleator/Pyro/Cyborg
	- Fixed crosshair charge bar breaking after spectating other people playing the same character.

`0.18.4`

- Nucleator
	- Irradiate
		- Damage reduced from 360%-720% -> 200%-720%
		
		*Old starting damage was making mashing do way more damage than charging. New starting damage puts mashing DPS in line with normal/Overcharge DPS.*

`0.18.3`

- Pyro
	- Airblast
		- Reduced knockback force from 3000 -> 2800

- Pyro/Nucleator
	- Secondary knockback mass scaling against flying enemies now caps at 7.5x to reduce the likelihood of boss instakills. (Number used for HAN-D's knockback)

`0.18.2`

- Nucleator
	- Secondary force reduced from 3200 -> 2800
	- Special now uses a unique Radiation Poisoning DoT instead of reusing Acrid's poison.
		- Deals 5% max health (minimum 500% damage) over 5s. Is able to kill enemies.
		
- Strange Can
	- Now plays unique SFX when dealing damage.

`0.18.1`

- Fixed Strange Can, Detritive Trematode, and Gouge counting as 2 debuffs towards Death Mark.

`0.18.0`

- New Survivor
	- Nucleator
		
- New Items
	- Detritive Trematode
		- Enemies below 25% health are infested for 100% (+100%) base damage per second.
		
	- Strange Can
		- 8.5% (+5%) chance to intoxicate enemies, causing them to lose 2.5% of their current health every second for 5s.
		
	- Nkota's Heritage (Disabled)
		- Receive an item on level up or starting the Teleporter event. Rerolls for a higher item tier 0 (+1 per stack) times.
			- Item tier uses same chances as a small chest.
			
- Survivors
	- Executioner
		- Execution
			- Added mass scaling to downwards force.
			
	- Nemmando
		- Gouge no longer triggers Bands when stacked.			

- Items
	- Droid Head
		- Increased max drone count from 3 -> 4
		
- Equipment
	- Greater Warbanner
		- Added placement sound.
		
- Fixes
	- Fixed most sounds not factoring in orientation.
	- Storm sounds now play globally, instead of originating from a specific point on the map.
		
`0.17.2`

- Chirr
	- Added failsafe for when Scepter fails to replace her Special.
		- Befriend now checks if your inventory has a Scepter if it doesn't find the Scepter skill. Skill icon won't change, but you'll be able to befriend bosses.

`0.17.1`

- Nemesis Invasion
	- Fixed Require Void Fields Completion not working. (It was setting the "Use Void Team" option instead)

`0.17.0`

- Chirr
	- Disabled flying autosprint due to being jank.

- Nemesis Invasion
	- Added config option to allow invasions to start even if you didn't fully complete Void Fields. (Disabled by default)
	- Increased per-player HP Multiplier from +30% -> +50%
	- Nemesis Commando
		- Reduced HP from 3800 -> 3600
		- Now attempts to drop SS2 Official Stirring Soul if SS2U Stirring Soul is disabled.
	- Nemesis Enforcer
		- Buffed HP from 3800 -> 5400 to match how Nemforcer is tankier than Nemmando
	- Added compatibility for SS2 Official Nemeses. (Enabled by default in config)

`0.16.14`

- Chirr
	- Fixed friends losing elite affixes between stages due to 0.16.10
	- Attempted to make Cruelty elite affixes persist between stages.

- Pyro
	- Fixed Return to Sender being able to reflect friendly projectiles and stationary projectiles.

`0.16.13`

- Nemmando
	- Decisive Strike Scepter
		- Fixed screen overlay persisting until stage end if killed mid-attack.
		- Fixed a minor dedicated server-specific nullref.
		- Enemy Nemmando NPCs now have lower range when using this to prevent insta-teamwipes from Vengeance. (64m -> 32m, non-Scepter is 25m)

`0.16.12`

- Chirr
	- M1 projectile impact effect is now green.

- Pyro
	- Suppressive Fire
		- Increased DPS from 840% -> 1200%
		- Increased projectile size by 33%
		- Increased blast radius from 3m -> 4m
		
		*Should be less of a gap in damage compared to Blaze Flare now. (100% Heat for 1008% damage -> 100% Heat for 1440% damage)*
		
	- Blaze Flare
		- Increased damage from 600% + 150% x 8 -> 400% + 200% x 8
		
		*Slight damage increase, but main change is that damage has been shifted to scale off of Heat more.*
		
	*Overall damage output of Pyro was lacking, with perfect Heat management only resulting in slightly below average killtimes, so his damage output on his Heat-related skills has been buffed.*

`0.16.11`

- Chirr
	- Fixed weird minion spawnpoints on stage start due to 0.16.10

`0.16.10`

- Chirr
	- Rewrote minion persistence code so it works on dedicated servers.
	
	*Let me know if anything broke.*

`0.16.9`

- Fixed Dungus not working online.
- Fixed Watch Metronome not working online.
- Reduced Watch Metronome chargeup duration from 8s -> 6s

`0.16.8`

- Nemmando
	- Distant Gash
		- Increased damage from 160%-500% -> 200%-600%
		- Reduced charge time from 1s -> 0.8s
		- Reduced endlag from 0.6s -> 0.4s
		
		*This was getting outperformed hard by gun alt due to having better overall DPS, being hitscan, and being usable alongside the primary.*

	- Fixed fall damage during Decisive Strike when using it online.
	
	- Added Client-Side options (can be changed in-game with RiskOfOptions)
		- Decisive Strike dash scales with move speed.
			- Default: True
		- Distant Gash always full-charges without having to hold the button.
			- Default: False

`0.16.7`

- Chirr
	- Flying Autosprint
		- Now only applies when moving forwards.
		- M1 orientation correction only happens when flying and moving forwards.

`0.16.6`

- Taunts
	- Tweaked taunt code to be less jank.
		- Special skin effects like the MGR music on Nemmando's Mastery aren't hardcoded into the main emote state anymore.
			- A side effect of this is that you can no longer use other taunts while already in a taunt state. (Taunt cancels were hardcoded to swap to the exact base emote states, but special effects are now in inherited states so they would be skipped).
		- Let me know if anything else broke.
	- Keybinds can now be changed in-game via Risk of Options. (This will reset your previous config setting)
	
- Chirr
	- Now autosprints when flying.
	- Added 2 taunts.
	- Headbutt
		- Lunge distance increased by 71%.
		- Lunge can now travel vertically if started airborne.
		- Increased knockback force from 2400 -> 2700
		- Increased cooldown from 4s -> 6s
		
	*Small QoL stuff from SS2O Chirr, but no plans to turn her into that since they're fundamentally very different characters.*

`0.16.5`

- Executioner
	- Execution
		- Slayer (low HP) damage bonus now applies to procs.
		
		*If you play with RiskyMod, nothing changes as it already fixes this. This change is done via a custom damagetype so it won't interfere with Vanilla Slayer attacks.*

`0.16.4`

- Executioner
	- Applying Fear now counts as a kill assist for charging Ion Burst.

`0.16.3`

- Executioner
	- Ion Burst
		- Increased damage from 2x250% -> 2x300%. (total 2500% -> 3000%)

`0.16.2`

- Pyro
	- Fixed normals on model. (Thanks FORCED_REASSEMBLY!)
	- Updated character select icon to the new model.
	
- Nemesis Invasion
	- Music is handled in a less jank way now.
		- Is tied to a NetworkBehavior instead of the Nemesis body.
		- Nemesis Music will no longer play when you spawn them in via DebugToolkit, and it won't overlap if multiple Nemeses are alive.

`0.16.1`

- Executioner
	- Disabled Dynamic Bones on skins. His model should look a LOT less jank when in motion now.

`0.16.0`

- Executioner
	- Primaries
		- Increased damage from 150% -> 180%
		
		*4 shots to kill a Lemurian on stage 1.*
		
	- Ion Burst
		- Increased damage from 320% -> 2x250%.
		- Reduced stocks from 10 -> 5 (3200% damage total -> 2500% damage total)
		- Recharges stocks every 12s, on top of the on-kill restock.
		- No longer gains double stocks from killing Feared enemies.
		
		*This skill struggled in MP, and often ended up requiring too much time to build up for too little payoff. Should be able to fire off big bursts more frequently now.*
		
	- Crowd Dispersion
		- Now fully Stuns instead of just Staggering.

`0.15.6`

- Watch Metronome
	- No longer calls RecalculateStats every frame.
	- Reduced network commands it sends (now only notifies server when buffcount needs to change, instead of every frame).
	- Fixed item logic being duplicated across all clients, potentially resulting in lots of lag (due to recalculate stats every frame and potentially 4-16 network commands being sent every frame).
	- Fixed jumping not pausing the charge (was only pausing the initial charge delay).

- Dormant Fungus
	- Tweaked network code a bit to be more failsafe.

`0.15.5`

- Fixed RiskyMod being set as a HardDependency.

`0.15.4`

- Fixed Dungus healing 100x more than intended.

`0.15.3`

- Added Erratic Gadget (Disabled by Default)
- Added Relic of Mass (Disabled by Default)

`0.15.2`

- Fixed being able to ignore the Greater Warbanner cap when respawning.

`0.15.1`

- Readme update.

`0.15.0`

- Items
	- Common
		- Fork
			- Increase base damage by 2.4 (+2.4 per stack).
			
			*Increases base damage by Commando's damage per level, same as in the original Starstorm.*
			
		- Molten Coin
			- 6% chance to ignite for 300% (+300%) base damage and earn $1. Scales with time.
				- Does not gain money in Bazaar.
			
		- Diary (Rework)
			- Increases move speed by 10% (+10%) and armor by 3 (+3).
			
			*Meant to be another way to boost your move speed stat while being a lower priority item than mainstays like Hoofs/Coffee/Energy Drinks. Numberwise it's comparable to 2/3 of a Hoof and 1/3 of a Hermit's Scarf.*
			
	- Uncommon
		- Watch Metronome
			- Walking on the ground charges up the speed of your next sprint by up to 200%. Decays over 4s (+2s per stack) of sprinting.
			
	- Rare
		- Droid Head
			- Killing Elites summons a Strike Drone with 100% damage (+50% damage per stack) that inherits the Elite type. Lasts 30s, limited to 3 active.
			
		- Green Chocolate
			- When receiving 20% or more of your max health as damage, any damage over the threshold is reduced by 50%. Gain +50% damage for 7s upon triggering this effect, stacks up to 1 (+1 per stack) time.
			
	- Boss
		- Stirring Soul
			- Dropped by Nemmando.
		
- Equipment
	- Cloaking Headband
		- Become invisible for 10 seconds. (45s CD)

	- Greater Warbanner
		- Place a greater warbanner that strengthens allies within 25m. Raises critical chance by 25%. Every second, reduces skill cooldowns by 0.5s and heals for 2.5% of your max health.
		
		*Is this networked? Attempted to fix some of the bugs from the old implementation.*
		
- Disabled by Default
	- Common
		- Dormant Fungus
		- Coffee Bag
		
		*These are vanilla, but left in the mod in case you want to use them for whatever reason.*
		
- Currently Unimplemented Items
	- Detritive Trematode/Strange Can
		- These are items based around dealing %HP damage. Should this be a thing?
		
	- Hunter's Sigil
		- Sharp Anchor from StormysItems does this better, and without the RNG crit chance.
		- Considered reworking it to give storable guaranteed crits when standing still, but this would mainly just be a buff to already strong single-hit characters.
		
	- Malice
		- I always felt this was way too strong in RoR2 in all iterations of it, due to free targeting being a thing in RoR2 as well as AoE attacks being more common in general.
		
	- Broken Blood Tester/Prototype Jet Boots
		- Useless.
		
	- Nkota's Heritage
		- Was only good in the SS ruleset due to Command + Sac and reds being more common on Stage 1.
		
	- Erratic Gadget/Relic of Mass
		- Already a part of Vanilla.

	- Pressurized Canister
		- Even if bugs are fixed, it inherently doesnt work with Gesture and is an equipment that would always get swapped for literally any other.

`0.14.5`

- Fixed in-game Pyro model using an older version of the mesh. (Thanks Rob!)

`0.14.4`

- Fixed a niche bug where enemy Nemmandos could permanently change team after falling off the map.

`0.14.3`

- Updated mod-specific Mithrix chat to have the right size.

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