using System.Collections.Generic;
using Its.ExpertModule;
using Its.TutoringModule.Common;
using Its.Utils.Config;
using Its.WorldModule;

namespace Its.TutoringModule.CMTutor
{
    public class CollectiveModelTutor : AbstractTutor
    {
        public CollectiveModelTutor(bool master, ITutorConfig config) : base(master, config)
        {
        }

        public CollectiveModelTutor(bool master, string ontologyPath, string logsPath, string expertConfPath, string worldConfPath, Dictionary<string, WorldControl> worldControl, ExpertControl expertControl) : base(master, ontologyPath, logsPath, expertConfPath, worldConfPath, worldControl, expertControl)
        {
        }

        public bool HasSupportForAction(string actionName, string domainName)
        {
            return false;
        }

        public override int ToTutor(string actionName, string domainName, string studentKey, string objectName, out List<string> messages)
        {
            throw new System.NotImplementedException();
        }
    }
}