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

				public PaletteImporterData myImporterData;

				//new public PaletteData myData;


				// Use this for initialization
				new void Awake ()
				{
						init ();
				}

				public void init ()
				{
						isColourLovers = false;
						isPLTTS = false;
						isLocalFile = false;

						myImporterData = new PaletteImporterData ();

						base.init ();
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

						HtmlParser parser = new HtmlParser ();
						Document doc = parser.Parse (html.text);

						this.myImporterData.colors = new Color[5];
						this.myImporterData.percentages = new float[5];

						if (isColourLovers) {

								extractFromColorlovers (doc);

								if (this.myImporterData.loadPercent) {
				
										for (int i = 0; i < this.myImporterData.percentages.Length; i++) {
												// totalWidth = 100% this.myData.percentages [i] = x%
												this.myImporterData.percentages [i] = this.myImporterData.percentages [i] / this.myImporterData.totalWidth;
										}
								} else {
										this.myImporterData.percentages = PaletteData.getDefaultPercentages ();
								}

						} else if (isPLTTS) {
								extractFromPLTTS (doc);
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

						myImporterData.paletteURL = URL;
				}

				private void extractFromColorlovers (Document doc)
				{
						int colorCount = 0;
						int percentCount = 0;
						this.myImporterData.totalWidth = 0;

						IEnumerable<Tag> headerTags = doc.FindAll ("h1");
						foreach (Tag headerTag in headerTags) {
//								if (!string.IsNullOrEmpty (headerTag.c)) {


								//this.myImporterData.name = headerTag.ToString ();
								Debug.Log (headerTag.ToString ().HtmlDecode ());
								//["class"] == "feature-detail-container") {

//								}
						}


						IEnumerable<Tag> links = doc.FindAll ("a");

						foreach (Tag a in links) {
								if (a ["class"] == "left pointer block") {

										string style = a ["style"];
										foreach (string styleCss in style.Split (';')) {
												if (myImporterData.loadPercent && styleCss.Contains ("width")) {

														string width = styleCss.Split (':') [1];
														width = width.Substring (0, width.IndexOf ("px"));
														float widthF = float.Parse (width.Trim ());
														this.myImporterData.totalWidth += widthF;
														this.myImporterData.percentages [percentCount++] = widthF;

												} else if (styleCss.Contains ("background-color")) {

														string bgColor = styleCss.Split (':') [1];
														bgColor = bgColor.Trim ().Substring (1);
														this.myImporterData.colors [colorCount++] = JSONPersistor.HexToColor (bgColor);
												}
										}

										//Debug.Log (style);
								}
						}


				}
	

				private void extractFromPLTTS (Document doc)
				{
						int colorCount = 0;
						int percentCount = 0;
						this.myImporterData.totalWidth = 0;

						Tag colorBlock = doc.Find (".palette-colors");
						//Debug.Log (colorBlock);

						foreach (Element colorDiv in colorBlock.Children) {

								if (!string.IsNullOrEmpty (colorDiv.ToString ().Trim ())) {
										// can contain empty Elements!

										//Tag colorTag = (HtmlSharp.Elements.Tags.Div)colorDiv;
										Tag colorTag = (Tag)colorDiv;

										string style = colorTag ["style"];
										foreach (string styleCss in style.Split (';')) {
												if (myImporterData.loadPercent && styleCss.Contains ("width")) {
						
														string width = styleCss.Split (':') [1];
														width = width.Substring (0, width.IndexOf ("%"));
														float widthF = float.Parse (width.Trim ());
														this.myImporterData.totalWidth += widthF / 100;
														this.myImporterData.percentages [percentCount++] = widthF / 100;
						
												} else if (styleCss.Contains ("background-color")) {
						
														string bgColor = styleCss.Split (':') [1];
														bgColor = bgColor.Trim ().Substring (1);
														this.myImporterData.colors [colorCount++] = JSONPersistor.HexToColor (bgColor);
												}
										}
				
										//Debug.Log (style);
								}
						}

						if (!myImporterData.loadPercent) {
								this.myImporterData.percentages = PaletteData.getDefaultPercentages ();
						}

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