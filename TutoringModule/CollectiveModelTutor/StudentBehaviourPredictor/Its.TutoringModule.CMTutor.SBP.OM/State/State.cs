namespace Its.TutoringModule.CMTutor.SBP.OM.State
{
	/// <summary>
	/// State.
	/// </summary>
	public class State
	{
		private long _eventFrequency = 0;
		private long _studentFrequency = 0;
		private Area _area;

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.TutoringModule.PredictionModel.ObjectModel.State"/> class.
		/// </summary>
		/// <param name="frequency">Frequency.</param>
		/// <param name="area">Area.</param>
		/*protected State(long frequency, Area area){
			this._frequency = frequency;
			this._area = area;
		}
			*/
		/// <summary>
		/// Initializes a new instance of the <see cref="Its.TutoringModule.PredictionModel.ObjectModel.State"/> class.
		/// </summary>
		/// <param name="area">Area.</param>
		protected State(Area area){
			this._eventFrequency = 0;
			this._studentFrequency = 0;
			this._area = area;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.TutoringModule.StudentBehaviorPredictor.ObjectModel.State"/> class.
		/// </summary>
		/// <param name="area">Area.</param>
		/// <param name="frequency">Frequency.</param>
		protected State(Area area, int eventFrequency, int studentFrequency){
			this._eventFrequency = eventFrequency;
			this._studentFrequency = studentFrequency;
			this._area = area;
		}

		/// <summary>
		/// Gets the total frequency (number of times that this state has occurred).
		/// </summary>
		/// <value>The total frequency.</value>
		public long EventFrequency{
			get{
				return _eventFrequency;
			}
		}

		public long StudentFrequency{
			get{
				return _studentFrequency;
			}
		}

		/// <summary>
		/// Gets the area to which this state belongs.
		/// </summary>
		/// <value>The area.</value>
		public Area Area{
			get{
				return _area;
			}
		}

		/// <summary>
		/// Increments the frequency by one.
		/// </summary>
		public void IncrementEvtFrequency(){
			_eventFrequency++;
		}

		/// <summary>
		/// Decrements the frequency.
		/// </summary>
		/// <returns><c>true</c>, if frequency goes to zero, <c>false</c> otherwise.</returns>
		public bool DecrementEvtFrequency(){
			_eventFrequency--;
			return _eventFrequency <= 0;
		}

		/// <summary>
		/// Increments the frequency by one.
		/// </summary>
		public void IncrementStuFrequency(){
			_studentFrequency++;
		}

		/// <summary>
		/// Decrements the frequency.
		/// </summary>
		/// <returns><c>true</c>, if frequency goes to zero, <c>false</c> otherwise.</returns>
		public bool DecrementStuFrequency(){
			_studentFrequency--;
			return _studentFrequency <= 0;
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="Its.PredictionModel.ObjectModel.State"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="Its.PredictionModel.ObjectModel.State"/>.</returns>
		public override string ToString(){
			return " Frecuency: "+_eventFrequency+" Area: "+_area;
		}
	}
}