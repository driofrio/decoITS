using Its.ExpertModule.ObjectModel;

namespace Its.TutoringModule.CMTutor.SBP.OM.Event
{
	public class NormalEvent : Event
	{
		private long _frequency=0;

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.TutoringModule.PredictionModel.ObjectModel.NormalEvent"/> class.
		/// </summary>
		/// <param name="_actionExecuted">Action executed.</param>
		public  NormalEvent(ActionAplication actionExecuted):base(actionExecuted){
			this._frequency=1;
		}

		/// <summary>
		/// Gets the frequency (number of students that perform the event).
		/// </summary>
		/// <value>The frequency.</value>
		public long Frequency{
			get{
				return _frequency;
			}
		}

		/// <summary>
		/// Increments the frequency by one.
		/// </summary>
		public void IncrementFrequency(){
			_frequency++;
		}

		/// <summary>
		/// Decrements the frequency.
		/// </summary>
		/// <returns><c>true</c>, if frequency goes to zero, <c>false</c> otherwise.</returns>
		public bool DecrementFrequency(){
			_frequency--;
			return _frequency <= 0;
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="Its.TutoringModule.PredictionModel.ObjectModel.NormalEvent"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="Its.TutoringModule.PredictionModel.ObjectModel.NormalEvent"/>.</returns>
		public override string ToString(){
			return "NormalEvent - ActionExecuted: " + base.ActionExecuted.Name + " Frecuency: " + _frequency.ToString ();
		}
	}
}

