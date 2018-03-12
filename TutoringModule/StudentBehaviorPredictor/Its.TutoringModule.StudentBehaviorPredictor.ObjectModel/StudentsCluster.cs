using System;
using Its.Utils.Math;
using System.Collections.Generic;
using Its.StudentModule.ObjectModel;
using System.Data;

namespace Its.TutoringModule.StudentBehaviorPredictor.ObjectModel
{
	public class StudentsCluster
	{
		/// <summary>
		/// Cluster number.
		/// </summary>
		private int _number;
		/// <summary>
		/// The minimum time.
		/// </summary>
		private double _minTime;
		/// <summary>
		/// The average of time.
		/// </summary>
		private double _avgTime;
		/// <summary>
		/// The max time.
		/// </summary>
		private double _maxTime;
		/// <summary>
		/// The minimum errors.
		/// </summary>
		private int _minErrors;
		/// <summary>
		/// The max errors.
		/// </summary>
		private int _maxErrors;
		/// <summary>
		/// The average of errors.
		/// </summary>
		private double _avgErrors;
		/// <summary>
		/// The automaton.
		/// </summary>
		private StudentActionsModel _studentActionsModel;
		/// <summary>
		/// The student keys.
		/// </summary>
		private DataTable _students;

		public StudentsCluster (int number, double minTime, double maxTime, double avgTime, int minErrors, int maxErrors, double avgErrors)
		{
			this._number = number;
			this._minTime = minTime;
			this._maxTime = maxTime;
			this._avgTime = avgTime;
			this._minErrors = minErrors;
			this._maxErrors = maxErrors;
			this._avgErrors = avgErrors;
			this._studentActionsModel = new StudentActionsModel (0);
			this._students = new DataTable ();
			DataColumn dc = new DataColumn ("StudenKey", typeof(string));
			this._students.Columns.Add(dc);
			DataColumn[] key=new DataColumn[1];
			key [0] = dc;
			this._students.PrimaryKey = key;
			this._students.Columns.Add("LastState", typeof(Node<State,Event>));
			this._students.Columns.Add("VectorEventCount", typeof(int));
		}

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="Its.TutoringModule.StudentBehaviorPredictor.ObjectModel.StudentsCluster"/> class.
		/// </summary>
		/// <param name="number">Number.</param>
		/// <param name="minTime">Minimum time.</param>
		/// <param name="maxTime">Max time.</param>
		/// <param name="minErrors">Minimum errors.</param>
		/// <param name="maxErrors">Max errors.</param>
		/// <param name="studentKeys">Student keys.</param>
		/// <param name="logs">Logs.</param>
		public StudentsCluster (int number, double minTime, double maxTime, double avgTime, int minErrors, int maxErrors, double avgErrors, List<string> studentKeys, DomainLog logs)
		{
			this._number = number;
			this._minTime = minTime;
			this._maxTime = maxTime;
			this._avgTime = avgTime;
			this._minErrors = minErrors;
			this._maxErrors = maxErrors;
			this._avgErrors = avgErrors;
			this._students = new DataTable ();
			DataColumn dc = new DataColumn ("StudenKey", typeof(string));
			this._students.Columns.Add(dc);
			DataColumn[] key=new DataColumn[1];
			key [0] = dc;
			this._students.PrimaryKey = key;
			this._students.Columns.Add("LastState", typeof(Node<State,Event>));
			this._students.Columns.Add("VectorEventCount", typeof(int));
			this._studentActionsModel = new StudentActionsModel (studentKeys.Count);
			Node<State,Event> lastState = _studentActionsModel.InitState;

			foreach (string studentKey in studentKeys) {
				StudentLog studentLogs = logs.GetStudentLog (studentKey);

				AddStudent (studentLogs, lastState);
			}
		}

		public StudentsCluster (int number, List<string> studentKeys, DomainLog logs)
		{
			this._number = number;
			this._students = new DataTable ();
			DataColumn dc = new DataColumn ("StudenKey", typeof(string));
			this._students.Columns.Add(dc);
			DataColumn[] key=new DataColumn[1];
			key [0] = dc;
			this._students.PrimaryKey = key;
			this._students.Columns.Add("LastState", typeof(Node<State,Event>));
			this._students.Columns.Add("VectorEventCount", typeof(int));
			this._studentActionsModel = new StudentActionsModel (studentKeys.Count);
			Node<State,Event> lastState = _studentActionsModel.InitState;

			foreach (string studentKey in studentKeys) {
				StudentLog studentLogs = logs.GetStudentLog (studentKey);

				AddStudent (studentLogs, lastState);
			}
		}

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="Its.TutoringModule.StudentBehaviorPredictor.ObjectModel.StudentsCluster"/> class from a date (for visualization purposes)..
		/// </summary>
		/// <param name="number">Number.</param>
		/// <param name="minTime">Minimum time.</param>
		/// <param name="maxTime">Max time.</param>
		/// <param name="avgTime">Avg time.</param>
		/// <param name="minErrors">Minimum errors.</param>
		/// <param name="maxErrors">Max errors.</param>
		/// <param name="avgErrors">Avg errors.</param>
		/// <param name="studentKeys">Student keys.</param>
		/// <param name="logs">Logs.</param>
		/// <param name="fromDate">From date.</param>
		public StudentsCluster (int number, double minTime, double maxTime, double avgTime, int minErrors, int maxErrors, double avgErrors, List<string> studentKeys, DomainLog logs, DateTime fromDate)
		{
			this._number = number;
			this._minTime = minTime;
			this._maxTime = maxTime;
			this._avgTime = avgTime;
			this._minErrors = minErrors;
			this._maxErrors = maxErrors;
			this._avgErrors = avgErrors;
			this._students = new DataTable ();
			DataColumn dc = new DataColumn ("StudenKey", typeof(string));
			this._students.Columns.Add(dc);
			DataColumn[] key=new DataColumn[1];
			key [0] = dc;
			this._students.PrimaryKey = key;
			this._students.Columns.Add("LastState", typeof(Node<State,Event>));
			this._students.Columns.Add("VectorEventCount", typeof(int));
			this._studentActionsModel = new StudentActionsModel (studentKeys.Count);
			Node<State,Event> lastState = _studentActionsModel.InitState;

			foreach (string studentKey in studentKeys) {
				StudentLog studentLogs = logs.GetStudentLog (studentKey);
				if (((LogEntry)studentLogs.Logs [0]).DateLog >= fromDate)
					AddStudent (studentLogs, lastState);
			}
		}

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="Its.TutoringModule.StudentBehaviorPredictor.ObjectModel.StudentsCluster"/> class craetes a cluster only with the automata (for validation purposes)..
		/// </summary>
		/// <param name="number">Number.</param>
		/// <param name="minTime">Minimum time.</param>
		/// <param name="maxTime">Max time.</param>
		/// <param name="avgTime">Avg time.</param>
		/// <param name="minErrors">Minimum errors.</param>
		/// <param name="maxErrors">Max errors.</param>
		/// <param name="avgErrors">Avg errors.</param>
		/// <param name="studentKeys">Student keys.</param>
		/// <param name="logs">Logs.</param>
		public StudentsCluster (List<StudentLog> logs)
		{
			this._number = 0;
			this._minTime = 0;
			this._maxTime = 0;
			this._avgTime = 0;
			this._minErrors = 0;
			this._maxErrors = 0;
			this._avgErrors = 0;
			this._students = new DataTable ();
			DataColumn dc = new DataColumn ("StudenKey", typeof(string));
			this._students.Columns.Add(dc);
			DataColumn[] key=new DataColumn[1];
			key [0] = dc;
			this._students.PrimaryKey = key;
			this._students.Columns.Add("LastState", typeof(Node<State,Event>));
			this._students.Columns.Add("VectorEventCount", typeof(int));
			this._studentActionsModel = new StudentActionsModel (logs.Count);
			Node<State,Event> lastState = _studentActionsModel.InitState;

			foreach (StudentLog studentLogs in logs) {
				AddStudent (studentLogs, lastState);
			}
		}

		/// <summary>
		/// Gets the cluster number.
		/// </summary>
		/// <value>The number.</value>
		public int Number{
			get{
				return _number;
			}
		}

		/// <summary>
		/// Gets the minimum time.
		/// </summary>
		/// <value>The minimum time.</value>
		public double MinTime{
			get{
				return _minTime;
			}
		}

		/// <summary>
		/// Gets the max time.
		/// </summary>
		/// <value>The max time.</value>
		public double MaxTime{
			get{
				return _maxTime;
			}
		}

		/// <summary>
		/// Gets the average of time.
		/// </summary>
		/// <value>The avg time.</value>
		public double AvgTime{
			get{
				return _avgTime;
			}
		}

		/// <summary>
		/// Gets the minimum errors.
		/// </summary>
		/// <value>The minimum errors.</value>
		public int MinErrors{
			get{
				return _minErrors;
			}
		}

		/// <summary>
		/// Gets the max errors.
		/// </summary>
		/// <value>The max errors.</value>
		public int MaxErrors{
			get{
				return _maxErrors;
			}
		}

		/// <summary>
		/// Gets the average of errors.
		/// </summary>
		/// <value>The avg errors.</value>
		public double AvgErrors{
			get{
				return _avgErrors;
			}
		}

		public StudentActionsModel StudentActionsModel{
			get{
				return _studentActionsModel;
			}
		}

		public int NumberOfStudents{
			get{
				return _students.Rows.Count;
			}
		}

		/// <summary>
		/// Determines whether this instance has a student with the specified keyey.
		/// </summary>
		/// <returns><c>true</c> if this instance has a student with the specified key; otherwise, <c>false</c>.</returns>
		/// <param name="studentKey">Student key.</param>
		public bool HasStudent(string studentKey){
			Node<State,Event> lastState = _studentActionsModel.InitState;
			return _students.Rows.Contains (studentKey);
		}

		/// <summary>
		/// Inits a student in this cluster.
		/// </summary>
		/// <param name="studentKey">Student key.</param>
		public void InitStudent(string studentKey){
			if (_students.Rows.Contains (studentKey)) {
				DataRow dr = _students.Rows.Find (studentKey);
				dr ["LastState"] = _studentActionsModel.InitState;
				dr ["VectorEventCount"] = 0;
			} else {
				object[] dr=new object[3];
				dr [0] = studentKey;
				dr [1] = _studentActionsModel.InitState;
				dr [2] = 0;
				_students.Rows.Add (dr);
			}
		}

		/// <summary>
		/// Updates the automaton.
		/// </summary>
		/// <param name="studentKey">Student key.</param>
		/// <param name="log">Log.</param>
		public void UpdateAutomaton(string studentKey, LogEntry log){
			DataRow dr = _students.Rows.Find (studentKey);
			Node<State,Event> newState = _studentActionsModel.ExpandAutomaton (log, (Node<State,Event>)dr ["LastState"], (int)dr ["VectorEventCount"]);
			if (newState == (Node<State,Event>)dr ["LastState"])
				dr ["VectorEventCount"] = ((int)dr ["VectorEventCount"]) + 1;
		}

		/// <summary>
		/// Adds a student from logs to this cluster.
		/// </summary>
		/// <param name="studentLogs">Student logs.</param>
		/// <param name="lastState">Last state.</param>
		public void AddStudent (StudentLog studentLogs, Node<State,Event> previousState)
		{
			List<string> tempPastRepetitives = new List<string> ();
			int iterationNumber = 0;
			Node<State, Event> newState = null;
			bool isActionLog = false;
			foreach (LogEntry log in studentLogs.Logs) {
				isActionLog = (log.GetType () == typeof(NoCorrectiveActionLog) || log.GetType () == typeof(CorrectiveActionLog));
				if (!isActionLog ||
					(isActionLog && !log.Action.IsRepetitive) ||
					(isActionLog && log.Action.IsRepetitive && !tempPastRepetitives.Contains (log.Action.Key))){
					if (previousState.Specification.Area == Area.IrrelevantErrors)
						newState = _studentActionsModel.ExpandAutomaton (log, previousState, iterationNumber);
					else
						newState = _studentActionsModel.ExpandAutomaton (log, previousState);
					if (newState == previousState)
						iterationNumber++;

					if (log.GetType()==typeof(NoCorrectiveActionLog) || log.GetType()==typeof(CorrectiveActionLog) 
						&& log.Action.IsRepetitive)
						tempPastRepetitives.Add (newState.Key);
					previousState = newState;
				}
			}
			object[] dr = new object[3];
			dr [0] = studentLogs.Owner.Key;
			dr [1] = previousState;
			dr [2] = iterationNumber;
			this._students.Rows.Add (dr);
		}

		/// <summary>
		/// Adds a new student to this cluster.
		/// </summary>
		/// <param name="studenKey">Studen key.</param>
		/// <param name="previousState">Previous state.</param>
		public void AddStudent (string studenKey)
		{
			object[] dr = new object[3];
			dr [0] = studenKey;
			dr [1] = _studentActionsModel.InitState;
			dr [2] = 0;
			this._students.Rows.Add (dr);
		}

		/// <summary>
		/// Removes a student from this cluster.
		/// </summary>
		/// <param name="studentLogs">Student logs.</param>
		public void RemoveStudent(StudentLog studentLogs){
			int iterationNumber = 0;
			Node<State, Event> removedState = null;
			Node<State,Event> previousState = this._studentActionsModel.InitState;
			foreach (LogEntry log in studentLogs.Logs) {
				if (previousState.Specification.Area == Area.IrrelevantErrors)
					removedState = _studentActionsModel.ReduceAutomaton (log, previousState, iterationNumber);
				else
					removedState = _studentActionsModel.ReduceAutomaton (log, previousState);
				if (removedState == previousState)
					iterationNumber++;
				previousState = removedState;
			}
			this._students.Rows.Remove (this._students.Rows.Find (studentLogs.Owner.Key));
		}

		/// <summary>
		/// Returns a string with all student keys and the number of this cluster next to each.
		/// </summary>
		public string StudentsByClusterToString(){
			string temp = "";
			foreach (DataRow dr in _students.Rows) {
				temp += dr["StudenKey"].ToString() + " " + _number + Environment.NewLine;
			}
			return temp;
		}
			
		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="Its.TutoringModule.StudentBehaviorPredictor.ObjectModel.StudentsCluster"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="Its.TutoringModule.StudentBehaviorPredictor.ObjectModel.StudentsCluster"/>.</returns>
		public override string ToString(){
			return "Cluster: " + _number + Environment.NewLine + "Automaton: " + Environment.NewLine + _studentActionsModel.ToString ();
		}
	}
}

