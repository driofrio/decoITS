using System;
using System.Collections.Generic;
using System.Linq;
using Its.StudentModule.DataAccess;
using Its.ExpertModule.DataAccess;
using Its.ExpertModule.ObjectModel;
using Its.ExpertModule.Exceptions;
using Its.TutoringModule.TutoringCoordinator.ReactiveTutor.ObjectModel;

namespace Its.Factories
{
	/// <author>
	///  Diego Dotor Jara.
	/// </author>
	/// <summary>
	/// This class will create DomainActions from Configuration File.
	/// </summary>
	public class DomainActionsFactory
	{

		/// <summary>
		/// The ontology.
		/// </summary>
		private static OntologyAccess ONTOLOGY;// = OntologyAccess.Instance;
		/// <summary>
		/// The action access.
		/// </summary>
		private static ActionAccess ACTION_ACCESS; //= ActionAccess.Instance;
		/// <summary>
		/// The DomainActionsFactory singleton instance.
		/// </summary>
		private static DomainActionsFactory _instance = null;
		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static DomainActionsFactory Instance (string ontologyPath, string logsPath, string expertConfPath, int initialCol, int initialRow){
			//get {
			if (_instance == null)
				_instance = new DomainActionsFactory (ontologyPath, logsPath, expertConfPath, initialCol, initialRow);

			return _instance;
			//}
		}
		/// <summary>
		/// The generator tutor message key. z
		/// </summary>
		private static int _genTutorMsgKey;
		/// <summary>
		/// The generator error message key.
		/// </summary>
		private static int _genErrorMsgKey;
		/// <summary>
		/// The generator error key.
		/// </summary>
		private static int _genErrorKey;

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.Factories.DomainActionsFactory"/> class.
		/// </summary>
		private DomainActionsFactory (string ontologyPath, string logsPath, string expertConfPath, int initialCol, int initialRow) 
		{
			ONTOLOGY = OntologyAccess.Instance (ontologyPath, logsPath);
			ACTION_ACCESS = ActionAccess.Instance (expertConfPath, initialCol, initialRow);
		}

		/// <summary>
		/// Creates the seq complex dep.
		/// </summary>
		/// <returns>The seq complex dep.</returns>
		/// <param name="structure">Structure.</param>
		/// <param name="dependencies">Dependencies.</param>
		/// <param name="errorMessages">Error messages.</param>
		/// <param name="orderErrorMessage">Order error message.</param>
		/// <param name="depContainer">Dep container.</param>
		/// <param name="actContainer">Act container.</param>
		/// <param name="errMsgContainer">Error message container.</param>
		/// <exception cref="IncorrectDependenceStructureException"> 
		/// Thrown if there is an incorrect structure in dependencies cell in configuration sheet.
		/// </exception>
		private SeqComplexDependence CreateSeqComplexDep(ref LinkedList<string> structure, ref LinkedList<string> dependencies, 
			ref LinkedList<string> errorMessages, ref LinkedList<string> orderErrorMessage, ref Dictionary<string, Dependence> depContainer, 
			ref List<ActionAplication> actContainer, ref Dictionary<string, ErrorMessage> errMsgContainer)
		{
			//The list in which will be saved the dependencies.
			List<Dependence> listDependencies = new List<Dependence>();
			//The auxiliar variable.
			Dependence dep;
			//Obtains the elements of the structure.
			while (structure.Count > 0) {
				//Obtains the element.
				string e = structure.First.Value;
				//Checks the first char of the first element.
				if (e[0] == '[') {
					//Obtains the index of the [ char.
					int i = e.IndexOf ('[');
					//Erases the char '['.
					string s = e.Substring (i+1);
					structure.RemoveFirst ();
					structure.AddFirst (s);
					//Creates a OptComplexDependence.
					dep = (Dependence)CreateOptComplexDep (ref structure, ref dependencies, ref errorMessages, ref orderErrorMessage,
						ref depContainer, ref actContainer, ref errMsgContainer);
					//Adds into the list of dependencies.
					listDependencies.Add (dep);
					//Refresh the value.
					e = structure.First.Value;
					//Checks if is necessary to close the dependence.
					if (e[0] == ')') {
						//Checks if there is another close char.
						if (e.Length > 1) {
							//Erases the char ')'.
							s = e.Substring (1);
							structure.RemoveFirst ();
							structure.AddFirst (s);
						} else {
							//Erases the element of the structure.
							structure.RemoveFirst ();
						}
						break;
					} else if (e[0] == ']') {
						//Throw an exception.
						throw new IncorrectDependenceStructureException ();
					}
				} else if (e[0] == '(') {
					//Obtains the index of the ( char.
					int i = e.IndexOf ('(');
					//Erases the char '('.
					string s = e.Substring (i+1);
					structure.RemoveFirst ();
					structure.AddFirst (s);
					//Creates a SeqComplexDependence.
					dep = (Dependence)CreateSeqComplexDep (ref structure, ref dependencies, ref errorMessages, ref orderErrorMessage,
						ref depContainer, ref actContainer, ref errMsgContainer);
					//Adds into the list of dependencies.
					listDependencies.Add (dep);
					//Refresh the value.
					e = structure.First.Value;
					//Checks if is necessary to close the dependence.
					if (e[0] == ')') {
						//Checks if there is another close char.
						if (e.Length > 1) {
							//Erases the char ')'.
							s = e.Substring (1);
							structure.RemoveFirst ();
							structure.AddFirst (s);
						} else {
							//Erases the element of the structure.
							structure.RemoveFirst ();
						}
						break;
					} else if (e[0] == ']') {
						//Throw an exception.
						throw new IncorrectDependenceStructureException ();
					}
				} else {
					//Creates a SimpleDependence.
					SimpleDependence sDep;
					//Gets the first simple dependence.
					string sDepStr = dependencies.First.Value;
					//Gets the first error.
					string err = errorMessages.First.Value;
					//Obtains the error information.
					string[] auxErrorInformation = err.Split (new char[] { '-' });
					//Checks if the dependence exists in the container.
					var queryDep1 =
						from o in depContainer
						where o.Key == sDepStr || o.Key.StartsWith (sDepStr + "_") == true
						select o.Value;
					if (queryDep1.Count () > 0) {
						//Searchs the SimpleDependence in the container.
						var queryDep2 = 
							from o in queryDep1
							where o.DependenceError.Message.Message == (auxErrorInformation [1])
							select o;
						//Checks if there is a Dependence with the error message.
						if (queryDep2.Count () == 0) {
							//Gets a copy of the dependence.
							sDep = ((SimpleDependence)queryDep1.First ()).Clone ();
							//Creates an ErrorMessage.
							ErrorMessage errMsg = new ErrorMessage (_genErrorMsgKey.ToString(), auxErrorInformation [1]);
							//Increase the generator.
							_genErrorMsgKey++;
							//Adds into the container.
							errMsgContainer.Add (auxErrorInformation [1], errMsg);
							//Creates an Error.
							Error error;
							//Creates a new instance.
							int i = int.Parse(auxErrorInformation [0]);
							bool b = false;
							if (i == 1)
								b = true;
							error = new Error (_genErrorKey.ToString(), errMsg, b);
							//Increase the generator.
							_genErrorKey++;
							//Sets the error.
							sDep.DependenceError = error;
							//Sets the key.
							sDep.Key = sDepStr + "_" + sDep.DependenceError.Key;
							//Adds the SimpleDependence into the container.
							depContainer.Add (sDep.Key, sDep);
						} else {
							//Gets the Dependence.
							sDep = (SimpleDependence)queryDep2.First ();
						}
						//Adds into the dependencies list.
						listDependencies.Add (sDep);
					} else {
						//Creates an ErrorMessage.
						ErrorMessage errMsg;
						//Checks if the ErrorMessage does not exist in the container
						//If not, creates the ErrorMessage
						//Else, obtains the ErrorMessage from the container.
						if (!errMsgContainer.ContainsKey(auxErrorInformation[1])) {
							//Creates a new instance.
							errMsg = new ErrorMessage (_genErrorMsgKey.ToString(), auxErrorInformation [1]);
							//Increase the generator.
							_genErrorMsgKey++;
							//Adds into the container.
							errMsgContainer.Add (auxErrorInformation [1], errMsg);
						} else {
							//Searchs the error message in the container.
							var queryErrMsg = 
								from o in errMsgContainer
								where o.Key == (auxErrorInformation [1])
								select o.Value;
							errMsg = queryErrMsg.First ();
						}
						//Creates an Error.
						Error error;
						//Creates a new instance.
						int i = int.Parse(auxErrorInformation [0]);
						bool b = false;
						if (i == 1)
							b = true;
						error = new Error (_genErrorKey.ToString(), errMsg, b);
						//Increase the generator.
						_genErrorKey++;
						//Searchs the ActionAplication in the container.
						var queryAct = 
							from o in actContainer
							where o.Key == (sDepStr)
							select o;
						//Creates a new instance of SimpleDependence.
						sDep = new SimpleDependence (sDepStr, queryAct.First (), error);
						//Adds the SimpleDependence into the container.
						depContainer.Add (sDep.Key, sDep);
						//Adds into the dependencies list.
						listDependencies.Add (sDep);
					}
					//Checks if the element is the last element into the list.
					if (e == structure.Last() && !e.Contains (")")) {
						//Throw an exception.
						throw new IncorrectDependenceStructureException ();
					} else if (e == structure.Last() && e.Contains (")")) {
						//Obtains the index of the ) char.
						int i = e.IndexOf (')');
						//Checks if the index is the last position.
						if (i == (e.Length - 1)) {
							//Checks if the element contains the ']' char. If it is contained, this means
							//that this char is placed before ')' char.
							if (e.Contains("]")) {
								//Throw an exception.
								throw new IncorrectDependenceStructureException ();
							}
							//Erases the element of the structure.
							structure.RemoveFirst ();
							//Erases the element of the dependencies.
							dependencies.RemoveFirst ();
							//Erases the element of the structure.
							errorMessages.RemoveFirst ();
							break;
						} else {
							//Erases all elements of the string from the beginning to the char ')', including this.
							string s = e.Substring (i+1);
							structure.RemoveFirst ();
							structure.AddFirst (s);
							//Erases the element of the dependencies.
							dependencies.RemoveFirst ();
							//Erases the element of the structure.
							errorMessages.RemoveFirst ();
							break;
						}
					} else if (e.Contains (")")) {
						//Obtains the index of the ) char.
						int i = e.IndexOf (')');
						//Checks if the index is the last position.
						if (i == (e.Length - 1)) {
							//Checks if the element contains the ']' char. If it is contained, this means
							//that this char is placed before ')' char.
							if (e.Contains("]")) {
								//Throw an exception.
								throw new IncorrectDependenceStructureException ();
							}
							//Erases the element of the structure.
							structure.RemoveFirst ();
							//Erases the element of the dependencies.
							dependencies.RemoveFirst ();
							//Erases the element of the structure.
							errorMessages.RemoveFirst ();
							break;
						} else {
							//Erases all elements of the string from the beginning to the char ')', including this.
							string s = e.Substring (i+1);
							structure.RemoveFirst ();
							structure.AddFirst (s);
							//Erases the element of the dependencies.
							dependencies.RemoveFirst ();
							//Erases the element of the structure.
							errorMessages.RemoveFirst ();
							break;
						}
					} else {
						//Checks if there is a optional close char before closing this dependence.
						if (e.Contains ("]")) {
							//Throw an exception.
							throw new IncorrectDependenceStructureException ();
						}
						//Erases the element of the structure.
						structure.RemoveFirst ();
						//Erases the element of the dependencies.
						dependencies.RemoveFirst ();
						//Erases the element of the structure.
						errorMessages.RemoveFirst ();
					}
				}
			}
			//Creates the key of the dependence.
			string key = "(";
			foreach (Dependence d in listDependencies)
				key += d.Key;
			key += ")";
			//Creates the variable to return.
			SeqComplexDependence depToReturn;
			//Checks if the dependence exists into the container.
			if (depContainer.ContainsKey(key)) {
				//Searchs the dependence in the container.
				var query = 
					from o in depContainer
					where o.Key == key
					select o.Value;
				depToReturn = (SeqComplexDependence)query.First ();
			} else {
				//Gets the first error.
				string err = orderErrorMessage.First.Value;
				//Obtains the error information.
				string[] auxErrorInformation = err.Split (new char[] { '-' });
				//Creates an ErrorMessage.
				ErrorMessage errMsg;
				//Checks if the ErrorMessage does not exist in the container
				//If not, creates the ErrorMessage
				//Else, obtains the ErrorMessage from the container.
				if (!errMsgContainer.ContainsKey(auxErrorInformation[1])) {
					//Creates a new instance.
					errMsg = new ErrorMessage (_genErrorMsgKey.ToString(), auxErrorInformation [1]);
					//Increase the generator.
					_genErrorMsgKey++;
					//Adds into the container.
					errMsgContainer.Add (auxErrorInformation [1], errMsg);
				} else {
					//Searchs the error message in the container.
					var query = 
						from o in errMsgContainer
						where o.Key == (auxErrorInformation [1])
						select o.Value;
					errMsg = query.First ();
				}
				//Creates an Error.
				Error error;
				//Creates a new instance.
				int i = int.Parse(auxErrorInformation [0]);
				bool b = false;
				if (i == 1)
					b = true;
				error = new Error (_genErrorKey.ToString(), errMsg, b);
				//Increase the generator.
				_genErrorKey++;
				//Erases the orden error from the container.
				orderErrorMessage.RemoveFirst ();
				//Creates a new instances of OptComplexDependence.
				depToReturn = new SeqComplexDependence (key, listDependencies, error);
				//Adds into the dependencies container.
				depContainer.Add(depToReturn.Key, (Dependence)depToReturn);
			}
			//Returns the dependence.
			return depToReturn;
		}

		/// <summary>
		/// Creates the opt complex dep.
		/// </summary>
		/// <returns>The opt complex dep.</returns>
		/// <param name="structure">Structure.</param>
		/// <param name="dependencies">Dependencies.</param>
		/// <param name="errorMessages">Error messages.</param>
		/// <param name="orderErrorMessage">Order error message.</param>
		/// <param name="depContainer">Dep container.</param>
		/// <param name="actContainer">Act container.</param>
		/// <param name="errMsgContainer">Error message container.</param>
		/// <exception cref="IncorrectDependenceStructureException"> 
		/// Thrown if there is an incorrect structure in dependencies cell in configuration sheet.
		/// </exception>
		private OptComplexDependence CreateOptComplexDep(ref LinkedList<string> structure, ref LinkedList<string> dependencies, 
			ref LinkedList<string> errorMessages, ref LinkedList<string> orderErrorMessage, ref Dictionary<string, Dependence> depContainer, 
			ref List<ActionAplication> actContainer, ref Dictionary<string, ErrorMessage> errMsgContainer)
		{
			//The list in which will be saved the dependencies.
			List<Dependence> listDependencies = new List<Dependence>();
			//The auxiliar variable.
			Dependence dep;
			//Obtains the elements of the structure.
			while (structure.Count > 0) {
				//Obtains the element.
				string e = structure.First.Value;
				//Checks the first char of the first element.
				if (e[0] == '[') {
					//Obtains the index of the [ char.
					int i = e.IndexOf ('[');
					//Erases the char '['.
					string s = e.Substring (i+1);
					structure.RemoveFirst ();
					structure.AddFirst (s);
					//Creates a OptComplexDependence.
					dep = (Dependence)CreateOptComplexDep (ref structure, ref dependencies, ref errorMessages, ref orderErrorMessage,
						ref depContainer, ref actContainer, ref errMsgContainer);
					//Adds into the list of dependencies.
					listDependencies.Add (dep);
					//Refresh the value.
					e = structure.First.Value;
					//Checks if is necessary to close the dependence.
					if (e[0] == ']') {
						//Checks if there is another close char.
						if (e.Length > 1) {
							//Erases the char ']'.
							s = e.Substring (1);
							structure.RemoveFirst ();
							structure.AddFirst (s);
						} else {
							//Erases the element of the structure.
							structure.RemoveFirst ();
						}
						break;
					} else if (e[0] == ')') {
						//Throw an exception.
						throw new IncorrectDependenceStructureException ();
					}
				} else if (e[0] == '(') {
					//Obtains the index of the ( char.
					int i = e.IndexOf ('(');
					//Erases the char '('.
					string s = e.Substring (i+1);
					structure.RemoveFirst ();
					structure.AddFirst (s);
					//Creates a SeqComplexDependence.
					dep = (Dependence)CreateSeqComplexDep (ref structure, ref dependencies, ref errorMessages, ref orderErrorMessage,
						ref depContainer, ref actContainer, ref errMsgContainer);
					//Adds into the list of dependencies.
					listDependencies.Add (dep);
					//Refresh the value.
					e = structure.First.Value;
					//Checks if is necessary to close the dependence.
					if (e[0] == ']') {
						//Checks if there is another close char.
						if (e.Length > 1) {
							//Erases the char ']'.
							s = e.Substring (1);
							structure.RemoveFirst ();
							structure.AddFirst (s);
						} else {
							//Erases the element of the structure.
							structure.RemoveFirst ();
						}
						break;
					} else if (e[0] == ')') {
						//Throw an exception.
						throw new IncorrectDependenceStructureException ();
					}
				} else {
					//Creates a SimpleDependence.
					SimpleDependence sDep;
					//Gets the first simple dependence.
					string sDepStr = dependencies.First.Value;
					//Gets the first error.
					string err = errorMessages.First.Value;
					//Obtains the error information.
					string[] auxErrorInformation = err.Split (new char[] { '-' });
					//Checks if the dependence exists in the container.
					var queryDep1 =
						from o in depContainer
						where o.Key == sDepStr || o.Key.StartsWith (sDepStr + "_") == true
						select o.Value;
					if (queryDep1.Count () > 0) {
						//Searchs the SimpleDependence in the container.
						var queryDep2 = 
							from o in queryDep1
							where o.DependenceError.Message.Message == (auxErrorInformation [1])
							select o;
						//Checks if there is a Dependence with the error message.
						if (queryDep2.Count () == 0) {
							//Gets a copy of the dependence.
							sDep = ((SimpleDependence)queryDep1.First ()).Clone ();
							//Creates an ErrorMessage.
							ErrorMessage errMsg;
							//Checks if the key exists.
							if (errMsgContainer.ContainsKey (auxErrorInformation [1])) {
								errMsg = errMsgContainer [auxErrorInformation [1]];
							} else {
								errMsg = new ErrorMessage (_genErrorMsgKey.ToString (), auxErrorInformation [1]);
								//Increase the generator.
								_genErrorMsgKey++;
								//Adds into the container.
								errMsgContainer.Add (auxErrorInformation [1], errMsg);
							}
							//Creates an Error.
							Error error;
							//Creates a new instance.
							int i = int.Parse(auxErrorInformation [0]);
							bool b = false;
							if (i == 1)
								b = true;
							error = new Error (_genErrorKey.ToString(), errMsg, b);
							//Increase the generator.
							_genErrorKey++;
							//Sets the error.
							sDep.DependenceError = error;
							//Sets the key.
							sDep.Key = sDepStr + "_" + sDep.DependenceError.Key;
							//Adds the SimpleDependence into the container.
							depContainer.Add (sDep.Key, sDep);
						} else {
							//Gets the Dependence.
							sDep = (SimpleDependence)queryDep2.First ();
						}
						//Adds into the dependencies list.
						listDependencies.Add (sDep);
					} else {
						//Creates an ErrorMessage.
						ErrorMessage errMsg;
						//Checks if the ErrorMessage does not exist in the container
						//If not, creates the ErrorMessage
						//Else, obtains the ErrorMessage from the container.
						if (!errMsgContainer.ContainsKey(auxErrorInformation[1])) {
							//Creates a new instance.
							errMsg = new ErrorMessage (_genErrorMsgKey.ToString(), auxErrorInformation [1]);
							//Increase the generator.
							_genErrorMsgKey++;
							//Adds into the container.
							errMsgContainer.Add (auxErrorInformation [1], errMsg);
						} else {
							//Searchs the error message in the container.
							var queryErrMsg = 
								from o in errMsgContainer
								where o.Key == (auxErrorInformation[1])
								select o.Value;
							errMsg = queryErrMsg.First ();
						}
						//Creates an Error.
						Error error;
						//Creates a new instance.
						int i = int.Parse(auxErrorInformation [0]);
						bool b = false;
						if (i == 1)
							b = true;
						error = new Error (_genErrorKey.ToString(), errMsg, b);
						//Increase the generator.
						_genErrorKey++;
						//Searchs the ActionAplication in the container.
						var queryAct = 
							from o in actContainer
							where o.Key == (sDepStr)
							select o;
						//Creates a new instance of SimpleDependence.
						sDep = new SimpleDependence (sDepStr, queryAct.First (), error);
						//Adds the SimpleDependence into the container.
						depContainer.Add (sDep.Key, sDep);
						//Adds into the dependencies list.
						listDependencies.Add (sDep);
					}
					//Checks if the element is the last element into the list.
					if (e == structure.Last() && !e.Contains ("]")) {
						//Throw an exception if there is not the end char.
						throw new IncorrectDependenceStructureException ();
					} else if (e == structure.Last() && e.Contains ("]")) {
						//Obtains the index of the ] char.
						int i = e.IndexOf (']');
						//Checks if the index is the last position.
						if (i == (e.Length - 1)) {
							//Checks if the element contains the ')' char. If it is contained, this means
							//that this char is placed before ']' char.
							if (e.Contains(")")) {
								//Throw an exception.
								throw new IncorrectDependenceStructureException ();
							}
							//Erases the element of the structure.
							structure.RemoveFirst ();
							//Erases the element of the dependencies.
							dependencies.RemoveFirst ();
							//Erases the element of the structure.
							errorMessages.RemoveFirst ();
							break;
						} else {
							//Erases all elements of the string from the beginning to the char ']', including this.
							string s = e.Substring (i+1);
							structure.RemoveFirst ();
							structure.AddFirst (s);
							//Erases the element of the dependencies.
							dependencies.RemoveFirst ();
							//Erases the element of the structure.
							errorMessages.RemoveFirst ();
							break;
						}
					} else if (e.Contains ("]")) {
						//Obtains the index of the ] char.
						int i = e.IndexOf (']');
						//Checks if the index is the last position.
						if (i == (e.Length - 1)) {
							//Checks if the element contains the ')' char. If it is contained, this means
							//that this char is placed before ']' char.
							if (e.Contains(")")) {
								//Throw an exception.
								throw new IncorrectDependenceStructureException ();
							}
							//Erases the element of the structure.
							structure.RemoveFirst ();
							//Erases the element of the dependencies.
							dependencies.RemoveFirst ();
							//Erases the element of the structure.
							errorMessages.RemoveFirst ();
							break;
						} else {
							//Erases all elements of the string from the beginning to the char ']', including this.
							string s = e.Substring (i+1);
							structure.RemoveFirst ();
							structure.AddFirst (s);
							//Erases the element of the dependencies.
							dependencies.RemoveFirst ();
							//Erases the element of the structure.
							errorMessages.RemoveFirst ();
							break;
						}
					} else {
						//Checks if there is a sequencial close char before closing this dependence.
						if (e.Contains (")")) {
							//Throw an exception.
							throw new IncorrectDependenceStructureException ();
						}
						//Erases the element of the structure.
						structure.RemoveFirst ();
						//Erases the element of the dependencies.
						dependencies.RemoveFirst ();
						//Erases the element of the structure.
						errorMessages.RemoveFirst ();
					}
				}
			}
			//Creates the key of the dependence.
			string key = "[";
			foreach (Dependence d in listDependencies)
				key += d.Key;
			key += "]";
			//Creates the variable to return.
			OptComplexDependence depToReturn;
			//Checks if the dependence exists into the container.
			if (depContainer.ContainsKey(key)) {
				//Searchs the dependence in the container.
				var query = 
					from o in depContainer
					where o.Key == key
					select o.Value;
				depToReturn = (OptComplexDependence)query.First ();
			} else {
				//Creates a new instances of OptComplexDependence.
				depToReturn = new OptComplexDependence (key, listDependencies, null);
				//Adds into the dependencies container.
				depContainer.Add(depToReturn.Key, depToReturn);
			}
			//Returns the dependence.
			return depToReturn;
		}

		/// <summary>
		/// Creates the dependence.
		/// </summary>
		/// <returns>The dependence.</returns>
		/// <param name="structure">Structure.</param>
		/// <param name="dependencies">Dependencies.</param>
		/// <param name="errorMessages">Error messages.</param>
		/// <param name="orderErrorMessage">Order error message.</param>
		/// <param name="depContainer">Dep container.</param>
		/// <param name="actContainer">Act container.</param>
		/// <param name="errMsgContainer">Error message container.</param>
		private ComplexDependence CreateDependence(ref LinkedList<string> structure, ref LinkedList<string> dependencies, 
			ref LinkedList<string> errorMessages, ref LinkedList<string> orderErrorMessage, ref Dictionary<string, Dependence> depContainer, 
			ref List<ActionAplication> actContainer, ref Dictionary<string, ErrorMessage> errMsgContainer)
		{
			//The variable which will be returned.
			ComplexDependence dep;
			//Obtains the first element of the structure.
			string e = structure.First.Value;
			//Checks the first char of the first element.
			if (e[0] == '[') {
				//Obtains the index of the [ char.
				int i = e.IndexOf ('[');
				//Erases the char '['.
				string s = e.Substring (i+1);
				structure.RemoveFirst ();
				structure.AddFirst (s);
				//Creates a OptComplexDependence.
				dep = (ComplexDependence)CreateOptComplexDep (ref structure, ref dependencies, ref errorMessages, ref orderErrorMessage, ref depContainer,
					ref actContainer, ref errMsgContainer);
				//Checks if all dependencies are closed.
				if (structure.Count > 0) {
					//Throw an exception.
					throw new IncorrectDependenceStructureException ();
				}
			} else if (e[0] == '(') {
				//Obtains the index of the [ char.
				int i = e.IndexOf ('(');
				//Erases the char '('.
				string s = e.Substring (i+1);
				structure.RemoveFirst ();
				structure.AddFirst (s);
				//Creates a SeqComplexDependence.
				dep = (ComplexDependence)CreateSeqComplexDep (ref structure, ref dependencies, ref errorMessages, ref orderErrorMessage, ref depContainer,
					ref actContainer, ref errMsgContainer);
				//Checks if all dependencies are closed.
				if (structure.Count > 0) {
					//Throw an exception.
					throw new IncorrectDependenceStructureException ();
				}
			} else {
				//Throw an exception.
				throw new IncorrectDependenceStructureException ();
			}
			//Value returned.
			return dep;
		}

		/// <summary>
		/// Creates the domain.
		/// </summary>
		/// <returns>The domain.</returns>
		public DomainActions CreateDomain (string domainKey) 
		{
			//Initialize the generators of keys.
			_genTutorMsgKey = 0;
			_genErrorMsgKey = 0;
			_genErrorKey = 0;
			//Creates temporary containers.
			Dictionary<string, Dependence> depContainer = new Dictionary<string, Dependence> ();
			Dictionary<string, ErrorMessage> errMsgContainer = new Dictionary<string, ErrorMessage> ();
			//Obtains the actions information.
			List<object[]> actionsInformation = ACTION_ACCESS.GetActions (domainKey);
			//Creates the ActionAplication list which will be used to create the DomainAction instance.
			List<ActionAplication> actions = new List<ActionAplication> ();
			//Auxiliar Dictionary in which it will be saved the ActionAplication key as the key, 
			//and the string array with the possible next actions keys.
			Dictionary<string, string[]> possibleNextActionsCont = new Dictionary<string, string[]> ();
			//Auxiliar Dictionary in which it will be saved the ActionAplication key as the key, 
			//and the string array with the incompatibilities keys.
			Dictionary<string, string[]> incompCont = new Dictionary<string, string[]> ();
			//Auxiliar Dictionary in which it will be saved the ActionAplication key as the key, 
			//and the string array with the incompatibilities errors.
			Dictionary<string, string[]> incompErrorCont = new Dictionary<string, string[]> ();
			//Creates all ActionAplication using action information list.
			foreach (object[] o in actionsInformation) {
				//Obtains the phase.
				int phase = int.Parse (o [0].ToString ());
				//Obtains the key.
				string key = o [1].ToString ();
				//Obtains the name.
				string name = o [2].ToString ();
				//Obtains the description.
				string description = o [2].ToString ();
				//Obtains the okMessage.
				//Auxiliar
				string s = o [3].ToString ();
				string[] ss = new string[] {};
				string okMessage = "";
				bool showOkMessage = false;
				int i = 0;
				if (s != "") {
					ss = s.Split (new char[] { '-' });
					okMessage = ss [1];
					//Obtains the showOkMessage value.
					i = int.Parse (ss [0]);;
					if (i == 1)
						showOkMessage = true;
				}
				//Obtains the possible next actions.
				s = o [4].ToString ();
				ss = new string[] {};
				if (s != "")
					ss = s.Split (new char[] { '-' });
				//Saves into the contaizner to use it afterwards.
				possibleNextActionsCont.Add (key, ss);
				//Obtains the tutor message.
				string tMsg = o [5].ToString ();
				//Creates a TutorMessage
				TutorMessage tutorMsg;
				//Checks if the tutor message is empty.
				if (tMsg != "") {
					//Adds a value.
					tutorMsg = new TutorMessage (_genTutorMsgKey.ToString (), tMsg);
					//Increase the generator.
					_genTutorMsgKey++;
				} else {
					tutorMsg = null;
				}
				//Obtains the dependencies.
				s = o [6].ToString ();
				//Creates the ComplexDependence.
				ComplexDependence dependence;
				//Checks if the dependencies string is empty.
				if (s != "") {
					//Splits the dependencies.
					string[] ssDepSt = s.Split (new char[] { '-' });
					string[] ssDeps = s.Split (new char[] { '-', '[', ']', '(', ')' });
					//Obtains the dependence errors.
					s = o [7].ToString ();
					string[] ssErr = s.Split (new char[] { '\\' });
					//Obtains the order dependence errors.
					s = o [8].ToString ();
					string[] sOrdErr = s.Split (new char[] { '\\' });
					//Creates the multiple LinkedList which will be passed as parameters.
					LinkedList<string> structure = new LinkedList<string> (ssDepSt);
					LinkedList<string> dependencies = new LinkedList<string> ();
					foreach (string d in ssDeps)
						if (d != "")
							dependencies.AddLast(d);

					LinkedList<string> errorMessages = new LinkedList<string> (ssErr);
					LinkedList<string> orderErrorMessage = new LinkedList<string> (sOrdErr);
					//Calls the function to create the dependencies.
					dependence = CreateDependence (ref structure, ref dependencies, ref errorMessages,
						ref orderErrorMessage, ref depContainer, ref actions, ref errMsgContainer);
				} else {
					dependence = null;
				}
				//Obtains the incompatibilities.
				s = o [9].ToString ();
				//Checks if the dependencies string is empty.
				if (s != "") {
					//Splits the incompatibilities.
					string[] ssInc = s.Split (new char[] { '-' });
					//Obtains the incompatibility errors.
					s = o [10].ToString ();
					string[] ssIncError = s.Split (new char[] { '\\' });
					//Saves into the container to use it afterwards.
					incompCont.Add (key, ssInc);
					incompErrorCont.Add (key, ssIncError);
				}
				//Creates the list in which will be saved the incompatibilities.
				List<Incompatibility> incompatibilities = null;
				//Creates lock and unlock booleans.
				bool lockObj = false;
				bool unlockObj = false;
				//Obtains the lock value.
				s = o [11].ToString ();
				//Checks if the value is empty string.
				if (s != "") {
					i = int.Parse (s);
					switch (i) {
					case 0:
						unlockObj = true;
						break;
					case 1:
						lockObj = true;
						break;
					default:
						break;
					}
				}
				//Obtains MaxTime and MinTime values.
				int maxTime = int.Parse (o [12].ToString ()==""?"0":o [12].ToString ());
				int minTime = int.Parse (o [13].ToString ()==""?"0":o [13].ToString ());
				//Creates an Error.
				Error maxTimeError = null;
				//Checks if MaxTime is greater than zero.
				if (maxTime > 0) {
					//Obtains MaxTimeError.
					s = o [14].ToString ();
					//Checks if the string is empty.
					if (s != "") {
						//Splits the string obtained.
						ss = s.Split (new char[] { '-' });
						//Creates an ErrorMessage.
						ErrorMessage errMsg;
						//Checks if the ErrorMessage does not exist in the container
						//If not, creates the ErrorMessage
						//Else, obtains the ErrorMessage from the container.
						if (!errMsgContainer.ContainsKey (ss [1])) {
							//Creates a new instance.
							errMsg = new ErrorMessage (_genErrorMsgKey.ToString (), ss [1]);
							//Increase the generator.
							_genErrorMsgKey++;
							//Adds into the container.
							errMsgContainer.Add (ss [1], errMsg);
						} else {
							//Searchs the error message in the container.
							var query = 
								from q in errMsgContainer
								where q.Key == (ss [1])
								select q.Value;
							errMsg = query.First ();
						}
						//Creates a new instance.
						i = int.Parse (ss [0]);
						bool b = false;
						if (i == 1)
							b = true;
						maxTimeError = new Error (_genErrorKey.ToString (), errMsg, b);
						//Increase the generator.
						_genErrorKey++;
					} else {
						maxTimeError = null;
					}
				}
				//Creates an Error.
				Error minTimeError = null;
				//Checks if MinTime is greater than zero.
				if (minTime > 0) {
					//Obtains MinTimeError.
					s = o [15].ToString ();
					//Checks if the string is empty.
					if (s != "") {
						//Splits the string obtained.
						ss = s.Split (new char[] { '-' });
						//Creates an ErrorMessage.
						ErrorMessage errMsg;
						//Checks if the ErrorMessage does not exist in the container
						//If not, creates the ErrorMessage
						//Else, obtains the ErrorMessage from the container.
						if (!errMsgContainer.ContainsKey (ss [1])) {
							//Creates a new instance.
							errMsg = new ErrorMessage (_genErrorMsgKey.ToString (), ss [1]);
							//Increase the generator.
							_genErrorMsgKey++;
							//Adds into the container.
							errMsgContainer.Add (ss [1], errMsg);
						} else {
							//Searchs the error message in the container.
							var query = 
								from q in errMsgContainer
								where q.Key == (ss [1])
								select q.Value;
							errMsg = query.First ();
						}
						//Creates a new instance.
						i = int.Parse (ss [0]);
						bool b = false;
						if (i == 1)
							b = true;
						minTimeError = new Error (_genErrorKey.ToString (), errMsg, b);
						//Increase the generator.
						_genErrorKey++;
					} else {
						minTimeError = null;
					}
				}
				//Creates isRepetitive boolean.
				bool isRepetitive = false;
				//Obtains the isRepetitive value.
				s = o [16].ToString ();
				i = int.Parse (s);
				if (i != 1)
					isRepetitive = true;
				//Creates validatePhaseErrors boolean.
				bool validatePhaseErrors = false;
				//Obtains the validatePhaseErrors value.
				s = o [17].ToString ();
				if (s != "")
					validatePhaseErrors = true;
				//Creates initPhase boolean.
				bool initPhase = false;
				//Obtains the initPhase value.
				s = o [18].ToString ();
				if (s != "")
					initPhase = true;
				//Creates the list in which will be saved the objects names.
				List<string> objectName;
				//Obtains the object names list.
				s = o [19].ToString ();
				ss = s.Split (new char[] { '-' });
				objectName = new List<string> (ss);

				//Obtains in plan booblean.
				s = o [20].ToString ();
				bool isInPlan = true;
				if (s == "0")
					isInPlan = false;

				//Creates the ActionAplication.
				ActionAplication action;
				//Checks if minTime is greater than zero.
				if (minTime > 0) {
					action = new ActionAplication (key, phase, name, description, objectName,
						lockObj, unlockObj, isRepetitive, initPhase, validatePhaseErrors, dependence, incompatibilities, 
						false, isInPlan, null, okMessage, showOkMessage, null, tutorMsg, minTime, minTimeError, maxTime, maxTimeError);

				} else if (maxTime > 0) {
					action = new ActionAplication (key, phase, name, description, objectName,
						lockObj, unlockObj, isRepetitive, initPhase, validatePhaseErrors, dependence, incompatibilities, 
						false, isInPlan, null, okMessage, showOkMessage, null, tutorMsg, minTime, minTimeError, maxTime, maxTimeError);
				}else {
					action = new ActionAplication (key, phase, name, description, objectName,
						lockObj, unlockObj, isRepetitive, initPhase, validatePhaseErrors, dependence, incompatibilities,
						false, isInPlan, null, okMessage, showOkMessage, null,  tutorMsg);
				}
				//Adds into the list.
				actions.Add (action);
			}

			//Completes the possibleNextActions list and incompatibilities for each ActionAplication.
			foreach (ActionAplication act in actions) {
				//Searchs the possibleNextActions list in the container.
				//The container key is the action key.
				var queryNextAct = 
					from o in possibleNextActionsCont
					where o.Key == act.Key
					select o.Value;
				string[] pblNextAct = queryNextAct.First ();
				//Creates the possibleNextActions list.
				List<ActionAplication> possibleNextActions = new List<ActionAplication> ();
				//Creates the list in which will be saved the incompatibilities.
				List<Incompatibility> incompatibilities = new List<Incompatibility> ();
				//Searchs the ActionAplications in the list and adds into the possibleNextActions list.
				foreach (string s in pblNextAct) {
					var q =
						from o in actions
						where o.Key == s
						select o;
					possibleNextActions.Add (q.First ());
				}
				//Obtains the value if the dictionary contains the key.
				if (incompCont.ContainsKey(act.Key)) {
					//Gets the incompatibilities and their associated errors.
					string[] ssInc = incompCont [act.Key];
					string[] ssIncErr = incompErrorCont [act.Key];
					//Creates an index.
					int i = 0;
					foreach (string inc in ssInc) {
						var qInc =
							from o in actions
							where o.Key == inc
							select o;
						//Splits the string obtained.
						string[] ss = ssIncErr [i].Split (new char[] { '-' });
						//Creates an ErrorMessage.
						ErrorMessage errMsg;
						//Checks if the ErrorMessage does not exist in the container
						//If not, creates the ErrorMessage
						//Else, obtains the ErrorMessage from the container.
						if (!errMsgContainer.ContainsKey (ss [1])) {
							//Creates a new instance.
							errMsg = new ErrorMessage (_genErrorMsgKey.ToString (), ss [1]);
							//Increase the generator.
							_genErrorMsgKey++;
							//Adds into the container.
							errMsgContainer.Add (ss [1], errMsg);
						} else {
							//Searchs the error message in the container.
							var query = 
								from q in errMsgContainer
								where q.Key == (ss [1])
								select q.Value;
							errMsg = query.First ();
						}
						//Creates a new instance.
						int j = int.Parse (ss [0]);
						bool b = false;
						if (j == 1)
							b = true;
						Error incError = new Error (_genErrorKey.ToString (), errMsg, b);
						//Increase the generator.
						_genErrorKey++;
						//Creates a new Incompatibility and adds into the list.
						incompatibilities.Add (new Incompatibility (inc, qInc.First (), incError));
						//Increases the index.
						i++;
					}
				} else {
					incompatibilities = null;
				}
				//Sets the list of possible next actions.
				act.PossibleNextActions = possibleNextActions;
				//Sets the list of incompatibilities.
				act.Incompatibilities = incompatibilities;
			}

				

			//Creates the DomainActions instance which will be returned.
			DomainActions domain = new DomainActions (domainKey, domainKey, actions, 0);

			//Adds the DomainActions into the ontology.
			ONTOLOGY.AddDomainIntoOnto (domain);
			//Adds the ActionAplication into the ontology.
			foreach (ActionAplication act in actions)
				ONTOLOGY.AddActionIntoOnto (act, domain);

			//Saves the ontology.
			ONTOLOGY.SaveOnto ();

			//Returns the DomainActions.
			return domain;
		}
	}
}