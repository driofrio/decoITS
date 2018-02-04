﻿using System;
using System.Collections.Generic;
using Its.TutoringModule.StudentBehaviorPredictor.ObjectModel;
using Its.StudentModule;
using Its.StudentModule.ObjectModel;
using Its.TutoringModule.StudentBehaviorPredictor.Exceptions;
using Its.Utils.Math;
using Its.ExpertModule.ObjectModel;

namespace Its.TutoringModule.StudentBehaviorPredictor
{
	public class StudentBehaviorPredictorControl
	{
		private Dictionary<string,PredictiveStudentModel> _models=new Dictionary<string, PredictiveStudentModel>();
		/// <summary>
		/// The instance.
		/// </summary>
		private static StudentBehaviorPredictorControl _instance = null;

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static StudentBehaviorPredictorControl Instance {
			get {
				if (_instance == null)
					_instance = new StudentBehaviorPredictorControl ();

				return _instance;
			}
		}

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="Its.TutoringModule.StudentBehaviorPredictor.StudentBehaviorPredictor"/> class.
		/// </summary>
		private StudentBehaviorPredictorControl ()
		{
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
			PredictiveStudentModel model = new PredictiveStudentModel (domainLog, cluMet, includeNoPlanActions, inPhases);
			_models.Add (model.Key+"_"+cluMet.ToString(), model);
		}

		/// <summary>
		/// Gets the model from an specific date.
		/// </summary>
		/// <returns>The model from date.</returns>
		/// <param name="domainLog">Domain log.</param>
		/// <param name="fromDate">From date.</param>
		public PredictiveStudentModel GetModelFromDate(DomainLog domainLog, DateTime fromDate, bool includeNoPlanActions){
			PredictiveStudentModel model = new PredictiveStudentModel (domainLog, fromDate, includeNoPlanActions);
			return model;
		}

		/// <summary>
		/// Gets the automata for validation.
		/// </summary>
		/// <returns>The automata for validation.</returns>
		/// <param name="logs">Logs.</param>
		/// <param name="domain">Domain.</param>
		public PredictiveStudentModel GetAutomataForValidation(List<StudentLog> logs, DomainActions domain, ClusterMethod cluMet, bool includeNoPlanActions){
			PredictiveStudentModel model = new PredictiveStudentModel (logs, domain, cluMet, includeNoPlanActions);
			return model;
		}

		/*/// <summary>
		/// Updates the model for a domain.
		/// </summary>
		/// <param name="domianKey">Domain key.</param>
		/// <param name="studetKey">Student key.</param>
		/// <param name="log">Log.</param>
		public void UpdateModel(string domainKey, string studentKey, LogEntry log){
			PredictiveStudentModel model = _models [domainKey];
			if (!model.ContainsStudent (studentKey)) {
				model.DefaultCluster.AddStudent (studentKey, log);
				//model.DefaultCluster.UpdateAutomaton (studentKey, log);
			} else
				model.UpdateModel (studentKey, log);
		}

		/// <summary>
		/// Updates the model and recluster the student.
		/// </summary>
		/// <param name="domainKey">Domain key.</param>
		/// <param name="studentKey">Student key.</param>
		/// <param name="log">Log.</param>
		/// <param name="studentLogs">Student logs.</param>
		public void UpdateModel(string domainKey, string studentKey, LogEntry log, StudentLog studentLogs){
			_models[domainKey].UpdateModel(studentKey, log, studentLogs);
		}*/

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
