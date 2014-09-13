using System;
using UnityEngine;
using System.Collections.Generic;
using SimpleJSON;

namespace ColorPalette
{
		[Serializable]
		public class PaletteCollectionData : UnityEngine.Object
		{
				public PaletteCollectionData ()
				{
						this.name = this.GetInstanceID ().ToString ();
						this.paletteURL = " ";
						this.loadPercent = false;
						this.palettes = new Dictionary<string, PaletteData> ();
				}

				[SerializeField]
				new public string
						name;

				[SerializeField]
				public string
						paletteURL;
		
				[SerializeField]
				public bool
						loadPercent;

				[SerializeField]
				public IDictionary<string, PaletteData>
						palettes;

		#region publicMethods

				public virtual JSONClass getJsonPalette ()
				{
						JSONClass jClass = new JSONClass ();

						jClass ["collectionName"] = name;

						foreach (KeyValuePair<string, PaletteData> kvp in this.palettes) {
								JSONClass paletteClass = kvp.Value.getJsonPalette ();
								jClass ["palettes"].Add (kvp.Key, paletteClass);
//jsonpalette: {"newPalette":{"name":"newPalette", "colors":[ "69D2E7", "A7DBD8", "E0E4CC", "F38630", "FA6900" ], "alphas":[ "1", "1", "1", "1", "1" ], "percentages":[ "0.2", "0.2", "0.2", "0.2", "0.2" ], "totalWidth":"0"}}
//Debug.Log ("name: " + kvp.Key + " jsonpalette: " + jClass ["palettes"].ToString ());

								//Debug.Log ("name: " + kvp.Key + " jsonpalette: " + jClass ["palettes"].ToString ());
								//Debug.Log ("name: " + kvp.Key + " the whole thing: " + paletteClass.ToString ());
						}
			
						jClass ["paletteURL"] = this.paletteURL;
						jClass ["loadPercent"].AsBool = this.loadPercent;
			
						//Debug.Log (this.name + " the whole collection: " + jClass.ToString ());
						//Debug.Log (this + "getJsonPalette: " + jClass.AsObject.ToString ());
						return jClass;
				}

				public virtual void setPalette (JSONClass jClass)
				{
						name = jClass ["collectionName"];

						this.paletteURL = jClass ["paletteURL"];
						this.loadPercent = jClass ["loadPercent"].AsBool;

						//Debug.Log (" " + jClass ["palettes"].ToString ());

						foreach (string key in jClass ["palettes"].Keys) {
								this.palettes.Add (key, PaletteData.getInstance (jClass ["palettes"] [key].AsObject));
						}

				}

				public bool setSize (int newSize, KeyValuePair<string, PaletteData> kvp = new KeyValuePair<string, PaletteData> ())
				{
						if (newSize != this.palettes.Count) {
				
								if (newSize > this.palettes.Count) {
					
										return CreatePalette (kvp);
								} else {

										IDictionary<string, PaletteData> newPalettes = new Dictionary<string, PaletteData> ();

										int i = 0;
										foreach (KeyValuePair<string, PaletteData> keyvalue in this.palettes) {
												
												if (i < this.palettes.Count - 1) {
														// don't add the last palette!
														newPalettes.Add (keyvalue);
												}
												i++;
										}

										this.palettes = newPalettes;
										return true;
								}
						}
			
						return false;
				}

				/// <summary>
				/// Creates the plaette.
				/// </summary>
				/// <returns><c>true</c>, if plaette was created, <c>false</c> otherwise.</returns>
				/// <param name="kvp">Kvp.</param>
				private bool CreatePalette (KeyValuePair<string, PaletteData> kvp = new KeyValuePair<string, PaletteData> ())
				{
						if (string.IsNullOrEmpty (kvp.Key)) {
								try {
										palettes.Add ("newPalette", new PaletteData ("newPalette"));
								} catch (System.ArgumentException e) {
										Debug.Log (e);
										Debug.Log (" make sure you change the 'newPalette' name first before adding a new palette!");
										return false;
								}
				
								return true;
				
						} else {
								if (palettes.ContainsKey (kvp.Key)) {
										return false;
								} else {
										palettes.Add (kvp);
										return true;
								}
						}
				}



		#endregion

		}
}

