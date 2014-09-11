using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Text; 
using SimpleJSON;

public class JSONPersistor
{

		public static readonly string filePath = "/SaveLoadObjects";
		public static readonly string startArray = "{";
		public static readonly string endArray = "}";
		public static readonly string nextEntry = ",";
		public static readonly string separator = ":";

		private static readonly JSONPersistor instance = new JSONPersistor ();
	
		private JSONPersistor ()
		{
		}
	
		public static JSONPersistor Instance {
				get {
						return instance; 
				}
		}

		public delegate void EventHandler (object listener,EventArgs e);

		public event EventHandler gameSaved;
		public event EventHandler gameLoaded;

		public bool fileExists (string fileName)
		{
				return File.Exists (getFullFilePath (fileName));
		}

		public bool directoryExists (string dirName)
		{
				return Directory.Exists (Application.dataPath + filePath);
		}

		public string getFullFilePath (string fileName)
		{
				return Application.dataPath + filePath + "//" + fileName + ".txt";
		}

		public void saveToFile (string fileName, JSONClass data)
		{
				// save to file already creates directories and the file!
				data.SaveToFile (getFullFilePath (fileName));
		}

		public void saveToFile (string fileName, JSONArray jArray)
		{
				/*		if (!directoryExists (Application.dataPath + filePath)) {
			Directory.CreateDirectory (Application.dataPath + filePath);
		}
		
		if (!fileExists (fileName)) {
			File.CreateText (getFullFilePath (fileName));
		}*/

				// save to file already creates directories and the file!
				jArray.SaveToFile (getFullFilePath (fileName));
		}

		public JSONClass loadJSONClassFromFile (string fileName)
		{
				JSONNode node = JSONNode.LoadFromFile (getFullFilePath (fileName));
				return node.AsObject;
		}

		public JSONArray loadJSONArrayFromFile (string fileName)
		{
				JSONNode node = JSONNode.LoadFromFile (getFullFilePath (fileName));
				return node.AsArray;
		}


	#region Events
	
		private void OnGameSaved (EventArgs e)
		{
				if (this.gameSaved == null) {
						//Debug.Log ("OnStart but no one to notify");
				} else {
						this.gameSaved (this, e);
				}
		}
	
		private void OnGameLoaded (EventArgs e)
		{
				if (this.gameLoaded == null) {
						//Debug.Log ("OnSuccess but no one to notify");
				} else {
						this.gameLoaded (this, e);
				}
		}

	#endregion

	#region conversion

		/// <summary>
		/// Gets the JSON key value pair 
		/// </summary>
		/// <returns>The JSON key value.</returns>
		/// <param name="name">Name.</param>
		/// <param name="aValue">A value.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static string getJSONKeyValue<T> (T aValue, string name = "")
		{
				JSONBinaryTag tag = JSONPersistor.GetJSONBinaryTag<T> (aValue);
				Debug.Log ("tag: " + tag);

				if (aValue.GetType ().IsPrimitive) {
						//val.GetType().ToString()
						if (string.IsNullOrEmpty (name)) {
								name = "someValue";
						}

						return "\"" + tag + "_" + name + "\"" + separator + "\"" + aValue.ToString () + "\"";

/*				} else if (tag == JSONBinaryTag.Array) {

						string arrayStr = name + " [ ";
						JSONArray arr = (JSONArray)aValue;

						if (arr.Count > 0) {
								int i = 0;
								foreach (JSONNode node in arr.Childs) {
										if (i > 0) {
												arrayStr += " , ";
										}
										arrayStr += node.Value;
										i++;
								}

						}
						arrayStr += " ]";

						return arrayStr;*/

				} else if (tag == JSONBinaryTag.Class) {

						JSONClass jClass = aValue as JSONClass;
						return jClass.SaveToBase64 ();
						//} else if (aValue.GetType ().IsClass) {
				}

				return "";
		}


		public static JSONBinaryTag GetJSONBinaryTag<T> (T t)
		{
				Type type = t.GetType ();
				//Debug.Log ("got type: " + type);

				if (type == typeof(Array))
						return JSONBinaryTag.Array;
				if (type == typeof(bool))
						return JSONBinaryTag.BoolValue;
				if (type == typeof(JSONClass))
						return JSONBinaryTag.Class;
				if (type == typeof(double))
						return JSONBinaryTag.DoubleValue;
				if (type == typeof(float))
						return JSONBinaryTag.FloatValue;
				if (type == typeof(int))
						return JSONBinaryTag.IntValue;

				/*
		if (type == typeof())
			return JSONBinaryTag.Value;
*/
				return JSONBinaryTag.Value;
		}
	#endregion

	#region static_color_methdos

		// Note that Color32 and Color implictly convert to each other.
		// You may pass a Color object to this method without first casting it.
		public static string ColorToHex (Color32 color)
		{
				string hex = color.r.ToString ("X2") + color.g.ToString ("X2") + color.b.ToString ("X2");
				return hex;
		}
	
		public static Color HexToColor (string hex)
		{
				if (hex.Length < 6) {
						throw new UnityException ("Hexadecimal Color Value is too short!");
				} else {
						byte r = byte.Parse (hex.Substring (0, 2), System.Globalization.NumberStyles.HexNumber);
						byte g = byte.Parse (hex.Substring (2, 2), System.Globalization.NumberStyles.HexNumber);
						byte b = byte.Parse (hex.Substring (4, 2), System.Globalization.NumberStyles.HexNumber);
						return new Color32 (r, g, b, 255);
				}
		}

		public static string[] getHexArrayFromColors (Color[] colors)
		{
				string[] hexArray = new string[colors.Length];
				for (int i = 0; i < colors.Length; i++) {
						hexArray [i] = JSONPersistor.ColorToHex (colors [i]);
				}
				return hexArray;
		}
	
		public static Color[] getColorsArrayFromHex (string[] hexArray)
		{
				Color[] colors = new Color[hexArray.Length];
				for (int i = 0; i < hexArray.Length; i++) {
						colors [i] = JSONPersistor.HexToColor (hexArray [i]);
				}
				return colors;
		}

	#endregion
}