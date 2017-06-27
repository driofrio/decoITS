using System;

namespace Its.Factories.Common
{

	/// <author>
	///  Diego Dotor Jara.
	/// </author>
	/// <summary>
	/// This namespace will contain exceptions.
	/// </summary>

	public class WorldObjectNotFoundException : Exception {
		public WorldObjectNotFoundException():base("The WorldObject has not been found.") {
		}
	}
}

