using System;
using System.Collections.Generic;
using System.IO;
using Its.TutoringModule.Common;
using Its.TutoringModule.Factories;
using Its.Utils.Config;

namespace SBPTestLogGenerator
{
    internal class Program
    {
        public const string DOMAIN_NAME = "SBPClusterTest";
        public const string OBJECT_NAME = "obj";
        
        public static void Main(string[] args)
        {
            Dictionary<int, List<Group>> clusters = new Dictionary<int, List<Group>>()
            {
                // Happy path cluster without any errors
                { 1, new List<Group>() {
                    new Group(new string[] {"1", "2", "3", "4", "5"}, new string[] {"Start", "t1", "t2", "t3"})
                }},
                // 1 irrelevant error (repeating action t6) and 2 relevant errors (t9 and t10)
                { 2, new List<Group>() {
                    new Group(new string[] {"6"}, new string[] {"Start", "t6", "t6", "t7", "t8"}),
                    new Group(new string[] {"7", "8"}, new string[] {"Start", "t6", "t7", "t9", "t8"}),
                    new Group(new string[] {"9", "10"}, new string[] {"Start", "t6", "t7", "t10", "t8"})
                }},
                // Loop t11-t12-t13
                { 3, new List<Group>() {
                    new Group(new string[] {"11", "12", "13", "14", "15"}, new string[] {"Start", "t11", "t12", "t13", "t11", "t12", "t13", "t14", "t15"})
                }},
            };

            GenerateLogs(clusters);
        }

        private static void GenerateLogs(Dictionary<int, List<Group>> clusters)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            ITutorConfig config = new DefaultTutorConfig();
            CleanOldLogs(config);

            TutorFactory tf = TutorFactory.Instance();
            ITutor tutor = tf.CreateReactiveTutor(DOMAIN_NAME, config);

            foreach (int key in clusters.Keys)
            {
                List<Group> clusterGroups = clusters[key];
                Console.WriteLine("===============================");
                Console.WriteLine("         Cluster "+ key);
                Console.WriteLine("===============================");
                
                foreach (Group group in clusterGroups)
                {
                    Console.WriteLine(group.ToString());
                    foreach (string studentKey in group.StudentKeys)
                    {
                        Console.Write("  - " + studentKey + " ");
                        foreach (string actionKey in group.ActionKeys)
                        {
                            List<string> errors;
                            tutor.Validate(actionKey, DOMAIN_NAME, studentKey, OBJECT_NAME, out errors);
                            tutor.GetTutorMessages(actionKey, DOMAIN_NAME, studentKey);
                        }
                        Console.Write('\u2714');
                        Console.WriteLine();
                    }
                }
            }
        }

        private static void CleanOldLogs(ITutorConfig config)
        {   
            // Wipe any previous logs if present
            if (Directory.Exists(config.LogsPath))
            {
                Directory.Delete(config.LogsPath, true);
            }

            if (Directory.Exists(config.DomainConfigurationPath + "Logs"))
            {
                Directory.Delete(config.DomainConfigurationPath + "Logs", true);
            }
        }
    }
}