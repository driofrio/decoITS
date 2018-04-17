using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Its.TutoringModule.Common;
using Its.TutoringModule.Factories;
using Its.Utils.Config;
using NUnit.Framework;

namespace Its.TutoringModule.ReactiveTutorTest
{
    [TestFixture]
    public class ReactiveTutorTest
    {
        public ITutorConfig InitializeTest(string testName)
        {
            string runtimePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string assetPath = runtimePath + "/TestAssets/";

            // Read default config values from Its.TutoringModule.CMTutorTest.dll.config
            ITutorConfig template = new DefaultTutorConfig();
            
            // Update default config paths with runtime values
            Dictionary<string, string> settings = new Dictionary<string, string>()
            {
                {"OntologyPath", assetPath + "Ontology/"},
                {"LogsPath", assetPath + testName + "/Logs/"},
                {"DomainConfigurationPath", assetPath + testName + "/DomainConf/"},
                {"WorldConfigurationPath", assetPath + testName + "/WorldConf/"}
            };
            
            ITutorConfig config = new DefaultTutorConfig(template, settings);
            
            // Wipe any previous logs if present
            Directory.Delete(config.LogsPath, true);
            Directory.Delete(config.DomainConfigurationPath + "/Logs", true);

            return config;
        }
        
        [Test]
        public void TestHappyPath()
        {
            // Arrange
            string testName = "HappyPath";
            ITutorConfig config = InitializeTest(testName);
            
            TutorFactory tf = TutorFactory.Instance();
            ITutor tutor = tf.CreateReactiveTutor(testName, config);

            string studentKey = "1";
//            try
//            {
//                tutor.CreateStudent(studentKey, "Test", "Test", "Test", "Test");
//            }
//            catch (Exception e)
//            {
//                // Most likely stuent with such ID already exists.
//            }
//            tutor.CreateStudent(studentKey, "Test", "Test", "Test", "Test");
            
            // Act
            // Assert

            int code;
            List<string> errors;
            Dictionary<string, List<string>> messages;

            // action1
            code = tutor.Validate("action1", testName, studentKey, "obj", out errors);
            Assert.AreEqual(code, 1);
            Assert.AreEqual(errors.Count, 0);
            messages = tutor.GetTutorMessages("action1", testName, studentKey);
            Assert.AreEqual(2, messages.Keys.Count);
            Assert.AreEqual("action1", messages["confirmation"][0]);
            Assert.AreEqual("action2", messages["tutor"][0]);
            
            // action2
            code = tutor.Validate("action2", testName, studentKey, "obj", out errors);
            Assert.AreEqual(code, 1);
            Assert.AreEqual(errors.Count, 0);
            messages = tutor.GetTutorMessages("action2", testName, studentKey);
            Assert.AreEqual(2, messages.Keys.Count);
            Assert.AreEqual("action2", messages["confirmation"][0]);
            Assert.AreEqual("action3", messages["tutor"][0]);
            
            // action3
            code = tutor.Validate("action3", testName, studentKey, "obj", out errors);
            Assert.AreEqual(code, 1);
            Assert.AreEqual(errors.Count, 0);
            messages = tutor.GetTutorMessages("action3", testName, studentKey);
            Assert.AreEqual(1, messages.Keys.Count);
            Assert.AreEqual("action3", messages["confirmation"][0]);
        }
    }
}