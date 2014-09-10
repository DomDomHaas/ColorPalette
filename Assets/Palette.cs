using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

namespace ColorPalette
{

		public class Palette : JSONPersistent
		{

				public PaletteData myData = new PaletteData ();

				// Use this for initialization
				void Awake ()
				{
						init ();
				}

				public void init ()
				{
						fileName = getFileName ();

						myData.totalWidth = 0;

						if (JSONPersistor.Instance.fileExists (fileName)) {
								load ();
						} else {
								//default Palette
								
								myData.colors = PaletteData.getDefaultColors ();
								myData.alphas = PaletteData.getDefaultAlphas ();
								myData.percentages = PaletteData.getDefaultPercentages ();
						}
				}
	
				// Update is called once per frame
				void Update ()
				{
	
				}

				public override SimpleJSON.JSONClass getDataClass ()
				{
						return this.myData.getJsonPalette ();
/*						JSONClass jClass = new JSONClass ();

						string[] hexArray = JSONPersistor.getHexArrayFromColors (myData.colors);

						for (int i = 0; i < hexArray.Length; i++) {
								jClass ["colors"] [i] = hexArray [i];
						}

						for (int i = 0; i < myData.alphas.Length; i++) {
								jClass ["alphas"] [i].AsFloat = myData.colors [i].a;
						}

			
						for (int i = 0; i < myData.percentages.Length; i++) {
								jClass ["percentages"] [i].AsFloat = myData.percentages [i];
						}

						jClass ["totalWidth"].AsFloat = myData.totalWidth;

						//Debug.Log ("getDataClass: " + jClass ["colors"].Count + " " + jClass ["percentages"].Count);
						return jClass;
*/
				}

				public override void setClassData (SimpleJSON.JSONClass jClass)
				{
						this.myData.setPalette (jClass);
/*						int size = jClass ["colors"].Count;

						string[] hexArray = new string[size];

						for (int i = 0; i < size; i++) {
								hexArray [i] = jClass ["colors"] [i];
						}
						
						myData.colors = JSONPersistor.getColorsArrayFromHex (hexArray);
				

						size = jClass ["alphas"].Count;
			
						// if the size of the file is different than the standard size -> init()
						if (myData.alphas.Length != size) {
								myData.alphas = new float[size];
						}

						for (int i = 0; i < size; i++) {
								float alphaValue = jClass ["alphas"] [i].AsFloat;
								myData.alphas [i] = alphaValue;
								myData.colors [i].a = alphaValue;
						}

						size = jClass ["percentages"].Count;

						// if the size of the file is different than the standard size -> init()
						if (myData.percentages.Length != size) {
								myData.percentages = new float[size];
						}

						for (int i = 0; i < size; i++) {
								myData.percentages [i] = jClass ["percentages"] [i].AsFloat;
						}

						myData.totalWidth = jClass ["totalWidth"].AsFloat;*/
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

										float[] newAlphas = new float[newSize];
										myData.alphas.CopyTo (newAlphas, 0);
										myData.alphas = newAlphas;

										float[] newPercentages = new float[newSize];
										myData.percentages.CopyTo (newPercentages, 0);
										myData.percentages = newPercentages;

										// when adding a new Color the % will adjust automaticlly due to the 
										// inspector script

										return true;
								} else {
										int sizeDiff = myData.colors.Length - newSize;

										Color[] newColors = new Color[newSize];
										float[] newAlphas = new float[newSize];
										float[] newPercentages = new float[newSize];

										for (int i = 0; i < newColors.Length; i++) {
												newColors [i] = myData.colors [i];
												newAlphas [i] = myData.colors [i].a;
												newPercentages [i] = myData.percentages [i];
										}
										myData.colors = newColors;
										myData.alphas = newAlphas;
										myData.percentages = newPercentages;

										// when removing though, the last value will be streched
										fillUpLastPercentage (sizeDiff);

										return true;
								}
						}
		
						return false;
				}

				protected void fillUpLastPercentage (int sizeDifference)
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
			


		}
}