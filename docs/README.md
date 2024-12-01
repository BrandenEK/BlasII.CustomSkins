# Blasphemous 2 Custom Skins

<img src="https://img.shields.io/github/downloads/BrandenEK/BlasII.CustomSkins/total?color=872124&style=for-the-badge">

---

## Contributors

***- Programming and design -*** <br>
[@BrandenEK](https://github.com/BrandenEK), [@ObsessiveBadguy](https://github.com/ObsessiveBadguy), [@Faust](https://github.com/FaustBaudelaire)

## Features
- 

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
| exportGroupHeight | The maximum height of exported groups in pixels | 6500 |

## Installation
This mod is available for download through the [Blasphemous Mod Installer](https://github.com/BrandenEK/Blasphemous.Modding.Installer)
- Required dependencies: Modding API, Cheat Console
