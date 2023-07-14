# MapVariantIDSwitcher
A tool for switching Halo Forge Map variant ids

This tool allows users to change a forge map variant file so it can be used on another map, such as a modded version of that map. The only restriction is that the new map should have the same forge palette as the old one, otherwise you can expect to encounter oddities or errors.

I have set the application up so that you can use it with every game in MCC that allows the use of Forge, however I have only tested it with Reach thus far.

## Install

Place it within your Excession install folder. It needs to be there so that it can use the Excession DLL files which I cannot distribute.

## Usage
* Set the Game Engine for the game you're targeting
* Set the Mod Map to the JSON file for your modded *map* for example `C:\HaloMods\MyHalo3Mod\multiplayer\supercoolmap.json`
* Set the Map Variant to the variant `.bin` file you want to switch the ID of. You can find these either in your MCC directories or from within `AppData\LocalLow\MCC\LocalFiles` for your saved map variants
* Set the Output Folder for where you want the new file with the switched ID to be placed in
* Click Switch Variant ID

In preferences you can setup default paths for looking for Mod Maps, Map Variants and the Output Folder.
