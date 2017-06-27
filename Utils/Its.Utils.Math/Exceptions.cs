using System;

namespace Its.Utils.Math.Exceptions
{
	public class NodeNotFoundException : Exception{
		public NodeNotFoundException():base("Node not found in model"){
		}
	}

	public class ArcNotFoundException : Exception{
		public ArcNotFoundException():base("Arc not found in model"){
		}
	}

	public class DuplicatedArcException : Exception{
		public DuplicatedArcException(string arcKey):base("Arc with key "+arcKey+" already exists"){
		}
	}
}

