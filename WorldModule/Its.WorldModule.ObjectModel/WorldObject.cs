using Its.StudentModule.ObjectModel;

namespace Its.WorldModule.ObjectModel
{
	/// <author>
	/// Diego Dotor Jara.
	/// </author>
	/// <summary>
	///  This class will contain the WorldObject atributes.
	/// </summary>
	public class WorldObject
	{
		/// <summary>
		/// The name.
		/// </summary>
		private string _name;
		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name {
			get {
				return _name;
			}
		}
		/// <summary>
		/// The is block.
		/// </summary>
		private bool _isBlock;
		/// <summary>
		/// Gets or sets a value indicating whether this instance is block.
		/// </summary>
		/// <value><c>true</c> if this instance is block; otherwise, <c>false</c>.</value>
		public bool IsBlock {
			get {
				return _isBlock;
			}
			set {
				_isBlock = value;
			}
		}
		/// <summary>
		/// The owner.
		/// </summary>
		private Student _owner;
		/// <summary>
		/// Gets or sets the owner.
		/// </summary>
		/// <value>The owner.</value>
		public Student Owner {
			get {
				return _owner;
			}
			set {
				_owner = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.WorldModule.ObjectModel.WorldObject"/> class.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="isBlock">If set to <c>true</c> is block.</param>
		/// <param name="owner">Owner.</param>
		public WorldObject (string name)
		{
			this._name = name;
			this._isBlock = false;
			this._owner = null;
		}
	}
}

