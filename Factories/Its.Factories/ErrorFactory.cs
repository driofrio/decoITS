using System.Collections.Generic;
using Its.StudentModule.DataAccess;
using Its.TutoringModule.ReactiveTutor.ObjectModel;

namespace Its.Factories
{
	/// <author>
	///  Diego Dotor Jara.
	/// </author>
	/// <summary>
	/// This class will create OtherError and WorldError from the ontology.
	/// </summary>
	public class ErrorFactory
	{

		/// <summary>
		/// The ontology.
		/// </summary>
		private static OntologyAccess ONTOLOGY; //= OntologyAccess.Instance;
		/// <summary>
		/// The instance.
		/// </summary>
		private static ErrorFactory _instance = null;
		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static ErrorFactory Instance (string ontologyPath, string logsPath){
			//get {
			if (_instance == null)
				_instance = new ErrorFactory (ontologyPath, logsPath);

			return _instance;
			//}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.Factories.ErrorFactory"/> class.
		/// </summary>
		private ErrorFactory (string ontologyPath, string logsPath)
		{
			ONTOLOGY = OntologyAccess.Instance (ontologyPath, logsPath);
		}

		/// <summary>
		/// Creates the other errors.
		/// </summary>
		/// <returns>The other errors.</returns>
		public List<Error> CreateOtherErrors ()
		{
			//Creates the list which will be returned.
			List<Error> otherErrors;
			//Calls the GetOtherErrors ontology method.
			otherErrors = ONTOLOGY.GetOtherErrors ();

			//Returns the list.
			return otherErrors;
		}

		/// <summary>
		/// Creates the world errors.
		/// </summary>
		/// <returns>The world errors.</returns>
		public List<Error> CreateWorldErrors ()
		{
			//Creates the list which will be returned.
			List<Error> worldErrors;
			//Calls the GetOtherErrors ontology method.
			worldErrors = ONTOLOGY.GetWorldErrors ();

			//Returns the list.
			return worldErrors;
		}
	}
}

