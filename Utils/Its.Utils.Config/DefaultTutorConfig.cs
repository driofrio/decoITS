using System.Collections.Generic;
using System.Configuration;

namespace Its.Utils.Config
{
    public class DefaultTutorConfig : ITutorConfig
    {
        private int _initialColumn;
        private int _finalColumn;
        private int _initialRow;
        private string _ontologyPath;
        private string _logsPath;
        private string _domainConfigurationPath;
        private string _worldConfigurationPath;
        private int _timePenalization;
        private int _noTutoringNoBlockErrorPenalization;
        private int _tutoringBlockErrorPenalization;
        private int _tutoringNoBlockErrorPenalization;
        private string _assConString;
        private string _bddConString;
        
        public int InitialColumn 
        { 
            get { return _initialColumn; } 
        }
        public int FinalColumn 
        { 
            get { return _finalColumn; } 
        }
        public int InitialRow 
        { 
            get { return _initialRow; } 
        }
        public string OntologyPath 
        { 
            get { return _ontologyPath; } 
        }
        public string LogsPath 
        { 
            get { return _logsPath; } 
        }
        public string DomainConfigurationPath 
        { 
            get { return _domainConfigurationPath; } 
        }
        public string WorldConfigurationPath 
        { 
            get { return _worldConfigurationPath; } 
        }
        public int TimePenalization 
        { 
            get { return _timePenalization; } 
        }
        public int NoTutoringNoBlockErrorPenalization 
        { 
            get { return _noTutoringNoBlockErrorPenalization; } 
        }
        public int TutoringBlockErrorPenalization 
        { 
            get { return _tutoringBlockErrorPenalization; } 
        }
        public int TutoringNoBlockErrorPenalization 
        { 
            get { return _tutoringNoBlockErrorPenalization; } 
        }
        public string ASSConString 
        { 
            get { return _assConString; } 
        }
        public string BDDConString 
        { 
            get { return _bddConString; } 
        }
        
        public DefaultTutorConfig()
        {
            _initialColumn = int.Parse(ConfigurationManager.AppSettings["initialColumn"]);
            _finalColumn = int.Parse(ConfigurationManager.AppSettings["finalColumn"]);
            _initialRow = int.Parse(ConfigurationManager.AppSettings["initialRow"]);
            _ontologyPath = ConfigurationManager.AppSettings["ontologyPath"];
            _logsPath = ConfigurationManager.AppSettings["logsPath"];
            _domainConfigurationPath = ConfigurationManager.AppSettings["domainConfigurationPath"];
            _worldConfigurationPath = ConfigurationManager.AppSettings["worldConfigurationPath"];
            _timePenalization = int.Parse(ConfigurationManager.AppSettings["TimePenalization"]);
            _noTutoringNoBlockErrorPenalization =
                int.Parse(ConfigurationManager.AppSettings["NoTutoringNoBlockErrorPenalization"]);
            _tutoringBlockErrorPenalization =
                int.Parse(ConfigurationManager.AppSettings["TutoringBlockErrorPenalization"]);
            _tutoringNoBlockErrorPenalization =
                int.Parse(ConfigurationManager.AppSettings["TutoringNoBlockErrorPenalization"]);
            _assConString = ConfigurationManager.AppSettings["ASSConString"];
            _bddConString = ConfigurationManager.AppSettings["BDDConString"];
        }
    }
}