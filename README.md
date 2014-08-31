ColorImporter
=============

ColorImporter is a plugin for Unity which supports loading in Palettes from www.pltts.me / www.colourlovers.com (by now)

Following things are needed:
- HtmlSharp (HtmlParser: https://github.com/wallerdev/htmlsharp extract the "HtmlSharp" folder into your asset folder)
- JSONPersistent Unity Tool (is included)
- The Playersettings "API Compatibility Level" of Unity has to be set to ".NET 2.0" NOT ".NET 2.0 subset" (otherwise the System.Web is missing)


Then do 3 steps:
1. insert an url which goes directly to either a pltts.me or colourlovers.com palette
2. click on "Import palette from URL"
3. if you like what you see click on "Save to File" on ensure the colors and percentages will be stored


![Tool UI preview ](https://raw.githubusercontent.com/DomDomHaas/ColorImporter/master/Preview.png "Color Importer")
