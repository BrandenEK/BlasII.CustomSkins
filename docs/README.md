# Blasphemous 2 Custom Skins

<img src="https://img.shields.io/github/downloads/BrandenEK/BlasII.CustomSkins/total?color=872124&style=for-the-badge">

---

## Contributors

***- Programming and design -*** <br>
[@BrandenEK](https://github.com/BrandenEK), [@ObsessiveBadguy](https://github.com/ObsessiveBadguy), [@Faust](https://github.com/FaustBaudelaire)

## How to play with custom skins
1. Open the "blasphemous2" folder on the [Community Skins repo](https://github.com/BrandenEK/Blasphemous.Community.Skins)
1. Find a skin you like and download everything in the 'textures' folder
1. Place all of the '.png' and the '.json' files into your "{GameRoot}/Modding/skins" folder
1. Everything in the "skins" folder will be loaded into the game, you can also use commands to modify your skin

## How to create custom skins
1. Open the cheat console with 'backslash' and type 'skin export'
1. Wait for the export process to finish, it will take a few minutes and about 1gb of ram
1. Navigate to the "{GameRoot}/Modding/content/Custom Skins" folder
1. Verify that there are 1200 files present in the folder
1. Modify any of these files with a photo editor of your choice
1. Upload the modified '.png' and '.json' file to the [Community Skins repo](https://github.com/BrandenEK/Blasphemous.Community.Skins)

## Available commands
| Command | Parameters | Description |
| ------- | ----------- | ------- |
| `skin merge` | FOLDER | Merges the current skin with one at "Modding/skins/{FOLDER}" |
| `skin replace` | FOLDER | Replaces the current skin with one at "Modding/skins/{FOLDER}" |
| `skin reset` | none | Removes all loaded skins |
| `skin export` | none | Extracts all unmodified player animations from the game |

## Configuration settings
| Setting | Description | Default |
| ------- | ----------- | :-----: |
| usePerformanceMode | Increases performance by only replacing sprites on the player object, but some animations won't appear modified | false |
| importsPerFrame | How many sprites are loaded per frame during the import process.  Higher numbers increase the speed, but may cause lag | 30 |
| exportAnimationWidth| The maximum width of exported animations in pixels | 2048 |
| exportGroupHeight | The maximum height of exported groups in pixels | 8192 |

## Installation
This mod is available for download through the [Blasphemous Mod Installer](https://github.com/BrandenEK/Blasphemous.Modding.Installer)
- Required dependencies: Modding API, Cheat Console
