@echo off

rem get ror2 plugins path for current user (add yours below if it's not the default)
set pluginpath=C:\Program Files (x86)\Steam\steamapps\common\Risk of Rain 2\BepInEx\plugins\Starstorm 2
if %username% == Flan	set pluginpath=F:\SteamLibrary\steamapps\common\Risk of Rain 2\BepInEx\plugins\Starstorm 2
if %username% == kurog	set pluginpath=E:\Steam\steamapps\common\Risk of Rain 2\BepInEx\plugins\Starstorm 2
if %username% == pmble	set pluginpath=D:\Steam\steamapps\common\Risk of Rain 2\BepInEx\plugins\Starstorm 2
if %username% == Erikbir set pluginpath=C:\Users\Erikbir\AppData\Roaming\r2modmanPlus-local\mods\profiles\Blinx\BepInEx\plugins\TeamMoonstorm-Starstorm2

rem weaver patch for mp compatibility
cd Weaver\
Unity.UNetWeaver.exe "..\libs\UnityEngine.CoreModule.dll" "..\libs\com.unity.multiplayer-hlapi.Runtime.dll" "..\Starstorm2Release" "..\bin\Debug\Starstorm2.dll" "..\libs"

rem package the mod
mkdir ..\Starstorm2Release\
cd ..\Starstorm2Release\
copy ..\README.md .
tar caf Starstorm2.zip icon.png	manifest.json README.md Starstorm2.dll
del README.md

rem copy the mod to your game plugins directory for testing
mkdir "%pluginpath%\"
copy Starstorm2.dll "%pluginpath%\"
rem copy starstorm2.language "%pluginpath%\"
