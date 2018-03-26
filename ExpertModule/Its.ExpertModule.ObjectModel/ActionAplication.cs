using System;
using System.Collections.Generic;
using Its.TutoringModule.ReactiveTutor.ObjectModel;
using Its.Utils.StringUtils;

namespace Its.ExpertModule.ObjectModel
{
	/// <author>
	/// Diego Dotor Jara.
	/// </author>
	/// <summary>
	///  This class will contain the Action atributes.
	/// </summary>
	public class ActionAplication
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
		/// What is its phase.
		/// </summary>
		private int _phase;
		/// <summary>
		/// Gets the phase.
		/// </summary>
		/// <value>The phase.</value>
		public int Phase {
			get {
				return _phase;
			}
		}
		/// <summary>
		/// Action name.
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
		/// Action description.
		/// </summary>
		private string _description;
		/// <summary>
		/// Gets the description.
		/// </summary>
		/// <value>The description.</value>
		public string Description {
			get {
				return _description;
			}
		}
		/// <summary>
		/// String with the object name associated with this ActionAplication.
		/// </summary>
		private List<string> _objectName;
		/// <summary>
		/// Gets the name of the object.
		/// </summary>
		/// <value>The name of the object.</value>
		public List<string> ObjectName {
			get {
				return _objectName;
			}
		}
		/// <summary>
		/// Boolean that denotes if this action lock the associated object.
		/// </summary>
		private bool _lockObj;
		/// <summary>
		/// Gets a value indicating whether this <see cref="Its.Expert.ObjectModel.ActionAplication"/> lock object.
		/// </summary>
		/// <value><c>true</c> if lock object; otherwise, <c>false</c>.</value>
		public bool LockObj {
			get {
				return _lockObj;
			}
		}
		/// <summary>
		/// Boolean that denotes if this action unlock the associated object.
		/// </summary>
		private bool _unlockObj;
		/// <summary>
		/// Gets a value indicating whether this <see cref="Its.Expert.ObjectModel.ActionAplication"/> unlock object.
		/// </summary>
		/// <value><c>true</c> if unlock object; otherwise, <c>false</c>.</value>
		public bool UnlockObj {
			get {
				return _unlockObj;
			}
		}
		/// <summary>
		/// Maximun time that the action can be executed. After this time, an error will be produced.
		/// </summary>
		private int _maxTime;
		/// <summary>
		/// Gets the max time.
		/// </summary>
		/// <value>The max time.</value>
		public int MaxTime {
			get {
				return _maxTime;
			}
		}
		/// <summary>
		/// Error associated with the maximun time.
		/// </summary>
		private Error _maxTimeError;
		/// <summary>
		/// Gets the max time error.
		/// </summary>
		/// <value>The max time error.</value>
		public Error MaxTimeError {
			get {
				return _maxTimeError;
			}
		}
		/// <summary>
		/// Minimun time that the action need to be executed. Before this time, an error will be produced.
		/// </summary>
		private int _minTime;
		/// <summary>
		/// Gets the minimum time.
		/// </summary>
		/// <value>The minimum time.</value>
		public int MinTime {
			get {
				return _minTime;
			}
		}
		/// <summary>
		/// Error associated with the minimun time.
		/// </summary>
		private Error _minTimeError;
		/// <summary>
		/// Gets the minimum time error.
		/// </summary>
		/// <value>The minimum time error.</value>
		public Error MinTimeError {
			get {
				return _minTimeError;
			}
		}
		/// <summary>
		/// Boolean that denotes if the action can be repeated.
		/// </summary>
		private bool _isRepetitive;
		/// <summary>
		/// Gets a value indicating whether this instance is repetitive.
		/// </summary>
		/// <value><c>true</c> if this instance is repetitive; otherwise, <c>false</c>.</value>
		public bool IsRepetitive {
			get {
				return _isRepetitive;
			}
		}
		/// <summary>
		/// Boolean that denotes the action is the first action of a new phase.
		/// </summary>
		private bool _initPhase;
		/// <summary>
		/// Gets a value indicating whether this <see cref="Its.Expert.ObjectModel.ActionAplication"/> init phase.
		/// </summary>
		/// <value><c>true</c> if init phase; otherwise, <c>false</c>.</value>
		public bool InitPhase {
			get {
				return _initPhase;
			}
		}
		/// <summary>
		/// Boolean that denotes if the ActionAplication validates the previous errors.
		/// </summary>
		private bool _validatePhaseErrors;
		/// <summary>
		/// Gets a value indicating whether this <see cref="Its.Expert.ObjectModel.ActionAplication"/> validate errors.
		/// </summary>
		/// <value><c>true</c> if validate errors; otherwise, <c>false</c>.</value>
		public bool ValidateErrors {
			get {
				return _validatePhaseErrors;
			}
		}
		/// <summary>
		/// List of dependence that the action has associated.
		/// </summary>
		private ComplexDependence _dependence;
		/// <summary>
		/// Gets the dependence.
		/// </summary>
		/// <value>The dependence.</value>
		public ComplexDependence Dependence {
			get {
				return _dependence;
			}
		}

		/// <summary>
		/// List in which incompatibilities action will be saved.
		/// </summary>
		private List<Incompatibility> _incompatibilities;
		/// <summary>
		/// Gets the incompatibilities.
		/// </summary>
		/// <value>The incompatibilities.</value>
		public List<Incompatibility> Incompatibilities {
			get {
				return _incompatibilities;
			}
			set {
				_incompatibilities = value;
			}
		}
		/// <summary>
		/// Boolean that denotes if this action corrects errors.
		/// </summary>
		private bool _correctiveAction;
		/// <summary>
		/// Gets a value indicating whether this <see cref="Its.Expert.ObjectModel.ActionAplication"/> corrective action.
		/// </summary>
		/// <value><c>true</c> if corrective action; otherwise, <c>false</c>.</value>
		public bool CorrectiveAction {
			get {
				return _correctiveAction;
			}
		}
		/// <summary>
		/// Boolean that denotes if this action is not part of the plan but is allowed in it.
		/// </summary>
		private bool _noPlanAction;
		/// <summary>
		/// Gets a value indicating whether this <see cref="Its.Expert.ObjectModel.ActionAplication"/> is not part of the plan but is allowed in it.
		/// </summary>
		/// <value><c>true</c> if is not in the plan; otherwise, <c>false</c>.</value>
		public bool NoPlanAction {
			get {
				return _noPlanAction;
			}
		}
		/// <summary>
		/// List in which is contained the errors that the action correct if the action is corrective.
		/// </summary>
		private List<Error> _errorsToCorrect;
		/// <summary>
		/// Gets the errors to correct.
		/// </summary>
		/// <value>The errors to correct.</value>
		public List<Error> ErrorsToCorrect {
			get {
				return _errorsToCorrect;
			}
		}
		/// <summary>
		/// The ok message.
		/// </summary>
		private string _okMessage;
		/// <summary>
		/// Gets the ok message.
		/// </summary>
		/// <value>The ok message.</value>
		public string OkMessage {
			get {
				return _okMessage;
			}
		}
		/// <summary>
		/// Boolean that denotes if this action shows the  ok message.
		/// </summary>
		private bool _showOkMessage;
		/// <summary>
		/// Gets a value indicating whether this <see cref="Its.Expert.ObjectModel.ActionAplication"/> show ok message.
		/// </summary>
		/// <value><c>true</c> if show ok message; otherwise, <c>false</c>.</value>
		public bool ShowOkMessage {
			get {
				return _showOkMessage;
			}
		}
		/// <summary>
		/// The possible next actions.
		/// </summary>
		private List<ActionAplication> _possibleNextActions;
		/// <summary>
		/// Gets or sets the possible next actions.
		/// </summary>
		/// <value>The possible next actions.</value>
		public List<ActionAplication> PossibleNextActions {
			get {
				return _possibleNextActions;
			}
			set {
				_possibleNextActions = value;
			}
		}
		/// <summary>
		/// The tutor message.
		/// </summary>
		private TutorMessage _tutorMsg;
		/// <summary>
		/// Gets the tutor message.
		/// </summary>
		/// <value>The tutor message.</value>
		public TutorMessage TutorMsg {
			get {
				return _tutorMsg;
			}
		}
		/// <summary>
		/// Whether action represents a checkpoint for student reclustering in Collective Student Model tutoring strategy.
		/// </summary>
		private bool _isCheckpoint;
		/// <summary>
		/// Gets the IsCheckpoint flag.
		/// </summary>
		/// <value>Boolean to indicate whether current action is a checkpoint for student reclustering.</value>
		public bool IsCheckpoint {
			get {
				return _isCheckpoint;
			}
		}
		/// <summary>
		/// The low-detail (most generic) tutor message for Collective Student Model tutor.
		/// </summary>
		private TutorMessage _tutorMsgLowDetail;
		/// <summary>
		/// Gets the tutor message.
		/// </summary>
		/// <value>The tutor message.</value>
		public TutorMessage TutorMsgLowDetail {
			get {
				return _tutorMsgLowDetail;
			}
		}
		/// <summary>
		/// The medium-detail tutor message for Collective Student Model tutor.
		/// </summary>
		private TutorMessage _tutorMsgMediumDetail;
		/// <summary>
		/// Gets the tutor message.
		/// </summary>
		/// <value>The tutor message.</value>
		public TutorMessage TutorMsgMediumDetail {
			get {
				return _tutorMsgMediumDetail;
			}
		}
		/// <summary>
		/// The high-detail (most detailed) tutor message for Collective Student Model tutor.
		/// </summary>
		private TutorMessage _tutorMsgHighDetail;
		/// <summary>
		/// Gets the tutor message.
		/// </summary>
		/// <value>The tutor message.</value>
		public TutorMessage TutorMsgHighDetail {
			get {
				return _tutorMsgHighDetail;
			}
		}
		 

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.Expert.ObjectModel.ActionAplication"/> class.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="phase">Phase.</param>
		/// <param name="name">Name.</param>
		/// <param name="description">Description.</param>
		/// <param name="objectName">Object name.</param>
		/// <param name="lockObj">If set to <c>true</c> lock object.</param>
		/// <param name="unlockObj">If set to <c>true</c> unlock object.</param>
		/// <param name="isRepetitive">If set to <c>true</c> is repetitive.</param>
		/// <param name="initPhase">If set to <c>true</c> init phase.</param>
		/// <param name="validatePhaseErrors">If set to <c>true</c> validate phase errors.</param>
		/// <param name="dependence">Dependence.</param>
		/// <param name="incompatibilities">Incompatibilities.</param>
		/// <param name="correctiveAction">If set to <c>true</c> corrective action.</param>
		/// <param name="errorsToCorrect">Errors to correct.</param>
		/// <param name="okMessage">Ok message.</param>
		/// <param name="showOkMessage">If set to <c>true</c> show ok message.</param>
		/// <param name="possibleNextActions">Possible next actions.</param>
		/// <param name="tutorMessage">Tutor message.</param>
		/// <exception cref="ArgumentException"> 
		/// Thrown if the <paramref name="key"/> is empty or blank.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="lockObj"/> has a <c>true</c> value and <paramref name="unlockObj"/> has a <c>true</c> value too.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="correctiveAction"/> has a <c>true</c> and <paramref name="errorsToCorrect"/> is empty or null.
		/// </exception>
		public ActionAplication (string key, int phase, string name, string description, List<string> objectName, bool lockObj, 
			bool unlockObj, bool isRepetitive, bool initPhase, bool validatePhaseErrors, ComplexDependence dependence, 
			List<Incompatibility> incompatibilities, bool correctiveAction, bool noPlanAction, List<Error> errorsToCorrect,
			string okMessage, bool showOkMessage, List<ActionAplication> possibleNextActions, TutorMessage tutorMessage,
			bool isCheckpoint, TutorMessage tutorMsgLowDetail, TutorMessage tutorMsgMediumDetail, TutorMessage tutorMsgHighDetail)
		{
			//If the key parameter is null or empty or his contained is a blank, a exception is thrown.
			if (StringUtils.IsNullOrWhiteSpace (key)) {  
				//This argument exception says that the key param is not correct.
				ArgumentException argEx = new ArgumentException ("The key parameter is not correct, " +
					"it can neither be null or empty string nor his contained be blank.", "key");
				throw argEx;
			} else if (lockObj == true && unlockObj == true) {
				//This argument exception says that an action cannot lock and unlock an object.
				ArgumentException argEx = new ArgumentException ("The lockObj and unlockObj parameters are not correct, " +
					"An action cannot lock and unlock an object at the same time.", "lockObj");
				throw argEx;
			} else if (correctiveAction == true && (errorsToCorrect.Count < 1 || errorsToCorrect == null)) {
				//This argument exception says that an action cannot be corrective and do not have any error in the
				//associated list.
				ArgumentException argEx = new ArgumentException ("A corrective action must have an error list associated, " +
					"This list must be neither empty nor null.", "errorsToCorrect");
				throw argEx;
			} else {
				this._key = key;
				this._phase = phase;
				this._name = name;
				this._description = description;
				this._objectName = objectName;
				this._lockObj = lockObj;
				this._unlockObj = unlockObj;
				this._maxTime = 0;
				this._maxTimeError = null;
				this._minTime = 0;
				this._minTimeError = null;
				this._isRepetitive = isRepetitive;
				this._initPhase = initPhase;
				this._validatePhaseErrors = validatePhaseErrors;
				this._dependence = dependence;
				this._incompatibilities = incompatibilities;
				this._correctiveAction = correctiveAction;
				this._noPlanAction = noPlanAction;
				this._errorsToCorrect = errorsToCorrect;
				this._okMessage = okMessage;
				this._showOkMessage = showOkMessage;
				this._possibleNextActions = possibleNextActions;
				this._tutorMsg = tutorMessage;
				this._isCheckpoint = isCheckpoint;
				this._tutorMsgLowDetail = tutorMsgLowDetail;
				this._tutorMsgMediumDetail = tutorMsgMediumDetail;
				this._tutorMsgHighDetail = tutorMsgHighDetail;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.Expert.ObjectModel.ActionAplication"/> class.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="phase">Phase.</param>
		/// <param name="name">Name.</param>
		/// <param name="description">Description.</param>
		/// <param name="objectName">Object name.</param>
		/// <param name="lockObj">If set to <c>true</c> lock object.</param>
		/// <param name="unlockObj">If set to <c>true</c> unlock object.</param>
		/// <param name="isRepetitive">If set to <c>true</c> is repetitive.</param>
		/// <param name="initPhase">If set to <c>true</c> init phase.</param>
		/// <param name="validatePhaseErrors">If set to <c>true</c> validate phase errors.</param>
		/// <param name="dependence">Dependence.</param>
		/// <param name="incompatibilities">Incompatibilities.</param>
		/// <param name="correctiveAction">If set to <c>true</c> corrective action.</param>
		/// <param name="errorsToCorrect">Errors to correct.</param>
		/// <param name="okMessage">Ok message.</param>
		/// <param name="showOkMessage">If set to <c>true</c> show ok message.</param>
		/// <param name="possibleNextActions">Possible next actions.</param>
		/// <param name="tutorMessage">Tutor message.</param>
		/// <param name="minTime">Minimum time.</param>
		/// <param name="minTimeError">Minimum time error.</param>
		/// <param name="maxTime">Max time.</param>
		/// <param name="maxTimeError">Max time error.</param>
		/// <exception cref="ArgumentException"> 
		/// Thrown if the <paramref name="key"/> is empty or blank.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="lockObj"/> has a <c>true</c> value and <paramref name="unlockObj"/> has a <c>true</c> value too.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="correctiveAction"/> has a <c>true</c> and <paramref name="errorsToCorrect"/> is empty or null.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="mintimeError"/> or <paramref name="maxTimeError"/> is not a correct type error.
		/// </exception>
		public ActionAplication (string key, int phase, string name, string description, List<string> objectName, bool lockObj, 
			bool unlockObj, bool isRepetitive, bool initPhase, bool validatePhaseErrors,
			ComplexDependence dependence, List<Incompatibility> incompatibilities, bool correctiveAction, bool noPlanAction,
			List<Error> errorsToCorrect, string okMessage, bool showOkMessage, List<ActionAplication> possibleNextActions,
			TutorMessage tutorMessage, int minTime, Error minTimeError, int maxTime = 0, Error maxTimeError = null,
			bool isCheckpoint = false, TutorMessage tutorMsgLowDetail = null, TutorMessage tutorMsgMediumDetail = null, TutorMessage tutorMsgHighDetail = null)
		{
			//If the key parameter is null or empty or his contained is a blank, a exception is thrown.
			if (StringUtils.IsNullOrWhiteSpace (key)) {  
				//This argument exception says that the key param is not correct.
				ArgumentException argEx = new ArgumentException ("The key parameter is not correct, " +
					"it can neither be null or empty string nor his contained be blank.", "key");
				throw argEx;
			} else if (lockObj == true && unlockObj == true) {
				//This argument exception says that an action cannot lock and unlock an object.
				ArgumentException argEx = new ArgumentException ("The lockObj and unlockObj parameters are not correct, " +
				                          "An action cannot lock and unlock an object at the same time.", "lockObj");
				throw argEx;
			} else if (correctiveAction == true && (errorsToCorrect.Count < 1 || errorsToCorrect == null)) {
				//This argument exception says that an action cannot be corrective and do not have any error in the
				//associated list.
				ArgumentException argEx = new ArgumentException ("A corrective action must have an error list associated, " +
				                          "This list must be neither empty nor null.", "errorsToCorrect");
				throw argEx;
			} else {
				this._key = key;
				this._phase = phase;
				this._name = name;
				this._description = description;
				this._objectName = objectName;
				this._lockObj = lockObj;
				this._unlockObj = unlockObj;
				this._isRepetitive = isRepetitive;
				this._initPhase = initPhase;
				this._validatePhaseErrors = validatePhaseErrors;
				this._dependence = dependence;
				this._incompatibilities = incompatibilities;
				this._correctiveAction = correctiveAction;
				this._noPlanAction = noPlanAction;
				this._errorsToCorrect = errorsToCorrect;
				this._okMessage = okMessage;
				this._showOkMessage = showOkMessage;
				this._possibleNextActions = possibleNextActions;
				this._tutorMsg = tutorMessage;
				this._minTime = minTime;
				this._minTimeError = minTimeError;
				this._maxTime = maxTime;
				this._maxTimeError = maxTimeError;
				this._isCheckpoint = isCheckpoint;
				this._tutorMsgLowDetail = tutorMsgLowDetail;
				this._tutorMsgMediumDetail = tutorMsgMediumDetail;
				this._tutorMsgHighDetail = tutorMsgHighDetail;
			}
		}

		/// <summary>
		/// Temporary constructor method.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="name">Name.</param>
		public ActionAplication (string key, string name)
		{
			this._key = key;
			this._name = name;
		}

		/// <summary>
		/// Temporary constructor method.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="name">Name.</param>
		/// <param name="dependence">Dependence.</param>
		public ActionAplication (string key, string name, ComplexDependence dependence)
		{
			this._key = key;
			this._name = name;
			this._dependence = dependence;
		}
	}
}
