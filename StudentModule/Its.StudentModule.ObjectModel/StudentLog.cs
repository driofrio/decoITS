using System.Collections.Generic;
using System.Linq;

namespace Its.StudentModule.ObjectModel
{
	/// <author>
	/// Diego Dotor Jara.
	/// </author>
	/// <summary>
	/// This class will contain all student's logs.
	/// </summary>
	public class StudentLog
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
		/// The owner.
		/// </summary>
		private Student _owner;
		/// <summary>
		/// Gets the owner.
		/// </summary>
		/// <value>The owner.</value>
		public Student Owner {
			get {
				return _owner;
			}
		}
		/// <summary>
		/// The logs.
		/// </summary>
		private List<LogEntry> _logs;
		/// <summary>
		/// Gets the logs.
		/// </summary>
		/// <value>The logs.</value>
		public List<LogEntry> Logs {
			get {
				return _logs.OrderBy(p=>p.DateLog).ToList<LogEntry>();
			}
		}
		/// <summary>
		/// Logs produced by last user action. Not written to ontology until StudentControl.FlushLastActionLogs() method is called
		/// </summary>
		private List<LogEntry> _lastActionLogs;
		/// <summary>
		/// Gets logs produced by last user action.
		/// </summary>
		/// <value>The logs.</value>
		public List<LogEntry> LastActionLogs {
			get {
				return _lastActionLogs.OrderBy(p=>p.DateLog).ToList<LogEntry>();
			}
		}
		/// <summary>
		/// The current phase.
		/// </summary>
		private int _currentPhase;
		/// <summary>
		/// Gets or sets the current phase.
		/// </summary>
		/// <value>The current phase.</value>
		public int CurrentPhase {
			get {
				return _currentPhase;
			}
			set {
				_currentPhase = value;
			}
		}
		/// <summary>
		/// The action key to erase.
		/// </summary>
		private static string _actionKeyToErase;

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.StudentModule.ObjectModel.StudentLog"/> class.
		/// </summary>
		/// <param name="student">Student.</param>
		public StudentLog (Student student)
		{
			this._key = student.Key;
			this._owner = student;
			this._logs = new List<LogEntry> ();
		}

		public StudentLog (Student student, List<LogEntry> logs)
		{
			this._key = student.Key;
			this._owner = student;
			this._logs = logs;
		}

		/// <summary>
		/// Adds the log.
		/// </summary>
		/// <param name="log">Log.</param>
		public void AddLog (LogEntry log)
		{
			//Adds the log into the list.
			this._logs.Add (log);
		}

		/// <summary>
		/// Checks the action order.
		/// </summary>
		/// <returns><c>true</c>, if action1 was done before action2, <c>false</c> otherwise.</returns>
		/// <param name="action1">Action1.</param>
		/// <param name="action2">Action2.</param>
		public bool CheckActionOrder (string action1, string action2)
		{
			//Creates the variable to be returned.
			bool checkValue = false;
			//Searchs into the log list until the action1 will be found before the action2.
			foreach (LogEntry log in _logs) {
				//Checks if the log action key is equal to action2.
				if (log.Error == null) {
					//If they are equal, then the action1 was done after the action2.
					if (log.Action.Key == action2) {
						checkValue = false;
						break;
					} else if (log.Action.Key == action1) {
						checkValue = true;
						break;
					}
				}
			}

			//Returns the check value.
			return checkValue;
		}

		/// <summary>
		/// Checks if the action has been done.
		/// </summary>
		/// <returns><c>true</c>, if action done was checked, <c>false</c> otherwise.</returns>
		/// <param name="actionKey">Action key.</param>
		public bool CheckActionDone (string actionKey)
		{
			//Creates the variable to be returned.
			bool checkValue = false;
			//Searchs into the log list until there is a action log with the given action key.
			foreach (LogEntry log in _logs) {
				//Checks if the log has the action key given.
				if (log.Action.Key == actionKey) {
					checkValue = true;
					break;
				}
			}

			//Returns the check value.
			return checkValue;
		}

		/// <summary>
		/// Resets the log.
		/// </summary>
		public void ResetLog ()
		{
			//Clears the dictionary.
			_logs.Clear ();
			//Sets current phase to initial phase.
			_currentPhase = 0;
		}

		/// <summary>
		/// Erases the action.
		/// </summary>
		/// <param name="actionKey">Action key.</param>
		public void EraseAction (string actionKey)
		{
			//Sets the action key to erase.
			_actionKeyToErase = actionKey;
			//Finds the last index.
			int index = _logs.FindLastIndex (FindAction);
			//Calculates the number of elements to erase.
			int count = _logs.Count - index;
			//Removes the elements.
			_logs.RemoveRange(index, count);
		}

		//Delegate function to find the last action with the given key.
		private static bool FindAction (LogEntry log)
		{
			if (log.Action.Key == _actionKeyToErase)
				return true;
			else
				return false;
		}
	}
}

