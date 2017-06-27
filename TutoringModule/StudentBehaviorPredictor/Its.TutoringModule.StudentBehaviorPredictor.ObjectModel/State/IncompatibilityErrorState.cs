using System;
using Its.ExpertModule.ObjectModel;


namespace Its.TutoringModule.StudentBehaviorPredictor.ObjectModel
{
	public class IncompatibilityErrorState : State
	{
		/// <summary>
		/// The incompatibility failed.
		/// </summary>
		private Incompatibility _incompatibilityFailed;

		/// <summary>
		/// Gets the incompatibility failed.
		/// </summary>
		/// <value>The incompatibility failed.</value>
		public Incompatibility IncompatibilityFailed{
			get{
				return _incompatibilityFailed;
			}
		}

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="Its.TutoringModule.StudentBehaviorPredictor.ObjectModel.IncompatibilityErrorState"/> class.
		/// </summary>
		/// <param name="area">Area.</param>
		/// <param name="incompatibilityFailed">Incompatibility failed.</param>
		public IncompatibilityErrorState (Area area, Incompatibility incompatibilityFailed) : base(area)
		{
			this._incompatibilityFailed = incompatibilityFailed;
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="Its.TutoringModule.StudentBehaviorPredictor.ObjectModel.IncompatibilityErrorState"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="Its.TutoringModule.StudentBehaviorPredictor.ObjectModel.IncompatibilityErrorState"/>.</returns>
		public override string ToString(){
			return "IncompatibilityErrorState: IncompatibilityFailed: " + _incompatibilityFailed.Key + base.ToString ();
		}
	}
}

