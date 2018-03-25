using System;

namespace Its.TutoringModule.CMTutor.SBP.Exceptions
{
	public class PredictiveStudentModelNotFoundException : Exception{
		public PredictiveStudentModelNotFoundException():base("Predictive Student Model not found"){
		}

		public PredictiveStudentModelNotFoundException(string key):base("Predictive Student Model "+key+" not found"){
		}
	}

	public class StateNotFoundException : Exception{
		public StateNotFoundException():base("State not found in model"){
		}

		public StateNotFoundException(string key):base("State "+key+" not found in model"){
		}
	}

	public class EventNotFoundException : Exception{
		public EventNotFoundException():base("Event not found in model"){
		}

		public EventNotFoundException(string stateKeyOut, string stateKeyIn):base("Event " + stateKeyOut +" - " + stateKeyIn + " not found in model"){
		}
	}

	public class ClusterNotFoundException : Exception{
		public ClusterNotFoundException():base("Cluster not found in model"){
		}

		public ClusterNotFoundException(int number):base("Cluster " + number + " not found in model"){
		}

		public ClusterNotFoundException(string keyStudent):base("Cluster not found in model for student: "+keyStudent){
		}
	}

	public class StudentModelNotFoundInClusterException : Exception{
		public StudentModelNotFoundInClusterException():base("Student model not found in cluster"){
		}

		public StudentModelNotFoundInClusterException(string key):base("Student model "+key+" not found in cluster"){
		}
	}

	public class EventIterarionOutOfBound : Exception{
		public EventIterarionOutOfBound():base("Iteration number was out of range"){
		}

		public EventIterarionOutOfBound(int iteration):base("Iteration number "+iteration+" was out of range"){
		}
	}
}