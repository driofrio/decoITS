using Its.ExpertModule.ObjectModel;

namespace Its.TutoringModule.CMTutor.SBP.OM.Event
{
	/// <summary>
	/// Event.
	/// </summary>
	public class Event
	{
		/// <summary>
		/// The action executed.
		/// </summary>
		private ActionAplication _actionExecuted;

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.TutoringModule.PredictionModel.ObjectModel.Event"/> class.
		/// </summary>
		/// <param name="actionExecuted">Action executed.</param>
		protected Event(ActionAplication actionExecuted){
			this._actionExecuted = actionExecuted;
		}

		/// <summary>
		/// Gets the action executed.
		/// </summary>
		/// <value>The action executed.</value>
		public ActionAplication ActionExecuted{
			get{
				return _actionExecuted;
			}
		}
	}
}

