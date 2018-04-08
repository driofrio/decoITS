﻿using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using Its.ExpertModule;
using Its.ExpertModule.ObjectModel;
using Its.StudentModule;
using Its.StudentModule.ObjectModel;
using Its.TutoringModule.CMTutor.SBP;
using Its.TutoringModule.CMTutor.SBP.OM;
using Its.TutoringModule.Common;
using Its.Utils.Config;
using Its.WorldModule;

namespace Its.TutoringModule.CMTutor
{
    public class CollectiveModelTutor : AbstractTutor
    {
        private static readonly ClusterMethod CLUSTER_METHOD = ClusterMethod.EventsByZone;
        
        private StudentBehaviorPredictorControl sbpControl;
        
        public CollectiveModelTutor(string domainKey, ITutorConfig config, bool master) : base(domainKey, config, master)
        {
            sbpControl = StudentBehaviorPredictorControl.Instance(_config);
        }

        public CollectiveModelTutor(string ontologyPath, string logsPath, string expertConfPath, string worldConfPath, Dictionary<string, WorldControl> worldControl, ExpertControl expertControl, StudentControl studentControl, ValidationHelper valiationHelper, ITutorConfig config, bool master)
            : base(ontologyPath, logsPath, expertConfPath, worldConfPath, worldControl, expertControl, studentControl, valiationHelper, config, master)
        {
            sbpControl = StudentBehaviorPredictorControl.Instance(_config);
        }

        public bool HasSupportForAction(string actionName, string domainName, string studentKey)
        {
            UpdateModel(actionName, domainName, studentKey);

            ActionAplication action = _expertControl.GetActionByName(domainName, actionName, studentKey);
            double actionSupportThreshold = action.SupportThreshold;
            double support = sbpControl.GetLastStateSupport(domainName, CLUSTER_METHOD, studentKey);
            
            if (support >= actionSupportThreshold)
            {
                return true;
            }
            
            return false;
        }

        public override int ToTutor(string actionName, string domainName, string studentKey, string objectName, out Dictionary<string, List<string>> messages)
        {
            throw new System.NotImplementedException();
        }
        
        private void UpdateModel(string actionName, string domainName, string studentKey)
        {
            StudentLog studentLog = _studentControl.GetStudentLog(domainName, studentKey);
            List<LogEntry> logs = studentLog.Logs;
            LogEntry lastActionLog = logs[logs.Count - 1];
            
            if (lastActionLog.Action.Name == actionName) {
                if (lastActionLog.Action.IsCheckpoint)
                {
                    sbpControl.UpdateModelAndRecluster(domainName, CLUSTER_METHOD, studentKey, lastActionLog, studentLog);
                }
                else
                {
                    sbpControl.UpdateModel(domainName, CLUSTER_METHOD, studentKey, lastActionLog);
                }
            }
        }
    }
}