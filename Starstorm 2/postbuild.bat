@echo off

rem get ror2 plugins path for current user (add yours below if it's not the default)
set pluginpath=C:\Program Files (x86)\Steam\steamapps\common\Risk of Rain 2\BepInEx\plugins\Starstorm 2
if %username% == Flan	set pluginpath=F:\SteamLibrary\steamapps\common\Risk of Rain 2\BepInEx\plugins\Starstorm 2
if %username% == kurog	set pluginpath=E:\Steam\steamapps\common\Risk of Rain 2\BepInEx\plugins\Starstorm 2
if %username% == pmble	set pluginpath=D:\Steam\steamapps\common\Risk of Rain 2\BepInEx\plugins\Starstorm 2
if %username% == Erikbir set pluginpath=C:\Users\Erikbir\AppData\Roaming\r2modmanPlus-local\mods\profiles\Blinx\BepInEx\plugins\TeamMoonstorm-Starstorm2
if %username% == Node	set pluginpath=D:\SteamLibrary\steamapps\common\Risk of Rain 2\BepInEx\plugins

rem weaver patch for mp compatibility
cd Weaver\
Unity.UNetWeaver.exe "..\libs\UnityEngine.CoreModule.dll" "..\libs\com.unity.multiplayer-hlapi.Runtime.dll" "..\Starstorm2Release\plugins\ChirrLover-Starstorm2Unofficial" "..\bin\Debug\Starstorm2Unofficial.dll" "..\libs"

rem package the mod
mkdir ..\Starstorm2Release\plugins\ChirrLover-Starstorm2Unofficial
cd ..\Starstorm2Release\plugins\ChirrLover-Starstorm2Unofficial
copy ..\README.md .
tar caf Starstorm2Unofficial.zip icon.png	manifest.json README.md Starstorm2Unofficial.dll
del README.md

rem copy the mod to your game plugins directory for testing
mkdir "%pluginpath%\"
copy Starstorm2Unofficial.dll "%pluginpath%\"
rem copy starstorm2.language "%pluginpath%\"