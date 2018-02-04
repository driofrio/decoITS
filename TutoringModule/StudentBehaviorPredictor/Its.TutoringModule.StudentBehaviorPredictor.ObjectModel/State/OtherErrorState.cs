﻿using System;
using Its.TutoringModule.TutoringCoordinator.ReactiveTutor.ObjectModel;

namespace Its.TutoringModule.StudentBehaviorPredictor.ObjectModel
{
	public class OtherErrorState : State
	{
		/// <summary>
		/// The error associated.
		/// </summary>
		private Error _errorAssociated;

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
		/// <see cref="Its.TutoringModule.StudentBehaviorPredictor.ObjectModel.WorldErrorState"/> class.
		/// </summary>
		/// <param name="area">Area.</param>
		/// <param name="errorAssociated">Error associated.</param>
		public OtherErrorState (Area area, Error errorAssociated) : base(area)
		{
			this._errorAssociated = errorAssociated;
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="Its.TutoringModule.StudentBehaviorPredictor.ObjectModel.WorldErrorState"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="Its.TutoringModule.StudentBehaviorPredictor.ObjectModel.WorldErrorState"/>.</returns>
		public override string ToString(){
			return "OtherErrorState: ErrorAssociated: " + _errorAssociated.Key + base.ToString ();
		}
	}
}
