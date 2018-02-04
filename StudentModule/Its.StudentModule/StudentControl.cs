using System;
using System.IO;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using Its.StudentModule.ObjectModel;
using Its.StudentModule.DataAccess;
using Its.ExpertModule.ObjectModel;
using Its.Factories;
using Its.TutoringModule.TutoringCoordinator.ReactiveTutor.ObjectModel;

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
			_instanceDomainLogFactory = Factories.DomainLogFactory.Instance (ontologyPath, logsPath);

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

		/// <summary>
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
			//Saves the log into the ontology.
			ONTOLOGY.AddLogIntoOnto (log, student, domain);

			//Creates a path.
			string path = _domainPath.Replace ('\\', Path.DirectorySeparatorChar) + "Logs" + Path.DirectorySeparatorChar
				+ domain.Key + Path.DirectorySeparatorChar;
			//Determine whether the directory exists. If the directory does not exist, it will be created.
			if (!Directory.Exists(path)) {
				Directory.CreateDirectory(path);
			}
			//Copies the log into a txt file.
			using (StreamWriter sw = File.AppendText (path + student.Key + ".txt")) {
				string text = action.Key + " " + action.Name + " " + log.DateLog.ToString();
				sw.WriteLine (text);
			}
		}

		/*public void MultiplyLogs(int mult, DomainActions domain){
			int countStudents = _instance._students.Count;
			int maxStudent = countStudents * mult;
			DomainLog domTemp = _domainLogs [domain.Key];
			int j = 1;
			for (int i = _instance._students.Count; i < maxStudent; i++) {
				Student stdTemp = _students [j.ToString ()];
				_domainLogs [domain.Key].AddStudentLog (stdTemp, domTemp.GetStudentLog (stdTemp.Key));
				if (j <= countStudents)
					j++;
				else
					j = 1;
			}
		}*/

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
			//Saves the log into the ontology.
			ONTOLOGY.AddLogIntoOnto (log, student, domain);

			//Creates a path.
			string path = _domainPath.Replace ('\\', Path.DirectorySeparatorChar) + "Logs" + Path.DirectorySeparatorChar
				+ domain.Key + Path.DirectorySeparatorChar;
			//Determine whether the directory exists. If the directory does not exist, it will be created.
			if (!Directory.Exists(path)) {
				Directory.CreateDirectory(path);
			}
			//Copies the log into a txt file.
			using (StreamWriter sw = File.AppendText (path + student.Key + ".txt")) {
				string text = action.Key + " " + action.Name + " " + log.DateLog.ToString();
				sw.WriteLine (text);
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
			//Saves the log into the ontology.
			ONTOLOGY.AddLogIntoOnto (log, student, domain);


			//Creates a path.
			string path = _domainPath.Replace ('\\', Path.DirectorySeparatorChar) + "Logs" + Path.DirectorySeparatorChar
				+ domain.Key + Path.DirectorySeparatorChar;
			//Determine whether the directory exists. If the directory does not exist, it will be created.
			if (!Directory.Exists(path)) {
				Directory.CreateDirectory(path);
			}
			//Copies the log into a txt file.
			using (StreamWriter sw = File.AppendText (path + student.Key + ".txt")) {
				string text = action.Key + " " + action.Name + " MinTimeError " + action.MinTimeError.Message.Message + " " + log.DateLog.ToString();
				sw.WriteLine (text);
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
			//Saves the log into the ontology.
			ONTOLOGY.AddLogIntoOnto (log, student, domain);

			//Creates a path.
			string path = _domainPath.Replace ('\\', Path.DirectorySeparatorChar) + "Logs" + Path.DirectorySeparatorChar
				+ domain.Key + Path.DirectorySeparatorChar;
			//Determine whether the directory exists. If the directory does not exist, it will be created.
			if (!Directory.Exists(path)) {
				Directory.CreateDirectory(path);
			}
			//Copies the log into a txt file.
			using (StreamWriter sw = File.AppendText (path + student.Key + ".txt")) {
				string text = action.Key + " " + action.Name + " MaxTimeError " + action.MaxTimeError.Message.Message + " " + log.DateLog.ToString();
				sw.WriteLine (text);
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
			//Saves the log into the ontology.
			ONTOLOGY.AddLogIntoOnto (log, student, domain);

			//Creates a path.
			string path = _domainPath.Replace ('\\', Path.DirectorySeparatorChar) + "Logs" + Path.DirectorySeparatorChar
			              + domain.Key + Path.DirectorySeparatorChar;
			//Determine whether the directory exists. If the directory does not exist, it will be created.
			if (!Directory.Exists(path)) {
				Directory.CreateDirectory(path);
			}
			//Copies the log into a txt file.
			using (StreamWriter sw = File.AppendText (path + student.Key + ".txt")) {
				string text = action.Key + " " + action.Name + " OtherError " + error.Message.Message + " " + log.DateLog.ToString();
				sw.WriteLine (text);
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
			//Saves the log into the ontology.
			ONTOLOGY.AddLogIntoOnto (log, student, domain);

			//Creates a path.
			string path = _domainPath.Replace ('\\', Path.DirectorySeparatorChar) + "Logs" + Path.DirectorySeparatorChar
				+ domain.Key + Path.DirectorySeparatorChar;
			//Determine whether the directory exists. If the directory does not exist, it will be created.
			if (!Directory.Exists(path)) {
				Directory.CreateDirectory(path);
			}
			//Copies the log into a txt file.
			using (StreamWriter sw = File.AppendText (path + student.Key + ".txt")) {
				string text = action.Key + " " + action.Name + " WorldError " + error.Message.Message + " " + log.DateLog.ToString();
				sw.WriteLine (text);
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
			//Saves the log into the ontology.
			ONTOLOGY.AddLogIntoOnto (log, student, domain);

			//Creates a path.
			string path = _domainPath.Replace ('\\', Path.DirectorySeparatorChar) + "Logs" + Path.DirectorySeparatorChar
				+ domain.Key + Path.DirectorySeparatorChar;
			//Determine whether the directory exists. If the directory does not exist, it will be created.
			if (!Directory.Exists(path)) {
				Directory.CreateDirectory(path);
			}
			//Copies the log into a txt file.
			using (StreamWriter sw = File.AppendText (path + student.Key + ".txt")) {
				string text = action.Key + " " + action.Name + " DependenceError " + failedDependence.DependenceError.Message.Message + " " + log.DateLog.ToString();
				sw.WriteLine (text);
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
			//Saves the log into the ontology.
			ONTOLOGY.AddLogIntoOnto (log, student, domain);

			//Creates a path.
			string path = _domainPath.Replace ('\\', Path.DirectorySeparatorChar) + "Logs" + Path.DirectorySeparatorChar
				+ domain.Key + Path.DirectorySeparatorChar;
			//Determine whether the directory exists. If the directory does not exist, it will be created.
			if (!Directory.Exists(path)) {
				Directory.CreateDirectory(path);
			}
			//Copies the log into a txt file.
			using (StreamWriter sw = File.AppendText (path + student.Key + ".txt")) {
				string text = action.Key + " " + action.Name + " IncompatibilityError " + failedIncomp.IncompatibilityError.Message.Message + " " + log.DateLog.ToString();
				sw.WriteLine (text);
			}
		}

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
	}
}

