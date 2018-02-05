using System;
using System.Collections.Generic;
using Its.Utils.StringUtils;

namespace Its.TutoringModule.TutoringCoordinator.ReactiveTutor.ObjectModel
{
	/// <author>
	/// Diego Dotor Jara.
	/// </author>
	/// <summary>
	/// This class will contain the Error message.
	/// </summary>
	public class TutorMessage
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
		/// The message.
		/// </summary>
		private string _message;
		/// <summary>
		/// Gets the message.
		/// </summary>
		/// <value>The message.</value>
		public string Message {
			get {
				return _message;
			}
		}
		/*/// <summary>
		/// The message. The key is the language and the value is the message.
		/// </summary>
		private Dictionary<string, string> _message;
		/// <summary>
		/// Gets the message.
		/// </summary>
		/// <value>The message.</value>
		public Dictionary<string, string> Message {
			get {
				return _message;
			}
		}*/

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.ReactiveTutor.ObjectModel.TutorMessage"/> class.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="message">Message.</param>
		/// <exception cref="ArgumentException"> 
		/// Thrown if the <paramref name="key"/> is empty or blank.
		/// </exception>
		public TutorMessage (string key, /*Dictionary<string, string>*/ string message)
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
			}
		}
	}
}

