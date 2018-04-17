using System;
using System.Collections.Generic;
using System.IO;
using Its.ExpertModule;
using Its.ExpertModule.ObjectModel;
using Its.StudentModule;
using Its.StudentModule.ObjectModel;
using Its.TutoringModule.ReactiveTutor.ObjectModel;
using Its.Utils.Config;
using Its.WorldModule;

namespace Its.TutoringModule.Common
{
    public abstract class AbstractTutor : ITutor
    {
	    /// <summary>
	    /// The world control.
	    /// </summary>
	    protected Dictionary<string, WorldControl> _worldControl;
	    /// <summary>
	    /// The student control.
	    /// </summary>
	    protected StudentControl _studentControl;
	    /// <summary>
	    /// The expert control.
	    /// </summary>
	    protected ExpertControl _expertControl;
	    /// <summary>
	    /// The ontology path.
	    /// </summary>
	    protected string _ontologyPath;
	    /// <summary>
	    /// The logs path.
	    /// </summary>
	    protected string _logsPath;
	    /// <summary>
	    /// The expert conf path.
	    /// </summary>
	    protected string _expertConfPath;
	    /// <summary>
	    /// The world conf path.
	    /// </summary>
	    protected string _worldConfPath;
	    /// <summary>
	    /// Determines whether current instance of tutor is "master".
	    /// In setups where a coordinator with several tutors is used, coordinator is marked as master 
	    /// and other tutors become "slaves". Amongst other things, master tutor is responsible
	    /// for maintaining single true copy of action logs.
	    /// </summary>
	    protected bool _master;

	    protected ITutorConfig _config;

	    /// <summary>
	    /// Result of the last call to Validate() method. Used by concrete tutors to determine tutoring strategy.
	    /// </summary>
	    protected int lastValidationResult;

	    public AbstractTutor(string domainKey, ITutorConfig config, bool master)
	    {
		    _master = master;
		    //Gets the configuration settings.
		    _ontologyPath = config.OntologyPath.Replace ('\\', Path.DirectorySeparatorChar);
		    _logsPath = config.LogsPath.Replace ('\\', Path.DirectorySeparatorChar);
		    _expertConfPath = config.DomainConfigurationPath.Replace ('\\', Path.DirectorySeparatorChar);
		    _worldConfPath = config.WorldConfigurationPath.Replace ('\\', Path.DirectorySeparatorChar);

		    //Initializes the dictionary.
		    _worldControl = new Dictionary<string, WorldControl> ();
		    //Gets the ExpertControl instance.
		    _expertControl = ExpertControl.Instance(_ontologyPath, _logsPath, _expertConfPath, 
			    config.InitialColumn, config.InitialRow);

		    if (_master)
		    {
			    Init(domainKey);
		    }

		    _config = config;
	    }
	    
	    public AbstractTutor(string ontologyPath, string logsPath, string expertConfPath, string worldConfPath, Dictionary<string, WorldControl> worldControl, 
		    ExpertControl expertControl, StudentControl studentControl, ITutorConfig config, bool master)
	    {
		    _master = master;
		    _ontologyPath = ontologyPath;
		    _logsPath = logsPath;
		    _expertConfPath = expertConfPath;
		    _worldConfPath = worldConfPath;
		    _worldControl = worldControl;
		    _expertControl = expertControl;
		    _studentControl = studentControl;
		    _config = config;
	    }
	    
//        abstract public int ToTutor(string actionName, string domainName, string studentKey, string objectName,
//            out Dictionary<string, List<string>> messages);
//        
//        /// <summary>
//		/// Validates the action. If Tutor calling this method is "master" entity, then actual validation is performed.
//		/// Otherwise, results of the last validated action are returned (on assumption that master entity has already performed validation).
//		/// </summary>
//		/// <returns>The action.</returns>
//		/// <param name="actionName">Action name.</param>
//		/// <param name="domainName">Domain name.</param>
//		/// <param name="studentKey">Student key.</param>
//		/// <param name="nameObject">Name object.</param>
//		/// <param name="outputError">Output error.</param>
//		protected int ValidateAction (string actionName, string domainName, string studentKey, string objectName, out List<Error> outputError)
//        {
//	        int result;
//	        
//			if (_master)
//			{
//				result = _valiationHelper.ValidateAction(actionName, domainName, studentKey, objectName, out outputError);
//			}
//			else
//			{
//				result = _valiationHelper.GetLastValidationResults(out outputError);
//			}
//
//	        return result;
//        }

	    private void Init(string key)
	    {
		    LoadStudents();
		    LoadWorld(key);
		    LoadDomain(key);
	    }

        /// <summary>
		/// Loads the domain.
		/// </summary>
		/// <param name="key">Key.</param>
		private void LoadDomain (string key)
		{
			//Creates the domain from ExpertoControl.
			_expertControl.CreateDomainIfDoesntExist(key);
			//Loads domain logs for this domain.
			_studentControl.GetDomainLogsFromOntology(_expertControl.GetDomainActions(key), _expertControl.OtherErrors, _worldControl[key].WorldErrors);
		}

		/// <summary>
		/// Loads the students.
		/// </summary>
		private void LoadStudents ()
		{
			//Gets the StudentControl instance.
			_studentControl = StudentControl.Instance(_ontologyPath, _logsPath, _expertConfPath);
		}

		/// <summary>
		/// Loads the world.
		/// </summary>
		/// <param name="key">Virtual environment key.</param>
		private void LoadWorld (string key)
		{
			//Creates a new WorldControl to the dictionary.
			WorldControl worldControl = WorldControl.Instance(_ontologyPath, _logsPath + Path.DirectorySeparatorChar + key);
			//Creates world objects data.
			worldControl.CreateWorldData(key, _worldConfPath);
			//Adds a new WorldControl to the dictionary.
			_worldControl.Add(key, worldControl);
		}

	    public int Validate(string actionName, string domainName, string studentKey, string objectName, out List<string> errorMessages)
	    {
		    //Creates the result variable.
		    int result = 1;
		    //Creates a bool variable to get if the action block or not the object.
		    bool blockObj;
		    //Gets the student with the given key.
		    Student student = _studentControl.GetStudent (studentKey);
		    //Validates if the object is block.
		    bool blockRes = _worldControl[domainName].ObjectBlockValidate(objectName, student);
		    //Checks the value returned.
		    if (blockRes == true) {
			    //Creates a WorldError.
			    List<string> objectNameList = new List<string> ();
			    objectNameList.Add (objectName);
			    Error worldError = _worldControl [domainName].GetWorldError ("objectblocked", objectNameList);
			    //Gets the action aplication.
			    ActionAplication action = _expertControl.GetActionByName (domainName, actionName, studentKey);
			    //Gets the domain.
			    DomainActions domain = _expertControl.GetDomainActions (domainName);
			    //Registers the error.
			    _studentControl.CreateWorldErrorLog (action, domain, student, false, worldError, "objectblocked");
			    //Adds the error into the list.
			    errorMessages = new List<string>();
			    errorMessages.Add(worldError.Message.Message);
			    //Sets the result.
			    result = 0;
		    } else {
			    //Calls the expert action validation and gets the result.
			    List<Error> errors;
			    result = _expertControl.ActionValidation (actionName, domainName, studentKey, objectName, out errors, out blockObj);
			    //Checks if the action blocks or not the object.
			    if (blockObj == true)
				    //Blocks the object.
				    _worldControl[domainName].BlockObject(objectName, student);

			    //Add errors to the list.
			    errorMessages = new List<string>();
			    foreach (Error err in errors)
			    {
				    errorMessages.Add(err.Message.Message);
			    }
		    }

		    this.lastValidationResult = result;
		    
		    //Returns the result.
		    return result;
	    }

	    public Dictionary<string, List<string>> GetTutorMessages(string actionName, string domainName, string studentKey)
	    {
		    // Call subclass method to obtain tutor-specific messages
		    Dictionary<string, List<string>> messages = GetTutoringStrategyMessages(actionName, domainName, studentKey);

		    // Ensure logs are flushed only once per action by the master tutor
		    if (_master)
		    {
			    // Finally flush LogEntries to ontology
			    DomainActions domain = _expertControl.GetDomainActions(domainName);
			    Student student = _studentControl.GetStudent(studentKey);
			    _studentControl.FlushLastActionLogs(domain, student);
		    }

		    return messages;
	    }

	    abstract protected Dictionary<string, List<string>> GetTutoringStrategyMessages(string actionName, string domainName, string studentKey);

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
            try
            {
                _studentControl.CreateStudent(key, name, middleName, surname, lastName);
            }
            catch
            {
                throw new Exception();
            }
			
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
			try
			{
				_studentControl.ResetPractice (domainName, studentKey);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				// Either domain or student with such key doesn't exist
			}
		}

	    /// <summary>
	    /// Helper method to populate message dictionary with an error message
	    /// </summary>
	    /// <param name="messages">Message dictionary.</param>
	    /// <param name="error">Error to add.</param>
	    protected void AddErrorMessage(ref Dictionary<string, List<string>> messages, Error error)
	    {
		    if (!messages.ContainsKey("errors"))
		    {
			    messages.Add("errors", new List<string>());
		    }
		    
		    messages["errors"].Add(error.Message.Message);
	    }
	    
	    /// <summary>
	    /// Helper method to populate message dictionary with multiple error messages in one go
	    /// </summary>
	    /// <param name="messages">Message dictionary.</param>
	    /// <param name="errors">Errors to add.</param>
	    protected void AddErrorMessages(ref Dictionary<string, List<string>> messages, List<Error> errors)
	    {
		    foreach (Error error in errors)
		    {
			    AddErrorMessage(ref messages, error);
		    }
	    }
	    
	    /// <summary>
	    /// Helper method to populate message dictionary with tutor message
	    /// </summary>
	    /// <param name="messages">Message dictionary.</param>
	    /// <param name="msg">Tutor message to add.</param>
	    protected void AddTutorMessage(ref Dictionary<string, List<string>> messages, TutorMessage msg)
	    {
		    if (!messages.ContainsKey("tutor"))
		    {
			    messages.Add("tutor", new List<string>());
		    }
		    
		    messages["tutor"].Add(msg.Message);
	    }
	    
	    /// <summary>
	    /// Helper method to populate message dictionary with multiple tutor message in one go
	    /// </summary>
	    /// <param name="messages">Message dictionary.</param>
	    /// <param name="tutorMessages">Tutor messages to add.</param>
	    protected void AddTutorMessages(ref Dictionary<string, List<string>> messages, List<TutorMessage> tutorMessages)
	    {   
		    foreach (TutorMessage msg in tutorMessages)
		    {
			    AddTutorMessage(ref messages, msg);
		    }
	    }
	    
	    /// <summary>
	    /// Helper method to populate message dictionary with confirmation message
	    /// </summary>
	    /// <param name="messages">Message dictionary.</param>
	    /// <param name="msg">Confirmation message to add.</param>
	    protected void AddConfirmationMessage(ref Dictionary<string, List<string>> messages, String msg)
	    {
		    if (!messages.ContainsKey("confirmation"))
		    {
			    messages.Add("confirmation", new List<string>());
		    }
		    
		    messages["confirmation"].Add(msg);
	    }
	    
	    /// <summary>
	    /// Helper method to populate message dictionary with error prevention message
	    /// </summary>
	    /// <param name="messages">Message dictionary.</param>
	    /// <param name="msg">Error prevention message to add.</param>
	    protected void AddErrorPreventionMessage(ref Dictionary<string, List<string>> messages, TutorMessage msg)
	    {
		    if (!messages.ContainsKey("errorPrevention"))
		    {
			    messages.Add("errorPrevention", new List<string>());
		    }
		    
		    messages["errorPrevention"].Add(msg.Message);
	    }
	    
	    /// <summary>
	    /// Helper method to populate message dictionary with multiple error prevention messages in one go
	    /// </summary>
	    /// <param name="messages">Message dictionary.</param>
	    /// <param name="errorPreventionMessages">Error prevention messages to add.</param>
	    protected void AddErrorPreventionMessages(ref Dictionary<string, List<string>> messages, List<TutorMessage> errorPreventionMessages)
	    {
		    foreach (TutorMessage msg in errorPreventionMessages)
		    {
			    AddErrorPreventionMessage(ref messages, msg);
		    }
	    }
    }
}
