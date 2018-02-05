using System;
using Its.TutoringModule.TutoringCoordinator.ReactiveTutor.ObjectModel;
using Its.Utils.StringUtils;

namespace Its.ExpertModule.ObjectModel
{

	/// <author>
	/// Diego Dotor Jara.
	/// </author>
	/// <summary>
	/// This class will contain the incompatibility.
	/// </summary>
	public class Incompatibility
	{
		/// <summary>
		/// Identification incompatibility number.
		/// </summary>
		private string _key;
		/// <summary>
		/// Gets the key.
		/// </summary>
		/// <value>The key.</value>
		public string Key {
			get {
				return _key;
			}
		}
		/// <summary>
		/// Action which is the incompatibility.
		/// </summary>
		private ActionAplication _incompatibilityAction;
		/// <summary>
		/// Gets the incompatibility action.
		/// </summary>
		/// <value>The incompatibility action.</value>
		public ActionAplication IncompatibilityAction {
			get {
				return _incompatibilityAction;
			}
		}

		/// <summary>
		/// Error which is the associated error.
		/// </summary>
		private Error _incompatibilityError;
		/// <summary>
		/// Gets the incompatibility error.
		/// </summary>
		/// <value>The incompatibility error.</value>
		public Error IncompatibilityError {
			get {
				return _incompatibilityError;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.Expert.ObjectModel.Incompatibility"/> class.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="incompatibilityAction">Incompatibility action.</param>
		/// <param name="incompatibilityError">Incompatibility error.</param>
		/// <exception cref="ArgumentException"> 
		/// Thrown if the <paramref name="key"/> is empty or blank.
		/// </exception>
		public Incompatibility (string key, ActionAplication incompatibilityAction, Error incompatibilityError)
		{
			//If the key parameter is null or empty or his contained is a blank, a exception is thrown.
			if (StringUtils.IsNullOrWhiteSpace (key)) {  
				//This argument exception says that the key param is not correct.
				ArgumentException argEx = new System.ArgumentException ("The key parameter is not correct, " +
				                          "it can neither be null or empty string nor his contained be blank.", "key");
				throw argEx;
			} else {
				this._key = key;
				this._incompatibilityAction = incompatibilityAction;
				this._incompatibilityError = incompatibilityError;
			}
		}
	}
}

