using System;
using UnityEngine;
using SimpleJSON;

namespace ColorPalette
{

		[Serializable]
		public class PaletteData
		{
				public PaletteData ()
				{
				}

				[SerializeField]
				public Color[]
						colors;

				[SerializeField]
				public float[]
						alphas;

				[SerializeField]
				public float[]
						percentages;
				
				[SerializeField]
				public float
						totalWidth;


				public virtual JSONClass getJsonPalette ()
				{
						JSONClass jClass = new JSONClass ();
			
						string[] hexArray = JSONPersistor.getHexArrayFromColors (this.colors);
			
						for (int i = 0; i < hexArray.Length; i++) {
								jClass ["colors"] [i] = hexArray [i];
						}
			
						for (int i = 0; i < this.alphas.Length; i++) {
								jClass ["alphas"] [i].AsFloat = this.colors [i].a;
						}
			
			
						for (int i = 0; i < this.percentages.Length; i++) {
								jClass ["percentages"] [i].AsFloat = this.percentages [i];
						}
			
						jClass ["totalWidth"].AsFloat = this.totalWidth;
			
						//Debug.Log ("getDataClass: " + jClass ["colors"].Count + " " + jClass ["percentages"].Count);
						return jClass;
				}


				public virtual void setPalette (JSONClass jClass)
				{
						int size = jClass ["colors"].Count;
			
						string[] hexArray = new string[size];
			
						for (int i = 0; i < size; i++) {
								hexArray [i] = jClass ["colors"] [i];
						}
			
						this.colors = JSONPersistor.getColorsArrayFromHex (hexArray);
			
			
						size = jClass ["alphas"].Count;
			
						// if the size of the file is different than the standard size -> init()
						if (this.alphas.Length != size) {
								this.alphas = new float[size];
						}
			
						for (int i = 0; i < size; i++) {
								float alphaValue = jClass ["alphas"] [i].AsFloat;
								this.alphas [i] = alphaValue;
								this.colors [i].a = alphaValue;
						}
			
						size = jClass ["percentages"].Count;
			
						// if the size of the file is different than the standard size -> init()
						if (this.percentages.Length != size) {
								this.percentages = new float[size];
						}
			
						for (int i = 0; i < size; i++) {
								this.percentages [i] = jClass ["percentages"] [i].AsFloat;
						}
			
						this.totalWidth = jClass ["totalWidth"].AsFloat;
				}


				public static PaletteData getInstance ()
				{
						PaletteData palette = new PaletteData ();
						palette.colors = getDefaultColors ();
						palette.alphas = getDefaultAlphas ();
						palette.percentages = getDefaultPercentages ();
						palette.totalWidth = 0;

						return palette;
				}

				public static color[] getDefaultColors ()
				{
						return JSONPersistor.getColorsArrayFromHex (new string[]{"69D2E7", "A7DBD8", "E0E4CC", "F38630", "FA6900"});
				}

				public static float[] getDefaultAlphas ()
				{
						return new float[]{1f, 1f, 1f, 1f, 1f};
				}
			
				public static float[] getDefaultPercentages ()
				{
						return new float[]{0.2f, 0.2f, 0.2f, 0.2f, 0.2f};
				}



		}
}

