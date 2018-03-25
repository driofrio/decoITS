using System;
using Its.ExpertModule.ObjectModel;

namespace Its.StudentModule.ObjectModel
{
	/// <author>
	/// Diego Dotor Jara.
	/// </author>
	/// <summary>
	/// This class will contain a correct executed no corective action log.
	/// </summary>
	public class NoCorrectiveActionLog : ActionLog
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Its.StudentModule.ObjectModel.NoCorrectiveActionLog"/> class.
		/// </summary>
		/// <param name="action">Action.</param>
		/// <param name="wasApplied">If set to <c>true</c> was applied.</param>
		public NoCorrectiveActionLog (ActionAplication action, bool wasApplied) : 
		base (action, wasApplied)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.StudentModule.ObjectModel.NoCorrectiveActionLog"/> class.
		/// </summary>
		/// <param name="action">Action.</param>
		/// <param name="wasApplied">If set to <c>true</c> was applied.</param>
		/// <param name="date">Date.</param>
		public NoCorrectiveActionLog (ActionAplication action, bool wasApplied,
			DateTime date) : base (action, wasApplied, date)
		{
		}
	}
}

