using System.Collections.Generic;
using System.IO;
using Its.ExpertModule;
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

        public ITutor CreateReactiveTutor(ITutorConfig config)
        {
            return new Tutor(true, config);
        }
        
        public ITutor CreateCollectiveModelTutor(ITutorConfig config)
        {
            return new CollectiveModelTutor(true, config);
        }

        public ITutor CreateTutoringCoordinator(ITutorConfig config)
        {
            string ontologyPath = config.OntologyPath.Replace ('\\', Path.DirectorySeparatorChar);
            string logsPath = config.LogsPath.Replace ('\\', Path.DirectorySeparatorChar);
            string expertConfPath = config.DomainConfigurationPath.Replace ('\\', Path.DirectorySeparatorChar);
            string worldConfPath = config.WorldConfigurationPath.Replace ('\\', Path.DirectorySeparatorChar);

            //Initializes the dictionary.
            Dictionary<string, WorldControl> worldControl = new Dictionary<string, WorldControl> ();
            //Gets the ExpertControl instance.
            ExpertControl expertControl = ExpertControl.Instance(ontologyPath, logsPath, expertConfPath, 
                config.InitialColumn, config.InitialRow);
            
            Tutor rTutor = new Tutor(false, ontologyPath, logsPath, expertConfPath, worldConfPath, worldControl, expertControl);
            CollectiveModelTutor cmTutor = new CollectiveModelTutor(false, ontologyPath, logsPath, expertConfPath, worldConfPath, worldControl, expertControl);
            TutoringCoordinator tc = new TutoringCoordinator(rTutor, cmTutor, ontologyPath, logsPath, expertConfPath, worldConfPath, worldControl, expertControl);

            return tc;
        }
    }
}
