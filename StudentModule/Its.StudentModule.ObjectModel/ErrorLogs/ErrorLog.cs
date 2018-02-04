﻿using System;
using Its.ExpertModule.ObjectModel;
using Its.TutoringModule.TutoringCoordinator.ReactiveTutor.ObjectModel;

namespace Its.StudentModule.ObjectModel
{

	/// <author>
	/// Diego Dotor Jara.
	/// </author>
	/// <summary>
	/// This class will contain an error log.
	/// </summary>
	public class ErrorLog : LogEntry
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Its.StudentModule.ObjectModel.ErrorLog"/> class.
		/// </summary>
		/// <param name="action">Action.</param>
		/// <param name="wasApplied">If set to <c>true</c> was applied.</param>
		protected ErrorLog (ActionAplication action, bool wasApplied) :
		base ( action, wasApplied) {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.StudentModule.ObjectModel.ErrorLog"/> class.
		/// </summary>
		/// <param name="action">Action.</param>
		/// <param name="wasApplied">If set to <c>true</c> was applied.</param>
		/// <param name="date">Date.</param>
		protected ErrorLog (ActionAplication action, bool wasApplied,
			DateTime date) : base (action, wasApplied, date) {
		}
	}
}
