using System;
using System.Collections.Generic;
using Its.ExpertModule.ObjectModel;
using Its.StudentModule.ObjectModel;
using System.Configuration;
using System.Linq;
using weka.core;
using weka.clusterers;
using weka.filters.unsupervised.attribute;
using Its.TutoringModule.StudentBehaviorPredictor.Exceptions;
using Its.Utils.Math;
using Microsoft.AnalysisServices.AdomdClient;
using System.Data;

namespace Its.TutoringModule.StudentBehaviorPredictor.ObjectModel
{
	public enum ClusterMethod
	{
		ErrorsAndTime,
		Sequences
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

		private StudentsCluster _defaultCluster;

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="Its.TutoringModule.StudentBehaviorPredictor.ObjectModel.StudentBehaviorModel"/> class.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="domain">Domain.</param>
		public PredictiveStudentModel(string key, DomainActions domain){
			this._key = key;
			this._domain = domain;
			this._clusters = new Dictionary<int, StudentsCluster> ();
		}

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="Its.TutoringModule.StudentBehaviorPredictor.ObjectModel.StudentBehaviorModel"/> class.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="domainLog">Domain log.</param>
		public PredictiveStudentModel(string key, DomainLog domainLog, ClusterMethod cluMet){
			this._key = key;
			this._domain = domainLog.Domain;
			this._clusters = new Dictionary<int, StudentsCluster> ();
			BuildModel (domainLog, cluMet);
		}

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="Its.TutoringModule.StudentBehaviorPredictor.ObjectModel.PredictiveStudentModel"/> class from a date (for visualization purposes)..
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="domainLog">Domain log.</param>
		/// <param name="fromDate">From date.</param>
		public PredictiveStudentModel(string key, DomainLog domainLog, DateTime fromDate){
			this._key = key;
			this._domain = domainLog.Domain;
			this._clusters = new Dictionary<int, StudentsCluster> ();
			BuildModel (domainLog, fromDate);
		}

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="Its.TutoringModule.StudentBehaviorPredictor.ObjectModel.PredictiveStudentModel"/> class with out clustering (for validation purposes)..
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="logs">Logs.</param>
		/// <param name="domain">Domain.</param>
		public PredictiveStudentModel(string key, List<StudentLog> logs, DomainActions domain){
			this._key = key;
			this._domain = domain;
			this._clusters = new Dictionary<int, StudentsCluster> ();
			BuildModel (logs);
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

		private Dictionary<ClusterStudentDataRow, int> ClusterByErrorsTime (DomainLog domainLog, out int numberOfClusters)
		{
			Instances clusterStudents = GetDataForClustering (domainLog);
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
			for (int i = 0; i < clusterStudents.numInstances (); i++) {
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
			numberOfClusters = clusterer.numberOfClusters ();
			//Console.WriteLine (numberOfClusters.ToString ());
			return studentByCluster;
		}

		private Dictionary<ClusterStudentDataRow, int> ClusterBySequences (DomainLog domainLog, out int numberOfClusters)
		{
			
			AdomdConnection con = new AdomdConnection (ConfigurationManager.AppSettings ["ASSConString"].ToString ());
			con.Open ();
			AdomdDataAdapter da = new AdomdDataAdapter("select NODE_NAME from [Students Seq Clustering].Content where NODE_TYPE=5", con);
			DataTable dtClusters = new DataTable ();
			da.Fill(dtClusters);
			numberOfClusters = dtClusters.Rows.Count;
			Dictionary<ClusterStudentDataRow, int> studentByCluster = new Dictionary<ClusterStudentDataRow, int> ();
			foreach (DataRow dr in dtClusters.Rows) {
				da = new AdomdDataAdapter("select [id] from [Students Seq Clustering].CASES where IsInNode('" + dr [0].ToString () + "')", con);
				DataTable dtStudentIds = new DataTable ();
				da.Fill(dtStudentIds);
				foreach (DataRow drStu in dtStudentIds.Rows) {
					ClusterStudentDataRow csdr = new ClusterStudentDataRow {
						Key = drStu[0].ToString()
					};
					studentByCluster.Add (csdr, int.Parse(dr [0].ToString()));
				}
			}

			con.Close ();
			return studentByCluster;
		}

		/// <summary>
		/// Builds the model.
		/// </summary>
		/// <param name="domainLog">Domain log.</param>
		private void BuildModel(DomainLog domainLog, ClusterMethod cluMet){
			int numberOfClusters = 0;
			Dictionary<ClusterStudentDataRow, int> studentByCluster = new Dictionary<ClusterStudentDataRow, int> ();
			if (cluMet == ClusterMethod.ErrorsAndTime)
				studentByCluster = ClusterByErrorsTime (domainLog, out numberOfClusters);
			else if (cluMet == ClusterMethod.Sequences)
				studentByCluster = ClusterBySequences (domainLog, out numberOfClusters);

			for (int i = 0; i < numberOfClusters; i++) {
				List<ClusterStudentDataRow> clStudentRows = studentByCluster.Where (p => p.Value == i).Select(p => p.Key).ToList();
				if (clStudentRows.Count > 0) {
					StudentsCluster sc = null;
					List<String> studentKeys = clStudentRows.Select (p => p.Key).Distinct ().ToList ();
					if (cluMet == ClusterMethod.ErrorsAndTime) {
						int minErrors = clStudentRows.Min (p => p.NumberErrors);
						int maxErrors = clStudentRows.Max (p => p.NumberErrors);
						double avgErrors = clStudentRows.Average (p => p.NumberErrors);
						double minTime = clStudentRows.Min (p => p.TotalTime);
						double maxTime = clStudentRows.Max (p => p.TotalTime);
						double avgTime = clStudentRows.Average (p => p.TotalTime);
						sc = new StudentsCluster (i, minTime, maxTime, avgTime, minErrors, maxErrors, avgErrors, studentKeys, domainLog);
					} else if (cluMet == ClusterMethod.Sequences) {
						sc = new StudentsCluster (i, studentKeys, domainLog);
						if (i == 0 || studentKeys.Count > _defaultCluster.NumberOfStudents)
							_defaultCluster = sc;
					}
					_clusters.Add (i, sc);
				}
			}

			if (cluMet == ClusterMethod.ErrorsAndTime) {
				double avgTotalErrors = _clusters.Values.Average (p => p.AvgErrors);
				double avgTotalTime = _clusters.Values.Average (p => p.AvgTime);

				double avgErrorsDif = 0;
				double avgTimeDif = 0;

				//Default cluster is calculated by the nearest centroid to the average
				for (int index = 0; index < _clusters.Count; index++) {
					StudentsCluster sc = _clusters.ElementAt (index).Value;
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
		}

		/// <summary>
		/// Builds the model with out clustering (for validation purposes).
		/// </summary>
		/// <param name="domainLog">Domain log.</param>
		private void BuildModel(List<StudentLog> logs){
			StudentsCluster sc = new StudentsCluster (logs);
			_clusters.Add (0, sc);
			_defaultCluster = _clusters [0];
		}

		/// <summary>
		/// Builds the model from a date (for visualization purposes).
		/// </summary>
		/// <param name="domainLog">Domain log.</param>
		private void BuildModel(DomainLog domainLog, DateTime fromDate){
			Instances clusterStudents = GetDataForClustering (domainLog, fromDate);
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
					StudentsCluster sc = new StudentsCluster (i, minTime, maxTime, avgTime, minErrors, maxErrors, avgErrors, studentKeys, domainLog, fromDate);
					_clusters.Add (i, sc);
				}
			}

			double avgTotalErrors = _clusters.Values.Average (p => p.AvgErrors);
			double avgTotalTime = _clusters.Values.Average (p => p.AvgTime);

			double avgErrorsDif = 0;
			double avgTimeDif = 0;

			//Default cluster is claculated by the nearest centroid to the average
			for (int index = 0; index < _clusters.Count; index++) {
				StudentsCluster sc = _clusters.ElementAt (index).Value;
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
		private Instances GetDataForClustering(DomainLog domainLog){
			List<Student> students = domainLog.GetStudents ();

			ActionAplication lastAction = _domain.GetLastAction ();

			weka.core.Attribute attKey = new weka.core.Attribute ("Key", (FastVector) null);
			weka.core.Attribute attNumberErrors = new weka.core.Attribute ("NumberErrors");
			weka.core.Attribute attTotalTime = new weka.core.Attribute ("TotalTime");
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
					if (log.GetType ().BaseType == typeof(ErrorLog)) {
						if (log.Error.IsBlock) {
							if (log.GetType () == typeof(WorldErrorLog))
								numErrors++;
							else
								numErrors += int.Parse (ConfigurationManager.AppSettings ["TutoringBlockErrorPenalization"]);
						} else {
							if (log.GetType () == typeof(WorldErrorLog))
								numErrors += int.Parse (ConfigurationManager.AppSettings ["NoTutoringNoBlockErrorPenalization"]);
							else
								numErrors += int.Parse (ConfigurationManager.AppSettings ["TutoringNoBlockErrorPenalization"]);
						}
					}
				}
				if (sudentLogs.Logs [sudentLogs.Logs.Count - 1].Action != lastAction && totalTime == 0)
					totalTime = totalTime + (_domain.EstimatedTime * 60 * 60) * int.Parse (ConfigurationManager.AppSettings ["TimePenalization"]);
				Instance instance = new Instance(3);
				instance.setValue((weka.core.Attribute)fvWekaAttributes.elementAt(0), student.Key);
				instance.setValue((weka.core.Attribute)fvWekaAttributes.elementAt(1), numErrors);
				instance.setValue((weka.core.Attribute)fvWekaAttributes.elementAt(2), totalTime);
				trainingSet.add(instance);
			}
			return trainingSet;

		}

		private Instances GetDataForClustering(DomainLog domainLog, DateTime fromDate){
			List<Student> students = domainLog.GetStudents ();

			ActionAplication lastAction = _domain.GetLastAction ();

			weka.core.Attribute attKey = new weka.core.Attribute ("Key", (FastVector) null);
			weka.core.Attribute attNumberErrors = new weka.core.Attribute ("NumberErrors");
			weka.core.Attribute attTotalTime = new weka.core.Attribute ("TotalTime");
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
						if (log.GetType ().BaseType == typeof(ErrorLog)) {
							if (log.Error.IsBlock) {
								if (log.GetType () == typeof(WorldErrorLog))
									numErrors++;
								else
									numErrors += int.Parse (ConfigurationManager.AppSettings ["TutoringBlockErrorPenalization"]);
							} else {
								if (log.GetType () == typeof(WorldErrorLog))
									numErrors += int.Parse (ConfigurationManager.AppSettings ["NoTutoringNoBlockErrorPenalization"]);
								else
									numErrors += int.Parse (ConfigurationManager.AppSettings ["TutoringNoBlockErrorPenalization"]);
							}
						}
					}
					if (sudentLogs.Logs [sudentLogs.Logs.Count - 1].Action != lastAction && totalTime == 0)
						totalTime = totalTime + (_domain.EstimatedTime * 60 * 60) * int.Parse (ConfigurationManager.AppSettings ["TimePenalization"]);
					Instance instance = new Instance (3);
					instance.setValue ((weka.core.Attribute)fvWekaAttributes.elementAt (0), student.Key);
					instance.setValue ((weka.core.Attribute)fvWekaAttributes.elementAt (1), numErrors);
					instance.setValue ((weka.core.Attribute)fvWekaAttributes.elementAt (2), totalTime);
					trainingSet.add (instance);
				}
			}
			return trainingSet;

		}

		/// <summary>
		/// Updates the model.
		/// </summary>
		/// <param name="studentKey">Student key.</param>
		/// <param name="log">Log.</param>
		public void UpdateModel(string studentKey, LogEntry log){
			StudentsCluster cluster = FindStudentCluster (studentKey);
			cluster.UpdateAutomaton (studentKey, log);
		}

		/// <summary>
		/// Updates the model reclustering the student.
		/// </summary>
		/// <param name="studentKey">Student key.</param>
		/// <param name="log">Log.</param>
		/// <param name="studentLogs">Student logs.</param>
		public void UpdateModel(string studentKey, LogEntry log, StudentLog studentLogs){
			StudentsCluster newCluster = ReclusterStudent (studentLogs);
			newCluster.UpdateAutomaton (studentKey, log);
		}

		/// <summary>
		/// Reclusters the student.
		/// </summary>
		/// <returns>The student.</returns>
		/// <param name="studentLogs">Student logs.</param>
		private StudentsCluster ReclusterStudent(StudentLog studentLogs){
			StudentsCluster oldCluster = FindStudentCluster (studentLogs.Owner.Key);

			StudentsCluster newCluster = GetNewCluster (studentLogs);

			if (newCluster != oldCluster) {
				oldCluster.RemoveStudent (studentLogs);
				newCluster.AddStudent (studentLogs, newCluster.StudentActionsModel.InitState);
			}
			return newCluster;
		}

		public int MatchClusterForNewStudent(StudentLog studentLogs){
			StudentsCluster newCluster = GetNewCluster (studentLogs);
			return newCluster.Number;
		}

		/// <summary>
		/// Finds the cluster that has a specific student.
		/// </summary>
		/// <returns>The student cluster.</returns>
		/// <param name="studentKey">Student key.</param>
		private StudentsCluster FindStudentCluster(string studentKey){
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
		private StudentsCluster GetNewCluster(StudentLog studentLogs){
			StudentsCluster newCluster = null;
			long maxFrequency = 0;
			foreach (StudentsCluster tmpcluster in _clusters.Values) {
				long tmpFrequency = 0;

				foreach (LogEntry log in studentLogs.Logs) {
					Node<State,Event> tmpState = tmpcluster.StudentActionsModel.FindState (log);
					if (tmpState != default(Node<State,Event>))
						tmpFrequency += tmpState.Specification.Frequency;
				}

				if (tmpFrequency > maxFrequency) {
					maxFrequency = tmpFrequency;
					newCluster = tmpcluster;
				}
			}
			if (newCluster == null)
				throw new ClusterNotFoundException (studentLogs.Owner.Key);
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
		public Arc<State,Event> GetNextProbableEvent(string studentKey, LogEntry lastLog){
			StudentsCluster cluster = FindStudentCluster (studentKey);
			Arc<State,Event> nextEvent = cluster.StudentActionsModel.GetNextProbableEvent (lastLog);
			return nextEvent;
		}

		private class ClusterStudentDataRow{
			public string Key{ get; set;}
			public int NumberErrors{ get; set;}
			public double TotalTime{ get; set;}
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="Its.TutoringModule.StudentBehaviorPredictor.ObjectModel.PredictiveStudentModel"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="Its.TutoringModule.StudentBehaviorPredictor.ObjectModel.PredictiveStudentModel"/>.</returns>
		public override string ToString(){
			string temp = "Model: " + _key + " Domain: " + _domain.Key + System.Environment.NewLine;
			foreach (StudentsCluster sc in _clusters.Values)
				temp += sc.ToString () + System.Environment.NewLine;
			return temp;
		}
	}
}

