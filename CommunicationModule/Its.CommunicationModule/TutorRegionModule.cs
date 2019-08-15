﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Its.TutoringModule.Common;
using Its.TutoringModule.Factories;
using Its.Utils.Config;
using log4net;
using Mono.Addins;
using Nini.Config;
using OpenMetaverse;
using OpenSim.Region.Framework.Interfaces;
using OpenSim.Region.Framework.Scenes;

[assembly: Addin("TutorRegionModule", "0.1")]
[assembly: AddinDependency("OpenSim", "0.8.1.1")]
namespace Its.CommunicationModule
//namespace TutorRegionModule
{

//	[Extension(Path = "/OpenSim/RegionModules", NodeName = "RegionModule", Id = "TutorRegionModule")]
//	public class TutorRegionModule:INonSharedRegionModule
//	{
//		private ITutor tutor = null;
//		private IConfig m_config = null;
//		private bool m_enabled = true;
//		private Scene m_scene = null;
//
//		private IScriptModuleComms m_comms;
//
//		private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
//
//		public string Name { get { return this.GetType().Name;} }        
//
//		public void Initialise(IConfigSource config)
//		{
//			TutorFactory tf = TutorFactory.Instance();
//			tutor = tf.CreateReactiveTutor(new DefaultTutorConfig());
//			m_log.WarnFormat("[TutorRegionModule] start configuration");
//			try 
//			{
//				if ((m_config = config.Configs["TutorRegionModule"]) != null)
//					m_enabled = m_config.GetBoolean("Enabled", m_enabled);
//			}
//			catch (Exception e)
//			{
//				m_log.ErrorFormat("[PruebaOS] initialization error: {0}",e.Message);
//				return;
//			}
//
//			m_log.ErrorFormat("[TutorRegionModule] module {0} enabled",(m_enabled ? "is" : "is not"));
//		}
//
//		public void PostInitialise()
//		{
//			if (m_enabled) {}
//		}
//
//		public void Close() { }
//		public void AddRegion(Scene scene) { }
//		public void RemoveRegion(Scene scene)  { }
//
//		public void RegionLoaded(Scene scene)
//		{
//			if (m_enabled)
//			{
//				m_scene = scene;
//				m_comms = m_scene.RequestModuleInterface<IScriptModuleComms>();
//				if (m_comms == null)
//				{
//					m_log.WarnFormat("[PruebaOS] ScriptModuleComms interface not defined");
//					m_enabled = false;
//
//					return;
//				}
//
//				m_comms.RegisterScriptInvocation(this,"CreateStudent");
//				m_comms.RegisterScriptInvocation(this,"LoadStudents");
//				m_comms.RegisterScriptInvocation(this,"LoadWorld");
//				m_comms.RegisterScriptInvocation(this,"LoadDomain");
//				m_comms.RegisterScriptInvocation(this,"ToTutor");
//
//				// Register some constants as well
//
//			}
//		}
//
//		public Type ReplaceableInterface
//		{
//			get { return null; }
//		}
//
//		#region ScriptInvocationInteface
//		public float CreateStudent(UUID hostID, UUID scriptID, string key, string name, string middleName, string surname, string lastName)
//		{
//			m_log.WarnFormat("[TutorRegionModule] Suma parameter");
//			tutor.CreateStudent (key, name, middleName, surname, lastName);
//			return 1;
//		}
//
//		public float LoadStudents(UUID hostID, UUID scriptID)
//		{
//			m_log.WarnFormat("[TutorRegionModule] Load students");
//			tutor.LoadStudents ();
//			return 1;
//		}
//
//		public float LoadWorld(UUID hostID, UUID scriptID, string virtualEnvironmentKey)
//		{
//			m_log.WarnFormat("[TutorRegionModule] Load students");
//			tutor.LoadWorld (virtualEnvironmentKey);
//			return 1;
//		}
//			
//		public float LoadDomain(UUID hostID, UUID scriptID, string key)
//		{
//			m_log.WarnFormat("[TutorRegionModule] Load students");
//			tutor.LoadDomain (key);
//			return 1;
//		}
//
//
//		public  object ToTutor(UUID hostID, UUID scriptID, string actionName, string domainName, string studentKey, string objectName)
//		{
//			m_log.WarnFormat("[TutorRegionModule] Load students");
//
//			List<string> messages=null;
//			tutor.ToTutor(actionName, domainName, studentKey, objectName,out messages);
//			return messages;
//		}
//
//
//		#endregion

			

	
//}

}

