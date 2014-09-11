using UnityEngine;
using UnityEditor;
using System.Collections; 
using System.Collections.Generic;
using System.Xml.Serialization;
using ColorPalette;

[CustomEditor(typeof(PaletteCollection))]
public class PaletteCollectionInspector : PaletteInspector
{ 

		private string URL = "";
		//private bool showImporter = false;
		private bool[] showPalette;

		private IDictionary<string, PaletteData> changeKeys = new Dictionary<string, PaletteData> ();
		private PaletteCollection myCollection;


		[ExecuteInEditMode]
		new public void OnEnable ()
		{
				myCollection = target as PaletteCollection;
				myCollection.init ();

				this.height = 50;

				if (showPalette == null) {
						showPalette = new bool[myCollection.collectionData.palettes.Count];
						for (int i = 0; i < showPalette.Length; i++) {
								showPalette [i] = false;
						}
				}
				URL = myCollection.collectionData.paletteURL;
		}

		public override void OnInspectorGUI ()
		{    
				// uncomment for debugging
				//base.DrawDefaultInspector ();

				myCollection = target as PaletteCollection;
		
				// margin box before buttons
				GUILayout.Space (10);

/*				showImporter = EditorGUILayout.Foldout (showImporter, " Import Palette Collection");
		
				if (showImporter) {
						drawURLImporter ();
				}*/

				drawAllPalettes ();

				drawSaveButtons ();
		} 

		protected void drawURLImporter ()
		{
				// margin box
				GUILayoutUtility.GetRect (Screen.width, 10);

				GUILayout.Label (new GUIContent ("Insert an URL to multiple Palettes: ", "Might take a while!"));
				string newURL = EditorGUILayout.TextField (URL);
				if (!string.IsNullOrEmpty (newURL)) {
						URL = newURL;
				}

				GUILayoutUtility.GetRect (Screen.width, 10);

				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.BeginVertical ();
		
				myCollection.collectionData.loadPercent = GUILayout.Toggle (myCollection.collectionData.loadPercent, " include Palette Percentage");
		
				EditorGUILayout.EndVertical ();
				EditorGUILayout.BeginVertical ();


				bool import = GUILayout.Button (new GUIContent ("Import a Collection from URL", "Might take a while!"),
		                                		GUILayout.Width (Screen.width / 2));

				EditorGUILayout.Toggle ("Import is Running ", myCollection.importIsRunning);
				EditorGUILayout.LabelField ("Bytes downloaded: ", myCollection.importedBytes.ToString ());
				
				//EditorGUI.ProgressBar (EditorGUILayout.GetControlRect (GUILayout.Height (20)), myCollection.importProgress, "Import progress");

				EditorGUILayout.EndVertical ();



				Event e = Event.current;
		
				EditorGUILayout.EndHorizontal ();
		
				if (import) {
						Debug.Log ("import started with " + URL);
						myCollection.ImportPaletteCollection (URL);
			
				} else if (e.type == EventType.MouseUp) {
						// after a non import click check for a URL update (in case myCollectionadFromFile the url changes)
						if (myCollection.collectionData.paletteURL != URL) {
								URL = myCollection.collectionData.paletteURL;
						}
				}

		}


		protected void drawAllPalettes ()
		{

				GUILayout.Space (15);

				int i = 0;
				if (this.showPalette != null) {
						if (this.showPalette.Length == myCollection.collectionData.palettes.Count) {

								foreach (KeyValuePair<string, PaletteData> kvp in myCollection.collectionData.palettes) {

										this.showPalette [i] = EditorGUILayout.Foldout (this.showPalette [i], " Open '" + kvp.Key + "' Palette");

										if (this.showPalette [i]) {
												//				myCollection.collectionData.palettes [kvp.Key] = 
												PaletteData changeData = base.drawColorPalette (kvp.Value);
												if (changeData.name != kvp.Key) {
														changeKeys.Add (kvp.Key, changeData);
												}
												//myCollection.collectionData.palettes [kvp.Key] =
												base.drawColorsAndPercentages (kvp.Value);
												//myCollection.collectionData.palettes [kvp.Key] =
												base.drawSizeButtons (kvp.Value);										
										}

										i++;
								}

								GUILayout.Space (25);

								if (changeKeys.Count > 0) {
										foreach (KeyValuePair<string, PaletteData> kvp in changeKeys) {
												myCollection.collectionData.palettes.Remove (kvp.Key);
												myCollection.collectionData.palettes.Add (kvp.Value.name, kvp.Value);
										}
										changeKeys = new Dictionary<string, PaletteData> ();
								}

						} else {
								Debug.Log ("showPalettes " + this.showPalette.Length + " palettes " + myCollection.collectionData.palettes.Count);
						}
				}
				
		}

		private void changeShowPalette (int newSize)
		{
				if (this.showPalette != null) {
						bool[] newBools = new bool[newSize];

						this.showPalette.CopyTo (newBools, 0);
						this.showPalette = newBools;
						this.showPalette [this.showPalette.Length - 1] = true;
				} else {
						this.showPalette = new bool[1]{true};
				}
		}

		protected override void drawSaveButtons ()
		{		
				GUILayout.Space (25);

				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("Collection functions");
				EditorGUILayout.EndHorizontal ();
		
				EditorGUILayout.BeginHorizontal ();

				if (GUILayout.Button (new GUIContent ("Add a new Palette", "Is added to the end"),
		                      GUILayout.Width (Screen.width / 2))) { 
						if (myCollection.CreatePalette ()) {
								changeShowPalette (myCollection.collectionData.palettes.Count);
						}
				} 
		
				if (GUILayout.Button (new GUIContent ("Remove last Palette", "Might take a while!"),
		                      GUILayout.Width (Screen.width / 2))) { 
						//myCollection.RemovePalette();
				} 
		
				EditorGUILayout.EndHorizontal ();


/*				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("Storing the Collection");
				EditorGUILayout.EndHorizontal ();
*/		
				EditorGUILayout.BeginHorizontal ();

				if (GUILayout.Button (new GUIContent ("Save Collection to file", "Might take a while!"),
		                      GUILayout.Width (Screen.width / 2))) { 
						myCollection.save ();
				} 
		
				if (GUILayout.Button (new GUIContent ("Load Collection from file", "Might take a while!")
		                      , GUILayout.Width (Screen.width / 2))) { 
						myCollection.load ();
				} 
		
				EditorGUILayout.EndHorizontal ();
		}


}