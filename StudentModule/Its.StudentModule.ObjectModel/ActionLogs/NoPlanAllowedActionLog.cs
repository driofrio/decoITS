using System;
using Its.ExpertModule.ObjectModel;

namespace Its.StudentModule.ObjectModel
{
	public class NoPlanAllowedActionLog : ActionLog
	{
		public NoPlanAllowedActionLog (ActionAplication action, bool wasApplied) : 
		base (action, wasApplied)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.StudentModule.ObjectModel.NoPlanAllowedActionLog"/> class.
		/// </summary>
		/// <param name="action">Action.</param>
		/// <param name="wasApplied">If set to <c>true</c> was applied.</param>
		/// <param name="date">Date.</param>
		public NoPlanAllowedActionLog (ActionAplication action, bool wasApplied,
			DateTime date) : base (action, wasApplied, date)
		{
		}
	}
}

