using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using Its.ExpertModule.ObjectModel;
using Its.StudentModule.ObjectModel;
using Its.TutoringModule.CMTutor.SBP.Exceptions;
using Its.TutoringModule.CMTutor.SBP.OM.State;
using Its.TutoringModule.ReactiveTutor.ObjectModel;
using Its.Utils.Config;
using Its.Utils.Math;
using Microsoft.AnalysisServices.AdomdClient;
using weka.clusterers;
using weka.core;
using weka.filters.unsupervised.attribute;
using Attribute = weka.core.Attribute;
using Environment = System.Environment;

namespace Its.TutoringModule.CMTutor.SBP.OM
{
	public enum ClusterMethod
	{
		ErrorsAndTime,
		Errors,
		EventsByZone,
		Sequences,
		NoClusters
	}

	/// <summary>
	/// Student behavior model.
	/// </summary>
	public class PredictiveStudentModel
	{
		/// <summary>
		/// The key.
		/// </summary>
		private string _key;
		/// <summary>
		/// The domain.
		/// </summary>
		private DomainActions _domain;
		/// <summary>
		/// The clusters.
		/// </summary>
		private Dictionary<int,StudentsCluster> _clusters;

		private Dictionary<string,ActionAplication> _incompatibilities;

		private StudentsCluster _defaultCluster;

		public string _clusteredData = "";
		
		/// <summary>
		/// ITutorConfig instance
		/// </summary>
		private ITutorConfig _config;

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="Its.TutoringModule.StudentBehaviorPredictor.ObjectModel.StudentBehaviorModel"/> class.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="domain">Domain.</param>
		public PredictiveStudentModel(DomainActions domain, ITutorConfig config){
			this._key = domain.Key;
			this._domain = domain;
			this._clusters = new Dictionary<int, StudentsCluster> ();
			this._incompatibilities = new Dictionary<string, ActionAplication> ();
			this._config = config;
			FillIncompatibilities ();
		}

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="Its.TutoringModule.StudentBehaviorPredictor.ObjectModel.StudentBehaviorModel"/> class.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="domainLog">Domain log.</param>
		public PredictiveStudentModel(DomainLog domainLog, ClusterMethod cluMet, bool includeNoPlanActions, bool inPhases, ITutorConfig config)
        {
			this._key = domainLog.Domain.Key;
			this._domain = domainLog.Domain;
			this._clusters = new Dictionary<int, StudentsCluster> ();
			this._incompatibilities = new Dictionary<string, ActionAplication> ();
	        this._config = config;
			FillIncompatibilities ();
			BuildModel (domainLog, cluMet, includeNoPlanActions, inPhases);
		}

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="Its.TutoringModule.StudentBehaviorPredictor.ObjectModel.PredictiveStudentModel"/> class from a date (for visualization purposes)..
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="domainLog">Domain log.</param>
		/// <param name="fromDate">From date.</param>
		public PredictiveStudentModel(DomainLog domainLog, DateTime fromDate, bool includeNoPlanActions, ITutorConfig config){
			this._key = domainLog.Domain.Key;
			this._domain = domainLog.Domain;
			this._clusters = new Dictionary<int, StudentsCluster> ();
			this._incompatibilities = new Dictionary<string, ActionAplication> ();
			this._config = config;
			FillIncompatibilities ();
			BuildModel (domainLog, fromDate, includeNoPlanActions);
		}

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="Its.TutoringModule.StudentBehaviorPredictor.ObjectModel.PredictiveStudentModel"/> class with out clustering (for validation purposes)..
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="logs">Logs.</param>
		/// <param name="domain">Domain.</param>
		public PredictiveStudentModel(List<StudentLog> logs, DomainActions domain, ClusterMethod cluMet, bool includeNoPlanActions, ITutorConfig config){
			this._key = domain.Key;
			this._domain = domain;
			this._clusters = new Dictionary<int, StudentsCluster> ();
			this._incompatibilities = new Dictionary<string, ActionAplication> ();
			this._config = config;
			FillIncompatibilities ();
			BuildModel (logs, cluMet, includeNoPlanActions);
		}

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
		/// Gets the domain.
		/// </summary>
		/// <value>The domain.</value>
		public DomainActions Domain {
			get {
				return _domain;
			}
		}

		/// <summary>
		/// Gets the number of clusters.
		/// </summary>
		/// <value>The number of clusters.</value>
		public int NumberOfClusters{
			get{ 
				return _clusters.Count;
			}
		}

		/// <summary>
		/// Gets the clusters.
		/// </summary>
		/// <value>The clusters.</value>
		public Dictionary<int,StudentsCluster> Clusters{
			get{ 
				return _clusters;
			}
		}

		/// <summary>
		/// Gets the default cluster.
		/// </summary>
		/// <value>The default cluster.</value>
		public StudentsCluster DefaultCluster{
			get{ 
				return _defaultCluster;
			}
		}

		private Dictionary<ClusterStudentDataRow, int> ClusterByErrorsTime (DomainLog domainLog, bool includeNoPlanActions, out List<int> numbersOfClusters)
		{
			Instances clusterStudents = GetDataForErrorTimeClustering (domainLog, includeNoPlanActions);
			//XMeans clusterer = new XMeans ();
			EM clusterer = new EM ();
			Remove remove = new Remove ();
			String[] options = new String[2];
			options [0] = "-R";
			// "range"
			options [1] = "1";
			// to ignore the attribute that is in the position '1'
			remove.setOptions (options);
			// set options
			remove.setInputFormat (clusterStudents);
			FilteredClusterer fc = new FilteredClusterer ();
			fc.setFilter (remove);
			fc.setClusterer (clusterer);
			fc.buildClusterer (clusterStudents);
			Dictionary<ClusterStudentDataRow, int> studentByCluster = new Dictionary<ClusterStudentDataRow, int> ();
			int cluster = 0;
			numbersOfClusters = new List<int> ();
			for (int i = 0; i < clusterStudents.numInstances (); i++) {
				Instance instance = clusterStudents.instance (i);
				cluster = fc.clusterInstance (instance);
				ClusterStudentDataRow csdr = new ClusterStudentDataRow {
					Key = instance.stringValue (0),
					NumberErrors = Convert.ToInt32 (instance.value (1)),
					TotalTime = instance.value (2)
				};
				studentByCluster.Add (csdr, cluster);
				if(!numbersOfClusters.Contains(cluster))
					numbersOfClusters.Add (cluster);
				_clusteredData+=instance.stringValue (0) + ";" + instance.value(1).ToString(new CultureInfo ("en-US")) + ";" + instance.value(2).ToString(new CultureInfo ("en-US")) + ";" + cluster+"\n";
			}
			return studentByCluster;
		}

		private Dictionary<ClusterStudentDataRow, int> ClusterByErrors (DomainLog domainLog, bool includeNoPlanActions, out List<int> numbersOfClusters)
		{
			Instances clusterStudents = GetDataForErrorClustering (domainLog, includeNoPlanActions);
			//XMeans clusterer = new XMeans ();
			EM clusterer = new EM ();
			Remove remove = new Remove ();
			String[] options = new String[2];
			options [0] = "-R";
			// "range"
			options [1] = "1";
			// to ignore the attribute that is in the position '1'
			remove.setOptions (options);
			// set options
			remove.setInputFormat (clusterStudents);
			FilteredClusterer fc = new FilteredClusterer ();
			fc.setFilter (remove);
			fc.setClusterer (clusterer);
			fc.buildClusterer (clusterStudents);
			Dictionary<ClusterStudentDataRow, int> studentByCluster = new Dictionary<ClusterStudentDataRow, int> ();
			int cluster = 0;
			numbersOfClusters = new List<int> ();
			for (int i = 0; i < clusterStudents.numInstances (); i++) {
				Instance instance = clusterStudents.instance (i);
				cluster = fc.clusterInstance (instance);
				ClusterStudentDataRow csdr = new ClusterStudentDataRow {
					Key = instance.stringValue (0),
					NumberErrors = Convert.ToInt32 (instance.value (1))
				};
				studentByCluster.Add (csdr, cluster);
				if(!numbersOfClusters.Contains(cluster))
					numbersOfClusters.Add (cluster);
				_clusteredData+=instance.stringValue (0) + ";" + instance.value(1).ToString(new CultureInfo ("en-US")) + ";" + cluster+"\n";
			}
			return studentByCluster;
		}

		private Dictionary<ClusterStudentDataRow, int> ClusterByEventsZone (DomainLog domainLog, bool includeNoPlanActions, out List<int> numbersOfClusters)
		{
			Instances clusterStudents = GetDataForEventsZoneClustering (domainLog, includeNoPlanActions);
			XMeans clusterer = new XMeans ();
			//EM clusterer = new EM ();
			Remove remove = new Remove ();
			String[] options = new String[2];
			options [0] = "-R";
			// "range"
			options [1] = "1";
			// to ignore the attribute that is in the position '1'
			remove.setOptions (options);
			// set options
			remove.setInputFormat (clusterStudents);
			FilteredClusterer fc = new FilteredClusterer ();
			fc.setFilter (remove);
			fc.setClusterer (clusterer);
			fc.buildClusterer (clusterStudents);
			Dictionary<ClusterStudentDataRow, int> studentByCluster = new Dictionary<ClusterStudentDataRow, int> ();
			int cluster = 0;
			numbersOfClusters = new List<int> ();
			for (int i = 0; i < clusterStudents.numInstances (); i++) {
				Instance instance = clusterStudents.instance (i);
				cluster = fc.clusterInstance (instance);
				ClusterStudentDataRow csdr = null;
				csdr = new ClusterStudentDataRow {
						Key = instance.stringValue (0),
						CorrectEvents = Convert.ToInt32 (instance.value (1)),
						IrrelevantErrorEvents = Convert.ToInt32 (instance.value (2)),
						RelevantErrorEvents = Convert.ToInt32 (instance.value (3))
					};
				studentByCluster.Add (csdr, cluster);
				if(!numbersOfClusters.Contains(cluster))
					numbersOfClusters.Add (cluster);
				_clusteredData += instance.stringValue (0) + ";" + instance.value (1).ToString (new CultureInfo ("en-US")) + ";" +
				instance.value (2).ToString (new CultureInfo ("en-US")) + ";" +
				(instance.value (3).ToString (new CultureInfo ("en-US")) + ";") + cluster + "\n";
			}
			return studentByCluster;
		}

		private Dictionary<ClusterStudentDataRow, int> ClusterBySequences (DomainLog domainLog, bool includeNoPlanActions, bool inPhases, out List<int> numbersOfClusters)
		{
			SaveSequencesBDD (domainLog, includeNoPlanActions, inPhases);
            AdomdConnection con = new AdomdConnection(_config.ASSConString);
            con.Open();
            AdomdCommand command = new AdomdCommand();
            AdomdDataAdapter da = new AdomdDataAdapter();
            if (!inPhases)
            {
                command = new AdomdCommand("INSERT INTO MINING MODEL [EntireLab]", con);
                command.ExecuteNonQuery();
                da = new AdomdDataAdapter("select NODE_NAME from [EntireLab].Content where NODE_TYPE=5", con);
            }
            else
            {
                string miningStruct = "Phase" + domainLog.Domain.Key.Split('_')[0];
                command = new AdomdCommand("INSERT INTO MINING MODEL ["+ miningStruct +"]", con);
                command.ExecuteNonQuery();
                da = new AdomdDataAdapter("select NODE_NAME from [" + miningStruct + "].Content where NODE_TYPE=5", con);
            }
			DataTable dtClusters = new DataTable ();
			da.Fill(dtClusters);
            con.Close();
            numbersOfClusters = new List<int> ();
			foreach (DataRow row in dtClusters.Rows)
			{
				numbersOfClusters.Add(int.Parse(row[0].ToString()));
			}
			Dictionary<ClusterStudentDataRow, int> studentByCluster = new Dictionary<ClusterStudentDataRow, int> ();
			List<Student> tmpStudents = domainLog.GetStudents ();
			foreach (DataRow dr in dtClusters.Rows) {
                con.Open();
                if (!inPhases)
                {
                    da = new AdomdDataAdapter("select [id] from [EntireLab].CASES where IsInNode('" + dr[0].ToString() + "')", con);
                }
                else {
                    string miningStruct = "Phase" + domainLog.Domain.Key.Split('_')[0];
                    da = new AdomdDataAdapter("select [id] from ["+miningStruct+"].CASES where IsInNode('" + dr[0].ToString() + "')", con);
                }
				DataTable dtStudentIds = new DataTable ();
				da.Fill(dtStudentIds);
                con.Close();
                foreach (DataRow drStu in dtStudentIds.Rows) {
					if (tmpStudents.Any (x => x.Key == drStu [0].ToString ())) {
						ClusterStudentDataRow csdr = new ClusterStudentDataRow {
							Key = drStu [0].ToString ()
						};
						studentByCluster.Add (csdr, int.Parse (dr [0].ToString ()));
					}
				}
			}
			return studentByCluster;
		}

		private void SaveSequencesBDD(DomainLog domainLog, bool includeNoPlanActions, bool inPhases)
        {
			string sql = "";
			string sql2 = "";
			string table1 = "Logs";
			string table2 = "Students";
            
            List<Student> students = domainLog.GetStudents ();
			foreach (Student stu in students)
			{
				StudentLog stuLog = domainLog.GetStudentLog (stu.Key);
				int seq = 1;
                if (inPhases)
                    table1 = "LogsPhase0";
                foreach (LogEntry log in stuLog.Logs) {
					if (log.GetType ().BaseType != typeof(NoPlanAllowedActionLog) || includeNoPlanActions) {
						string action = "";
						if (log.GetType ().BaseType == typeof(ActionLog)) {
							action = log.Action.Key;
						} else if (log.GetType () == typeof(DepErrorLog)) {
							Dependence fail = ((DepErrorLog)log).FailedDependence;
							action = log.Action.Key + "_" + fail.Key;
						} else if (log.GetType () == typeof(IncompErrorLog)) {
							Incompatibility fail = ((IncompErrorLog)log).FailedIncompatibility;
							action = log.Action.Key + "_" + fail.Key;
						} else if (log.GetType ().BaseType == typeof(TimeErrorLog)) {
							Error fail = (log.GetType () == typeof(MinTimeErrorLog)) ? log.Action.MinTimeError : log.Action.MaxTimeError;
							action = log.Action.Key + "_" + fail.Key;
						} else if (log.GetType () == typeof(WorldErrorLog)) {
							Error fail = ((WorldErrorLog)log).ErrorAssociated;
							action = log.Action.Key + "_" + fail.Key;
						} else if (log.GetType () == typeof(OtherErrorLog)) {
							Error fail = ((OtherErrorLog)log).ErrorAssociated;
							action = log.Action.Key + "_" + fail.Key;
						}

                        if (inPhases)
                        {
                            table1 = "LogsPhase" + domainLog.Key.Split('_')[0];
                        }

                        sql = sql + "insert into " + table1 + " (studentid, domain, actionkey, sequence, date) values('"
							+ stu.Key + "','"
							+ domainLog.Domain.Key + "','"
							+ action + "',"
							+ seq + ",'"
							+ log.DateLog.ToString ("yyyy-MM-dd HH:mm:ss") + "') ";
						seq++;
					}
				}

				sql2 = sql2 + "insert into "+table2+" (id) values('"
					+ stu.Key +"') ";
			}
			using (SqlConnection con = new SqlConnection (_config.BDDConString)) {
				con.Open ();
                SqlCommand cmd = new SqlCommand("delete from Logs; delete from LogsPhase0; delete from LogsPhase1; delete from LogsPhase2; delete from LogsPhase3; delete from Students;", con);
                cmd.ExecuteNonQuery ();

				cmd = new SqlCommand (sql, con);
				cmd.ExecuteNonQuery ();
				cmd.CommandText = sql2;
				cmd.ExecuteNonQuery ();
				con.Close ();
			}
		}

		private Dictionary<ClusterStudentDataRow, int> NoCLusters (DomainLog domainLog, out List<int> numbersOfClusters)
		{
			List<Student> students = domainLog.GetStudents ();
			numbersOfClusters = new List<int> ();
			Dictionary<ClusterStudentDataRow, int> studentByCluster = new Dictionary<ClusterStudentDataRow, int> ();
			foreach (Student std in students) {
				ClusterStudentDataRow csdr = new ClusterStudentDataRow {
					Key = std.Key
				};
				studentByCluster.Add (csdr, 0);
			}
			numbersOfClusters.Add (0);

			return studentByCluster;
		}

		private void FillIncompatibilities(){
			foreach (ActionAplication act in this._domain.Actions) {
				if (act.Incompatibilities !=null && act.Incompatibilities.Count > 0) {
					foreach (Incompatibility inc in act.Incompatibilities) {
						if (!this._incompatibilities.ContainsKey (inc.IncompatibilityAction.Key)) {
							this._incompatibilities.Add (inc.IncompatibilityAction.Key, inc.IncompatibilityAction);
						}
					}
				}
			}
		}

		/// <summary>
		/// Builds the model.
		/// </summary>
		/// <param name="domainLog">Domain log.</param>
		private void BuildModel(DomainLog domainLog, ClusterMethod cluMet, bool includeNoPlanActions, bool inPhases)
        {
			List<int> numbersOfClusters = new List<int> ();
			Dictionary<ClusterStudentDataRow, int> studentByCluster = new Dictionary<ClusterStudentDataRow, int> ();
			switch (cluMet) {
			case ClusterMethod.ErrorsAndTime:
				studentByCluster = ClusterByErrorsTime (domainLog, includeNoPlanActions, out numbersOfClusters);
				break;
			case ClusterMethod.Sequences:
				studentByCluster = ClusterBySequences (domainLog, includeNoPlanActions, inPhases, out numbersOfClusters);
				break;
			case ClusterMethod.Errors:
				studentByCluster = ClusterByErrors (domainLog, includeNoPlanActions, out numbersOfClusters);
				break;
			case ClusterMethod.NoClusters:
				studentByCluster = NoCLusters (domainLog, out numbersOfClusters);
				break;
			case ClusterMethod.EventsByZone:
				studentByCluster = ClusterByEventsZone (domainLog, includeNoPlanActions, out numbersOfClusters);
				break;
			}

			foreach (int cluNumber in numbersOfClusters) {
				List<ClusterStudentDataRow> clStudentRows = studentByCluster.Where (p => p.Value == cluNumber).Select (p => p.Key).ToList ();
				if (clStudentRows.Count > 0) {
					StudentsCluster sc = null;
					List<String> studentKeys = clStudentRows.Select (p => p.Key).Distinct ().ToList ();
					switch (cluMet) {
					case ClusterMethod.ErrorsAndTime:
						int minErrors = clStudentRows.Min (p => p.NumberErrors);
						int maxErrors = clStudentRows.Max (p => p.NumberErrors);
						double avgErrors = clStudentRows.Average (p => p.NumberErrors);
						double minTime = clStudentRows.Min (p => p.TotalTime);
						double maxTime = clStudentRows.Max (p => p.TotalTime);
						double avgTime = clStudentRows.Average (p => p.TotalTime);
						sc = new StudentsClusterByErrorTime (cluNumber, minTime, maxTime, avgTime, minErrors, maxErrors, avgErrors, studentKeys, domainLog, _incompatibilities, includeNoPlanActions);
						break;
					case ClusterMethod.Sequences:
						sc = new StudentsCluster (cluNumber, studentKeys, domainLog, _incompatibilities, includeNoPlanActions);
						if (cluNumber == numbersOfClusters.First () || studentKeys.Count > _defaultCluster.NumberOfStudents)
							_defaultCluster = sc;
						break;
					case ClusterMethod.Errors:
						minErrors = clStudentRows.Min (p => p.NumberErrors);
						maxErrors = clStudentRows.Max (p => p.NumberErrors);
						avgErrors = clStudentRows.Average (p => p.NumberErrors);
						sc = new StudentsClusterByErrors (cluNumber, minErrors, maxErrors, avgErrors, studentKeys, domainLog, _incompatibilities, includeNoPlanActions);
						break;
					case ClusterMethod.NoClusters:
						sc = new StudentsCluster (cluNumber, studentKeys, domainLog, _incompatibilities, includeNoPlanActions);
						_defaultCluster = sc;
						break;
					case ClusterMethod.EventsByZone:
						int minCorrectEvents = clStudentRows.Min (p => p.CorrectEvents);
						int maxCorrectEvents = clStudentRows.Max (p => p.CorrectEvents);
						double avgCorrect = clStudentRows.Average (p => p.CorrectEvents);
						int minIrrelevantErrorEvents = clStudentRows.Min (p => p.IrrelevantErrorEvents);
						int maxIrrelevantErrorEvents = clStudentRows.Max (p => p.IrrelevantErrorEvents);
						double avgIrrelevant = clStudentRows.Average (p => p.IrrelevantErrorEvents);
						int minRelevantErrorEvents = clStudentRows.Min (p => p.RelevantErrorEvents);
						int maxRelevantErrorEvents = clStudentRows.Max (p => p.RelevantErrorEvents);
						double avgRelevant = clStudentRows.Average (p => p.RelevantErrorEvents);
						sc = new StudentsClusterByEventsZone (cluNumber, minCorrectEvents, maxCorrectEvents, avgCorrect, 
							minIrrelevantErrorEvents, maxIrrelevantErrorEvents, avgIrrelevant, 
							minRelevantErrorEvents, maxRelevantErrorEvents, avgRelevant, studentKeys, domainLog, _incompatibilities, includeNoPlanActions);
						break;
					}
					_clusters.Add (cluNumber, sc);
				}
			}

			switch (cluMet) {
			case ClusterMethod.ErrorsAndTime:
				double avgTotalErrors = _clusters.Values.Cast<StudentsClusterByErrorTime> ().Average (p => p.AvgErrors);
				double avgTotalTime = _clusters.Values.Cast<StudentsClusterByErrorTime> ().Average (p => p.AvgTime);

				double avgErrorsDif = 0;
				double avgTimeDif = 0;

				//Default cluster is calculated by the nearest centroid to the average
				for (int index = 0; index < _clusters.Count; index++) {
					StudentsClusterByErrorTime sc = (StudentsClusterByErrorTime)_clusters.ElementAt (index).Value;
					if (index == 0) {
						_defaultCluster = sc;
						avgErrorsDif = Math.Abs (avgTotalErrors - sc.AvgErrors);
						avgTimeDif = Math.Abs (avgTotalTime - sc.AvgTime);
					} else {
						double tmpavgErrorsDif = Math.Abs (avgTotalErrors - sc.AvgErrors);
						double tmpavgTimeDif = Math.Abs (avgTotalTime - sc.AvgTime);
						if (tmpavgErrorsDif < avgErrorsDif && tmpavgTimeDif < avgTimeDif) {
							_defaultCluster = sc;
							avgErrorsDif = Math.Abs (avgTotalErrors - sc.AvgErrors);
							avgTimeDif = Math.Abs (avgTotalTime - sc.AvgTime);
						}
					}
				}
				break;
			case ClusterMethod.Errors:
				avgTotalErrors = _clusters.Values.Cast<StudentsClusterByErrors> ().Average (p => p.AvgErrors);
				avgErrorsDif = 0;

				//Default cluster is calculated by the nearest centroid to the average
				for (int index = 0; index < _clusters.Count; index++) {
					StudentsClusterByErrors sc = (StudentsClusterByErrors)_clusters.ElementAt (index).Value;
					if (index == 0) {
						_defaultCluster = sc;
						avgErrorsDif = Math.Abs (avgTotalErrors - sc.AvgErrors);
					} else {
						double tmpavgErrorsDif = Math.Abs (avgTotalErrors - sc.AvgErrors);
						if (tmpavgErrorsDif < avgErrorsDif) {
							_defaultCluster = sc;
							avgErrorsDif = Math.Abs (avgTotalErrors - sc.AvgErrors);
						}
					}
				}
				break;
			case ClusterMethod.EventsByZone:
				double avgCorrectEvents = _clusters.Values.Cast<StudentsClusterByEventsZone> ().Average (p => p.AvgCorrectEvents);
				double avgIrrelevantEvents = _clusters.Values.Cast<StudentsClusterByEventsZone> ().Average (p => p.AvgIrrelevantErrorEvents);
				double avgRelevantEvents = _clusters.Values.Cast<StudentsClusterByEventsZone> ().Average (p => p.AvgRelevantErrorEvents);

				double avgCorrectEventsDif = 0;
				double avgIrrelevantEventsDif = 0;
				double avgRelevantEventsDif = 0;

				//Default cluster is calculated by the nearest centroid to the average
				for (int index = 0; index < _clusters.Count; index++) {
					StudentsClusterByEventsZone sc = (StudentsClusterByEventsZone)_clusters.ElementAt (index).Value;
					if (index == 0) {
						_defaultCluster = sc;
						avgCorrectEventsDif = Math.Abs (avgCorrectEvents - sc.AvgCorrectEvents);
						avgIrrelevantEventsDif = Math.Abs (avgIrrelevantEvents - sc.AvgIrrelevantErrorEvents);
						avgRelevantEventsDif = Math.Abs (avgRelevantEvents - sc.AvgRelevantErrorEvents);
					} else {
						double tmpavgCorrectEventsDif = Math.Abs (avgCorrectEvents - sc.AvgCorrectEvents);
						double tmpavgIrrelevantEventsDif = Math.Abs (avgIrrelevantEvents - sc.AvgIrrelevantErrorEvents);
						double tmpavgRelevantEventsDif = Math.Abs (avgRelevantEvents - sc.AvgRelevantErrorEvents);
						if (tmpavgCorrectEventsDif < avgCorrectEventsDif && 
							tmpavgIrrelevantEventsDif < avgIrrelevantEventsDif  && 
							tmpavgRelevantEventsDif < avgRelevantEventsDif) {
							_defaultCluster = sc;
							avgCorrectEventsDif = Math.Abs (avgCorrectEvents - sc.AvgCorrectEvents);
							avgIrrelevantEventsDif = Math.Abs (avgIrrelevantEvents - sc.AvgIrrelevantErrorEvents);
							avgRelevantEventsDif = Math.Abs (avgRelevantEvents - sc.AvgRelevantErrorEvents);
						}
					}
				}
				break;
			}
		}

		/// <summary>
		/// Builds the model with out clustering (for validation purposes).
		/// </summary>
		/// <param name="domainLog">Domain log.</param>
		private void BuildModel(List<StudentLog> logs, ClusterMethod cluMet, bool includeNoPlanActions){
			StudentsCluster sc = null;
			switch(cluMet){
			case ClusterMethod.ErrorsAndTime:
				sc = new StudentsCluster (logs, this._incompatibilities, includeNoPlanActions);
				break;
			}
			_clusters.Add (0, sc);
			_defaultCluster = _clusters [0];
		}

		/// <summary>
		/// Builds the model from a date (for visualization purposes).
		/// </summary>
		/// <param name="domainLog">Domain log.</param>
		private void BuildModel(DomainLog domainLog, DateTime fromDate, bool includeNoPlanActions){
			Instances clusterStudents = GetDataForErrorTimeClustering (domainLog, fromDate, includeNoPlanActions);
			//XMeans clusterer = new XMeans ();
			EM clusterer = new EM();
			Remove remove = new Remove();
			String[] options = new String[2];
			options[0] = "-R"; // "range"
			options[1] = "1"; // to ignore the attribute that is in the position '1'
			remove.setOptions(options); // set options
			remove.setInputFormat(clusterStudents);
			FilteredClusterer fc = new FilteredClusterer();
			fc.setFilter(remove);
			fc.setClusterer(clusterer);
			fc.buildClusterer(clusterStudents);

			Dictionary<ClusterStudentDataRow,int> studentByCluster=new Dictionary<ClusterStudentDataRow, int>();
			int cluster = 0;

			for (int i = 0; i < clusterStudents.numInstances(); i++) {
				Instance instance = clusterStudents.instance (i);
				cluster = fc.clusterInstance (instance);
				ClusterStudentDataRow csdr = new ClusterStudentDataRow {
					Key = instance.stringValue (0),
					NumberErrors = Convert.ToInt32 (instance.value (1)),
					TotalTime = instance.value (2)
				};
				studentByCluster.Add (csdr, cluster);
				//Console.WriteLine (instance.stringValue (0) + ";" + instance.value(1).ToString() + ";" + instance.value(2).ToString() + ";" + cluster);
			}

			int numberOfClusters = clusterer.numberOfClusters ();
			//Console.WriteLine (numberOfClusters.ToString ());
			for (int i = 0; i < numberOfClusters; i++) {
				List<ClusterStudentDataRow> clStudentRows = studentByCluster.Where (p => p.Value == i).Select(p => p.Key).ToList();
				if (clStudentRows.Count > 0) {
					int minErrors = clStudentRows.Min (p => p.NumberErrors);
					int maxErrors = clStudentRows.Max (p => p.NumberErrors);
					double avgErrors = clStudentRows.Average (p => p.NumberErrors);
					double minTime = clStudentRows.Min (p => p.TotalTime);
					double maxTime = clStudentRows.Max (p => p.TotalTime);
					double avgTime = clStudentRows.Average (p => p.TotalTime);
					List<String> studentKeys = clStudentRows.Select (p => p.Key).Distinct ().ToList ();
					StudentsClusterByErrorTime sc = new StudentsClusterByErrorTime (i, minTime, maxTime, avgTime, minErrors, maxErrors, avgErrors, studentKeys, domainLog, fromDate, this._incompatibilities, includeNoPlanActions);
					_clusters.Add (i, sc);
				}
			}

			double avgTotalErrors = _clusters.Values.Cast<StudentsClusterByErrorTime>().Average (p => p.AvgErrors);
			double avgTotalTime = _clusters.Values.Cast<StudentsClusterByErrorTime>().Average (p => p.AvgTime);

			double avgErrorsDif = 0;
			double avgTimeDif = 0;

			//Default cluster is claculated by the nearest centroid to the average
			for (int index = 0; index < _clusters.Count; index++) {
				StudentsClusterByErrorTime sc = (StudentsClusterByErrorTime)_clusters.ElementAt (index).Value;
				if (index == 0) {
					_defaultCluster = sc;
					avgErrorsDif = Math.Abs (avgTotalErrors - sc.AvgErrors);
					avgTimeDif = Math.Abs (avgTotalTime - sc.AvgTime);
				} else {
					double tmpavgErrorsDif = Math.Abs (avgTotalErrors - sc.AvgErrors);
					double tmpavgTimeDif = Math.Abs (avgTotalTime - sc.AvgTime);
					if (tmpavgErrorsDif < avgErrorsDif && tmpavgTimeDif < avgTimeDif) {
						_defaultCluster = sc;
						avgErrorsDif = Math.Abs (avgTotalErrors - sc.AvgErrors);
						avgTimeDif = Math.Abs (avgTotalTime - sc.AvgTime);
					}
				}
			}
		}

		/// <summary>
		/// Gets the data for clustering.
		/// </summary>
		/// <returns>The data for clustering.</returns>
		/// <param name="domainLog">Domain log.</param>
		private Instances GetDataForErrorTimeClustering(DomainLog domainLog, bool includeNoPlanActions){
			List<Student> students = domainLog.GetStudents ();

			ActionAplication lastAction = _domain.GetLastAction ();

			Attribute attKey = new Attribute ("Key", (FastVector) null);
			Attribute attNumberErrors = new Attribute ("NumberErrors");
			Attribute attTotalTime = new Attribute ("TotalTime");
			FastVector fvWekaAttributes = new FastVector(3);
			fvWekaAttributes.addElement(attKey);
			fvWekaAttributes.addElement(attNumberErrors);
			fvWekaAttributes.addElement(attTotalTime);

			Instances trainingSet = new Instances("ClusterStudentData", fvWekaAttributes, students.Count);
			foreach (Student student in students) {
				StudentLog sudentLogs = domainLog.GetStudentLog(student.Key);
				int numErrors = 0;
				double totalTime = 0;
				DateTime initTime = sudentLogs.Logs.Min(p=>p.DateLog);
				DateTime maxTime = sudentLogs.Logs.Max (p => p.DateLog);
				totalTime = (maxTime - initTime).TotalSeconds;
				foreach (LogEntry log in sudentLogs.Logs) {
					if (includeNoPlanActions || log.GetType ().BaseType != typeof(NoPlanAllowedActionLog)) {
						if (log.GetType ().BaseType == typeof(ErrorLog)) {
							if (log.Error.IsBlock) {
								if (log.GetType () == typeof(WorldErrorLog))
									numErrors++;
								else
									numErrors += _config.TutoringBlockErrorPenalization;
							} else {
								if (log.GetType () == typeof(WorldErrorLog))
									numErrors += _config.NoTutoringNoBlockErrorPenalization;
								else
									numErrors += _config.TutoringNoBlockErrorPenalization;
							}
						}
					}
				}
				if (sudentLogs.Logs [sudentLogs.Logs.Count - 1].Action != lastAction && totalTime == 0)
					totalTime = totalTime + (_domain.EstimatedTime * 60 * 60) * _config.TimePenalization;
				Instance instance = new Instance(3);
				instance.setValue((Attribute)fvWekaAttributes.elementAt(0), student.Key);
				instance.setValue((Attribute)fvWekaAttributes.elementAt(1), numErrors);
				instance.setValue((Attribute)fvWekaAttributes.elementAt(2), totalTime);
				trainingSet.add(instance);
			}
			return trainingSet;

		}

		private Instances GetDataForErrorClustering(DomainLog domainLog, bool includeNoPlanActions){
			List<Student> students = domainLog.GetStudents ();

			ActionAplication lastAction = _domain.GetLastAction ();

			Attribute attKey = new Attribute ("Key", (FastVector) null);
			Attribute attNumberErrors = new Attribute ("Number");
			FastVector fvWekaAttributes = new FastVector(2);
			fvWekaAttributes.addElement(attKey);
			fvWekaAttributes.addElement(attNumberErrors);

			Instances trainingSet = new Instances("ClusterStudentData", fvWekaAttributes, students.Count);
			foreach (Student student in students) {
				StudentLog sudentLogs = domainLog.GetStudentLog(student.Key);
				int numErrors = 0;
				foreach (LogEntry log in sudentLogs.Logs) {
					if (includeNoPlanActions || log.GetType ().BaseType != typeof(NoPlanAllowedActionLog)) {
						if (log.GetType ().BaseType == typeof(ErrorLog)) {
							if (log.Error.IsBlock) {
								if (log.GetType () == typeof(WorldErrorLog))
									numErrors++;
								else
									numErrors += _config.TutoringBlockErrorPenalization;
							} else {
								if (log.GetType () == typeof(WorldErrorLog))
									numErrors += _config.NoTutoringNoBlockErrorPenalization;
								else
									numErrors += _config.TutoringNoBlockErrorPenalization;
							}
						}
					}
				}
				Instance instance = new Instance(2);
				instance.setValue((Attribute)fvWekaAttributes.elementAt(0), student.Key);
				instance.setValue((Attribute)fvWekaAttributes.elementAt(1), numErrors);
				trainingSet.add(instance);
			}
			return trainingSet;

		}

		private Instances GetDataForEventsZoneClustering(DomainLog domainLog, bool includeNoPlanActions){
			List<Student> students = domainLog.GetStudents ();

			ActionAplication lastAction = _domain.GetLastAction ();

			Attribute attKey = new Attribute ("Key", (FastVector) null);
			Attribute attCorrectEvents = new Attribute ("CorrectEvents");
			Attribute attIrrelevantEvents = new Attribute ("IrrelevantEvents");
			Attribute attRelevantEvents = new Attribute ("RelevantEvents");
			FastVector fvWekaAttributes = new FastVector(4);
			fvWekaAttributes.addElement(attKey);
			fvWekaAttributes.addElement(attCorrectEvents);
			fvWekaAttributes.addElement (attIrrelevantEvents);
			fvWekaAttributes.addElement(attRelevantEvents);

			Instances trainingSet = new Instances("ClusterStudentData", fvWekaAttributes, students.Count);
			Node<State.State, Event.Event> initState = new Node<State.State, Event.Event> ("initState", "", new CorrectState (Area.CorrectFlow, null, false, 1, 1));
			foreach (Student student in students) {
				StudentLog sudentLogs = domainLog.GetStudentLog(student.Key);
				Node<State.State, Event.Event> previousState = initState;
				List<Node<State.State, Event.Event>> pastNodes = new List<Node<State.State, Event.Event>> ();
				int numCorrectEvents = 0;
				int numIrrelevantEvents = 0;
				int numRelevantEvents = 0;
				foreach (LogEntry log in sudentLogs.Logs) {
					if (includeNoPlanActions || log.GetType ().BaseType != typeof(NoPlanAllowedActionLog)) {
						Area tmpArea = GetArea (log, previousState, pastNodes, initState);
						Node<State.State, Event.Event> node = CreateState (log, tmpArea);
						switch (tmpArea) {
						case Area.CorrectFlow:
							numCorrectEvents++;
							break;
						case Area.IrrelevantErrors:
							numIrrelevantEvents++;
							break;
						case Area.RelevantErrors:
							numRelevantEvents++;
							break;
						}
						previousState = node;
						pastNodes.Add (node);
					}
				}
				Instance instance = new Instance(4);
				instance.setValue((Attribute)fvWekaAttributes.elementAt(0), student.Key);
				instance.setValue((Attribute)fvWekaAttributes.elementAt(1), numCorrectEvents);
				instance.setValue ((Attribute)fvWekaAttributes.elementAt (2), numIrrelevantEvents);
				instance.setValue((Attribute)fvWekaAttributes.elementAt(3), numRelevantEvents);
				trainingSet.add(instance);
			}
			return trainingSet;

		}

		private Instances GetDataForErrorTimeClustering(DomainLog domainLog, DateTime fromDate, bool includeNoPlanActions){
			List<Student> students = domainLog.GetStudents ();

			ActionAplication lastAction = _domain.GetLastAction ();

			Attribute attKey = new Attribute ("Key", (FastVector) null);
			Attribute attNumberErrors = new Attribute ("NumberErrors");
			Attribute attTotalTime = new Attribute ("TotalTime");
			FastVector fvWekaAttributes = new FastVector(3);
			fvWekaAttributes.addElement(attKey);
			fvWekaAttributes.addElement(attNumberErrors);
			fvWekaAttributes.addElement(attTotalTime);

			Instances trainingSet = new Instances("ClusterStudentData", fvWekaAttributes, students.Count);
			foreach (Student student in students) {
				StudentLog sudentLogs = domainLog.GetStudentLog(student.Key);
				int numErrors = 0;
				double totalTime = 0;
				DateTime initTime = sudentLogs.Logs.Min(p=>p.DateLog);
				DateTime maxTime = sudentLogs.Logs.Max (p => p.DateLog);
				if (initTime >= fromDate) {
					totalTime = (maxTime - initTime).TotalSeconds;
					foreach (LogEntry log in sudentLogs.Logs) {
						if (includeNoPlanActions || log.GetType ().BaseType != typeof(NoPlanAllowedActionLog)) {
							if (log.GetType ().BaseType == typeof(ErrorLog)) {
								if (log.Error.IsBlock) {
									if (log.GetType () == typeof(WorldErrorLog))
										numErrors++;
									else
										numErrors += _config.TutoringBlockErrorPenalization;
								} else {
									if (log.GetType () == typeof(WorldErrorLog))
										numErrors += _config.NoTutoringNoBlockErrorPenalization;
									else
										numErrors += _config.TutoringNoBlockErrorPenalization;
								}
							}
						}
					}
					if (sudentLogs.Logs [sudentLogs.Logs.Count - 1].Action != lastAction && totalTime == 0)
						totalTime = totalTime + (_domain.EstimatedTime * 60 * 60) * _config.TimePenalization;
					Instance instance = new Instance (3);
					instance.setValue ((Attribute)fvWekaAttributes.elementAt (0), student.Key);
					instance.setValue ((Attribute)fvWekaAttributes.elementAt (1), numErrors);
					instance.setValue ((Attribute)fvWekaAttributes.elementAt (2), totalTime);
					trainingSet.add (instance);
				}
			}
			return trainingSet;

		}

		private Area GetArea(LogEntry log, Node<State.State,Event.Event> previousState, List<Node<State.State,Event.Event>> pastNodes, Node<State.State,Event.Event> initState){
			Area area=previousState.Specification.Area;
			if (log.GetType ().BaseType == typeof(ActionLog)) {
				if (area == Area.CorrectFlow || area == Area.IrrelevantErrors) {
					if ((((ActionLog)log).Action.Dependence != null)) {
						if (IsDependeceRelevantError (((ActionLog)log).Action.Dependence.DependenciesList, pastNodes, initState) || HasPastNodeRelevantError(pastNodes))
							area = Area.RelevantErrors;
						else 
							area = Area.CorrectFlow;
					} else
						if (HasPastNodeRelevantError(pastNodes))
							area = Area.RelevantErrors;
						else 
							area = Area.CorrectFlow;
				} else {
					if (previousState.Specification.GetType() == typeof(CorrectState)) {
						if (((CorrectState)previousState.Specification).IsCorrective)
							area = Area.CorrectFlow;
						else
							area = Area.RelevantErrors;
					}else
						area = Area.RelevantErrors;
				}

			}else if (log.GetType ().BaseType == typeof(ErrorLog)) {
				if (log.Error.IsBlock)
					area = Area.IrrelevantErrors;
				else
					area = Area.RelevantErrors;
			}
			return area;
		}

		private static bool IsDependeceRelevantError (List<Dependence> dependences, List<Node<State.State,Event.Event>> pastNodes, Node<State.State,Event.Event> initState)
		{
			bool isRelevantError = false;
			List<Node<State.State,Event.Event>> tempPasNodes = pastNodes.Where (x => x.Specification.GetType () == typeof(CorrectState) && x != initState).ToList();
			foreach (Dependence dep in dependences) {
				if (dep.GetType () == typeof(SimpleDependence)) {
					if (tempPasNodes.Count (x => ((CorrectState)x.Specification).Action.Key == ((SimpleDependence)dep).ActionDependence.Key) > 0) {
						if (tempPasNodes.Single (y => ((CorrectState)y.Specification).Action.Key == ((SimpleDependence)dep).ActionDependence.Key).Specification.Area == Area.RelevantErrors) {
							isRelevantError = true;
							break;
						}
					}
				} else {
					isRelevantError = IsDependeceRelevantError (((ComplexDependence)dep).DependenciesList, pastNodes, initState);
					if (isRelevantError)
						break;
				}
			}
			return isRelevantError;
		}

		private bool HasPastNodeRelevantError (List<Node<State.State,Event.Event>> pastNodes)
		{
			bool isRelevantError = false;
			for (int i = pastNodes.Count - 1; i >= 0 ; i--) {
				if (pastNodes [i].Specification.Area == Area.RelevantErrors) {
					isRelevantError = true;
					break;
				}
			}
			return isRelevantError;
		}

		private Node<State.State,Event.Event> CreateState(LogEntry log, Area area){
			State.State state=null;
			Node<State.State,Event.Event> newState = null;

			if (log.GetType ().BaseType == typeof(ActionLog)) {
				state = new CorrectState (area, log.Action, log.GetType () == typeof(CorrectiveActionLog));
				newState = new Node<State.State,Event.Event> (log.Action.Key+"_"+area.ToString(), log.Action.Description, state);
			} else if (log.GetType () == typeof(DepErrorLog)) {
				Dependence fail = ((DepErrorLog)log).FailedDependence;
				state = new DependenceErrorState (area, fail);
				newState = new Node<State.State,Event.Event> (log.Action.Key + "_" + fail.Key+"_"+area.ToString(), log.Action.Description, state);
			} else if (log.GetType () == typeof(IncompErrorLog)) {
				Incompatibility fail = ((IncompErrorLog)log).FailedIncompatibility;
				state = new IncompatibilityErrorState (area, fail);
				newState = new Node<State.State,Event.Event> (log.Action.Key + "_" + fail.Key+"_"+area.ToString(), log.Action.Description, state);
			} else if (log.GetType ().BaseType == typeof(TimeErrorLog)) {
				Error fail = (log.GetType () == typeof(MinTimeErrorLog)) ? log.Action.MinTimeError : log.Action.MaxTimeError;
				state = new TimeErrorState (area, ((TimeErrorLog)log).Time, fail);
				newState = new Node<State.State,Event.Event> (log.Action.Key + "_" + fail.Key+"_"+area.ToString(), log.Action.Description, state);
			} else if (log.GetType () == typeof(WorldErrorLog)) {
				Error fail = ((WorldErrorLog)log).ErrorAssociated;
				state = new WorldErrorState (area, fail, ((WorldErrorLog)log).Type);
				newState = new Node<State.State,Event.Event> (log.Action.Key + "_" + fail.Key+"_"+area.ToString(), log.Action.Description, state);
			} else if (log.GetType () == typeof(OtherErrorLog)) {
				Error fail = ((OtherErrorLog)log).ErrorAssociated;
				state = new OtherErrorState (area, fail);
				newState = new Node<State.State,Event.Event> (log.Action.Key + "_" + fail.Key+"_"+area.ToString(), log.Action.Description, state);
			}

			return newState;
		}

		/// <summary>
		/// Updates the model.
		/// </summary>
		/// <param name="studentKey">Student key.</param>
		/// <param name="log">Log.</param>
		public void UpdateModel(string studentKey, LogEntry log){
			StudentsCluster cluster = FindStudentCluster (studentKey);
			cluster.UpdateAutomaton (studentKey, log, this._incompatibilities);
		}

		/// <summary>
		/// Updates the model reclustering the student.
		/// </summary>
		/// <param name="studentKey">Student key.</param>
		/// <param name="log">Log.</param>
		/// <param name="studentLogs">Student logs.</param>
		public void UpdateModelAndRecluster(string studentKey, LogEntry log, StudentLog studentLogs){
			StudentsCluster newCluster = ReclusterStudent (studentKey, studentLogs, true);
			newCluster.UpdateAutomaton (studentKey, log, this._incompatibilities);
		}

		/// <summary>
		/// Reclusters the student.
		/// </summary>
		/// <returns>The student.</returns>
		/// <param name="studentLogs">Student logs.</param>
		private StudentsCluster ReclusterStudent(string studentKey, StudentLog studentLogs, bool includeNoPlanActions){
			StudentsCluster oldCluster = FindStudentCluster (studentKey);
			List<Node<State.State,Event.Event>> studentNodes = oldCluster.GetStudentNodes (studentKey);
			StudentsCluster newCluster = GetNewCluster (studentNodes);

			if (newCluster != oldCluster) {
				oldCluster.RemoveStudent (studentKey, studentNodes, studentLogs.Logs.Count);
				newCluster.AddStudent (studentLogs, newCluster.StudentActionsModel.InitState, this._incompatibilities, includeNoPlanActions);
			}
			return newCluster;
		}

		public int MatchClusterForNewStudent(StudentLog studentLogs, ClusterMethod cluMeth, bool includeNoPlanActions, bool inPhases, string domainKey)
        {
			StudentsCluster cluMatch = null;
			switch (cluMeth) {
			case ClusterMethod.ErrorsAndTime:
				cluMatch = MatchClusterByErrorTime (studentLogs);
				break;
			case ClusterMethod.Errors:
				cluMatch = MatchClusterByErrors (studentLogs);
				break;
			case ClusterMethod.EventsByZone:
				cluMatch = MatchClusterByEventsZone (studentLogs, includeNoPlanActions);
				break;
			case ClusterMethod.Sequences:
				cluMatch = MatchClusterBySequences (studentLogs.Owner.Key, inPhases, domainKey);
				break;
			}

			if (cluMatch == null) {
				cluMatch = _defaultCluster;
			}

			//StudentsCluster newCluster = GetNewCluster (studentLogs);
			return cluMatch.Number;
		}

		private StudentsClusterByErrorTime MatchClusterByErrorTime(StudentLog studentLogs){
			StudentsClusterByErrorTime cluMatch = null;
			int numErrors = 0;
			double totalTime = 0;
			DateTime initTime = studentLogs.Logs.Min (p => p.DateLog);
			DateTime maxTime = studentLogs.Logs.Max (p => p.DateLog);
			totalTime = (maxTime - initTime).TotalSeconds;
			foreach (LogEntry log in studentLogs.Logs) {
				if (log.GetType ().BaseType == typeof(ErrorLog)) {
					if (log.Error.IsBlock) {
						if (log.GetType () == typeof(WorldErrorLog))
							numErrors++;
						else
							numErrors += _config.TutoringBlockErrorPenalization;
					} else {
						if (log.GetType () == typeof(WorldErrorLog))
							numErrors += _config.NoTutoringNoBlockErrorPenalization;
						else
							numErrors += _config.TutoringNoBlockErrorPenalization;
					}
				}
			}

			foreach (StudentsClusterByErrorTime clu in  _clusters.Values) {
				if (totalTime <= clu.MaxTime && totalTime >= clu.MinTime && numErrors <= clu.MaxErrors && numErrors >= clu.MinErrors) {
					if (cluMatch == null)
						cluMatch = clu;
					else {
						if (Math.Abs (cluMatch.AvgTime - totalTime) < Math.Abs (clu.AvgTime - totalTime) &&
							Math.Abs (cluMatch.AvgErrors - numErrors) < Math.Abs (clu.AvgErrors - numErrors))
							cluMatch = clu;
					}
				}
			}
			return cluMatch;
		}

		private StudentsClusterByErrors MatchClusterByErrors(StudentLog studentLogs){
			StudentsClusterByErrors cluMatch = null;
			int numErrors = 0;
			foreach (LogEntry log in studentLogs.Logs) {
				if (log.GetType ().BaseType == typeof(ErrorLog)) {
					if (log.Error.IsBlock) {
						if (log.GetType () == typeof(WorldErrorLog))
							numErrors++;
						else
							numErrors += _config.TutoringBlockErrorPenalization;
					} else {
						if (log.GetType () == typeof(WorldErrorLog))
							numErrors += _config.NoTutoringNoBlockErrorPenalization;
						else
							numErrors += _config.TutoringNoBlockErrorPenalization;
					}
				}
			}

			foreach (StudentsClusterByErrors clu in  _clusters.Values) {
				if (numErrors <= clu.MaxErrors && numErrors >= clu.MinErrors) {
					if (cluMatch == null)
						cluMatch = clu;
					else {
						if (Math.Abs (cluMatch.AvgErrors - numErrors) < Math.Abs (clu.AvgErrors - numErrors))
							cluMatch = clu;
					}
				}
			}
			return cluMatch;
		}

		private StudentsCluster MatchClusterBySequences(string studentKey, bool inPhases, string domainKey)
        {
			AdomdConnection con = new AdomdConnection (_config.ASSConString);
			con.Open ();
            AdomdDataAdapter da = new AdomdDataAdapter();
            if (!inPhases)
            {
                da = new AdomdDataAdapter("SELECT Cluster(),t.[id] From [EntireLab] PREDICTION JOIN  SHAPE { OPENQUERY([Lab Biotecnologia], 'SELECT [id] FROM [dbo].[StudentsVal] where id=" + studentKey + " ORDER BY [id]')} APPEND ({OPENQUERY([Lab Biotecnologia], 'SELECT [actionkey], [sequence], [studentid] FROM [dbo].[LogsVal] where studentid=" + studentKey + " ORDER BY [studentid]')} RELATE [id] TO [studentid]) AS [LogsVal] AS t ON [EntireLab].[Logs].[Actionkey] = t.[LogsVal].[actionkey] AND [EntireLab].[Logs].[Sequence] = t.[LogsVal].[sequence]", con);
            }
            else
            {
                string miningStruct = "Phase" + domainKey.Split('_')[0];
                string table1 = "LogsVal";
                string table2 = "LogsValPhase" + domainKey.Split('_')[0];
                string table3 = "Logs Phase" + domainKey.Split('_')[0];

                da = new AdomdDataAdapter("SELECT Cluster(),t.[id] From ["+ miningStruct + "] PREDICTION JOIN  SHAPE { OPENQUERY([Lab Biotecnologia], 'SELECT [id] FROM [dbo].[StudentsVal] where id=" + studentKey + " ORDER BY [id]')} APPEND ({OPENQUERY([Lab Biotecnologia], 'SELECT [actionkey], [sequence], [studentid] FROM [dbo].["+ table1 + "] where studentid=" + studentKey + " ORDER BY [studentid]')} RELATE [id] TO [studentid]) AS ["+ table2 + "] AS t ON ["+ miningStruct + "].["+ table3 + "].[Actionkey] = t.["+ table2 + "].[actionkey] AND ["+ miningStruct + "].["+ table3 + "].[Sequence] = t.["+ table2 + "].[sequence]", con);
            }
            DataTable dtlogstemp = new DataTable ();
			da.Fill (dtlogstemp);
			con.Close ();
			return _clusters [int.Parse (dtlogstemp.Rows [0].ItemArray [0].ToString ().Split(' ')[1])];

		}
		private StudentsClusterByEventsZone MatchClusterByEventsZone(StudentLog studentLogs, bool includeNoPlanActions){
			StudentsClusterByEventsZone cluMatch = null;
			Node<State.State, Event.Event> initState = new Node<State.State, Event.Event> ("initState", "", new CorrectState (Area.CorrectFlow, null, false, 1, 1));
			Node<State.State, Event.Event> previousState = initState;
			List<Node<State.State, Event.Event>> pastNodes = new List<Node<State.State, Event.Event>> ();
			int numCorrectEvents = 0;
			int numIrrelevantEvents = 0;
			int numRelevantEvents = 0;
			foreach (LogEntry log in studentLogs.Logs) {
				if (includeNoPlanActions || log.GetType ().BaseType != typeof(NoPlanAllowedActionLog)) {
					Area tmpArea = GetArea (log, previousState, pastNodes, initState);
					Node<State.State, Event.Event> node = CreateState (log, tmpArea);
					switch (tmpArea) {
					case Area.CorrectFlow:
						numCorrectEvents++;
						break;
					case Area.IrrelevantErrors:
						numIrrelevantEvents++;
						break;
					case Area.RelevantErrors:
						numRelevantEvents++;
						break;
					}
				}
			}
			foreach (StudentsClusterByEventsZone clu in  _clusters.Values) {
				if (numCorrectEvents <= clu.MaxCorrectEvents && numCorrectEvents >= clu.MinCorrectEvents && 
					numIrrelevantEvents <= clu.MaxIrrelevantErrorEvents && numIrrelevantEvents >= clu.MinIrrelevantErrorEvents &&
					numRelevantEvents <= clu.MaxRelevantErrorEvents && numRelevantEvents >= clu.MinRelevantErrorEvents) {
					if (cluMatch == null)
						cluMatch = clu;
					else {
						if (Math.Abs (cluMatch.AvgCorrectEvents - numCorrectEvents) < Math.Abs (clu.AvgCorrectEvents - numCorrectEvents) &&
							Math.Abs (cluMatch.AvgIrrelevantErrorEvents - numIrrelevantEvents) < Math.Abs (clu.AvgIrrelevantErrorEvents - numIrrelevantEvents) &&
							Math.Abs (cluMatch.AvgRelevantErrorEvents - numRelevantEvents) < Math.Abs (clu.AvgRelevantErrorEvents - numRelevantEvents))
							cluMatch = clu;
					}
				}
			}
			return cluMatch;
		}

		/// <summary>
		/// Finds the cluster that has a specific student.
		/// </summary>
		/// <returns>The student cluster.</returns>
		/// <param name="studentKey">Student key.</param>
		public StudentsCluster FindStudentCluster(string studentKey){
			StudentsCluster cluster = null;
			foreach (StudentsCluster tmpcluster in _clusters.Values) {
				if (tmpcluster.HasStudent (studentKey)) {
					cluster = tmpcluster;
					break;
				}
			}
			if (cluster == null)
				throw new ClusterNotFoundException (studentKey);
			return cluster;
		}

		/// <summary>
		/// Containses a student in a cluster of the model.
		/// </summary>
		/// <returns><c>true</c>, if student was containsed, <c>false</c> otherwise.</returns>
		/// <param name="studentKey">Student key.</param>
		public bool ContainsStudent(string studentKey){
			bool contains = false;
			foreach (StudentsCluster tmpcluster in _clusters.Values) {
				if (tmpcluster.HasStudent (studentKey)) {
					contains = true;
					break;
				}
			}
			return contains;
		}

		/// <summary>
		/// Gets a new cluster where the student's action sequence is best suited. 
		/// For each cluster in this model gets the sum of frequencies of states that represent the logs of the student. 
		/// For this, for each log in studentLogs searches a state in StudentActionsModel, that fit with the log, and sums its frequency. 
		/// The return cluster is the one that has the highest sum of state frequencies.
		/// </summary>
		/// <returns>The new cluster.</returns>
		/// <param name="studentLogs">Student logs.</param>
		private StudentsCluster GetNewCluster(List<Node<State.State,Event.Event>> studentNodes){
			StudentsCluster newCluster = null;
			long maxFrequency = 0;
			foreach (StudentsCluster tmpcluster in _clusters.Values) {
				long tmpFrequency = 0;
				Node<State.State,Event.Event> previousState = tmpcluster.StudentActionsModel.InitState;
				foreach (Node<State.State,Event.Event> tmpState in studentNodes) {
					if (tmpState != default(Node<State.State,Event.Event>))
						tmpFrequency += tmpState.Specification.EventFrequency;
					previousState = tmpState;
				}

				if (tmpFrequency > maxFrequency) {
					maxFrequency = tmpFrequency;
					newCluster = tmpcluster;
				}
			}
			if (newCluster == null)
				throw new ClusterNotFoundException ();
			return newCluster;
		}

		/// <summary>
		/// Gets a cluster.
		/// </summary>
		/// <returns>The cluster.</returns>
		/// <param name="clusterNumber">Cluster number.</param>
		public StudentsCluster GetCluster(int clusterNumber){
			StudentsCluster cluster = null;
			if (!_clusters.TryGetValue (clusterNumber, out cluster))
				throw new ClusterNotFoundException (clusterNumber);
			return cluster;
		}

		/// <summary>
		/// Gets the next most probable event.
		/// </summary>
		/// <returns>The next most probable event.</returns>
		/// <param name="studentKey">Student key.</param>
		/// <param name="lastLog">Last log.</param>
		public Arc<State.State,Event.Event> GetNextProbableEvent(string studentKey){
			StudentsCluster cluster = FindStudentCluster (studentKey);
			Arc<State.State,Event.Event> nextEvent = cluster.GetNextProbableEvent (studentKey);
			return nextEvent;
		}

		private class ClusterStudentDataRow{
			public string Key{ get; set;}
			public int NumberErrors{ get; set;}
			public double TotalTime{ get; set;}
			public int CorrectEvents{ get; set;}
			public int IrrelevantErrorEvents{ get; set;}
			public int RelevantErrorEvents{ get; set;}
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="PredictiveStudentModel"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="PredictiveStudentModel"/>.</returns>
		public override string ToString(){
			string temp = "Model: " + _key + " Domain: " + _domain.Key + Environment.NewLine;
			foreach (StudentsCluster sc in _clusters.Values)
				temp += sc.ToString () + Environment.NewLine;
			return temp;
		}
	}
}

