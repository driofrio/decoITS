using System;

namespace Its.WorldModule.ObjectModel
{
	/// <author>
	/// Diego Dotor Jara.
	/// </author>
	/// <summary>
	///  This class will contain all information which is needed by the npc.
	/// </summary>
	public class NpcActionPosition
	{
		/// <summary>
		/// The world object.
		/// </summary>
		private WorldObject _worldObj;
		/// <summary>
		/// Gets the world object.
		/// </summary>
		/// <value>The world object.</value>
		public WorldObject WorldObj {
			get {
				return _worldObj;
			}
		}
		/// <summary>
		/// The action.
		/// </summary>
		private string _action;
		/// <summary>
		/// Gets the action.
		/// </summary>
		/// <value>The action.</value>
		public string Action {
			get {
				return _action;
			}
		}
		/// <summary>
		/// The domain.
		/// </summary>
		private string _plan;
		/// <summary>
		/// Gets the domain.
		/// </summary>
		/// <value>The domain.</value>
		public string Plan {
			get {
				return _plan;
			}
		}
		/// <summary>
		/// The npc coordinate.
		/// </summary>
		private float[] _npcCoord;
		/// <summary>
		/// Gets the npc coordinate.
		/// </summary>
		/// <value>The npc coordinate.</value>
		public float[] NpcCoord {
			get {
				return _npcCoord;
			}
		}
		/// <summary>
		/// The npc orientation.
		/// </summary>
		private float[] _npcOrientation;
		/// <summary>
		/// Gets the npc orientation.
		/// </summary>
		/// <value>The npc orientation.</value>
		public float[] NpcOrientation {
			get {
				return _npcOrientation;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.WorldModule.ObjectModel.npcActionPosition"/> class.
		/// </summary>
		/// <param name="worldObj">World object.</param>
		/// <param name="action">Action.</param>
		/// <param name="domain">Domain.</param>
		/// <param name="npcCoord">Npc coordinate.</param>
		/// <param name="npcOrientation">Npc orientation.</param>
		/// <exception cref="ArgumentException"> 
		/// Thrown if the <paramref name="npcCoord"/> has not the correct number of elements.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="npcOrientation"/> has not the correct number of elements.
		/// </exception>
		public NpcActionPosition (WorldObject worldObj, string action, string plan, float[] npcCoord,
			float[] npcOrientation)
		{
			this._worldObj = worldObj;
			this._action = action;
			this._plan = plan;
			//Checks if npcCoord has more than elements three or less than 3.
			if (npcCoord.Length > 3 || npcCoord.Length < 3) {
				//This argument exception says that the npcCoord param is not correct.
				ArgumentException argEx = new ArgumentException ("The number of elements is not correct. It must have" +
				                          "three elements, x y z", "npcCoord");
				throw argEx;
			}
			//Checks if npcOrientation has more than elements three or less than 3.
			if (npcOrientation.Length > 3 || npcOrientation.Length < 3) {
				//This argument exception says that the npcOrientation param is not correct.
				ArgumentException argEx = new ArgumentException ("The number of elements is not correct. It must have" +
				                          "three elements, x y z", "npcOrientation");
				throw argEx;
			}
			this._npcCoord = npcCoord;
			this._npcOrientation = npcOrientation;
		}
	}
}

