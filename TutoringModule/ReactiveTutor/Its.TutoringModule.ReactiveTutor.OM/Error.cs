using System;
using Its.Utils.StringUtils;

namespace Its.TutoringModule.ReactiveTutor.ObjectModel
{

	/// <author>
	///  Diego Dotor Jara.
	/// </author>
	/// <summary>
	/// This class will contain the Error.
	/// </summary>
	public class Error
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
		/// Error message.
		/// </summary>
		private ErrorMessage _message;
		/// <summary>
		/// Gets the message.
		/// </summary>
		/// <value>The message.</value>
		public ErrorMessage Message {
			get {
				return _message;
			}
		}
		/// <summary>
		/// Boolean that denotes if the error blocks the action.
		/// </summary>
		private bool _isBlock;
		/// <summary>
		/// Gets a value indicating whether this instance is block.
		/// </summary>
		/// <value><c>true</c> if this instance is block; otherwise, <c>false</c>.</value>
		public bool IsBlock {
			get {
				return _isBlock;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Error"/> class.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="message">Message.</param>
		/// <param name="isBlock">If set to <c>true</c> is block.</param>
		/// <exception cref="ArgumentException"> 
		/// Thrown if the <paramref name="key"/> is empty or blank.
		/// </exception>
		public Error (string key, ErrorMessage message, bool isBlock)
		{
			//If the key parameter is null or empty or his contained is a blank, a exception is thrown.
			if (StringUtils.IsNullOrWhiteSpace (key)) {  
				//This argument exception says that the key param is not correct.
				ArgumentException argEx = new System.ArgumentException ("The key parameter is not correct, " +
				                          "it can neither be null or empty string nor his contained be blank.", "key");
				throw argEx;
			} else {
				this._key = key;
				this._message = message;
				this._isBlock = isBlock;
			}
		}

		public Error(){
		}

		/// <summary>
		/// Clone this instance.
		/// </summary>
		public Error Clone()
		{
			//Creates a new Error.
			Error error = new Error(Key, Message, IsBlock);
			return error;
		}
	}
}

