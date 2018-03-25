using System;
using Its.ExpertModule.ObjectModel;
using Its.TutoringModule.ReactiveTutor.ObjectModel;

namespace Its.StudentModule.ObjectModel
{

	/// <author>
	/// Diego Dotor Jara.
	/// </author>
	/// <summary>
	/// This class will contain all log information.
	/// </summary>
	public class LogEntry
	{
		/// <summary>
		/// Boolean that denotes if this action was applied or not.
		/// </summary>
		private bool _wasApplied;
		/// <summary>
		/// Gets a value indicating whether this <see cref="Its.Student.ObjectModel.LogEntry"/> was applied.
		/// </summary>
		/// <value><c>true</c> if was applied; otherwise, <c>false</c>.</value>
		public bool WasApplied {
			get {
				return _wasApplied;
			}
		}
		/// <summary>
		/// Action associated with this log.
		/// </summary>
		private ActionAplication _action;
		/// <summary>
		/// Gets the action.
		/// </summary>
		/// <value>The action.</value>
		public ActionAplication Action {
			get {
				return _action;
			}
		}
		/// <summary>
		/// Date in which the log has been created.
		/// </summary>
		private DateTime _dateLog;
		/// <summary>
		/// Gets the date log.
		/// </summary>
		/// <value>The date log.</value>
		public DateTime DateLog {
			get {
				return this._dateLog;
			}
			set{
				_dateLog = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.StudentModule.ObjectModel.LogEntry"/> class.
		/// </summary>
		/// <param name="action">Action.</param>
		/// <param name="wasApplied">If set to <c>true</c> was applied.</param>
		protected LogEntry (ActionAplication action, bool wasApplied)
		{
			this._action = action;
			this._dateLog = DateTime.Now;
			this._wasApplied = wasApplied;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.StudentModule.ObjectModel.LogEntry"/> class.
		/// </summary>
		/// <param name="action">Action.</param>
		/// <param name="wasApplied">If set to <c>true</c> was applied.</param>
		/// <param name="date">Date.</param>
		protected LogEntry (ActionAplication action, bool wasApplied, DateTime date)
		{
			this._action = action;
			this._wasApplied = wasApplied;
			this._dateLog = date;
		}

		/// <summary>
		/// Gets the error in this log.
		/// </summary>
		/// <returns>The error in this log.</returns>
		public Error Error
		{
			get{
				Error tempError = null;
				if (this.GetType () == typeof(OtherErrorLog))
					tempError = ((OtherErrorLog)this).ErrorAssociated;
				else if (this.GetType () == typeof(WorldErrorLog))
					tempError = ((WorldErrorLog)this).ErrorAssociated;
				else if (this.GetType () == typeof(MaxTimeErrorLog))
					tempError = this.Action.MaxTimeError;
				else if (this.GetType () == typeof(MinTimeErrorLog))
					tempError = this.Action.MinTimeError;
				else if (this.GetType () == typeof(IncompErrorLog))
					tempError = ((IncompErrorLog)this).FailedIncompatibility.IncompatibilityError;
				else if (this.GetType () == typeof(DepErrorLog)) {
					if (((DepErrorLog)this).IsOrderError)
						tempError = ((DepErrorLog)this).Action.Dependence.DependenceError;
					else
						tempError = ((DepErrorLog)this).FailedDependence.DependenceError;
				}
				return tempError;
			}
		}
	}
}

