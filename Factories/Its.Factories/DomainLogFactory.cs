using System;
using System.Linq;
using System.Collections.Generic;
using Its.StudentModule.ObjectModel;
using Its.StudentModule.DataAccess;
using Its.ExpertModule.ObjectModel;
using Its.TutoringModule.ReactiveTutor.ObjectModel;

namespace Its.Factories
{

	/// <author>
	/// Diego Dotor Jara.
	/// </author>
	/// <summary>
	/// This class will be the factory that creates logs objects. This class is a singleton class.
	/// </summary>
	public class DomainLogFactory
	{
		/// <summary>
		/// The ONTOLOG.
		/// </summary>
		private static OntologyAccess ONTOLOGY; //= OntologyAccess.Instance;
		/// <summary>
		/// LogFactory singleton instance.
		/// </summary>
		private static DomainLogFactory _instance = null;
		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static DomainLogFactory Instance(string ontologyPath, string logsPath) {
			//get {
			if (_instance == null)
				_instance = new DomainLogFactory (ontologyPath, logsPath);

			return _instance;
			//}
		}


		/// <summary>
		/// Constructor method.
		/// </summary>
		/// <remarks>
		/// Description: Creates an instance of LogFactory object.
		/// </remarks>
		private DomainLogFactory (string ontologyPath, string logsPath)
		{
			ONTOLOGY = OntologyAccess.Instance (ontologyPath, logsPath);
		}

		/// <summary>
		/// Creates the logs from ontology.
		/// </summary>
		/// <returns>The logs from ontology.</returns>
		/// <param name="domain">Domain.</param>
		/// <param name="students">Students.</param>
		/// <param name="otherErrors">Other errors.</param>
		/// <param name="worldErrors">World errors.</param>
		public DomainLog CreateLogsFromOntology (DomainActions domain, Dictionary<string, Student> students, 
			Dictionary<string, Error> otherErrors, Dictionary<string, Error> worldErrors)
		{
			//Creates the DomainLog which will be returned.
			DomainLog domainLog = new DomainLog (domain);
			//Creates an auxiliar list.
			List<object[]> lst = ONTOLOGY.GetLogInformation (domain);
			//Creates the logs.
			foreach (object[] o in lst) {
				//Creates a Student.
				Student student;
				//Creates an ActionAplication.
				ActionAplication action;
				//Creates an auxiliar StudentLog.
				StudentLog studentLog;
				//Gets the StudenLog with the given key.
				try {
					studentLog = domainLog.GetStudentLog((string)o [0]);
				}
				//If the StudentLog does not exist, it will be create.
				catch (ArgumentException e) {
					//Searchs and gets the student with the key.
					students.TryGetValue ((string)o [0], out student);
					//Creates the StudentLog with the previous Student.
					domainLog.CreateStudentLog (student);
					//Gets the StudentLog created.
					studentLog = domainLog.GetStudentLog((string)o [0]);
				}
				//Searchs the action with the key.
				var qAction = 
					from act in domain.Actions
					where (string)o [2] == act.Key
					select act;
				//Gets the value if it is not an action which is not in the domain.
				try {
					action = qAction.First ();
				}
				//If the action does not exist in the domain, a new action is created.
				catch (InvalidOperationException e) {
					action = new ActionAplication((string)o [2], (string)o [2]);
				}
				//Checks the type of the log.
				switch ((string)o[5]) {
				case "corrective":
					//Creates the log.
					CorrectiveActionLog correctiveActionLog = new CorrectiveActionLog (action, (bool)o [3],
						                                          (DateTime)o [4], (bool)o [6]);
					//Adds into the StudentLog.
					studentLog.AddLog (correctiveActionLog);
					break;
				case "noCorrective":
					//Creates the log.
					NoCorrectiveActionLog noCorrectiveActionLog = new NoCorrectiveActionLog (action, (bool)o [3],
						                                              (DateTime)o [4]);
					//Adds into the StudentLog.
					studentLog.AddLog (noCorrectiveActionLog);
					break;
				case "noplan":
					//Creates the log.
					NoPlanAllowedActionLog noPlanActionLog = new NoPlanAllowedActionLog (action, (bool)o [3],
						(DateTime)o [4]);
					//Adds into the StudentLog.
					studentLog.AddLog (noPlanActionLog);
					break;
				case "dependence":
					//Creates a Dependence.
					Dependence dependence;
					//Gets the dependence with the key.
					if (action.Dependence.Key == (string)o [7])
						dependence = action.Dependence;
					else
						dependence = action.Dependence.GetDependence ((string)o [7]);
					//Creates the log.
					DepErrorLog depErrorLog = new DepErrorLog (action, (bool)o [3], (DateTime)o [4],
						dependence, bool.Parse((string)o [8]));
					//Adds into the StudentLog.
					studentLog.AddLog (depErrorLog);
					break;
				case "incompatibility":
					//Creates an Incompatibility.
					Incompatibility incompatibility;
					//Searchs the incompatibility with the key.
					var qIncomp =
						from inc in action.Incompatibilities
						where (string)o [7] == inc.Key
						select inc;
					//Gets the value.
					incompatibility = qIncomp.First ();
					//Creates the log.
					IncompErrorLog incompErrorLog = new IncompErrorLog (action, (bool)o [3],
						                                (DateTime)o [4], incompatibility);
					//Adds into the StudentLog.
					studentLog.AddLog (incompErrorLog);
					break;
				case "minTime":
					//Creates the log.
					MinTimeErrorLog minTimeErrorLog = new MinTimeErrorLog (action, (bool)o [3],
						                                  (DateTime)o [4], action.MinTime);
					//Adds into the StudentLog.
					studentLog.AddLog (minTimeErrorLog);
					break;
				case "maxTime":
					//Creates the log.
					MaxTimeErrorLog maxTimeErrorLog = new MaxTimeErrorLog (action, (bool)o [3],
						(DateTime)o [4], action.MaxTime);
					//Adds into the StudentLog.
					studentLog.AddLog (maxTimeErrorLog);
					break;
				case "world":
					//Creates an Error.
					Error worldError;
					//Searchs the error with the key.
					var qWorld = 
						from error in worldErrors
						where (string)o [7] == error.Key
						select error.Value;
					//Gets the value.
					worldError = qWorld.First ();
					//Creates the log.
					WorldErrorLog worldErrorLog = new WorldErrorLog (action, (bool)o [3],
						                              (DateTime)o [4], worldError, (string)o [8]);
					//Adds into the StudentLog.
					studentLog.AddLog (worldErrorLog);
					break;
				case "other":
					//Creates an Error.
					Error otherError;
					//Searchs the error with the key.
					var qOther = 
						from error in otherErrors
						where (string)o [7] == error.Key
						select error.Value;
					//Gets the value.
					otherError = qOther.First ();
					//Creates the log.
					OtherErrorLog otherErrorLog = new OtherErrorLog (action, (bool)o [3],
						                              (DateTime)o [4], otherError);
					//Adds into the StudentLog.
					studentLog.AddLog (otherErrorLog);
					break;
				}
			}

			//Updates the current Phase value for each student.
			List<Student> tmpStudents= domainLog.GetStudents();
			foreach (Student std in tmpStudents) {
				//Gets the student.
				StudentLog log = domainLog.GetStudentLog (std.Key);
				//Sets the currentPhase.
				log.CurrentPhase = log.Logs.Last().Action.Phase;
			}

			//Returns the DomainLog.
			return domainLog;
		}
	}
}