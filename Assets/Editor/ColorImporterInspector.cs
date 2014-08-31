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
						foreach (float pct in myImporter.myData.percentages) {
								GUILayout.TextField (pct.ToString ());
						}
				}

				EditorUtility.SetDirty (myImporter);
		} 
	

} 


