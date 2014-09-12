using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using HtmlSharp;
using HtmlSharp.Elements;
using System.IO;


namespace ColorPalette
{

		public class PaletteCollection : JSONPersistent
		{

				public PaletteCollectionData collectionData;

				public bool importIsRunning = false;
				public float importedBytes = 0;

				private bool isColourLovers = false;
				private bool isPLTTS = false;
				private bool isLocalFile = false;

				// Use this for initialization
				new void Awake ()
				{
						init ();
				}

				public void init ()
				{
						collectionData.palettes = new Dictionary<string, PaletteData> ();

						fileName = getFileName ();
						reset ();

						if (JSONPersistor.Instance.fileExists (fileName)) {
								load ();
						}
				}

	
				// Update is called once per frame
				void Update ()
				{
	
				}

				/// <summary>
				/// Creates the plaette.
				/// </summary>
				/// <returns><c>true</c>, if plaette was created, <c>false</c> otherwise.</returns>
				/// <param name="kvp">Kvp.</param>
				public bool CreatePalette (KeyValuePair<string, PaletteData> kvp = new KeyValuePair<string, PaletteData> ())
				{
						if (string.IsNullOrEmpty (kvp.Key)) {
								try {
										collectionData.palettes.Add ("newPalette", new PaletteData ("newPalette"));
								} catch (System.ArgumentException e) {
										Debug.Log (e);
										return false;
								}

								return true;

						} else {
								if (collectionData.palettes.ContainsKey (kvp.Key)) {
										return false;
								} else {
										collectionData.palettes.Add (kvp);
										return true;
								}
						}
				}



				private void reset ()
				{
						this.isColourLovers = false;
						this.isLocalFile = false;
						this.isPLTTS = false;
						this.importIsRunning = false;
						this.importedBytes = 0;
				}
		
				public bool ImportPaletteCollection (string newURL)
				{
						reset ();

						analizeURL (newURL);
			
						WWW html = new WWW (newURL);

						while (!html.isDone) {
				
								if (!string.IsNullOrEmpty (html.error)) {
										throw new UnityException ("error loading URL: " + html.error);
								}
						}

						this.importedBytes = html.bytesDownloaded;
						Debug.Log ("download finished, loaded " + html.bytesDownloaded + " bytes");

						collectionData.paletteURL = newURL;
			
						HtmlParser parser = new HtmlParser ();
						Document doc = parser.Parse (html.text);

						//Debug.Log (doc.ToString ());

						PaletteData extracedData = null;

						if (isColourLovers) {
				
								extracedData = PaletteImporter.extractFromColorlovers (doc, this.collectionData.loadPercent);

								if (this.collectionData.loadPercent) {
					
										for (int i = 0; i < extracedData.percentages.Length; i++) {
												// totalWidth = 100% this.myData.percentages [i] = x%
												extracedData.percentages [i] = extracedData.percentages [i] / extracedData.totalWidth;
										}
								} else {
										extracedData.percentages = PaletteData.getDefaultPercentages ();
								}

						} else if (isPLTTS) {
								extracedData = PaletteImporter.extractFromPLTTS (doc, this.collectionData.loadPercent);
						}
						
/*						if (extracedData != null) {
								return CreatePalette (new KeyValuePair<string, PaletteData> (extracedData.name, extracedData));
						} else {
								Debug.Log ("Palette :'" + extracedData.percentages [0] + "' could not be load... is the URL correct? ");
								return false;
						}
*/
						return CreatePalette (new KeyValuePair<string, PaletteData> (extracedData.name, extracedData));
				}


				private void analizeURL (string URL)
				{
						if (URL.Contains ("colourlovers")) {
								isColourLovers = true;
								Debug.Log ("recognized colourlovers URL: " + URL);
						} else if (URL.Contains ("pltts")) {
								isPLTTS = true;
								Debug.Log ("recognized pllts URL: " + URL);
						} else if (URL.Contains ("file:")) {
								isLocalFile = true;
						} else {
								throw new UnityException ("Unkown URL, so far only colourlovers.com and pltts.me is supported! " + URL);
						}
			
						if (isLocalFile) {
				
								string fileName = Path.GetFileNameWithoutExtension (URL);
								int filenr = 0;
								if (int.TryParse (fileName, out filenr)) {
										isPLTTS = true;
								} else {
										isColourLovers = true;
										Debug.LogWarning ("reading local file: " + fileName + " expecting it to be from Colourlovers.com");
								}
						}
			
						collectionData.paletteURL = URL;
				}

				/*
				private void extractCollectionFromColorlovers (Document doc)
				{
						int colorCount = 0;
						int percentCount = 0;

						IEnumerable<Tag> divs = doc.FindAll ("div");

						//Debug.Log ("amount of divs: " + divs);
			
						foreach (Tag div in divs) {

								//Debug.Log ("id: " + div ["id"] + " class: " + div ["class"]);

								//if (div ["class"] == "detail-row") {
								if (div ["id"] == "browse-palettes") {

										foreach (Element element in  div.Children) {

												if (!string.IsNullOrEmpty (element.ToString ().Trim ())) {

														Tag elementTag = (Tag)element;
														Debug.Log ("id: " + elementTag ["id"] + " class: " + elementTag ["class"]);
												}

										}

										//Debug.Log ("id: " + div ["id"] + " class: " + div ["class"]);


										string style = a ["style"];
										foreach (string styleCss in style.Split (';')) {
												if (((PaletteImporterData)myData).loadPercent && styleCss.Contains ("width")) {
							
														string width = styleCss.Split (':') [1];
														width = width.Substring (0, width.IndexOf ("px"));
														float widthF = float.Parse (width.Trim ());
														this.myData.totalWidth += widthF;
														this.myData.percentages [percentCount++] = widthF;

														if (((PaletteImporterData)myData).loadPercent) {
								
																for (int i = 0; i < this.myData.percentages.Length; i++) {
																		// totalWidth = 100% this.myData.percentages [i] = x%
																		this.myData.percentages [i] = this.myData.percentages [i] / this.myData.totalWidth;
																}
														} else {
																this.myData.percentages = PaletteData.getDefaultPercentages ();
														}
							
												} else if (styleCss.Contains ("background-color")) {
							
														string bgColor = styleCss.Split (':') [1];
														bgColor = bgColor.Trim ().Substring (1);
														this.myData.colors [colorCount++] = JSONPersistor.HexToColor (bgColor);
												}


										}


								}
				
				
						}
			
			
				}
				*/

				/*

		
				private void extractCollectionFromPLTTS (Document doc)
				{
						int colorCount = 0;
						int percentCount = 0;
						this.myData.totalWidth = 0;
			
						Tag colorBlock = doc.Find (".palette-colors");
						//Debug.Log (colorBlock);
			
						foreach (Element colorDiv in colorBlock.Children) {
				
								if (!string.IsNullOrEmpty (colorDiv.ToString ().Trim ())) {
										// can contain empty Elements!
					
										//Tag colorTag = (HtmlSharp.Elements.Tags.Div)colorDiv;
										Tag colorTag = (Tag)colorDiv;
					
										string style = colorTag ["style"];
										foreach (string styleCss in style.Split (';')) {
												if (((PaletteImporterData)myData).loadPercent && styleCss.Contains ("width")) {
							
														string width = styleCss.Split (':') [1];
														width = width.Substring (0, width.IndexOf ("%"));
														float widthF = float.Parse (width.Trim ());
														this.myData.totalWidth += widthF / 100;
														this.myData.percentages [percentCount++] = widthF / 100;
							
												} else if (styleCss.Contains ("background-color")) {
							
														string bgColor = styleCss.Split (':') [1];
														bgColor = bgColor.Trim ().Substring (1);
														this.myData.colors [colorCount++] = JSONPersistor.HexToColor (bgColor);
												}
										}
					
										//Debug.Log (style);
								}
						}
			
						if (!((PaletteImporterData)myData).loadPercent) {
								this.myData.percentages = PaletteData.getDefaultPercentages ();
						}
			
				}
*/

				public override string getFileName ()
				{
						return this.gameObject.name + "_palettes";
				}

				public override JSONClass getDataClass ()
				{
						return this.collectionData.getJsonPalette ();
				}

				public override void setClassData (JSONClass jClass)
				{
						collectionData.setPalette (jClass);
				}

				public override void save ()
				{
						fileName = getFileName ();	
						base.save ();
				}
		
				public override void load ()
				{
						fileName = getFileName ();	
						base.load ();
				}

		}


}