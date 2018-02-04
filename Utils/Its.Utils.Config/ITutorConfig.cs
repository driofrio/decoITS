﻿namespace Its.Util.Config
{
    public interface ITutorConfig
    {
        int InitialColumn { get; }
        int FinalColumn { get; }
        int InitialRow { get; }
        string OntologyPath { get; }
        string LogsPath { get; }
        string DomainConfigurationPath { get; }
        string WorldConfigurationPath { get; }
        
        int TimePenalization { get; }
        int NoTutoringNoBlockErrorPenalization { get; }
        int TutoringBlockErrorPenalization { get; }
        int TutoringNoBlockErrorPenalization { get; }
        string ASSConString { get; }
        string BDDConString { get; }
    }
}