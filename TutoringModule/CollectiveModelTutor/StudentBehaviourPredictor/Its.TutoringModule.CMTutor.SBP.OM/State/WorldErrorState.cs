using Its.TutoringModule.ReactiveTutor.ObjectModel;

namespace Its.TutoringModule.CMTutor.SBP.OM.State
{
	public class WorldErrorState : State
	{
		/// <summary>
		/// The error associated.
		/// </summary>
		private Error _errorAssociated;
		/// <summary>
		/// The type.
		/// </summary>
		private string _type;

		/// <summary>
		/// Gets the type.
		/// </summary>
		/// <value>The type.</value>
		public string Type{
			get{
				return _type;
			}
		}

		/// <summary>
		/// Gets the error associated.
		/// </summary>
		/// <value>The error associated.</value>
		public Error ErrorAssociated{
			get{
				return _errorAssociated;
			}
		}

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="WorldErrorState"/> class.
		/// </summary>
		/// <param name="area">Area.</param>
		/// <param name="errorAssociated">Error associated.</param>
		public WorldErrorState (Area area, Error errorAssociated, string type) : base(area)
		{
			this._errorAssociated = errorAssociated;
			this._type = type;
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="WorldErrorState"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="WorldErrorState"/>.</returns>
		public override string ToString(){
			return "WorldErrorState: Type: " + _type + "ErrorAssociated: " + _errorAssociated.Key + base.ToString ();
		}
	}
}

