using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Its.ExpertModule.ObjectModel;
using Its.StudentModule.ObjectModel;
using Its.TutoringModule.CMTutor.SBP.OM.State;
using Its.Utils.Math;

namespace Its.TutoringModule.CMTutor.SBP.OM
{
	public class StudentsCluster
	{
		/// <summary>
		/// Cluster number.
		/// </summary>
		private int _number;
		/// <summary>
		/// The total number of events used to create de model in this clusters.
		/// </summary>
		private long _numberEvents;
		/// <summary>
		/// The automaton.
		/// </summary>
		private StudentActionsModel _studentActionsModel;
		/// <summary>
		/// The student keys.
		/// </summary>
		private DataTable _studentStates;
		private Dictionary<string,List<Arc<State.State,Event.Event>>> _studentEvents;

		public StudentsCluster (int number)
		{
			this._number = number;
			this._numberEvents = 0;
			this._studentActionsModel = new StudentActionsModel (0);
			this._studentStates = new DataTable ();
			DataColumn dc = new DataColumn ("StudenKey", typeof(string));
			this._studentStates.Columns.Add(dc);
			DataColumn[] key=new DataColumn[1];
			key [0] = dc;
			this._studentStates.PrimaryKey = key;
			this._studentStates.Columns.Add("LastState", typeof(Node<State.State,Event.Event>));
			this._studentStates.Columns.Add("VectorEventCount", typeof(int));
			this._studentStates.Columns.Add("States", typeof(List<Node<State.State,Event.Event>>));
			this._studentEvents = new Dictionary<string, List<Arc<State.State, Event.Event>>> ();
		}

		public StudentsCluster (int number, List<string> studentKeys, DomainLog logs, Dictionary<string,ActionAplication> incompatibilities, bool includeNoPlanActions)
		{
			this._number = number;
			this._numberEvents = 0;
			this._studentStates = new DataTable ();
			DataColumn dc = new DataColumn ("StudenKey", typeof(string));
			this._studentStates.Columns.Add(dc);
			DataColumn[] key=new DataColumn[1];
			key [0] = dc;
			this._studentStates.PrimaryKey = key;
			this._studentStates.Columns.Add("LastState", typeof(Node<State.State,Event.Event>));
			this._studentStates.Columns.Add("VectorEventCount", typeof(int));
			this._studentStates.Columns.Add("States", typeof(List<Node<State.State,Event.Event>>));
			this._studentEvents = new Dictionary<string, List<Arc<State.State, Event.Event>>> ();
			this._studentActionsModel = new StudentActionsModel (studentKeys.Count);
			Node<State.State,Event.Event> lastState = _studentActionsModel.InitState;

			foreach (string studentKey in studentKeys) {
				StudentLog studentLogs = logs.GetStudentLog (studentKey);

				AddStudent (studentLogs, lastState, incompatibilities, includeNoPlanActions);
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
		public StudentsCluster (List<StudentLog> logs, Dictionary<string,ActionAplication> incompatibilities, bool includeNoPlanActions)
		{
			this._number = 0;
			this._numberEvents = 0;
			this._studentStates = new DataTable ();
			DataColumn dc = new DataColumn ("StudenKey", typeof(string));
			this._studentStates.Columns.Add(dc);
			DataColumn[] key=new DataColumn[1];
			key [0] = dc;
			this._studentStates.PrimaryKey = key;
			this._studentStates.Columns.Add("LastState", typeof(Node<State.State,Event.Event>));
			this._studentStates.Columns.Add("VectorEventCount", typeof(int));
			this._studentStates.Columns.Add("States", typeof(List<Node<State.State,Event.Event>>));
			this._studentEvents = new Dictionary<string, List<Arc<State.State, Event.Event>>> ();
			this._studentActionsModel = new StudentActionsModel (logs.Count);
			Node<State.State,Event.Event> lastState = _studentActionsModel.InitState;

			foreach (StudentLog studentLogs in logs) {
				AddStudent (studentLogs, lastState, incompatibilities, includeNoPlanActions);
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
		public StudentsCluster (int number, List<string> studentKeys, DomainLog logs, DateTime fromDate, Dictionary<string,ActionAplication> incompatibilities, bool includeNoPlanActions)
		{
			this._number = number;
			this._numberEvents = 0;
			this._studentStates = new DataTable ();
			DataColumn dc = new DataColumn ("StudenKey", typeof(string));
			this._studentStates.Columns.Add(dc);
			DataColumn[] key=new DataColumn[1];
			key [0] = dc;
			this._studentStates.PrimaryKey = key;
			this._studentStates.Columns.Add("LastState", typeof(Node<State.State,Event.Event>));
			this._studentStates.Columns.Add("VectorEventCount", typeof(int));
			this._studentStates.Columns.Add("States", typeof(List<Node<State.State,Event.Event>>));
			this._studentEvents = new Dictionary<string, List<Arc<State.State, Event.Event>>> ();
			this._studentActionsModel = new StudentActionsModel (studentKeys.Count);
			Node<State.State,Event.Event> lastState = _studentActionsModel.InitState;

			foreach (string studentKey in studentKeys) {
				StudentLog studentLogs = logs.GetStudentLog (studentKey);
				if (((LogEntry)studentLogs.Logs [0]).DateLog >= fromDate)
					AddStudent (studentLogs, lastState, incompatibilities, includeNoPlanActions);
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

		public StudentActionsModel StudentActionsModel{
			get{
				return _studentActionsModel;
			}
		}

		public int NumberOfStudents{
			get{
				return _studentStates.Rows.Count;
			}
		}

		public long NumberOfEvents{
			get{
				return _numberEvents;
			}
		}

		/// <summary>
		/// Determines whether this instance has a student with the specified keyey.
		/// </summary>
		/// <returns><c>true</c> if this instance has a student with the specified key; otherwise, <c>false</c>.</returns>
		/// <param name="studentKey">Student key.</param>
		public bool HasStudent(string studentKey){
			Node<State.State,Event.Event> lastState = _studentActionsModel.InitState;
			return _studentStates.Rows.Contains (studentKey);
		}

		/// <summary>
		/// Inits a student in this cluster.
		/// </summary>
		/// <param name="studentKey">Student key.</param>
		public void InitStudent(string studentKey){
			if (_studentStates.Rows.Contains (studentKey)) {
				DataRow dr = _studentStates.Rows.Find (studentKey);
				dr ["LastState"] = _studentActionsModel.InitState;
				dr ["VectorEventCount"] = 0;
			} else {
				object[] dr=new object[4];
				dr [0] = studentKey;
				dr [1] = _studentActionsModel.InitState;
				dr [2] = 0;
				dr [3] = new List<Node<State.State,Event.Event>> ();
				_studentStates.Rows.Add (dr);
			}
		}

		/// <summary>
		/// Updates the automaton.
		/// </summary>
		/// <param name="studentKey">Student key.</param>
		/// <param name="log">Log.</param>
		public void UpdateAutomaton(string studentKey, LogEntry log, Dictionary<string,ActionAplication> incompatibilities){
			DataRow dr = _studentStates.Rows.Find (studentKey);
			Node<State.State,Event.Event> newState = _studentActionsModel.ExpandAutomaton (log, (Node<State.State,Event.Event>)dr ["LastState"], 
				(List<Node<State.State,Event.Event>>)dr ["States"], true, incompatibilities, (int)dr ["VectorEventCount"]);
			if (newState == (Node<State.State,Event.Event>)dr ["LastState"])
				dr ["VectorEventCount"] = ((int)dr ["VectorEventCount"]) + 1;
		}

		/// <summary>
		/// Adds a student from logs to this cluster.
		/// </summary>
		/// <param name="studentLogs">Student logs.</param>
		/// <param name="lastState">Last state.</param>
		public void AddStudent (StudentLog studentLogs, Node<State.State,Event.Event> previousState, Dictionary<string,ActionAplication> incompatibilities, bool includeNoPlanActions)
		{
			List<Node<State.State,Event.Event>> tempPastRepetitives = new List<Node<State.State,Event.Event>> ();
			List<Node<State.State,Event.Event>> tempPastNodes = new List<Node<State.State,Event.Event>> ();
			int iterationNumber = 0;
			Node<State.State, Event.Event> newState = null;
			bool isPastRepetitive = false;
			bool isActionLog = false;
			this._studentEvents.Add (studentLogs.Owner.Key, new List<Arc<State.State, Event.Event>> ());
			Area tempAreaForRepetitives = Area.CorrectFlow;
			foreach (LogEntry log in studentLogs.Logs) {
				if (includeNoPlanActions || log.GetType () != typeof(NoPlanAllowedActionLog)) {
					isActionLog = (log.GetType () == typeof(NoCorrectiveActionLog) || log.GetType () == typeof(CorrectiveActionLog) || log.GetType () == typeof(NoPlanAllowedActionLog));
					isPastRepetitive = false;
					if (isActionLog) {
						Node<State.State, Event.Event> tmp =
							tempPastRepetitives.SingleOrDefault (s => ((CorrectState)s.Specification).Action.Key == log.Action.Key &&
								s.Specification.Area == tempAreaForRepetitives);
						if (tmp != default(Node<State.State, Event.Event>))
							isPastRepetitive = true;
					}
					if (previousState.Specification.Area == Area.IrrelevantErrors)
						newState = _studentActionsModel.ExpandAutomaton (log, previousState, tempPastNodes, isPastRepetitive, incompatibilities, iterationNumber);
					else
						newState = _studentActionsModel.ExpandAutomaton (log, previousState, tempPastNodes, isPastRepetitive, incompatibilities);

					if (newState.Specification.Area != Area.IrrelevantErrors)
						tempAreaForRepetitives = newState.Specification.Area;
					
					if (newState == previousState && !isPastRepetitive)
						iterationNumber++;
					else if(!isPastRepetitive)
						this._studentEvents [studentLogs.Owner.Key].Add (this.StudentActionsModel.GetEvent (previousState.Key, newState.Key));
					if (isActionLog && log.Action.IsRepetitive && !isPastRepetitive && !tempPastRepetitives.Contains (newState))
						tempPastRepetitives.Add (newState);
					if (!tempPastNodes.Contains (newState)) {
						tempPastNodes.Add (newState);
					} else {
						int index = tempPastNodes.IndexOf (tempPastNodes.Find (x => x == newState));
						tempPastNodes [index] = newState;
					}

					/*if ((previousState.Key.Contains ("f1t31") && newState.Key.Contains ("f1t32_f1t31"))
					   || (previousState.Key.Contains ("f1t13") && newState.Key.Contains ("f1t20_f1t19"))
					   || (previousState.Key.Contains ("f1t13") && newState.Key.Contains ("f1t20_f1t16"))
					   || (previousState.Key.Contains ("f1t13") && newState.Key.Contains ("f1t20_f1t17"))
					   || (previousState.Key.Contains ("f1t11") && newState.Key.Contains ("f1t16_f1t12"))) {
						int x = 0;
					}*/

					previousState = newState;
				}
			}
			_numberEvents += studentLogs.Logs.Count;
			object[] dr = new object[4];
			dr [0] = studentLogs.Owner.Key;
			dr [1] = previousState;
			dr [2] = iterationNumber;
			dr [3] = tempPastNodes;
			this._studentStates.Rows.Add (dr);
		}

		/// <summary>
		/// Adds a new student to this cluster.
		/// </summary>
		/// <param name="studenKey">Studen key.</param>
		/// <param name="previousState">Previous state.</param>
		public void AddStudent (string studenKey, LogEntry log, Dictionary<string,ActionAplication> incompatibilities, bool includeNoPlanActions)
		{
			if (includeNoPlanActions || log.GetType () != typeof(NoPlanAllowedActionLog)) {
				object[] dr = new object[4];
				dr [0] = studenKey;
				dr [1] = _studentActionsModel.InitState;
				dr [2] = 0;
				dr [3] = new List<Node<State.State,Event.Event>> ();
				this._studentStates.Rows.Add (dr);
				this._studentEvents.Add (studenKey, new List<Arc<State.State, Event.Event>> ());
				UpdateAutomaton (studenKey, log, incompatibilities);
			}
		}

		/// <summary>
		/// Removes a student from this cluster.
		/// </summary>
		/// <param name="studentLogs">Student logs.</param>
		public void RemoveStudent(string studentKey, List<Node<State.State,Event.Event>> pastNodes, int numberStudentLogs){
			int iterationNumber = 0;
			Node<State.State,Event.Event> previousState = this._studentActionsModel.InitState;
			foreach (Node<State.State,Event.Event> node in pastNodes) {
				if (previousState.Specification.Area == Area.IrrelevantErrors)
					_studentActionsModel.ReduceAutomaton (previousState, node, iterationNumber);
				else
					_studentActionsModel.ReduceAutomaton (previousState, node);
				if (node == previousState)
					iterationNumber++;
				previousState = node;
			}
			_numberEvents -= numberStudentLogs;
			this._studentStates.Rows.Remove (this._studentStates.Rows.Find (studentKey));
		}

		/// <summary>
		/// Returns a string with all student keys and the number of this cluster next to each.
		/// </summary>
		public string StudentsByClusterToString(){
			string temp = "";
			foreach (DataRow dr in _studentStates.Rows) {
				temp += dr["StudenKey"].ToString() + " " + _number + Environment.NewLine;
			}
			return temp;
		}

		public List<string> GetStudentKeys(){
			List<string> temp = new List<string> ();
			foreach (DataRow dr in _studentStates.Rows) {
				temp.Add(dr["StudenKey"].ToString());
			}
			return temp;
		}

		/// <summary>
		/// Gets the nodes through which a student has passed.
		/// </summary>
		/// <returns>The student nodes.</returns>
		/// <param name="StudentKey">Student key.</param>
		public List<Node<State.State,Event.Event>> GetStudentNodes(string StudentKey){
			return (List<Node<State.State,Event.Event>>)_studentStates.Rows.Find(StudentKey)[3];
		}

		public List<Arc<State.State,Event.Event>> GetStudentEvents(string StudentKey){
			return _studentEvents[StudentKey];
		}

		/// <summary>
		/// Gets the next most probable event.
		/// </summary>
		/// <returns>The next probable event.</returns>
		/// <param name="studentKey">Student key.</param>
		public Arc<State.State,Event.Event> GetNextProbableEvent(string studentKey){
			return _studentActionsModel.GetNextProbableEvent ((Node<State.State,Event.Event>)_studentStates.Rows.Find (studentKey) [1]);
		}

		/// <summary>
		/// Gets the support value for the last state corresponding to last performed action
		/// </summary>
		/// <returns>Support value in the range [0, 1].</returns>
		/// <param name="studentKey">Student key.</param>
		public double GetLastStateSupport(string studentKey)
		{
			DataRow dr = _studentStates.Rows.Find (studentKey);
			Node<State.State, Event.Event> lastState = (Node<State.State, Event.Event>) dr["LastState"];
			double gamma = lastState.Specification.StudentFrequency;
			double support = gamma / NumberOfStudents;
			return support;
		}
			
		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="StudentsCluster"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="StudentsCluster"/>.</returns>
		public override string ToString(){
			return "Cluster: " + _number + Environment.NewLine + "Automaton: " + Environment.NewLine + _studentActionsModel.ToString ();
		}
	}
}

