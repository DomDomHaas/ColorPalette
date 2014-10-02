using UnityEngine;
using UnityEditor;

using System.Collections; 
using System.Xml.Serialization; 

[CustomEditor(typeof(JSONPersistentArray))]
public class JSONPersistentArrayInspector : Editor
{ 

		public float windowHeight = 16;
		public Rect myGUIRect;

		private JSONPersistentArray myArray;


		public override void OnInspectorGUI ()
		{    
				base.DrawDefaultInspector ();

				myArray = target as JSONPersistentArray;

				myGUIRect = GUILayoutUtility.GetRect (Screen.width, windowHeight);
			
				EditorGUILayout.BeginHorizontal ();

				if (GUILayout.Button ("Load")) { 
						//myArray.load ();
				} 

				if (GUILayout.Button ("Save")) { 
						//myArray.save ();
				} 

				EditorGUILayout.EndHorizontal ();
		
		} 
	

} 


