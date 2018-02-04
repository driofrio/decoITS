﻿using System;
using Its.ExpertModule.ObjectModel;
using Its.TutoringModule.TutoringCoordinator.ReactiveTutor.ObjectModel;

namespace Its.StudentModule.ObjectModel
{

	/// <author>
	/// Diego Dotor Jara.
	/// </author>
	/// <summary>
	/// This class will contain a time error log.
	/// </summary>
	public class TimeErrorLog : ErrorLog
	{

		/// <summary>
		/// Time that the action need to be executed.
		/// </summary>
		private int _time;
		/// <summary>
		/// Gets the time.
		/// </summary>
		/// <value>The time.</value>
		public int Time {
			get {
				return _time;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.StudentModule.ObjectModel.TimeErrorLog"/> class.
		/// </summary>
		/// <param name="action">Action.</param>
		/// <param name="wasApplied">If set to <c>true</c> was applied.</param>
		/// <param name="time">Time.</param>
		protected TimeErrorLog (ActionAplication action, bool wasApplied,
			int time) : base (action, wasApplied)
		{
			this._time = time;
		}
			
		/// <summary>
		/// Initializes a new instance of the <see cref="Its.StudentModule.ObjectModel.TimeErrorLog"/> class.
		/// </summary>
		/// <param name="action">Action.</param>
		/// <param name="wasApplied">If set to <c>true</c> was applied.</param>
		/// <param name="date">Date.</param>
		/// <param name="time">Time.</param>
		protected TimeErrorLog (ActionAplication action, bool wasApplied,
			DateTime date, int time) : base (action, wasApplied, date)
		{
			this._time = time;
		}
	}
}
