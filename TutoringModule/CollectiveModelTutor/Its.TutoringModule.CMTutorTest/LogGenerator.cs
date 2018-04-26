using System;
using System.Collections.Generic;
using Its.TutoringModule.Common;
using Its.TutoringModule.Factories;
using Its.Utils.Config;

namespace Its.TutoringModule.CMTutorTest
{
    public class LogGenerator
    {
        public static void GenerateLogs(ITutor tutor, string domainName, string objectName, List<Group> groupConfiguration)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            foreach (Group group in groupConfiguration)
            {
                Console.WriteLine(group.ToString());
                foreach (string studentKey in group.StudentKeys)
                {
                    Console.Write("  - " + studentKey + " ");
                    foreach (string actionKey in group.ActionKeys)
                    {
                        List<string> errors;
                        tutor.Validate(actionKey, domainName, studentKey, objectName, out errors);
                        tutor.GetTutorMessages(actionKey, domainName, studentKey);
                    }
                    Console.Write('\u2714');
                    Console.WriteLine();
                }
            }
        }
    }
}