using System.Collections.Generic;
using Its.StudentModule.DataAccess;
using Its.StudentModule.ObjectModel;

namespace Its.Factories
{
	/// <author>
	///  Diego Dotor Jara.
	/// </author>
	/// <summary>
	/// This class will create Students from the ontology.
	/// </summary>
	public class StudentFactory
	{
		/// <summary>
		/// The ontology.
		/// </summary>
		private OntologyAccess ONTOLOGY;
		/// <summary>
		/// The instance.
		/// </summary>
		private static StudentFactory _instance = null;
		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static StudentFactory Instance (string ontologyPath, string logsPath){
			//get {
			if (_instance == null)
				_instance = new StudentFactory (ontologyPath, logsPath);

			return _instance;
			//}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.Factories.StudentFactory"/> class.
		/// </summary>
		private StudentFactory (string ontologyPath, string logsPath)
		{
			ONTOLOGY = OntologyAccess.Instance (ontologyPath, logsPath);
		}

		/// <summary>
		/// Creates the students.
		/// </summary>
		/// <returns>The students.</returns>
		public Dictionary<string, Student> CreateStudents ()
		{
			//Creates an auxiliar variable.
			List<Student> std;
			//Creates the student list which will be returned.
			Dictionary<string, Student> students =  new Dictionary<string, Student>();
			//Calls the GetStudents ontology method.
			std = ONTOLOGY.GetStudents ();

			foreach (Student st in std)
				students.Add (st.Key, st);

			//Returns the list.
			return students;
		}
		
		public static void DisposeInstance() {
			if (_instance != null) {
				_instance = null;
			}
		}
	}
}

