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
		private bool showImporter = false;
		private bool[] showPalettes;

		private IDictionary<string, PaletteData> changeKeys = new Dictionary<string, PaletteData> ();
		private PaletteCollection myCollection;

		private Texture2D minusPalette;
		private Texture2D plusPalette;

		[ExecuteInEditMode]
		new public void OnEnable ()
		{
				loadButtonTextures ();

				myCollection = target as PaletteCollection;
				myCollection.init ();

				this.height = 50;

				if (showPalettes == null) {
						showPalettes = new bool[myCollection.collectionData.palettes.Count];
						for (int i = 0; i < showPalettes.Length; i++) {
								showPalettes [i] = false;
						}
				}
				URL = myCollection.collectionData.paletteURL;
		}

		protected override void loadButtonTextures (string pathToTextures = null)
		{

				string folderPath = "";
				if (string.IsNullOrEmpty (pathToTextures)) {
						folderPath = Application.dataPath + "/ColorPalettes/Editor/";
				} else {
						folderPath = pathToTextures;
				}

				base.loadButtonTextures (folderPath);

				plusPalette = JSONPersistor.getTextureFromWWW (folderPath + "plus_palette.png");
				plusPalette.hideFlags = HideFlags.HideAndDontSave;
				minusPalette = JSONPersistor.getTextureFromWWW (folderPath + "minus_palette.png");
				minusPalette.hideFlags = HideFlags.HideAndDontSave;
		}

	
		[ExecuteInEditMode]
		public void OnDisable ()
		{
				DestroyImmediate (plusTex);
				DestroyImmediate (minusTex);
				DestroyImmediate (plusPalette);
				DestroyImmediate (minusPalette);
		}

		public override void OnInspectorGUI ()
		{    
				// uncomment for debugging
				//base.DrawDefaultInspector ();

				myCollection = target as PaletteCollection;
		
				// margin box before buttons
				GUILayout.Space (10);

				showImporter = EditorGUILayout.Foldout (showImporter, " Import Palette to Collection");
		
				if (showImporter) {
						drawURLImporter ();
				}

				drawAllPalettes ();

				drawSaveButtons ();
		} 

		protected void drawURLImporter ()
		{
				// margin box
				GUILayoutUtility.GetRect (Screen.width, 10);

				GUILayout.Label (new GUIContent ("Insert an URL: ", "Might take a while!"));
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


				bool import = GUILayout.Button (new GUIContent ("Import Palette to Collection", "Might take a while!"),
		                                		GUILayout.Width (Screen.width / 2));

				EditorGUILayout.EndVertical ();

				EditorGUILayout.EndHorizontal ();



				Event e = Event.current;

				if (import) {
						Debug.Log ("import started with " + URL);
						myCollection.ImportPaletteCollection (URL);

/*						if (myCollection.ImportPaletteCollection (URL)) {
								changeShowPalette (myCollection.collectionData.palettes.Count);
						}
*/			
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
				if (this.showPalettes != null) {
						if (this.showPalettes.Length != myCollection.collectionData.palettes.Count) {
								changeShowPalette (myCollection.collectionData.palettes.Count);
						}

						foreach (KeyValuePair<string, PaletteData> kvp in myCollection.collectionData.palettes) {

								this.showPalettes [i] = EditorGUILayout.Foldout (this.showPalettes [i], kvp.Key + " ColorPalette");

								if (this.showPalettes [i]) {
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


						if (changeKeys.Count > 0) {
								foreach (KeyValuePair<string, PaletteData> kvp in changeKeys) {
										myCollection.collectionData.palettes.Remove (kvp.Key);
										myCollection.collectionData.palettes.Add (kvp.Value.name, kvp.Value);
								}
								changeKeys = new Dictionary<string, PaletteData> ();
						}

/*				} else {
						Debug.Log ("showPalettes " + this.showPalettes.Length + " palettes " + myCollection.collectionData.palettes.Count);
*/
				}
				
				
		}

		private void changeShowPalette (int newSize)
		{
				if (this.showPalettes != null) {
						if (newSize > this.showPalettes.Length) {
								bool[] newBools = new bool[newSize];

								this.showPalettes.CopyTo (newBools, 0);
								this.showPalettes = newBools;
								this.showPalettes [this.showPalettes.Length - 1] = true;
						} else {

								bool[] newBools = new bool[newSize];
								for (int i = 0; i < newSize; i++) {
										newBools [i] = this.showPalettes [i];
								}
								this.showPalettes = newBools;
						}
				} else {
						this.showPalettes = new bool[1]{true};
				}
		}

		protected override void drawSaveButtons ()
		{		
				GUILayout.Space (25);

				Rect collectionRect = EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("Collection functions");
				EditorGUILayout.EndHorizontal ();
		

				if (GUI.Button (new Rect (Screen.width - 50 - 40 - buttonMarginBetween, collectionRect.y, 40, 40), new GUIContent (plusPalette, "Add new Palette"), EditorStyles.miniButtonRight)) { 
						myCollection.collectionData.setSize (myCollection.collectionData.palettes.Count + 1);
				}

				if (GUI.Button (new Rect (Screen.width - 50, collectionRect.y, 40, 40), new GUIContent (minusPalette, "Remove last Palette"), EditorStyles.miniButton)) { 
						myCollection.collectionData.setSize (myCollection.collectionData.palettes.Count - 1);
				}


				GUILayout.Space (40);

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