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
			// Caching past repetitive states to avoid recalculating whenever new student Action is added to the cluster
			this._studentStates.Columns.Add("RepetitiveStates", typeof(List<Node<State.State,Event.Event>>));
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
			// Caching past repetitive states to avoid recalculating whenever new student Action is added to the cluster
			this._studentStates.Columns.Add("RepetitiveStates", typeof(List<Node<State.State,Event.Event>>));
			this._studentEvents = new Dictionary<string, List<Arc<State.State, Event.Event>>> ();
			this._studentActionsModel = new StudentActionsModel (studentKeys.Count);

			foreach (string studentKey in studentKeys) {
				StudentLog studentLogs = logs.GetStudentLog (studentKey);

				AddStudent (studentLogs, incompatibilities, includeNoPlanActions);
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
			// Caching past repetitive states to avoid recalculating whenever new student Action is added to the cluster
			this._studentStates.Columns.Add("RepetitiveStates", typeof(List<Node<State.State,Event.Event>>));
			this._studentEvents = new Dictionary<string, List<Arc<State.State, Event.Event>>> ();
			this._studentActionsModel = new StudentActionsModel (logs.Count);

			foreach (StudentLog studentLogs in logs) {
				AddStudent (studentLogs, incompatibilities, includeNoPlanActions);
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
			// Caching past repetitive states to avoid recalculating whenever new student Action is added to the cluster
			this._studentStates.Columns.Add("RepetitiveStates", typeof(List<Node<State.State,Event.Event>>));
			this._studentEvents = new Dictionary<string, List<Arc<State.State, Event.Event>>> ();
			this._studentActionsModel = new StudentActionsModel (studentKeys.Count);

			foreach (string studentKey in studentKeys) {
				StudentLog studentLogs = logs.GetStudentLog (studentKey);
				if (((LogEntry)studentLogs.Logs [0]).DateLog >= fromDate)
					AddStudent (studentLogs, incompatibilities, includeNoPlanActions);
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
				dr [4] = new List<Node<State.State,Event.Event>> ();
				_studentStates.Rows.Add (dr);
			}
		}

		/// <summary>
		/// Updates the automaton.
		/// </summary>
		/// <param name="studentKey">Student key.</param>
		/// <param name="log">Log.</param>
		private void UpdateAutomaton(string studentKey, LogEntry log, Dictionary<string,ActionAplication> incompatibilities, DataRow studentDataRow){
			List<Node<State.State,Event.Event>> tempPastRepetitives = (List<Node<State.State,Event.Event>>)studentDataRow["RepetitiveStates"];
			List<Node<State.State,Event.Event>> tempPastNodes = (List<Node<State.State,Event.Event>>)studentDataRow["States"];
			int iterationNumber = (int)studentDataRow["VectorEventCount"];
			Node<State.State, Event.Event> previousState = (Node<State.State,Event.Event>)studentDataRow["LastState"];
			
			Area tempAreaForRepetitives = previousState.Specification.Area;
			Node<State.State, Event.Event> newState = null;
			bool isPastRepetitive = false;
			// Loop start
			bool isActionLog = (log.GetType () == typeof(NoCorrectiveActionLog) || 
			                    log.GetType () == typeof(CorrectiveActionLog) || 
			                    log.GetType () == typeof(NoPlanAllowedActionLog));
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
			
			if (newState == previousState && !isPastRepetitive)
				iterationNumber++;
			else if(!isPastRepetitive)
				this._studentEvents [studentKey].Add (this.StudentActionsModel.GetEvent (previousState.Key, newState.Key));
			
			if (isActionLog && log.Action.IsRepetitive && !isPastRepetitive && !tempPastRepetitives.Contains (newState))
				tempPastRepetitives.Add (newState);
			if (!tempPastNodes.Contains (newState)) {
				tempPastNodes.Add (newState);
			} else {
				int index = tempPastNodes.IndexOf (tempPastNodes.Find (x => x == newState));
				tempPastNodes [index] = newState;
			}

			studentDataRow["States"] = tempPastNodes;
			studentDataRow["LastState"] = newState;
			studentDataRow["RepetitiveStates"] = tempPastRepetitives;
			studentDataRow["VectorEventCount"] = iterationNumber;
		}

		/// <summary>
		/// Adds a student from logs to this cluster.
		/// </summary>
		/// <param name="studentLogs">Student logs.</param>
		/// <param name="lastState">Last state.</param>
		public void AddStudent (StudentLog studentLogs, Dictionary<string,ActionAplication> incompatibilities, bool includeNoPlanActions)
		{
			string studentKey = studentLogs.Owner.Key;
			
			_numberEvents += studentLogs.Logs.Count;
			this._studentEvents.Add (studentKey, new List<Arc<State.State, Event.Event>> ());
			InitStudent(studentKey);
			
			DataRow dr = _studentStates.Rows.Find (studentKey);
			// Suspending row events until all log entries are processed
			dr.BeginEdit();
			
			foreach (LogEntry log in studentLogs.Logs) {
				if (includeNoPlanActions || log.GetType () != typeof(NoPlanAllowedActionLog)) {
					UpdateAutomaton(studentKey, log, incompatibilities, dr);
				}
			}
			
			dr.AcceptChanges();
		}

		/// <summary>
		/// Adds a new student to this cluster.
		/// </summary>
		/// <param name="studenKey">Studen key.</param>
		/// <param name="previousState">Previous state.</param>
		public void AddStudent (string studentKey, LogEntry log, Dictionary<string,ActionAplication> incompatibilities, bool includeNoPlanActions)
		{
			_numberEvents += 1;
			this._studentEvents.Add (studentKey, new List<Arc<State.State, Event.Event>> ());
			InitStudent(studentKey);
			
			DataRow dr = _studentStates.Rows.Find (studentKey);
			
			if (includeNoPlanActions || log.GetType () != typeof(NoPlanAllowedActionLog)) {				
				UpdateAutomaton (studentKey, log, incompatibilities, dr);
			}
		}
		
		/// <summary>
		/// Adds a new student action to this cluster.
		/// </summary>
		/// <param name="studenKey">Studen key.</param>
		/// <param name="previousState">Previous state.</param>
		public void AddStudentAction (string studentKey, LogEntry log, Dictionary<string,ActionAplication> incompatibilities, bool includeNoPlanActions)
		{
			_numberEvents += 1;
			if (includeNoPlanActions || log.GetType () != typeof(NoPlanAllowedActionLog)) {
				DataRow dr = _studentStates.Rows.Find (studentKey);
				UpdateAutomaton (studentKey, log, incompatibilities, dr);
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

		public Node<State.State, Event.Event> GetLastState(string studentKey)
		{
			DataRow dr = _studentStates.Rows.Find (studentKey);
			return (Node<State.State, Event.Event>) dr["LastState"];
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
		/// Gets the next most probable correct event.
		/// </summary>
		/// <returns>The next probable event.</returns>
		/// <param name="studentKey">Student key.</param>
		public Arc<State.State,Event.Event> GetNextProbableCorrectEvent(string studentKey){
			return _studentActionsModel.GetNextProbableCorrectEvent ((Node<State.State,Event.Event>)_studentStates.Rows.Find (studentKey) [1]);
		}

		/// <summary>
		/// Gets the support value for the last state corresponding to last performed action
		/// </summary>
		/// <returns>Support value in the range [0, 1].</returns>
		/// <param name="studentKey">Student key.</param>
		public double GetLastStateSupport(string studentKey)
		{
			return _studentActionsModel.GetStateSupport(GetLastState(studentKey), NumberOfStudents);
		}

		public List<Arc<State.State, Event.Event>> GetAllNextEvents(string studentKey)
		{
			DataRow dr = _studentStates.Rows.Find (studentKey);
			Node<State.State, Event.Event> lastState = (Node<State.State, Event.Event>) dr["LastState"];
			return lastState.OutArcs.Values.ToList();
		}
		
		public List<Arc<State.State, Event.Event>> GetNextCorrectEventsAboveThreshold(string studentKey, double threshold)
		{
			List<Arc<State.State, Event.Event>> events = this.GetAllNextEvents(studentKey);
			events = _studentActionsModel.SelectCorrectEvents(events);
			events = _studentActionsModel.SelectEventsAboveConfidenceThreshold(events, threshold);
			return events;
		}
		
		public double GetEventConfidence(Arc<State.State, Event.Event> evt)
		{
			return _studentActionsModel.GetEventConfidence(evt);
		}
		
		public List<Node<State.State, Event.Event>> GetAllREStatesAboveThreshold(double supportThreshold)
		{
			List<Node<State.State, Event.Event>> results = new List<Node<State.State, Event.Event>>();
			List<Node<State.State, Event.Event>> reStates = _studentActionsModel.GetStatesByArea(Area.RelevantErrors);
			foreach (Node<State.State, Event.Event> state in reStates)
			{
				if (_studentActionsModel.GetStateSupport(state, NumberOfStudents) >= supportThreshold)
				{
					results.Add(state);
				}
			}
			
			return results;
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

