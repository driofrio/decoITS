using System;
using System.Collections.Generic;
using System.IO;
using Its.ExpertModule.ObjectModel;
using Its.Factories;
using Its.StudentModule.DataAccess;
using Its.StudentModule.ObjectModel;
using Its.TutoringModule.ReactiveTutor.ObjectModel;

namespace Its.StudentModule
{
	/// <author>
	///  Diego Dotor Jara.
	/// </author>
	/// <summary>
	/// This class will control all the students information as well as the logs information.
	/// </summary>
	public class StudentControl
	{
		/// <summary>
		/// The ontology.
		/// </summary>
		private static OntologyAccess ONTOLOGY; //= OntologyAccess.Instance;
		/// <summary>
		/// The instance.
		/// </summary>
		private static StudentControl _instance = null;
		/// <summary>
		/// The instance domain log factory.
		/// </summary>
		private static DomainLogFactory _instanceDomainLogFactory;
		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static StudentControl Instance (string ontologyPath, string logsPath, string domainPath){
			//get {
				if (_instance == null)
					_instance = new StudentControl (ontologyPath, logsPath, domainPath);

				return _instance;
			//}
		}
		/// <summary>
		/// The students.
		/// </summary>
		private Dictionary<string, Student> _students;
		/// <summary>
		/// The domain logs.
		/// </summary>
		private Dictionary<string, DomainLog> _domainLogs;
		private string _domainPath;
		/// <summary>
		/// Initializes a new instance of the <see cref="Its.StudentModule.StudentControl"/> class.
		/// </summary>
		private StudentControl (string ontologyPath, string logsPath, string domainPath)
		{
			_instanceDomainLogFactory = DomainLogFactory.Instance (ontologyPath, logsPath);

			ONTOLOGY = OntologyAccess.Instance (ontologyPath, logsPath);
			//Initializes the domainLogs dictionary.
			this._domainLogs = new Dictionary<string, DomainLog> ();
			//Creates all students from the ontology.
			_students = StudentFactory.Instance(ontologyPath, logsPath).CreateStudents ();
			_domainPath = domainPath;
		}

		public static void DisposeInstance() {
			if (_instance != null) {
				_instance = null;
			}
		}

		public static LogEntry CreateCorrectiveActionLog(ActionAplication action, bool wasApplied, bool errorsFixed)
		{
			return new CorrectiveActionLog (action, wasApplied, errorsFixed);
		}

		public static LogEntry CreateNoCorrectiveActionLog(ActionAplication action, bool wasApplied)
		{
			return new NoCorrectiveActionLog(action, wasApplied);
		}

		public static LogEntry CreateMinTimeErrorLog(ActionAplication action, bool wasApplied, int time)
		{
			return new MinTimeErrorLog(action, wasApplied, time);
		}

		public static LogEntry CreateMaxTimeErrorLog(ActionAplication action, bool wasApplied, int time)
		{
			return new MaxTimeErrorLog(action, wasApplied, time);
		}

		public static LogEntry CreateOtherErrorLog(ActionAplication action, bool wasApplied, Error error)
		{
			return new OtherErrorLog(action, wasApplied, error);
		}

		public static LogEntry CreateWorldErrorLog(ActionAplication action, bool wasApplied, Error error, string type)
		{
			return new WorldErrorLog(action, wasApplied, error, type);
		}

		public static LogEntry CreateDepErrorLog(ActionAplication action, bool wasApplied, Dependence failedDependence)
		{
			bool isOrderError = failedDependence.GetType() == typeof(SeqComplexDependence);

			return new DepErrorLog(action, wasApplied, failedDependence, isOrderError);
		}

		public static LogEntry CreateIncompErrorLog(ActionAplication action, bool wasApplied, Incompatibility failedIncomp)
		{
			return new IncompErrorLog(action, wasApplied, failedIncomp);
		}

/*		/// <summary>
		/// Creates the corrective action log.
		/// </summary>
		/// <param name="action">Action.</param>
		/// <param name="domain">Domain.</param>
		/// <param name="studentKey">Student key.</param>
		/// <param name="errorsFixed">If set to <c>true</c> errors fixed.</param>
		/// <exception cref="ArgumentException"> 
		/// Thrown if the <paramref name="domain"/> does not exist.
		/// </exception>
		public void CreateCorrectiveActionLog (ActionAplication action, DomainActions domain, Student student,
			bool wasApplied, bool errorsFixed)
		{
			//Gets the domainLog.
			DomainLog domainLog;
			if (!_domainLogs.TryGetValue (domain.Key, out domainLog))
				throw new ArgumentException ("There is not any DomainLog with the given domain.");
			//Gets the StudentLog.
			StudentLog studentLog = domainLog.GetStudentLog (student.Key);
			//Creates the new CorrectiveActionLog.
			CorrectiveActionLog log = new CorrectiveActionLog (action, wasApplied, errorsFixed);
			//Adds the log into the StudentLog.
			studentLog.AddLog (log);
			studentLog.AddToLogBuffer(log);

			//Creates a path.
			string path = _domainPath.Replace ('\\', Path.DirectorySeparatorChar) + "Logs" + Path.DirectorySeparatorChar
				+ domain.Key + Path.DirectorySeparatorChar;
			//Determine whether the directory exists. If the directory does not exist, it will be created.
			if (!Directory.Exists(path)) {
				Directory.CreateDirectory(path);
			}
			//Copies the log into a txt file.
			using (StreamWriter sw = File.AppendText (path + student.Key + ".txt")) {
				sw.WriteLine (log.TxtLogString());
			}
		}

		/// <summary>
		/// Creates the no corrective action log.
		/// </summary>
		/// <param name="action">Action.</param>
		/// <param name="domain">Domain.</param>
		/// <param name="studentKey">Student key.</param>
		/// <param name="wasApplied">If set to <c>true</c> was applied.</param>
		/// <exception cref="ArgumentException"> 
		/// Thrown if the <paramref name="domain"/> does not exist.
		/// </exception>
		public void CreateNoCorrectiveActionLog (ActionAplication action, DomainActions domain, Student student,
			bool wasApplied)
		{
			//Gets the domainLog.
			DomainLog domainLog;
			if (!_domainLogs.TryGetValue (domain.Key, out domainLog))
				throw new ArgumentException ("There is not any DomainLog with the given domain.");
			//Gets the StudentLog.
			StudentLog studentLog = domainLog.GetStudentLog (student.Key);
			//Creates the new NoCorrectiveActionLog.
			NoCorrectiveActionLog log = new NoCorrectiveActionLog (action, wasApplied);
			//Adds the log into the StudentLog.
			studentLog.AddLog (log);
			studentLog.AddToLogBuffer(log);

			//Creates a path.
			string path = _domainPath.Replace ('\\', Path.DirectorySeparatorChar) + "Logs" + Path.DirectorySeparatorChar
				+ domain.Key + Path.DirectorySeparatorChar;
			//Determine whether the directory exists. If the directory does not exist, it will be created.
			if (!Directory.Exists(path)) {
				Directory.CreateDirectory(path);
			}
			//Copies the log into a txt file.
			using (StreamWriter sw = File.AppendText (path + student.Key + ".txt")) {
				sw.WriteLine (log.TxtLogString());
			}
		}

		/// <summary>
		/// Creates the minimum time error log.
		/// </summary>
		/// <param name="action">Action.</param>
		/// <param name="domain">Domain.</param>
		/// <param name="studentKey">Student key.</param>
		/// <param name="wasApplied">If set to <c>true</c> was applied.</param>
		/// <param name="time">Time.</param>
		/// <exception cref="ArgumentException"> 
		/// Thrown if the <paramref name="domain"/> does not exist.
		/// </exception>
		public void CreateMinTimeErrorLog (ActionAplication action, DomainActions domain, Student student,
			bool wasApplied, int time)
		{
			//Gets the domainLog.
			DomainLog domainLog;
			if (!_domainLogs.TryGetValue (domain.Key, out domainLog))
				throw new ArgumentException ("There is not any DomainLog with the given domain.");
			//Gets the StudentLog.
			StudentLog studentLog = domainLog.GetStudentLog (student.Key);
			//Creates the new MinTimeErrorLog.
			MinTimeErrorLog log = new MinTimeErrorLog (action, wasApplied, time);
			//Adds the log into the StudentLog.
			studentLog.AddLog (log);
			studentLog.AddToLogBuffer(log);


			//Creates a path.
			string path = _domainPath.Replace ('\\', Path.DirectorySeparatorChar) + "Logs" + Path.DirectorySeparatorChar
				+ domain.Key + Path.DirectorySeparatorChar;
			//Determine whether the directory exists. If the directory does not exist, it will be created.
			if (!Directory.Exists(path)) {
				Directory.CreateDirectory(path);
			}
			//Copies the log into a txt file.
			using (StreamWriter sw = File.AppendText (path + student.Key + ".txt")) {
				sw.WriteLine (log.TxtLogString());
			}
		}

		/// <summary>
		/// Creates the max time error log.
		/// </summary>
		/// <param name="action">Action.</param>
		/// <param name="domain">Domain.</param>
		/// <param name="studentKey">Student key.</param>
		/// <param name="wasApplied">If set to <c>true</c> was applied.</param>
		/// <param name="time">Time.</param>
		/// <exception cref="ArgumentException"> 
		/// Thrown if the <paramref name="domain"/> does not exist.
		/// </exception>
		public void CreateMaxTimeErrorLog (ActionAplication action, DomainActions domain, Student student,
			bool wasApplied, int time)
		{
			//Gets the domainLog.
			DomainLog domainLog;
			if (!_domainLogs.TryGetValue (domain.Key, out domainLog))
				throw new ArgumentException ("There is not any DomainLog with the given domain.");
			//Gets the StudentLog.
			StudentLog studentLog = domainLog.GetStudentLog (student.Key);
			//Creates the new MaxTimeErrorLog.
			MaxTimeErrorLog log = new MaxTimeErrorLog (action, wasApplied, time);
			//Adds the log into the StudentLog.
			studentLog.AddLog (log);
			studentLog.AddToLogBuffer(log);

			//Creates a path.
			string path = _domainPath.Replace ('\\', Path.DirectorySeparatorChar) + "Logs" + Path.DirectorySeparatorChar
				+ domain.Key + Path.DirectorySeparatorChar;
			//Determine whether the directory exists. If the directory does not exist, it will be created.
			if (!Directory.Exists(path)) {
				Directory.CreateDirectory(path);
			}
			//Copies the log into a txt file.
			using (StreamWriter sw = File.AppendText (path + student.Key + ".txt")) {
				sw.WriteLine (log.TxtLogString());
			}
		}

		/// <summary>
		/// Creates the other error log.
		/// </summary>
		/// <param name="action">Action.</param>
		/// <param name="domain">Domain.</param>
		/// <param name="studentKey">Student key.</param>
		/// <param name="wasApplied">If set to <c>true</c> was applied.</param>
		/// <exception cref="ArgumentException"> 
		/// Thrown if the <paramref name="domain"/> does not exist.
		/// </exception>
		public void CreateOtherErrorLog (ActionAplication action, DomainActions domain, Student student,
			bool wasApplied, Error error)
		{
			//Gets the domainLog.
			DomainLog domainLog;
			if (!_domainLogs.TryGetValue (domain.Key, out domainLog))
				throw new ArgumentException ("There is not any DomainLog with the given domain.");
			//Gets the StudentLog.
			StudentLog studentLog = domainLog.GetStudentLog (student.Key);
			//Creates the new OtherErrorLog.
			OtherErrorLog log = new OtherErrorLog (action, wasApplied, error);
			//Adds the log into the StudentLog.
			studentLog.AddLog (log);
			studentLog.AddToLogBuffer(log);

			//Creates a path.
			string path = _domainPath.Replace ('\\', Path.DirectorySeparatorChar) + "Logs" + Path.DirectorySeparatorChar
			              + domain.Key + Path.DirectorySeparatorChar;
			//Determine whether the directory exists. If the directory does not exist, it will be created.
			if (!Directory.Exists(path)) {
				Directory.CreateDirectory(path);
			}
			//Copies the log into a txt file.
			using (StreamWriter sw = File.AppendText (path + student.Key + ".txt")) {
				sw.WriteLine (log.TxtLogString());
			}
		}

		/// <summary>
		/// Creates the world error log.
		/// </summary>
		/// <param name="action">Action.</param>
		/// <param name="domain">Domain.</param>
		/// <param name="studentKey">Student key.</param>
		/// <param name="wasApplied">If set to <c>true</c> was applied.</param>
		/// <param name="error">Error.</param>
		/// <exception cref="ArgumentException"> 
		/// Thrown if the <paramref name="domain"/> does not exist.
		/// </exception>
		public void CreateWorldErrorLog (ActionAplication action, DomainActions domain, Student student,
			bool wasApplied, Error error, string type)
		{
			//Gets the domainLog.
			DomainLog domainLog;
			if (!_domainLogs.TryGetValue (domain.Key, out domainLog))
				throw new ArgumentException ("There is not any DomainLog with the given domain.");
			//Gets the StudentLog.
			StudentLog studentLog = domainLog.GetStudentLog (student.Key);
			//Creates the new WorldErrorLog.
			WorldErrorLog log = new WorldErrorLog (action, wasApplied, error, type);
			//Adds the log into the StudentLog.
			studentLog.AddLog (log);
			studentLog.AddToLogBuffer(log);

			//Creates a path.
			string path = _domainPath.Replace ('\\', Path.DirectorySeparatorChar) + "Logs" + Path.DirectorySeparatorChar
				+ domain.Key + Path.DirectorySeparatorChar;
			//Determine whether the directory exists. If the directory does not exist, it will be created.
			if (!Directory.Exists(path)) {
				Directory.CreateDirectory(path);
			}
			//Copies the log into a txt file.
			using (StreamWriter sw = File.AppendText (path + student.Key + ".txt")) {
				sw.WriteLine (log.TxtLogString());
			}
		}

		/// <summary>
		/// Creates the dep error log.
		/// </summary>
		/// <param name="action">Action.</param>
		/// <param name="domain">Domain.</param>
		/// <param name="studentKey">Student key.</param>
		/// <param name="wasApplied">If set to <c>true</c> was applied.</param>
		/// <param name="failedDependence">Failed dependence.</param>
		/// <exception cref="ArgumentException"> 
		/// Thrown if the <paramref name="domain"/> does not exist.
		/// </exception>
		public void CreateDepErrorLog (ActionAplication action, DomainActions domain, Student student,
			bool wasApplied, Dependence failedDependence)
		{
			//Gets the domainLog.
			DomainLog domainLog;
			if (!_domainLogs.TryGetValue (domain.Key, out domainLog))
				throw new ArgumentException ("There is not any DomainLog with the given domain.");
			//Gets the StudentLog.
			StudentLog studentLog = domainLog.GetStudentLog (student.Key);
			//Obtains if the dependence is an order dependence.
			bool isOrderError = false;
			//Checks the depedence type.
			if (failedDependence.GetType () == typeof(SeqComplexDependence))
				isOrderError = true;
			//Creates the new DepErrorLog.
			DepErrorLog log = new DepErrorLog (action, wasApplied,failedDependence, isOrderError);
			//Adds the log into the StudentLog.
			studentLog.AddLog (log);
			studentLog.AddToLogBuffer(log);

			//Creates a path.
			string path = _domainPath.Replace ('\\', Path.DirectorySeparatorChar) + "Logs" + Path.DirectorySeparatorChar
				+ domain.Key + Path.DirectorySeparatorChar;
			//Determine whether the directory exists. If the directory does not exist, it will be created.
			if (!Directory.Exists(path)) {
				Directory.CreateDirectory(path);
			}
			//Copies the log into a txt file.
			using (StreamWriter sw = File.AppendText (path + student.Key + ".txt")) {
				sw.WriteLine (log.TxtLogString());
			}
		}

		/// <summary>
		/// Creates the incomp error log.
		/// </summary>
		/// <param name="action">Action.</param>
		/// <param name="domain">Domain.</param>
		/// <param name="studentKey">Student key.</param>
		/// <param name="wasApplied">If set to <c>true</c> was applied.</param>
		/// <param name="failedIncomp">Failed incomp.</param>
		/// <exception cref="ArgumentException"> 
		/// Thrown if the <paramref name="domain"/> does not exist.
		/// </exception>
		public void CreateIncompErrorLog (ActionAplication action, DomainActions domain, Student student,
			bool wasApplied, Incompatibility failedIncomp)
		{
			//Gets the domainLog.
			DomainLog domainLog;
			if (!_domainLogs.TryGetValue (domain.Key, out domainLog))
				throw new ArgumentException ("There is not any DomainLog with the given domain.");
			//Gets the StudentLog.
			StudentLog studentLog = domainLog.GetStudentLog (student.Key);
			//Creates the new IncompErrorLog.
			IncompErrorLog log = new IncompErrorLog (action, wasApplied, failedIncomp);
			//Adds the log into the StudentLog.
			studentLog.AddLog (log);
			studentLog.AddToLogBuffer(log);

			//Creates a path.
			string path = _domainPath.Replace ('\\', Path.DirectorySeparatorChar) + "Logs" + Path.DirectorySeparatorChar
				+ domain.Key + Path.DirectorySeparatorChar;
			//Determine whether the directory exists. If the directory does not exist, it will be created.
			if (!Directory.Exists(path)) {
				Directory.CreateDirectory(path);
			}
			//Copies the log into a txt file.
			using (StreamWriter sw = File.AppendText (path + student.Key + ".txt")) {
				sw.WriteLine (log.TxtLogString());
			}
		}
*/
		/// <summary>
		/// Creates the domain log.
		/// </summary>
		/// <param name="domain">Domain.</param>
		public void CreateDomainLog (DomainActions domain)
		{
			//Creates a new DomainLog.
			DomainLog domainLog = new DomainLog (domain);
			//Adds into the dictionary.
			_domainLogs.Add (domainLog.Key, domainLog);
		}

		/// <summary>
		/// Creates the student log.
		/// </summary>
		/// <param name="domainKey">Domain key.</param>
		/// <param name="studentKey">Student key.</param>
		/// <exception cref="ArgumentException"> 
		/// Thrown if the <paramref name="domainKey"/> does not exist.
		/// </exception>
		/// <exception cref="ArgumentException"> 
		/// Thrown if the <paramref name="studentKey"/> does not exist.
		/// </exception>
		public void CreateStudentLog (string domainKey, string studentKey)
		{
			//Creates an auxiliar variable.
			DomainLog domainLog;
			//Tries to obtain the DomainLog with the given key.
			if (!_domainLogs.TryGetValue (domainKey, out domainLog)) {
				throw new ArgumentException ("There is not any DomainLog with the given key.");
			} else {
				//Creates an auxiliar variable.
				Student student;
				//Tries to obtain the Student with the given key.
				if (!_students.TryGetValue (studentKey, out student)) {
					throw new ArgumentException ("There is not any Student with the given key.");
				} else {
					//Creates the StudentLog.
					domainLog.CreateStudentLog (student);
				}
			}
		}

		/// <summary>
		/// Creates the student.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="name">Name.</param>
		/// <param name="middleName">Middle name.</param>
		/// <param name="surname">Surname.</param>
		/// <param name="lastName">Last name.</param>
		public void CreateStudent (string key, string name, string middleName, string surname, string lastName)
		{
			//Creates an auxiliar variable.
			Student student;
			//Creates the new Student.
			if (middleName == "" && lastName == "") {
				student = new Student (key, name, surname);
			} else {
				student = new Student (key, name, middleName, surname, lastName);
			}
			//Adds the Student into the dictionary.
			_students.Add (key, student);
			//Saves the student into the ontology.
			ONTOLOGY.AddStudentIntoOnto(student);
		}

		/// <summary>
		/// Gets the student log.
		/// </summary>
		/// <returns>The student log.</returns>
		/// <param name="domainKey">Domain key.</param>
		/// <param name="studentKey">Student key.</param>
		/// <exception cref="ArgumentException"> 
		/// Thrown if the <paramref name="domainKey"/> does not exist.
		/// </exception>
		public StudentLog GetStudentLog (string domainKey, string studentKey)
		{
			//Creates an auxiliar variable.
			DomainLog domainLog;
			//Creates an auxiliar variable.
			StudentLog studentLog;
			//Tries to obtain the DomainLog with the given key.
			if (!_domainLogs.TryGetValue (domainKey, out domainLog)) {
				throw new ArgumentException ("There is not any DomainLog with the given key.");
			} else {
				//Obtains the StudentLog.
				studentLog = domainLog.GetStudentLog (studentKey);
			}

			//Returns the StudentLog.
			return studentLog;
		}

		/// <summary>
		/// Checks the log action order.
		/// </summary>
		/// <returns><c>true</c>, if log action order was checked, <c>false</c> otherwise.</returns>
		/// <param name="domainKey">Domain key.</param>
		/// <param name="studentKey">Student key.</param>
		/// <param name="action1">Action1.</param>
		/// <param name="action2">Action2.</param>
		public bool CheckLogActionOrder (string domainKey, string studentKey, string action1, string action2)
		{
			//Creates an auxiliar variable.
			StudentLog studentLog = this.GetStudentLog (domainKey, studentKey);

			//Returns the check value.
			return studentLog.CheckActionOrder (action1, action2);
		}

		/// <summary>
		/// Gets the student.
		/// </summary>
		/// <returns>The student.</returns>
		/// <param name="studentKey">Student key.</param>
		/// <exception cref="ArgumentException"> 
		/// Thrown if the <paramref name="studentKey"/> does not exist.
		/// </exception>
		public Student GetStudent (string studentKey)
		{
			//Creates the variable which will be returned.
			Student student;
			//Tries to obtain the Student with the given key.
			if (!_students.TryGetValue (studentKey, out student)) {
				throw new ArgumentException ("There is not any Student with the given key.");
			}

			//Returns the Student.
			return student;
		}

		/// <summary>
		/// Gets the domain logs from ontology.
		/// </summary>
		/// <returns>The domain logs from ontology.</returns>
		/// <param name="domain">Domain.</param>
		/// <param name="otherErrors">Other errors.</param>
		/// <param name="worldErrors">World errors.</param>
		public DomainLog GetDomainLogsFromOntology (DomainActions domain, Dictionary<string, Error> otherErrors, Dictionary<string, Error> worldErrors)
		{
			//Creates domain log for the given domain.
			DomainLog domainLog = _instanceDomainLogFactory.CreateLogsFromOntology(domain, _students, otherErrors, worldErrors);
			//Adds into the dictionary.
			_domainLogs[domain.Key] = domainLog;
			//Returns the domain log.
			return domainLog;
		}

		/// <summary>
		/// Erases the action.
		/// </summary>
		/// <param name="actionName">Action name.</param>
		/// <param name="domainName">Domain name.</param>
		/// <param name="studentKey">Student key.</param>
		public void EraseAction (string actionKey, string domainName, string studentKey)
		{
			//Erases the action.
			GetStudentLog(domainName, studentKey).EraseAction (actionKey);
		}

		/// <summary>
		/// Resets the practice.
		/// </summary>
		/// <param name="domainName">Domain name.</param>
		/// <param name="studentKey">Student key.</param>
		public void ResetPractice (string domainName, string studentKey)
		{
			//Resets the log of the student.
			GetStudentLog (domainName, studentKey).ResetLog ();
		}

		/// <summary>
		/// Adds LogEntry to student log and to buffer that will be flushed to file (ontology xml)
		/// upon calling FlushLastActionLogs()
		/// </summary>
		/// <param name="domainName">Domain name.</param>
		/// <param name="studentKey">Student key.</param>
		/// <param name="logEntry">Log entry.</param>
		public void AddLog(DomainActions domain, Student student, LogEntry logEntry)
		{
			//Gets the domainLog.
			DomainLog domainLog;
			if (!_domainLogs.TryGetValue (domain.Key, out domainLog))
				throw new ArgumentException ("There is not any DomainLog with the given domain.");
			//Gets the StudentLog.
			StudentLog studentLog = domainLog.GetStudentLog (student.Key);
			
			studentLog.AddLog(logEntry);
			studentLog.AddToLogBuffer(logEntry);
		}
		
		/// <summary>
		/// Adds list of LogEntry objects to student log and to buffer that will be flushed to file (ontology xml)
		/// upon calling FlushLastActionLogs()
		/// </summary>
		/// <param name="domainName">Domain name.</param>
		/// <param name="studentKey">Student key.</param>
		/// <param name="logEntries">List of Log entry objects.</param>
		public void AddLog(DomainActions domain, Student student, List<LogEntry> logEntries)
		{
			//Gets the domainLog.
			DomainLog domainLog;
			if (!_domainLogs.TryGetValue (domain.Key, out domainLog))
				throw new ArgumentException ("There is not any DomainLog with the given domain.");
			//Gets the StudentLog.
			StudentLog studentLog = domainLog.GetStudentLog (student.Key);
			
			studentLog.AddLog(logEntries);
			studentLog.AddToLogBuffer(logEntries);
		}
		
		/// <summary>
		/// Writes log entries generated by last user action to ontology.
		/// </summary>
		/// <param name="domainName">Domain name.</param>
		/// <param name="studentKey">Student key.</param>
		public void FlushLastActionLogs (DomainActions domain, Student student)
		{
			//Gets the domainLog.
			DomainLog domainLog;
			if (!_domainLogs.TryGetValue (domain.Key, out domainLog))
				throw new ArgumentException ("There is not any DomainLog with the given domain.");
			//Gets the StudentLog.
			StudentLog studentLog = domainLog.GetStudentLog (student.Key);
			bool persist = false;

			// First, update ontology model with all log entries without writing each update to disk
			foreach (LogEntry log in studentLog.ActionLogBuffer)
			{	
				if (log.GetType () == typeof(NoCorrectiveActionLog))
					ONTOLOGY.AddNoCorrectiveActionLogIntoOnto(log, student, domain, persist);
				else if (log.GetType () == typeof(CorrectiveActionLog))
					ONTOLOGY.AddCorrectiveActionLogIntoOnto(log, student, domain, persist);
				else if (log.GetType () == typeof(NoPlanAllowedActionLog))
					ONTOLOGY.AddNoPlanAllowedActionLogIntoOnto(log, student, domain, persist);
				else if (log.GetType () == typeof(OtherErrorLog))
					ONTOLOGY.AddOtherErrorLogIntoOnto(log, student, domain, persist);
				else if (log.GetType () == typeof(DepErrorLog))
					ONTOLOGY.AddDepErrorLogIntoOnto(log, student, domain, persist);
				else if (log.GetType () == typeof(IncompErrorLog))
					ONTOLOGY.AddIncompErrorLogIntoOnto(log, student, domain, persist);
				else if (log.GetType () == typeof(WorldErrorLog))
					ONTOLOGY.AddWorldErrorLogIntoOnto(log, student, domain, persist);
				else if (log.GetType () == typeof(MinTimeErrorLog))
					ONTOLOGY.AddMinTimeErrorLogIntoOnto(log, student, domain, persist);
				else if (log.GetType () == typeof(MaxTimeErrorLog))
					ONTOLOGY.AddMaxTimeErrorLogIntoOnto(log, student, domain, persist);
			}
			
			// Write all updates to file in one go
			ONTOLOGY.SaveStudentTraceOnto(domain.Key, student.Key);

			studentLog.ActionLogBuffer.Clear();
		}
	}
}

