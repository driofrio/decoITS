using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Its.TutoringModule.CMTutor.EPM.PathFind;
using Its.TutoringModule.CMTutor.SBP;
using Its.TutoringModule.CMTutor.SBP.OM;
using Its.TutoringModule.CMTutor.SBP.OM.State;
using Its.TutoringModule.CMTutor.SBP.OM.Event;
using Its.TutoringModule.Common;
using Its.TutoringModule.Factories;
using Its.TutoringModule.ReactiveTutor.ObjectModel;
using Its.Utils.Config;
using Its.Utils.Math;
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

        [TearDown]
        public void TearDownTest()
        {
            Util.DestroyModel();
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
            
            // High level checks
            ClusterMethod clusterMethod = ClusterMethod.NoClusters;
            double noErrorPreventionSupportThreshold = 0.5;
            double noErrorPreventionConfidenceThreshold = 0.25;
            
            PredictiveStudentModel model = sbpControl.GetModel(domainName, clusterMethod);
            StudentsCluster cluster = model.GetCluster(0);
            StudentActionsModel sam = cluster.StudentActionsModel;

            Assert.AreEqual(4, sam.GetStatesByArea(Area.RelevantErrors).Count());
            Assert.AreEqual(0, sam.GetStatesByArea(Area.IrrelevantErrors).Count());
            Assert.AreEqual(6, sam.GetStatesByArea(Area.CorrectFlow).Count());
            
            Node<State, Event> initState = cluster.StudentActionsModel.InitState;
            
            // Get all relevant error states with sufficient support
            List<Node<State, Event>> reStates = sbpControl.GetAllREErrorStatesAboveThreshold(domainName, clusterMethod,
                "1", noErrorPreventionSupportThreshold);
            
            HashSet<string> reStateKeys = new HashSet<string>();
            foreach (Node<State, Event> node in reStates)
            {
                reStateKeys.Add(node.Key);
            }
            
            Assert.AreEqual(2, reStateKeys.Count);
            Assert.True(reStateKeys.Contains("f0t5_f0t3_RelevantErrors"));
            Assert.True(reStateKeys.Contains("f0t4_f0t3_RelevantErrors"));
            
            
            // Check probable errors from Initial State
            PathFinder pf = new PathFinder(sam);
            Dictionary<string, double> reStatePathConfidences =
                pf.FindPathsAboveThreshold(initState.Key, reStateKeys, noErrorPreventionConfidenceThreshold);
            
            Assert.AreEqual(2, reStatePathConfidences.Count);
            Assert.That(reStatePathConfidences["f0t4_f0t3_RelevantErrors"], Is.EqualTo(0.6).Within(Util.DOUBLE_PRECISION));
            Assert.That(reStatePathConfidences["f0t5_f0t3_RelevantErrors"], Is.EqualTo(0.7).Within(Util.DOUBLE_PRECISION));
        }
        
        
        [Test]
        public void TestDomain1Graph2()
        {
            //                           [Graph 2]
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
            //+-------------------------------------------------------------------------------------------+
            //                                           |                         |    Relevant Errors
            //                                           |                         |
            //                                        +--v--+        +-----+       |
            //                                        | ERR |   2    |     |       |
            //                                        |(4_3)+--------> (5) |       |
            //                                        |     |        |     |       |
            //                                        +--+--+        +--+--+       |
            //                                           |              |          |
            //                                         4 |              |          |
            //                                           |            2 |          |
            //                                        +--v--+           |       +--v--+
            //                                        |     |           +-------> ERR |
            //                                        | (3) |                   |(5_3)|
            //                                        |     <-------------------+     |
            //                                        +-----+         3         +-----+




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
                new Group(new string[] {"5", "6"}, new string[] {"t1", "t2", "t4", "t5", "t3"}),
                new Group(new string[] {"7"}, new string[] {"t1", "t2", "t4", "t3"}),
                new Group(new string[] {"8", "9", "10"}, new string[] {"t1", "t4", "t3"})
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
            //     STUDENT 7
            ///////////////////////
            studentNodeKeys = Util.GetStudentNodeKeyListInSequence(cluster, "7");
            Assert.AreEqual(5, studentNodeKeys.Count());
            Assert.AreEqual("f0t1_CorrectFlow", studentNodeKeys[0]);
            Assert.AreEqual("f0t2_CorrectFlow", studentNodeKeys[1]);
            Assert.AreEqual("f0t4_CorrectFlow", studentNodeKeys[2]);
            Assert.AreEqual("f0t4_f0t3_RelevantErrors", studentNodeKeys[3]);
            Assert.AreEqual("f0t3_RelevantErrors", studentNodeKeys[4]);
            
            
            ///////////////////////
            //     STUDENT 8
            ///////////////////////
            studentNodeKeys = Util.GetStudentNodeKeyListInSequence(cluster, "8");
            Assert.AreEqual(4, studentNodeKeys.Count());
            Assert.AreEqual("f0t1_CorrectFlow", studentNodeKeys[0]);
            Assert.AreEqual("f0t4_CorrectFlow", studentNodeKeys[1]);
            Assert.AreEqual("f0t4_f0t3_RelevantErrors", studentNodeKeys[2]);
            Assert.AreEqual("f0t3_RelevantErrors", studentNodeKeys[3]);
        }
        
        [Test]
        public void TestDomain1Graph2PathFinding()
        {   
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
                new Group(new string[] {"5", "6"}, new string[] {"t1", "t2", "t4", "t5", "t3"}),
                new Group(new string[] {"7"}, new string[] {"t1", "t2", "t4", "t3"}),
                new Group(new string[] {"8", "9", "10"}, new string[] {"t1", "t4", "t3"})
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
            
            ClusterMethod clusterMethod = ClusterMethod.NoClusters;
            double noErrorPreventionSupportThreshold = 0.3;
            double noErrorPreventionConfidenceThreshold = 0.33;
            
            PredictiveStudentModel model = sbpControl.GetModel(domainName, ClusterMethod.NoClusters);
            StudentsCluster cluster = model.GetCluster(0);
            StudentActionsModel sam = cluster.StudentActionsModel;

            Assert.AreEqual(4, sam.GetStatesByArea(Area.RelevantErrors).Count());
            Assert.AreEqual(0, sam.GetStatesByArea(Area.IrrelevantErrors).Count());
            Assert.AreEqual(6, sam.GetStatesByArea(Area.CorrectFlow).Count());
            
            Node<State, Event> initState = cluster.StudentActionsModel.InitState;
            
            // Get all relevant error states with sufficient support
            List<Node<State, Event>> reStates = sbpControl.GetAllREErrorStatesAboveThreshold(domainName, clusterMethod,
                "1", noErrorPreventionSupportThreshold);
            
            HashSet<string> reStateKeys = new HashSet<string>();
            foreach (Node<State, Event> node in reStates)
            {
                reStateKeys.Add(node.Key);
            }
            
            Assert.AreEqual(2, reStateKeys.Count);
            Assert.True(reStateKeys.Contains("f0t5_f0t3_RelevantErrors"));
            Assert.True(reStateKeys.Contains("f0t4_f0t3_RelevantErrors"));
            
            
            // Check probable errors from Initial State
            PathFinder pf = new PathFinder(sam);
            Dictionary<string, double> reStatePathConfidences =
                pf.FindPathsAboveThreshold(initState.Key, reStateKeys, noErrorPreventionConfidenceThreshold);
            
            Assert.AreEqual(1, reStatePathConfidences.Count);
            Assert.That(reStatePathConfidences["f0t4_f0t3_RelevantErrors"], Is.EqualTo(0.6).Within(Util.DOUBLE_PRECISION));
            
            // Check probable errors from f0t4_CorrectFlow
            reStatePathConfidences = pf.FindPathsAboveThreshold("f0t4_CorrectFlow", reStateKeys, noErrorPreventionConfidenceThreshold);
            
            Assert.AreEqual(2, reStatePathConfidences.Count);
            Assert.That(reStatePathConfidences["f0t4_f0t3_RelevantErrors"], Is.EqualTo(1).Within(Util.DOUBLE_PRECISION));
            Assert.That(reStatePathConfidences["f0t5_f0t3_RelevantErrors"], Is.EqualTo(0.3333).Within(Util.DOUBLE_PRECISION));
            
        }
    }
}