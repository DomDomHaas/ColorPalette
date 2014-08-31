using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public abstract class JSONPersistentArray : MonoBehaviour
{
		private string fileName;

		private List<JSONPersistent>
				persitentList = new List<JSONPersistent> ();

		private JSONArray persistentArray = new JSONArray ();

		protected void Awake ()
		{
				fileName = getFileName ();	
		}
	
		public virtual string getFileName ()
		{
				return "array_" + this.gameObject.name + "_" + GetInstanceID ();
		}

		public List<JSONPersistent> getPersistList ()
		{
				return persitentList;
		}

		public JSONArray getJSONArray ()
		{
				return persistentArray;
		}


		/// <summary>
		/// replaces the existing JSONArray
		/// </summary>
		/// <param name="newArray">New array.</param>
		public void setJSONArray (JSONArray newArray)
		{
				persistentArray = newArray;
		}

		/// <summary>
		/// replaces the existing List<JSONPersistent>
		/// </summary>
		/// <param name="newArray">New array.</param>
		public void setPersistentList (List<JSONPersistent> newPersistentList)
		{
				persitentList = newPersistentList;
		}


	
		public static JSONArray convertPersistentListToJSONArray (List<JSONPersistent> list)
		{		
				JSONArray jArray = new JSONArray ();
		
				foreach (JSONPersistent persist in list) {
						jArray.Add (persist.getDataClass ());
				}

				return jArray;
		}

		public static List<JSONPersistent> convertJSONArrayToPersistentList (JSONArray jArray, Type persistentType)
		{
				List<JSONPersistent> list = new List<JSONPersistent> ();

				for (int i = 0; i < jArray.Count; i++) {

						JSONPersistent persist = Activator.CreateInstance (persistentType) as JSONPersistent;
						persist.setClassData (jArray [i].AsObject);
						list.Add (persist);
				}

				return list;
		}

		/// <summary>
		/// Saves the persistent list, every JSONPersistent knows where it has to be saved (in their specific file).
		/// </summary>
		protected void savePersistentList ()
		{
				foreach (JSONPersistent persist in this.persitentList) {
						persist.save ();
				}
		}

		/// <summary>
		/// Saves the persistentArray in one file
		/// </summary>
		protected void saveArrayToFile ()
		{
				if (persistentArray.Count <= 0) {
						Debug.LogException (new Exception (this.gameObject.name + " is trying to save to " + fileName + " it's array but it's empty!"));
				} else {
						JSONPersistor.Instance.saveToFile (fileName, getJSONArray ());
				}
		}

		protected void loadJSONArrayFromFile ()
		{
				JSONArray jArray = JSONPersistor.Instance.loadJSONArrayFromFile (getFileName ());
				setJSONArray (jArray);
		}

		protected void loadPersistentListFromFile (Type persistentType)
		{
				JSONArray jArray = JSONPersistor.Instance.loadJSONArrayFromFile (getFileName ());
				//Debug.Log ("loadPersistentListFromFile loaded (" + jArray.Count + ") : " + jArray.ToString ());
				List<JSONPersistent> list = JSONPersistentArray.convertJSONArrayToPersistentList (jArray, persistentType);
				setPersistentList (list);
		}

}
