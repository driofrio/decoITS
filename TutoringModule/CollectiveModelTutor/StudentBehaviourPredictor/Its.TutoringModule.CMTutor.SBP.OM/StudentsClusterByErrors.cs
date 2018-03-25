using System;
using System.Collections.Generic;
using Its.ExpertModule.ObjectModel;
using Its.StudentModule.ObjectModel;

namespace Its.TutoringModule.CMTutor.SBP.OM
{
	public class StudentsClusterByErrors:StudentsCluster
	{
		private int _minErrors;
		private int _maxErrors;
		private double _avgErrors;

		public StudentsClusterByErrors (int number, int minErrors, int maxErrors, double avgErrors):base(number)
		{
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
		public StudentsClusterByErrors (int number, int minErrors, int maxErrors, double avgErrors, List<string> studentKeys, DomainLog logs, Dictionary<string,ActionAplication> incompatibilities, bool includeNoPlanActions):base(number,studentKeys,logs,incompatibilities,includeNoPlanActions)
		{
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
		public StudentsClusterByErrors (int number, int minErrors, int maxErrors, double avgErrors, List<string> studentKeys, DomainLog logs, DateTime fromDate, Dictionary<string,ActionAplication> incompatibilities, bool includeNoPlanActions):base(number,studentKeys,logs,fromDate,incompatibilities,includeNoPlanActions)
		{
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
		public StudentsClusterByErrors (List<StudentLog> logs, Dictionary<string,ActionAplication> incompatibilities, bool includeNoPlanActions):base(logs,incompatibilities,includeNoPlanActions)
		{
			this._minErrors = 0;
			this._maxErrors = 0;
			this._avgErrors = 0;
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

