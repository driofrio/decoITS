using System;
using System.Collections.Generic;
using Its.TutoringModule.ReactiveTutor.ObjectModel;

namespace Its.ExpertModule.ObjectModel
{

	/// <author>
	/// Diego Dotor Jara.
	/// </author>
	/// <summary>
	/// This interface defines the complex dependences.
	/// </summary>
	public interface ComplexDependence : Dependence
	{
		/// <summary>
		/// Gets the list of dependences.
		/// </summary>
		/// <value>The list of dependences.</value>
		List<Dependence> DependenciesList {
			get;
		}

		/// <summary>
		/// Gets the simple dependence by action key.
		/// </summary>
		/// <returns>The simple dependence by action key.</returns>
		/// <param name="compDep">Comp dep.</param>
		/// <param name="actionKey">Action key.</param>
		SimpleDependence GetSimpleDependenceByActionKey (string actionKey);

		/// <summary>
		/// Gets the dependence.
		/// </summary>
		/// <returns>The dependence.</returns>
		/// <param name="dependenceKey">Dependence key.</param>
		Dependence GetDependence (string dependenceKey);
	}
}

