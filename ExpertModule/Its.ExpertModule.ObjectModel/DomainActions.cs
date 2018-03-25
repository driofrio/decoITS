using System;
using System.Collections.Generic;
using Its.Utils.StringUtils;

namespace Its.ExpertModule.ObjectModel
{

	/// <author>
	/// Diego Dotor Jara.
	/// </author>
	/// <summary>
	/// This class will contain the actions set of a practice.
	/// </summary>
	public class DomainActions
	{
		///<summary>
		/// Identification key.
		/// </summary>
		/// <value>
		/// string
		/// </value>
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
		/// Action description.
		/// </summary>
		private string _description;
		/// <summary>
		/// Gets the description.
		/// </summary>
		/// <value>The description.</value>
		public string Description {
			get {
				return _description;
			}
		}
		/// <summary>
		/// List of actions that the practice has associated.
		/// </summary>
		private List<ActionAplication> _actions;
		/// <summary>
		/// Gets the actions.
		/// </summary>
		/// <value>The actions.</value>
		public List<ActionAplication> Actions {
			get {
				return _actions;
			}
		}

		/// <summary>
		/// Action description.
		/// </summary>
		private int _estimatedTime;
		/// <summary>
		/// Gets the description.
		/// </summary>
		/// <value>The estimaed time it takes (hours).</value>
		public int EstimatedTime {
			get {
				return _estimatedTime;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.Expert.ObjectModel.DomainActions"/> class.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="description">Description.</param>
		/// <param name="actions">Actions.</param>
		/// <param name="estimatedTime">Estimaed time it takes (hours).</param>
		/// <exception cref="ArgumentException"> 
		/// Thrown if the <paramref name="key"/> is empty or blank.
		/// </exception>
		public DomainActions (string key, string description, List<ActionAplication> actions, int estimatedTime)
		{
			//If the key parameter is null or empty or his contained is a blank, a exception is thrown.
			if (StringUtils.IsNullOrWhiteSpace (key)) {  
				//This argument exception says that the key param is not correct.
				ArgumentException argEx = new ArgumentException ("The key parameter is not correct, " +
				                          "it can neither be null or empty string nor his contained be blank.", "key");
				throw argEx;
			} else {
				this._key = key;
				this._description = description;
				this._actions = actions;
				this._estimatedTime = estimatedTime;
			}
		}

		/// <summary>
		/// Gets the first action.
		/// </summary>
		/// <returns>The first action.</returns>
		public ActionAplication GetFirstAction(){
			return _actions [0];
		}

		/// <summary>
		/// Gets the last action.
		/// </summary>
		/// <returns>The last action.</returns>
		public ActionAplication GetLastAction(){
			return _actions [_actions.Count-1];
		}

		/// <summary>
		/// Gets the previous action.
		/// </summary>
		/// <returns>The previous action.</returns>
		/// <param name="actionKey">Action key.</param>
		public ActionAplication GetPreviousAction (string actionKey)
		{
			//Creates auxiliar variables.
			ActionAplication previousAction = null;
			ActionAplication currentAction;
			//Checks if an action with the given key is contained in the action list.
			if (ActionWithKey(actionKey)) {
				//Gets the list enumerator.
				List<ActionAplication>.Enumerator enumerator = _actions.GetEnumerator ();
				//Gets the previous action.
				previousAction = enumerator.Current;
				//Searchs the action given an the previous one.
				while (enumerator.MoveNext ()) {
					//Gets the current action.
					currentAction = enumerator.Current;
					//Checks if the current action has the given key.
					if (currentAction.Key == actionKey)
						break;
					else
						previousAction = enumerator.Current;
				}
			}

			//Returns the result.
			return previousAction;
		}

		/// <summary>
		/// Delegate function that return if exist an element with the given key.
		/// </summary>
		/// <returns><c>true</c>, if with key was actioned, <c>false</c> otherwise.</returns>
		/// <param name="actionKey">Action key.</param>
		private bool ActionWithKey (string actionKey)
		{
			//Creates an auxiliar variable.
			bool result = false;
			//Seearchs if there is an action with the given key.
			foreach (ActionAplication a in _actions) {
				if (a.Key == actionKey) {
					result = true;
					break;
				}
			}

			//Returns the result.
			return result;
		}
	}
}

