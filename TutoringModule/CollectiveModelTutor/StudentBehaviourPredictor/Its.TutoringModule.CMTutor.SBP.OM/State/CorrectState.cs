using Its.ExpertModule.ObjectModel;

namespace Its.TutoringModule.CMTutor.SBP.OM.State
{
	public class CorrectState : State
	{
		/// <summary>
		/// The action.
		/// </summary>
		private ActionAplication _action;
		private bool _isCorrective;

		/// <summary>
		/// Gets the action.
		/// </summary>
		/// <value>The action.</value>
		public ActionAplication Action{
			get{ 
				return _action;
			}
		}

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="CorrectState"/> class.
		/// </summary>
		/// <param name="action">Action.</param>
		/// <param name="area">Area.</param>
		public CorrectState (Area area, ActionAplication action, bool isCorrective) : base(area)
		{
			this._action = action;
			this._isCorrective = isCorrective;
		}

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="Its.TutoringModule.StudentBehaviorPredictor.ObjectModel.CorrectState"/> class.
		/// </summary>
		/// <param name="area">Area.</param>
		/// <param name="action">Action.</param>
		/// <param name="isCorrective">If set to <c>true</c> is corrective.</param>
		/// <param name="frequency">Frequency.</param>
		public CorrectState (Area area, ActionAplication action, bool isCorrective, int evtFreq, int stuFreq) : base(area, evtFreq, stuFreq)
		{
			this._action = action;
			this._isCorrective = isCorrective;
		}

		/// <summary>
		/// Gets a value indicating whether this state is corrective.
		/// </summary>
		/// <value><c>true</c> if this instance is corrective; otherwise, <c>false</c>.</value>
		public bool IsCorrective{
			get{
				return _isCorrective;
			}
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="CorrectState"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="CorrectState"/>.</returns>
		public override string ToString(){
			if (_action == null)
				return "Init " + base.ToString ();
			else
				return "CorrectState: Action: " + _action.Key + base.ToString ();
		}
	}
}

