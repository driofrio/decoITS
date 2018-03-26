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

	    private ValidationHelper _valiationHelper;

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
		    
		    _valiationHelper = new ValidationHelper(_worldControl, _studentControl, _expertControl);
	    }
	    
	    public AbstractTutor(string ontologyPath, string logsPath, string expertConfPath, string worldConfPath, Dictionary<string, WorldControl> worldControl, 
		    ExpertControl expertControl, StudentControl studentControl, ValidationHelper validationHelper, bool master)
	    {
		    _master = master;
		    _ontologyPath = ontologyPath;
		    _logsPath = logsPath;
		    _expertConfPath = expertConfPath;
		    _worldConfPath = worldConfPath;
		    _worldControl = worldControl;
		    _expertControl = expertControl;
		    _studentControl = studentControl;
		    _valiationHelper = validationHelper;
	    }
	    
        abstract public int ToTutor(string actionName, string domainName, string studentKey, string objectName,
            out List<string> messages);
        
        /// <summary>
		/// Validates the action. If Tutor calling this method is "master" entity, then actual validation is performed.
		/// Otherwise, results of the last validated action are returned (on assumption that master entity has already performed validation).
		/// </summary>
		/// <returns>The action.</returns>
		/// <param name="actionName">Action name.</param>
		/// <param name="domainName">Domain name.</param>
		/// <param name="studentKey">Student key.</param>
		/// <param name="nameObject">Name object.</param>
		/// <param name="outputError">Output error.</param>
		protected int ValidateAction (string actionName, string domainName, string studentKey, string objectName, out List<Error> outputError)
        {
	        int result;
	        
			if (_master)
			{
				result = _valiationHelper.ValidateAction(actionName, domainName, studentKey, objectName, out outputError);
			}
			else
			{
				result = _valiationHelper.GetLastValidationResults(out outputError);
			}

	        return result;
        }

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
			_expertControl.CreateDomain (key);
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
			_studentControl.ResetPractice (domainName, studentKey);
		}
    }
}
