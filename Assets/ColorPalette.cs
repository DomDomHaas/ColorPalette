using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using HtmlSharp;
using HtmlSharp.Elements;
using SimpleJSON;

public class ColorPalette : JSONPersistent
{

		private bool isColourLovers = false;
		private bool isPLTTS = false;
		private bool isLocalFile = false;


		[Serializable]
		public class PaletteData
		{

				[SerializeField]
				public string
						paletteURL;

				[SerializeField]
				public bool
						loadPercent;


				[SerializeField]
				public Color[]
						colors;

				[SerializeField]
				public float[]
						percentages;

				[SerializeField]
				public float
						totalWidth;
		}

		public PaletteData myData = new PaletteData ();


		// Use this for initialization
		void Awake ()
		{
				init ();
		}

		public void init ()
		{
				myData.paletteURL = "";
				myData.loadPercent = false;
				myData.colors = new Color[5];
				myData.percentages = new float[5];
				myData.totalWidth = 0;

				if (FileExists ()) {
						load ();
				} else {
						string[] hexArray = new string[]{"69D2E7", "A7DBD8", "E0E4CC", "F38630", "FA6900"};
						myData.colors = JSONPersistor.getColorsArrayFromHex (hexArray);
						setEvenPercentages ();
				}
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
			
				string[] hexArray = JSONPersistor.getHexArrayFromColors (myData.colors);

				for (int i = 0; i < hexArray.Length; i++) {
						jClass ["colors"] [i] = hexArray [i];
				}

				for (int i = 0; i < myData.percentages.Length; i++) {
						jClass ["percentages"] [i].AsFloat = myData.percentages [i];
				}

				jClass ["totalWidth"].AsFloat = myData.totalWidth;

				//Debug.Log ("getDataClass: " + jClass ["colors"].Count + " " + jClass ["percentages"].Count);
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
						
				myData.colors = JSONPersistor.getColorsArrayFromHex (hexArray);
				
				size = jClass ["percentages"].Count;

				if (myData.percentages.Length != size) {
						// if the size of the file is different than the standard size -> init()
						myData.percentages = new float[size];
				}

				for (int i = 0; i < size; i++) {
						myData.percentages [i] = jClass ["percentages"] [i].AsFloat;
				}

				myData.totalWidth = jClass ["totalWidth"].AsFloat;
		}

		public virtual string getFileName ()
		{
				return this.gameObject.name + "_myData";
		}

		public bool setSize (int newSize)
		{
				if (newSize != myData.colors.Length) {

						if (newSize > myData.colors.Length) {

								Color[] newColors = new Color[newSize];
								myData.colors.CopyTo (newColors, 0);
								myData.colors = newColors;

								float[] newPercentages = new float[newSize];
								myData.percentages.CopyTo (newPercentages, 0);
								myData.percentages = newPercentages;

								// when adding a new Color the % will adjust automaticlly due to the 
								// inspector script

								return true;
						} else {
								int sizeDiff = myData.colors.Length - newSize;

								Color[] newColors = new Color[newSize];
								float[] newPercentages = new float[newSize];
								for (int i = 0; i < newColors.Length; i++) {
										newColors [i] = myData.colors [i];
										newPercentages [i] = myData.percentages [i];
								}
								myData.colors = newColors;
								myData.percentages = newPercentages;

								// when removing though, the last value will be streched
								fillUpLastPercentage (sizeDiff);

								return true;
						}
				}
		
				return false;
		}

		private void fillUpLastPercentage (int sizeDifference)
		{
				float currentTotal = getTotalPct ();
				if (currentTotal < 1) {
						myData.percentages [myData.percentages.Length - 1] += (1 - currentTotal);
				}
		}

		public float getTotalPct ()
		{
				float total = 0;
				foreach (float pct in myData.percentages) {
						total += pct;
				}
				return total;
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

}
