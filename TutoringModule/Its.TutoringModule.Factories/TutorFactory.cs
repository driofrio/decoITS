using System.Collections.Generic;
using System.IO;
using Its.ExpertModule;
using Its.StudentModule;
using Its.TutoringModule.CMTutor;
using Its.TutoringModule.Common;
using Its.TutoringModule.ReactiveTutor;
using Its.TutoringModule.TC;
using Its.Utils.Config;
using Its.WorldModule;

namespace Its.TutoringModule.Factories
{
    public class TutorFactory
    {
        private static TutorFactory _instance = null;

        public static TutorFactory Instance (){
            if (_instance == null)
            {
                _instance = new TutorFactory();   
            }
            return _instance;
        }

        public ITutor CreateReactiveTutor(string domainKey, ITutorConfig config)
        {
            return new Tutor(domainKey, config, true);
        }
        
        public ITutor CreateCollectiveModelTutor(string domainKey, ITutorConfig config)
        {
            return new CollectiveModelTutor(domainKey, config, true);
        }

        public ITutor CreateTutoringCoordinator(string domainKey, ITutorConfig config)
        {
            string ontologyPath = config.OntologyPath.Replace ('\\', Path.DirectorySeparatorChar);
            string logsPath = config.LogsPath.Replace ('\\', Path.DirectorySeparatorChar);
            string expertConfPath = config.DomainConfigurationPath.Replace ('\\', Path.DirectorySeparatorChar);
            string worldConfPath = config.WorldConfigurationPath.Replace ('\\', Path.DirectorySeparatorChar);
            
            //Initializes the WorldControl dictionary.
            Dictionary<string, WorldControl> worldControlDict = new Dictionary<string, WorldControl> ();
            WorldControl worldControl = WorldControl.Instance(ontologyPath, logsPath + Path.DirectorySeparatorChar + domainKey);
            worldControl.CreateWorldData(domainKey, worldConfPath);
            worldControlDict.Add(domainKey, worldControl);
            
            //Gets the ExpertControl instance.
            ExpertControl expertControl = ExpertControl.Instance(ontologyPath, logsPath, expertConfPath, 
                config.InitialColumn, config.InitialRow);
            expertControl.CreateDomain (domainKey);
            
            StudentControl studentControl = StudentControl.Instance(ontologyPath, logsPath, expertConfPath);
            studentControl.GetDomainLogsFromOntology(expertControl.GetDomainActions(domainKey), expertControl.OtherErrors, worldControlDict[domainKey].WorldErrors);
            
            Tutor rTutor = new Tutor(ontologyPath, logsPath, expertConfPath, worldConfPath, worldControlDict, expertControl, studentControl, config, false);
            CollectiveModelTutor cmTutor = new CollectiveModelTutor(domainKey, ontologyPath, logsPath, expertConfPath, worldConfPath, worldControlDict, expertControl, studentControl, config, false);
            TutoringCoordinator tc = new TutoringCoordinator(rTutor, cmTutor, ontologyPath, logsPath, expertConfPath, worldConfPath, worldControlDict, expertControl, studentControl, config);

            return tc;
        }
    }
}
