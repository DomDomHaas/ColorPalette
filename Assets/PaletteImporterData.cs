using System;
using UnityEngine;
using SimpleJSON;

namespace ColorPalette
{
		[Serializable]
		public class PaletteImporterData : PaletteData
		{
				public PaletteImporterData () : base()
				{

				}

				[SerializeField]
				public string
						paletteURL;
		
				[SerializeField]
				public bool
						loadPercent;


				public override JSONClass getJsonPalette ()
				{
						JSONClass jClass = base.getJsonPalette ();
			
						jClass ["paletteURL"] = this.paletteURL;
						jClass ["loadPercent"].AsBool = this.loadPercent;

						//Debug.Log ("getDataClass: " + jClass ["colors"].Count + " " + jClass ["percentages"].Count);
						return jClass;
				}

				public override void setPalette (JSONClass jClass)
				{
						this.paletteURL = jClass ["paletteURL"];
						this.loadPercent = jClass ["loadPercent"].AsBool;			

						base.setPalette (jClass);
				}


		}
}

