using System;
using UnityEngine;
using System.Collections.Generic;

namespace ColorPalette
{
		[Serializable]
		public class PaletteCollectionData
		{
				public PaletteCollectionData ()
				{

				}

				[SerializeField]
				public IDictionary<string, PaletteData>
						palettes;

				[SerializeField]
				public string
						paletteURL;
		
				[SerializeField]
				public bool
						loadPercent;

		}
}

