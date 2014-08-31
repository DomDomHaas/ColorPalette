using UnityEngine;
using UnityEditor;

using System.Collections; 
using System.Xml.Serialization; 

[CustomEditor(typeof(JSONPersistent))]
public class JSONPersitentInspector : Editor
{ 

		public float windowHeight = 16;
		public Rect myGUIRect;

		private JSONPersistent myPersist;


		public override void OnInspectorGUI ()
		{    
				base.DrawDefaultInspector ();

				myPersist = target as JSONPersistent;

				myGUIRect = GUILayoutUtility.GetRect (Screen.width, windowHeight);
			
				EditorGUILayout.BeginHorizontal ();

				if (GUILayout.Button ("Load")) { 
						myPersist.load ();
				} 

				if (GUILayout.Button ("Save")) { 
						myPersist.save ();
				} 

				EditorGUILayout.EndHorizontal ();
		
		} 
	

} 


