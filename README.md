ColorPalettes for Unity
=============


**ColorPalette**
---
is a plugin for Unity which let's you create ColorPalettes.

![Palette UI preview ](https://raw.githubusercontent.com/DomDomHaas/ColorImporter/master/Preview.png "ColorPalette Screenshot")

If you want to use only the ColorPaletts make sure to use following scripts from this repo:
- PaletteData.cs
- Palette.cs
- at least the PaletteInspector.cs from the Editor folder (it has to be put into a "Editor" folder)
- All files of the folder **JSONPersistent** (it's actually another Tool for saving things to a txt-file)

Then add the Palette.cs on a gameobject and start creating your palette. Make sure to click on the **Save-Button** once you're finished!

**To use** the colors you have to do a little coding and access the `myData.colors` or `myData.percentages` via script.


**ColorPaletteImporter**
---
supports loading in Palettes from www.pltts.me / www.colourlovers.com (by now)

![PaletteImporter UI preview ](https://raw.githubusercontent.com/DomDomHaas/ColorImporter/master/Preview_Importer.png "ColorPaletteImporter Screenshot")

If you want to use the ColorPalettImporter make sure to include the scripts above and the follwing:
- PaletteImporterData.cs
- PaletteImporter.cs
- All files from the Editor folder
- **Additionally** add HtmlSharp (HtmlParser: https://github.com/wallerdev/htmlsharp extract at least the "HtmlSharp" folder into your asset folder)
- **Change the Playersettings** "API Compatibility Level" of Unity has to be set to ".NET 2.0" NOT ".NET 2.0 subset" (otherwise the System.Web is missing)


Then simply add the ColorPaletteImporter.cs script on a gameObject.
To Import Palettes do 3 steps:
- insert an url which goes directly to either a pltts.me or colourlovers.com palette
- click on "Import palette from URL"
- if you like what you see click on "Save to File" on ensure the colors and percentages will be stored



The "load from file" will be called in the Awake of the ColorPalette script. If your using multiple gameobjects with ColorPalette scripts, make sure they have different names.


**KNOW ISSUSE**
---
- Plaette scripts on multiple GameObjects with the same name will cause errors when trying to save the file or will overwrite it.
- So far I haven't had the chance to test it on mobile devices (andoird / iOS builds) and I'm pretty sure it won't work on the webplayer since where would the file be stored!
- I couldn't make a drag'n'drop from the colors to other objects in the scene.

