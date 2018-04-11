using System.Collections.Generic;
using Its.ExpertModule;
using Its.StudentModule;
using Its.TutoringModule.CMTutor;
using Its.TutoringModule.Common;
using Its.TutoringModule.ReactiveTutor;
using Its.TutoringModule.ReactiveTutor.ObjectModel;
using Its.Utils.Config;
using Its.WorldModule;

namespace Its.TutoringModule.TC
{
	public class TutoringCoordinator : AbstractTutor
	{
		private Tutor rTutor;
		private CollectiveModelTutor cmTutor;

		public TutoringCoordinator(Tutor rTutor, CollectiveModelTutor cmTutor, string ontologyPath, string logsPath, string expertConfPath, string worldConfPath, Dictionary<string, WorldControl> worldControl, ExpertControl expertControl, StudentControl studentControl, ITutorConfig config)
			: base(ontologyPath, logsPath, expertConfPath, worldConfPath, worldControl, expertControl, studentControl, config, true)
		{
			this.rTutor = rTutor;
			this.cmTutor = cmTutor;
		}

		protected override Dictionary<string, List<string>> GetTutoringStrategyMessages(string actionName, string domainName, string studentKey)
		{
			if (cmTutor.HasSupportForAction(actionName, domainName, studentKey))
			{
				return cmTutor.GetTutorMessages(actionName, domainName, studentKey);
			}
			else
			{
				return rTutor.GetTutorMessages(actionName, domainName, studentKey);
			}
		}
	}
}

