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
        /// If more than one path exists, priority is given to the paths with higher path confidence,
        /// i.e. paths that students are more likely to take.
        ///
        /// BFS is used to explore the graph and pathConfThreshold parameter is used to stop algorithm early if sum of
        /// all path confidences in the graph is below threshold (path confidence will keep shrinking as it is multiplied with values (0; 1] at every step).
        /// </summary>
        /// <param name="fromNode"></param>
        /// <param name="targetNodes"></param>
        /// <param name="pathConfThreshold"></param>
        /// <returns>
        /// List of PathInfo objects that contain info about destination nodes that paths could be found to and
        /// maximum possible path confidence.
        /// </returns>
        public List<PathInfo> Find(string fromNodeKey, HashSet<string> targetNodeKeys, double pathConfThreshold)
        {
            // Init the finder                        
            PathInfo p = new PathInfo(fromNodeKey, fromNodeKey, 0, 1);
            Queue<PathInfo> toExplore = new Queue<PathInfo>();
            toExplore.Enqueue(p);

            List<PathInfo> pathsToTargets = ExplorePaths(toExplore, targetNodeKeys, pathConfThreshold);

            return pathsToTargets;
        }
        
        /// <summary>
        /// Generic version of the path finding implementation that does NOT use total path confidence threshold to abort the search early.
        /// </summary>
        /// <param name="fromNodeKey"></param>
        /// <param name="targetNodeKeys"></param>
        /// <returns></returns>
        public List<PathInfo> Find(string fromNodeKey, HashSet<string> targetNodeKeys)
        {
            return Find(fromNodeKey, targetNodeKeys, 0);
        }

        /// <summary>
        /// Returns list of all possible paths that lead from start node to any of the target nodes
        /// </summary>
        /// <param name="toExplore"></param>
        /// <param name="targetNodeKeys"></param>
        /// <param name="pathConfThreshold"></param>
        /// <returns></returns>
        private List<PathInfo> ExplorePaths(Queue<PathInfo> toExplore, HashSet<string> targetNodeKeys, double pathConfThreshold)
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
                    string nextNodeKey = model.GetInStateKey(transition);
                    Node<State, Event> nextNode = model.GetState(nextNodeKey);
                    int length = pathInfo.PathLength + 1;
                    double newPathConfidence = pathInfo.PathConfidence * model.GetEventConfidence(transition);
                    
                    // Replace old path condifence with the sum of confidences of all child paths
                    allPathConfidenceSum += newPathConfidence;
                    
                    PathInfo updatedPath = new PathInfo(pathInfo.FromNodeKey, nextNodeKey, length, newPathConfidence);
                    toExplore.Enqueue(updatedPath);
                    
                    // check if next node is one of the targets
                    if (targetNodeKeys.Contains(nextNodeKey))
                    {
                        pathsToTargets.Add(updatedPath);
                    }
                }

                // Check total Confidence agains threshold
                if (allPathConfidenceSum < pathConfThreshold)
                {
                    // Stop exploring and return empty list
                    return new List<PathInfo>();
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
                
                targetNodePathConfidenceSum.Add(path.ToNodeKey, targetNodePathConfidenceSum[path.ToNodeKey] + path.PathConfidence);
            }

            return targetNodePathConfidenceSum;
        }

        public Dictionary<string, double> FindPathsAboveThreshold(string fromNodeKey, HashSet<string> targetNodeKeys,
            double pathConfThreshold)
        {
            Dictionary<string, double> results = new Dictionary<string, double>();
            List<PathInfo> allPathsToTargets = Find(fromNodeKey, targetNodeKeys, pathConfThreshold);
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