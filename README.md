# RPG Engine

this is a remake of the RPG portion of the TV script for Space Engineers that i used to remake Dragon Warrior. i'm starting with the map editor.

it now supports multiple tilesets. and uses a single tilesheet; so fewer images to convert with Whip's Image Converter. still need to manually set which "layer" the tiles are. `w` for wall and some other stuff. there's now an overlay layer that can do hidden paths... and other than a few special layers most are for if you want to have walkable floors that are like toxic or whatever.

anwyays.... this is all massively under construction lol.... at time of writing i got the map editor part mostly working. just like creating a new map. and doing all the ground and ceiling tiles... and saving and loading. still need to do map options. (resize, change tileset, set default exit) i've been working on it since the start of October.... and like... the TV script and Dragon Warrior remake stuff kinda took me September through December last year.

## GameAction scripting quick reference

### Variables and Values

 * `value` - a simple value. (can't include `.`)
 * `Obj.key` - a simple variable. ex: `Bools.Chest1`, `Ints.partyGold`

### Commands

 * `set`:`Destination`=`Source1` - sets the value of Source1 to Destination. ex: `set:Ints.partGold=100;`
 * `add`:`Destination`=`Source1`[,`Source2`,`Source3`] - sums Sources and stores in Destination. `add:Ints.damage=Ints.PlayerStr,Ints.WeaponAtk;` or adds Source to Destination. `add:Ints.partyGold=10;`
 * `sub`:`Destination`=`Source1`[,`Source2`,`Source3`] - subtracts Sources from first source and stores in Destination. `add:Ints.damage=Ints.EnemyAttack,Ints.PlayerDef;` or subtracts Source to Destination. `add:Ints.partyGold=10;`
 * `mul`:`Destination`=`Source1`[,`Source2`] - multiplies Sources and stores in Destination. `add:Ints.damage=Ints.PlayerStr,Ints.WeaponAtk;` or multiplies Source with Destination. `add:Ints.partyGold=10;`
 * `div`:`Destination`=`Source1`[,`Source2`] - divides Sources and stores in Destination. `add:Ints.damage=Ints.PlayerStr,Ints.WeaponAtk;` or devides Source from Destination. `add:Ints.partyGold=10;`

### Game UI Commands
 
 * `say`:`Message`[=`fontsize`[,`x`,`y`]] - displays a dialog window with the message. optional fontsize and position. `say:Hello World;` or `say:Hellow World=10;` for fontsize of `0.1f`
 * `startScene`:`GameAction` - starts a Game UI Scene with the name of the game action to run when no interactable elements are on screen.
 * `endScene` - removes the scene from the Game UI.
 * `addMenu`:`Header`=`x`,`y`,`width`,`height` - creates a menu with a header at x,y with a size of height,width.
 * `addMenuItem`:`Game Action`=`multi`,`part`,`label` - adds an item to the menu with the action to call when it's selected and the display text. ex: `addMenuItem:UseHeal=Heal [,Ints.HealPotionCount,];`
 * `showMenu` - finalizes and shows the menu.
 * `addSprite`:`SpriteName`=`ScreenX`,`ScreenY`,`SpriteSheetIndex`,`SpriteSheetX`,`SpriteSheetY`,`SpriteWidth`,`SpriteHeight` - add a sprite to the screen.
 * `moveSprite`:`SpriteName`=`ScreenX`,`ScreenY` - move the sprite on the screen.
 * `replaceSprite`:`SpriteName`=`SpriteSheetIndex`,`SpriteSheetX`,`SpriteSheetY`,`SpriteWidth`,`SpriteHeight` - replace the sprite image
 * `removeSprite`:`SpriteName` - remove the sprite from the screen
