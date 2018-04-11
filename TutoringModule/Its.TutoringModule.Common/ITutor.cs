using System.Collections.Generic;

namespace Its.TutoringModule.Common
{
    public interface ITutor
    {
        int Validate(string actionName, string domainName, string studentKey, string objectName, out List<string> errors);
        
        Dictionary<string, List<string>> GetTutorMessages(string actionName, string domainName, string studentKey);
        
        void CreateStudent(string key, string name, string middleName, string surname, string lastName);

        string WorldError(string domainKey, string actionName, string studentKey, string type, List<string> nameObject);

        void EraseAction(string actionKey, string domainName, string studentKey);

        void ResetPractice(string domainName, string studentKey);
    }
}
