using System;

namespace Its.ExpertModule.Exceptions
{	
	/// <author>
	///  Diego Dotor Jara.
	/// </author>
	/// <summary>
	/// This namespace will contain exceptions.
	/// </summary>

	public class IncorrectDependenceStructureException : Exception {
		public IncorrectDependenceStructureException():base("There is an incorrect dependence structure in the domain configuration file.") {
		}
	}
}

