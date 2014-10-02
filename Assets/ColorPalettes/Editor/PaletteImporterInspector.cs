using UnityEngine;
using UnityEditor;
using System.Collections; 
using System.Xml.Serialization;
using ColorPalette;

[CustomEditor(typeof(PaletteImporter))]
public class PaletteImporterInspector : PaletteInspector
{ 

		private string URL = "";
		private bool showImporter = false;

	
		private PaletteImporter myPaletteImporter;


		[ExecuteInEditMode]
		new public void OnEnable ()
		{
				base.OnEnable ();

				myPaletteImporter = target as PaletteImporter;
				myPaletteImporter.init ();
				URL = myPaletteImporter.myImporterData.paletteURL;
		}

		public override void OnInspectorGUI ()
		{    
				// uncomment for debugging
				//base.DrawDefaultInspector ();

				myPaletteImporter = target as PaletteImporter;
		
				// margin box before buttons
				GUILayoutUtility.GetRect (Screen.width, 10);


				showImporter = EditorGUILayout.Foldout (showImporter, " Import Palette");
		
				if (showImporter) {
						drawURLImporter ();
				}



				showPalette = EditorGUILayout.Foldout (showPalette, myPaletteImporter.myImporterData.name + " ColorPalette");
		
				if (showPalette) {
						myPaletteImporter.myImporterData = drawColorPalette (myPaletteImporter.myImporterData) as PaletteImporterData;
				}
		
		
				changeColors = EditorGUILayout.Foldout (changeColors, " Change Colors");
		
				if (changeColors) {
						myPaletteImporter.myImporterData = drawColorsAndPercentages (myPaletteImporter.myImporterData) as PaletteImporterData;
				}
		
				// margin box
				GUILayoutUtility.GetRect (Screen.width, 25);
		
				myPaletteImporter.myImporterData = drawSizeButtons (myPaletteImporter.myImporterData) as PaletteImporterData;
		
				drawSaveButtons ();
		
				// margin box
				GUILayoutUtility.GetRect (Screen.width, 25);
		 

		} 

		protected void drawURLImporter ()
		{
				// margin box
				GUILayoutUtility.GetRect (Screen.width, 10);

				GUILayout.Label ("Insert an URL: ");
				string newURL = EditorGUILayout.TextField (URL);
				if (!string.IsNullOrEmpty (newURL)) {
						URL = newURL;
				}

				GUILayoutUtility.GetRect (Screen.width, 10);

				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.BeginVertical ();

				myPaletteImporter.myImporterData.loadPercent = GUILayout.Toggle (myPaletteImporter.myImporterData.loadPercent, " include Palette Percentage");
		
				EditorGUILayout.EndVertical ();

				bool import = GUILayout.Button (new GUIContent ("Import from URL", "this might take a few seconds!"),
		                                		GUILayout.Width (Screen.width / 2));

				Event e = Event.current;
		
				EditorGUILayout.EndHorizontal ();
		
				if (import) {
						Debug.Log ("import started with " + URL);
						myPaletteImporter.ImportPalette (URL);
			
				} else if (e.type == EventType.MouseUp) {
						// after a non import click check for a URL update (in case of the loadFromFile the url changes)
						if (myPaletteImporter.myImporterData.paletteURL != URL) {
								URL = myPaletteImporter.myImporterData.paletteURL;
						}
				}

				GUILayoutUtility.GetRect (Screen.width, 10);
		}

		new protected virtual void drawSaveButtons ()
		{
		
				GUILayout.Space (10);
		
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("Storing the Palette");
				EditorGUILayout.EndHorizontal ();
		
				EditorGUILayout.BeginHorizontal ();
		
				if (GUILayout.Button ("Save to File", GUILayout.Width (Screen.width / 2))) { 
						myPaletteImporter.save ();
				} 
		
				if (GUILayout.Button ("Load from File", GUILayout.Width (Screen.width / 2))) { 
						myPaletteImporter.load ();
				} 
		
				EditorGUILayout.EndHorizontal ();
		}


}