using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using Its.TutoringModule.Common;
using Its.TutoringModule.Factories;
using Its.TutoringModule.ReactiveTutor.ObjectModel;
using Its.Utils.Config;
using NUnit.Framework;

namespace Its.TutoringModule.CMTutorTest
{
    [TestFixture]
    public class CMTutorTest
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
            
            // Init predictive model tutor
            ITutorConfig cmConfig = new DefaultTutorConfig(config, new Dictionary<string, string>()
            {
                {"NoErrorPreventionSupportThreshold", "0.3"},
                {"NoErrorPreventionConfidenceThreshold", "0.33"},
            });
            
            ITutor cmTutor = tf.CreateCollectiveModelTutor(domainName, cmConfig);
            string actionName;
            string studentKey = "11";
            List<string> errors;
            Dictionary<string, List<string>> tutorMessages;

            actionName = "t1";
            cmTutor.Validate(actionName, domainName, studentKey, "obj", out errors);
            Assert.AreEqual(0, errors.Count);
            tutorMessages = cmTutor.GetTutorMessages(actionName, domainName, studentKey);
            Assert.AreEqual("Ok", tutorMessages["confirmation"][0]);
            Assert.AreEqual("t2_low_detail", tutorMessages["tutor"][0]);
            Assert.AreEqual("f0t5_f0t3_low", tutorMessages["errorPrevention"][0]);
            Assert.AreEqual("f0t4_f0t3_low", tutorMessages["errorPrevention"][1]);
            
            actionName = "t2";
            cmTutor.Validate(actionName, domainName, studentKey, "obj", out errors);
            Assert.AreEqual(0, errors.Count);
            tutorMessages = cmTutor.GetTutorMessages(actionName, domainName, studentKey);
            Assert.AreEqual("Ok", tutorMessages["confirmation"][0]);
            Assert.AreEqual("t3_medium_detail", tutorMessages["tutor"][0]);
            Assert.AreEqual("f0t5_f0t3_low", tutorMessages["errorPrevention"][0]);
            Assert.AreEqual("f0t4_f0t3_low", tutorMessages["errorPrevention"][1]);

            
            actionName = "t4";
            cmTutor.Validate(actionName, domainName, studentKey, "obj", out errors);
            Assert.AreEqual(0, errors.Count); // Non blocking error is not reported at this point
            tutorMessages = cmTutor.GetTutorMessages(actionName, domainName, studentKey);
            Assert.AreEqual("Ok", tutorMessages["confirmation"][0]);
            Assert.False(tutorMessages.ContainsKey("tutor"));
            Assert.AreEqual("f0t5_f0t3_medium", tutorMessages["errorPrevention"][0]);
        }
    }
}