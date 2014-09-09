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

	
		private PaletteImporter myPalette;


		[ExecuteInEditMode]
		public void OnEnable ()
		{
				myPalette = target as PaletteImporter;
				myPalette.init ();
				URL = ((PaletteImporterData)myPalette.myData).paletteURL;
		}

		public override void OnInspectorGUI ()
		{    
				// uncomment for debugging
				//base.DrawDefaultInspector ();

				myPalette = target as PaletteImporter;
		
				// margin box before buttons
				GUILayoutUtility.GetRect (Screen.width, 10);


				showImporter = EditorGUILayout.Foldout (showImporter, " Import Palette");
		
				if (showImporter) {
						drawURLImporter ();
				}

				base.OnInspectorGUI ();
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
		
				((PaletteImporterData)myPalette.myData).loadPercent = GUILayout.Toggle (((PaletteImporterData)myPalette.myData).loadPercent, " include Palette Percentage");
		
		
				bool import = GUILayout.Button ("import Palette from URL", GUILayout.Width (Screen.width / 2));
				Event e = Event.current;
		
				EditorGUILayout.EndHorizontal ();
		
				if (import) {
						Debug.Log ("import started with " + URL);
						myPalette.ImportPalette (URL);
			
				} else if (e.type == EventType.MouseUp) {
						// after a non import click check for a URL update (in case of the loadFromFile the url changes)
						if (((PaletteImporterData)myPalette.myData).paletteURL != URL) {
								URL = ((PaletteImporterData)myPalette.myData).paletteURL;
						}
				}

				GUILayoutUtility.GetRect (Screen.width, 10);
		}


}