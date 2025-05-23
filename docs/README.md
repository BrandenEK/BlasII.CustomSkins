# Blasphemous 2 Custom Skins

<img src="https://img.shields.io/github/downloads/BrandenEK/BlasII.CustomSkins/total?color=872124&style=for-the-badge">

---

## Contributors

***- Programming and design -*** <br>
[@BrandenEK](https://github.com/BrandenEK), [@ObsessiveBadguy](https://github.com/ObsessiveBadguy), [@Faust](https://github.com/FaustBaudelaire)

## How to play with custom skins
1. Open the "blasphemous2" folder on the [Community Skins repo](https://github.com/BrandenEK/Blasphemous.Community.Skins)
1. Find a skin you like and download the entire folder
1. Place the downloaded folder into your "{GameRoot}/Modding/skins" folder
1. Once in-game, use the command 'skin set {ID}'

## How to create custom skins
1. Open the cheat console with 'backslash' and type 'skin export {GROUP}'
1. Wait for the export process to finish, it will take a few minutes and about 1gb of ram
1. Navigate to the "{GameRoot}/Modding/content/Custom Skins" folder
1. Modify any of these files with a photo editor of your choice
1. Upload the modified '.png' and '.json' file to the [Community Skins repo](https://github.com/BrandenEK/Blasphemous.Community.Skins)

## Built-in animation groups
These are all of the valid group names that can be used in the export command.  They can be combined with '+', or use just the initial part to export multiple groups at once.
```
all
player.body
player.vfx
weapons.censer.body
weapons.censer.vfx
weapons.meaculpa.body
weapons.meaculpa.vfx
weapons.meaculpa.projectile
weapons.rapier.body
weapons.rapier.vfx
weapons.rosary.body
weapons.rosary.vfx
```
For example,
- Export everything: ```skin export all```
- Export just the player: ```skin export player```
- Export just the player and weapons: ```skin export player+weapons```
- Export just the censer body and vfx: ```skin export weapons.censer```
- Export just the censer vfx: ```skin export weapons.censer.vfx```

## Available commands
| Command | Parameters | Description |
| ------- | ----------- | ------- |
| `skin set` | ID | Replaces the current skin with one at "Modding/skins/{ID}" |
| `skin merge` | ID | Merges the current skin with one at "Modding/skins/{ID}" |
| `skin reset` | none | Resets the current skin to default |
| `skin list` | none | Displays a list of all installed skins |
| `skin export` | GROUP | Extracts all unmodified player animations in the specified group(s) from the game |

## Configuration settings
| Setting | Description | Default |
| ------- | ----------- | :-----: |
| usePerformanceMode | Increases performance by only replacing sprites on the player object, but some animations won't appear modified | false |
| importsPerFrame | How many sprites are loaded per frame during the import process.  Higher numbers increase the speed, but may cause lag | 30 |
| exportAnimationWidth| The maximum width of exported animations in pixels | 2048 |
| exportGroupHeight | The maximum height of exported groups in pixels | 8192 |

## Installation
This mod is available for download through the [Blasphemous Mod Installer](https://github.com/BrandenEK/Blasphemous.Modding.Installer) <br>
Required dependencies:
- [Modding API](https://github.com/BrandenEK/BlasII.ModdingAPI)
- [Cheat Console](https://github.com/BrandenEK/BlasII.CheatConsole)
