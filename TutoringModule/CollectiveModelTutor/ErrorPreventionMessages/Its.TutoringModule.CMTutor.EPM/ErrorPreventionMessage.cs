namespace Its.TutoringModule.CMTutor.EPM
{
    public class ErrorPreventionMessage
    {
        // State key is a string in the format as defined by StudentActionsModel.AddState()
        private string _stateKey;
        private string _lowDetailMessage;
        private string _mediumDetailMessage;
        private string _highDetailMessage;
        
        public string StateKey
        {
            get { return _stateKey; }
        }

        public string LowDetailMessage
        {
            get { return _lowDetailMessage; }
        }

        public string MediumDetailMessage
        {
            get { return _mediumDetailMessage; }
        }

        public string HighDetailMessage
        {
            get { return _highDetailMessage; }
        }

        public ErrorPreventionMessage(string stateKey, string lowDetailMessage, string mediumDetailMessage, string highDetailMessage)
        {
            this._stateKey = stateKey;
            this._lowDetailMessage = lowDetailMessage;
            this._mediumDetailMessage = mediumDetailMessage;
            this._highDetailMessage = highDetailMessage;
        }
    }
}