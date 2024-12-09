# RPG Engine

this is a remake of the RPG portion of the TV script for Space Engineers that i used to remake Dragon Warrior. i'm starting with the map editor.

it now supports multiple tilesets. and uses a single tilesheet; so fewer images to convert with Whip's Image Converter. still need to manually set which "layer" the tiles are. `w` for wall and some other stuff. there's now an overlay layer that can do hidden paths... and other than a few special layers most are for if you want to have walkable floors that are like toxic or whatever.

anwyays.... this is all massively under construction lol.... at time of writing i got the map editor part mostly working. just like creating a new map. and doing all the ground and ceiling tiles... and saving and loading. still need to do map options. (resize, change tileset, set default exit) i've been working on it since the start of October.... and like... the TV script and Dragon Warrior remake stuff kinda took me September through December last year.

## GameAction scripting quick reference

### Variables and Values

 * `value` - a simple value.
 * `@Obj.key` - a simple variable. ex: `@Bools.Chest1`, `@Ints.partyGold`
 * `@Obj.index.key` - for accessing lists. ex: `@Party.0.Stat.hp`
 * `@Obj.#index.key` - dynamic index in a list. ex: `@Party.#i.Stat.hp`
 * `@Obj.$string.key` - dynamic key. ex: `@Items.$ItemName.useAction`

### Variable Tree

 * Ints.`key` (some objects can remap #`IntName` for a dynamic index)
 * Bools.`key`
 * Strings.`key` (some objects can remap $`StringName` for a dynamic key)
 * Inventory.`Item Name` (`@Inventory.$ItemName`, `@Inventory.ItemName`)
 * Inventory.Count
 * Inventory.Keys.`index` (`@Inventory.Keys.#i`)
 * Player.X
 * Player.Y
 * Player.Direction
 * Player.SpriteId
 * Player.Visible
 * NPC.X
 * NPC.Y
 * NPC.Direction
 * NPC.SpriteId
 * NPC.Enabled
 * Map.Visible
 * GridInfo.`VarName`
 * Party.Count
 * Party.`index`.Stat.`key`
 * Party.`index`.MaxStat.`key`
 * Party.`index`.Status.`key`
 * Party.`index`.Gear.`key`
 * Items.`key`.`key`

### Commands

 * `set`:`Destination`=`Source1` - sets the value of Source1 to Destination. ex: `set:Ints.partGold=100;`
 * `add`:`Destination`=`Source1`[,`Source2`,`Source3`] - sums Sources and stores in Destination. `add:Ints.damage=Ints.PlayerStr,Ints.WeaponAtk;` or adds Source to Destination. `add:Ints.partyGold=10;`
 * `sub`:`Destination`=`Source1`[,`Source2`,`Source3`] - subtracts Sources from first source and stores in Destination. `add:Ints.damage=Ints.EnemyAttack,Ints.PlayerDef;` or subtracts Source to Destination. `add:Ints.partyGold=10;`
 * `mul`:`Destination`=`Source1`[,`Source2`] - multiplies Sources and stores in Destination. `add:Ints.damage=Ints.PlayerStr,Ints.WeaponAtk;` or multiplies Source with Destination. `add:Ints.partyGold=10;`
 * `div`:`Destination`=`Source1`[,`Source2`] - divides Sources and stores in Destination. `add:Ints.damage=Ints.PlayerStr,Ints.WeaponAtk;` or devides Source from Destination. `add:Ints.partyGold=10;`
 * `if`:`VarA`==`VarB` - checks to see if the condition is met. can do ==, >=, >, <=, < (you can nest if statements)
 * `else` - tells the script parser that the following commands are for when the if is false
 * `endif` - tells the script parser that the if block has ended.
 * `for`:`Iterator`,`Start`,`End`,`Step` - does a for loop from Start through End with at Step.... and will store the current value of the loop in the Iterator.  `for:@Ints.i,0,3,1;`
 * `endfor` - ends the for loop block
 * `run`:`Destination` - runs another action `run:TestAction;`

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
 * `addArea`:[`Header`]=`x`,`y`,`width`,`height`[,`virtical`[,`background`]] - starts adding a layout area (areas can be nested in other areas... in theory x,y doesn't matter for the nested areas)
 * `addAreaText`:`text`[=`fontSize`] - adds a text element to the layout area.
 * `addAreaVar`:`label`=`var` - lets you add a bound var to a layout area (within a scene)
 * `endArea` and `showArea` - finalize the layout area. both do the same thing but it could make the code more readable if nested areas use endArea and the last one is showArea...

### Party and Enemy

 * `clearParty` - clear's the party list
 * `addParty`:`name`=`job` - adds a party member with a job name to get their stats
 * `removeParty`[:`index`] - removes the last party member (or at the optional index)
 * `clearEnemies` - clear the current enemies list
