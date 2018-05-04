using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Its.ExpertModule;
using Its.ExpertModule.ObjectModel;
using Its.StudentModule;
using Its.StudentModule.ObjectModel;
using Its.TutoringModule.CMTutor.SBP;
using Its.TutoringModule.CMTutor.SBP.OM;
using Its.TutoringModule.Common;
using Its.TutoringModule.Factories;
using Its.Utils.Config;
using Its.WorldModule;
using NUnit.Framework;

namespace Its.TutoringModule.CMTutorTest
{
    [TestFixture]
    [Parallelizable(ParallelScope.Self)]
    public class ErrorPreventionMessageTest
    {   
        private static ITutorConfig config;
        
        [OneTimeSetUp]
        public void InitFixture()
        {
            string runtimePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string assetPath = runtimePath + "/TestAssets/";

            // Read default config values from Its.TutoringModule.CMTutorTest.dll.config
            ITutorConfig template = new DefaultTutorConfig();
            
            // Update default config paths with runtime values
            Dictionary<string, string> settings = new Dictionary<string, string>()
            {
                {"OntologyPath", assetPath + "Ontology/"},
                {"LogsPath", assetPath + "/Logs/"},
                {"DomainConfigurationPath", assetPath + "/DomainConf/"},
                {"WorldConfigurationPath", assetPath + "/WorldConf/"}
            };
            
            config = new DefaultTutorConfig(template, settings);
            
            // Wipe any previous logs if present
            if (Directory.Exists(config.LogsPath))
            {
                Directory.Delete(config.LogsPath, true);
            }

            if (Directory.Exists(config.DomainConfigurationPath + "/Logs"))
            {
                Directory.Delete(config.DomainConfigurationPath + "/Logs", true);
            }
        }
        
        public void InitTest(string domainName)
        { 
            // Wipe any previous logs if present
            if (Directory.Exists(config.LogsPath + domainName))
            {
                Directory.Delete(config.LogsPath + domainName, true);
            }

            if (Directory.Exists(config.DomainConfigurationPath + "/Logs/" + domainName))
            {
                Directory.Delete(config.DomainConfigurationPath + "/Logs/" + domainName, true);
            }
        }
        
        [Test]
        public void TestDomain1Graph1()
        {
            //                           [Graph 1]
            //                                                3     +-----+
            //                                           +---------->     |
            //                                           |          | (3) |
            //                                        +--+--+       |     |
            //                                 7      |     |       +-----+
            //                           +------------> (2) |
            //                           |            |     |
            //      +-----+           +--+--+         ++--+-+       +-----+
            //      |     |    10     |     |          |  |    1    |     |    1
            //      |(Ini)+-----------> (1) |         3|  +---------> (5) +--------+
            //      |     |           |     |          |            |     |        |
            //      +-----+           +--+--+         +v----+       +-----+        |
            //                           |            |     |                      |
            //                           +------------> (4) |                      |
            //                                 3      |     |                      |
            //                                        +--+--+                      |
            //                                           |                         |
            //                                          6|                         |    Correct Flow
            //+----------------------------------------------------------------------------------------+
            //                                           |                         |    Relevant Errors
            //                                           |                         |
            //                                        +--v--+        +-----+       |
            //                                        | ERR |   6    |     |       |
            //                                        |(4_3)+--------> (5) |       |
            //                                        |     |        |     |       |
            //                                        +-----+        +--+--+       |
            //                                                          |          |
            //                                                          |          |
            //                                                         6|          |
            //                                        +-----+           |       +--v--+
            //                                        |     |           +-------> ERR |
            //                                        | (3) |        7          |(5_3)|
            //                                        |     <-------------------+     |
            //                                        +-----+                   +-----+


            // Below test validates that sequence of actions defined by GroupConfig objects below
            // indeed result in the graph as shown above.
            
            // Arrange
            string domainName = "Domain1";
            InitTest(domainName);
            
            TutorFactory tf = TutorFactory.Instance();
            ITutor generatorTutor = tf.CreateReactiveTutor(domainName, config);

            List<Group> groupConfig = new List<Group>()
            {
                //                         Students            Actions
                new Group(new string[] {"1", "2", "3"}, new string[] {"t1", "t2", "t3"}),
                new Group(new string[] {"4"}, new string[] {"t1", "t2", "t5", "t3"}),
                new Group(new string[] {"5", "6", "7"}, new string[] {"t1", "t2", "t4", "t5", "t3"}),
                new Group(new string[] {"8", "9", "10"}, new string[] {"t1", "t4", "t5", "t3"})
            };
            
            Util.GenerateLogs(generatorTutor, domainName, "obj", groupConfig);
            

            ///////////////////////
            //
            // Act
            //
            ///////////////////////
            
            // Init predictive model
            ClusterMethod cluMet = ClusterMethod.NoClusters;
            Util.InitModel(config, domainName, cluMet, true, true);
            StudentBehaviorPredictorControl sbpControl = StudentBehaviorPredictorControl.Instance(config);
            
            
            ////////////////////////
            //
            //     Assert
            //
            ////////////////////////
            
            PredictiveStudentModel model = sbpControl.GetModel(domainName, ClusterMethod.NoClusters);
            StudentsCluster cluster = model.GetCluster(0);
            StudentActionsModel sam = cluster.StudentActionsModel;

            Assert.AreEqual(4, sam.GetStatesByArea(Area.RelevantErrors).Count());
            Assert.AreEqual(0, sam.GetStatesByArea(Area.IrrelevantErrors).Count());
            Assert.AreEqual(6, sam.GetStatesByArea(Area.CorrectFlow).Count());
            
            
            List<string> studentNodeKeys;
              
            ///////////////////////
            //     STUDENT 1
            ///////////////////////
            studentNodeKeys = Util.GetStudentNodeKeyListInSequence(cluster, "1");
            Assert.AreEqual(3, studentNodeKeys.Count());
            Assert.AreEqual("f0t1_CorrectFlow", studentNodeKeys[0]);
            Assert.AreEqual("f0t2_CorrectFlow", studentNodeKeys[1]);
            Assert.AreEqual("f0t3_CorrectFlow", studentNodeKeys[2]);
            
            
            ///////////////////////
            //     STUDENT 4
            ///////////////////////
            studentNodeKeys = Util.GetStudentNodeKeyListInSequence(cluster, "4");
            Assert.AreEqual(5, studentNodeKeys.Count());
            Assert.AreEqual("f0t1_CorrectFlow", studentNodeKeys[0]);
            Assert.AreEqual("f0t2_CorrectFlow", studentNodeKeys[1]);
            Assert.AreEqual("f0t5_CorrectFlow", studentNodeKeys[2]);
            Assert.AreEqual("f0t5_f0t3_RelevantErrors", studentNodeKeys[3]);
            Assert.AreEqual("f0t3_RelevantErrors", studentNodeKeys[4]);
            
            
            ///////////////////////
            //     STUDENT 5
            ///////////////////////
            studentNodeKeys = Util.GetStudentNodeKeyListInSequence(cluster, "5");
            Assert.AreEqual(7, studentNodeKeys.Count());
            Assert.AreEqual("f0t1_CorrectFlow", studentNodeKeys[0]);
            Assert.AreEqual("f0t2_CorrectFlow", studentNodeKeys[1]);
            Assert.AreEqual("f0t4_CorrectFlow", studentNodeKeys[2]);
            Assert.AreEqual("f0t4_f0t3_RelevantErrors", studentNodeKeys[3]);
            Assert.AreEqual("f0t5_RelevantErrors", studentNodeKeys[4]);
            Assert.AreEqual("f0t5_f0t3_RelevantErrors", studentNodeKeys[5]);
            Assert.AreEqual("f0t3_RelevantErrors", studentNodeKeys[6]);
            
            
            ///////////////////////
            //     STUDENT 8
            ///////////////////////
            studentNodeKeys = Util.GetStudentNodeKeyListInSequence(cluster, "8");
            Assert.AreEqual(6, studentNodeKeys.Count());
            Assert.AreEqual("f0t1_CorrectFlow", studentNodeKeys[0]);
            Assert.AreEqual("f0t4_CorrectFlow", studentNodeKeys[1]);
            Assert.AreEqual("f0t4_f0t3_RelevantErrors", studentNodeKeys[2]);
            Assert.AreEqual("f0t5_RelevantErrors", studentNodeKeys[3]);
            Assert.AreEqual("f0t5_f0t3_RelevantErrors", studentNodeKeys[4]);
            Assert.AreEqual("f0t3_RelevantErrors", studentNodeKeys[5]);
        }
        
        [Test]
        public void TestDomain1Graph1PathFinding()
        {
            // Below test uses Domain1 Graph1 to validate PathFinding algorithm
            
            // Arrange
            string domainName = "Domain1";
            InitTest(domainName);
            
            TutorFactory tf = TutorFactory.Instance();
            ITutor generatorTutor = tf.CreateReactiveTutor(domainName, config);

            List<Group> groupConfig = new List<Group>()
            {
                //                         Students            Actions
                new Group(new string[] {"1", "2", "3"}, new string[] {"t1", "t2", "t3"}),
                new Group(new string[] {"4"}, new string[] {"t1", "t2", "t5", "t3"}),
                new Group(new string[] {"5", "6", "7"}, new string[] {"t1", "t2", "t4", "t5", "t3"}),
                new Group(new string[] {"8", "9", "10"}, new string[] {"t1", "t4", "t5", "t3"})
            };
            
            Util.GenerateLogs(generatorTutor, domainName, "obj", groupConfig);
            

            ///////////////////////
            //
            // Act
            //
            ///////////////////////
            
            // Init predictive model
            ClusterMethod cluMet = ClusterMethod.NoClusters;
            Util.InitModel(config, domainName, cluMet, true, true);
            StudentBehaviorPredictorControl sbpControl = StudentBehaviorPredictorControl.Instance(config);
            
            
            ////////////////////////
            //
            //     Assert
            //
            ////////////////////////
            
            PredictiveStudentModel model = sbpControl.GetModel(domainName, ClusterMethod.NoClusters);
            StudentsCluster cluster = model.GetCluster(0);
            StudentActionsModel sam = cluster.StudentActionsModel;

            Assert.AreEqual(4, sam.GetStatesByArea(Area.RelevantErrors).Count());
            Assert.AreEqual(0, sam.GetStatesByArea(Area.IrrelevantErrors).Count());
            Assert.AreEqual(6, sam.GetStatesByArea(Area.CorrectFlow).Count());
            
        }
    }
}