using System.Collections.Generic;
using System.IO;
using Its.ExpertModule;
using Its.StudentModule;
using Its.TutoringModule.Common;
using Its.TutoringModule.ReactiveTutor;
using Its.Utils.Config;
using Its.WorldModule;

namespace Its.TutoringModule.Factories
{
    public class TutorFactory
    {
        private static TutorFactory _instance = null;
        
        private ITutorConfig _config;
        
        /// <summary>
        /// The world control.
        /// </summary>
        protected Dictionary<string, WorldControl> _worldControl;
        /// <summary>
        /// The student control.
        /// </summary>
        protected StudentControl _studentControl;
        /// <summary>
        /// The expert control.
        /// </summary>
        protected ExpertControl _expertControl;
        /// <summary>
        /// The ontology path.
        /// </summary>
        protected string _ontologyPath;
        /// <summary>
        /// The logs path.
        /// </summary>
        protected string _logsPath;
        /// <summary>
        /// The expert conf path.
        /// </summary>
        protected string _expertConfPath;
        /// <summary>
        /// The world conf path.
        /// </summary>
        protected string _worldConfPath;

        public static TutorFactory Instance (ITutorConfig config){
            if (_instance == null)
            {
                _instance = new TutorFactory (config);   
            }
            return _instance;
        }

        private TutorFactory(ITutorConfig config)
        {
            _config = config;
            
            _ontologyPath = config.OntologyPath.Replace ('\\', Path.DirectorySeparatorChar);
            _logsPath = config.LogsPath.Replace ('\\', Path.DirectorySeparatorChar);
            _expertConfPath = config.DomainConfigurationPath.Replace ('\\', Path.DirectorySeparatorChar);
            _worldConfPath = config.WorldConfigurationPath.Replace ('\\', Path.DirectorySeparatorChar);

            //Initializes the dictionary.
            _worldControl = new Dictionary<string, WorldControl> ();
            //Gets the ExpertControl instance.
            _expertControl = ExpertControl.Instance(_ontologyPath, _logsPath, _expertConfPath, 
                config.InitialColumn, config.InitialRow);
        }

        public ITutor CreateReactiveTutor()
        {
            return new Tutor(true, _ontologyPath, _logsPath, _expertConfPath, _worldConfPath, _worldControl, _expertControl);
        }
        
//        public ITutor CreateCollectiveModelTutor()
//        {
//            return new CollectiveModelTutor(true, _ontologyPath, _logsPath, _expertConfPath, _worldConfPath, _worldControl, _expertControl);
//        }
    }
}
