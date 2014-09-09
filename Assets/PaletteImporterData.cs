using System;
using UnityEngine;

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

		}
}

