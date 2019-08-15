using System;
using Its.ExpertModule.ObjectModel;

namespace Its.StudentModule.ObjectModel
{
	/// <author>
	/// Diego Dotor Jara.
	/// </author>
	/// <summary>
	/// This class will contain a incompatibility error log.
	/// </summary>
	public class IncompErrorLog : ErrorLog
	{
		/// <summary>
		/// Incompatibility that the error has associated.
		/// </summary>
		private Incompatibility _failedIncompatibility;
		/// <summary>
		/// Gets the incomp.
		/// </summary>
		/// <value>The incomp.</value>
		public Incompatibility FailedIncompatibility {
			get {
				return _failedIncompatibility;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.StudentModule.ObjectModel.IncompErrorLog"/> class.
		/// </summary>
		/// <param name="action">Action.</param>
		/// <param name="wasApplied">If set to <c>true</c> was applied.</param>
		/// <param name="incompatibility">Incompatibility.</param>
		public IncompErrorLog (ActionAplication action, bool wasApplied, 
			Incompatibility incompatibility) : base (action, wasApplied)
		{
			this._failedIncompatibility = incompatibility;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.StudentModule.ObjectModel.IncompErrorLog"/> class.
		/// </summary>
		/// <param name="action">Action.</param>
		/// <param name="wasApplied">If set to <c>true</c> was applied.</param>
		/// <param name="date">Date.</param>
		/// <param name="incompatibility">Incompatibility.</param>
		public IncompErrorLog (ActionAplication action, bool wasApplied, 
			DateTime date, Incompatibility incompatibility) : base (action, wasApplied, date)
		{
			this._failedIncompatibility = incompatibility;
		}

		public override string TxtLogString()
		{
			return Action.Key + " " + Action.Name + " IncompatibilityError " + FailedIncompatibility.IncompatibilityError.Message.Message + " " + DateLog.ToString();
		}
	}
}

