using System.Collections.Generic;
using System.Linq;
using Its.TutoringModule.CMTutor.SBP.OM;
using Its.TutoringModule.CMTutor.SBP.OM.Event;
using Its.TutoringModule.CMTutor.SBP.OM.State;
using Its.Utils.Math;

namespace Its.TutoringModule.CMTutor.EPM.PathFind
{
    public class PathFinder
    {
        private StudentActionsModel model;

        public PathFinder(StudentActionsModel model)
        {
            this.model = model;
        }
            
        /// <summary>
        /// Find path in the automaton between a single start node and one or many destination nodes.
        /// BFS is used to explore the graph.
        ///
        /// If graph conntains cycles, infinite loops are prevented by keeping track inside of PathInfo object which
        /// transitions have already been followed
        /// </summary>
        /// <param name="fromNode"></param>
        /// <param name="targetNodes"></param>
        /// <param name="pathConfThreshold"></param>
        /// <returns>
        /// List of PathInfo objects that contain info about destination nodes that paths could be found to and
        /// maximum possible path confidence.
        /// </returns>
        public List<PathInfo> Find(string fromNodeKey, HashSet<string> targetNodeKeys)
        {
            // Init the finder                        
            PathInfo p = new PathInfo(fromNodeKey, fromNodeKey, 0, 1);
            Queue<PathInfo> toExplore = new Queue<PathInfo>();
            toExplore.Enqueue(p);

            List<PathInfo> pathsToTargets = ExplorePaths(toExplore, targetNodeKeys);

            return pathsToTargets;
        }

        /// <summary>
        /// Returns list of all possible paths that lead from start node to any of the target nodes
        /// </summary>
        /// <param name="toExplore"></param>
        /// <param name="targetNodeKeys"></param>
        /// <param name="pathConfThreshold"></param>
        /// <returns></returns>
        private List<PathInfo> ExplorePaths(Queue<PathInfo> toExplore, HashSet<string> targetNodeKeys)
        {
            double allPathConfidenceSum = 1;
            List<PathInfo> pathsToTargets = new List<PathInfo>();
                
            // Loop ends when either
            // 1. All paths have been explored from the start node until the end of the acyclic graph represented by StudentActionsModel
            // 2. Current sum of all path confidences is below the threshold (sum may include paths not leading to target nodes, but can be used as an upper limit for estimation)
            while (toExplore.Count > 0)
            {
                // Deque
                PathInfo pathInfo = toExplore.Dequeue();
                Node<State, Event> node = model.GetState(pathInfo.ToNodeKey);

                // First, substract old path confidence from the total
                allPathConfidenceSum -= pathInfo.PathConfidence;

                // Create + enqueue PathInfo for all transitions
                foreach (Arc<State, Event> transition in node.OutArcs.Values)
                {
                    if (pathInfo.Visited(transition.Key.ToString()))
                    {
                        continue;
                    }
                    else
                    {
                        string nextNodeKey = model.GetInStateKey(transition);
                        Node<State, Event> nextNode = model.GetState(nextNodeKey);
                        int length = pathInfo.PathLength + 1;
                        double newPathConfidence = pathInfo.PathConfidence * model.GetEventConfidence(transition);
                    
                        // Replace old path condifence with the sum of confidences of all child paths
                        allPathConfidenceSum += newPathConfidence;

                        // Create new pathInfo object to be queued
                        HashSet<string> trace = new HashSet<string>(pathInfo.Trace);
                        trace.Add(transition.Key.ToString());
                        PathInfo updatedPath = new PathInfo(pathInfo.FromNodeKey, nextNodeKey, length, newPathConfidence, trace);
                        toExplore.Enqueue(updatedPath);
                    
                        // Check if next node is one of the targets
                        if (targetNodeKeys.Contains(nextNodeKey))
                        {
                            pathsToTargets.Add(updatedPath);
                        }   
                    }
                }
            }

            return pathsToTargets;
        }

        public Dictionary<string, double> AddPathConfidenceByTarget(List<PathInfo> paths)
        {
            Dictionary<string, double> targetNodePathConfidenceSum = new Dictionary<string, double>();

            foreach (PathInfo path in paths)
            {
                if (!targetNodePathConfidenceSum.ContainsKey(path.ToNodeKey))
                {
                    targetNodePathConfidenceSum.Add(path.ToNodeKey, 0);
                }
                
                targetNodePathConfidenceSum[path.ToNodeKey] = targetNodePathConfidenceSum[path.ToNodeKey] + path.PathConfidence;
            }

            return targetNodePathConfidenceSum;
        }

        public Dictionary<string, double> FindPathsAboveThreshold(string fromNodeKey, HashSet<string> targetNodeKeys,
            double pathConfThreshold)
        {
            Dictionary<string, double> results = new Dictionary<string, double>();
//            List<PathInfo> allPathsToTargets = Find(fromNodeKey, targetNodeKeys, pathConfThreshold);
            List<PathInfo> allPathsToTargets = Find(fromNodeKey, targetNodeKeys);
            Dictionary<string, double> totalPathConfidencesByTarget = AddPathConfidenceByTarget(allPathsToTargets);

            foreach (string key in totalPathConfidencesByTarget.Keys)
            {
                if (totalPathConfidencesByTarget[key] >= pathConfThreshold)
                {
                    results.Add(key, totalPathConfidencesByTarget[key]);
                }
            }

            return results;
        }
    }
}