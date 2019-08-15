using System;
using System.Collections.Generic;
using Its.ExpertModule.ObjectModel;
using Its.StudentModule.ObjectModel;

namespace Its.TutoringModule.CMTutor.SBP.OM
{
	public class StudentsClusterByErrorTime:StudentsCluster
	{
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

		public StudentsClusterByErrorTime (int number, double minTime, double maxTime, double avgTime, int minErrors, int maxErrors, double avgErrors):base(number)
		{
			this._minTime = minTime;
			this._maxTime = maxTime;
			this._avgTime = avgTime;
			this._minErrors = minErrors;
			this._maxErrors = maxErrors;
			this._avgErrors = avgErrors;
		}

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="StudentsCluster"/> class.
		/// </summary>
		/// <param name="number">Number.</param>
		/// <param name="minTime">Minimum time.</param>
		/// <param name="maxTime">Max time.</param>
		/// <param name="minErrors">Minimum errors.</param>
		/// <param name="maxErrors">Max errors.</param>
		/// <param name="studentKeys">Student keys.</param>
		/// <param name="logs">Logs.</param>
		public StudentsClusterByErrorTime (int number, double minTime, double maxTime, double avgTime, int minErrors, int maxErrors, double avgErrors, List<string> studentKeys, DomainLog logs, Dictionary<string,ActionAplication> incompatibilities, bool includeNoPlanActions):base(number,studentKeys,logs,incompatibilities,includeNoPlanActions)
		{
			this._minTime = minTime;
			this._maxTime = maxTime;
			this._avgTime = avgTime;
			this._minErrors = minErrors;
			this._maxErrors = maxErrors;
			this._avgErrors = avgErrors;
		}

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="StudentsCluster"/> class from a date (for visualization purposes)..
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
		public StudentsClusterByErrorTime (int number, double minTime, double maxTime, double avgTime, int minErrors, int maxErrors, double avgErrors, List<string> studentKeys, DomainLog logs, DateTime fromDate, Dictionary<string,ActionAplication> incompatibilities, bool includeNoPlanActions):base(number,studentKeys,logs,fromDate,incompatibilities,includeNoPlanActions)
		{
			this._minTime = minTime;
			this._maxTime = maxTime;
			this._avgTime = avgTime;
			this._minErrors = minErrors;
			this._maxErrors = maxErrors;
			this._avgErrors = avgErrors;
		}

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="StudentsCluster"/> class craetes a cluster only with the automata (for validation purposes)..
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
		public StudentsClusterByErrorTime (List<StudentLog> logs, Dictionary<string,ActionAplication> incompatibilities, bool includeNoPlanActions):base(logs,incompatibilities,includeNoPlanActions)
		{
			this._minTime = 0;
			this._maxTime = 0;
			this._avgTime = 0;
			this._minErrors = 0;
			this._maxErrors = 0;
			this._avgErrors = 0;
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
	}
}

