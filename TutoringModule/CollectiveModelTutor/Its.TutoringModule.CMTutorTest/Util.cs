using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Its.ExpertModule;
using Its.ExpertModule.ObjectModel;
using Its.StudentModule;
using Its.StudentModule.ObjectModel;
using Its.TutoringModule.CMTutor.SBP;
using Its.TutoringModule.CMTutor.SBP.OM;
using Its.TutoringModule.CMTutor.SBP.OM.State;
using Its.TutoringModule.CMTutor.SBP.OM.Event;
using Its.TutoringModule.Common;
using Its.Utils.Config;
using Its.Utils.Math;
using Its.WorldModule;

namespace Its.TutoringModule.CMTutorTest
{
    /// <summary>
    /// Defines a group of students via a sequence of actions that they should perform and IDs of the students belonging to this group
    /// </summary>
    public class Group
    {
        public string[] StudentKeys;
        public string[] ActionKeys;

        public Group(string[] studentKeys, string[] actionKeys)
        {
            StudentKeys = studentKeys;
            ActionKeys = actionKeys;
        }

        public string ToString()
        {
            string output = "";
            output += "Students: [" + string.Join(", ", StudentKeys) + "]\n";
            output += "Actions:  [" + string.Join(", ", ActionKeys) + "]";

            return output;
        }
    }
    
    public class Util
    {
        public static void GenerateLogs(ITutor tutor, string domainName, string objectName, List<Group> groupConfiguration)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            foreach (Group group in groupConfiguration)
            {
                Console.WriteLine(group.ToString());
                foreach (string studentKey in group.StudentKeys)
                {
                    Console.Write("  - " + studentKey + " ");
                    foreach (string actionKey in group.ActionKeys)
                    {
                        List<string> errors;
                        tutor.Validate(actionKey, domainName, studentKey, objectName, out errors);
                        tutor.GetTutorMessages(actionKey, domainName, studentKey);
                    }
                    Console.Write('\u2714');
                    Console.WriteLine();
                }
            }
        }
        
        public static List<string> GetStudentNodeKeyListInSequence(StudentsCluster cluster, string studentKey)
        {
            List<string> nodeKeys = new List<string>(); 
            List<Arc<State, Event>> studentEvents = cluster.GetStudentEvents(studentKey);
            Node<State, Event> nextNode = cluster.StudentActionsModel.InitState;

            while (nextNode != null)
            {
                var nodeQuery = from evt in studentEvents
                                where evt.NodeOut.Key == nextNode.Key
                                select evt.NodeIn;
                if (nodeQuery.Count() == 0)
                {
                    nextNode = null;
                }
                else
                {
                    nextNode = nodeQuery.First();
                    nodeKeys.Add(nextNode.Key);
                }
            }

            return nodeKeys;
        }
        
        public static void InitModel(ITutorConfig config, string strDomainName, ClusterMethod cluMet, bool includeNoPlanActions, bool inPhases)
        {
            string ontologyPath = config.OntologyPath.Replace('\\', Path.DirectorySeparatorChar);
            string logsPath = config.LogsPath.Replace('\\', Path.DirectorySeparatorChar);
            string expertConfPath = config.DomainConfigurationPath.Replace('\\', Path.DirectorySeparatorChar);
            int initialCol = config.InitialColumn;
            int intialRow = config.InitialRow;
            
            ExpertControl expert = ExpertControl.Instance(ontologyPath, logsPath, expertConfPath, initialCol, intialRow);
            DomainActions domain = expert.GetDomainActions(strDomainName);
            if (domain == null)
            {
                domain = expert.CreateDomain(strDomainName);
            }

            WorldControl world = WorldControl.Instance(ontologyPath, logsPath);
            DomainLog logs = StudentControl.Instance(ontologyPath, logsPath, expertConfPath).GetDomainLogsFromOntology(domain, expert.OtherErrors, world.WorldErrors);
            StudentBehaviorPredictorControl.Instance(config).AddModel(logs, cluMet, includeNoPlanActions, inPhases);
        }
    }
}