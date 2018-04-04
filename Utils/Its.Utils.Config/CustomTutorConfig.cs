using System.Collections.Generic;
using System.Configuration;

namespace Its.Utils.Config
{
    public class CustomTutorConfig : ITutorConfig
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

        public CustomTutorConfig(ITutorConfig config)
        {
            InitFromConfig(config);
        }
        
        public CustomTutorConfig(DefaultTutorConfig config, Dictionary<string, string> overrideValues)
        {
            InitFromConfig(config);
            InitFromDictionary(overrideValues);
        }

        public CustomTutorConfig(Dictionary<string, string> values)
        {
            InitFromDictionary(values);
        }

        private void InitFromConfig(ITutorConfig config)
        {
            _initialColumn = config.InitialColumn;
            _finalColumn = config.FinalColumn;
            _initialRow = config.InitialRow;
            _ontologyPath = config.OntologyPath;
            _logsPath = config.LogsPath;
            _domainConfigurationPath = config.DomainConfigurationPath;
            _worldConfigurationPath = config.WorldConfigurationPath;
            _timePenalization = config.TimePenalization;
            _noTutoringNoBlockErrorPenalization = config.NoTutoringNoBlockErrorPenalization;
            _tutoringBlockErrorPenalization = config.TutoringBlockErrorPenalization;
            _tutoringNoBlockErrorPenalization = config.TutoringNoBlockErrorPenalization;
            _assConString = config.ASSConString;
            _bddConString = config.BDDConString;
        }

        private void InitFromDictionary(Dictionary<string, string> values)
        {
            string initialColumn_;
            string finalColumn_;
            string initialRow_;
            string ontologyPath_;
            string logsPath_;
            string domainConfigurationPath_;
            string worldConfigurationPath_;
            string timePenalization_;
            string noTutoringNoBlockErrorPenalization_;
            string tutoringBlockErrorPenalization_;
            string tutoringNoBlockErrorPenalization_;
            string assConString_;
            string bddConString_;

            if (values.TryGetValue("InitialColumn", out initialColumn_))
            {
                _initialColumn = int.Parse(initialColumn_);
            }

            if (values.TryGetValue("FinalColumn", out finalColumn_))
            {
                _finalColumn = int.Parse(finalColumn_);
            }

            if (values.TryGetValue("InitialRow", out initialRow_))
            {
                _initialRow = int.Parse(initialRow_);
            }

            if (values.TryGetValue("OntologyPath", out ontologyPath_))
            {
                _ontologyPath = ontologyPath_;
            }

            if (values.TryGetValue("LogsPath", out logsPath_))
            {
                _logsPath = logsPath_;
            }

            if (values.TryGetValue("DomainConfigurationPath", out domainConfigurationPath_))
            {
                _domainConfigurationPath = domainConfigurationPath_;
            }

            if (values.TryGetValue("WorldConfigurationPath", out worldConfigurationPath_))
            {
                _worldConfigurationPath = worldConfigurationPath_;
            }

            if (values.TryGetValue("TimePenalization", out timePenalization_))
            {
                _timePenalization = int.Parse(timePenalization_);
            }

            if (values.TryGetValue("NoTutoringNoBlockErrorPenalization",
                out noTutoringNoBlockErrorPenalization_))
            {
                _noTutoringNoBlockErrorPenalization = int.Parse(noTutoringNoBlockErrorPenalization_);
            }

            if (values.TryGetValue("TutoringBlockErrorPenalization", out tutoringBlockErrorPenalization_))
            {
                _tutoringBlockErrorPenalization = int.Parse(tutoringBlockErrorPenalization_);
            }

            if (values.TryGetValue("TutoringNoBlockErrorPenalization", out tutoringNoBlockErrorPenalization_))
            {
                _tutoringNoBlockErrorPenalization = int.Parse(tutoringNoBlockErrorPenalization_);
            }

            if (values.TryGetValue("ASSConString", out assConString_))
            {
                _assConString = assConString_;
            }

            if (values.TryGetValue("InitialColumn", out bddConString_))
            {
                _bddConString = bddConString_;
            }
        }
    }
}