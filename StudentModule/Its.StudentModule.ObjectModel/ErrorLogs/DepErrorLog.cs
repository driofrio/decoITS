using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Its.ExpertModule.ObjectModel;

namespace Its.StudentModule.ObjectModel
{

	/// <author>
	/// Diego Dotor Jara.
	/// </author>
	/// <summary>
	/// This class will contain a dependence error log.
	/// </summary>
	public class DepErrorLog : ErrorLog
	{
		/// <summary>
		/// Dependence that the error has associated.
		/// </summary>
		private Dependence _failedDependence;
		/// <summary>
		/// Gets the failed dependence.
		/// </summary>
		/// <value>The failed dependence.</value>
		public Dependence FailedDependence {
			get {
				return _failedDependence;
			}
		}

		/// <summary>
		/// Boolean that denotes if the dependence error was caused by an order error.
		/// </summary>
		private bool _isOrderError;
		/// <summary>
		/// Gets a value indicating whether this instance is order error.
		/// </summary>
		/// <value><c>true</c> if this instance is order error; otherwise, <c>false</c>.</value>
		public bool IsOrderError {
			get {
				return _isOrderError;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.StudentModule.ObjectModel.DepErrorLog"/> class.
		/// </summary>
		/// <param name="action">Action.</param>
		/// <param name="wasApplied">If set to <c>true</c> was applied.</param>
		/// <param name="failedDependence">Failed dependence.</param>
		/// <param name="isOrderError">If set to <c>true</c> is order error.</param>
		public DepErrorLog (ActionAplication action, bool wasApplied,
			Dependence failedDependence, bool isOrderError) : base (action, wasApplied)
		{
			this._failedDependence = failedDependence;
			this._isOrderError = isOrderError;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.StudentModule.ObjectModel.DepErrorLog"/> class.
		/// </summary>
		/// <param name="action">Action.</param>
		/// <param name="wasApplied">If set to <c>true</c> was applied.</param>
		/// <param name="date">Date.</param>
		/// <param name="failedDependence">Failed dependence.</param>
		/// <param name="isOrderError">If set to <c>true</c> is order error.</param>
		public DepErrorLog (ActionAplication action, bool wasApplied,
			DateTime date, Dependence failedDependence, bool isOrderError) : base (action, wasApplied, date)
		{
			this._failedDependence = failedDependence;
			this._isOrderError = isOrderError;
		}

		public override string TxtLogString()
		{
			return Action.Key + " " + Action.Name + " DependenceError " + FailedDependence.DependenceError.Message.Message +
			       " " + DateLog.ToString();
		}
	}
}

