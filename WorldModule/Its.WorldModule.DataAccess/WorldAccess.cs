using System;
using System.Collections.Generic;
using System.IO;

namespace Its.WorldModule.DataAccess
{
	public class WorldAccess
	{
		/// <summary>
		/// The instance.
		/// </summary>
		private static WorldAccess _instance = null;
		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static WorldAccess Instance(string confPath) {
			//get {
				if (_instance == null)
				_instance = new WorldAccess (confPath);

				return _instance;
		//	}
		}
		/// <summary>
		/// The conf path.
		/// </summary>
		private string _confPath;

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.WorldModule.DataAccess.WorldAccess"/> class.
		/// </summary>
		private WorldAccess (string confPath)
		{
			//Obtains the configuration path from AppSettings.
			_confPath = confPath;//ConfigurationManager.AppSettings ["worldConfigurationPath"].ToString();
			//_confPath = _confPath.Replace ('\\', Path.DirectorySeparatorChar);
		}
			
		/// <summary>
		/// Gets the world objects.
		/// </summary>
		/// <returns>The world objects.</returns>
		/// <param name="domainKey">Domain key.</param>
		/// <exception cref="ArgumentException"> 
		/// Thrown if the file <paramref name="domainKey"/> does not exist.
		/// </exception>
		public List<string> GetWorldObjects (string domainKey)
		{
			//Creates the list which will be returned.
			List<string> worldObjInfo = new List<string> ();
			//Creates the file path.
			string filePath = _confPath + "BlockObjects" + Path.DirectorySeparatorChar + domainKey + ".txt";
			//Checks if the file exists.
			if (!File.Exists (filePath)) {
				ArgumentException argEx = new ArgumentException ("There is not a file with the given name.", "domainKey");
				throw argEx;
			}
			//Reads all the lines of the file.
			string[] lines = File.ReadAllLines (filePath);
			//Adds the object names into the list.
			foreach (string s in lines)
				worldObjInfo.Add (s);

			//Returns the list with the world objects information.
			return worldObjInfo;
		}
			
		/// <summary>
		/// Gets the npc action position.
		/// </summary>
		/// <returns>The npc action position.</returns>
		/// <param name="domainKey">Domain key.</param>
		/// <exception cref="ArgumentException"> 
		/// Thrown if the file <paramref name="domainKey"/> does not exist.
		/// </exception>
		public List<object[]> GetNpcActionPosition (string domainKey)
		{
			//Creates the list which will be returned.
			List<object[]> npcInfo = new List<object[]> ();
			//Creates the file path.
			string filePath = _confPath + "NpcManagerMapping" + Path.DirectorySeparatorChar + domainKey + ".txt";
			//Checks if the file exists.
			if (!File.Exists (filePath)) {
				ArgumentException argEx = new ArgumentException ("There is not a file with the given name.", "domainKey");
				throw argEx;
			}
			//Reads all the lines of the file.
			string[] lines = File.ReadAllLines (filePath);

			//Read line per line and creates the objects.
			foreach (string s in lines) {
				//Creates an auxiliar string array.
				string[] ss = s.Split ('|');
				//Obtains the position.
				string sPos = ss [3];
				//Obtains the rotation.
				string sRot = ss [4];
				//Creates the float arrays with the position values.
				sPos = sPos.Replace ("<", "");
				sPos = sPos.Replace (">", "");
				string[] ssPos = sPos.Split (',');
				float[] pos = {float.Parse(ssPos[0]), float.Parse(ssPos[1]), float.Parse(ssPos[2])};
				sRot = sRot.Replace ("<", "");
				sRot = sRot.Replace (">", "");
				string[] ssRot = sRot.Split (',');
				float[] rot = {float.Parse(ssRot[0]), float.Parse(ssRot[1]), float.Parse(ssRot[2])};
				//Creates the object array.
				object[] o = { ss [0], ss [1], ss [2], pos, rot };
				//Adds into the list.
				npcInfo.Add (o);
			}

			//Returns the list.
			return npcInfo;
		}
	}
}

