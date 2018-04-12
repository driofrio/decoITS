using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Its.TutoringModule.Common;
using Its.TutoringModule.Factories;
using Its.Utils.Config;
using NUnit.Framework;

namespace Its.TutoringModule.CMTutorTest
{
    [TestFixture]
    public class CMTutorTest
    {
        public static ITutorConfig config;

        [SetUp]
        public void Init()
        {
            string runtimePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string assetPath = runtimePath + "/TestAssets/";

            // Read default config values from Its.TutoringModule.CMTutorTest.dll.config
            ITutorConfig template = new DefaultTutorConfig();
            
            // Update default config paths with runtime values
            Dictionary<string, string> settings = new Dictionary<string, string>()
            {
                {"OntologyPath", assetPath + "Ontology/"},
                {"LogsPath", assetPath + "Logs/"},
                {"DomainConfigurationPath", assetPath + "DomainConf/"},
                {"WorldConfigurationPath", assetPath + "WorldConf/"}
            };
            
            config = new DefaultTutorConfig(template, settings);
        }
        
        [Test]
        public void Test1()
        {
            // Arrange
            
            TutorFactory tf = TutorFactory.Instance();
            ITutor tutor = tf.CreateCollectiveModelTutor("Breakfast", config);
            // Act


            // Assert
        }
    }
}