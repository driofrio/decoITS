namespace Its.TutoringModule.CMTutor.EPM
{
    public class ErrorPreventionMessage
    {
        // State key is a string in the format as defined by StudentActionsModel.AddState()
        private string _stateKey;
        private string _message;
        
        public string StateKey
        {
            get { return _stateKey; }
        }

        public string Message
        {
            get { return _message; }
        }

        public ErrorPreventionMessage(string stateKey, string message)
        {
            this._stateKey = stateKey;
            this._message = message;
        }
    }
}