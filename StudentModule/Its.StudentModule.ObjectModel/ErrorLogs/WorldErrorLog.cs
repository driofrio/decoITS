using System;
using Its.ExpertModule.ObjectModel;
using Its.TutoringModule.ReactiveTutor.ObjectModel;

namespace Its.StudentModule.ObjectModel
{

	/// <author>
	/// Diego Dotor Jara.
	/// </author>
	/// <summary>
	/// This class will contain a world error log.
	/// </summary>
	public class WorldErrorLog : ErrorLog
	{
		/// <summary>
		/// Error associated with this world error.
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
		/// The type.
		/// </summary>
		private string _type;
		/// <summary>
		/// Gets the type.
		/// </summary>
		/// <value>The type.</value>
		public string Type {
			get {
				return _type;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.StudentModule.ObjectModel.WorldErrorLog"/> class.
		/// </summary>
		/// <param name="action">Action.</param>
		/// <param name="wasApplied">If set to <c>true</c> was applied.</param>
		/// <param name="errorAssociated">Error associated.</param>
		/// <param name="type">Type.</param>
		public WorldErrorLog (ActionAplication action, bool wasApplied,
			Error errorAssociated, string type) : base (action, wasApplied)
		{
			this._errorAssociated = errorAssociated;
			this._type = type;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.StudentModule.ObjectModel.WorldErrorLog"/> class.
		/// </summary>
		/// <param name="action">Action.</param>
		/// <param name="wasApplied">If set to <c>true</c> was applied.</param>
		/// <param name="date">Date.</param>
		/// <param name="errorAssociated">Error associated.</param>
		/// <param name="type">Type.</param>
		public WorldErrorLog (ActionAplication action, bool wasApplied,
			DateTime date, Error errorAssociated, string type) : base ( action, wasApplied, date)
		{
			this._errorAssociated = errorAssociated;
			this._type = type;
		}

		public override string TxtLogString()
		{
			return Action.Key + " " + Action.Name + " WorldError " + Error.Message.Message + " " + DateLog.ToString();
		}
	}
}

