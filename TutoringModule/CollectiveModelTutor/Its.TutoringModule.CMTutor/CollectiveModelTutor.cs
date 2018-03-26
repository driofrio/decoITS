using System.Collections.Generic;
using Its.ExpertModule;
using Its.StudentModule;
using Its.TutoringModule.Common;
using Its.Utils.Config;
using Its.WorldModule;

namespace Its.TutoringModule.CMTutor
{
    public class CollectiveModelTutor : AbstractTutor
    {
        public CollectiveModelTutor(string domainKey, ITutorConfig config, bool master) : base(domainKey, config, master)
        {
        }

        public CollectiveModelTutor(string ontologyPath, string logsPath, string expertConfPath, string worldConfPath, Dictionary<string, WorldControl> worldControl, ExpertControl expertControl, StudentControl studentControl, ValidationHelper valiationHelper, bool master)
            : base(ontologyPath, logsPath, expertConfPath, worldConfPath, worldControl, expertControl, studentControl, valiationHelper, master)
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