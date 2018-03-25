using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Its.Factories;
using Its.StudentModule.DataAccess;
using Its.StudentModule.ObjectModel;
using Its.TutoringModule.ReactiveTutor.ObjectModel;
using Its.WorldModule.ObjectModel;

namespace Its.WorldModule
{
	/// <author>
	/// Diego Dotor Jara.
	/// </author>
	/// <summary>
	/// This class will control all the information related to the world.
	/// </summary>
	public class WorldControl
	{
		/// <summary>
		/// The ONTOLOGY.
		/// </summary>
		private static OntologyAccess ONTOLOGY; //= OntologyAccess.Instance;
		/// <summary>
		/// The world objects.
		/// </summary>
		private Dictionary<string, WorldObject> _worldObjects;
		/// <summary>
		/// The world errors.
		/// </summary>
		private Dictionary<string, Error> _worldErrors;
		/// <summary>
		/// Gets the world errors.
		/// </summary>
		/// <value>The world errors.</value>
		public Dictionary<string, Error> WorldErrors{
			get{
				return _worldErrors;
			}
		}
		/// <summary>
		/// The npc action positions.
		/// </summary>
		private Dictionary<string, NpcActionPosition> _npcActionPositions;
		/// <summary>
		/// Gets the npc action positions.
		/// </summary>
		/// <value>The npc action positions.</value>
		public Dictionary<string, NpcActionPosition> NpcActionPositions {
			get {
				return _npcActionPositions;
			}
		}

		/// <summary>
		/// The instance.
		/// </summary>
		private static WorldControl _instance = null;

		public static WorldControl Instance (string ontologyPath, string logsPath){
			if (_instance == null)
				_instance = new WorldControl (ontologyPath, logsPath);

			return _instance;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.WorldModule.WorldControl"/> class.
		/// </summary>
		private WorldControl (string ontologyPath, string logsPath)
		{
			ONTOLOGY = OntologyAccess.Instance (ontologyPath, logsPath);
			//Initializes the dictionaries.
			_worldErrors = new Dictionary<string, Error> ();
			_worldObjects = new Dictionary<string, WorldObject> ();
			//Brings all world errors from the ontogoly using the factory.
			List<Error> lstWorldError = ErrorFactory.Instance(ontologyPath, logsPath).CreateWorldErrors ();
			//Adds the errors into the dictionary.
			_worldErrors = new Dictionary<string, Error> ();
			foreach (Error e in lstWorldError)
				_worldErrors.Add (e.Key, e);
		}

		public static void DisposeInstance() {
			if (_instance != null) {
				_instance = null;
			}
		}

		/// <summary>
		/// Gets the world error.
		/// </summary>
		/// <returns>The world error.</returns>
		/// <param name="type">Type.</param>
		/// <param name="objectName">Object name.</param>
		/// <exception cref="ArgumentException"> 
		/// Thrown if the number of elements in <paramref name="objectName"/> is not correct.
		/// </exception>
		/// <exception cref="ArgumentException"> 
		/// Thrown if the <paramref name="type"/> does not exist.
		/// </exception>
		public Error GetWorldError (string type, List<string> objectName)
		{
			//Creates a variable which will be returned.
			Error error;
			//Checks if the number of object names is the correct for the type of WorldError.
			switch (type) {
			case "avataroutofrange":
				if (objectName.Count != 1)
					throw new ArgumentException ("The number of object names is not the allowed.");
				break;
			case "faileddrop":
				if (objectName.Count != 1)
					throw new ArgumentException ("The number of object names is not the allowed.");
				break;
			case "objectblocked":
				if (objectName.Count != 1)
					throw new ArgumentException ("The number of object names is not the allowed.");
				break;
			case "objectoutofrange":
				if (objectName.Count != 2)
					throw new ArgumentException ("The number of object names is not the allowed.");
				break;
			default:
				throw new ArgumentException ("The type argument does not exist.");
			}
			//Creates a string key to search in the dictionary.
			string key = type;
			//Concatenates the objects name.
			foreach (string s in objectName)
				key = string.Concat (key, s);
			//Search in the dictionary the WorldError.
			if (!_worldErrors.TryGetValue(key, out error))
				error = this.CreateWorldError(type, objectName);
			//Returns the error.
			return error;
		}

		/// <summary>
		/// Creates the world error.
		/// </summary>
		/// <returns>The world error.</returns>
		/// <param name="type">Type.</param>
		/// <param name="objectName">Object name.</param>
		private Error CreateWorldError (string type, List<string> objectName)
		{
			//Creates a new variable.
			Error error;
			//Get the generic message for the specific type.
			string errorMsg = ONTOLOGY.GetGenericErrorMessage (type);
			//Creates the key.
			string errorKey = type;
			//Concatenates the objects name.
			foreach (string s in objectName) {
				errorKey = string.Concat (errorKey, s);
				//Replaces the objects names.
				errorMsg = String.Format (errorMsg, s);
			}
			//Creates a ErrorMessage instance.
			ErrorMessage errorMessage = new ErrorMessage (errorKey, errorMsg);
			//Creates the Error instance.
			error = new Error (errorKey, errorMessage, false);
			//Returns the error.
			return error;
		}

		/// <summary>
		/// Validate if the object is blocked.
		/// </summary>
		/// <returns><c>true</c>, if the object is blocked, <c>false</c> otherwise.</returns>
		/// <param name="objectName">Object name.</param>
		/// <param name="student">Student.</param>
		public bool ObjectBlockValidate (string objectName, Student student)
		{
			//Create the boolean to return.
			bool result;
			//Creates an auxiliar WorldObject.
			WorldObject worldOjb = null;
			//Searchs the WorldObject with the given name and the given student.
			var queryWorldObj =
				from wObj in _worldObjects
				where wObj.Key == objectName
				select wObj.Value;
			if (queryWorldObj.Count() > 0)
				//Selects the worldObjects.
				worldOjb = queryWorldObj.First ();

			//Checks if the object exists
			if (worldOjb != null) {
				//Checks if the owner is the student given.
				if (worldOjb.Owner == student || worldOjb.Owner == null)
					result = false;
				else
					result = true;
			} else {
				result = false;
			}

			//Returns the isBLock value.
			return result;
		}

		/// <summary>
		/// Blocks the object.
		/// </summary>
		/// <param name="objectName">Object name.</param>
		/// <param name="student">Student.</param>
		public void BlockObject (string objectName, Student student)
		{
			//Creates an auxiliar WorldObject.
			WorldObject worldOjb;
			//Searchs the WorldObject with the given name.
			var queryWorldObj =
				from wObj in _worldObjects
					where wObj.Key == objectName
				select wObj.Value;
			//Selects the worldObjects.
			worldOjb = queryWorldObj.First ();

			//Sets the isBLock value.
			worldOjb.IsBlock = true;
			//Sets the owner.
			worldOjb.Owner = student;
		}

		/// <summary>
		/// Gets the free object.
		/// </summary>
		/// <returns>The free object.</returns>
		/// <param name="plan">Plan.</param>
		/// <param name="action">Action.</param>
		/// <param name="regularExp">Regular exp.</param>
		public List<WorldObject> GetFreeObject (string plan, string action, string regularExp)
		{
			//Creates the list which will be returned.
			List<WorldObject> lstWorldObj = new List<WorldObject> ();
			//Searchs the element with the attributes equals to the given params.
			var queryActPos = 
				from actPos in _npcActionPositions
				where actPos.Value.Action == action && actPos.Value.Plan == plan
				    && (Regex.Match (actPos.Value.WorldObj.Name, regularExp)).Success
				select actPos.Value.WorldObj;
			//Adds the worldObjects into the list.
			foreach (WorldObject o in queryActPos)
				lstWorldObj.Add (o);
			//Returns the list.
			return lstWorldObj;
		}

		/// <summary>
		/// Creates the world data.
		/// </summary>
		/// <param name="domainKey">Domain key.</param>
		public void CreateWorldData (string domainKey, string worldConfPath)
		{
			//Creates the WorldObjects.
			WorldObjectFactory worldFactory=WorldObjectFactory.Instance(worldConfPath);
			_worldObjects = worldFactory.CreateWorldObjs (domainKey);
			//Creates the NpcActionPosition
			//_npcActionPositions = worldFactory.CreateNpcActionPositions (domainKey, _worldObjects);
		}
	}
}

