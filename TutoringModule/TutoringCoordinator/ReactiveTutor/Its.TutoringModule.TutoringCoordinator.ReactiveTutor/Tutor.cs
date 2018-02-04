using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Its.ExpertModule;
using Its.ExpertModule.ObjectModel;
using Its.StudentModule;
using Its.StudentModule.ObjectModel;
using Its.TutoringModule.TutoringCoordinator.ReactiveTutor.ObjectModel;
using Its.WorldModule;
using Its.Utils.Config;

namespace Its.TutoringModule.TutoringCoordinator.ReactiveTutor
{
	public class Tutor
	{
		/// <summary>
		/// The world control.
		/// </summary>
		private Dictionary<string, WorldControl> _worldControl;
		/// <summary>
		/// The student control.
		/// </summary>
		private StudentControl _studentControl;
		/// <summary>
		/// The expert control.
		/// </summary>
		private ExpertControl _expertControl;
		/// <summary>
		/// The ontology path.
		/// </summary>
		private string _ontologyPath;
		/// <summary>
		/// The logs path.
		/// </summary>
		private string _logsPath;
		/// <summary>
		/// The expert conf path.
		/// </summary>
		private string _expertConfPath;
		/// <summary>
		/// The world conf path.
		/// </summary>
		private string _worldConfPath;

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.TutoringModule.TutoringCoordinator.ReactiveTutor.ReactiveTutor"/> class.
		/// </summary>
		public Tutor (ITutorConfig config)
		{
			//Gets the configuration settings.
			_ontologyPath = config.OntologyPath.Replace ('\\', Path.DirectorySeparatorChar);
			_logsPath = config.LogsPath.Replace ('\\', Path.DirectorySeparatorChar);
			_expertConfPath = config.DomainConfigurationPath.Replace ('\\', Path.DirectorySeparatorChar);
			_worldConfPath = config.WorldConfigurationPath.Replace ('\\', Path.DirectorySeparatorChar);

			//Initializes the dictionary.
			_worldControl = new Dictionary<string, WorldControl> ();
			//Gets the ExpertControl instance.
			_expertControl = ExpertControl.Instance(_ontologyPath, _logsPath, _expertConfPath);
		}

		/// <summary>
		/// Does the tutoring for an action.
		/// </summary>
		/// <returns>The tutor.</returns>
		/// <param name="actionName">Action name.</param>
		/// <param name="domainName">Domain name.</param>
		/// <param name="studentKey">Student key.</param>
		/// <param name="nameObject">Name object.</param>
		public int ToTutor (string actionName, string domainName, string studentKey, string objectName, out List<string> messages)
		{
			//Creates a list for the errors.
			List<Error> errorList;
			//Creates a list for tutor messages.
			List<TutorMessage> tutorMessages;
			//Creates a list for the message.
			messages = new List<string>() ;
			//Creates a string for correct message.
			string okMessage;
			//Calls Validation Method.
			int result = ValidateAction(actionName, domainName, studentKey, objectName, out errorList);
			//Checks the result.
			switch (result) {
			//No valido bloqueante
			case 0:
				//Adds the error message to the list.
				foreach (Error e in errorList)
					messages.Add (e.Message.Message);
				break;
			//Valido
			case 1:
				//Calls the GetMessages method.
				GetMessages (actionName, domainName, studentKey, out tutorMessages, out okMessage);
				//Adds into the list that will be returned.
				messages.Add (okMessage);
				//Adds the tutor messages if they exist.
				if (tutorMessages.Count > 0)
					foreach (TutorMessage tm in tutorMessages)
						messages.Add (tm.Message);
				//Adds the error message to the list, whether they exist.
				if (errorList.Count > 0)
					foreach (Error e in errorList)
						messages.Add (e.Message.Message);
				break;
			//No valido no bloqueante
			case -1:
				//Calls the GetMessages method.
				GetMessages (actionName, domainName, studentKey, out tutorMessages, out okMessage);
				//Adds into the list that will be returned.
				messages.Add (okMessage);
				//Adds the tutor messages if they exist.
				if (tutorMessages.Count > 0)
					foreach (TutorMessage tm in tutorMessages)
						messages.Add (tm.Message);
				//Adds the error message to the list, whether they exist.
				if (errorList.Count > 0)
					foreach (Error e in errorList)
						messages.Add (e.Message.Message);
				break;
			}

			//Returns the result.
			return result;
		}

		/// <summary>
		/// Loads the domain.
		/// </summary>
		/// <param name="key">Key.</param>
		public void LoadDomain (string key)
		{
			//Creates the domain from ExpertoControl.
			_expertControl.CreateDomain (key);
			//Loads domain logs for this domain.
			_studentControl.GetDomainLogsFromOntology(_expertControl.GetDomainActions(key), _expertControl.OtherErrors, _worldControl[key].WorldErrors);
		}

		/// <summary>
		/// Loads the students.
		/// </summary>
		public void LoadStudents ()
		{
			//Gets the StudentControl instance.
			_studentControl = StudentControl.Instance(_ontologyPath, _logsPath);
		}

		/// <summary>
		/// Loads the world.
		/// </summary>
		/// <param name="virtualEnvironmentKey">Virtual environment key.</param>
		public void LoadWorld (string virtualEnvironmentKey)
		{
			//Creates a new WorldControl to the dictionary.
			WorldControl worldControl = WorldControl.Instance(_ontologyPath, _logsPath + Path.DirectorySeparatorChar + virtualEnvironmentKey);
			//Creates world objects data.
			worldControl.CreateWorldData(virtualEnvironmentKey, _worldConfPath);
			//Adds a new WorldControl to the dictionary.
			_worldControl.Add(virtualEnvironmentKey, worldControl);
		}

		/// <summary>
		/// Creates the student.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="middleName">Middle name.</param>
		/// <param name="surname">Surname.</param>
		/// <param name="lastName">Last name.</param>
		public void CreateStudent (string key, string name, string middleName, string surname, string lastName)
		{
			//Creates the student with the given information.
			_studentControl.CreateStudent (key, name, middleName, surname, lastName);
		}

		/// <summary>
		/// Generate a world error log.
		/// </summary>
		/// <returns>The error.</returns>
		/// <param name="domain">Domain.</param>
		/// <param name="actionName">Action name.</param>
		/// <param name="studentKey">Student key.</param>
		/// <param name="type">Type.</param>
		/// <param name="nameObject">Name object.</param>
		public string WorldError (string domainKey, string actionName, string studentKey, string type, List<string> nameObject)
		{
			//Gets the WorldControl with the given domain.
			WorldControl worldControl;
			if (_worldControl.TryGetValue (domainKey, out worldControl)) {
				//Creates the world error.
				Error worldError = worldControl.GetWorldError(type, nameObject);
				//Gets the ActionAplication with the given name.
				ActionAplication action = _expertControl.GetActionByName(domainKey, actionName, studentKey);
				//Gets the DomainActions with the given key.
				DomainActions domain = _expertControl.GetDomainActions(domainKey);
				//Gets the student with the given key.
				Student student = _studentControl.GetStudent(studentKey);
				//Generates the world error log.
				_studentControl.CreateWorldErrorLog(action, domain, student, false, worldError, type);
				//Returns the world error message.
				return worldError.Message.Message;
			} else {
				throw new ArgumentException ("The domain given does not exist.");
			}
		}

		/// <summary>
		/// Validates the action.
		/// </summary>
		/// <returns>The action.</returns>
		/// <param name="actionName">Action name.</param>
		/// <param name="domainName">Domain name.</param>
		/// <param name="studentKey">Student key.</param>
		/// <param name="nameObject">Name object.</param>
		/// <param name="outputError">Output error.</param>
		private int ValidateAction (string actionName, string domainName, string studentKey, string objectName, out List<Error> outputError)
		{
			//Creates a bool variable to get if the action block or not the object.
			bool blockObj;
			//Calls the expert action validation and gets the result.
			int result = _expertControl.ActionValidation (actionName, domainName, studentKey, objectName,out outputError,out blockObj);
			//Checks if the action blocks or not the object.
			if (blockObj == true) {
				//Gets the student with the given key.
				Student student = _studentControl.GetStudent (studentKey);
				//Validates if the object is block.
				bool blockRes = _worldControl[domainName].ObjectBlockValidate(objectName, student);
				//Checks the value returned.
				if (blockRes == true) {
					//Creates a WorldError.
					List<string> objectNameList = new List<string>();
					objectNameList.Add (objectName);
					Error worldError = _worldControl [domainName].GetWorldError ("objectblocked", objectNameList);
					//Gets the action aplication.
					ActionAplication action = _expertControl.GetActionByName(domainName, actionName, studentKey);
					//Gets the domain.
					DomainActions domain = _expertControl.GetDomainActions(domainName);
					//Registers the error.
					_studentControl.CreateWorldErrorLog(action, domain, student, false, worldError, "objectblocked");
				} else {
					//Blocks the object.
					_worldControl[domainName].BlockObject(objectName, student);
				}
			}
			//Returns the result.
			return result;
		}

		/// <summary>
		/// Gets the messages.
		/// </summary>
		/// <param name="actionName">Action name.</param>
		/// <param name="domainName">Domain name.</param>
		/// <param name="studentKey">Student key.</param>
		/// <param name="tutorMessage">Tutor message.</param>
		/// <param name="actionOrErrorMessage">Action or error message.</param>
		private void GetMessages (string actionName, string domainName, string studentKey, out List<TutorMessage> tutorMessage, out string okMessage)
		{
			//Calls the expertControl GetMessages method.
			_expertControl.GetMessages (actionName, domainName, studentKey, out okMessage, out tutorMessage); 
		}

		/// <summary>
		/// Erases the action.
		/// </summary>
		/// <param name="actionName">Action name.</param>
		/// <param name="domainName">Domain name.</param>
		/// <param name="studentKey">Student key.</param>
		public void EraseAction (string actionKey, string domainName, string studentKey)
		{
			//Erases the action.
			_studentControl.EraseAction(actionKey, domainName, studentKey);
		}

		/// <summary>
		/// Resets the practice.
		/// </summary>
		/// <param name="domainName">Domain name.</param>
		/// <param name="studentKey">Student key.</param>
		public void ResetPractice (string domainName, string studentKey)
		{
			//Resets the practice.
			_studentControl.ResetPractice (domainName, studentKey);
		}
	}
}

