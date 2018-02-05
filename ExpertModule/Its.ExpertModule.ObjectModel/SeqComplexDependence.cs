using System;
using System.Collections.Generic;
using Its.TutoringModule.TutoringCoordinator.ReactiveTutor.ObjectModel;
using Its.Utils.StringUtils;

namespace Its.ExpertModule.ObjectModel
{
	public class SeqComplexDependence : ComplexDependence
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
		}
		/// <summary>
		/// List in which will be saved the dependences.
		/// </summary>
		private List<Dependence> _dependenciesList;
		/// <summary>
		/// Gets the list of dependences.
		/// </summary>
		/// <value>The list of dependences.</value>
		public List<Dependence> DependenciesList {
			get {
				return _dependenciesList;
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
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.Expert.ObjectModel.SeqComplexDependence"/> class.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="dependenciesList">List dependence.</param>
		/// <param name="error">Error.</param>
		/// <exception cref="ArgumentException"> 
		/// Thrown if the <paramref name="key"/> is empty or blank.
		/// </exception>
		public SeqComplexDependence (string key, List<Dependence> dependenciesList, Error error)
		{
			//If the key parameter is null or empty or his contained is a blank, a exception is thrown.
			if (StringUtils.IsNullOrWhiteSpace (key)) {  
				//This argument exception says that the key param is not correct.
				ArgumentException argEx = new System.ArgumentException ("The key parameter is not correct, " +
				                          "it can neither be null or empty string nor his contained be blank.", "key");
				throw argEx;
			} else {
				this._key = key;
				this._dependenciesList = dependenciesList;
				this._dependenceError = error;
			}
		}

		/// <summary>
		/// Gets the simple dependence by action key.
		/// </summary>
		/// <returns>The simple dependence by action key.</returns>
		/// <param name="compDep">Comp dep.</param>
		/// <param name="actionKey">Action key.</param>
		public SimpleDependence GetSimpleDependenceByActionKey(string actionKey)
		{
			SimpleDependence simpDep=null;
			foreach (Dependence dep in this.DependenciesList) {
				if (dep.GetType () == typeof(SimpleDependence)) {
					if (((SimpleDependence)dep).ActionDependence.Key == actionKey) {
						simpDep = (SimpleDependence)dep;
					}
				} else {
					simpDep = ((ComplexDependence)dep).GetSimpleDependenceByActionKey (actionKey);
				}
				if(simpDep!=null)
					break;
			}
			return simpDep;
		}

		/// <summary>
		/// Gets the dependence.
		/// </summary>
		/// <returns>The dependence.</returns>
		/// <param name="dependenceKey">Dependence key.</param>
		public Dependence GetDependence (string dependenceKey)
		{
			//Creates a Dependence.
			Dependence dep = null;
			//Searchs the Dependence with the given key.
			foreach (Dependence d in this.DependenciesList) {
				//Checks the type of the dependence.
				if (d.GetType () == typeof(SimpleDependence)) {
					//Checks if the dependence key is equals to the given.
					if (((SimpleDependence)d).Key == dependenceKey) {
						//Gets the dependence.
						dep = d;
					}
				} else {
					//Calls the GetDepedence method.
					dep = ((ComplexDependence)d).GetDependence (dependenceKey);
				}
				//If the dependence has been found then break the loop.
				if(dep!=null)
					break;
			}

			//Returns the dependence.
			return dep;
		}
	}
}

