namespace Its.Utils.Config
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
        
        double NoTutoringEventConfidenceThreshold { get; }
        double LowDetailTutoringEventConfidenceThreshold { get; }
        double MediumDetailTutoringEventConfidenceThreshold { get; }
        int TimePenalization { get; }
        int NoTutoringNoBlockErrorPenalization { get; }
        int TutoringBlockErrorPenalization { get; }
        int TutoringNoBlockErrorPenalization { get; }
        string ASSConString { get; }
        string BDDConString { get; }
        double NoErrorPreventionSupportThreshold { get; }
        double NoErrorPreventionConfidenceThreshold { get; }
        double MediumDetailErrorPreventionConfidenceThreshold { get; }
        double HighDetailErrorPreventionConfidenceThreshold { get; }
    }
}