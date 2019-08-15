using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
using System.Web.Script.Serialization;
//using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
using System.Configuration;


namespace WebApplication1.Controllers
{
    public class MapController : Controller
    {
        private static PredictiveStudentModel model;
        private List<ActionAplication> dominio = new List<ActionAplication>();
        private int busquedaPosicion(string clave, SortedDictionary<int, Node<State, Event>> found, SortedDictionary<int, List<string>> notFound)
        {
            int c = 0;
            bool find = false;
            string clave1 = clave + "_CorrectFlow";
            string clave2 = clave + "_RelevantErrors";
            for (int i = 0; i < found.Count && !find; i++)
            {
                if (found[i].Key.ToString().Equals(clave1) || found[i].Key.ToString().Equals(clave2))
                {
                    c = i + 1;
                    find = true;
                }
            }
            /*if (!find)
            {
                find = true;
                string clave2 = clave + "_RelevantErrors";
                List<Node<State, Event>> posi = new List<Node<State, Event>>();
                foreach (KeyValuePair<int, List<Node<State, Event>>> entry in relevant)
                {
                    posi = entry.Value;
                    for (int i = 0; i < posi.Count && !find; i++)
                    {
                        if (posi[i].Key == clave2)
                        {
                            c = entry.Key;
                            find = true;
                        }
                        if (find)
                            break;
                    }
                }
            }*/
            if(!find){
                List<string> posi = new List<string>();
                foreach(KeyValuePair<int,List<string>> entry in notFound)
                {
                    posi = entry.Value;
                    for(int i=0; i<posi.Count && !find; i++)
                    {
                        if (posi[i] == clave)
                        {
                            c = entry.Key;
                            find = true;
                        }
                        if (find)
                            break;
                    }
                }
            }
            return c;
        }

        public ActionResult GetEventInfo(int modo, string strDomainName, int Cluster, string nodeOut, string nodeIn)
        {
            PredictiveStudentModel model = MapService.getModelByDomain(modo,strDomainName);
            StudentsCluster cluster = model.GetCluster(Cluster);
            Arc<State, Event> arc = null;
            string reJson = "";
            List<string> outarcList = new List<string>();
            List<string> newNodesList = new List<string>();
            JavaScriptSerializer js = new JavaScriptSerializer();
            if (cluster.StudentActionsModel.TryGetEvent(nodeOut, nodeIn, out arc))
            {
                Type arcType = arc.GetType();
                string response = "";
                if (arcType == typeof(VectorEvent))
                {
                    response = "{\"NodeKey1\":" + js.Serialize(arc.NodeOut.Key) + ","
                                   + "\"NodeKey2\":" + js.Serialize(arc.NodeIn.Key) + ","
                                   + "\"Element\": \"VectorEvent\","
                                   + "\"Frequency\":\"" + js.Serialize(((VectorEvent)arc.Specification).Frequency) + "\"}";

                }
                else if (arcType == typeof(NormalEvent))
                {
                    response = "{\"NodeKey1\":" + js.Serialize(arc.NodeOut.Key) + ","
                                  + "\"NodeKey2\":" + js.Serialize(arc.NodeIn.Key) + ","
                                  + "\"Element\": \"NormalEvent\","
                                  + "\"Frequency\":\"" + js.Serialize(((NormalEvent)arc.Specification).Frequency) + "\"}";
                }
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            else
            {
                reJson = "Event not found";
                return Json(reJson, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetStateInfo(int modo, string strDomainName, int Cluster, string nodeKey)
        {
            PredictiveStudentModel model = MapService.getModelByDomain(modo, strDomainName);
            StudentsCluster cluster = model.GetCluster(Cluster);
            Node<State, Event> fromNode = null;
            string reJson = "";
            List<string> outarcList = new List<string>();
            List<string> newNodesList = new List<string>();
            List<Node<State, Event>> nextNodes = new List<Node<State, Event>>();
            List<Node<State, Event>> currentNodes = new List<Node<State, Event>>();
            JavaScriptSerializer js = new JavaScriptSerializer();
            if (cluster.StudentActionsModel.TryGetState(nodeKey, out fromNode))
            {
                //int n = fromNode.OutArcs.Count;
                Type typeNode = fromNode.GetType();
                string extra = "";
                if (typeNode == typeof(CorrectState)){
                    extra = "\"Name\":\"" + ((CorrectState)fromNode.Specification).Action.Name + "\"," 
                        //+ "\"Element\": \"state\"," 
                        + "\"StateType\":\"" + "Correct" + "\"}";
                }
                else if (typeNode == typeof(WorldErrorState))
                {
                    extra = "\"Message\":\"" + ((WorldErrorState)fromNode.Specification).ErrorAssociated.Message.Message + "\","
                        //+ "\"Element\": \"state\"," 
                        + "\"StateType\":\"" + "WorldError" + "\"}";
                }
                else if (typeNode == typeof(TimeErrorState))
                {
                    extra = "\"Message\":\"" + ((TimeErrorState)fromNode.Specification).ErrorAssociated.Message.Message + "\","
                        + "\"Time\":\"" + ((TimeErrorState)fromNode.Specification).Time + "\","
                        //+ "\"Element\": \"state\","
                        + "\"StateType\":\"" + "TimeError" + "\"}";
                }
                else if (typeNode == typeof(IncompatibilityErrorState))
                {
                    extra = "\"Message\":\"" + ((IncompatibilityErrorState)fromNode.Specification).IncompatibilityFailed.IncompatibilityError.Message.Message + "\","
                        + "\"Action\":\"" + ((IncompatibilityErrorState)fromNode.Specification).IncompatibilityFailed.IncompatibilityAction.Name + "\","
                        //+ "\"Element\": \"state\","
                        + "\"StateType\":\"" + "IncompatibilityError" + "\"}";
                }
                else if (typeNode == typeof(OtherErrorState))
                {
                    extra = "\"Message\":\"" + ((OtherErrorState)fromNode.Specification).ErrorAssociated.Message.Message + "\","
                        //+ "\"Element\": \"state\","
                        + "\"StateType\":\"" + "OtherError" + "\"}";
                }
                else if (typeNode == typeof(DependenceErrorState))
                {
                    var tempState = (DependenceErrorState)fromNode.Specification;
                    typeNode = tempState.DependenceFailed.GetType();
                    if (typeNode == typeof(SeqComplexDependence))
                    {
                        var tempDependence = (SeqComplexDependence)tempState.DependenceFailed;
                        extra = "\"Message\":\"" + tempDependence.DependenceError.Message.Message + "\","
                                + "\"ComplexDependenceKey\":\"" + tempDependence.Key + "\","
                                //+ "\"Element\": \"state\","
                                //+ "\"Columns\": " + columns + ","
                                + "\"StateType\":\"" + "SeqComplexDependenceError" + "\"}";
                    }
                    else if (typeNode == typeof(SimpleDependence))
                    {
                        var tempDependence = (SimpleDependence)tempState.DependenceFailed;
                        extra = "\"Message\":\"" + tempDependence.DependenceError.Message.Message + "\","
                                + "\"Action\":\"" + tempDependence.ActionDependence.Name + "\","
                                //+ "\"Element\": \"state\","
                                //+ "\"Columns\": " + columns + ","
                                + "\"StateType\":\"" + "SimpleDependenceError" + "\"}";
                    }
                }
                string response = "{\"Key\":" + js.Serialize(fromNode.Key) + ","
                    + "\"Area\":" + js.Serialize(fromNode.Specification.Area.ToString()) + ","
                    + "\"FrequencyS\":\"" + js.Serialize(fromNode.Specification.StudentFrequency) + "\","
                    + "\"FrequencyE\":\"" + js.Serialize(fromNode.Specification.EventFrequency) + "\","
                    + extra;
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            else
            {
                reJson = "State not found";
                return Json(reJson, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetStates(int modo, string strDomainName, int Cluster, string fromNodeKey, int num)
        {
            PredictiveStudentModel model = MapService.getModelByDomain(modo,strDomainName);
            StudentsCluster cluster = model.GetCluster(Cluster);
            Node<State, Event> fromNode = null;
            string reJson = "";
            List<string> outarcList = new List<string>();
            List<string> newNodesList = new List<string>();
            List<Node<State,Event>> nextNodes = new List<Node<State,Event>>();
            List<Node<State,Event>> currentNodes = new List<Node<State,Event>>();
            JavaScriptSerializer js = new JavaScriptSerializer();
            if (cluster.StudentActionsModel.TryGetState(fromNodeKey, out fromNode))
            {
                //newNodes.Add(js.Serialize(fromNode.Specification));
                currentNodes.Add(fromNode);
                for (int i=0;i< num; i++)
                {
                    int columns = i;
                    foreach(var node in currentNodes)
                    {
                        int nOfOutArcs = node.OutArcs.Count;

                        foreach (var arc in node.OutArcs)
                        {

                            //prepare json due to arc types
                            Type arcType = arc.Value.Specification.GetType();
                            Type nodeInType = arc.Value.NodeIn.Specification.GetType();
                            if (arcType == typeof(VectorEvent))
                            {
                                outarcList.Add("{\"NodeKey1\":" + js.Serialize(arc.Value.Key.NodeKey1) + ","
                                               + "\"NodeKey2\":" + js.Serialize(arc.Value.Key.NodeKey2) + ","
                                               + "\"Element\": \"VectorEvent\","
                                               + "\"Frequency\":\"" + js.Serialize(((VectorEvent)arc.Value.Specification).Frequency) + "\"}");   
                            }
                            else if(arcType == typeof(NormalEvent))
                            {
                                outarcList.Add("{\"NodeKey1\":" + js.Serialize(arc.Value.Key.NodeKey1) + ","
                                              + "\"NodeKey2\":" + js.Serialize(arc.Value.Key.NodeKey2) + ","
                                              + "\"Element\": \"NormalEvent\","
                                              + "\"Frequency\":\"" + js.Serialize(((NormalEvent)arc.Value.Specification).Frequency) + "\"}");
                            }
        
                            //prepare json due to state types

                            if(nodeInType == typeof(CorrectState))
                            {
                                newNodesList.Add("{\"Key\":" + js.Serialize(arc.Value.NodeIn.Key) + ","
                                             + "\"Area\":" + js.Serialize(arc.Value.NodeIn.Specification.Area.ToString()) + ","
                                             + "\"FrequencyS\":\"" + js.Serialize(arc.Value.NodeIn.Specification.StudentFrequency) + "\","
                                             + "\"FrequencyE\":\"" + js.Serialize(arc.Value.NodeIn.Specification.EventFrequency) + "\","
                                             + "\"Name\":\"" + ((CorrectState)arc.Value.NodeIn.Specification).Action.Name + "\","
                                             + "\"Element\": \"state\","
                                             + "\"Columns\": "+ columns + ","
                                             + "\"StateType\":\"" + "Correct" + "\"}");
                            }
                            else if (nodeInType == typeof(WorldErrorState))
                            {
                                newNodesList.Add("{\"Key\":" + js.Serialize(arc.Value.NodeIn.Key) + ","
                                             + "\"Area\":" + js.Serialize(arc.Value.NodeIn.Specification.Area.ToString()) + ","
                                             + "\"FrequencyS\":\"" + js.Serialize(arc.Value.NodeIn.Specification.StudentFrequency) + "\","
                                             + "\"FrequencyE\":\"" + js.Serialize(arc.Value.NodeIn.Specification.EventFrequency) + "\","
                                             + "\"Message\":\"" + ((WorldErrorState)arc.Value.NodeIn.Specification).ErrorAssociated.Message.Message + "\","
                                             + "\"Element\": \"state\","
                                             + "\"Columns\": " + columns + ","
                                             + "\"StateType\":\"" + "WorldError" + "\"}");
                            }
                            else if (nodeInType == typeof(TimeErrorState))
                            {
                                var tempState = (TimeErrorState)arc.Value.NodeIn.Specification;
                                newNodesList.Add("{\"Key\":" + js.Serialize(arc.Value.NodeIn.Key) + ","
                                             + "\"Area\":" + js.Serialize(arc.Value.NodeIn.Specification.Area.ToString()) + ","
                                             + "\"FrequencyS\":\"" + js.Serialize(arc.Value.NodeIn.Specification.StudentFrequency) + "\","
                                             + "\"FrequencyE\":\"" + js.Serialize(arc.Value.NodeIn.Specification.EventFrequency) + "\","
                                             + "\"Message\":\"" + tempState.ErrorAssociated.Message.Message + "\","
                                             + "\"Time\":\"" + tempState.Time + "\","
                                             + "\"Element\": \"state\","
                                             + "\"Columns\": " + columns + ","
                                             + "\"StateType\":\"" + "TimeError" + "\"}");
                            }
                            else if (nodeInType == typeof(IncompatibilityErrorState))
                            {
                                var tempState = (IncompatibilityErrorState)arc.Value.NodeIn.Specification;
                                newNodesList.Add("{\"Key\":" + js.Serialize(arc.Value.NodeIn.Key) + ","
                                             + "\"Area\":" + js.Serialize(arc.Value.NodeIn.Specification.Area.ToString()) + ","
                                             + "\"FrequencyS\":\"" + js.Serialize(arc.Value.NodeIn.Specification.StudentFrequency) + "\","
                                             + "\"FrequencyE\":\"" + js.Serialize(arc.Value.NodeIn.Specification.EventFrequency) + "\","
                                             + "\"Message\":\"" + tempState.IncompatibilityFailed.IncompatibilityError.Message.Message + "\","
                                             + "\"Action\":\"" + tempState.IncompatibilityFailed.IncompatibilityAction.Name + "\","
                                             + "\"Element\": \"state\","
                                             + "\"Columns\": " + columns + ","
                                             + "\"StateType\":\"" + "IncompatibilityError" + "\"}");
                            }
                            else if (nodeInType == typeof(OtherErrorState))
                            {
                                newNodesList.Add("{\"Key\":" + js.Serialize(arc.Value.NodeIn.Key) + ","
                                             + "\"Area\":" + js.Serialize(arc.Value.NodeIn.Specification.Area.ToString()) + ","
                                             + "\"FrequencyS\":\"" + js.Serialize(arc.Value.NodeIn.Specification.StudentFrequency) + "\","
                                             + "\"FrequencyE\":\"" + js.Serialize(arc.Value.NodeIn.Specification.EventFrequency) + "\","
                                             + "\"Message\":\"" + ((OtherErrorState)arc.Value.NodeIn.Specification).ErrorAssociated.Message.Message + "\","
                                             + "\"Element\": \"state\","
                                             + "\"Columns\": " + columns + ","
                                             + "\"StateType\":\"" + "OtherError" + "\"}");
                            }
                            else if (nodeInType == typeof(DependenceErrorState))
                            {
                                var tempState = (DependenceErrorState)arc.Value.NodeIn.Specification;
                                nodeInType = tempState.DependenceFailed.GetType();
                                if (nodeInType == typeof(SeqComplexDependence))
                                {
                                    var tempDependence = (SeqComplexDependence)tempState.DependenceFailed;
                                    newNodesList.Add("{\"Key\":" + js.Serialize(arc.Value.NodeIn.Key) + ","
                                            + "\"Area\":" + js.Serialize(arc.Value.NodeIn.Specification.Area.ToString()) + ","
                                            + "\"FrequencyS\":\"" + js.Serialize(arc.Value.NodeIn.Specification.StudentFrequency) + "\","
                                            + "\"FrequencyE\":\"" + js.Serialize(arc.Value.NodeIn.Specification.EventFrequency) + "\","
                                            + "\"Message\":\"" + tempDependence.DependenceError.Message.Message + "\","
                                            + "\"ComplexDependenceKey\":\"" + tempDependence.Key + "\","
                                            + "\"Element\": \"state\","
                                            + "\"Columns\": " + columns + ","
                                            + "\"StateType\":\"" + "SeqComplexDependenceError" + "\"}");
                                }
                                else if (nodeInType == typeof(SimpleDependence))
                                {
                                    var tempDependence = (SimpleDependence)tempState.DependenceFailed;
                                    newNodesList.Add("{\"Key\":" + js.Serialize(arc.Value.NodeIn.Key) + ","
                                            + "\"Area\":" + js.Serialize(arc.Value.NodeIn.Specification.Area.ToString()) + ","
                                            + "\"FrequencyS\":\"" + js.Serialize(arc.Value.NodeIn.Specification.StudentFrequency) + "\","
                                            + "\"FrequencyE\":\"" + js.Serialize(arc.Value.NodeIn.Specification.EventFrequency) + "\","
                                            + "\"Message\":\"" + tempDependence.DependenceError.Message.Message + "\","
                                            + "\"Action\":\"" + tempDependence.ActionDependence.Name + "\","
                                            + "\"Element\": \"state\","
                                            + "\"Columns\": " + columns + ","
                                            + "\"StateType\":\"" + "SimpleDependenceError" + "\"}");
                                }
                            }
                            nextNodes.Add(arc.Value.NodeIn);
                        }
                    }
                    currentNodes.Clear();
                    currentNodes.AddRange(nextNodes);
                    nextNodes.Clear();
                }
                outarcList.AddRange(newNodesList);

                return Json(outarcList, JsonRequestBehavior.AllowGet);
            }
            else
            {
                reJson = "States not found";
                return Json(reJson, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetAllIds(int modo, string strDomainName, int Cluster, string fromNodeKey)
        {
            //int contCol = 0;
            //PredictiveStudentModel model = MapService.getModelByDomain(modo,strDomainName);
            dominio = model.Domain.Actions;
            StudentsCluster cluster = model.GetCluster(Cluster-1);
            int totalStudent = cluster.NumberOfStudents;
            Node<State, Event> fromNode, nodo = null;
            //Node<State, Event> nodeAux = null;
            string reJson = "";
            List<string> outarcList = new List<string>();
            List<string> newNodesList = new List<string>();
            //List<Node<State, Event>> nextNodes = new List<Node<State, Event>>();
            List<Node<State, Event>> currentNodes = new List<Node<State, Event>>();
            List<Node<State, Event>> otherErrorNodes = new List<Node<State, Event>>();
            List<Node<State, Event>> nodesRest = new List<Node<State, Event>>();
            List<Arc<State, Event>> salidaAP = new List<Arc<State, Event>>();
            List<Arc<State, Event>> entradaAP = new List<Arc<State, Event>>();
            List<Arc<State, Event>> cicloAP = new List<Arc<State, Event>>();
            List<Arc<State, Event>> salidaNF = new List<Arc<State, Event>>();
            List<Arc<State, Event>> entradaNF = new List<Arc<State, Event>>();
            List<Arc<State, Event>> cicloNF = new List<Arc<State, Event>>();
            List<Arc<State, Event>> nfToAp = new List<Arc<State, Event>>();
            List<Arc<State, Event>> apToNf = new List<Arc<State, Event>>();
            //List<Node<State, Event>> correct = new List<Node<State, Event>>();
            SortedDictionary<int, Node<State, Event>> found = new SortedDictionary<int, Node<State, Event>>();
            //SortedDictionary<int, List<Node<State, Event>>> relevant = new SortedDictionary<int, List<Node<State, Event>>>();
            SortedDictionary<int, List<string>> notFound = new SortedDictionary<int, List<string>>();
            SortedDictionary<int, List<Node<State, Event>>> map = new SortedDictionary<int, List<Node<State, Event>>>();
            JavaScriptSerializer js = new JavaScriptSerializer();
            if (cluster.StudentActionsModel.TryGetState(fromNodeKey, out fromNode))
            {
                int contador = 0;
                for (int i = 0; i < dominio.Count; i++)
                {
                    Boolean founded = false;
                    string busca = dominio[i].Key + "_CorrectFlow";
                    if (cluster.StudentActionsModel.TryGetState(busca, out nodo))
                    {
                        if (nodo.Specification.Area.ToString() == "CorrectFlow")
                        {
                            //correct.Add(nodo);
                            found.Add(contador, nodo);
                            contador++;
                            founded = true;
                        }
                    }
                    if(!founded)
                    {
                        busca = dominio[i].Key + "_RelevantErrors";
                        if (cluster.StudentActionsModel.TryGetState(busca, out nodo))
                        {
                            if (nodo.Specification.Area.ToString() == "RelevantErrors")
                            {
                                found.Add(contador, nodo);
                                contador++;
                                founded = true;
                            }
                            /*List<Node<State, Event>> rele = new List<Node<State, Event>>();
                            if (!relevant.ContainsKey(correct.Count))
                            {
                                rele.Add(nodo);
                                relevant.Add(correct.Count, rele);
                            }
                            else {
                                rele = relevant[correct.Count];
                                relevant.Remove(correct.Count);
                                rele.Add(nodo);
                                relevant.Add(correct.Count, rele);
                            }*/
                        }
                    }
                    if(!founded)
                    {
                        List<string> nF = new List<string>();
                        if (!notFound.ContainsKey(found.Count))
                        {
                            nF.Add(dominio[i].Key);
                            notFound.Add(found.Count, nF);
                        }
                        else
                        {
                            nF = notFound[found.Count];
                            notFound.Remove(found.Count);
                            nF.Add(dominio[i].Key);
                            notFound.Add(found.Count, nF);
                        }
                    }
                }
                //newNodes.Add(js.Serialize(fromNode.Specification));
                nodesRest.Add(fromNode);
                Boolean fin = false;
                //correct.Insert(0, fromNode);
                //correct.Add(0, fromNode);
                List<Node<State, Event>> l0 = new List<Node<State, Event>>();
                l0.Add(fromNode);
                map.Add(0, l0);
                //Error en el foreach, mirar a ver a que se debe
                while(!fin)
                {
                    //int columns = i;
                    var node = nodesRest[0];
                    //foreach (var node in nodesRest)
                    //{
                    currentNodes.Add(node);
                    int nOfOutArcs = node.OutArcs.Count;
                    foreach (var arc in node.OutArcs)
                    {
                        //prepare json due to arc types
                        Type arcType = arc.Value.Specification.GetType();
                        Type nodeInType = arc.Value.NodeIn.Specification.GetType();
                        Type nodeOutType = arc.Value.NodeOut.Specification.GetType();
                        if (arcType == typeof(VectorEvent))
                        {
                            if(nodeInType!=typeof(OtherErrorState) && nodeOutType != typeof(OtherErrorState)){
                                outarcList.Add("{\"NodeKey1\":" + js.Serialize(arc.Value.Key.NodeKey1) + ","
                                    + "\"NodeKey2\":" + js.Serialize(arc.Value.Key.NodeKey2) + ","
                                    + "\"Element\": \"VectorEvent\","
                                    + "\"Frequency\":\"" + js.Serialize(((VectorEvent)arc.Value.Specification).Frequency) + "\"}"); 
                            }
                        }
                        else if (arcType == typeof(NormalEvent))
                        {
                            if(nodeInType!=typeof(OtherErrorState) && nodeOutType != typeof(OtherErrorState)){
                                outarcList.Add("{\"NodeKey1\":" + js.Serialize(arc.Value.Key.NodeKey1) + ","
                                    + "\"NodeKey2\":" + js.Serialize(arc.Value.Key.NodeKey2) + ","
                                    + "\"Element\": \"NormalEvent\","
                                    + "\"Frequency\":\"" + js.Serialize(((NormalEvent)arc.Value.Specification).Frequency) + "\"}");
                            }
                        }

                        //prepare json due to state types

                        if (nodeInType == typeof(CorrectState))
                        {
                            if (!currentNodes.Contains(arc.Value.NodeIn) && !nodesRest.Contains(arc.Value.NodeIn))
                            {
                                nodesRest.Add(arc.Value.NodeIn);
                                Boolean encontrado = false;
                                int columna = 0;
                                string clave = arc.Value.NodeIn.Key;
                                int p = clave.IndexOf('_');
                                //int u = clave.LastIndexOf('_');
                                clave = clave.Substring(0, p);
                                columna = busquedaPosicion(clave, found, notFound);
                                /*for (int i = 0; i < found.Count && !encontrado; i++)
                                {
                                    if (found[i].Key == clave)
                                    {
                                        columna = i + 1;
                                        encontrado = true;
                                    }
                                }*/
                                /*if (!encontrado && arc.Value.NodeIn.Specification.Area.ToString().Equals("RelevantErrors"))
                                {
                                    List<Node<State, Event>> posi = new List<Node<State, Event>>();
                                    foreach(KeyValuePair<int,List<Node<State,Event>>> entry in relevant)
                                    {
                                        posi = entry.Value;
                                        for (int i = 0; i < posi.Count && !encontrado; i++)
                                        {
                                            if (posi[i].Key == arc.Value.NodeIn.Key)
                                            {
                                                columna = entry.Key;
                                                encontrado = true;
                                            }
                                            if (encontrado)
                                                break;
                                        }
                                    }
                                }*/
                                //nodesRest.Add(arc.Value.NodeIn);
                                List<Node<State, Event>> col = new List<Node<State, Event>>();
                                if (!map.ContainsKey(columna))
                                {
                                    col.Add(arc.Value.NodeIn);
                                    map.Add(columna, col);
                                }
                                else
                                {
                                    col = map[columna];
                                    map.Remove(columna);
                                    col.Add(arc.Value.NodeIn);
                                    map.Add(columna, col);
                                }
                                //Name o Key (crear log contiene f0?? no/si) es la buena para coger el nombre
                                //Key parece la mejor opción
                                newNodesList.Add("{\"Key\":" + js.Serialize(arc.Value.NodeIn.Key) + ","
                                    + "\"Area\":" + js.Serialize(arc.Value.NodeIn.Specification.Area.ToString()) + ","
                                    + "\"FrequencyS\":\"" + js.Serialize(arc.Value.NodeIn.Specification.StudentFrequency) + "\","
                                    + "\"FrequencyE\":\"" + js.Serialize(arc.Value.NodeIn.Specification.EventFrequency) + "\","
                                    + "\"Name\":\"" + ((CorrectState)arc.Value.NodeIn.Specification).Action.Name + "\","
                                    + "\"Element\": \"state\","
                                    + "\"Column\": " + columna + ","
                                    + "\"StateType\":\"" + "Correct" + "\"}");
                                //cluster.StudentActionsModel.TryGetState(fromNodeKey, out fromNode)
                            }
                        }
                        else if (nodeInType == typeof(WorldErrorState))
                        {
                            //Primera parte de arc.Value.NodeIn.Key
                            if (!currentNodes.Contains(arc.Value.NodeIn) && !nodesRest.Contains(arc.Value.NodeIn))
                            {
                                //No hay otra forma más que cortando la Key
                                nodesRest.Add(arc.Value.NodeIn);
                                int c = 0;
                                string clave = arc.Value.NodeIn.Key;
                                int p = clave.IndexOf('_');
                                //int u = clave.LastIndexOf('_');
                                clave = clave.Substring(0, p);
                                c = busquedaPosicion(clave, found, notFound);
                                List<Node<State, Event>> col = new List<Node<State, Event>>();
                                if (!map.ContainsKey(c))
                                {
                                    col.Add(arc.Value.NodeIn);
                                    map.Add(c, col);
                                }
                                else
                                {
                                    col = map[c];
                                    map.Remove(c);
                                    col.Add(arc.Value.NodeIn);
                                    map.Add(c, col);
                                }
                                newNodesList.Add("{\"Key\":" + js.Serialize(arc.Value.NodeIn.Key) + ","
                                + "\"Area\":" + js.Serialize(arc.Value.NodeIn.Specification.Area.ToString()) + ","
                                + "\"FrequencyS\":\"" + js.Serialize(arc.Value.NodeIn.Specification.StudentFrequency) + "\","
                                + "\"FrequencyE\":\"" + js.Serialize(arc.Value.NodeIn.Specification.EventFrequency) + "\","
                                + "\"Message\":\"" + ((WorldErrorState)arc.Value.NodeIn.Specification).ErrorAssociated.Message.Message + "\","
                                + "\"Element\": \"state\","
                                + "\"Column\": " + c + ","
                                + "\"StateType\":\"" + "WorldError" + "\"}");
                            }
                        }
                        else if (nodeInType == typeof(TimeErrorState))
                        {
                            if (!currentNodes.Contains(arc.Value.NodeIn) && !nodesRest.Contains(arc.Value.NodeIn))
                            {
                                nodesRest.Add(arc.Value.NodeIn);
                                int c = 0;
                                string clave = arc.Value.NodeIn.Key;
                                int p = clave.IndexOf('_');
                                int u = clave.LastIndexOf('_');
                                clave = clave.Substring(p + 1, u - p - 1);
                                c = busquedaPosicion(clave, found, notFound);
                                List<Node<State, Event>> col = new List<Node<State, Event>>();
                                if (!map.ContainsKey(c))
                                {
                                    col.Add(arc.Value.NodeIn);
                                    map.Add(c, col);
                                }
                                else
                                {
                                    col = map[c];
                                    map.Remove(c);
                                    col.Add(arc.Value.NodeIn);
                                    map.Add(c, col);
                                }
                                var tempState = (TimeErrorState)arc.Value.NodeIn.Specification;
                                newNodesList.Add("{\"Key\":" + js.Serialize(arc.Value.NodeIn.Key) + ","
                                    + "\"Area\":" + js.Serialize(arc.Value.NodeIn.Specification.Area.ToString()) + ","
                                    + "\"FrequencyS\":\"" + js.Serialize(arc.Value.NodeIn.Specification.StudentFrequency) + "\","
                                    + "\"FrequencyE\":\"" + js.Serialize(arc.Value.NodeIn.Specification.EventFrequency) + "\","
                                    + "\"Message\":\"" + tempState.ErrorAssociated.Message.Message + "\","
                                    + "\"Time\":\"" + tempState.Time + "\","
                                    + "\"Element\": \"state\","
                                    + "\"Column\": " + c + ","
                                    + "\"StateType\":\"" + "TimeError" + "\"}");
                            }
                        }
                        else if (nodeInType == typeof(IncompatibilityErrorState))
                        {
                            if (!currentNodes.Contains(arc.Value.NodeIn) && !nodesRest.Contains(arc.Value.NodeIn))
                            {
                                nodesRest.Add(arc.Value.NodeIn);
                                int c = 0;
                                var tempState = (IncompatibilityErrorState)arc.Value.NodeIn.Specification;
                                string clave = arc.Value.NodeIn.Key;
                                int p = clave.IndexOf('_');
                                clave = clave.Substring(0, p);
                                c = busquedaPosicion(clave, found, notFound);
                                List<Node<State, Event>> col = new List<Node<State, Event>>();
                                if (!map.ContainsKey(c))
                                {
                                    col.Add(arc.Value.NodeIn);
                                    map.Add(c, col);
                                }
                                else
                                {
                                    col = map[c];
                                    map.Remove(c);
                                    col.Add(arc.Value.NodeIn);
                                    map.Add(c, col);
                                }
                                newNodesList.Add("{\"Key\":" + js.Serialize(arc.Value.NodeIn.Key) + ","
                                     + "\"Area\":" + js.Serialize(arc.Value.NodeIn.Specification.Area.ToString()) + ","
                                     + "\"FrequencyS\":\"" + js.Serialize(arc.Value.NodeIn.Specification.StudentFrequency) + "\","
                                     + "\"FrequencyE\":\"" + js.Serialize(arc.Value.NodeIn.Specification.EventFrequency) + "\","
                                     + "\"Message\":\"" + tempState.IncompatibilityFailed.IncompatibilityError.Message.Message + "\","
                                     + "\"Action\":\"" + tempState.IncompatibilityFailed.IncompatibilityAction.Name + "\","
                                     + "\"Element\": \"state\","
                                     + "\"Column\": " + c + ","
                                     + "\"StateType\":\"" + "IncompatibilityError" + "\"}");
                            }
                        }
                        else if (nodeInType == typeof(OtherErrorState))
                        {
                            if (!currentNodes.Contains(arc.Value.NodeIn) && !nodesRest.Contains(arc.Value.NodeIn))
                            {
                                otherErrorNodes.Add(arc.Value.NodeIn);
                                //Message es la buena o Key del ErrorAssociated también es valida
                                nodesRest.Add(arc.Value.NodeIn);
                                /*newNodesList.Add("{\"Key\":" + js.Serialize(arc.Value.NodeIn.Key) + ","
                                 + "\"Area\":" + js.Serialize(arc.Value.NodeIn.Specification.Area.ToString()) + ","
                                 //+ "\"Frequency\":\"" + js.Serialize(arc.Value.NodeIn.Specification.Frequency) + "\","
                                 //+ "\"Message\":\"" + ((OtherErrorState)arc.Value.NodeIn.Specification).ErrorAssociated.Message.Message + "\","
                                 + "\"Element\": \"state\","
                                 //+ "\"Columns\": " + columns + ","
                                 + "\"StateType\":\"" + "OtherError" + "\"}");*/
                            }
                        }
                        else if (nodeInType == typeof(DependenceErrorState))
                        {
                            var tempState = (DependenceErrorState)arc.Value.NodeIn.Specification;
                            nodeInType = tempState.DependenceFailed.GetType();
                            if (nodeInType == typeof(SeqComplexDependence))
                            {
                                if (!currentNodes.Contains(arc.Value.NodeIn) && !nodesRest.Contains(arc.Value.NodeIn))
                                {
                                    //tempState.DependenceFailed.DependenciesList[0].Key
                                    //Es una lista con varias claves lo que contiene DependanceFailed.Key
                                    nodesRest.Add(arc.Value.NodeIn);
                                    int c = 0;
                                    var tempDependence = (SeqComplexDependence)tempState.DependenceFailed;
                                    string clave = arc.Value.NodeIn.Key;
                                    int hay = clave.IndexOf('_');
                                    clave = clave.Substring(0, hay);
                                    c = busquedaPosicion(clave, found, notFound);
                                    List<Node<State, Event>> col = new List<Node<State, Event>>();
                                    if (!map.ContainsKey(c))
                                    {
                                        col.Add(arc.Value.NodeIn);
                                        map.Add(c, col);
                                    }
                                    else
                                    {
                                        col = map[c];
                                        map.Remove(c);
                                        col.Add(arc.Value.NodeIn);
                                        map.Add(c, col);
                                    }
                                    newNodesList.Add("{\"Key\":" + js.Serialize(arc.Value.NodeIn.Key) + ","
                                        + "\"Area\":" + js.Serialize(arc.Value.NodeIn.Specification.Area.ToString()) + ","
                                        + "\"FrequencyS\":\"" + js.Serialize(arc.Value.NodeIn.Specification.StudentFrequency) + "\","
                                        + "\"FrequencyE\":\"" + js.Serialize(arc.Value.NodeIn.Specification.EventFrequency) + "\","
                                        + "\"Message\":\"" + tempDependence.DependenceError.Message.Message + "\","
                                        + "\"ComplexDependenceKey\":\"" + tempDependence.Key + "\","
                                        + "\"Element\": \"state\","
                                        + "\"Column\": " + c + ","
                                        + "\"StateType\":\"" + "SeqComplexDependenceError" + "\"}");
                                }
                            }
                            else if (nodeInType == typeof(SimpleDependence))
                            {
                                if (!currentNodes.Contains(arc.Value.NodeIn) && !nodesRest.Contains(arc.Value.NodeIn))
                                {
                                    //arc.Value.NodeIn.Specification.DependenceFailed.Key contiene la clave para la unificación
                                    nodesRest.Add(arc.Value.NodeIn);
                                    int c = 0;
                                    var tempDependence = (SimpleDependence)tempState.DependenceFailed;
                                    string clave = arc.Value.NodeIn.Key;
                                    int hay = clave.IndexOf('_');
                                    clave = clave.Substring(0, hay);
                                    c = busquedaPosicion(clave, found, notFound);
                                    List<Node<State, Event>> col = new List<Node<State, Event>>();
                                    if (!map.ContainsKey(c))
                                    {
                                        col.Add(arc.Value.NodeIn);
                                        map.Add(c, col);
                                    }
                                    else
                                    {
                                        col = map[c];
                                        map.Remove(c);
                                        col.Add(arc.Value.NodeIn);
                                        map.Add(c, col);
                                    }
                                    newNodesList.Add("{\"Key\":" + js.Serialize(arc.Value.NodeIn.Key) + ","
                                        + "\"Area\":" + js.Serialize(arc.Value.NodeIn.Specification.Area.ToString()) + ","
                                        + "\"FrequencyS\":\"" + js.Serialize(arc.Value.NodeIn.Specification.StudentFrequency) + "\","
                                        + "\"FrequencyE\":\"" + js.Serialize(arc.Value.NodeIn.Specification.EventFrequency) + "\","
                                        + "\"Message\":\"" + tempDependence.DependenceError.Message.Message + "\","
                                        + "\"Action\":\"" + tempDependence.ActionDependence.Name + "\","
                                        + "\"Element\": \"state\","
                                        + "\"Column\": " + c + ","
                                        + "\"StateType\":\"" + "SimpleDependenceError" + "\"}");
                                }
                            }
                        }
                        //nextNodes.Add(arc.Value.NodeIn);
                    }
                    nodesRest.Remove(node);
                    if (!nodesRest.Any())
                        fin = true;
                    //}
                    //currentNodes.AddRange(nextNodes);
                    //nextNodes.Clear();
                }
                foreach(var nodo1 in otherErrorNodes)
                {
                    //((OtherErrorState)arc.Value.NodeIn.Specification).ErrorAssociated.
                    string clave = ((OtherErrorState)nodo1.Specification).ErrorAssociated.Key;
                    //arc.Value.NodeIn.Specification.GetType()
                    if (clave == "actionalreadyperformed") {
                        foreach (var arco in nodo1.OutArcs)
                        {
                            var nodoSal = arco.Value.NodeIn;
                            if (nodoSal.Specification.GetType() == typeof(OtherErrorState))
                            {
                                string clave2 = ((OtherErrorState)nodoSal.Specification).ErrorAssociated.Key;
                                if (clave == clave2)
                                    cicloAP.Add(arco.Value);
                                else if (clave2 == "actionnotfound")
                                    apToNf.Add(arco.Value);
                                //Ampliar si hubiera más clases de OtherErrorState
                            }
                            else
                                salidaAP.Add(arco.Value);
                        }
                        foreach (var arco in nodo1.InArcs)
                        {
                            var nodoSal = arco.Value.NodeOut;
                            if(nodoSal.Specification.GetType() == typeof(OtherErrorState))
                            {
                                string clave2 = ((OtherErrorState)nodoSal.Specification).ErrorAssociated.Key;
                                if (clave == clave2)
                                    cicloAP.Add(arco.Value);
                                else if (clave2 == "actionnotfound")
                                    nfToAp.Add(arco.Value);
                            }
                            else
                                entradaAP.Add(arco.Value);
                        }
                    }
                    else if (clave == "actionnotfound") {
                        foreach (var arco in nodo1.OutArcs)
                        {
                            var nodoSal = arco.Value.NodeIn;
                            if(nodoSal.Specification.GetType() == typeof(OtherErrorState))
                            {
                                string clave2 = ((OtherErrorState)nodoSal.Specification).ErrorAssociated.Key;
                                if (clave == clave2)
                                    cicloNF.Add(arco.Value);
                                else if (clave2 != "actionalreadyperformed")
                                    salidaNF.Add(arco.Value);
                            }
                            else
                                salidaNF.Add(arco.Value);
                        }
                        foreach (var arco in nodo1.InArcs)
                        {
                            var nodoSal = arco.Value.NodeOut;
                            if (nodoSal.Specification.GetType() == typeof(OtherErrorState))
                            {
                                string clave2 = ((OtherErrorState)nodoSal.Specification).ErrorAssociated.Key;
                                if (clave == clave2)
                                    cicloNF.Add(arco.Value);
                                else if (clave2 != "actionalreadyperformed")
                                    entradaNF.Add(arco.Value);
                            }
                            else
                                entradaNF.Add(arco.Value);
                        }
                    }
                }//todos los nodos recorridos, ahora recorrer todos los arcos para filtrar la info necesaria
                //totalStudent y dominio
                int division = totalStudent / 5; //Es la cantidad de de ActionAlreadyPerformed y ActionNotFound que habrá
                /*while (c < correct.Count)
                {
                    if (correct[c] == null)
                        correct.RemoveAt(c);
                    else
                        c++;
                }*/
                outarcList.AddRange(ordenar(salidaAP, true, false, map));
                outarcList.AddRange(ordenar(entradaAP, false, false, map));
                outarcList.AddRange(ordenar(salidaNF, true, true, map));
                outarcList.AddRange(ordenar(entradaNF, false, true, map));
                string info = "";
                float freq = 0;
                int cont = 0;
                foreach(var arco in nfToAp)
                {
                    if (cont==0)
                    {
                        int hasta1 = arco.NodeOut.Key.LastIndexOf("_");
                        int hasta2 = arco.NodeIn.Key.LastIndexOf("_");
                        if (hasta1 == -1)
                            info = arco.NodeOut.Key + " ==> ";
                        else
                            info = arco.NodeOut.Key.Substring(0, hasta1) + " ==> ";
                        if (hasta2 == -1)
                            info = info + arco.NodeIn.Key;
                        else
                            info = info + arco.NodeIn.Key.Substring(0, hasta2);
                    }
                    else
                    {
                        int hasta1 = arco.NodeOut.Key.LastIndexOf("_");
                        int hasta2 = arco.NodeIn.Key.LastIndexOf("_");
                        if (hasta1 == -1)
                            info = info + "====" + arco.NodeOut.Key + " ==> ";
                        else
                            info = info + "====" + arco.NodeOut.Key.Substring(0, hasta1) + " ==> ";
                        if (hasta2 == -1)
                            info = info + arco.NodeIn.Key;
                        else
                            info = info + arco.NodeIn.Key.Substring(0, hasta2);
                    }
                    if (arco.Specification.GetType() == typeof(VectorEvent))
                    {
                        info = info + "====" + "[";
                        float aux = 0;
                        for (int k = 0; k < ((VectorEvent)arco.Specification).Frequency.Count(); k++) {
                            aux += ((VectorEvent)arco.Specification).Frequency[k];
                            info = info + ((VectorEvent)arco.Specification).Frequency[k] + ",";
                        }
                        info = info + "]";
                        freq += aux / ((VectorEvent)arco.Specification).Frequency.Count();
                    }
                    else
                    {
                        info = info + "====" + ((NormalEvent)arco.Specification).Frequency;
                        freq += ((NormalEvent)arco.Specification).Frequency;
                    }
                    cont++;
                }
                outarcList.Add("{\"NodeKey1\": \"ActionNotFound1\","
                                    + "\"NodeKey2\": \"ActionAlreadyPerformed1\","
                                    + "\"Element\": \"GroupEvent\","
                                    //+ "\"Info\":\"" + info + "\","
                                    + "\"Frequency\":\"" + js.Serialize(freq) + "\"}");
                info = "";
                freq = 0;
                cont = 0;
                foreach (var arco in apToNf)
                {
                    if (cont == 0)
                    {
                        int hasta1 = arco.NodeOut.Key.LastIndexOf("_");
                        int hasta2 = arco.NodeIn.Key.LastIndexOf("_");
                        if (hasta1 == -1)
                            info = arco.NodeOut.Key + " ==> ";
                        else
                            info = arco.NodeOut.Key.Substring(0, hasta1) + " ==> ";
                        if (hasta2 == -1)
                            info = info + arco.NodeIn.Key;
                        else
                            info = info + arco.NodeIn.Key.Substring(0, hasta2);
                    }
                    else
                    {
                        int hasta1 = arco.NodeOut.Key.LastIndexOf("_");
                        int hasta2 = arco.NodeIn.Key.LastIndexOf("_");
                        if (hasta1 == -1)
                            info = info + "====" + arco.NodeOut.Key + " ==> ";
                        else
                            info = info + "====" + arco.NodeOut.Key.Substring(0, hasta1) + " ==> ";
                        if (hasta2 == -1)
                            info = info + arco.NodeIn.Key;
                        else
                            info = info + arco.NodeIn.Key.Substring(0, hasta2);
                    }
                    if (arco.Specification.GetType() == typeof(VectorEvent))
                    {
                        info = info + "====" + "[";
                        float aux = 0;
                        for (int k = 0; k < ((VectorEvent)arco.Specification).Frequency.Count(); k++)
                        {
                            aux += ((VectorEvent)arco.Specification).Frequency[k];
                            info = info + ((VectorEvent)arco.Specification).Frequency[k] + ",";
                        }
                        info = info + "]";
                        freq += aux / ((VectorEvent)arco.Specification).Frequency.Count();
                    }
                    else
                    {
                        info = info + "====" + ((NormalEvent)arco.Specification).Frequency;
                        freq += ((NormalEvent)arco.Specification).Frequency;
                    }
                    cont++;
                }
                outarcList.Add("{\"NodeKey1\": \"ActionAlreadyPerformed0\","
                                    + "\"NodeKey2\": \"ActionNotFound0\","
                                    + "\"Element\": \"GroupEvent\","
                                    //+ "\"Info\":\"" + info + "\","
                                    + "\"Frequency\":\"" + js.Serialize(freq) + "\"}");
                for (int a = 0; a <= found.Count / 5; a++)
                {
                    int d = a*5 + 2;
                    newNodesList.Add("{\"Key\": \"ActionNotFound" + a + "\","
                                 + "\"Area\": \"IrrelevantErrors\","
                                 + "\"Element\": \"state\","
                                 + "\"Column\":" + js.Serialize(d) + ","
                                 + "\"StateType\":\"" + "OtherError" + "\"}");
                    newNodesList.Add("{\"Key\": \"ActionAlreadyPerformed"+ a +"\","
                                     + "\"Area\": \"IrrelevantErrors\","
                                     + "\"Element\": \"state\","
                                     + "\"Column\":" + js.Serialize(d) + ","
                                     + "\"StateType\":\"" + "OtherError" + "\"}");
                }
                outarcList.AddRange(newNodesList);
                return Json(outarcList, JsonRequestBehavior.AllowGet);
            }
            else
            {
                reJson = "States not found";
                return Json(reJson, JsonRequestBehavior.AllowGet);
            }
        }

        private List<string> ordenar(List<Arc<State, Event>> ini, bool salida, bool nf, SortedDictionary<int, List<Node<State, Event>>> map)
        {
            List<string> arcosNuevos = new List<string>();
            string[] info = new string[ini.Count()];
            float[] freq = new float[ini.Count()];
            int i = 0;
            bool fin = false;
            List<Arc<State, Event>> ordenada = new List<Arc<State, Event>>();
            JavaScriptSerializer js = new JavaScriptSerializer();
            while (i<ini.Count() && !fin)
            {
                var actual = ini[i];
                int j = i + 1;
                int hasta1 = actual.NodeOut.Key.LastIndexOf("_");
                int hasta2 = actual.NodeIn.Key.LastIndexOf("_");
                if (hasta1 == -1)
                    info[i] = actual.NodeOut.Key + " ==> ";
                else
                    info[i] = actual.NodeOut.Key.Substring(0, hasta1) + " ==> ";
                if(hasta2==-1)
                    info[i] = info[i] + actual.NodeIn.Key;
                else
                    info[i] = info[i] + actual.NodeIn.Key.Substring(0, hasta2);
                if (actual.Specification.GetType() == typeof(VectorEvent))
                {
                    info[i] = info[i] + "====" + "[";
                    for (int k = 0; k < ((VectorEvent)actual.Specification).Frequency.Count(); k++)
                    {
                        info[i] = info[i] + ((VectorEvent)actual.Specification).Frequency[k] + ",";
                        freq[i] += ((VectorEvent)actual.Specification).Frequency[k];
                    }
                    info[i] = info[i] + "]";
                    freq[i] = freq[i] / ((VectorEvent)actual.Specification).Frequency.Count();
                }
                else
                {
                    info[i] = info[i] + "====" + ((NormalEvent)actual.Specification).Frequency;
                    freq[i] = ((NormalEvent)actual.Specification).Frequency;
                }
                //Unificar que todos los nodos que salgan o vayan a un mismo nodo
                while (j<ini.Count())
                {
                    var nueva = ini[j];
                    if((salida && actual.NodeIn.Equals(nueva.NodeIn)) || (!salida && actual.NodeOut.Equals(nueva.NodeOut)))
                    {
                        int hasta11 = nueva.NodeOut.Key.LastIndexOf("_");
                        int hasta21 = nueva.NodeIn.Key.LastIndexOf("_");
                        if (hasta11 == -1)
                            info[i] = info[i] + "====" + nueva.NodeOut.Key + " ==> ";
                        else
                            info[i] = info[i] + "====" + nueva.NodeOut.Key.Substring(0, hasta11) + " ==> ";
                        if (hasta21 == -1)
                            info[i] = info[i] + nueva.NodeIn.Key;
                        else
                            info[i] = info[i] + nueva.NodeIn.Key.Substring(0, hasta21);
                        float freqN = 0;
                        if (nueva.Specification.GetType() == typeof(VectorEvent))
                        {
                            info[i] = info[i] + "====" + "[";
                            for (int k = 0; k < ((VectorEvent)nueva.Specification).Frequency.Count(); k++)
                            {
                                info[i] = info[i] + ((VectorEvent)nueva.Specification).Frequency[k] + ",";
                                freqN += ((VectorEvent)nueva.Specification).Frequency[k];
                            }
                            info[i] = info[i] + "]";
                            freqN = freqN / ((VectorEvent)nueva.Specification).Frequency.Count();
                        }
                        else
                        {
                            freqN = ((NormalEvent)nueva.Specification).Frequency;
                            info[i] = info[i] + "====" + ((NormalEvent)actual.Specification).Frequency;
                        }
                        freq[i] += freqN;
                        ini.Remove(nueva);
                    }
                    else
                        j++;
                }
                i++;
            }
            for(int l = 0; l<ini.Count(); l++)
            {
                /*
                if (!map.ContainsKey(columna))
                {
                    col.Add(arc.Value.NodeIn)
                    map.Add(columna, col);
                }
                else
                                    {
                                        col = map[c];
                                        map.Remove(c);
                                        col.Add(arc.Value.NodeIn);
                                        map.Add(c, col);
                                    }
                */
                var arc = ini[l];
                string nodekey1 = arc.Key.NodeKey1; //NodeOut
                string nodekey2 = arc.Key.NodeKey2; //NodeIn
                if (salida)
                {
                    //int hasta = nodekey2.IndexOf("_");
                    //string prueba = nodekey2;
                    //if (hasta != -1)
                    //    prueba = nodekey2.Substring(0, hasta);
                    int pos = -1;
                    bool encontrado = false;
                    //if (correctos[0].Key == prueba)
                    //    pos = 0;
                    //else
                    for (int b = 1; b < map.Count && !encontrado; b++)
                    {
                        int tt;
                        if (!map.ContainsKey(b))
                            tt = 0;
                        List<Node<State, Event>> mapi = map[b];
                        for (int b1 = 0; b1<mapi.Count && !encontrado; b1++)
                        {
                            string clavePos = mapi[b1].Key;
                            if (clavePos.Equals(nodekey2))
                            {
                                pos = b;
                                encontrado = true;
                            }
                        }
                    }
                    pos = pos / 5;
                    //string clave = ((CorrectState)arc.NodeIn.Specification).Action.Key;
                    if (nf)
                        nodekey1 = "ActionNotFound" + pos;
                    else
                        nodekey1 = "ActionAlreadyPerformed" + pos;
                }
                else
                {
                    //int hasta = nodekey1.IndexOf("_");
                    //string prueba = nodekey1;
                    //if (hasta != -1)
                    //    prueba = nodekey1.Substring(0, hasta);
                    int pos = -1;
                    bool encontrado = false;
                    //if (correctos[0].Key == prueba)
                    //    pos = 0;
                    //else
                    for (int b = 1; b < map.Count && !encontrado; b++)
                    {
                        int tt;
                        if (!map.ContainsKey(b))
                            tt = 0;
                        List<Node<State, Event>> mapi = map[b];
                        for (int b1 = 0; b1 < mapi.Count && !encontrado; b1++)
                        {
                            string clavePos = mapi[b1].Key;
                            if (clavePos.Equals(nodekey1))
                            {
                                pos = b;
                                encontrado = true;
                            }
                        }
                    }
                    pos = pos / 5;
                    if (nf)
                        nodekey2 = "ActionNotFound" + pos;
                    else
                        nodekey2 = "ActionAlreadyPerformed" + pos;
                }
                arcosNuevos.Add("{\"NodeKey1\":" + js.Serialize(nodekey1) + ","
                                    + "\"NodeKey2\":" + js.Serialize(nodekey2) + ","
                                    + "\"Element\": \"GroupEvent\","
                                    + "\"Info\":\"" + info[l] + "\","
                                    + "\"Frequency\":\"" + js.Serialize(freq[l]) + "\"}");
            }
            return arcosNuevos;
        }

        public ActionResult GetIniNode(int modo, string strDomainName, int Cluster)
        {
            
            //PredictiveStudentModel model = MapService.getModelByDomain(modo,strDomainName);
            StudentsCluster cluster = model.GetCluster(Cluster-1);
            Node<State, Event> iniNode = cluster.StudentActionsModel.InitState;
            JavaScriptSerializer js = new JavaScriptSerializer();
            var re = "{\"Key\":" + js.Serialize(iniNode.Key)+","
                      + "\"Area\":" + js.Serialize(iniNode.Specification.Area.ToString()) + ","
                      + "\"Element\": \"state\","
                      + "\"FrequencyS\":\"" + js.Serialize(iniNode.Specification.StudentFrequency) + "\","
                      + "\"FrequencyE\":\"" + js.Serialize(iniNode.Specification.EventFrequency) + "\","
                      + "\"TotalEvents\":" + cluster.NumberOfEvents + ","
                      + "\"TotalStudent\":"+ cluster.NumberOfStudents+"}";
            return Json(re, JsonRequestBehavior.AllowGet);
            //return new JSONPResult { Data = re };  //返回 jsonp 数据，输出回调函数
        }

        public ActionResult GetNodeStudents(string strDomainName, int Cluster, string StudentKey, int modo)
        {
            //PredictiveStudentModel model = MapService.getModelByDomain(modo,strDomainName);
            StudentsCluster cluster = model.GetCluster(Cluster - 1);
            dominio = model.Domain.Actions;
            //string student = cluster.GetStudentKeys()[Student-1];
            string student = StudentKey;
            Node<State, Event> nodo = null;
            int lastC = 0;
            SortedDictionary<int, Node<State, Event>> found = new SortedDictionary<int, Node<State, Event>>();
            //List<Node<State, Event>> correct = new List<Node<State, Event>>();
            //SortedDictionary<int, List<Node<State, Event>>> relevant = new SortedDictionary<int, List<Node<State, Event>>>();
            SortedDictionary<int, List<string>> notFound = new SortedDictionary<int, List<string>>();
            SortedDictionary<int, List<Node<State, Event>>> map = new SortedDictionary<int, List<Node<State, Event>>>();
            JavaScriptSerializer js = new JavaScriptSerializer();
            List<string> newNodesList = new List<string>();
            Node<State, Event> iniNode = cluster.StudentActionsModel.InitState;
            newNodesList.Add("{\"Key\":" + js.Serialize(iniNode.Key) + ","
                      + "\"Area\":" + js.Serialize(iniNode.Specification.Area.ToString()) + ","
                      + "\"Element\": \"state\","
                      + "\"Frequency\":\"" + "1" + "\","
                      + "\"StateType\":\"" + "Init" + "\"}");
            int contador = 0;
            for (int i = 0; i < dominio.Count; i++)
            {
                bool founded = false;
                string busca = dominio[i].Key + "_CorrectFlow";
                if (cluster.StudentActionsModel.TryGetState(busca, out nodo))
                {
                    if (nodo.Specification.Area.ToString() == "CorrectFlow")
                    {
                        //correct.Add(nodo);
                        found.Add(contador, nodo);
                        contador++;
                        founded = true;
                    }
                }
                if(!founded)
                {
                    busca = dominio[i].Key + "_RelevantErrors";
                    if (cluster.StudentActionsModel.TryGetState(busca, out nodo))
                    {
                        if (nodo.Specification.Area.ToString() == "RelevantErrors")
                        {
                            found.Add(contador, nodo);
                            contador++;
                            founded = true;
                        }
                        /*List<Node<State, Event>> rele = new List<Node<State, Event>>();
                        if (!relevant.ContainsKey(correct.Count))
                        {
                            rele.Add(nodo);
                            relevant.Add(correct.Count, rele);
                        }
                        else {
                            rele = relevant[correct.Count];
                            relevant.Remove(correct.Count);
                            rele.Add(nodo);
                            relevant.Add(correct.Count, rele);
                        }*/
                    }
                }
                if(!founded)
                {
                    List<string> nF = new List<string>();
                    if (!notFound.ContainsKey(found.Count))
                    {
                        nF.Add(dominio[i].Key);
                        notFound.Add(found.Count, nF);
                    }
                    else
                    {
                        nF = notFound[found.Count];
                        notFound.Remove(found.Count);
                        nF.Add(dominio[i].Key);
                        notFound.Add(found.Count, nF);
                    }
                }
            }
            List<Node<State, Event>> nodes = cluster.GetStudentNodes(student);
            nodo = null;
            for (int k = 0; k < nodes.Count; k++)
            {
                nodo = nodes[k];
                Type nodeInType = nodo.Specification.GetType();
                if (nodeInType == typeof(CorrectState))
                {
                    Boolean encontrado = false;
                    int columna = 0;
                    string clave = nodo.Key;
                    int p = clave.IndexOf('_');
                    clave = clave.Substring(0, p);
                    columna = busquedaPosicion(clave, found, notFound);
                    /*if (!encontrado && nodo.Specification.Area.ToString().Equals("RelevantErrors"))
                    {
                        List<Node<State, Event>> posi = new List<Node<State, Event>>();
                        foreach (KeyValuePair<int, List<Node<State, Event>>> entry in relevant)
                        {
                            posi = entry.Value;
                            for (int i = 0; i < posi.Count && !encontrado; i++)
                            {
                                if (posi[i].Key == nodo.Key)
                                {
                                    columna = entry.Key;
                                    encontrado = true;
                                }
                                if (encontrado)
                                    break;
                            }
                        }
                    }*/
                    List<Node<State, Event>> col = new List<Node<State, Event>>();
                    if (!map.ContainsKey(columna))
                    {
                        col.Add(nodo);
                        map.Add(columna, col);
                    }
                    else
                    {
                        col = map[columna];
                        map.Remove(columna);
                        col.Add(nodo);
                        map.Add(columna, col);
                    }
                    newNodesList.Add("{\"Key\":" + js.Serialize(nodo.Key) + ","
                        + "\"Area\":" + js.Serialize(nodo.Specification.Area.ToString()) + ","
                        + "\"Frequency\":\"" + "1" + "\","
                        + "\"Name\":\"" + ((CorrectState)nodo.Specification).Action.Name + "\","
                        + "\"Element\": \"state\","
                        + "\"Column\": " + columna + ","
                        + "\"StateType\":\"" + "Correct" + "\"}");
                }
                else if (nodeInType == typeof(WorldErrorState))
                {
                    int c = 0;
                    string clave = nodo.Key;
                    int p = clave.IndexOf('_');
                    clave = clave.Substring(0, p);
                    c = busquedaPosicion(clave, found, notFound);
                    List<Node<State, Event>> col = new List<Node<State, Event>>();
                    if (!map.ContainsKey(c))
                    {
                        col.Add(nodo);
                        map.Add(c, col);
                    }
                    else
                    {
                        col = map[c];
                        map.Remove(c);
                        col.Add(nodo);
                        map.Add(c, col);
                    }
                    newNodesList.Add("{\"Key\":" + js.Serialize(nodo.Key) + ","
                        + "\"Area\":" + js.Serialize(nodo.Specification.Area.ToString()) + ","
                        + "\"Frequency\":\"" + "1" + "\","
                        + "\"Message\":\"" + ((WorldErrorState)nodo.Specification).ErrorAssociated.Message.Message + "\","
                        + "\"Element\": \"state\","
                        + "\"Column\": " + c + ","
                        + "\"StateType\":\"" + "WorldError" + "\"}");
                }
                else if (nodeInType == typeof(TimeErrorState))
                {
                    int c = 0;
                    string clave = nodo.Key;
                    int p = clave.IndexOf('_');
                    int u = clave.LastIndexOf('_');
                    clave = clave.Substring(p + 1, u - p - 1);
                    c = busquedaPosicion(clave, found, notFound);
                    List<Node<State, Event>> col = new List<Node<State, Event>>();
                    if (!map.ContainsKey(c))
                    {
                        col.Add(nodo);
                        map.Add(c, col);
                    }
                    else
                    {
                        col = map[c];
                        map.Remove(c);
                        col.Add(nodo);
                        map.Add(c, col);
                    }
                    var tempState = (TimeErrorState)nodo.Specification;
                    newNodesList.Add("{\"Key\":" + js.Serialize(nodo.Key) + ","
                        + "\"Area\":" + js.Serialize(nodo.Specification.Area.ToString()) + ","
                        + "\"Frequency\":\"" + "1" + "\","
                        + "\"Message\":\"" + tempState.ErrorAssociated.Message.Message + "\","
                        + "\"Time\":\"" + tempState.Time + "\","
                        + "\"Element\": \"state\","
                        + "\"Column\": " + c + ","
                        + "\"StateType\":\"" + "TimeError" + "\"}");
                }
                else if (nodeInType == typeof(IncompatibilityErrorState))
                {
                    int c = 0;
                    var tempState = (IncompatibilityErrorState)nodo.Specification;
                    string clave = nodo.Key;
                    int p = clave.IndexOf('_');
                    clave = clave.Substring(0, p);
                    c = busquedaPosicion(clave, found, notFound);
                    List<Node<State, Event>> col = new List<Node<State, Event>>();
                    if (!map.ContainsKey(c))
                    {
                        col.Add(nodo);
                        map.Add(c, col);
                    }
                    else
                    {
                        col = map[c];
                        map.Remove(c);
                        col.Add(nodo);
                        map.Add(c, col);
                    }
                    newNodesList.Add("{\"Key\":" + js.Serialize(nodo.Key) + ","
                         + "\"Area\":" + js.Serialize(nodo.Specification.Area.ToString()) + ","
                         + "\"Frequency\":\"" + "1" + "\","
                         + "\"Message\":\"" + tempState.IncompatibilityFailed.IncompatibilityError.Message.Message + "\","
                         + "\"Action\":\"" + tempState.IncompatibilityFailed.IncompatibilityAction.Name + "\","
                         + "\"Element\": \"state\","
                         + "\"Column\": " + c + ","
                         + "\"StateType\":\"" + "IncompatibilityError" + "\"}");
                }
                else if (nodeInType == typeof(OtherErrorState))
                {
                    int c = 0;
                    string clave = nodo.Key;
                    int p = clave.IndexOf('_');
                    clave = clave.Substring(0, p);
                    c = busquedaPosicion(clave, found, notFound);
                    if (c != 0)
                        lastC = c;
                    else
                        c = lastC;
                    List<Node<State, Event>> col = new List<Node<State, Event>>();
                    if (!map.ContainsKey(c))
                    {
                        col.Add(nodo);
                        map.Add(c, col);
                    }
                    else
                    {
                        col = map[c];
                        map.Remove(c);
                        col.Add(nodo);
                        map.Add(c, col);
                    }
                    newNodesList.Add("{\"Key\":" + js.Serialize(nodo.Key) + ","
                         + "\"Area\":" + js.Serialize(nodo.Specification.Area.ToString()) + ","
                         + "\"Frequency\":\"" + "1" + "\","
                         + "\"Message\":\"" + ((OtherErrorState)nodo.Specification).ErrorAssociated.Message.Message + "\","
                         + "\"Element\": \"state\","
                         + "\"Columns\": " + c + ","
                         + "\"StateType\":\"" + "OtherError" + "\"}");
                }
                else if (nodeInType == typeof(DependenceErrorState))
                {
                    var tempState = (DependenceErrorState)nodo.Specification;
                    nodeInType = tempState.DependenceFailed.GetType();
                    if (nodeInType == typeof(SeqComplexDependence))
                    {
                        int c = 0;
                        var tempDependence = (SeqComplexDependence)tempState.DependenceFailed;
                        string clave = nodo.Key;
                        int hay = clave.IndexOf('_');
                        clave = clave.Substring(0, hay);
                        c = busquedaPosicion(clave, found, notFound);
                        List<Node<State, Event>> col = new List<Node<State, Event>>();
                        if (!map.ContainsKey(c))
                        {
                            col.Add(nodo);
                            map.Add(c, col);
                        }
                        else
                        {
                            col = map[c];
                            map.Remove(c);
                            col.Add(nodo);
                            map.Add(c, col);
                        }
                        newNodesList.Add("{\"Key\":" + js.Serialize(nodo.Key) + ","
                            + "\"Area\":" + js.Serialize(nodo.Specification.Area.ToString()) + ","
                            + "\"Frequency\":\"" + "1" + "\","
                            + "\"Message\":\"" + tempDependence.DependenceError.Message.Message + "\","
                            + "\"ComplexDependenceKey\":\"" + tempDependence.Key + "\","
                            + "\"Element\": \"state\","
                            + "\"Column\": " + c + ","
                            + "\"StateType\":\"" + "SeqComplexDependenceError" + "\"}");
                    }
                    else if (nodeInType == typeof(SimpleDependence))
                    {
                        int c = 0;
                        var tempDependence = (SimpleDependence)tempState.DependenceFailed;
                        string clave = nodo.Key;
                        int hay = clave.IndexOf('_');
                        clave = clave.Substring(0, hay);
                        c = busquedaPosicion(clave, found, notFound);
                        List<Node<State, Event>> col = new List<Node<State, Event>>();
                        if (!map.ContainsKey(c))
                        {
                            col.Add(nodo);
                            map.Add(c, col);
                        }
                        else
                        {
                            col = map[c];
                            map.Remove(c);
                            col.Add(nodo);
                            map.Add(c, col);
                        }
                        newNodesList.Add("{\"Key\":" + js.Serialize(nodo.Key) + ","
                            + "\"Area\":" + js.Serialize(nodo.Specification.Area.ToString()) + ","
                            + "\"Frequency\":\"" + "1" + "\","
                            + "\"Message\":\"" + tempDependence.DependenceError.Message.Message + "\","
                            + "\"Action\":\"" + tempDependence.ActionDependence.Name + "\","
                            + "\"Element\": \"state\","
                            + "\"Column\": " + c + ","
                            + "\"StateType\":\"" + "SimpleDependenceError" + "\"}");
                    }
                }
            }
            List<Arc<State, Event>> arcos = cluster.GetStudentEvents(student);
            /*newNodesList.Add("{\"NodeKey1\":" + js.Serialize(iniNode.Key) + ","
                        + "\"NodeKey2\":" + js.Serialize(nodes[0].Key) + ","
                        + "\"Element\": \"NormalEvent\","
                        + "\"Frequency\":\"" + "1" + "\"}");*/
            for (int j=0; j<arcos.Count; j++)
            {
                Arc<State,Event> arco = arcos[j];
                newNodesList.Add("{\"NodeKey1\":" + js.Serialize(arco.Key.NodeKey1) + ","
                        + "\"NodeKey2\":" + js.Serialize(arco.Key.NodeKey2) + ","
                        + "\"Element\": \"NormalEvent\","
                        + "\"Frequency\":\"" + "1" + "\"}");
            }
            return Json(newNodesList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetClusters(int modo, string strDomainName)
        {
            model = MapService.getModelByDomain(modo,strDomainName);
            dominio = model.Domain.Actions;
            int NumOfClusters = model.NumberOfClusters;
            //object re = new object();
            var re = "{\"NumOfClusters\":" + NumOfClusters +"}";
            return Json(re, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDomains()
        {
            int totalDomains = int.Parse(ConfigurationManager.AppSettings["totalDomains"]);
            string domains = "";
            for (int i = 0; i < totalDomains; i++)
            {
                string domainNum = "domain" + (i + 1).ToString();
                if(i==0)
                    domains += "\"" + ConfigurationManager.AppSettings[domainNum] + "\"";
                else
                    domains += ",\"" + ConfigurationManager.AppSettings[domainNum]+ "\"";
            }

            var re ="{\"Domains\":[" + domains + "]}";
            return Json(re, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetStudents(string strDomainName, int Cluster, int modo)
        {
            //PredictiveStudentModel model = MapService.getModelByDomain(modo,strDomainName);
            StudentsCluster cluster = model.GetCluster(Cluster-1);
            List<string> studentKeys = cluster.GetStudentKeys();
            studentKeys.Sort();
            List<string> quotedStudentKeys = new List<string>();
            foreach(string key in studentKeys)
            {
                quotedStudentKeys.Add("\"" + key + "\"");
            }

            var re = "[" + String.Join(", ", quotedStudentKeys.ToArray()) + "]";
            //cluster.GetStudentNodes("id");
            //cluster.GetStudentEvents("id");
            // int NumOfStudents = cluster.NumberOfStudents;
            // var re = "{\"NumOfStudents\":" + NumOfStudents + "}";
            return Json(re, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetClustersDate(string date,string strDomainName)
        {
            int pos1 = 5;
            int pos2 = 8;
            int year = Int32.Parse(date.Substring(0, 4));
            int month = Int32.Parse(date.Substring(pos1, 2));
            int day = Int32.Parse(date.Substring(pos2));
            DateTime dateFormat = new DateTime(year,month,day);
            Tuple<PredictiveStudentModel, int> busqueda = null;
            if (strDomainName.Contains("Populus"))
                busqueda = MapService.getModelByDate(dateFormat, 1);
            else
                busqueda = MapService.getModelByDate(dateFormat, 0);
            if (busqueda.Item2 == 0)
                model = busqueda.Item1;
            else
                return Json("{\"ErrorDate\":" + 1 + "}", JsonRequestBehavior.AllowGet);
            int NumOfClusters = model.NumberOfClusters;
            var re = "{\"NumOfClusters\":" + NumOfClusters + ","
                        + "\"ErrorDate\":\"" + 0 + "\"}";
            return Json(re, JsonRequestBehavior.AllowGet);
        }

        public class JSONPResult : JsonResult
        {
            public JSONPResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            }
            public string Callback { get; set; }
            ///<summary>
            ///对操作结果进行处理
            ///</summary>
            ///<paramname="context"></param>
            public override void ExecuteResult(ControllerContext context)
            {
                var httpContext = context.HttpContext;
                var callBack = Callback;
                if (string.IsNullOrWhiteSpace(callBack))
                    callBack = httpContext.Request["callback"]; //获得客户端提交的回调函数名称
                // 返回客户端定义的回调函数
                httpContext.Response.Write(callBack + "[");
                httpContext.Response.Write(Data);          //Data 是服务器返回的数据 
                httpContext.Response.Write("]");            //将函数输出给客户端，由客户端执行
            }
        }
    }
}
