using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Its.ExpertModule;
using Its.ExpertModule.ObjectModel;
using Its.StudentModule;
using Its.StudentModule.ObjectModel;
using Its.TutoringModule.CMTutor.SBP;
using Its.TutoringModule.CMTutor.SBP.OM;
using Its.TutoringModule.CMTutor.SBP.OM.Event;
using Its.TutoringModule.CMTutor.SBP.OM.State;
using Its.TutoringModule.Common;
using Its.TutoringModule.Factories;
using Its.Utils.Config;
using Its.Utils.Math;
using Its.WorldModule;
using NUnit.Framework;
using NUnit.Framework.Constraints;

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
        public void TestDomain1Graph1PathFinding()
        {
            //                           [Graph 1]
            //
            //   10               10                 7
            //+-----+          +-----+      7     +-----+
            //|     |    10    |     +------------>     |           3
            //| (0) +----------> (1) |            | (2) +--------------------+
            //|     |          |     |     +------+     |                    |
            //+-----+          +--+--+     |      +---+-+                    |
            //                    |      3 |          |                      |       Correct Flow
            //+-------------------------------------------------------------------------------------+
            //                    |        |        1 |                      |       Relevant Errors
            //                  3 |     +--v--+       |      +-----+        +v----+
            //                    |     |     |       +------>     |    7   |     |
            //                    +-----> (4) |              | (5) +--------> (3) |
            //                          |     +-------------->     |        |     |
            //                          +-----+      6       +-----+        +-----+
            //                             6                    7              10

            
            // Arrange
            string domainName = "Domain1";
            InitTest(domainName);
            
            TutorFactory tf = TutorFactory.Instance();
            ITutor generatorTutor = tf.CreateReactiveTutor(domainName, config);

            List<Group> groupConfig = new List<Group>()
            {
                //                         Students            Actions
//                new Group(new string[] {"1", "2", "3"}, new string[] {"t1", "t2", "t3"}),
                new Group(new string[] {"4"}, new string[] {"t1", "t2", "t5", "t3"}),
//                new Group(new string[] {"5", "6", "7"}, new string[] {"t1", "t2", "t4", "t5", "t3"}),
//                new Group(new string[] {"8", "9", "10"}, new string[] {"t1", "t4", "t5", "t3"})
            };
            
            LogGenerator.GenerateLogs(generatorTutor, domainName, "obj", groupConfig);

            ClusterMethod cluMet = ClusterMethod.NoClusters;
            InitModel(config, domainName, cluMet, true, true);

            
            // Act
            StudentBehaviorPredictorControl sbpControl = StudentBehaviorPredictorControl.Instance(config);
//            string fromState = "f0t1";
//            List<string> errorStates = new List<Group>() {"f"}

            PredictiveStudentModel model = sbpControl.GetModel(domainName, ClusterMethod.NoClusters);
            StudentsCluster cluster = model.GetCluster(0);
            StudentActionsModel sam = cluster.StudentActionsModel;
            List<Node<State, Event>> reStates = sam.GetStatesByArea(Area.RelevantErrors);
            List<Node<State, Event>> ieStates = sam.GetStatesByArea(Area.IrrelevantErrors);
            List<Node<State, Event>> correctStates = sam.GetStatesByArea(Area.CorrectFlow);


            Console.WriteLine("CF: " + correctStates.Count);
            foreach (Node<State, Event> node in correctStates)
            {
                Console.WriteLine(node.Key);
            }
            Console.WriteLine("RE: " + reStates.Count);
            foreach (Node<State, Event> node in reStates)
            {
                Console.WriteLine(node.Key);
            }
            Console.WriteLine("IE: " + ieStates.Count);
            foreach (Node<State, Event> node in ieStates)
            {
                Console.WriteLine(node.Key);
            }
            Console.WriteLine("---------------------------");
            

            // Assert

//            int code;
//            List<string> errors;
//            Dictionary<string, List<string>> messages;
//
//            // action1
//            code = tutor.Validate("action1", domainName, studentKey, "obj", out errors);
//            Assert.AreEqual(1, code);
//            Assert.AreEqual(0, errors.Count);
//            messages = tutor.GetTutorMessages("action1", domainName, studentKey);
//            Assert.AreEqual(2, messages.Keys.Count);
//            Assert.AreEqual("action1", messages["confirmation"][0]);
//            Assert.AreEqual("action2", messages["tutor"][0]);
//            
//            // action2
//            // Bug present in displaying confirmation message - control flag in spreadsheet is currently ignored
//            code = tutor.Validate("action2", domainName, studentKey, "obj", out errors);
//            Assert.AreEqual(1, code);
//            Assert.AreEqual(0, errors.Count);
//            messages = tutor.GetTutorMessages("action2", domainName, studentKey);
//            Assert.AreEqual(2, messages.Keys.Count);
//            Assert.AreEqual("action2", messages["confirmation"][0]);
//            Assert.AreEqual("action3", messages["tutor"][0]);
//            
//            // action3
//            code = tutor.Validate("action3", domainName, studentKey, "obj", out errors);
//            Assert.AreEqual(1, code);
//            Assert.AreEqual(0, errors.Count);
//            messages = tutor.GetTutorMessages("action3", domainName, studentKey);
//            Assert.AreEqual(1, messages.Keys.Count);
//            Assert.AreEqual("action3", messages["confirmation"][0]);
        }
        
        private void InitModel(ITutorConfig config, string strDomainName, ClusterMethod cluMet, bool includeNoPlanActions, bool inPhases)
        {
            string ontologyPath = config.OntologyPath.Replace('\\', Path.DirectorySeparatorChar);
            string logsPath = config.LogsPath.Replace('\\', Path.DirectorySeparatorChar);
            string expertConfPath = config.DomainConfigurationPath.Replace('\\', Path.DirectorySeparatorChar);
            int initialCol = config.InitialColumn;
            int intialRow = config.InitialRow;
            
            ExpertControl expert = ExpertControl.Instance(ontologyPath, logsPath, expertConfPath, initialCol, intialRow);
            DomainActions domain = expert.GetDomainActions(strDomainName);
            if (domain == null)
            {
                domain = expert.CreateDomain(strDomainName);
            }

            WorldControl world = WorldControl.Instance(ontologyPath, logsPath);
            DomainLog logs = StudentControl.Instance(ontologyPath, logsPath, expertConfPath).GetDomainLogsFromOntology(domain, expert.OtherErrors, world.WorldErrors);
            StudentBehaviorPredictorControl.Instance(config).AddModel(logs, cluMet, includeNoPlanActions, inPhases);
        }
    }
}