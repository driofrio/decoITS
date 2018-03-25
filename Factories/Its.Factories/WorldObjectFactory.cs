using System.Collections.Generic;
using Its.Factories.Common;
using Its.WorldModule.DataAccess;
using Its.WorldModule.ObjectModel;

namespace Its.Factories
{

	/// <author>
	///  Diego Dotor Jara.
	/// </author>
	/// <summary>
	/// This class will create WorldObject from a text file.
	/// </summary>
	public class WorldObjectFactory
	{
		/// <summary>
		/// The WORL d ACCES.
		/// </summary>
		private static WorldAccess WORLD_ACCESS; //= WorldAccess.Instance;
		/// <summary>
		/// The instance.
		/// </summary>
		private static WorldObjectFactory _instance = null;
		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static WorldObjectFactory Instance (string worldConfPath){
			//get {
			if (_instance == null)
				_instance = new WorldObjectFactory (worldConfPath);

			return _instance;
			//}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.Factories.WorldObjectFactory"/> class.
		/// </summary>
		private WorldObjectFactory (string worldConfPath)
		{
			WORLD_ACCESS = WorldAccess.Instance (worldConfPath);
		}

		/// <summary>
		/// Creates the world objects.
		/// </summary>
		/// <returns>The world objects.</returns>
		/// <param name="domainKey">Domain key.</param>
		public Dictionary<string, WorldObject> CreateWorldObjs (string domainKey)
		{
			//Creates the dictionary which will be returned.
			Dictionary<string, WorldObject> worldObjects = new Dictionary<string, WorldObject> ();
			//Gets the world objects information from the file.
			List<string> objs = WORLD_ACCESS.GetWorldObjects (domainKey);
			//Creates the WorldObjects.
			foreach (string s in objs) {
				worldObjects.Add(s, new WorldObject(s));
			}

			//Returns the dictionary.
			return worldObjects;
		}

		/// <summary>
		/// Creates the npc action positions.
		/// </summary>
		/// <returns>The npc action positions.</returns>
		/// <param name="domainKey">Domain key.</param>
		public Dictionary<string, NpcActionPosition> CreateNpcActionPositions (string domainKey,
			Dictionary<string, WorldObject> worldObjects)
		{
			//Creates the dictionary which will be returned.
			Dictionary<string, NpcActionPosition> npcActionPositions = new Dictionary<string, NpcActionPosition> ();
			//Gets the npc information from the file.
			List<object[]> npcInfo = WORLD_ACCESS.GetNpcActionPosition (domainKey);
			//Creates the NpcActionPositions.
			foreach (object[] o in npcInfo) {
				//Creates an auxiliar variable.
				WorldObject obj;
				//Obtains the WorldObject from the dictionary.
				if (!worldObjects.TryGetValue (o [1].ToString(), out obj)) {
					//Throws an exception.
					throw new WorldObjectNotFoundException ();
				} else {
					//Adds into the dictionary.
					npcActionPositions.Add(obj.Name + (string)o[2] + (string)o[0],
						new NpcActionPosition(obj, (string)o[2], (string)o[0], (float[])o[3], (float[])o[4]));
				}
			}

			//Returns the dictionary.
			return npcActionPositions;
		}
	}
}

