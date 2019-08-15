using Its.ExpertModule.ObjectModel;

namespace Its.TutoringModule.CMTutor.SBP.OM.State
{
	public class DependenceErrorState : State
	{
		/// <summary>
		/// The dependence failed.
		/// </summary>
		private Dependence _dependenceFailed;

		/// <summary>
		/// Gets the dependence failed.
		/// </summary>
		/// <value>The dependence failed.</value>
		public Dependence DependenceFailed{
			get{
				return _dependenceFailed;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.TutoringModule.PredictionModel.ObjectModel.DepErrorState"/> class.
		/// </summary>
		/// <param name="frequency">Frequency.</param>
		/// <param name="area">Area.</param>
		/// <param name="dependenceFailed">Dependence failed.</param>
		public DependenceErrorState (Area area, Dependence dependenceFailed):base(area)
		{
			this._dependenceFailed = dependenceFailed;
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="Its.TutoringModule.PredictionModel.ObjectModel.DepErrorState"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="Its.TutoringModule.PredictionModel.ObjectModel.DepErrorState"/>.</returns>
		public override string ToString(){
			return "DependenceErrorState: DependenceFailed: " + _dependenceFailed.Key + base.ToString ();
		}
	}
}

