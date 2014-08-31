using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using HtmlSharp;
using HtmlSharp.Elements;
using SimpleJSON;

public class ColorImporter : JSONPersistent
{

		private bool isColourLovers = false;
		private bool isPLTTS = false;
		private bool isLocalFile = false;


		[Serializable]
		public class data
		{

				[SerializeField]
				public string
						paletteURL = "";

				[SerializeField]
				public bool
						loadPercent = false;


				[SerializeField]
				public Color[]
						colors = new Color[5];

				[SerializeField]
				public float[]
						percentages = new float[5];

				[SerializeField]
				public float
						totalWidth = 0;
		}

		public data myData;


		// Use this for initialization
		void Awake ()
		{
				load ();
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}

		public override SimpleJSON.JSONClass getDataClass ()
		{
				JSONClass jClass = new JSONClass ();

				jClass ["paletteURL"] = myData.paletteURL;
				jClass ["loadPercent"].AsBool = myData.loadPercent;
			
				string[] hexArray = getColorsHexArray (myData.colors);

				for (int i = 0; i < hexArray.Length; i++) {
						jClass ["colors"] [i] = hexArray [i];
				}

				for (int i = 0; i < myData.percentages.Length; i++) {
						jClass ["percentages"] [i].AsFloat = myData.percentages [i];
				}

				jClass ["totalWidth"].AsFloat = myData.totalWidth;
		
				return jClass;
		}

		public override void setClassData (SimpleJSON.JSONClass jClass)
		{
				myData.paletteURL = jClass ["paletteURL"];
				myData.loadPercent = jClass ["loadPercent"].AsBool;

				int size = jClass ["colors"].Count;

				string[] hexArray = new string[size];

				for (int i = 0; i < size; i++) {
						hexArray [i] = jClass ["colors"] [i];
				}
						
				myData.colors = getHexArrayColors (hexArray);
				
				size = jClass ["percentages"].Count;
		
				for (int i = 0; i < size; i++) {
						myData.percentages [i] = jClass ["percentages"] [i].AsFloat;
				}

				myData.totalWidth = jClass ["totalWidth"].AsFloat;
		}

		public virtual string getFileName ()
		{
				return this.gameObject.name + "_myData";
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
								yield return null;
						}

						//Debug.Log ("downloading Palette " + html.progress + "%");
				}

				Debug.Log ("download finished, loaded " + html.bytesDownloaded + " bytes");
				this.myData.paletteURL = newURL;

				HtmlParser parser = new HtmlParser ();
				Document doc = parser.Parse (html.text);

				this.myData.colors = new Color[5];
				this.myData.percentages = new float[5];

				if (isColourLovers) {

						extractFromColorlovers (doc);

						if (this.myData.loadPercent) {
				
								for (int i = 0; i < this.myData.percentages.Length; i++) {
										// totalWidth = 100% this.myData.percentages [i] = x%
										this.myData.percentages [i] = this.myData.percentages [i] / this.myData.totalWidth;
								}
						} else {
								setEvenPercentages ();
						}

				} else if (isPLTTS) {
						extractFromPLTTS (doc);
				}

		}

		private void setEvenPercentages ()
		{
				this.myData.percentages = new float[]{0.2f, 0.2f, 0.2f, 0.2f, 0.2f};
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

				this.myData.paletteURL = URL;
		}

		private void extractFromColorlovers (Document doc)
		{
				int colorCount = 0;
				int percentCount = 0;
				this.myData.totalWidth = 0;

				//Tag colorBlock = doc.Find ("span.block");// .feature .feature-detail ");
				
				IEnumerable<Tag> links = doc.FindAll ("a");

				foreach (Tag a in links) {
						if (a ["class"] == "left pointer block") {

								string style = a ["style"];
								foreach (string styleCss in style.Split (';')) {
										if (this.myData.loadPercent && styleCss.Contains ("width")) {

												string width = styleCss.Split (':') [1];
												width = width.Substring (0, width.IndexOf ("px"));
												float widthF = float.Parse (width.Trim ());
												this.myData.totalWidth += widthF;
												this.myData.percentages [percentCount++] = widthF;

										} else if (styleCss.Contains ("background-color")) {

												string bgColor = styleCss.Split (':') [1];
												bgColor = bgColor.Trim ().Substring (1);
												this.myData.colors [colorCount++] = JSONPersistor.HexToColor (bgColor);
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
										if (this.myData.loadPercent && styleCss.Contains ("width")) {
						
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

				if (!this.myData.loadPercent) {
						setEvenPercentages ();
				}

		}


		private string[] getColorsHexArray (Color[] colors)
		{
				string[] hexArray = new string[5];
				for (int i = 0; i < colors.Length; i++) {
						hexArray [i] = JSONPersistor.ColorToHex (colors [i]);
				}
				return hexArray;
		}

		private Color[] getHexArrayColors (string[] hexArray)
		{
				Color[] colors = new Color[5];
				for (int i = 0; i < hexArray.Length; i++) {
						colors [i] = JSONPersistor.HexToColor (hexArray [i]);
				}
				return colors;
		}



}
