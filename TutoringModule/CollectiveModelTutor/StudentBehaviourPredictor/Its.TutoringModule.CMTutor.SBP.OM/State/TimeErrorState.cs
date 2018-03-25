using Its.TutoringModule.ReactiveTutor.ObjectModel;

namespace Its.TutoringModule.CMTutor.SBP.OM.State
{
	public class TimeErrorState : State
	{
		/// <summary>
		/// The time.
		/// </summary>
		private int _time;
		/// <summary>
		/// The error associated.
		/// </summary>
		private Error _errorAssociated;

		/// <summary>
		/// Gets the time.
		/// </summary>
		/// <value>The time.</value>
		public int Time{
			get{
				return _time;
			}
		}

		/// <summary>
		/// Gets the error associated.
		/// </summary>
		/// <value>The error associated.</value>
		public Error ErrorAssociated{
			get{
				return _errorAssociated;
			}
		}

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="TimeErrorState"/> class.
		/// </summary>
		/// <param name="area">Area.</param>
		/// <param name="time">Time.</param>
		/// <param name="errorAssociated">Error associated.</param>
		public TimeErrorState (Area area, int time, Error errorAssociated) : base(area)
		{
			this._time = time;
			this._errorAssociated = errorAssociated;
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="TimeErrorState"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="TimeErrorState"/>.</returns>
		public override string ToString(){
			return "TimeErrorState: ErrorAssociated: " + _errorAssociated.Key +" Time: "+_time + base.ToString ();
		}
	}
}

