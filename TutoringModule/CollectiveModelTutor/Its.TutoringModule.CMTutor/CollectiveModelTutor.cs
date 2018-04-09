using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using Its.ExpertModule;
using Its.ExpertModule.ObjectModel;
using Its.StudentModule;
using Its.StudentModule.ObjectModel;
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
            List<Error> errorList = new List<Error>();
            List<TutorMessage> tutorMessages = new List<TutorMessage>();
            List<TutorMessage> errorPreventionMessages = new List<TutorMessage>();
            string okMessage;
            messages = new Dictionary<string, List<string>>() ;

            int result = ValidateAction(actionName, domainName, studentKey, objectName, out errorList);

            okMessage = GetConfirmationMessage(actionName, domainName, studentKey);
            
            // 1. Obtain tutor messages for zones of proximal development (ZDP)
            if (ZDPTutoringNeeded(domainName, studentKey))
            {
                ZDPTutoring(actionName, domainName, studentKey, out tutorMessages);
            }

            // 2. Obtain error prevention messages for most probable next action
            
            // 3. Add messages to the dictionary depending on the validation result
            
            AddConfirmationMessage(ref messages, okMessage);
            AddTutorMessages(ref messages, tutorMessages);

            return result;
        }

        private void ZDPTutoring(string actionName, string domainName, string studentKey, out List<TutorMessage> tutorMessages)
        {
            tutorMessages = new List<TutorMessage>();
            if (ZDPTutoringNeeded(domainName, studentKey))
            {
                Arc<State, Event> nextEvent = sbpControl.GetNextProbableCorrectEvent(domainName, CLUSTER_METHOD, studentKey);
                double conf = sbpControl.GetEventConfidence(domainName, CLUSTER_METHOD, studentKey, nextEvent);
                double noTutoringThreshold = _config.NoTutoringEventConfidenceThreshold;
                double lowDetailThreshold = _config.LowDetailTutoringEventConfidenceThreshold;
                double mediumDetailThreshold = _config.MediumDetailTutoringEventConfidenceThreshold;

                TutorMessage message = null;

                if (noTutoringThreshold >= conf && conf > lowDetailThreshold)
                {
                    // low detail message
                    message = GetTutorMessage(actionName, domainName, studentKey, TutorMessageLevel.LowDetailMessage);
                }
                else if (lowDetailThreshold >= conf && conf > mediumDetailThreshold)
                {
                    // medium detail message
                    message = GetTutorMessage(actionName, domainName, studentKey, TutorMessageLevel.MediumDetailMessage);
                }
                else if (mediumDetailThreshold >= conf)
                {
                    // high detail message
                    message = GetTutorMessage(actionName, domainName, studentKey, TutorMessageLevel.HighDetailMessage);
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

        private TutorMessage GetTutorMessage(string actionName, string domainName, string studentKey,
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
    }
}