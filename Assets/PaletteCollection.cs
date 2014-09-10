using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace ColorPalette
{

		public class PaletteCollection : JSONPersistent
		{

				public PaletteCollectionData palettes;

				// Use this for initialization
				void Awake ()
				{
						init ();
				}

				public void init ()
				{
						palettes = new Dictionary<string, PaletteData> ();

						fileName = getFileName ();

						if (JSONPersistor.Instance.fileExists (fileName)) {
								load ();
						}
				}

	
				// Update is called once per frame
				void Update ()
				{
	
				}



				public override string getFileName ()
				{
						return this.gameObject.name + "_palettes";
				}

				public override SimpleJSON.JSONClass getDataClass ()
				{
						throw new System.NotImplementedException ();
				}

				public override void setClassData (SimpleJSON.JSONClass jClass)
				{
						throw new System.NotImplementedException ();
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