using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public abstract class JSONPersistent : MonoBehaviour
{
		/// <summary>
		/// Is required to save the json
		/// </summary>
		protected string fileName;

		[Serializable]
		public abstract class JSONPersistentData
		{

		};

/* example: 

    [Serializable]
	public class data
	{
		[SerializeField]
		public float
			startRange; 

		[SerializeField]
		public float
			endRange;

		[SerializeField]
		public float
			minimumValue; 

	}
*/


		protected void Awake ()
		{
				fileName = getFileName ();	
		}

		public bool FileExists ()
		{
				return JSONPersistor.Instance.fileExists (getFileName ());
		}

		public virtual string getFileName ()
		{
				return this.gameObject.name + "_" + GetInstanceID ();
		}
	
		public abstract JSONClass getDataClass ();

/* example:
 * protected JSONClass getDataClass ()
	{
		JSONClass jClass = new JSONClass ();

		jClass ["startRange"].AsFloat = myData.startRange;
		jClass ["endRange"].AsFloat = myData.endRange;
		jClass ["minimumValue"].AsFloat = myData.minimumValue;

		return jClass;
	}
*/

		public abstract void setClassData (JSONClass jClass);

/* example:
 * 
 * protected void setClassData (JSONClass jClass)
	{
		myData.startRange = jClass ["startRange"].AsFloat;
		myData.endRange = jClass ["endRange"].AsFloat;
		myData.minimumValue = jClass ["minimumValue"].AsFloat;
	}
*/


/*	protected JSONClass Deserialize (string json)
	{
		JSONNode node = JSONNode.LoadFromBase64 (json);
		return node.AsObject;
	}*/

		public virtual void save ()
		{
				//string jsonString = Serialize (myData);
				JSONPersistor.Instance.saveToFile (fileName, getDataClass ());
				//Debug.Log ("saved " + fileName);
		}

		public virtual void load ()
		{
				JSONClass jClass = JSONPersistor.Instance.loadJSONClassFromFile (fileName);
				setClassData (jClass);
		}
}