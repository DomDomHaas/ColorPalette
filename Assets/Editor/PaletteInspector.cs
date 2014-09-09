using UnityEngine;
using UnityEditor;
using System.Collections; 
using System.Xml.Serialization; 
using ColorPalette;

[CustomEditor(typeof(Palette))]
public class PaletteInspector : Editor
{ 
		protected float height = 100;

		protected bool showPalette = true;
		protected bool changeColors = false;

		protected bool adjustPCTBefore = false;
		protected float minPct = 0.01f;

		protected float paletteHeight = 100;
		protected float paletteTopMargin = 40;
		protected float paletteBotMargin = 20;

		protected float hexFieldWidth = 55;

		protected float colorChangerRowHeight = 20;
		protected float colorChangeLeftMargin = 5;
		protected float colorChangeRightMargin = 20;
		protected float colorChangeMarginBetween = 25;

	
	
		private Palette myPalette;


		[ExecuteInEditMode]
		public void OnEnable ()
		{
				myPalette = target as Palette;
				myPalette.init ();
		}

		public override void OnInspectorGUI ()
		{    
				// uncomment for debugging
				//base.DrawDefaultInspector ();

				myPalette = target as Palette;
		
				// margin box before buttons
				GUILayoutUtility.GetRect (Screen.width, 10);


				showPalette = EditorGUILayout.Foldout (showPalette, " Palette");

				if (showPalette) {
						drawColorPalette ();
				}


				changeColors = EditorGUILayout.Foldout (changeColors, " Change Colors");

				if (changeColors) {
						drawColorsAndPercentages ();
				}

				// margin box
				GUILayoutUtility.GetRect (Screen.width, 25);

				drawButtons ();

				// margin box
				GUILayoutUtility.GetRect (Screen.width, 25);


				EditorUtility.SetDirty (myPalette);
		} 



		protected void drawColorPalette ()
		{
				// margin box
				GUILayoutUtility.GetRect (Screen.width, 10);

				// palette height silder
				//height = EditorGUILayout.Slider ("Height", height, 100, 200);
		
				//paletteHeight = height;
		
				Rect paletteRect = GUILayoutUtility.GetRect (Screen.width, paletteHeight + paletteTopMargin);
		
		
				// show the palette
				float start = 20;
				float end = 0;
				for (int i = 0; i < myPalette.myData.colors.Length; i++) {
						Color col = myPalette.myData.colors [i];
						float colWidth = myPalette.myData.percentages [i] * (Screen.width - 35);

						//Debug.Log (i + " starts " + start + " width " + colWidth);

						Rect colRect = new Rect (start,
			                         paletteRect.position.y + paletteTopMargin,
			                         colWidth,
			                         paletteHeight - paletteBotMargin);
			
						EditorGUIUtility.DrawColorSwatch (colRect, col);
			
						Rect lableRect = colRect;
						lableRect.width = 60;
						lableRect.height = 15;
			
						lableRect.y -= paletteTopMargin * 0.5f;
			
						if (i % 2 == 0) {
								lableRect.y -= 15;
						}
			
			
						string hexString = JSONPersistor.ColorToHex (col);
						Rect labelHexRect = new Rect (lableRect);
						labelHexRect.width = hexFieldWidth;


						string newHex = EditorGUI.TextField (labelHexRect, hexString);

						if (!newHex.Equals (JSONPersistor.ColorToHex (col))) {
								myPalette.myData.colors [i] = JSONPersistor.HexToColor (newHex);
						}

			
						start += colWidth;
				}

		}

		protected void drawButtons ()
		{
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("Change the size of the Palette");
				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.BeginHorizontal ();

				if (GUILayout.Button ("Add Color", GUILayout.Width (Screen.width / 2))) { 
						myPalette.setSize (myPalette.myData.colors.Length + 1);
				} 

				if (GUILayout.Button ("Remove last Color", GUILayout.Width (Screen.width / 2))) { 
						myPalette.setSize (myPalette.myData.colors.Length - 1);
				} 

				EditorGUILayout.EndHorizontal ();

				GUILayoutUtility.GetRect (Screen.width, 10);

				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("Storing the Palette");
				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.BeginHorizontal ();
		
				if (GUILayout.Button ("Save to File", GUILayout.Width (Screen.width / 2))) { 
						myPalette.save ();
				} 
		
				if (GUILayout.Button ("Load from File", GUILayout.Width (Screen.width / 2))) { 
						myPalette.load ();
				} 
		
				EditorGUILayout.EndHorizontal ();


		}

		protected void drawColorsAndPercentages ()
		{
				GUILayoutUtility.GetRect (Screen.width, 10);

				adjustPCTBefore = GUILayout.Toggle (adjustPCTBefore, " adjust percentage to the left");

				Rect colorChangerRect = GUILayoutUtility.GetRect (Screen.width, myPalette.myData.colors.Length * colorChangerRowHeight);
				colorChangerRect.x += colorChangeLeftMargin;
				colorChangerRect.width -= colorChangeRightMargin;

				GUILayoutUtility.GetRect (Screen.width, 10);

				float startY = colorChangerRect.y + 10;

				for (int i = 0; i < myPalette.myData.colors.Length; i++) {
						// draw a little preview of the current color
						Rect colRect = new Rect (colorChangerRect.x, startY,
			                         				150, colorChangerRowHeight);
			
						Color currentColor = myPalette.myData.colors [i];
						Color newColor = EditorGUI.ColorField (colRect, currentColor);
		
						string currentHex = JSONPersistor.ColorToHex (currentColor);

						Rect hexRect = new Rect (colorChangerRect.x + colRect.width + colorChangeMarginBetween,
			                         startY, hexFieldWidth, colorChangerRowHeight);

						string newHex = EditorGUI.TextField (hexRect, currentHex);

						if (!currentHex.Equals (newHex)) {
								myPalette.myData.colors [i] = JSONPersistor.HexToColor (newHex);				
						} else if (!currentColor.ToString ().Equals (newColor.ToString ())) {
								myPalette.myData.colors [i] = newColor;
								myPalette.myData.alphas [i] = newColor.a;
						}

						float currentPct = myPalette.myData.percentages [i];
						float maxPct = 1.0f - (myPalette.myData.percentages.Length - 1) * this.minPct;
						//Debug.Log ("max % " + maxPct);

						Rect silderRect = new Rect (colorChangerRect.x + colRect.width + colorChangeMarginBetween + hexRect.width + colorChangeMarginBetween, startY,
			                            colorChangerRect.width - colorChangeMarginBetween - colRect.width - colorChangeMarginBetween - hexRect.width,
			                            colorChangerRowHeight);

						float newPct = EditorGUI.Slider (silderRect, currentPct, this.minPct, maxPct);
						adjustPct (i, newPct, currentPct, maxPct);
			

						startY += colorChangerRowHeight;
				}
		}
	

		protected void adjustPct (int i, float newPct, float currentPct, float maxPct)
		{
				if (newPct < this.minPct) {
						newPct = this.minPct;
				}

				if (newPct > maxPct) {
						newPct = maxPct;
				}

				if (newPct != currentPct) {

						//if (adjustNeighborPCT (i, currentPct - newPct)) {

						adjustNeighborPCT (i, currentPct - newPct);
						myPalette.myData.percentages [i] = newPct;

						// changes been made to neighbors, check if totalPcts is still 1 -> 100%
						float totalPcts = myPalette.getTotalPct ();
				
						if (totalPcts >= 1f) {
								float rounding = totalPcts - 1f;
								if (rounding > 0) {
										//Debug.Log ("rounding " + newPct + " minus " + rounding + " to " + (newPct - rounding));
										// always cut the last for the rounding!
										myPalette.myData.percentages [myPalette.myData.percentages.Length - 1] -= rounding;
								} else if (rounding < 0) {
										myPalette.myData.percentages [myPalette.myData.percentages.Length - 1] += rounding;
								}
						}

				}


		
		}
	
		/// <summary>
		/// Adjusts the neighbor Percentage, returns false if the neighbor value woulde be under this.minPct!.
		/// </summary>
		/// <returns><c>true</c>, if neighbor PC was adjusted, <c>false</c> otherwise.</returns>
		/// <param name="i">The index.</param>
		/// <param name="pctDiff">Pct diff.</param>
		protected bool adjustNeighborPCT (int i, float pctDiff)
		{
				int neiborIndex = i;

				if (adjustPCTBefore) {
						if (i - 1 >= 0) {
								neiborIndex = i - 1;
						} else {
								neiborIndex = myPalette.myData.percentages.Length - 1;
						}
				} else {
						if (i + 1 <= myPalette.myData.percentages.Length - 1) {
								neiborIndex = i + 1;
						} else {
								neiborIndex = 0;
						}
				}

				myPalette.myData.percentages [neiborIndex] += pctDiff;
				float newNeighborValue = myPalette.myData.percentages [neiborIndex];

				if (newNeighborValue < this.minPct) {
						myPalette.myData.percentages [neiborIndex] = this.minPct;
						adjustNeighborPCT (neiborIndex, newNeighborValue - this.minPct);
				}
		
				return true;
		} 



}