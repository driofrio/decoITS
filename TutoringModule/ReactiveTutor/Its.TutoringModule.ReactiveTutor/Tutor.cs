using System.Collections.Generic;
using Its.ExpertModule;
using Its.StudentModule;
using Its.TutoringModule.Common;
using Its.TutoringModule.ReactiveTutor.ObjectModel;
using Its.Utils.Config;
using Its.WorldModule;

namespace Its.TutoringModule.ReactiveTutor
{
	public class Tutor : AbstractTutor
	{

		public Tutor(string domainKey, ITutorConfig config, bool master) : base(domainKey, config, master)
		{
			// Base constructor
		}

		public Tutor(string ontologyPath, string logsPath, string expertConfPath, string worldConfPath,
			Dictionary<string, WorldControl> worldControl, ExpertControl expertControl, StudentControl studentControl, bool master)
			: base(ontologyPath, logsPath, expertConfPath, worldConfPath, worldControl, expertControl, studentControl, master)
		{
			// Base constructor
		}

		/// <summary>
		/// Does the tutoring for an action.
		/// </summary>
		/// <returns>The tutor.</returns>
		/// <param name="actionName">Action name.</param>
		/// <param name="domainName">Domain name.</param>
		/// <param name="studentKey">Student key.</param>
		/// <param name="nameObject">Name object.</param>
		public override int ToTutor (string actionName, string domainName, string studentKey, string objectName, out List<string> messages)
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
                if (okMessage != string.Empty)
                    messages.Add(okMessage);
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
                if (okMessage != string.Empty)
                    messages.Add(okMessage);
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
	}
}

