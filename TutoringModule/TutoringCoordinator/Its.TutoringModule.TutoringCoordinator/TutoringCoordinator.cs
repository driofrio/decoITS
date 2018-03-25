using System.Collections.Generic;
using Its.ExpertModule;
using Its.TutoringModule.Common;
using Its.Utils.Config;
using Its.WorldModule;

namespace Its.TutoringModule.TutoringCoordinator
{
	public class TutoringCoordinator : AbstractTutor
	{
		public TutoringCoordinator(bool master, ITutorConfig config) : base(master, config)
		{
		}

		public TutoringCoordinator(bool master, string ontologyPath, string logsPath, string expertConfPath, string worldConfPath, Dictionary<string, WorldControl> worldControl, ExpertControl expertControl) : base(master, ontologyPath, logsPath, expertConfPath, worldConfPath, worldControl, expertControl)
		{
		}

		public override int ToTutor(string actionName, string domainName, string studentKey, string objectName, out List<string> messages)
		{
			throw new System.NotImplementedException();
		}
	}
}

