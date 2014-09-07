using UnityEngine;
using UnityEditor;

using System.Collections; 
using System.Xml.Serialization; 

[CustomEditor(typeof(ColorImporter))]
public class ColorImporterInspector : Editor
{ 
		public float
				height = 100;

		private string URL = "";
		private bool pickColors = false;
		private bool pickPercentages = false;

		private bool adjustPCTBefore = false;
		private float minPct = 0.01f;

		private ColorImporter myImporter;


		[ExecuteInEditMode]
		public void OnEnable ()
		{
				myImporter = target as ColorImporter;
				myImporter.load ();
				URL = myImporter.myData.paletteURL;
		}

		public override void OnInspectorGUI ()
		{    
				//base.DrawDefaultInspector ();

				myImporter = target as ColorImporter;
				
				// margin box on top
				GUILayoutUtility.GetRect (Screen.width, 25);

				GUILayout.Label ("Insert an URL: ");
				string newURL = EditorGUILayout.TextField (URL);
				if (!string.IsNullOrEmpty (newURL)) {
						URL = newURL;
				}

				myImporter.myData.loadPercent = GUILayout.Toggle (myImporter.myData.loadPercent, " include Palette Percentage");

		
				// margin box before buttons
				GUILayoutUtility.GetRect (Screen.width, 25);

				bool import = GUILayout.Button ("import Palette from URL", GUILayout.Width (Screen.width / 2));
				Event e = Event.current;
		
				if (import) {
						Debug.Log ("import started with " + URL);
						myImporter.ImportPalette (URL);

				} else if (e.type == EventType.MouseUp) {
						// after a non import click check for a URL update (in case of the loadFromFile the url changes)
						if (myImporter.myData.paletteURL != URL) {
								URL = myImporter.myData.paletteURL;
						}
				}
		

		
				if (GUILayout.Button ("Save to File", GUILayout.Width (Screen.width / 2))) { 
						myImporter.save ();
				} 

				if (GUILayout.Button ("Load from File", GUILayout.Width (Screen.width / 2))) { 
						myImporter.load ();
				} 



				// palette height
				height = EditorGUILayout.Slider ("palette height", height, 100, 250);

				float paletteHeight = height;
				float paletteMargin = 60;

				Rect paletteRect = GUILayoutUtility.GetRect (Screen.width, paletteHeight + paletteMargin);

				// show the palette

				float start = 20;
				float end = 0;
				for (int i = 0; i < myImporter.myData.colors.Length; i++) {
						Color col = myImporter.myData.colors [i];
						float width = myImporter.myData.percentages [i] * (Screen.width - 35);
						Rect colRect = new Rect (start,
			                         paletteRect.position.y + paletteMargin,
			                         width,
			                         paletteHeight - paletteMargin);

						EditorGUIUtility.DrawColorSwatch (colRect, col);

						Rect lableRect = colRect;
						lableRect.width = 60;
						lableRect.height = 15;

						lableRect.y -= paletteMargin * 0.5f;

						if (i % 2 == 0) {
								lableRect.y -= 15;
						}


						string hexString = "#" + JSONPersistor.ColorToHex (col);
						EditorGUI.TextField (lableRect, hexString);


						start += width;
				}

				pickColors = GUILayout.Toggle (pickColors, " Pick Colors");

				if (pickColors) {
						foreach (Color col in myImporter.myData.colors) {
								EditorGUILayout.ColorField (col);
						}
				}

				pickPercentages = GUILayout.Toggle (pickPercentages, " Pick Percent");
		
				if (pickPercentages) {
						drawPercentageUI ();
				}

				EditorUtility.SetDirty (myImporter);
		} 


		private void drawColorPicker ()
		{

		}

		private void drawPercentageUI ()
		{

				// margin box before buttons
				GUILayoutUtility.GetRect (Screen.width, 20);

				adjustPCTBefore = GUILayout.Toggle (adjustPCTBefore, " adjust percentage to the left");

				// margin box before buttons
				GUILayoutUtility.GetRect (Screen.width, 20);

				/*
		if (){

		}
		*/

				for (int i = 0; i < myImporter.myData.percentages.Length; i++) {

						Rect percentageRow = EditorGUILayout.BeginHorizontal ();

						// draw a little preview of the current color
						Rect colRect = new Rect (percentageRow.x, percentageRow.y, percentageRow.width * 0.35f, percentageRow.height);
						

						//EditorGUILayout.BeginVertical ();

						EditorGUIUtility.DrawColorSwatch (colRect, myImporter.myData.colors [i]);

						//EditorGUILayout.EndVertical ();

						float pct = myImporter.myData.percentages [i];
						float maxPct = 1.0f - myImporter.myData.percentages.Length * this.minPct;


						//EditorGUILayout.BeginVertical ();

						float newPct = EditorGUILayout.Slider (" ", pct, this.minPct, maxPct);

						//EditorGUILayout.EndVertical ();

						if (newPct != pct) {
								//Debug.Log ("change on " + i + " old " + pct + " new " + newPct);

								adjustNeighborPCT (i, pct - newPct);

								myImporter.myData.percentages [i] = newPct;
						}
						//GUILayout.TextField (pct.ToString ());

						EditorGUILayout.EndHorizontal ();

				}

		}

		private void adjustNeighborPCT (int i, float pctDiff)
		{
				if (adjustPCTBefore) {
						if (i - 1 >= 0) {
/*								float pctToTheLeft = myImporter.myData.percentages [i - 1];
								pctToTheLeft += pctDiff;
								myImporter.myData.percentages [i - 1] = pctToTheLeft;
*/
								myImporter.myData.percentages [i - 1] += pctDiff;
						} else {
								myImporter.myData.percentages [myImporter.myData.percentages.Length - 1] += pctDiff;
/*							float pctFarRight = myImporter.myData.percentages [myImporter.myData.percentages.Length - 1];
								pctFarRight += pctDiff;
								myImporter.myData.percentages [i - 1] = pctToTheLeft;
*/
						}
				} else {
						if (i + 1 <= myImporter.myData.percentages.Length - 1) {
/*								float pctToTheRight = myImporter.myData.percentages [i + 1];
								pctToTheRight += pctDiff;
								myImporter.myData.percentages [i + 1] = pctToTheRight;
*/
								myImporter.myData.percentages [i + 1] += pctDiff;
						} else {
								myImporter.myData.percentages [0] += pctDiff;
						}
				}

		} 



}