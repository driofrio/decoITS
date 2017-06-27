using System;
/*using PicNetML;
using PicNetML.Clstr;
using PicNetML.Arff;*/
using System.Collections.Generic;
using System.IO;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml;
using Its.ExpertModule.ObjectModel;
using Its.TutoringModule.StudentBehaviorPredictor;
using Its.TutoringModule.TutoringCoordinator.ReactiveTutor.ObjectModel;
using Its.StudentModule.ObjectModel;
using Its.TutoringModule.StudentBehaviorPredictor.ObjectModel;
using Its.Utils.Math;
using Its.WorldModule;
using Its.StudentModule;
using Its.ExpertModule;
//using Its.TutoringModule.StudentBehaviorPredictor.ObjectModel;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Web.Script.Serialization;

using System.Web.Services;
using System.Web.SessionState;
using System.Web;
using System.Web.Hosting;

namespace WebApplication1
{
	public class MapService
	{
        private static DomainLog[] logsG = new DomainLog[int.Parse(ConfigurationManager.AppSettings["totalDomains"])];
        private static int j = 0;

        public static void CreateStudentModels()
		{
            int totalDomains= int.Parse(ConfigurationManager.AppSettings["totalDomains"]);
            for (int i=0;i<totalDomains; i++)
            {
                string domainNum = "domain" + (i+1).ToString();
                string strDomainName = ConfigurationManager.AppSettings[domainNum];//Console.ReadLine();            
                DomainActions domain = CreateModel(strDomainName);
            }
        }

        public static Tuple<PredictiveStudentModel,int> getModelByDate(DateTime date, int domain)
        {
            bool includeNoPlanActions = bool.Parse(ConfigurationManager.AppSettings["includeNoPlanActions"].ToString());
            try {
                if (domain == 0)
                    return Tuple.Create(StudentBehaviorPredictorControl.Instance.GetModelFromDate(logsG[0], date, includeNoPlanActions),0);
                else
                    return Tuple.Create(StudentBehaviorPredictorControl.Instance.GetModelFromDate(logsG[1], date, includeNoPlanActions),0);
            }
            catch (Exception e)
            {
                DateTime aux = new DateTime(1900, 1, 1);
                return Tuple.Create(StudentBehaviorPredictorControl.Instance.GetModelFromDate(logsG[0], aux, includeNoPlanActions), 1);
            }
        }

        public static PredictiveStudentModel getModelByDomain(int modo, string strDomainName)
        {
            PredictiveStudentModel model = null;
            switch (modo)
            {
                case 0:
                    model = StudentBehaviorPredictorControl.Instance.GetModel(strDomainName, ClusterMethod.Errors);
                    break;
                case 1:
                    model = StudentBehaviorPredictorControl.Instance.GetModel(strDomainName, ClusterMethod.ErrorsAndTime);
                    break;
                case 2:
                    model = StudentBehaviorPredictorControl.Instance.GetModel(strDomainName, ClusterMethod.EventsByZone);
                    break;
                case 3:
                    model = StudentBehaviorPredictorControl.Instance.GetModel(strDomainName, ClusterMethod.NoClusters);
                    break;
            }
            return model;
        }

        private static DomainActions CreateModel(string strDomainName)
        {
            //["ErrorsAndTime", "Errors", "EventsByZone", "NoClusters"]
            string ontologyPath = HostingEnvironment.MapPath(ConfigurationManager.AppSettings["ontologyPath"].ToString());
            string logsPath = HostingEnvironment.MapPath(ConfigurationManager.AppSettings["logsPath"].ToString());
            string expertConfPath = HostingEnvironment.MapPath(ConfigurationManager.AppSettings["domainConfigurationPath"].ToString());
            ExpertControl expert = ExpertControl.Instance(ontologyPath, logsPath, expertConfPath);
            DomainActions domain = expert.CreateDomain(strDomainName);
            WorldControl world = WorldControl.Instance(ontologyPath, logsPath);
            DomainLog logs = StudentControl.Instance(ontologyPath, logsPath).GetDomainLogsFromOntology(domain, expert.OtherErrors, world.WorldErrors);
            logsG[j] = logs;
            j++;
            bool includeNoPlanActions = bool.Parse(ConfigurationManager.AppSettings["includeNoPlanActions"].ToString());
            StudentBehaviorPredictorControl.Instance.AddModel(logs, ClusterMethod.Errors, includeNoPlanActions, false);
            StudentBehaviorPredictorControl.Instance.AddModel(logs, ClusterMethod.ErrorsAndTime, includeNoPlanActions, false);
            StudentBehaviorPredictorControl.Instance.AddModel(logs, ClusterMethod.EventsByZone, includeNoPlanActions, false);
            StudentBehaviorPredictorControl.Instance.AddModel(logs, ClusterMethod.NoClusters, includeNoPlanActions, false);
            return domain;
        }
    }

}


