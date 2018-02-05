using System;
using System.Collections.Generic;
using Its.ExpertModule.ObjectModel;
using Its.TutoringModule.StudentBehaviorPredictor.Exceptions;

namespace Its.TutoringModule.StudentBehaviorPredictor.ObjectModel
{
	public class VectorEvent : Event
	{
		private List<long> _frequency;

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.TutoringModule.PredictionModel.ObjectModel.VectorEvent"/> class.
		/// </summary>
		/// <param name="_actionExecuted">Action executed.</param>
		public VectorEvent(ActionAplication _actionExecuted):base(_actionExecuted){
			_frequency = new List<long> ();
		}

		/// <summary>
		/// Gets the frequency list (number of students that perform the event depending the number of iterations).
		/// </summary>
		/// <value>The frequency.</value>
		public List<long> Frequency{
			get{
				return _frequency;
			}
		}

		/// <summary>
		/// Increments the frequency by one.
		/// </summary>
		public void IncrementFrequency(int iteractionNumber){
			if (iteractionNumber < _frequency.Count)
				_frequency [iteractionNumber] = _frequency [iteractionNumber] + 1;
			else {
				for (int i = 0; i < iteractionNumber + 1; i++) {
					if (i == iteractionNumber)
						_frequency.Add (1);
					else if (!_frequency.Contains (i))
						_frequency.Add (0);
				}
			}
		}

		/// <summary>
		/// Decrements the frequency.
		/// </summary>
		/// <returns><c>true</c>, if frequency goes to zero, <c>false</c> otherwise.</returns>
		/// <param name="iteractionNumber">Iteraction number.</param>
		public bool DecrementFrequency(int iteractionNumber){
			if (_frequency.Count <= iteractionNumber)
				_frequency [iteractionNumber] = _frequency [iteractionNumber] - 1;
			else
				new EventIterarionOutOfBound (iteractionNumber);
			return _frequency [iteractionNumber] <= 0;
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="Its.TutoringModule.PredictionModel.ObjectModel.VectorEvent"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="Its.TutoringModule.PredictionModel.ObjectModel.VectorEvent"/>.</returns>
		public override string ToString(){
			String[] frequencyStrings = new List<long>(_frequency).ConvertAll(i => i.ToString()).ToArray(); 
			return "VectorEvent - ActionExecuted: " + base.ActionExecuted.Name + " Frecuency: [" + String.Join(", ", frequencyStrings)+"]";
		}
	}
}

