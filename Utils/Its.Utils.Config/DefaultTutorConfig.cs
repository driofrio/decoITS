﻿using System.Configuration;

namespace Its.Util.Config
{
    public class DefaultTutorConfig : ITutorConfig
    {
        public int InitialColumn
        {
            get { return int.Parse(ConfigurationManager.AppSettings["initialColumn"]); }
        }

        public int FinalColumn
        {
            get { return int.Parse(ConfigurationManager.AppSettings["finalColumn"]); }
        }

        public int InitialRow
        {
            get { return int.Parse(ConfigurationManager.AppSettings["initialRow"]); }
        }

        public string OntologyPath
        {
            get { return ConfigurationManager.AppSettings["ontologyPath"]; }
        }

        public string LogsPath
        {
            get { return ConfigurationManager.AppSettings["logsPath"]; }
        }

        public string DomainConfigurationPath
        {
            get { return ConfigurationManager.AppSettings["domainConfigurationPath"]; }
        }

        public string WorldConfigurationPath
        {
            get { return ConfigurationManager.AppSettings["worldConfigurationPath"]; }
        }

        public int TimePenalization
        {
            get { return int.Parse(ConfigurationManager.AppSettings["TimePenalization"]); }
        }

        public int NoTutoringNoBlockErrorPenalization
        {
            get { return int.Parse(ConfigurationManager.AppSettings["NoTutoringNoBlockErrorPenalization"]); }
        }

        public int TutoringBlockErrorPenalization
        {
            get { return int.Parse(ConfigurationManager.AppSettings["TutoringBlockErrorPenalization"]); }
        }

        public int TutoringNoBlockErrorPenalization
        {
            get { return int.Parse(ConfigurationManager.AppSettings["TutoringNoBlockErrorPenalization"]); }
        }

        public string ASSConString
        {
            get { return ConfigurationManager.AppSettings["ASSConString"]; }
        }

        public string BDDConString
        {
            get { return ConfigurationManager.AppSettings["BDDConString"]; }
        }
    }
}