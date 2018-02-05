using System;
using Its.Utils.StringUtils;

namespace Its.StudentModule.ObjectModel
{

	/// <author>
	/// Diego Dotor Jara.
	/// </author>
	/// <summary>
	/// This class will contain all student's information.
	/// </summary>
	public class Student
	{

		/// <summary>
		/// Identification student string.
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
		/// Student name.
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
		/// Student middle name.
		/// </summary>
		private string _middleName;
		/// <summary>
		/// Gets the name of the middle.
		/// </summary>
		/// <value>The name of the middle.</value>
		public string MiddleName {
			get {
				return _middleName;
			}
		}

		/// <summary>
		/// Student surname.
		/// </summary>
		private string _surname;
		/// <summary>
		/// Gets the surname.
		/// </summary>
		/// <value>The surname.</value>
		public string Surname {
			get {
				return _surname;
			}
		}

		/// <summary>
		/// Student last name.
		/// </summary>
		private string _lastName;
		/// <summary>
		/// Gets the last name.
		/// </summary>
		/// <value>The last name.</value>
		public string LastName {
			get {
				return _lastName;
			}
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="Its.Student.ObjectModel.Student"/> class.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="name">Name.</param>
		/// <param name="surname">Surname.</param>
		/// <exception cref="ArgumentException"> 
		/// Thrown if the <paramref name="key"/>, <paramref name="name"/> or <paramref name="surname"/> are null or blank.
		/// </exception>
		public Student (string key, string name, string surname)
		{
			//If the key parameter is null or empty or his contained is a blank, a exception is thrown.
			//For the name and the surname parameters also a exception is thrown.
			if (StringUtils.IsNullOrWhiteSpace (key)) {  
				//This argument exception says that the key param is not correct.
				ArgumentException argEx = new System.ArgumentException ("The key parameter is not correct, " +
					"it can neither be null or empty string nor his contained be blank.", "key");
				throw argEx;
			}  else if (StringUtils.IsNullOrWhiteSpace (name)) {
				//This argument exception says that the name param is not correct.
				ArgumentException argEx = new System.ArgumentException ("The name parameter is not correct, " +
					"it can neither be null or empty string nor his contained be blank.", "name");
				throw argEx;
			}  else if (StringUtils.IsNullOrWhiteSpace (surname)) {
				//This argument exception says that the surname param is not correct.
				ArgumentException argEx = new System.ArgumentException ("The surname parameter is not correct, " +
					"it can neither be null or empty string nor his contained be blank.", "surname");
				throw argEx;
			}  else {
				this._key = key;
				this._name = name;
				this._surname = surname;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.Student.ObjectModel.Student"/> class.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="name">Name.</param>
		/// <param name="middleName">Middle name.</param>
		/// <param name="surname">Surname.</param>
		/// <param name="lastName">Last name.</param>
		/// <exception cref="ArgumentException"> 
		/// Thrown if the <paramref name="key"/>, <paramref name="name"/> or <paramref name="surname"/> are null or blank.
		/// </exception>
		public Student (string key, string name, string middleName, string surname, string lastName)
		{
			//If the key parameter is null or empty or his contained is a blank, a exception is thrown.
			//For the name, surname, middleName and lastName parameters also a exception is thrown.
			if (StringUtils.IsNullOrWhiteSpace (key)) {  
				//This argument exception says that the key param is not correct.
				ArgumentException argEx = new System.ArgumentException ("The key parameter is not correct, " +
					"it can neither be null or empty string nor his contained be blank .", "key");
				throw argEx;
			}  else if (StringUtils.IsNullOrWhiteSpace (name)) {
				//This argument exception says that the name param is not correct.
				ArgumentException argEx = new System.ArgumentException ("The name parameter is not correct, " +
					"it can neither be null or empty string nor his contained be blank .", "name");
				throw argEx;
			}  else if (StringUtils.IsNullOrWhiteSpace (surname)) {
				//This argument exception says that the surname param is not correct.
				ArgumentException argEx = new System.ArgumentException ("The surname parameter is not correct, " +
					"it can neither be null or empty string nor his contained be blank .", "surname");
				throw argEx;
			}  else {
				this._key = key;
				this._name = name;
				this._middleName = middleName;
				this._surname = surname;
				this._lastName = lastName;
			}
		}
	}
}
