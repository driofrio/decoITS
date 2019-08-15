using System;
using Its.ExpertModule.ObjectModel;
using Its.TutoringModule.ReactiveTutor.ObjectModel;

namespace Its.StudentModule.ObjectModel
{
	public class OtherErrorLog : ErrorLog
	{
		/// <summary>
		/// Error associated with other error.
		/// </summary>
		private Error _errorAssociated;
		/// <summary>
		/// Gets the error associated.
		/// </summary>
		/// <value>The error associated.</value>
		public Error ErrorAssociated {
			get {
				return _errorAssociated;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.StudentModule.ObjectModel.OtherErrorLog"/> class.
		/// </summary>
		/// <param name="action">Action.</param>
		/// <param name="wasApplied">If set to <c>true</c> was applied.</param>
		/// <param name="errorAssociated">Error associated.</param>
		public OtherErrorLog (ActionAplication action, bool wasApplied,
			Error errorAssociated)  : base (action, wasApplied)
		{
			this._errorAssociated = errorAssociated;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.StudentModule.ObjectModel.OtherErrorLog"/> class.
		/// </summary>
		/// <param name="action">Action.</param>
		/// <param name="wasApplied">If set to <c>true</c> was applied.</param>
		/// <param name="date">Date.</param>
		/// <param name="errorAssociated">Error associated.</param>
		public OtherErrorLog (ActionAplication action, bool wasApplied,
			DateTime date, Error errorAssociated)  : base (action, wasApplied, date)
		{
			this._errorAssociated = errorAssociated;
		}

		public override string TxtLogString()
		{
			return Action.Key + " " + Action.Name + " OtherError " + Error.Message.Message + " " + DateLog.ToString();
		}
	}
}

