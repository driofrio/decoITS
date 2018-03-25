using System;
using Its.TutoringModule.ReactiveTutor.ObjectModel;

namespace Its.ExpertModule.ObjectModel
{
	/// <author>
	/// Diego Dotor Jara.
	/// </author>
	/// <summary>
	/// This interface defines the dependence.
	/// </summary>
	public interface Dependence
	{
		/// <summary>
		/// Gets the key.
		/// </summary>
		/// <value>The key.</value>
		string Key {
			get;
		}
		/// <summary>
		/// Gets the dependence error.
		/// </summary>
		/// <value>The dependence error.</value>
		Error DependenceError {
			get;
		}
	}
}

