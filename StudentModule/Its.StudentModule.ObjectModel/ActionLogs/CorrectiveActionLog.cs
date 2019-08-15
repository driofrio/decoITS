using System;
using Its.ExpertModule.ObjectModel;

namespace Its.StudentModule.ObjectModel
{
	/// <author>
	/// Diego Dotor Jara.
	/// </author>
	/// <summary>
	/// This class will contain a correct executed corective action log.
	/// </summary>
	public class CorrectiveActionLog : ActionLog
	{
		/// <summary>
		/// The errors fixed.
		/// </summary>
		private bool _errorsFixed = false;
		/// <summary>
		/// Gets a value indicating whether this <see cref="Its.Student.ObjectModel.CorrectiveActionLog"/> errors fixed.
		/// </summary>
		/// <value><c>true</c> if errors fixed; otherwise, <c>false</c>.</value>
		public bool ErrorsFixed {
			get {
				return _errorsFixed;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.StudentModule.ObjectModel.CorrectiveActionLog"/> class.
		/// </summary>
		/// <param name="action">Action.</param>
		/// <param name="wasApplied">If set to <c>true</c> was applied.</param>
		/// <param name="errorsFixed">If set to <c>true</c> errors fixed.</param>
		public CorrectiveActionLog (ActionAplication action, bool wasApplied,
			bool errorsFixed) : base (action, wasApplied)
		{
			this._errorsFixed = errorsFixed;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.StudentModule.ObjectModel.CorrectiveActionLog"/> class.
		/// </summary>
		/// <param name="action">Action.</param>
		/// <param name="wasApplied">If set to <c>true</c> was applied.</param>
		/// <param name="date">Date.</param>
		/// <param name="errorsFixed">If set to <c>true</c> errors fixed.</param>
		public CorrectiveActionLog (ActionAplication action, bool wasApplied,
			DateTime date, bool errorsFixed) : base (action, wasApplied, date)
		{
			this._errorsFixed = errorsFixed;
		}
	}
}

