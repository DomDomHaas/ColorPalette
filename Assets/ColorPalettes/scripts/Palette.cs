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

				public PaletteData myData;

				// Use this for initialization
				new void Awake ()
				{
						init ();
				}

				public void init ()
				{
						myData = new PaletteData ();

						fileName = getFileName ();


						if (JSONPersistor.Instance.fileExists (fileName)) {
								load ();
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

				public override string getFileName ()
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
			


		}
}