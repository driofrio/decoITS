using System;
using Its.ExpertModule.ObjectModel;

namespace Its.StudentModule.ObjectModel
{

	/// <author>
	/// Diego Dotor Jara.
	/// </author>
	/// <summary>
	/// This class will contain a correct executed action log.
	/// </summary>
	public class ActionLog : LogEntry
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Its.StudentModule.ObjectModel.ActionLog"/> class.
		/// </summary>
		/// <param name="action">Action.</param>
		/// <param name="wasApplied">If set to <c>true</c> was applied.</param>
		protected ActionLog (ActionAplication action, bool wasApplied) : 
		base ( action, wasApplied)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.StudentModule.ObjectModel.ActionLog"/> class.
		/// </summary>
		/// <param name="action">Action.</param>
		/// <param name="wasApplied">If set to <c>true</c> was applied.</param>
		/// <param name="date">Date.</param>
		protected ActionLog ( ActionAplication action, bool wasApplied,
			DateTime date) : base (action, wasApplied, date)
		{
		}
	}
}

