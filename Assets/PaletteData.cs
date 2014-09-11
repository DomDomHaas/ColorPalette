using System;
using UnityEngine;
using SimpleJSON;

namespace ColorPalette
{

		[Serializable]
		public class PaletteData : UnityEngine.Object
		{

				public PaletteData (string name = null)
				{
						if (string.IsNullOrEmpty (name)) {
								this.name = this.GetInstanceID ().ToString ();
						} else {
								this.name = name;
						}
						this.colors = getDefaultColors ();

						this.alphas = getDefaultAlphas ();
						this.percentages = getDefaultPercentages ();

						this.totalWidth = 0;
				}

				[SerializeField]
				new public string
						name;

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


		#region publicMethods

				public virtual JSONClass getJsonPalette ()
				{
						JSONClass jClass = new JSONClass ();

						jClass ["name"] = name;

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
			
						//Debug.Log ("name: " + this.name + " " + jClass ["colors"].ToString () + " " + jClass ["percentages"].ToString ());
			
						return jClass;
				}


				public virtual void setPalette (JSONClass jClass)
				{
						int size = jClass ["colors"].Count;

						name = jClass ["name"];

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


				/// <summary>
				/// changes the size of the colors and the other arrays. Make sure to initialize it first!
				/// </summary>
				/// <returns><c>true</c>, if size was changed, <c>false</c> otherwise.</returns>
				/// <param name="newSize">New size.</param>
				public bool setSize (int newSize)
				{
						if (newSize != this.colors.Length) {
				
								if (newSize > this.colors.Length) {
					
										Color[] newColors = new Color[newSize];
										this.colors.CopyTo (newColors, 0);
										this.colors = newColors;
					
										float[] newAlphas = new float[newSize];
										this.alphas.CopyTo (newAlphas, 0);
										this.alphas = newAlphas;
					
										float[] newPercentages = new float[newSize];
										this.percentages.CopyTo (newPercentages, 0);
										this.percentages = newPercentages;
					
										// when adding a new Color the % will adjust automaticlly due to the 
										// inspector script
					
										return true;
								} else {
										int sizeDiff = this.colors.Length - newSize;
					
										Color[] newColors = new Color[newSize];
										float[] newAlphas = new float[newSize];
										float[] newPercentages = new float[newSize];
					
										for (int i = 0; i < newColors.Length; i++) {
												newColors [i] = this.colors [i];
												newAlphas [i] = this.colors [i].a;
												newPercentages [i] = this.percentages [i];
										}
										this.colors = newColors;
										this.alphas = newAlphas;
										this.percentages = newPercentages;
					
										// when removing though, the last value will be streched
										fillUpLastPercentage (sizeDiff);
					
										return true;
								}
						}
			
						return false;
				}

				public float getTotalPct ()
				{
						float total = 0;
						foreach (float pct in this.percentages) {
								total += pct;
						}
						return total;
				}

				public void setName (string name)
				{
						this.name = name;
				}

		#endregion

				protected void fillUpLastPercentage (int sizeDifference)
				{
						float currentTotal = getTotalPct ();
						if (currentTotal < 1) {
								this.percentages [this.percentages.Length - 1] += (1 - currentTotal);
						}
				}


		#region staticMethods

				public static PaletteData getInstance (JSONClass jClass)
				{
						PaletteData palette = new PaletteData ();
						//Debug.Log ("getInstance: " + jClass.ToString ());
						palette.setPalette (jClass);
						return palette;
				}

				public static Color[] getDefaultColors ()
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

		#endregion

		}
}

