# RPG Engine

this is an in game script that can run rpg games using game data and sprites stored in the GridDB database. it has a built in map editor that lets you create and edit maps. adding npcs and doors between maps.

it uses a custom Game Actions Scripting language that handles all of the actual game logic. with some hooks like when you take a step or interact with an npc. with game actions you can start a scene and hide the map and add whatever sprites and areas and menus you want.

## Final Fantasy Demo

i made a demo of the opening area of Final Fantasy on the NES to benchmark the rpg engine and make sure it could do the things. it uses the custom scene feature to do the shops as well as the main menu where you can equip the gear you buy. it also uses the custom scenes to do the combat stuff.
