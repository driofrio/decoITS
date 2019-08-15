using System;
using System.Collections.Generic;

namespace Its.TutoringModule.CMTutor.EPM
{
    public class EPMController
    {
        private EPMConfigReader _configReader;
        private Dictionary<string, ErrorPreventionMessage> _messages;
        
        public EPMController(string domainPath, string domainName)
        {
            _configReader = EPMConfigReader.Instance(domainPath);
            _messages = new Dictionary<string, ErrorPreventionMessage>();

            // Store messages in a dictionary by the state key
            List<ErrorPreventionMessage> messages = _configReader.GetMessages(domainName);
            foreach (ErrorPreventionMessage message in messages)
            {
                _messages.Add(message.StateKey, message);
            }
        }

        public bool HasMessageForState(string stateKey)
        {
            return _messages.ContainsKey(stateKey);
        }
        
        public bool HasHighDetailMessageForState(string stateKey)
        {
            return _messages.ContainsKey(stateKey) && !String.IsNullOrEmpty(_messages[stateKey].HighDetailMessage);
        }

        public ErrorPreventionMessage GetMessageForState(string stateKey)
        {
            ErrorPreventionMessage message;
            if (!_messages.TryGetValue(stateKey, out message))
            {
                throw new ArgumentException("No Error Prevention Message exists for state \"" + stateKey + "\"");
            }

            return message;
        }
    }
}