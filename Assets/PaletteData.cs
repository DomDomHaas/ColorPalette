using System;
using UnityEngine;

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

		}
}

