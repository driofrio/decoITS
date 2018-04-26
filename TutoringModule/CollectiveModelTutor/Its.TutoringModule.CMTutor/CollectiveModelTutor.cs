using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Contexts;
using Its.ExpertModule;
using Its.ExpertModule.ObjectModel;
using Its.StudentModule;
using Its.StudentModule.ObjectModel;
using Its.TutoringModule.CMTutor.EPM;
using Its.TutoringModule.CMTutor.EPM.PathFind;
using Its.TutoringModule.CMTutor.SBP;
using Its.TutoringModule.CMTutor.SBP.OM;
using Its.TutoringModule.CMTutor.SBP.OM.Event;
using Its.TutoringModule.CMTutor.SBP.OM.State;
using Its.TutoringModule.Common;
using Its.TutoringModule.ReactiveTutor.ObjectModel;
using Its.Utils.Config;
using Its.Utils.Math;
using Its.WorldModule;

namespace Its.TutoringModule.CMTutor
{
    public enum TutorMessageLevel
    {
        LowDetailMessage,    // most generic message level 
        MediumDetailMessage, // mssage level with some extra details compared to low detail message
        HighDetailMessage    // most detailed message level
    }
    
    public class CollectiveModelTutor : AbstractTutor
    {
        private static readonly ClusterMethod CLUSTER_METHOD = ClusterMethod.EventsByZone;
        
        private StudentBehaviorPredictorControl sbpControl;
        private EPMController epmController;
        
        public CollectiveModelTutor(string domainKey, ITutorConfig config, bool master) : base(domainKey, config, master)
        {
            InitModel(domainKey, CLUSTER_METHOD, true, true);
            epmController = new EPMController(config.DomainConfigurationPath, domainKey);
        }

        public CollectiveModelTutor(string domainKey, string ontologyPath, string logsPath, string expertConfPath, string worldConfPath, Dictionary<string, WorldControl> worldControl, ExpertControl expertControl, StudentControl studentControl, ITutorConfig config, bool master)
            : base(ontologyPath, logsPath, expertConfPath, worldConfPath, worldControl, expertControl, studentControl, config, master)
        {
            InitModel(domainKey, CLUSTER_METHOD, true, true);
            epmController = new EPMController(config.DomainConfigurationPath, domainKey);
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
        
        protected override Dictionary<string, List<string>> GetTutoringStrategyMessages(string actionName, string domainName, string studentKey)
        {
            List<TutorMessage> tutorMessages = new List<TutorMessage>();
            List<TutorMessage> errorPreventionMessages = new List<TutorMessage>();
            string okMessage;
            Dictionary<string, List<string>> messages = new Dictionary<string, List<string>>() ;

            okMessage = GetConfirmationMessage(actionName, domainName, studentKey);
            
            // 1. Obtain tutor messages for zones of proximal development (ZDP)
            if (ZDPTutoringNeeded(domainName, studentKey))
            {
                ZDPTutoring(actionName, domainName, studentKey, ref tutorMessages);
            }

            // 2.1 Obtain relevant error prevention messages for probable errors
            GetRelevantErrorPreventionMessages(actionName, domainName, studentKey, ref errorPreventionMessages);
            
            // 2.2 Obtain irrelevant error prevention messages for probable errors
            
            // 3. Add messages to the dictionary depending on the validation result
            
            AddConfirmationMessage(ref messages, okMessage);
            AddTutorMessages(ref messages, tutorMessages);

            return messages;
        }

        private void GetRelevantErrorPreventionMessages(string actionName, string domainName, string studentKey,
            ref List<TutorMessage> errorPreventionMessages)
        {
            Node<State, Event> lastState = sbpControl.GetLastState(domainName, CLUSTER_METHOD, studentKey);
            
            // Get all relevant error states with sufficient support
            List<Node<State, Event>> reStates = sbpControl.GetAllREStatesAboveThreshold(domainName, CLUSTER_METHOD,
                studentKey, _config.NoErrorPreventionSupportThreshold);
            
            // Compute path confidences from last state to the relevant error states
            // a) futher restrict target error states by checking which have error prevention messages configured
            // b) find paths and compute total path confidence
            HashSet<string> reStateKeys = new HashSet<string>();
            foreach (Node<State, Event> node in reStates)
            {
                if (epmController.HasMessageForState(node.Key))
                {
                    reStateKeys.Add(node.Key);
                }
            }
            
            PathFinder pf = new PathFinder(sbpControl.GetStudentActionsModel(domainName, CLUSTER_METHOD, studentKey));
            Dictionary<string, double> reStatePathConfidences =
                pf.FindPathsAboveThreshold(lastState.Key, reStateKeys, _config.NoErrorPreventionConfidenceThreshold); 

            // Add messages of the right level of detail to the output container
            foreach (string targetStateKey in reStatePathConfidences.Keys)
            {
                // Conf = 1
                // 
                //     Error is likely to be made, provide high detail prevention message
                // 
                // [High Detail Threshold]
                // 
                //     Mistake is probable, give medium detail prevention message
                // 
                // [Medium Detail Threshold]
                // 
                //     Mistake could be made, give low detail prevention message
                //
                // [No Error Prevention Threshold]
                //
                //     Below error prevention threshold, error is extremely unlikely to be made, don't provide any messages
                //
                // Conf = 0
                if (reStatePathConfidences[targetStateKey] >= _config.HighDetailErrorPreventionConfidenceThreshold)
                {
                    errorPreventionMessages.Add(new TutorMessage(targetStateKey + "_highDetail", epmController.GetMessageForState(targetStateKey).HighDetailMessage));
                    
                } else if (reStatePathConfidences[targetStateKey] < _config.HighDetailErrorPreventionConfidenceThreshold &&
                           reStatePathConfidences[targetStateKey] >= _config.MediumDetailErrorPreventionConfidenceThreshold)
                {
                    errorPreventionMessages.Add(new TutorMessage(targetStateKey + "_mediumDetail", epmController.GetMessageForState(targetStateKey).MediumDetailMessage));
                }
                else if (reStatePathConfidences[targetStateKey] < _config.MediumDetailErrorPreventionConfidenceThreshold)
                {
                    errorPreventionMessages.Add(new TutorMessage(targetStateKey + "_lowDetail", epmController.GetMessageForState(targetStateKey).LowDetailMessage));
                }
            }
        }

        private void ZDPTutoring(string actionName, string domainName, string studentKey, ref List<TutorMessage> tutorMessages)
        {
            if (ZDPTutoringNeeded(domainName, studentKey))
            {
                Arc<State, Event> nextEvent = sbpControl.GetNextProbableCorrectEvent(domainName, CLUSTER_METHOD, studentKey);
                double conf = sbpControl.GetEventConfidence(domainName, CLUSTER_METHOD, studentKey, nextEvent);
                double noTutoringThreshold = _config.NoTutoringEventConfidenceThreshold;
                double lowDetailThreshold = _config.LowDetailTutoringEventConfidenceThreshold;
                double mediumDetailThreshold = _config.MediumDetailTutoringEventConfidenceThreshold;

                TutorMessage message = null;

                // Conf = 1
                //
                //     Student is extremely likely to make correct action so don't provide any tutoring
                //
                // [No Tutoring Conf Threshold]
                //
                //     Student is probably going to make correct action so provide a generic (low detail) tutoring message
                //
                // [Low Detail Threshold]
                //
                //     Student maybe is going to make a correct action so provide a medium detail tutoring message
                //
                // [Medium Detail Threshold]
                //
                //     Student is unlikely to make next correct action so provide a high detail tutoring message
                //
                // Conf = 0  
                if (noTutoringThreshold >= conf && conf > lowDetailThreshold)
                {
                    // low detail message
                    message = GetCollectiveModelTutorMessage(actionName, domainName, studentKey, TutorMessageLevel.LowDetailMessage);
                }
                else if (lowDetailThreshold >= conf && conf > mediumDetailThreshold)
                {
                    // medium detail message
                    message = GetCollectiveModelTutorMessage(actionName, domainName, studentKey, TutorMessageLevel.MediumDetailMessage);
                }
                else if (mediumDetailThreshold >= conf)
                {
                    // high detail message
                    message = GetCollectiveModelTutorMessage(actionName, domainName, studentKey, TutorMessageLevel.HighDetailMessage);
                }

                if (message != null)
                {
                    tutorMessages.Add(message);
                }
            }
        }

        private bool ZDPTutoringNeeded(string domainName, string studentKey)
        {
            double noTutoringThreshold = _config.NoTutoringEventConfidenceThreshold;
            List<Arc<State, Event>> correctEventsAboveNoTutoringThreshold = 
                sbpControl.GetNextCorrectEventsAboveThreshold(domainName, CLUSTER_METHOD, studentKey, noTutoringThreshold);
            if (correctEventsAboveNoTutoringThreshold != null && correctEventsAboveNoTutoringThreshold.Count > 0)
            {
                // There is next CORRECT event with confidence above "no tutoring threshold"
                // so ZDP tutoring is not needed
                return false;
            }
            
            // Check that there are other correct events with lower confidence 
            List<Arc<State, Event>> correctEvents = 
                sbpControl.GetNextCorrectEventsAboveThreshold(domainName, CLUSTER_METHOD, studentKey, 0);
            if (correctEvents == null || correctEvents.Count == 0)
            {
                // SBP is not aware of any potential next correct events
                // so no ZDP Tutoring can be provided
                return false;
            }
            
            return true;
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

        private string GetConfirmationMessage(string actionName, string domainName, string studentKey)
        {
            string okMessage;
            List<TutorMessage> tutorMessage;
            
            _expertControl.GetMessages (actionName, domainName, studentKey, out okMessage, out tutorMessage);

            return okMessage;
        }

        private TutorMessage GetCollectiveModelTutorMessage(string actionName, string domainName, string studentKey,
            TutorMessageLevel messageLevel)
        {
            TutorMessage message = null;
            ActionAplication action = _expertControl.GetActionByName(domainName, actionName, studentKey);
            if (messageLevel == TutorMessageLevel.LowDetailMessage)
            {
                message = action.TutorMsgLowDetail;
            } 
            else if (messageLevel == TutorMessageLevel.MediumDetailMessage)
            {
                message = action.TutorMsgMediumDetail;
            }
            else
            {
                message = action.TutorMsgHighDetail;
            }

            return message;
        }
        
        private void InitModel(string strDomainName, ClusterMethod cluMet, bool includeNoPlanActions, bool inPhases)
        {
            string ontologyPath = _config.OntologyPath.Replace('\\', Path.DirectorySeparatorChar);
            string logsPath = _config.LogsPath.Replace('\\', Path.DirectorySeparatorChar);
            string expertConfPath = _config.DomainConfigurationPath.Replace('\\', Path.DirectorySeparatorChar);
            int initialCol = _config.InitialColumn;
            int intialRow = _config.InitialRow;
            
            ExpertControl expert = ExpertControl.Instance(ontologyPath, logsPath, expertConfPath, initialCol, intialRow);
            DomainActions domain = expert.GetDomainActions(strDomainName);
            if (domain == null)
            {
                domain = expert.CreateDomain(strDomainName);
            }

            WorldControl world = WorldControl.Instance(ontologyPath, logsPath);
            DomainLog logs = StudentControl.Instance(ontologyPath, logsPath, expertConfPath).GetDomainLogsFromOntology(domain, expert.OtherErrors, world.WorldErrors);
            sbpControl = StudentBehaviorPredictorControl.Instance(_config);
            sbpControl.AddModel(logs, cluMet, includeNoPlanActions, inPhases);
        }
    }
}