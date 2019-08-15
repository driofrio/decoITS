﻿using System;
using Its.ExpertModule.ObjectModel;

namespace Its.StudentModule.ObjectModel
{

	/// <author>
	/// Diego Dotor Jara.
	/// </author>
	/// <summary>
	/// This class will contain a minimum time error log.
	/// </summary>
	public class MinTimeErrorLog : TimeErrorLog
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Its.StudentModule.ObjectModel.MinTimeErrorLog"/> class.
		/// </summary>
		/// <param name="action">Action.</param>
		/// <param name="wasApplied">If set to <c>true</c> was applied.</param>
		/// <param name="time">Time.</param>
		public MinTimeErrorLog (ActionAplication action, bool wasApplied, 
			int time) : base (action,  wasApplied, time)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.StudentModule.ObjectModel.MinTimeErrorLog"/> class.
		/// </summary>
		/// <param name="action">Action.</param>
		/// <param name="wasApplied">If set to <c>true</c> was applied.</param>
		/// <param name="date">Date.</param>
		/// <param name="time">Time.</param>
		public MinTimeErrorLog (ActionAplication action, bool wasApplied, 
			DateTime date, int time) : base (action,  wasApplied, date, time)
		{
		}

		public override string TxtLogString()
		{
			return Action.Key + " " + Action.Name + " MinTimeError " + Action.MinTimeError.Message.Message + " " + DateLog.ToString();
		}
	}
}

