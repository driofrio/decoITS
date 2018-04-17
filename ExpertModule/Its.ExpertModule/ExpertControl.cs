using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Its.ExpertModule.ObjectModel;
using Its.Factories;
using Its.StudentModule;
using Its.StudentModule.ObjectModel;
using Its.TutoringModule.ReactiveTutor.ObjectModel;

namespace Its.ExpertModule
{
	public class ExpertControl
	{
		/// <summary>
		/// The instance.
		/// </summary>
		private static ExpertControl _instance = null;
		/// <summary>
		/// The instance domain actions factory.
		/// </summary>
		private static DomainActionsFactory _instanceDomainActionsFactory;
		/// <summary>
		/// The instance student control.
		/// </summary>
		private static StudentControl _instanceStudentControl;
		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static ExpertControl Instance (string ontologyPath, string logsPath, string expertConfPath, int initialCol, int initialRow){
			//get {
			if (_instance == null)
				_instance = new ExpertControl (ontologyPath, logsPath, expertConfPath, initialCol, initialRow);

			return _instance;
			//}
		}
		/// <summary>
		/// The domain actions list.
		/// </summary>
		private static Dictionary<string, DomainActions> _domainActionsList;
		/// <summary>
		/// The other errors.
		/// </summary>
		private static Dictionary<string, Error> _otherErrors;
		/// <summary>
		/// Gets the other errors.
		/// </summary>
		/// <value>The other errors.</value>
		public Dictionary<string, Error> OtherErrors{
			get{
				return _otherErrors;
			}
		}
		/// <summary>
		/// The dictionary for minimun errors.
		/// </summary>
		private static Dictionary<string,Dictionary<string, Thread>> _minTimerController;
		/// <summary>
		/// The dictionary for maximun errors.
		/// </summary>
		private static Dictionary<string,Dictionary<string, Thread>> _maxTimerController;

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.ExpertModule.ExpertControl"/> class.
		/// </summary>
		private ExpertControl (string ontologyPath, string logsPath, string expertConfPath, int initialCol, int initialRow)
		{
			_instanceStudentControl = StudentControl.Instance (ontologyPath, logsPath, expertConfPath);
			_instanceDomainActionsFactory = DomainActionsFactory.Instance (ontologyPath, logsPath, expertConfPath, initialCol, initialRow);
			_domainActionsList = new Dictionary<string, DomainActions> ();
			//Gets OtherErrors.
			List<Error> otherErrors = ErrorFactory.Instance(ontologyPath, logsPath).CreateOtherErrors();
			//Adds OtherErrors into the dictionary.
			_otherErrors = new Dictionary<string, Error> ();
			foreach (Error e in otherErrors)
				_otherErrors.Add (e.Key, e);
			//Instantiates timers controllers.
			_minTimerController = new Dictionary<string, Dictionary<string, Thread>> ();
			_maxTimerController = new Dictionary<string, Dictionary<string, Thread>> ();
		}

		public static void DisposeInstance() {
			if (_instance != null) {
				_instance = null;
			}
		}

		/// <summary>
		/// Creates the domain.
		/// </summary>
		/// <param name="domainKey">Domain key.</param>
		public DomainActions CreateDomain (string domainKey)
		{
			//Creates the DomainActions with the given key.
			DomainActions domain = _instanceDomainActionsFactory.CreateDomain(domainKey);
			//Adds into the dictionary.
			_domainActionsList.Add(domain.Key, domain);
			//Creates the domain log.
			_instanceStudentControl.CreateDomainLog (domain);
			//Returns the result.
			return domain;
		}
		
		/// <summary>
		/// Creates the domain.
		/// </summary>
		/// <param name="domainKey">Domain key.</param>
		public DomainActions CreateDomainIfDoesntExist (string domainKey)
		{
			if (!_domainActionsList.ContainsKey(domainKey))
			{
				return CreateDomain(domainKey);
			}
			else
			{
				return _domainActionsList[domainKey];
			}
		}

		/// <summary>
		/// Gets the messages.
		/// </summary>
		/// <param name="actionName">Action name.</param>
		/// <param name="domainKey">Domain key.</param>
		/// <param name="studentKey">Student key.</param>
		/// <param name="okMessage">Ok message.</param>
		/// <param name="tutorMessages">Tutor messages.</param>
		public void GetMessages (string actionName, string domainKey, string studentKey, out string okMessage,
			out List<TutorMessage> tutorMessages)
		{
			//Creates an auxiliar variable.
			DomainActions domain;
			//Gest the DomainActions from the dictionary.
			if (_domainActionsList.TryGetValue (domainKey, out domain) == false) {
				//Throws an exception if the domain key does not exist.
				throw new ArgumentException ("DomainKey argument does not exist in the current context.");
			} else {
				//Gets the StudentLog with the given key.
				StudentLog studentLog = _instanceStudentControl.GetStudentLog(domainKey, studentKey);
				//Searchs and gets the action with the actionName given.
				var oQuery = 
					from o in domain.Actions
						where o.Name == actionName && o.Phase == studentLog.CurrentPhase
					select o;
				//Checks if the action with the given name does exist.
				if (oQuery.ToList().Count == 0) {
					throw new ArgumentException ("ActionName argument does not exist in the current context.");
				} else {
					//Gets the okMessage and TutorMessages.
					okMessage = oQuery.First ().OkMessage;
					tutorMessages = new List<TutorMessage> ();
					foreach (ActionAplication act in oQuery.First().PossibleNextActions)
                        if (act.TutorMsg != null)
						    tutorMessages.Add (act.TutorMsg); 
				}
			}
		}

		/// <summary>
		/// Validates the action.
		/// </summary>
		/// <returns>The validation.</returns>
		/// <param name="actionName">Action name.</param>
		/// <param name="domainKey">Domain key.</param>
		/// <param name="studentKey">Student key.</param>
		/// <param name="obj">Object.</param>
		/// <param name="outputError">Output error.</param>
		/// <param name="blockObject">Block object.</param>
		public int ActionValidation (string actionName, string domainKey, string studentKey, string objName,
			out List<Error> outputError, out bool blockObject)
		{
			/*
			 * 1. Comprobar previa ejecución de la acción y si esta es repetitiva. Error si no es repetitiva.
			 * 2. Comprobar dependencias.
			 * 		2.1. Si es bloqueante, se notifica error.
			 * 		2.2. Si no es bloqueante, se guarda el error.
			 * 3. Comprobar incompatibilidades.
			 * 4. Comprobar si la acción debe mostrar todos los mensajes cometidos previamente.
			 */
			//Creates a variable with the result to return.
			int result = 1;
			//Sets the value of blockObject to a default value.
			blockObject = false;
			//Gets the DomainActions.
			DomainActions domain = _domainActionsList [domainKey];
			//Gets the Student.
			Student student = _instanceStudentControl.GetStudent (studentKey);
			//Gets the StudentLog of the given student for the specific domain.
			StudentLog studentLog;
			//Creates an auxiliar variable.
			Dictionary<string, Thread> dic;
			try {
				studentLog = _instanceStudentControl.GetStudentLog (domainKey, studentKey);
			} catch (ArgumentException e) {
				//Checks if the exception message is the following message.
				if (e.Message == "There is not any StudentLog with the given key.") {
					//Creates a new studentLog.
					_instanceStudentControl.CreateStudentLog (domainKey, studentKey);
					//Gets the studentLog.
					studentLog = _instanceStudentControl.GetStudentLog (domainKey, studentKey);
					//Initializes the current phase counter.
					studentLog.CurrentPhase = 0;
				} else {
					throw new ArgumentException ("There is not any DomainLog with the given key.");
				}
			}

			//Searchs and gets the action with the actionName given.
			ActionAplication action = null;
			outputError = new List<Error> ();
			var actQuery = 
				from o in domain.Actions
					where o.Name == actionName && o.Phase == studentLog.CurrentPhase
				select o;
			//Checks if the action exist.
			if (actQuery.ToList().Count > 0) {
				action = actQuery.First ();
			} else {
				//Get the error.
				Error error = _otherErrors["actionnotfound"];
				//Creates the log.
				_instanceStudentControl.CreateOtherErrorLog(action, domain, student, false, error);
				//Adds the error to the answer param.
				outputError.Add(error);
				//Returns the value.
				return 0;
			}
			//Checks whether the action has already done.
			var oQuery = 
				from o in studentLog.Logs
					where o.Action.Equals(action) && o.Error == null
				select o.Action;
			//Whether the action with the given name has just done previously.
			if (oQuery.ToList().Count != 0) {
				//Checks if the action can be repeated.
				if (action.IsRepetitive == false) {
					//Get the error.
					Error error = _otherErrors["actionalreadyperformed"];
					//Creates the log.
					_instanceStudentControl.CreateOtherErrorLog(action, domain,student, false, error);
					//Adds the error to the answer param.
					outputError.Add(error);
					//Returns the value.
					return 0;
				} else {
					//Gets action dependence.
					ComplexDependence actionDependence = action.Dependence;
					//Creates an auxiliar variable.
					int resultCheckDependence = 0;
					//Creates an auxiliar list in which will be saved the dependencies with errors.
					List<Dependence> dependenceErrors = new List<Dependence>();
					//Checks if the action has a dependence.
					if (actionDependence != null) {
						//Checks the dependence type.
						if (actionDependence.GetType () == typeof(SeqComplexDependence))
							resultCheckDependence = CheckSeqComplexDependencies ((SeqComplexDependence)actionDependence, action.Key, 
								studentLog, domain, ref dependenceErrors);
						else if (actionDependence.GetType () == typeof(OptComplexDependence))
							resultCheckDependence = CheckOptComplexDependencies ((OptComplexDependence)actionDependence, action.Key, 
								studentLog, domain, ref dependenceErrors);
						//Checks the result.
						if (resultCheckDependence == 0) {
							//Creates the log.
							_instanceStudentControl.CreateDepErrorLog (action, domain, student, false,
								dependenceErrors.Last ());
							//Adds the error to the answer param.
							outputError.Add (dependenceErrors.Last ().DependenceError);
							//Returns the value.
							return 0;
						} else if (resultCheckDependence == -1) {
							//Sets the result.
							result = -1;
							//Creates a log for each dependence with an error.
							foreach (Dependence d in dependenceErrors) {
								//Creates the log.
								_instanceStudentControl.CreateDepErrorLog (action, domain, student, false,
									d);
							}
						}
					}
					//Checks if there are any incompatibility.
					if (action.Incompatibilities != null) {
						//Checks if an incompatibility of the action exist.
						foreach (Incompatibility i in action.Incompatibilities) {
							//Checks if the incompatibility was done before the action
							if (_instanceStudentControl.CheckLogActionOrder (domainKey, studentKey, 
								   i.IncompatibilityAction.Key, action.Key)) {
								//Checks if the incompatibility blocks.
								if (i.IncompatibilityError.IsBlock) {
									//Creates the log.
									_instanceStudentControl.CreateIncompErrorLog (action, domain, student, false, i);
									//Adds the error to the answer param.
									outputError.Add (i.IncompatibilityError);
									//Returns the value.
									return 0;
								} else {
									//Creates the log.
									_instanceStudentControl.CreateIncompErrorLog (action, domain, student, false, i);
									//Sets the result.
									result = -1;
								}
							}
						}
					}
					//Checks if the action blocks the object.
					if (action.LockObj)
						blockObject = true;
					//Check if the action validate the phase errors.
					if (action.ValidateErrors) {
						//Gets all errors done by the user previously.
						var errQuery = 
							from o in studentLog.Logs
								where o.Action.Phase == studentLog.CurrentPhase
							select o.Error;
						//Adds errors into the list that will be returned.
						foreach (Error e in errQuery.ToList()) {
							if (e != null)
								outputError.Add (e);
						}
					}
					//Checks if the action change the current phase.
					if (action.InitPhase)
						studentLog.CurrentPhase += 1;
					//Checks if the action has minimun and/or maximun time.
					if (action.MinTime > 0) {
						//Creates a thread to control the minimun timer.
						Thread minTimerThread = new Thread (new ParameterizedThreadStart(MinTimer));
						//Checks if student has an entry.
						if (_minTimerController.ContainsKey (student.Key)) {
							//Checks if a entry for the specific action already exists.
							if (_minTimerController.TryGetValue (student.Key, out dic)) {
								if (dic.ContainsKey (action.Key)) {
									//Get the error.
									Error error = _otherErrors ["actionalreadyperformed"];
									//Creates the log.
									_instanceStudentControl.CreateOtherErrorLog (action, domain, student, false, error);
									//Adds the error to the answer param.
									outputError.Add (error);
									//Returns the value.
									return 0;
								} else {
									//Adds a new entry to the dictionary.
									if (_minTimerController.TryGetValue (student.Key, out dic))
										dic.Add (action.Key, minTimerThread);
								}
							}
						} else {
							//Adds a new entry to the dictionary.
							_minTimerController.Add(student.Key, new Dictionary<string, Thread>());
							//Adds the thread to the dictionary.
							//Adds a new entry to the dictionary.
								if (_minTimerController.TryGetValue(student.Key, out dic))
									dic.Add (action.Key, minTimerThread);
						}
						//Starts the thread.
						ArrayList param = new ArrayList();
						param.Add (minTimerThread);
						param.Add (action);
						param.Add (domain);
						param.Add (student);
						minTimerThread.Start (param);
					}
					if (action.MaxTime > 0) {
						//Creates a thread to control the minimun timer.
						Thread maxTimerThread = new Thread (new ParameterizedThreadStart(MaxTimer));
						//Checks if student has an entry.
						if (_maxTimerController.ContainsKey (student.Key)) {
							//Checks if a entry for the specific action already exists.
							if (_maxTimerController.TryGetValue (student.Key, out dic)) {
								if (dic.ContainsKey (action.Key)) {
									//Get the error.
									Error error = _otherErrors ["actionalreadyperformed"];
									//Creates the log.
									_instanceStudentControl.CreateOtherErrorLog (action, domain, student, false, error);
									//Adds the error to the answer param.
									outputError.Add (error);
									//Returns the value.
									return 0;
								} else {
									//Adds a new entry to the dictionary.
									if (_maxTimerController.TryGetValue (student.Key, out dic))
										dic.Add (action.Key, maxTimerThread);
								}
							}
						} else {
							//Adds a new entry to the dictionary.
							_maxTimerController.Add(student.Key, new Dictionary<string, Thread>());
							//Adds the thread to the dictionary.
							//Adds a new entry to the dictionary.
							if (_maxTimerController.TryGetValue(student.Key, out dic))
								dic.Add (action.Key, maxTimerThread);
						}

						//Creates a thread to control the maximun timer.
						maxTimerThread = new Thread (new ParameterizedThreadStart(MaxTimer));
						//Starts the thread.
						ArrayList param = new ArrayList();
						param.Add (maxTimerThread);
						param.Add (action);
						param.Add (student);
						param.Add (action.MaxTime);
						//Starts the thread.
						maxTimerThread.Start (param);
					}
					//At this moment, the action has been successful. Then, the action will be registered.
					if (!action.CorrectiveAction && action.MinTime <= 0)
						_instanceStudentControl.CreateNoCorrectiveActionLog (action, domain, student, true);
					//Gets the previous action.
					ActionAplication previousAction = domain.GetPreviousAction(action.Key);
					//Checks if this is the first action done by the student.
					if (previousAction != null) {
						//Creates an auxiliar variable that will not use.
						Thread t;
						Dictionary<string, Thread> maxTDic;
						//Checks if the previous action created a maximun timer.
						if (_maxTimerController.TryGetValue (studentLog.Owner.Key, out maxTDic)) {
							if (!maxTDic.TryGetValue (previousAction.Key, out t) && previousAction.MaxTime > 0) {
								//Generates a max time error.
								_instanceStudentControl.CreateMaxTimeErrorLog (previousAction, domain, studentLog.Owner, false, previousAction.MaxTime);
								//Checks if the dependence blocks or not.
								if (previousAction.MaxTimeError.IsBlock) {
									//The result will be zero.
									result = 0;
									//Adds the dependence into the list.
									outputError.Add (previousAction.MaxTimeError);
								} else {
									//The result will be -1.
									result = -1;
								}
							}
						}
					}
				}
			} else {
				//Gets action dependence.
				ComplexDependence actionDependence = action.Dependence;
				//Creates an auxiliar variable.
				int resultCheckDependence = 0;
				//Creates an auxiliar list in which will be saved the dependencies with errors.
				List<Dependence> dependenceErrors = new List<Dependence>();
				//Checks if the action has a dependence.
				if (actionDependence != null) {
					//Checks the dependence type.
					if (actionDependence.GetType () == typeof(SeqComplexDependence))
						resultCheckDependence = CheckSeqComplexDependencies ((SeqComplexDependence)actionDependence, action.Key, 
							studentLog, domain, ref dependenceErrors);
					else if (actionDependence.GetType () == typeof(OptComplexDependence))
						resultCheckDependence = CheckOptComplexDependencies ((OptComplexDependence)actionDependence, action.Key, 
							studentLog, domain, ref dependenceErrors);
					//Checks the result.
					if (resultCheckDependence == 0) {
						//Creates the log.
						_instanceStudentControl.CreateDepErrorLog (action, domain, student, false,
							dependenceErrors.Last ());
						//Adds the error to the answer param.
						outputError.Add (dependenceErrors.Last ().DependenceError);
						//Returns the value.
						return 0;
					} else if (resultCheckDependence == -1) {
						//Sets the result.
						result = -1;
						//Creates a log for each dependence with an error.
						foreach (Dependence d in dependenceErrors) {
							//Creates the log.
							_instanceStudentControl.CreateDepErrorLog (action, domain, student, false,
								d);
						}
					}
				}
				//Checks if there are any incompatibility.
				if (action.Incompatibilities != null) {
					//Checks if an incompatibility of the action exist.
					foreach (Incompatibility i in action.Incompatibilities) {
						//Checks if the incompatibility was done before the action
						if (_instanceStudentControl.CheckLogActionOrder (domainKey, studentKey, 
							   i.IncompatibilityAction.Key, action.Key)) {
							//Checks if the incompatibility blocks.
							if (i.IncompatibilityError.IsBlock) {
								//Creates the log.
								_instanceStudentControl.CreateIncompErrorLog (action, domain, student, false, i);
								//Adds the error to the answer param.
								outputError.Add (i.IncompatibilityError);
								//Returns the value.
								return 0;
							} else {
								//Creates the log.
								_instanceStudentControl.CreateIncompErrorLog (action, domain, student, false, i);
								//Sets the result.
								result = -1;
							}
						}
					}
				}
				//Checks if the action blocks the object.
				if (action.LockObj)
					blockObject = true;
				//Check if the action validate the phase errors.
				if (action.ValidateErrors) {
					//Gets all errors done by the user previously.
					var errQuery = 
						from o in studentLog.Logs
							where o.Action.Phase == studentLog.CurrentPhase
						select o.Error;
					//Adds errors into the list that will be returned.
					foreach (Error e in errQuery.ToList()) {
						if (e != null)
							outputError.Add (e);
					}
				}
				//Checks if the action change the current phase.
				if (action.InitPhase)
					studentLog.CurrentPhase += 1;
				//Checks if the action has minimun and/or maximun time.
				if (action.MinTime > 0) {
					//Creates a thread to control the minimun timer.
					Thread minTimerThread = new Thread (new ParameterizedThreadStart(MinTimer));
					//Checks if student has an entry.
					if (_minTimerController.ContainsKey (student.Key)) {
						//Checks if a entry for the specific action already exists.
						if (_minTimerController.TryGetValue (student.Key, out dic)) {
							if (dic.ContainsKey (action.Key)) {
								//Get the error.
								Error error = _otherErrors ["actionalreadyperformed"];
								//Creates the log.
								_instanceStudentControl.CreateOtherErrorLog (action, domain, student, false, error);
								//Adds the error to the answer param.
								outputError.Add (error);
								//Returns the value.
								return 0;
							} else {
								//Adds a new entry to the dictionary.
								if (_minTimerController.TryGetValue (student.Key, out dic))
									dic.Add (action.Key, minTimerThread);
							}
						}
					} else {
						//Adds a new entry to the dictionary.
						_minTimerController.Add(student.Key, new Dictionary<string, Thread>());
						//Adds the thread to the dictionary.
						//Adds a new entry to the dictionary.
						if (_minTimerController.TryGetValue(student.Key, out dic))
							dic.Add (action.Key, minTimerThread);
					}
					//Starts the thread.
					ArrayList param = new ArrayList();
					param.Add (minTimerThread);
					param.Add (action);
					param.Add (domain);
					param.Add (student);
					minTimerThread.Start (param);
				}
				if (action.MaxTime > 0) {
					//Creates a thread to control the minimun timer.
					Thread maxTimerThread = new Thread (new ParameterizedThreadStart(MaxTimer));
					//Checks if student has an entry.
					if (_maxTimerController.ContainsKey (student.Key)) {
						//Checks if a entry for the specific action already exists.
						if (_maxTimerController.TryGetValue (student.Key, out dic)) {
							if (dic.ContainsKey (action.Key)) {
								//Get the error.
								Error error = _otherErrors ["actionalreadyperformed"];
								//Creates the log.
								_instanceStudentControl.CreateOtherErrorLog (action, domain, student, false, error);
								//Adds the error to the answer param.
								outputError.Add (error);
								//Returns the value.
								return 0;
							} else {
								//Adds a new entry to the dictionary.
								if (_maxTimerController.TryGetValue (student.Key, out dic))
									dic.Add (action.Key, maxTimerThread);
							}
						}
					} else {
						//Adds a new entry to the dictionary.
						_maxTimerController.Add(student.Key, new Dictionary<string, Thread>());
						//Adds the thread to the dictionary.
						//Adds a new entry to the dictionary.
						if (_maxTimerController.TryGetValue(student.Key, out dic))
							dic.Add (action.Key, maxTimerThread);
					}

					//Creates a thread to control the maximun timer.
					maxTimerThread = new Thread (new ParameterizedThreadStart(MaxTimer));
					//Starts the thread.
					ArrayList param = new ArrayList();
					param.Add (maxTimerThread);
					param.Add (action);
					param.Add (student);
					param.Add (action.MaxTime);
					//Starts the thread.
					maxTimerThread.Start (param);
				}
				//At this moment, the action has been successful. Then, the action will be registered.
				if (!action.CorrectiveAction && action.MinTime <= 0)
					_instanceStudentControl.CreateNoCorrectiveActionLog (action, domain, student, true);
				//Gets the previous action.
				ActionAplication previousAction = domain.GetPreviousAction(action.Key);
				//Checks if this is the first action done by the student.
				if (previousAction != null) {
					//Creates an auxiliar variable that will not use.
					Thread t;
					Dictionary<string, Thread> maxTDic;
					//Checks if the previous action created a maximun timer.
					if (_maxTimerController.TryGetValue (studentLog.Owner.Key, out maxTDic)) {
						if (!maxTDic.TryGetValue (previousAction.Key, out t)  && previousAction.MaxTime > 0) {
							//Generates a max time error.
							_instanceStudentControl.CreateMaxTimeErrorLog (previousAction, domain, studentLog.Owner, false, previousAction.MaxTime);
							//Checks if the dependence blocks or not.
							if (previousAction.MaxTimeError.IsBlock) {
								//The result will be zero.
								result = 0;
								//Adds the dependence into the list.
								outputError.Add (previousAction.MaxTimeError);
							} else {
								//The result will be -1.
								result = -1;
							}
						}
					}
				}
			}

			//Returns.
			return result;
		}

		/// <summary>
		/// Function that controls minimun timer.
		/// </summary>
		/// <param name="second">Second.</param>
		private static void MinTimer (object data)
		{
			ArrayList param = data as ArrayList;
			Thread t = param [0] as Thread;
			ActionAplication action = param [1] as ActionAplication;
			DomainActions domain = param [2] as DomainActions;
			Student student = param [3] as Student;
			//Sleeps the thread during the time the action required.
			Thread.Sleep (action.MinTime * 1000);
			//Registers the action log.
			_instanceStudentControl.CreateNoCorrectiveActionLog (action, domain, student, true);
			//Gets the thread and removes it.
			Dictionary<string, Thread> minTDic;
			if (_maxTimerController.TryGetValue (student.Key, out minTDic))
				minTDic.Remove (action.Key);
		}

		/// <summary>
		/// Function that controls maximun timer.
		/// </summary>
		/// <param name="second">Second.</param>
		private static void MaxTimer (object data)
		{
			ArrayList param = data as ArrayList;
			Thread t = param [0] as Thread;
			ActionAplication action = param [1] as ActionAplication;
			Student student = param [2] as Student;
			int second = (int)param [3];
			//Sleeps the thread during the time the action required.
			Thread.Sleep (second * 1000);
			//Gets the thread and removes it.
			Dictionary<string, Thread> maxTDic;
			if (_maxTimerController.TryGetValue (student.Key, out maxTDic))
				maxTDic.Remove (action.Key);
		}

		/// <summary>
		/// Checks the opt complex dependencies.
		/// </summary>
		/// <returns>Integer that indicates: 0 if there has been a blocking error, -1 if there has been a no
		/// blocking error and 1 if there has not been any error.</returns>
		/// <param name="complexDependence">Complex dependence.</param>
		/// <param name="action">Action.</param>
		/// <param name="studentLog">Student log.</param>
		/// <param name="errors">Errors.</param>
		private int CheckOptComplexDependencies (OptComplexDependence complexDependence, string actionKey,
			StudentLog studentLog, DomainActions domain, ref List<Dependence> errors)
		{
			//Creates an auxiliar variable.
			int result = 2;
			//Checks each dependence.
			foreach (Dependence d in complexDependence.DependenciesList) {
				//Checks the dependence type.
				if (d.GetType () == typeof(SeqComplexDependence)) {
					//Calls the dependence check method.
					result = CheckSeqComplexDependencies ((SeqComplexDependence)d, actionKey, studentLog, domain, ref errors);
					//Checks the result of the call.
					if (result == 0) {
						//Adds the dependence into the list.
						errors.Add (d);
						//Breacks the loop.
						break;
					}
				} else if (d.GetType () == typeof(SimpleDependence)) {
					//Converts the Dependence to SimpleDependence.
					SimpleDependence dep = d as SimpleDependence;
					//Checks if the dependence action was done previously.
					if (studentLog.CheckActionDone (dep.ActionDependence.Key) == false) {
						//Creates an auxiliar variable that will not use.
						Thread t;
						//Creates an auxiliar variable.
						Dictionary<string, Thread> dic;
						//Checks if there is a timer for this action.
						if (_minTimerController.TryGetValue (studentLog.Owner.Key, out dic)) {
							if (dic.TryGetValue (dep.ActionDependence.Key, out t)) {
								//Generates a min time error.
								_instanceStudentControl.CreateMinTimeErrorLog (dep.ActionDependence, domain, studentLog.Owner, false, dep.ActionDependence.MinTime);
								//Checks if the dependence blocks or not.
								if (dep.ActionDependence.MinTimeError.IsBlock) {
									//The result will be zero.
									result = 0;
									//Adds the dependence into the list.
									errors.Add (dep);
									//Breacks the loop.
									break;
								} else {
									//The result will be -1.
									result = -1;
								}
							} else {
								//Checks if the dependence blocks or not.
								if (d.DependenceError.IsBlock) {
									//The result will be zero.
									result = 0;
									//Adds the dependence into the list.
									errors.Add (d);
									//Breacks the loop.
									break;
								} else {
									//The result will be -1.
									result = -1;
								}
							}
						} else {
							//Checks if the dependence blocks or not.
							if (d.DependenceError.IsBlock) {
								//The result will be zero.
								result = 0;
								//Adds the dependence into the list.
								errors.Add (d);
								//Breacks the loop.
								break;
							} else {
								//The result will be -1.
								result = -1;
							}
						}
					} else {
						//Checks if the result has just been modified by a previous dependence.
						if (result > 1)
							//There has not been a previous dependence. The result will be one,
							//until there will be an error.
							result = 1;
					}
				} 
			}
			//Returns the value.
			return result;
		}

		/// <summary>
		/// Checks the seq complex dependencies.
		/// </summary>
		/// <returns>Integer that indicates: 0 if there has been a blocking error, -1 if there has been a no
		/// blocking error and 1 if there has not been any error.</returns>
		/// <param name="complexdependence">Complexdependence.</param>
		/// <param name="actionKey">Action key.</param>
		/// <param name="studentLog">Student log.</param>
		/// <param name="errors">Errors.</param>
		private int CheckSeqComplexDependencies (SeqComplexDependence complexDependence,string actionKey,
			StudentLog studentLog, DomainActions domain, ref List<Dependence> errors)
		{
			//Creates an auxiliar int variable.
			int result = 2;
			//Creates an auxiliar dependence variable.
			Dependence prevDepencence = null;
			//Checks each dependence.
			foreach (Dependence d in complexDependence.DependenciesList) {
				//Checks the dependence type.
				if (d.GetType () == typeof(OptComplexDependence)) {
					//Calls the dependence check method.
					result = CheckOptComplexDependencies ((OptComplexDependence)d, actionKey, studentLog, domain, ref errors);
					//Checks the result of the call.
					if (result == 0) {
						//Adds the dependence into the list.
						errors.Add (d);
						//Breacks the loop.
						break;
					} else {
						//Checks the previous dependence type.
						if (prevDepencence.GetType () == typeof(OptComplexDependence)) {
							//Gets the specific previous dependence type.
							OptComplexDependence prevDep = prevDepencence as OptComplexDependence;
							//Creates an auxiliar variable.
							bool checkResult = false;
							//Checks each previous dependence with all dependence in current seqDependence.
							foreach (Dependence dd in prevDep.DependenciesList) {
								//Calls CheckDependenceOrder method.
								checkResult = CheckDependenceOrder(dd, d, studentLog);
								//Breaks if the checkResult is false.
								if (checkResult == false) {
									//Breacks the loop.
									break;
								}
							}
							//Checks the result.
							if (checkResult == false) {
								//Checks if the dependence blocks.
								if (complexDependence.DependenceError.IsBlock == true)
									//The result will be zero.
									result = 0;
								else
									//The result will be zero.
									result = -1;
								//Adds the dependence into the list.
								errors.Add (complexDependence);
								//Breacks the loop.
								break;
							}
						} else if (prevDepencence.GetType () == typeof(SimpleDependence)) {
							//Creates an auxiliar variable.
							bool checkResult = false;
							//Calls CheckDependenceOrder method.
							checkResult = CheckDependenceOrder(prevDepencence, d, studentLog);
							//Breaks if the checkResult is false.
							if (checkResult == false) {
								//Checks if the dependence blocks.
								if (complexDependence.DependenceError.IsBlock == true)
								//The result will be zero.
								result = 0;
								else
								//The result will be zero.
								result = -1;
								//Adds the dependence into the list.
								errors.Add (complexDependence);
								//Breacks the loop.
								break;
							}
						}
					}
				} else if (d.GetType () == typeof(SimpleDependence)) {
					//Converts the Dependence to SimpleDependence.
					SimpleDependence dep = d as SimpleDependence;
					//Checks if the dependence action was done previously.
					if (studentLog.CheckActionDone (dep.ActionDependence.Key) == false) {
						//Creates an auxiliar variable that will not use.
						Thread t;
						//Creates an auxiliar variable.
						Dictionary<string, Thread> dic;
						//Checks if there is a timer for this action.
						if (_minTimerController.TryGetValue (studentLog.Owner.Key, out dic)) {
							if (dic.TryGetValue (dep.ActionDependence.Key, out t)) {
								//Generates a min time error.
								_instanceStudentControl.CreateMinTimeErrorLog (dep.ActionDependence, domain, studentLog.Owner, false, dep.ActionDependence.MinTime);
								//Checks if the dependence blocks or not.
								if (dep.ActionDependence.MinTimeError.IsBlock) {
									//The result will be zero.
									result = 0;
									//Adds the dependence into the list.
									errors.Add (dep);
									//Breacks the loop.
									break;
								} else {
									//The result will be -1.
									result = -1;
								}
							} else {
								//Checks if the dependence blocks or not.
								if (d.DependenceError.IsBlock) {
									//The result will be zero.
									result = 0;
									//Adds the dependence into the list.
									errors.Add (d);
									//Breacks the loop.
									break;
								} else {
									//The result will be -1.
									result = -1;
								}
							}
						} else {
							//Check if the dependence blocks or not.
							if (d.DependenceError.IsBlock) {
								//The result will be zero.
								result = 0;
								//Adds the dependence into the list.
								errors.Add (d);
								//Breacks the loop.
								break;
							} else {
								//The result will be -1.
								result = -1;
							}
						}
					} else {
						//Checks if the prevDependence is null, and the dependence is the first dependence.
						if (prevDepencence != null) {
							//Creates an auxiliar variable.
							bool checkResult = false;
							//Calls CheckDependenceOrder method.
							checkResult = CheckDependenceOrder (prevDepencence, d, studentLog);
							//Breaks if the checkResult is false.
							if (checkResult == false) {
								//Checks if the dependence blocks.
								if (complexDependence.DependenceError.IsBlock == true)
								//The result will be zero.
								result = 0;
								else
								//The result will be zero.
								result = -1;
								//Adds the dependence into the list.
								errors.Add (complexDependence);
								//Breacks the loop.
								break;
							}
						}
						//Checks if the result has just been modified by a previous dependence.
						if (result > 1)
							//There has not been a previous dependence. The result will be one,
							//until there will be an error.
							result = 1;
					}
				}
				prevDepencence = d;
			}
			//Returns the value.
			return result;
		}

		/// <summary>
		/// Checks the dependence order.
		/// </summary>
		/// <returns><c>true</c>, if dependence1 was done previously to dependence2, <c>false</c> otherwise.</returns>
		/// <param name="dependence1">Dependence1.</param>
		/// <param name="dependence2">Dependence2.</param>
		/// <param name="studentLog">Student log.</param>
		private bool CheckDependenceOrder (Dependence dependence1, Dependence dependence2, StudentLog studentLog)
		{
			//Creates the variable will be returned.
			bool checkResult = false;
			//Checks the dependence1 type.
			if (dependence1.GetType () == typeof(SeqComplexDependence)) {
				//Creates an auxiliar variable.
				SeqComplexDependence dep = dependence1 as SeqComplexDependence;
				//Checks each dependence.
				foreach (Dependence d in dep.DependenciesList) {
					//Calls CheckDependenceOrder method.
					checkResult = CheckDependenceOrder(d, dependence2, studentLog);
					//Breaks if the checkResult is false.
					if (checkResult == false)
						break;
				}
			} else if (dependence1.GetType() == typeof(OptComplexDependence)) {
				//Creates an auxiliar variable.
				OptComplexDependence dep = dependence1 as OptComplexDependence;
				//Checks each dependence.
				foreach (Dependence d in dep.DependenciesList) {
					//Calls CheckDependenceOrder method.
					checkResult = CheckDependenceOrder(d, dependence2, studentLog);
					//Breaks if the checkResult is false.
					if (checkResult == false)
						break;
				}
			} else if (dependence1.GetType() == typeof(SimpleDependence)) {
				SimpleDependence dep1 = dependence1 as SimpleDependence;
				//Checks the type of the dependence.
				if (dependence2.GetType () == typeof(SeqComplexDependence)) {
					//Creates an auxiliar variable.
					SeqComplexDependence dep = dependence2 as SeqComplexDependence;
					//Checks each dependence.
					foreach (Dependence d in dep.DependenciesList) {
						if (dependence2.GetType () == typeof(SeqComplexDependence)) {
							//Calls CheckDependenceOrder method.
							checkResult = CheckDependenceOrder(dependence1, d, studentLog);
							//Breaks if the checkResult is false.
							if (checkResult == false)
								break;
						} else if (dependence2.GetType () == typeof(OptComplexDependence)) {
							//Calls CheckDependenceOrder method.
							checkResult = CheckDependenceOrder(dependence1, d, studentLog);
							//Breaks if the checkResult is false.
							if (checkResult == false)
								break;
						} else if (dependence2.GetType () == typeof(SimpleDependence)) {
							//Creates an auxiliar variable.
							SimpleDependence dep2 = dependence2 as SimpleDependence;
							//Calls the CheckActionOrder method.
							checkResult = studentLog.CheckActionOrder(dep1.ActionDependence.Key, dep2.ActionDependence.Key);
							//Breaks if the checkResult is false.
							if (checkResult == false)
								break;
						}
					}
				} else if (dependence2.GetType () == typeof(OptComplexDependence)) {
					//Creates an auxiliar variable.
					OptComplexDependence dep = dependence2 as OptComplexDependence;
					//Checks each dependence.
					foreach (Dependence d in dep.DependenciesList) {
						if (dependence2.GetType () == typeof(SeqComplexDependence)) {
							//Calls CheckDependenceOrder method.
							checkResult = CheckDependenceOrder(dependence1, d, studentLog);
							//Breaks if the checkResult is false.
							if (checkResult == false)
								break;
						} else if (dependence2.GetType () == typeof(OptComplexDependence)) {
							//Calls CheckDependenceOrder method.
							checkResult = CheckDependenceOrder(dependence1, d, studentLog);
							//Breaks if the checkResult is false.
							if (checkResult == false)
								break;
						} else if (dependence2.GetType () == typeof(SimpleDependence)) {
							//Creates an auxiliar variable.
							SimpleDependence dep2 = dependence2 as SimpleDependence;
							//Calls the CheckActionOrder method.
							checkResult = studentLog.CheckActionOrder(dep1.ActionDependence.Key, dep2.ActionDependence.Key);
							//Breaks if the checkResult is false.
							if (checkResult == false)
								break;
						}
					}
				} else if (dependence2.GetType () == typeof(SimpleDependence)) {
					//Creates an auxiliar variable.
					SimpleDependence dep2 = dependence2 as SimpleDependence;
					//Calls the CheckActionOrder method.
					checkResult = studentLog.CheckActionOrder(dep1.ActionDependence.Key, dep2.ActionDependence.Key);
				}
			}

			//Returns the result.
			return checkResult;
   		}

		/// <summary>
		/// Gets the action by the name.
		/// </summary>
		/// <returns>The action by name.</returns>
		/// <param name="domain">Domain.</param>
		/// <param name="actionName">Action name.</param>
		/// <param name="studentKey">Student key.</param>
		public ActionAplication GetActionByName (string domainKey, string actionName, string studentKey)
		{
			//Gets the StudentLog of the given student for the specific domain.
			StudentLog studentLog = _instanceStudentControl.GetStudentLog (domainKey, studentKey);
			//Gets the DomainActions.
			DomainActions domain = _domainActionsList[domainKey];
			//Creates an auxiliar variable.
			ActionAplication action;
			//Search action with the given name in the current phase of the student.
			var actQuery = 
				from o in domain.Actions
					where o.Name == actionName && o.Phase == studentLog.CurrentPhase
				select o;
			//Checks if the action exist.
			if (actQuery.ToList ().Count > 0) {
				action = actQuery.First ();
				//Returns the action.
				return action;
			} else {
				throw new ArgumentException ("The action does not exist.");
			}
		}

		/// <summary>
		/// Gets the domain actions.
		/// </summary>
		/// <returns>The domain actions.</returns>
		/// <param name="domainKey">Domain key.</param>
		public DomainActions GetDomainActions (string domainKey)
		{
			//Returns the result.
			DomainActions value = null;
			if (!_domainActionsList.TryGetValue(domainKey, out value))
			{
				throw new ArgumentException ("There is not any Domain with the given key: \"" + domainKey + "\"");	
			}
			
			return value;
		}
	}
}