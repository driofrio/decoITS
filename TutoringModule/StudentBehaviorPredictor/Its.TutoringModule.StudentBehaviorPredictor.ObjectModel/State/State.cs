using System;
using System.Collections;
using System.Collections.Generic;
using Its.Utils.Math;
using Its.TutoringModule.StudentBehaviorPredictor.Exceptions;

namespace Its.TutoringModule.StudentBehaviorPredictor.ObjectModel
{
	/// <summary>
	/// State.
	/// </summary>
	public class State
	{
		private long _frequency = 0;
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
			this._frequency = 1;
			this._area = area;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.TutoringModule.StudentBehaviorPredictor.ObjectModel.State"/> class.
		/// </summary>
		/// <param name="area">Area.</param>
		/// <param name="frequency">Frequency.</param>
		protected State(Area area, int frequency){
			this._frequency = frequency;
			this._area = area;
		}

		/// <summary>
		/// Gets the total frequency (number of times that this state has occurred).
		/// </summary>
		/// <value>The total frequency.</value>
		public long Frequency{
			get{
				return _frequency;
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
		/// Returns a <see cref="System.String"/> that represents the current <see cref="Its.PredictionModel.ObjectModel.State"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="Its.PredictionModel.ObjectModel.State"/>.</returns>
		public override string ToString(){
			return " Frecuency: "+_frequency+" Area: "+_area;
		}
	}
}