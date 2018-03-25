using System.Collections.Generic;
using Its.ExpertModule;
using Its.TutoringModule.CMTutor;
using Its.TutoringModule.Common;
using Its.TutoringModule.ReactiveTutor;
using Its.TutoringModule.ReactiveTutor.ObjectModel;
using Its.WorldModule;

namespace Its.TutoringModule.TC
{
	public class TutoringCoordinator : AbstractTutor
	{
		private Tutor rTutor;
		private CollectiveModelTutor cmTutor;

		public TutoringCoordinator(Tutor rTutor, CollectiveModelTutor cmTutor, string ontologyPath, string logsPath, string expertConfPath, string worldConfPath, Dictionary<string, WorldControl> worldControl, ExpertControl expertControl)
			: base(true, ontologyPath, logsPath, expertConfPath, worldConfPath, worldControl, expertControl)
		{
			this.rTutor = rTutor;
			this.cmTutor = cmTutor;
		}

		public override int ToTutor(string actionName, string domainName, string studentKey, string objectName, out List<string> messages)
		{
			List<Error> errorList;
			ValidateAction(actionName, domainName, studentKey, objectName, out errorList);

			if (cmTutor.HasSupportForAction(actionName, domainName))
			{
				return cmTutor.ToTutor(actionName, domainName, studentKey, objectName, out messages);
			}
			else
			{
				return rTutor.ToTutor(actionName, domainName, studentKey, objectName, out messages);
			}
		}
	}
}

