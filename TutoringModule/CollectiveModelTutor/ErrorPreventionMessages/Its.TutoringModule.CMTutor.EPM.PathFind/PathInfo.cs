using Its.TutoringModule.CMTutor.SBP.OM.Event;
using Its.TutoringModule.CMTutor.SBP.OM.State;
using Its.Utils.Math;

namespace Its.TutoringModule.CMTutor.EPM.PathFind
{
    public class PathInfo
    {
        private string _fromNodeKey;
        private string _toNodeKey;
        private int _pathLength;
        private double _pathConfidence;

        public string FromNodeKey
        {
            get { return _fromNodeKey; }
        }

        public string ToNodeKey
        {
            get { return _toNodeKey; }
        }

        public int PathLength
        {
            get { return _pathLength; }
        }

        public double PathConfidence
        {
            get { return _pathConfidence; }
        }
        
        public PathInfo(string fromNodeKey, string toNodeKey, int pathLength, double pathConfidence)
        {
            _fromNodeKey = fromNodeKey;
            _toNodeKey = toNodeKey;
            _pathLength = pathLength;
            _pathConfidence = pathConfidence;
        }
    }
}