using System;
using System.Collections.Generic;
using Its.ExpertModule.ObjectModel;
using Its.StudentModule.ObjectModel;
using Its.TutoringModule.CMTutor.SBP.Exceptions;
using Its.TutoringModule.CMTutor.SBP.OM;
using Its.TutoringModule.CMTutor.SBP.OM.Event;
using Its.TutoringModule.CMTutor.SBP.OM.State;
using Its.Utils.Config;
using Its.Utils.Math;

namespace Its.TutoringModule.CMTutor.SBP
{
	public class StudentBehaviorPredictorControl
	{
		private Dictionary<string,PredictiveStudentModel> _models=new Dictionary<string, PredictiveStudentModel>();
		/// <summary>
		/// The instance.
		/// </summary>
		private static StudentBehaviorPredictorControl _instance = null;

		private ITutorConfig _config;

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static StudentBehaviorPredictorControl Instance (ITutorConfig config) {
			if (_instance == null)
				_instance = new StudentBehaviorPredictorControl (config);

			return _instance;
		}

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="Its.TutoringModule.StudentBehaviorPredictor.StudentBehaviorPredictor"/> class.
		/// </summary>
		private StudentBehaviorPredictorControl (ITutorConfig config)
		{
			this._config = config;
		}

		public static void DisposeInstance() {
			if (_instance != null) {
				_instance = null;
			}
		}

		/// <summary>
		/// Gets a predictive student model.
		/// </summary>
		/// <returns>The model.</returns>
		/// <param name="domainKey">Domain key.</param>
		public PredictiveStudentModel GetModel(string domainKey, ClusterMethod cluMet){
			PredictiveStudentModel model = null;
			if (!_models.TryGetValue (domainKey+"_"+cluMet.ToString(), out model))
				throw new PredictiveStudentModelNotFoundException (domainKey+"_"+cluMet.ToString());
			return model;
		}

		/// <summary>
		/// Adds a new model for a domain.
		/// </summary>
		/// <param name="domainLog">Domain log.</param>
		public void AddModel(DomainLog domainLog, ClusterMethod cluMet, bool includeNoPlanActions, bool inPhases)
        {
			PredictiveStudentModel model = new PredictiveStudentModel (domainLog, cluMet, includeNoPlanActions, inPhases, _config);
			_models.Add (model.Key+"_"+cluMet.ToString(), model);
		}

		/// <summary>
		/// Gets the model from an specific date.
		/// </summary>
		/// <returns>The model from date.</returns>
		/// <param name="domainLog">Domain log.</param>
		/// <param name="fromDate">From date.</param>
		public PredictiveStudentModel GetModelFromDate(DomainLog domainLog, DateTime fromDate, bool includeNoPlanActions){
			PredictiveStudentModel model = new PredictiveStudentModel (domainLog, fromDate, includeNoPlanActions, _config);
			return model;
		}

		/// <summary>
		/// Gets the automata for validation.
		/// </summary>
		/// <returns>The automata for validation.</returns>
		/// <param name="logs">Logs.</param>
		/// <param name="domain">Domain.</param>
		public PredictiveStudentModel GetAutomataForValidation(List<StudentLog> logs, DomainActions domain, ClusterMethod cluMet, bool includeNoPlanActions){
			PredictiveStudentModel model = new PredictiveStudentModel (logs, domain, cluMet, includeNoPlanActions, _config);
			return model;
		}

		/// <summary>
		/// Updates the model for a domain.
		/// </summary>
		/// <param name="domianKey">Domain key.</param>
		/// <param name="studetKey">Student key.</param>
		/// <param name="log">Log.</param>
		public void UpdateModel(string domainKey, ClusterMethod cluMet, string studentKey, LogEntry log){
			PredictiveStudentModel model = GetModel(domainKey, cluMet);
			if (!model.ContainsStudent (studentKey)) 
			{
				model.DefaultCluster.AddStudent (studentKey, log, new Dictionary<string,ActionAplication>(), true);
			}
			else
			{
				model.UpdateModel (studentKey, log);	
			}
		}

		/// <summary>
		/// Updates the model and recluster the student.
		/// </summary>
		/// <param name="domainKey">Domain key.</param>
		/// <param name="studentKey">Student key.</param>
		/// <param name="log">Log.</param>
		/// <param name="studentLogs">Student logs.</param>
		public void UpdateModelAndRecluster(string domainKey, ClusterMethod cluMet, string studentKey, LogEntry log, StudentLog studentLogs)
		{
			PredictiveStudentModel model = GetModel(domainKey, cluMet);
			model.UpdateModelAndRecluster(studentKey, log, studentLogs);
		}

		/// <summary>
		/// Gets the next most probable event.
		/// </summary>
		/// <returns>The next most probable event.</returns>
		/// <param name="domainKey">Domain key.</param>
		/// <param name="studentKey">Student key.</param>
		/// <param name="lastLog">Last log.</param>
		public Arc<State,Event> GetNextProbableEvent(PredictiveStudentModel model, string studentKey){
			return model.GetNextProbableEvent (studentKey);
		}
		
		/// <summary>
		/// Gets the next most probable CORRECT event.
		/// </summary>
		/// <returns>The next most probable CORRECT event.</returns>
		/// <param name="domainKey">Domain key.</param>
		/// <param name="studentKey">Student key.</param>
		public Arc<State,Event> GetNextProbableCorrectEvent(string domainKey, ClusterMethod cluMet, string studentKey, HashSet<string> possibleNextActions)
		{
			PredictiveStudentModel model = GetModel(domainKey, cluMet);
			return model.GetNextProbableCorrectEvent(studentKey, possibleNextActions);
		}
		
		public Node<State, Event> GetLastState(string domainKey, ClusterMethod cluMet, string studentKey)
		{
			PredictiveStudentModel model = GetModel(domainKey, cluMet);
			StudentsCluster cluster = model.FindStudentCluster(studentKey);
			return cluster.GetLastState(studentKey);
		}

		/// <summary>
		/// Gets the support value for the last state corresponding to last performed action
		/// </summary>
		/// <returns>Support value in the range [0, 1].</returns>
		/// <param name="studentKey">Student key.</param>
		public double GetLastStateSupport(string domainKey, ClusterMethod cluMet, string studentKey)
		{
			PredictiveStudentModel model = GetModel(domainKey, cluMet);
			StudentsCluster cluster = model.FindStudentCluster(studentKey);
			return cluster.GetLastStateSupport(studentKey);
		}

		public List<Arc<State, Event>> GetNextCorrectEventsAboveThreshold(string domainKey, ClusterMethod cluMet, string studentKey, double threshold)
		{
			PredictiveStudentModel model = GetModel(domainKey, cluMet);
			StudentsCluster cluster = model.FindStudentCluster(studentKey);
			return cluster.GetNextCorrectEventsAboveThreshold(studentKey, threshold); 
		}
		
		public List<Arc<State, Event>> GetNextIEAreaEventsAboveThreshold(string domainKey, ClusterMethod cluMet, string studentKey, double threshold)
		{
			PredictiveStudentModel model = GetModel(domainKey, cluMet);
			StudentsCluster cluster = model.FindStudentCluster(studentKey);
			return cluster.GetNextIEAreaEventsAboveThreshold(studentKey, threshold); 
		}
		
		public double GetEventConfidence(string domainKey, ClusterMethod cluMet, string studentKey, Arc<State, Event> evt)
		{
			PredictiveStudentModel model = GetModel(domainKey, cluMet);
			StudentsCluster cluster = model.FindStudentCluster(studentKey);
			return cluster.GetEventConfidence(evt);
		}

		public List<Node<State, Event>> GetAllREStatesAboveThreshold(string domainKey, ClusterMethod cluMet, string studentKey, double supportThreshold)
		{
			PredictiveStudentModel model = GetModel(domainKey, cluMet);
			StudentsCluster cluster = model.FindStudentCluster(studentKey);
			return cluster.GetAllREStatesAboveThreshold(supportThreshold); 
		}
		
		public List<Node<State, Event>> GetAllREErrorStatesAboveThreshold(string domainKey, ClusterMethod cluMet, string studentKey, double supportThreshold)
		{
			PredictiveStudentModel model = GetModel(domainKey, cluMet);
			StudentsCluster cluster = model.FindStudentCluster(studentKey);
			return cluster.GetAllREErrorStatesAboveThreshold(supportThreshold); 
		}

		public StudentActionsModel GetStudentActionsModel(string domainKey, ClusterMethod cluMet, string studentKey)
		{
			PredictiveStudentModel model = GetModel(domainKey, cluMet);
			StudentsCluster cluster = model.FindStudentCluster(studentKey);
			return cluster.StudentActionsModel;
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="Its.TutoringModule.StudentBehaviorPredictor.StudentBehaviorPredictor"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="Its.TutoringModule.StudentBehaviorPredictor.StudentBehaviorPredictor"/>.</returns>
		/*public override string ToString(){
			string temp = "Models: " + Environment.NewLine;
			foreach (PredictiveStudentModel model in _models.Values)
				temp += model.ToString () + Environment.NewLine;
			return temp;
		}*/
	}
}

