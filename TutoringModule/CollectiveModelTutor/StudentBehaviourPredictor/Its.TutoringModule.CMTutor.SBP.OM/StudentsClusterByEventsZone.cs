using System;
using System.Collections.Generic;
using Its.ExpertModule.ObjectModel;
using Its.StudentModule.ObjectModel;

namespace Its.TutoringModule.CMTutor.SBP.OM
{
	public class StudentsClusterByEventsZone:StudentsCluster
	{
		private int _minCorrectEvents;
		private int _maxCorrectEvents;
		private double _avgCorrectEvents;

		private int _minIrrelevantErrorEvents;
		private int _maxIrrelevantErrorEvents;
		private double _avgIrrelevantErrorEvents;

		private int _minRelevantErrorEvents;
		private int _maxRelevantErrorEvents;
		private double _avgRelevantErrorEvents;

		public StudentsClusterByEventsZone (int number, int minCorrectEvents, int maxCorrectEvents, double avgCorrectEvents,
			int minIrrelevantErrorEvents, int maxIrrelevantErrorEvents, double avgIrrelevantErrorEvents, 
			int minRelevantErrorEvents, int maxRelevantErrorEvents, double avgRelevantErrorEvents):base(number)
		{
			_minCorrectEvents=minCorrectEvents;
			_maxCorrectEvents=maxCorrectEvents;
			_avgCorrectEvents = avgCorrectEvents;
			_minIrrelevantErrorEvents=minIrrelevantErrorEvents;
			_maxIrrelevantErrorEvents=maxIrrelevantErrorEvents;
			_avgIrrelevantErrorEvents = avgIrrelevantErrorEvents;
			_minRelevantErrorEvents=minRelevantErrorEvents;
			_maxRelevantErrorEvents = maxRelevantErrorEvents;
			_avgRelevantErrorEvents = avgRelevantErrorEvents;
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
		public StudentsClusterByEventsZone (int number, int minCorrectEvents, int maxCorrectEvents, double avgCorrectEvents,
			int minIrrelevantErrorEvents, int maxIrrelevantErrorEvents, double avgIrrelevantErrorEvents, 
			int minRelevantErrorEvents, int maxRelevantErrorEvents, double avgRelevantErrorEvents, 
			List<string> studentKeys, DomainLog logs, Dictionary<string,ActionAplication> incompatibilities, bool includeNoPlanActions):base(number,studentKeys,logs,incompatibilities,includeNoPlanActions)
		{
			_minCorrectEvents=minCorrectEvents;
			_maxCorrectEvents=maxCorrectEvents;
			_minIrrelevantErrorEvents=minIrrelevantErrorEvents;
			_maxIrrelevantErrorEvents=maxIrrelevantErrorEvents;
			_minRelevantErrorEvents=minRelevantErrorEvents;
			_maxRelevantErrorEvents = maxRelevantErrorEvents;
			_avgCorrectEvents = avgCorrectEvents;
			_avgIrrelevantErrorEvents = avgIrrelevantErrorEvents;
			_avgRelevantErrorEvents = avgRelevantErrorEvents;
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
		public StudentsClusterByEventsZone (int number, int minCorrectEvents, int maxCorrectEvents, double avgCorrectEvents,
			int minIrrelevantErrorEvents, int maxIrrelevantErrorEvents, double avgIrrelevantErrorEvents, 
			int minRelevantErrorEvents, int maxRelevantErrorEvents, double avgRelevantErrorEvents,
			List<string> studentKeys, DomainLog logs, DateTime fromDate, Dictionary<string,ActionAplication> incompatibilities, bool includeNoPlanActions):base(number,studentKeys,logs,fromDate,incompatibilities,includeNoPlanActions)
		{
			_minCorrectEvents=minCorrectEvents;
			_maxCorrectEvents=maxCorrectEvents;
			_minIrrelevantErrorEvents=minIrrelevantErrorEvents;
			_maxIrrelevantErrorEvents=maxIrrelevantErrorEvents;
			_minRelevantErrorEvents=minRelevantErrorEvents;
			_maxRelevantErrorEvents = maxRelevantErrorEvents;
			_avgCorrectEvents = avgCorrectEvents;
			_avgIrrelevantErrorEvents = avgIrrelevantErrorEvents;
			_avgRelevantErrorEvents = avgRelevantErrorEvents;
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
		public StudentsClusterByEventsZone (List<StudentLog> logs, Dictionary<string,ActionAplication> incompatibilities, bool includeNoPlanActions):base(logs,incompatibilities,includeNoPlanActions)
		{
			_minCorrectEvents=0;
			_maxCorrectEvents=0;
			_minIrrelevantErrorEvents=0;
			_maxIrrelevantErrorEvents=0;
			_minRelevantErrorEvents=0;
			_maxRelevantErrorEvents = 0;
			_avgCorrectEvents = 0;
			_avgIrrelevantErrorEvents = 0;
			_avgRelevantErrorEvents = 0;
		}

		public double MinCorrectEvents{
			get{
				return _minCorrectEvents;
			}
		}

		public double MaxCorrectEvents{
			get{
				return _maxCorrectEvents;
			}
		}

		public double MinIrrelevantErrorEvents{
			get{
				return _minIrrelevantErrorEvents;
			}
		}

		public int MaxIrrelevantErrorEvents{
			get{
				return _maxIrrelevantErrorEvents;
			}
		}

		public int MinRelevantErrorEvents{
			get{
				return _minRelevantErrorEvents;
			}
		}

		public double MaxRelevantErrorEvents{
			get{
				return _maxRelevantErrorEvents;
			}
		}

		public double AvgCorrectEvents{
			get{
				return _avgCorrectEvents;
			}
		}

		public double AvgIrrelevantErrorEvents{
			get{
				return _avgIrrelevantErrorEvents;
			}
		}

		public double AvgRelevantErrorEvents{
			get{
				return _avgRelevantErrorEvents;
			}
		}
	}
}

