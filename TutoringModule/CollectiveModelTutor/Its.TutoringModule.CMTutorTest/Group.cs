using System.Collections.Generic;
using System.Linq;

namespace Its.TutoringModule.CMTutorTest
{
    /// <summary>
    /// Defines a group of students via a sequence of actions that they should perform and IDs of the students belonging to this group
    /// </summary>
    public class Group
    {
        public string[] StudentKeys;
        public string[] ActionKeys;

        public Group(string[] studentKeys, string[] actionKeys)
        {
            StudentKeys = studentKeys;
            ActionKeys = actionKeys;
        }

        public string ToString()
        {
            string output = "";
            output += "Students: [" + string.Join(", ", StudentKeys) + "]\n";
            output += "Actions:  [" + string.Join(", ", ActionKeys) + "]";

            return output;
        }
    }
}