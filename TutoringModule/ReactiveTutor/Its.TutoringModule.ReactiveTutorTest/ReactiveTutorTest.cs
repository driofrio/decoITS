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
    [Parallelizable(ParallelScope.Self)]
    public class ReactiveTutorTest
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
        public void TestHappyPath()
        {
            // Arrange
            string domainName = "HappyPath";
            InitTest(domainName);
            
            TutorFactory tf = TutorFactory.Instance();
            ITutor tutor = tf.CreateReactiveTutor(domainName, config);

            string studentKey = "1";
            
            // Clean any previous test run logs from memory
            tutor.ResetPractice(domainName, studentKey);
            
            // Act
            // Assert

            int code;
            List<string> errors;
            Dictionary<string, List<string>> messages;

            // action1
            code = tutor.Validate("action1", domainName, studentKey, "obj", out errors);
            Assert.AreEqual(1, code);
            Assert.AreEqual(0, errors.Count);
            messages = tutor.GetTutorMessages("action1", domainName, studentKey);
            Assert.AreEqual(2, messages.Keys.Count);
            Assert.AreEqual("action1", messages["confirmation"][0]);
            Assert.AreEqual("action2", messages["tutor"][0]);
            
            // action2
            // Bug present in displaying confirmation message - control flag in spreadsheet is currently ignored
            code = tutor.Validate("action2", domainName, studentKey, "obj", out errors);
            Assert.AreEqual(1, code);
            Assert.AreEqual(0, errors.Count);
            messages = tutor.GetTutorMessages("action2", domainName, studentKey);
            Assert.AreEqual(2, messages.Keys.Count);
            Assert.AreEqual("action2", messages["confirmation"][0]);
            Assert.AreEqual("action3", messages["tutor"][0]);
            
            // action3
            code = tutor.Validate("action3", domainName, studentKey, "obj", out errors);
            Assert.AreEqual(1, code);
            Assert.AreEqual(0, errors.Count);
            messages = tutor.GetTutorMessages("action3", domainName, studentKey);
            Assert.AreEqual(1, messages.Keys.Count);
            Assert.AreEqual("action3", messages["confirmation"][0]);
        }
        
        [Test]
        public void TestSimpleDependency()
        {
            // Arrange
            string domainName = "SimpleDependency";
            InitTest(domainName);
            
            TutorFactory tf = TutorFactory.Instance();
            ITutor tutor = tf.CreateReactiveTutor(domainName, config);

            string studentKey = "1";
            
            // Clean any previous test run logs from memory
            tutor.ResetPractice(domainName, studentKey);
            
            // Act
            // Assert

            int code;
            List<string> errors;
            Dictionary<string, List<string>> messages;

            // action1
            code = tutor.Validate("action1", domainName, studentKey, "obj", out errors);
            Assert.AreEqual(1, code);
            Assert.AreEqual(0, errors.Count);
            messages = tutor.GetTutorMessages("action1", domainName, studentKey);
            Assert.AreEqual(2, messages.Keys.Count);
            Assert.AreEqual("action1", messages["confirmation"][0]);
            Assert.AreEqual("action2", messages["tutor"][0]);
            
            // action2
            code = tutor.Validate("action2", domainName, studentKey, "obj", out errors);
            Assert.AreEqual(1, code);
            Assert.AreEqual(0, errors.Count);
            messages = tutor.GetTutorMessages("action2", domainName, studentKey);
            Assert.AreEqual(1, messages.Keys.Count);
            Assert.AreEqual("action2", messages["confirmation"][0]);
        }
        
        [Test]
        public void TestSimpleDependencyError()
        {
            // Arrange
            string domainName = "SimpleDependency";
            InitTest(domainName);
            
            TutorFactory tf = TutorFactory.Instance();
            ITutor tutor = tf.CreateReactiveTutor(domainName, config);

            string studentKey = "1";
            
            // Clean any previous test run logs from memory
            tutor.ResetPractice(domainName, studentKey);
            
            // Act
            // Assert

            int code;
            List<string> errors;
            Dictionary<string, List<string>> messages;

            // action1
            // Skip to cause validation error when performing action2
            
            // action2
            code = tutor.Validate("action2", domainName, studentKey, "obj", out errors);
            Assert.AreEqual(0, code);
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual("action1", errors[0]);
        }
        
        [Test]
        public void TestOptDependency()
        {
            // Arrange
            string domainName = "OptDependency";
            InitTest(domainName);
            
            TutorFactory tf = TutorFactory.Instance();
            ITutor tutor = tf.CreateReactiveTutor(domainName, config);

            string studentKey = "1";
            
            // Clean any previous test run logs from memory
            tutor.ResetPractice(domainName, studentKey);
            
            // Act
            // Assert

            int code;
            List<string> errors;
            Dictionary<string, List<string>> messages;

            // action1
            code = tutor.Validate("action1", domainName, studentKey, "obj", out errors);
            Assert.AreEqual(1, code);
            Assert.AreEqual(0, errors.Count);
            messages = tutor.GetTutorMessages("action1", domainName, studentKey);
            Assert.AreEqual(2, messages.Keys.Count);
            Assert.AreEqual("action1", messages["confirmation"][0]);
            Assert.AreEqual("action2", messages["tutor"][0]);
            
            // action2
            code = tutor.Validate("action2", domainName, studentKey, "obj", out errors);
            Assert.AreEqual(1, code);
            Assert.AreEqual(0, errors.Count);
            messages = tutor.GetTutorMessages("action2", domainName, studentKey);
            Assert.AreEqual(2, messages.Keys.Count);
            Assert.AreEqual("action2", messages["confirmation"][0]);
            Assert.AreEqual("action3", messages["tutor"][0]);
            
            // action3
            code = tutor.Validate("action3", domainName, studentKey, "obj", out errors);
            Assert.AreEqual(1, code);
            Assert.AreEqual(0, errors.Count);
            messages = tutor.GetTutorMessages("action3", domainName, studentKey);
            Assert.AreEqual(1, messages.Keys.Count);
            Assert.AreEqual("action3", messages["confirmation"][0]);
        }
        
        [Test]
        public void TestOptDependencyError()
        {
            // Arrange
            string domainName = "OptDependency";
            InitTest(domainName);
            
            TutorFactory tf = TutorFactory.Instance();
            ITutor tutor = tf.CreateReactiveTutor(domainName, config);

            string studentKey = "1";
            
            // Clean any previous test run logs from memory
            tutor.ResetPractice(domainName, studentKey);
            
            // Act
            // Assert

            int code;
            List<string> errors;
            Dictionary<string, List<string>> messages;

            // action1
            code = tutor.Validate("action1", domainName, studentKey, "obj", out errors);
            Assert.AreEqual(1, code);
            Assert.AreEqual(0, errors.Count);
            messages = tutor.GetTutorMessages("action1", domainName, studentKey);
            Assert.AreEqual(2, messages.Keys.Count);
            Assert.AreEqual("action1", messages["confirmation"][0]);
            Assert.AreEqual("action2", messages["tutor"][0]);
            
            // action2
            // skipping
            
            // action3
            code = tutor.Validate("action3", domainName, studentKey, "obj", out errors);
            Assert.AreEqual(0, code);
            Assert.AreEqual(1, errors.Count);
            Assert.AreEqual("action2", errors[0]);
        }
    }
}