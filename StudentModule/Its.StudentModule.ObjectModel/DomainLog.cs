using System;
using System.Collections.Generic;
using Its.ExpertModule.ObjectModel;

namespace Its.StudentModule.ObjectModel
{
	/// <author>
	/// Diego Dotor Jara.
	/// </author>
	/// <summary>
	/// This class will contain all student's log from a specific domain.
	/// </summary>
	public class DomainLog
	{
		/// <summary>
		/// The key.
		/// </summary>
		private string _key;
		/// <summary>
		/// Gets the key.
		/// </summary>
		/// <value>The key.</value>
		public string Key {
			get {
				return _key;
			}
		}
		/// <summary>
		/// The domain.
		/// </summary>
		private DomainActions _domain;
		/// <summary>
		/// Gets the domain.
		/// </summary>
		/// <value>The domain.</value>
		public DomainActions Domain {
			get {
				return _domain;
			}
		}
		/// <summary>
		/// The student logs.
		/// </summary>
		private Dictionary<string, StudentLog> _studentLogs;

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.StudentModule.ObjectModel.DomainLog"/> class.
		/// </summary>
		/// <param name="domain">Domain.</param>
		public DomainLog (DomainActions domain)
		{
			this._key = domain.Key;
			this._domain = domain;
			this._studentLogs = new Dictionary<string, StudentLog> ();
		}

		/// <summary>
		/// Creates the student log.
		/// </summary>
		/// <param name="student">Student.</param>
		public void CreateStudentLog (Student student)
		{
			//Creates a new StudentLog.
			StudentLog stdLog = new StudentLog (student);
			//Adds into the dictionary.
			this._studentLogs.Add (student.Key, stdLog);
		}

		/// <summary>
		/// Gets the student log.
		/// </summary>
		/// <returns>The student log.</returns>
		/// <param name="studentKey">Student key.</param>
		/// <exception cref="ArgumentException"> 
		/// Thrown if the <paramref name="studentKey"/> does not exist.
		/// </exception>
		public StudentLog GetStudentLog (string studentKey)
		{
			//Creates a variable which will be returned.
			StudentLog stdLog;
			//Searchs the StudentLog with the given key.
			if (!_studentLogs.TryGetValue (studentKey, out stdLog))
				//Throw an exception if there is not any StudentLog with the given key. 
				throw new ArgumentException ("There is not any StudentLog with the given key.");
			return stdLog;
		}

		/// <summary>
		/// Gets the collection of students that have logs in this domain.
		/// </summary>
		/// <returns>The student.</returns>
		public List<Student> GetStudents(){
			List<Student> students = new List<Student> ();
			foreach (StudentLog log in _studentLogs.Values) {
				students.Add (log.Owner);
			}
			return students;
		}
	}
}

