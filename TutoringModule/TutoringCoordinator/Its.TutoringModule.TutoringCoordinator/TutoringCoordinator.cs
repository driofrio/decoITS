using System;
using Its.TutoringModule.StudentBehaviorPredictor;
using Its.StudentModule;
using Its.ExpertModule;

namespace Its.TutoringModule.TutoringCoordinator
{
	public class TutoringCoordinator
	{
		private static StudentControl _studentControl;
		private static ExpertControl _expertControl;
		//private static ReactiveTutor.ReactiveTutor _reactiveTutor;
		private static Its.TutoringModule.StudentBehaviorPredictor.StudentBehaviorPredictorControl _studentBehaviorPredictor;


		/// <summary>
		/// The instance.
		/// </summary>
		private static TutoringCoordinator _instance = null;

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static TutoringCoordinator Instance {
			get {
				if (_instance == null)
					_instance = new TutoringCoordinator ();

				return _instance;
			}
		}

		private TutoringCoordinator ()
		{
			//_studentControl = StudentControl.Instance;
			//_expertControl = ExpertControl.Instance;
			//_reactiveTutor = ReactiveTutor.Instance;
			//_studentBehaviorPredictor = StudentBehaviorPredictor.StudentBehaviorPredictor.Instance;
		}
	}
}

