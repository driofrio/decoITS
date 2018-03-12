using System;
using Its.Utils.Math;
using Its.TutoringModule.StudentBehaviorPredictor.Exceptions;
using Its.StudentModule.ObjectModel;
using Its.ExpertModule.ObjectModel;
using Its.TutoringModule.TutoringCoordinator.ReactiveTutor.ObjectModel;
using System.Collections.Generic;
using Its.Utils.Math.Exceptions;

namespace Its.TutoringModule.StudentBehaviorPredictor.ObjectModel
{
	public class StudentActionsModel : Automaton<State,Event>
	{
		/// <summary>
		/// The init state of the automaton.
		/// </summary>
		private Node<State,Event> _initState;

		public StudentActionsModel (int numberOfStudents):base()
		{
			_initState = new Node<State, Event> ("initState", "", new CorrectState (Area.CorrectFlow, null, false, numberOfStudents));
			this.AddNode (_initState);
		}

		/// <summary>
		/// Gets the init state of the automaton.
		/// </summary>
		/// <value>The state of the init.</value>
		public Node<State,Event> InitState{
			get{
				return _initState;
			}
		}

		/// <summary>
		/// Adds a new state or increment its frequency if it already exists. Creates a state of a specific type depending on the type of the log.
		/// </summary>
		/// <returns>The new state or increment state.</returns>
		/// <param name="log">Log.</param>
		/// <param name="area">Area.</param>
		private Node<State,Event> AddState(LogEntry log, Area area){
			State state=null;
			Node<State,Event> newState = null;

			if (log.GetType ().BaseType == typeof(ActionLog)) {
				state = new CorrectState (area, log.Action, log.GetType () == typeof(CorrectiveActionLog));
				newState = new Node<State,Event> (log.Action.Key, log.Action.Description, state);
			} else if (log.GetType () == typeof(DepErrorLog)) {
				Dependence fail = ((DepErrorLog)log).FailedDependence;
				state = new DependenceErrorState (area, fail);
				newState = new Node<State,Event> (log.Action.Key + "_" + fail.Key, log.Action.Description, state);
			} else if (log.GetType () == typeof(IncompErrorLog)) {
				Incompatibility fail = ((IncompErrorLog)log).FailedIncompatibility;
				state = new IncompatibilityErrorState (area, fail);
				newState = new Node<State,Event> (log.Action.Key + "_" + fail.Key, log.Action.Description, state);
			} else if (log.GetType ().BaseType == typeof(TimeErrorLog)) {
				Error fail = (log.GetType () == typeof(MinTimeErrorLog)) ? log.Action.MinTimeError : log.Action.MaxTimeError;
				state = new TimeErrorState (area, ((TimeErrorLog)log).Time, fail);
				newState = new Node<State,Event> (log.Action.Key + "_" + fail.Key, log.Action.Description, state);
			} else if (log.GetType () == typeof(WorldErrorLog)) {
				Error fail = ((WorldErrorLog)log).ErrorAssociated;
				state = new WorldErrorState (area, fail, ((WorldErrorLog)log).Type);
				newState = new Node<State,Event> (log.Action.Key + "_" + fail.Key, log.Action.Description, state);
			} else if (log.GetType () == typeof(OtherErrorLog)) {
				Error fail = ((OtherErrorLog)log).ErrorAssociated;
				state = new OtherErrorState (area, fail);
				newState = new Node<State,Event> (log.Action.Key + "_" + fail.Key, log.Action.Description, state);
			}
				
			Node<State,Event> tempState = null;
			if (this.TryGetNode (newState.Key, out tempState)) {
				tempState.Specification.IncrementFrequency ();
				newState = tempState;
			}else
				this.AddNode (newState);
			return newState;
		}

		/// <summary>
		/// Removes a state or decrement its frequency if it already exists. First creates a state key depending on the type of the log, then decrements the frequency of the state and if this frequency goes to zero removes it.
		/// </summary>
		/// <returns>The state.</returns>
		/// <param name="log">Log.</param>
		/// <param name="area">Area.</param>
		private Node<State,Event> RemoveState(LogEntry log, Area area){
			string stateKey = GetStateKey (log);

			Node<State,Event> tempState = null;
			if (this.TryGetNode (stateKey, out tempState)) {
				if (tempState.Specification.DecrementFrequency ())
					this.RemoveNode (stateKey);
			} else
				throw new StateNotFoundException (stateKey);
			return tempState;
		}

		/// <summary>
		/// Determines whether the automaton contains a state with a specific key.
		/// </summary>
		/// <returns><c>true</c>, if the state exists, <c>false</c> otherwise.</returns>
		/// <param name="stateKey">State key.</param>
		public bool ContainsState(string stateKey){
			return this.Nodes.ContainsKey (stateKey);
		}

		/// <summary>
		/// Finds a state that fits with a log.
		/// </summary>
		/// <returns>The state.</returns>
		/// <param name="log">Log.</param>
		public Node<State,Event> FindState(LogEntry log){
			Node<State,Event> state = null;
			string stateKey = GetStateKey (log);
			this.Nodes.TryGetValue (stateKey, out state);
			return state;
		}

		/// <summary>
		/// Gets a state key depending on de log type.
		/// </summary>
		/// <returns>The state key.</returns>
		/// <param name="log">Log.</param>
		private string GetStateKey (LogEntry log)
		{
			string stateKey = "";
			if (log.GetType ().BaseType == typeof(ActionLog)) {
				stateKey = log.Action.Key;
			} else if (log.GetType () == typeof(DepErrorLog)) {
				Dependence fail = ((DepErrorLog)log).FailedDependence;
				stateKey = log.Action.Key + "_" + fail.Key;
			} else if (log.GetType () == typeof(IncompErrorLog)) {
				Incompatibility fail = ((IncompErrorLog)log).FailedIncompatibility;
				stateKey = log.Action.Key + "_" + fail.Key;
			} else if (log.GetType ().BaseType == typeof(TimeErrorLog)) {
				Error fail = (log.GetType () == typeof(MinTimeErrorLog)) ? log.Action.MinTimeError : log.Action.MaxTimeError;
				stateKey = log.Action.Key + "_" + fail.Key;
			} else if (log.GetType () == typeof(WorldErrorLog)) {
				Error fail = ((WorldErrorLog)log).ErrorAssociated;
				stateKey = log.Action.Key + "_" + fail.Key;
			} else if (log.GetType () == typeof(OtherErrorLog)) {
				Error fail = ((OtherErrorLog)log).ErrorAssociated;
				stateKey = log.Action.Key + "_" + fail.Key;
			}
			return stateKey;
		}

		/// <summary>
		/// Adds a normal event.
		/// </summary>
		/// <param name="stateOut">State out.</param>
		/// <param name="stateIn">State in.</param>
		/// <param name="actionExecuted">Action executed.</param>
		private void AddNormalEvent(Node<State,Event> stateOut, Node<State,Event> stateIn, ActionAplication actionExecuted){
			Event newEvent = new NormalEvent (actionExecuted);

			Arc<State,Event> newArc = null;

			if (this.TryGetEvent (stateOut.Key, stateIn.Key, out newArc)) {
				((NormalEvent)newArc.Specification).IncrementFrequency ();
			} else {
				newArc = new Arc<State,Event> (stateOut, stateIn, newEvent);
				this.AddArc (newArc);
			}
			
			stateIn.AddInArc (newArc);
			stateOut.AddOutArc (newArc);
		}

		/// <summary>
		/// Removes a normal event.
		/// </summary>
		/// <param name="stateOut">State out.</param>
		/// <param name="stateIn">State in.</param>
		/// <param name="actionExecuted">Action executed.</param>
		private void RemoveNormalEvent(Node<State,Event> stateOut, Node<State,Event> stateIn, ActionAplication actionExecuted){
			Arc<State,Event> newArc = null;
			if (this.TryGetEvent (stateOut.Key, stateIn.Key, out newArc)) {
				if (((NormalEvent)newArc.Specification).DecrementFrequency ())
					this.RemoveArc (stateOut.Key, stateIn.Key);
			} else
				throw new EventNotFoundException (stateOut.Key, stateIn.Key);

			stateIn.AddInArc (newArc);
			stateOut.AddOutArc (newArc);
		}

		/// <summary>
		/// Adds a vector event.
		/// </summary>
		/// <param name="stateOut">State out.</param>
		/// <param name="stateIn">State in.</param>
		/// <param name="iterationNumber">Iteration number.</param>
		/// <param name="actionExecuted">Action executed.</param>
		private void AddVectorEvent(Node<State,Event> stateOut, Node<State,Event> stateIn, int iterationNumber, ActionAplication actionExecuted){
			Event newEvent = new VectorEvent (actionExecuted);

			Arc<State,Event> newArc = null;
			if (this.TryGetEvent (stateOut.Key, stateIn.Key, out newArc)) {
				((VectorEvent)newArc.Specification).IncrementFrequency (iterationNumber);
			} else {
				newArc = new Arc<State,Event> (stateOut, stateIn, newEvent);
				((VectorEvent)newArc.Specification).IncrementFrequency (iterationNumber);
				this.AddArc (newArc);
			}

			stateIn.AddInArc (newArc);
			stateOut.AddOutArc (newArc);
		}
	
		/// <summary>
		/// Removes a vector event.
		/// </summary>
		/// <param name="stateOut">State out.</param>
		/// <param name="stateIn">State in.</param>
		/// <param name="iterationNumber">Iteration number.</param>
		/// <param name="actionExecuted">Action executed.</param>
		private void RemoveVectorEvent(Node<State,Event> stateOut, Node<State,Event> stateIn, int iterationNumber, ActionAplication actionExecuted){
			Arc<State,Event> newArc = null;
			if (this.TryGetEvent (stateOut.Key, stateIn.Key, out newArc)) {
				if (((VectorEvent)newArc.Specification).DecrementFrequency (iterationNumber))
					this.RemoveArc (stateOut.Key, stateIn.Key);
			} else
				throw new EventNotFoundException (stateOut.Key, stateIn.Key);

			stateIn.AddInArc (newArc);
			stateOut.AddOutArc (newArc);
		}

		/// <summary>
		/// Determines whether the automaton contains an event with a specific key.
		/// </summary>
		/// <returns><c>true</c>, if transition was containsed, <c>false</c> otherwise.</returns>
		/// <param name="stateKey">State key.</param>
		public bool ContainsEvent(ArcKey eventKey){
			return this.Arcs.ContainsKey (eventKey.ToString());
		}

		/// <summary>
		/// Tries to get a state.
		/// </summary>
		/// <returns><c>true</c>, if get state was tryed, <c>false</c> otherwise.</returns>
		/// <param name="stateKey">State key.</param>
		/// <param name="state">State.</param>
		public bool TryGetState(string stateKey, out Node<State,Event> state){
			return this.TryGetNode (stateKey, out state);
		}

		/// <summary>
		/// Tries to get an  event.
		/// </summary>
		/// <returns><c>true</c>, if get event was tryed, <c>false</c> otherwise.</returns>
		/// <param name="nodeKey">Node key.</param>
		/// <param name="evnt">Evnt.</param>
		public bool TryGetEvent(string stateKeyOut, string stateKeyIn, out Arc<State,Event> evnt){
			return this.TryGetArc (stateKeyOut, stateKeyIn, out evnt);
		}

		/// <summary>
		/// Tries to get an event.
		/// </summary>
		/// <returns><c>true</c>, if get event was tryed, <c>false</c> otherwise.</returns>
		/// <param name="arcKey">Arc key.</param>
		/// <param name="evnt">Evnt.</param>
		public bool TryGetEvent(ArcKey arcKey, out Arc<State,Event> evnt){
			return this.TryGetArc (arcKey, out evnt);
		}

		/// <summary>
		/// Gets a state.
		/// </summary>
		/// <returns>The state.</returns>
		/// <param name="stateKey">State key.</param>
		public Node<State,Event> GetState(string stateKey){
			Node<State,Event> state=null;
			if(this.TryGetNode(stateKey, out state)){
				return state;
			}else{
				throw new Exceptions.StateNotFoundException(stateKey);
			}
		}

		/// <summary>
		/// Gets an event.
		/// </summary>
		/// <returns>The event.</returns>
		/// <param name="stateKeyOut">State key out.</param>
		/// <param name="stateKeyIn">State key in.</param>
		public Arc<State,Event> GetEvent(string stateKeyOut, string stateKeyIn){
			Arc<State,Event> ev=null;
			if(this.TryGetArc(stateKeyOut, stateKeyIn, out ev)){
				return ev;
			}else{
				throw new Exceptions.EventNotFoundException (stateKeyOut, stateKeyIn);
			}
		}

		/// <summary>
		/// Gets the events that are produced by the execution of an action or an action attempt,
		/// could be more than one event due that action could be executed in different  phases.
		/// </summary>
		/// <returns>The events.</returns>
		/// <param name="actionName">Action name.</param>
		public List<Arc<State,Event>> GetEventsByActionExecuted(string actionName){
			List<Arc<State,Event>> events = new List<Arc<State, Event>> ();
			foreach (Arc<State,Event> ev in this.Arcs.Values) {
				if (ev.Specification.ActionExecuted.Name == actionName)
					events.Add (ev);
			}
			return events;
		}

		/// <summary>
		/// Expands the automaton.
		/// </summary>
		/// <returns>The automaton.</returns>
		/// <param name="log">Log.</param>
		/// <param name="lastState">Last state.</param>
		/// <param name="iterationNumber">Iteration number (in case of an repetitive event).</param>
		public Node<State,Event> ExpandAutomaton(LogEntry log, Node<State,Event> previousState, int iterationNumber=0){
			Node<State,Event> newState=null;
			Area area=previousState.Specification.Area;
			if (log.GetType ().BaseType == typeof(ActionLog)) {
				if (area == Area.CorrectFlow || area == Area.IrrelevantErrors) {
					area = Area.CorrectFlow;
				} else {
					if (previousState.Specification.GetType() == typeof(CorrectState)) {
						if (((CorrectState)previousState.Specification).IsCorrective)
							area = Area.CorrectFlow;
						else
							area = Area.RelevantErrors;
					}else
						area = Area.RelevantErrors;
				}

			}else if (log.GetType ().BaseType == typeof(ErrorLog)) {
				if (log.Error.IsBlock)
					area = Area.IrrelevantErrors;
				else
					area = Area.RelevantErrors;
			}
			newState = this.AddState (log, area);
			if (previousState.Specification.Area != Area.IrrelevantErrors)
				this.AddNormalEvent (previousState, newState, log.Action);
			else {
				if (newState != previousState) {
					this.AddVectorEvent (previousState, newState, iterationNumber, log.Action);
				}
			}

			return newState;
		}

		/// <summary>
		/// Eliminates a log from the automaton.
		/// </summary>
		/// <returns>The automaton.</returns>
		/// <param name="log">Log.</param>
		/// <param name="previousState">Previous state.</param>
		/// <param name="iterationNumber">Iteration number.</param>
		public Node<State,Event> ReduceAutomaton(LogEntry log, Node<State,Event> previousState, int iterationNumber=0){
			Node<State,Event> stateRemoved=null;
			Area area=previousState.Specification.Area;
			if (log.GetType ().BaseType == typeof(ActionLog)) {
				if (area == Area.CorrectFlow || area == Area.IrrelevantErrors) {
					area = Area.CorrectFlow;
				} else {
					if (previousState.Specification.GetType() == typeof(CorrectState)) {
						if (((CorrectState)previousState.Specification).IsCorrective)
							area = Area.CorrectFlow;
						else
							area = Area.RelevantErrors;
					}else
						area = Area.RelevantErrors;
				}
			}else if (log.GetType ().BaseType == typeof(ErrorLog)) {
				if (log.Error.IsBlock)
					area = Area.IrrelevantErrors;
				else
					area = Area.RelevantErrors;
			}
			stateRemoved = this.RemoveState (log, area);

			if (previousState.Specification.Area != Area.IrrelevantErrors)
				this.RemoveNormalEvent (previousState, stateRemoved, log.Action);
			else {
				if (stateRemoved != previousState) {
					this.RemoveVectorEvent (previousState, stateRemoved, iterationNumber, log.Action);
				}
			}

			return stateRemoved;
		}

		/// <summary>
		/// Gets the next most probable event.
		/// </summary>
		/// <returns>The next most probable event.</returns>
		/// <param name="lastLog">Last log.</param>
		public Arc<State,Event> GetNextProbableEvent(LogEntry lastLog){
			Node<State,Event> lastState = FindState (lastLog);
			long maxFrequency = 0;
			Arc<State,Event> mostFrequentEvent = null;
			foreach (Arc<State,Event> evt in lastState.OutArcs.Values) {
				if (evt.Specification.GetType () == typeof(NormalEvent)) {
					NormalEvent tmpEvt = (NormalEvent)evt.Specification;
					if (tmpEvt.Frequency > maxFrequency) {
						maxFrequency = tmpEvt.Frequency;
						mostFrequentEvent = evt;
					}
				} else {
					VectorEvent tmpEvt = (VectorEvent)evt.Specification;
					foreach(long freq in tmpEvt.Frequency){
						if (freq > maxFrequency) {
							maxFrequency = freq;
							mostFrequentEvent = evt;
						}
					}
				}
			}
			return mostFrequentEvent;
		}
	}
}

