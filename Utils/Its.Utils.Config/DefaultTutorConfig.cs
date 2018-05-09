using System.Collections.Generic;
using System.Configuration;

namespace Its.Utils.Config
{
    public class DefaultTutorConfig : ITutorConfig
    {
        private int initialColumn;
        private int finalColumn;
        private int initialRow;
        private string ontologyPath;
        private string logsPath;
        private string domainConfigurationPath;
        private string worldConfigurationPath;
        private int timePenalization;
        private int noTutoringNoBlockErrorPenalization;
        private int tutoringBlockErrorPenalization;
        private int tutoringNoBlockErrorPenalization;
        private string assConString;
        private string bddConString;
        private double noTutoringEventConfidenceThreshold;
        private double lowDetailTutoringEventConfidenceThreshold;
        private double mediumDetailTutoringEventConfidenceThreshold;
        private double noErrorPreventionSupportThreshold;
        private double noErrorPreventionConfidenceThreshold;
        private double highDetailErrorPreventionConfidenceThreshold;
        private double mediumDetailErrorPreventionConfidenceThreshold;
        private double noErrorPreventionIEConfidenceThreshold;
        private double noErrorPreventionIERepetitionThreshold;
        private int errorPreventionSearchDepthLimit;
        
        public int InitialColumn
        {
            get { return initialColumn; }
        }

        public int FinalColumn
        {
            get { return finalColumn; }
        }

        public int InitialRow
        {
            get { return initialRow; }
        }

        public string OntologyPath
        {
            get { return ontologyPath; }
        }

        public string LogsPath
        {
            get { return logsPath; }
        }

        public string DomainConfigurationPath
        {
            get { return domainConfigurationPath; }
        }

        public string WorldConfigurationPath
        {
            get { return worldConfigurationPath; }
        }

        public int TimePenalization
        {
            get { return timePenalization; }
        }

        public int NoTutoringNoBlockErrorPenalization
        {
            get { return noTutoringNoBlockErrorPenalization; }
        }

        public int TutoringBlockErrorPenalization
        {
            get { return tutoringBlockErrorPenalization; }
        }

        public int TutoringNoBlockErrorPenalization
        {
            get { return tutoringNoBlockErrorPenalization; }
        }

        public string ASSConString
        {
            get { return assConString; }
        }

        public string BDDConString
        {
            get { return bddConString; }
        }

        public double NoTutoringEventConfidenceThreshold
        {
            get { return noTutoringEventConfidenceThreshold; }
        }

        public double LowDetailTutoringEventConfidenceThreshold
        {
            get { return lowDetailTutoringEventConfidenceThreshold; }
        }

        public double MediumDetailTutoringEventConfidenceThreshold
        {
            get { return mediumDetailTutoringEventConfidenceThreshold; }
        }
        
        public double NoErrorPreventionSupportThreshold
        {
            get { return noErrorPreventionSupportThreshold; }
        }
        
        public double NoErrorPreventionConfidenceThreshold
        {
            get { return noErrorPreventionConfidenceThreshold; }
        }
        
        public double HighDetailErrorPreventionConfidenceThreshold
        {
            get { return highDetailErrorPreventionConfidenceThreshold; }
        }
        
        public double MediumDetailErrorPreventionConfidenceThreshold
        {
            get { return mediumDetailErrorPreventionConfidenceThreshold; }
        }
        
        public double NoErrorPreventionIEConfidenceThreshold
        {
            get { return noErrorPreventionIEConfidenceThreshold; }
        }
        
        public double NoErrorPreventionIERepetitionThreshold
        {
            get { return noErrorPreventionIERepetitionThreshold; }
        }
        
        public int ErrorPreventionSearchDepthLimit
        {
            get { return errorPreventionSearchDepthLimit; }
        }
        
        public DefaultTutorConfig()
        {
            InitFromSystemConfig();
        }
        
        public DefaultTutorConfig(ITutorConfig config, Dictionary<string, string> overrideValues)
        {
            InitFromTutorConfig(config);
            InitFromDictionary(overrideValues);
        }
        
        public DefaultTutorConfig(Dictionary<string, string> values)
        {
            InitFromDictionary(values);
        }

        private void InitFromSystemConfig()
        {
            initialColumn = int.Parse(ConfigurationManager.AppSettings["initialColumn"]);
            finalColumn = int.Parse(ConfigurationManager.AppSettings["finalColumn"]);
            initialRow = int.Parse(ConfigurationManager.AppSettings["initialRow"]);
            ontologyPath = ConfigurationManager.AppSettings["ontologyPath"];
            logsPath = ConfigurationManager.AppSettings["logsPath"];
            domainConfigurationPath = ConfigurationManager.AppSettings["domainConfigurationPath"];
            worldConfigurationPath = ConfigurationManager.AppSettings["worldConfigurationPath"];
            timePenalization = int.Parse(ConfigurationManager.AppSettings["TimePenalization"]);
            noTutoringNoBlockErrorPenalization = int.Parse(ConfigurationManager.AppSettings["NoTutoringNoBlockErrorPenalization"]);
            tutoringBlockErrorPenalization = int.Parse(ConfigurationManager.AppSettings["TutoringBlockErrorPenalization"]);
            tutoringNoBlockErrorPenalization = int.Parse(ConfigurationManager.AppSettings["TutoringNoBlockErrorPenalization"]);
            assConString = ConfigurationManager.AppSettings["ASSConString"];
            bddConString = ConfigurationManager.AppSettings["BDDConString"];
            noTutoringEventConfidenceThreshold = double.Parse(ConfigurationManager.AppSettings["NoTutoringEventConfidenceThreshold"]);
            lowDetailTutoringEventConfidenceThreshold = double.Parse(ConfigurationManager.AppSettings["LowDetailTutoringEventConfidenceThreshold"]);
            mediumDetailTutoringEventConfidenceThreshold = double.Parse(ConfigurationManager.AppSettings["MediumDetailTutoringEventConfidenceThreshold"]);
            noErrorPreventionSupportThreshold = double.Parse(ConfigurationManager.AppSettings["NoErrorPreventionSupportThreshold"]);
            noErrorPreventionConfidenceThreshold = double.Parse(ConfigurationManager.AppSettings["NoErrorPreventionConfidenceThreshold"]);
            highDetailErrorPreventionConfidenceThreshold = double.Parse(ConfigurationManager.AppSettings["HighDetailErrorPreventionConfidenceThreshold"]);
            mediumDetailErrorPreventionConfidenceThreshold = double.Parse(ConfigurationManager.AppSettings["MediumDetailErrorPreventionConfidenceThreshold"]);
            noErrorPreventionIEConfidenceThreshold = double.Parse(ConfigurationManager.AppSettings["NoErrorPreventionIEConfidenceThreshold"]);
            noErrorPreventionIERepetitionThreshold = double.Parse(ConfigurationManager.AppSettings["NoErrorPreventionIERepetitionThreshold"]);
            errorPreventionSearchDepthLimit = int.Parse(ConfigurationManager.AppSettings["ErrorPreventionSearchDepthLimit"]);
        }
        
        private void InitFromTutorConfig(ITutorConfig config)
        {
            initialColumn = config.InitialColumn;
            finalColumn = config.FinalColumn;
            initialRow = config.InitialRow;
            ontologyPath = config.OntologyPath;
            logsPath = config.LogsPath;
            domainConfigurationPath = config.DomainConfigurationPath;
            worldConfigurationPath = config.WorldConfigurationPath;
            timePenalization = config.TimePenalization;
            noTutoringNoBlockErrorPenalization = config.NoTutoringNoBlockErrorPenalization;
            tutoringBlockErrorPenalization = config.TutoringBlockErrorPenalization;
            tutoringNoBlockErrorPenalization = config.TutoringNoBlockErrorPenalization;
            assConString = config.ASSConString;
            bddConString = config.BDDConString;
            noTutoringEventConfidenceThreshold = config.NoTutoringEventConfidenceThreshold;
            lowDetailTutoringEventConfidenceThreshold = config.LowDetailTutoringEventConfidenceThreshold;
            mediumDetailTutoringEventConfidenceThreshold = config.MediumDetailTutoringEventConfidenceThreshold;
            noErrorPreventionSupportThreshold = config.NoErrorPreventionSupportThreshold;
            noErrorPreventionConfidenceThreshold = config.NoErrorPreventionConfidenceThreshold;
            highDetailErrorPreventionConfidenceThreshold = config.HighDetailErrorPreventionConfidenceThreshold;
            mediumDetailErrorPreventionConfidenceThreshold = config.MediumDetailErrorPreventionConfidenceThreshold;
            noErrorPreventionIEConfidenceThreshold = config.NoErrorPreventionIEConfidenceThreshold;
            noErrorPreventionIERepetitionThreshold = config.NoErrorPreventionIERepetitionThreshold;
            errorPreventionSearchDepthLimit = config.ErrorPreventionSearchDepthLimit;
        }

        private void InitFromDictionary(Dictionary<string, string> values)
        {
            string initialColumn;
            string finalColumn;
            string initialRow;
            string ontologyPath;
            string logsPath;
            string domainConfigurationPath;
            string worldConfigurationPath;
            string timePenalization;
            string noTutoringNoBlockErrorPenalization;
            string tutoringBlockErrorPenalization;
            string tutoringNoBlockErrorPenalization;
            string assConString;
            string bddConString;
            string noTutoringEventConfidenceThreshold;
            string lowDetailTutoringEventConfidenceThreshold;
            string mediumDetailTutoringEventConfidenceThreshold;
            string noErrorPreventionSupportThreshold;
            string noErrorPreventionConfidenceThreshold;
            string highDetailErrorPreventionConfidenceThreshold;
            string mediumDetailErrorPreventionConfidenceThreshold;
            string noErrorPreventionIEConfidenceThreshold;
            string noErrorPreventionIERepetitionThreshold;
            string errorPreventionSearchDepthLimit;

            if (values.TryGetValue("InitialColumn", out initialColumn))
            {
                this.initialColumn = int.Parse(initialColumn);
            }

            if (values.TryGetValue("FinalColumn", out finalColumn))
            {
                this.finalColumn = int.Parse(finalColumn);
            }

            if (values.TryGetValue("InitialRow", out initialRow))
            {
                this.initialRow = int.Parse(initialRow);
            }

            if (values.TryGetValue("OntologyPath", out ontologyPath))
            {
                this.ontologyPath = ontologyPath;
            }

            if (values.TryGetValue("LogsPath", out logsPath))
            {
                this.logsPath = logsPath;
            }

            if (values.TryGetValue("DomainConfigurationPath", out domainConfigurationPath))
            {
                this.domainConfigurationPath = domainConfigurationPath;
            }

            if (values.TryGetValue("WorldConfigurationPath", out worldConfigurationPath))
            {
                this.worldConfigurationPath = worldConfigurationPath;
            }

            if (values.TryGetValue("TimePenalization", out timePenalization))
            {
                this.timePenalization = int.Parse(timePenalization);
            }

            if (values.TryGetValue("NoTutoringNoBlockErrorPenalization",
                                    out noTutoringNoBlockErrorPenalization))
            {
                this.noTutoringNoBlockErrorPenalization = int.Parse(noTutoringNoBlockErrorPenalization);
            }

            if (values.TryGetValue("TutoringBlockErrorPenalization", out tutoringBlockErrorPenalization))
            {
                this.tutoringBlockErrorPenalization = int.Parse(tutoringBlockErrorPenalization);
            }

            if (values.TryGetValue("TutoringNoBlockErrorPenalization", out tutoringNoBlockErrorPenalization))
            {
                this.tutoringNoBlockErrorPenalization = int.Parse(tutoringNoBlockErrorPenalization);
            }

            if (values.TryGetValue("ASSConString", out assConString))
            {
                this.assConString = assConString;
            }

            if (values.TryGetValue("InitialColumn", out bddConString))
            {
                this.bddConString = bddConString;
            }

            if (values.TryGetValue("NoTutoringEventConfidenceThreshold", out noTutoringEventConfidenceThreshold))
            {
                this.noTutoringEventConfidenceThreshold = double.Parse(noTutoringEventConfidenceThreshold);
            }

            if (values.TryGetValue("LowDetailTutoringEventConfidenceThreshold", out lowDetailTutoringEventConfidenceThreshold))
            {
                this.lowDetailTutoringEventConfidenceThreshold = double.Parse(lowDetailTutoringEventConfidenceThreshold);
            }

            if (values.TryGetValue("MediumDetailTutoringEventConfidenceThreshold", out mediumDetailTutoringEventConfidenceThreshold))
            {
                this.mediumDetailTutoringEventConfidenceThreshold = double.Parse(mediumDetailTutoringEventConfidenceThreshold);
            }
            
            if (values.TryGetValue("NoErrorPreventionSupportThreshold", out noErrorPreventionSupportThreshold))
            {
                this.noErrorPreventionSupportThreshold = double.Parse(noErrorPreventionSupportThreshold);
            }
            
            if (values.TryGetValue("NoErrorPreventionConfidenceThreshold", out noErrorPreventionConfidenceThreshold))
            {
                this.noErrorPreventionConfidenceThreshold = double.Parse(noErrorPreventionConfidenceThreshold);
            }
            
            if (values.TryGetValue("LowDetailErrorPreventionConfidenceThreshold", out highDetailErrorPreventionConfidenceThreshold))
            {
                this.highDetailErrorPreventionConfidenceThreshold = double.Parse(highDetailErrorPreventionConfidenceThreshold);
            }
            
            if (values.TryGetValue("MediumDetailErrorPreventionConfidenceThreshold", out mediumDetailErrorPreventionConfidenceThreshold))
            {
                this.mediumDetailErrorPreventionConfidenceThreshold = double.Parse(mediumDetailErrorPreventionConfidenceThreshold);
            }
            
            if (values.TryGetValue("NoErrorPreventionIEConfidenceThreshold", out noErrorPreventionIEConfidenceThreshold))
            {
                this.noErrorPreventionIEConfidenceThreshold = double.Parse(noErrorPreventionIEConfidenceThreshold);
            }
            
            if (values.TryGetValue("NoErrorPreventionIERepetitionThreshold", out noErrorPreventionIERepetitionThreshold))
            {
                this.noErrorPreventionIERepetitionThreshold = double.Parse(noErrorPreventionIERepetitionThreshold);
            }
            
            if (values.TryGetValue("ErrorPreventionSearchDepthLimit", out errorPreventionSearchDepthLimit))
            {
                this.errorPreventionSearchDepthLimit = int.Parse(errorPreventionSearchDepthLimit);
            }
        }
    }
}