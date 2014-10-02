using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using HtmlSharp;
using HtmlSharp.Elements;
using HtmlSharp.Extensions;
using SimpleJSON;

namespace ColorPalette
{
		public class PaletteImporter : Palette
		{

				private bool isColourLovers = false;
				private bool isPLTTS = false;
				private bool isLocalFile = false;

				public PaletteImporterData myImporterData = null;

				//new public PaletteData myData;


				// Use this for initialization
				new void Awake ()
				{
						init ();
				}

				new public void init ()
				{
						if (myImporterData == null) {
								isColourLovers = false;
								isPLTTS = false;
								isLocalFile = false;

								myImporterData = new PaletteImporterData ();

								base.init ();
						}
				}
	
				// Update is called once per frame
				void Update ()
				{
	
				}

/*				public override SimpleJSON.JSONClass getDataClass ()
				{
						JSONClass jClass = base.getDataClass ();

						jClass ["paletteURL"] = ((PaletteImporterData)myData).paletteURL;
						jClass ["loadPercent"].AsBool = ((PaletteImporterData)myData).loadPercent;


						//Debug.Log ("getDataClass: " + jClass ["colors"].Count + " " + jClass ["percentages"].Count);
						return jClass;
				}
*/
/*				public override void setClassData (SimpleJSON.JSONClass jClass)
				{
						((PaletteImporterData)myData).paletteURL = jClass ["paletteURL"];
						((PaletteImporterData)myData).loadPercent = jClass ["loadPercent"].AsBool;

						base.setClassData (jClass);
				}
*/
				public void ImportPalette (string newURL)
				{
						reset ();
						StartCoroutine (ImportPaletteFromURL (newURL));
				}

				private void reset ()
				{
						this.isColourLovers = false;
						this.isLocalFile = false;
						this.isPLTTS = false;
				}

				private IEnumerator ImportPaletteFromURL (string newURL)
				{
						analizeURL (newURL);

						WWW html = new WWW (newURL);

						while (!html.isDone) {

								if (!string.IsNullOrEmpty (html.error)) {
										throw new UnityException ("error loading URL: " + html.error);
								}

								//Debug.Log ("downloading Palette " + html.progress + "%");
						}

						Debug.Log ("download finished, loaded " + html.bytesDownloaded + " bytes");
						this.myImporterData.paletteURL = newURL;
						string[] splitted = newURL.Split ('/');
						string paletteName = splitted [splitted.Length - 1];


						HtmlParser parser = new HtmlParser ();
						Document doc = parser.Parse (html.text);

/*						this.myImporterData.colors = new Color[5];
						this.myImporterData.percentages = new float[5];
*/
						PaletteData extracedData = null;

						if (isColourLovers) {

								extracedData = extractFromColorlovers (doc, this.myImporterData.loadPercent);

								// don't copy directly because of PaletteData vs PaletteImporterData
								this.myImporterData.totalWidth = extracedData.totalWidth;
								this.myImporterData.name = paletteName;
								this.myImporterData.colors = extracedData.colors;
								this.myImporterData.alphas = extracedData.alphas;


								if (this.myImporterData.loadPercent) {

										this.myImporterData.percentages = extracedData.percentages;

				
										for (int i = 0; i < this.myImporterData.percentages.Length; i++) {
												// totalWidth = 100% this.myData.percentages [i] = x%

												this.myImporterData.percentages [i] = this.myImporterData.percentages [i] / this.myImporterData.totalWidth;
												//Debug.Log ("totalWidth " + this.myImporterData.totalWidth + " % " + this.myImporterData.percentages [i]);

										}
								} else {
										this.myImporterData.percentages = PaletteData.getDefaultPercentages ();
								}

						} else if (isPLTTS) {

								extracedData = extractFromPLTTS (doc, this.myImporterData.loadPercent);

								this.myImporterData.name = paletteName;
								this.myImporterData.colors = extracedData.colors;
								this.myImporterData.percentages = extracedData.percentages;
								this.myImporterData.alphas = extracedData.alphas;

						}

						yield return null;
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
								throw new UnityException ("Unkown URL '" + URL + "' , so far only colourlovers.com and pltts.me is supported! ");
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

						myImporterData.paletteURL = URL;
				}

				/// <summary>
				/// Extracts from colorlovers, the percentages only the pixel-width, so it as to be divied with the totalwidth
				/// </summary>
				/// <returns>The from colorlovers.</returns>
				/// <param name="doc">Document.</param>
				/// <param name="loadPercentages">If set to <c>true</c> load percentages.</param>
				public static PaletteData extractFromColorlovers (Document doc, bool loadPercentages)
				{
						int colorCount = 0;
						int percentCount = 0;
						PaletteData palette = new PaletteData ();

/*
 						IEnumerable<Tag> headerTags = doc.FindAll ("h1");
						foreach (Tag headerTag in headerTags) {
//								if (!string.IsNullOrEmpty (headerTag.c)) {


								//palette.name = headerTag.ToString ();
								Debug.Log (headerTag.ToString ().HtmlDecode ());
								//["class"] == "feature-detail-container") {

//								}
						}
			 */


						IEnumerable<Tag> links = doc.FindAll ("a");


						foreach (Tag a in links) {
								if (a ["class"] == "left pointer block") {

										string style = a ["style"];
										//Debug.Log ("style.Split (';') " + style.Split (';').Length);

										foreach (string styleCss in style.Split (';')) {
												if (loadPercentages && styleCss.Contains ("width")) {

														string width = styleCss.Split (':') [1];
														width = width.Substring (0, width.IndexOf ("px"));
														float widthF = float.Parse (width.Trim ());

														//Debug.Log ("found % " + widthF + " from " + styleCss);

														palette.totalWidth += widthF;
														palette.percentages [percentCount++] = widthF;

												} else if (styleCss.Contains ("background-color")) {

														string bgColor = styleCss.Split (':') [1];
														bgColor = bgColor.Trim ().Substring (1);
														palette.colors [colorCount++] = JSONPersistor.HexToColor (bgColor);
												}
										}

										//Debug.Log (style);
								}
						}

//						Debug.Log (palette.percentages [0]);

						return palette;
				}
	

				public static PaletteData extractFromPLTTS (Document doc, bool loadPercent)
				{
						int colorCount = 0;
						int percentCount = 0;
						PaletteData palette = new PaletteData ();

						Tag colorBlock = doc.Find (".palette-colors");
						//Debug.Log (colorBlock);

						foreach (Element colorDiv in colorBlock.Children) {

								if (!string.IsNullOrEmpty (colorDiv.ToString ().Trim ())) {
										// can contain empty Elements!

										//Tag colorTag = (HtmlSharp.Elements.Tags.Div)colorDiv;
										Tag colorTag = (Tag)colorDiv;

										string style = colorTag ["style"];
										foreach (string styleCss in style.Split (';')) {
												if (loadPercent && styleCss.Contains ("width")) {
						
														string width = styleCss.Split (':') [1];
														width = width.Substring (0, width.IndexOf ("%"));
														float widthF = float.Parse (width.Trim ());
														palette.totalWidth += widthF / 100;
														palette.percentages [percentCount++] = widthF / 100;
						
												} else if (styleCss.Contains ("background-color")) {
						
														string bgColor = styleCss.Split (':') [1];
														bgColor = bgColor.Trim ().Substring (1);
														palette.colors [colorCount++] = JSONPersistor.HexToColor (bgColor);
												}
										}
				
										//Debug.Log (style);
								}
						}

						if (!loadPercent) {
								palette.percentages = PaletteData.getDefaultPercentages ();
						}

						return palette;
				}

				public override JSONClass getDataClass ()
				{
						return this.myImporterData.getJsonPalette ();
				}
		
				public override void setClassData (JSONClass jClass)
				{
						this.myImporterData.setPalette (jClass);
				}

		}
}