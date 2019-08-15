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
			Dictionary<string, WorldControl> worldControl, ExpertControl expertControl, StudentControl studentControl, ITutorConfig config, bool master)
			: base(ontologyPath, logsPath, expertConfPath, worldConfPath, worldControl, expertControl, studentControl, config, master)
		{
			// Base constructor
		}

		protected override Dictionary<string, List<string>> GetTutoringStrategyMessages(string actionName, string domainName, string studentKey)
		{
			string okMessage;
			List<TutorMessage> tutorMessages;
			Dictionary<string, List<string>> messages = new Dictionary<string, List<string>>();
			
			//Checks the result.
			switch (lastValidationResult) {
				//No valido bloqueante
				case 0:
					// No tutoring provided if blocking error was made
					break;
				//Valido
				case 1:
					_expertControl.GetMessages(actionName, domainName, studentKey, out okMessage, out tutorMessages);
                
					//Adds into the list that will be returned.
					if (okMessage != string.Empty)
						AddConfirmationMessage(ref messages, okMessage);
				
					//Adds the tutor messages if they exist.
					if (tutorMessages.Count > 0)
						AddTutorMessages(ref messages, tutorMessages);
					
					break;
				//No valido no bloqueante
				case -1:
					_expertControl.GetMessages(actionName, domainName, studentKey, out okMessage, out tutorMessages);
                
					//Adds into the list that will be returned.
					if (okMessage != string.Empty)
						AddConfirmationMessage(ref messages, okMessage);
				
					//Adds the tutor messages if they exist.
					if (tutorMessages.Count > 0)
						AddTutorMessages(ref messages, tutorMessages);
				
					break;
			}

			return messages;
		}
	}
}

