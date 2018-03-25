using System;
using System.Collections.Generic;
using Its.TutoringModule.ReactiveTutor.ObjectModel;
using Its.Utils.StringUtils;

namespace Its.ExpertModule.ObjectModel
{
	/// <author>
	/// Diego Dotor Jara.
	/// </author>
	/// <summary>
	/// This class will contain the dependence list of an Action, in which only there is an Action.
	/// </summary>
	public class SimpleDependence : Dependence
	{
		/// <summary>
		/// Identification action number.
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
			set {
				_key = value;
			}
		}
		/// <summary>
		/// Dependence action.
		/// </summary>
		private ActionAplication _actionDependence;
		/// <summary>
		/// Gets the action dependence.
		/// </summary>
		/// <value>The action dependence.</value>
		public ActionAplication ActionDependence {
			get {
				return _actionDependence;
			}
		}
		/// <summary>
		/// Error associated with this dependence.
		/// </summary>
		private Error _dependenceError;
		/// <summary>
		/// Gets the dependence error.
		/// </summary>
		/// <value>The dependence error.</value>
		public Error DependenceError {
			get {
				return _dependenceError;
			}
			set {
				_dependenceError = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.Expert.ObjectModel.SimpleDependence"/> class.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="action">Action.</param>
		/// <param name="dependenceError">Dependence error.</param>
		/// <exception cref="ArgumentException"> 
		/// Thrown if the <paramref name="key"/> is empty or blank.
		/// </exception>
		public SimpleDependence (string key, ActionAplication action, Error dependenceError)
		{
			//If the key parameter is null or empty or his contained is a blank, a exception is thrown.
			if (StringUtils.IsNullOrWhiteSpace (key)) {  
				//This argument exception says that the key param is not correct.
				ArgumentException argEx = new System.ArgumentException ("The key parameter is not correct, " +
				                          "it can neither be null or empty string nor his contained be blank.", "key");
				throw argEx;
			} else {
				this._key = key;
				this._actionDependence = action;
				this._dependenceError = dependenceError;
			}
		}

		/// <summary>
		/// Clone this instance.
		/// </summary>
		public SimpleDependence Clone()
		{
			//Creates a new SimpleDependence.
			SimpleDependence dependence = new SimpleDependence(Key, ActionDependence, DependenceError);
			return dependence;
		}
	}
}

