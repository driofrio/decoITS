using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using com.hp.hpl.jena.datatypes.xsd;
using com.hp.hpl.jena.ontology;
using com.hp.hpl.jena.rdf.model;
using Its.ExpertModule.ObjectModel;
using Its.StudentModule.ObjectModel;
using Its.TutoringModule.ReactiveTutor.ObjectModel;
using java.io;
using java.util;
using File = System.IO.File;

namespace Its.StudentModule.DataAccess
{

	/// <author>
	///  Diego Dotor Jara.
	/// </author>
	/// <summary>
	/// This class will access to the student ontology.
	/// </summary>
	public class OntologyAccess
	{
		/// <summary>
		/// The ontology URI.
		/// </summary>
		private const string STUDENT_INFORMATION_URI = "http://www.owl-ontologies.com/student_information#";
		/// <summary>
		/// The student_state URI.
		/// </summary>
		private const string STUDENT_STATE_URI = "http://www.owl-ontologies.com/student_state.owl#";
		/// <summary>
		/// The studen_profile URI.
		/// </summary>
		private const string STUDENT_PROFILE_URI = "http://www.owl-ontologies.com/student_profile.owl#";
		/// <summary>
		/// The student_monitoring URI.
		/// </summary>
		private const string STUDENT_MONITORING_URI = "http://www.owl-ontologies.com/student_monitoring.owl#";
		/// <summary>
		/// The student_trace URI.
		/// </summary>
		private const string STUDENT_TRACE_URI = "http://www.owl-ontologies.com/student_trace.owl#";
		/// <summary>
		/// The learning_objective URI.
		/// </summary>
		private const string LEARNING_OBJECTIVE_URI = "http://www.owl-ontologies.com/learning_objective.owl#";
		/// <summary>
		/// The knowledge_object URI.
		/// </summary>
		private const string KNOWLEDGE_OBJECT_URI = "http://www.owl-ontologies.com/knowledge_object.owl#";
		/// <summary>
		/// The OntologyAccess singleton instance.
		/// </summary>
		private static OntologyAccess _instance = null;
		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static OntologyAccess Instance (string ontologyPath, string logsPath){
			//get {
				if (_instance == null) {
				_instance = new OntologyAccess (ontologyPath, logsPath);
				}

			return _instance;
			//}
		}
		/// <summary>
		/// The ontology model in which it will be saved student information.
		/// </summary>
		private static OntModel _studentDataModel;
		/// <summary>
		/// The ontology model in which it will be worked with domain and action information.
		/// </summary>
		private static OntModel _domainDataModel;
		/// <summary>
		/// The other data model.
		/// </summary>
		private static OntModel _otherDataModel;
		/// <summary>
		/// The world data model.
		/// </summary>
		private static OntModel _worldDataModel;
		/// <summary>
		/// The general ontology model.
		/// </summary>
		private static OntModel _ontModel;
		/// <summary>
		/// The student ontology.
		/// </summary>
		private static Ontology _studentOnto;
		/// <summary>
		/// The domain ontology.
		/// </summary>
		private static Ontology _domainOnto;
		/// <summary>
		/// The other ontology.
		/// </summary>
		private static Ontology _otherOnto;
		/// <summary>
		/// The world ontology.
		/// </summary>
		private static Ontology _worldOnto;
		/// <summary>
		/// The ont models.
		/// </summary>
		private static Dictionary<string, OntModel> _logModels = new Dictionary<string, OntModel>();
		/// <summary>
		/// The ontology path.
		/// </summary>
		private static string _ontologyPath;
		/// <summary>
		/// The logs path.
		/// </summary>
		private static string _logsPath;

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.Student.DataAccess.OntologyAccess"/> class.
		/// </summary>
		private OntologyAccess (string ontologyPath, string logsPath)
		{
			//Creates an ontology model empty instance.
			_studentDataModel = ModelFactory.createOntologyModel ();
			//Creates an ontology model empty instance.
			_domainDataModel = ModelFactory.createOntologyModel ();
			//Creates an ontology model empty instance.
			_otherDataModel = ModelFactory.createOntologyModel ();
			//Creates an ontology model empty instance.
			_worldDataModel = ModelFactory.createOntologyModel ();
			//Creates an ontology model.
			_studentOnto = _studentDataModel.createOntology ("");
			//Creates an ontology model.
			_domainOnto = _domainDataModel.createOntology ("");
			//Creates an ontology model.
			_otherOnto = _otherDataModel.createOntology ("");
			//Creates an ontology model.
			_worldOnto = _worldDataModel.createOntology ("");
			//Imports the ontology resource given by ontology URI.
			_studentOnto.addImport (_studentDataModel.createResource (STUDENT_INFORMATION_URI));
			//Imports the ontology resource given by ontology URI.
			_domainOnto.addImport (_domainDataModel.createResource (STUDENT_INFORMATION_URI));
			//Imports the ontology resource given by ontology URI.
			_otherOnto.addImport (_domainDataModel.createResource (STUDENT_INFORMATION_URI));
			//Imports the ontology resource given by ontology URI.
			_worldOnto.addImport (_domainDataModel.createResource (STUDENT_INFORMATION_URI));
			////Creates an ontology model instance.
			_ontModel = ModelFactory.createOntologyModel (OntModelSpec.OWL_DL_MEM, null);
			//Obtains the ontology path.
			_ontologyPath = ontologyPath;//ConfigurationManager.AppSettings ["ontologyPath"].ToString ();
			//_ontologyPath = _ontologyPath.Replace ('\\', Path.DirectorySeparatorChar);
			//Obtains the logs path.
			_logsPath = logsPath;//ConfigurationManager.AppSettings ["logsPath"].ToString ();
			//_logsPath = _logsPath.Replace ('\\', Path.DirectorySeparatorChar);
			//Adds the documents necesary.
			_ontModel.getDocumentManager().addAltEntry( "http://www.owl-ontologies.com/student_information",
				"file:" + _ontologyPath + "student_information.owl" );
			_ontModel.getDocumentManager().addAltEntry( "http://www.owl-ontologies.com/student_state.owl",
				"file:" + _ontologyPath + "student_state.owl" );
			_ontModel.getDocumentManager().addAltEntry( "http://www.owl-ontologies.com/student_profile.owl",
				"file:" + _ontologyPath + "student_profile.owl" );
			_ontModel.getDocumentManager().addAltEntry( "http://www.owl-ontologies.com/student_monitoring.owl",
				"file:" + _ontologyPath + "student_monitoring.owl" );
			_ontModel.getDocumentManager().addAltEntry( "http://www.owl-ontologies.com/student_trace.owl",
				"file:" + _ontologyPath + "student_trace.owl" );
			_ontModel.getDocumentManager().addAltEntry( "http://www.owl-ontologies.com/learning_objective.owl",
				"file:" + _ontologyPath + "learning_objective.owl" );
			_ontModel.getDocumentManager().addAltEntry( "http://www.owl-ontologies.com/knowledge_object.owl",
				"file:" + _ontologyPath + "knowledge_object.owl" );
			_ontModel.getDocumentManager().addAltEntry ("http://www.owl-ontologies.com/knowledge_object.owl/other_error.owl",
				"file:" + _ontologyPath + "other_error.owl");
			_ontModel.getDocumentManager().addAltEntry ("http://www.owl-ontologies.com/knowledge_object.owl/world_error.owl",
				"file:" + _ontologyPath + "world_error.owl");
			//Loads the document into the ontology model.
			_ontModel.read ("http://www.owl-ontologies.com/student_information");
			/*try{

			_otherDataModel.read ("http://www.owl-ontologies.com/knowledge_object.owl/other_error.owl");
			}catch(System.Security.Cryptography.CryptographicException e){}*/
			InputStream fe;

			try{
			if (File.Exists (_ontologyPath + "other_error.owl")) {
				fe = new FileInputStream (_ontologyPath + "other_error.owl");
				_otherDataModel.read (new InputStreamReader (fe, "UTF-8"), "");
				fe.close ();
			}
			}catch(CryptographicException e){}

			if (File.Exists (_ontologyPath + "world_error.owl")) {
				fe = new FileInputStream (_ontologyPath + "world_error.owl");
				_worldDataModel.read (new InputStreamReader (fe, "UTF-8"), "");
				fe.close ();
			}

			//_worldDataModel.read ("file:" + _ontologyPath + "world_error.owl");

			if (File.Exists (_ontologyPath + "student_data.owl")) {
				fe = new FileInputStream (_ontologyPath + "student_data.owl");
				_studentDataModel.read (new InputStreamReader (fe, "UTF-8"), "");
				fe.close ();
			}
			/*if (System.IO.File.Exists(_ontologyPath + "student_data.owl")) {
				//Loads the student information.
				_studentDataModel.read ("file:" + _ontologyPath + "student_data.owl");
			}*/
			if (File.Exists (_ontologyPath + "plan_data.owl")) {
				fe = new FileInputStream (_ontologyPath + "plan_data.owl");
				_domainDataModel.read (new InputStreamReader (fe, "UTF-8"), "");
				fe.close ();
			}
			/*if (System.IO.File.Exists(_ontologyPath + "plan_data.owl")) {
				//Loads the domain information.
				_domainDataModel.read ("file:" + _ontologyPath + "plan_data.owl");
			}*/
			//Creates a prefix in the empty ontology model.
			_studentDataModel.setNsPrefix ("student_information", "http://www.owl-ontologies.com/student_information#");
			_studentDataModel.setNsPrefix ("student_state", "http://www.owl-ontologies.com/student_state.owl#");
			_studentDataModel.setNsPrefix ("student_profile", "http://www.owl-ontologies.com/student_profile.owl#");
			_studentDataModel.setNsPrefix ("student_trace", "http://www.owl-ontologies.com/student_trace.owl#");
			_studentDataModel.setNsPrefix ("knowledge_object", "http://www.owl-ontologies.com/knowledge_object.owl#");
			_studentDataModel.setNsPrefix ("", "http://www.owl-ontologies.com/student_data.owl");
			//Creates a prefix in the empty ontology model.
			_domainDataModel.setNsPrefix ("student_information", "http://www.owl-ontologies.com/student_information#");
			_domainDataModel.setNsPrefix ("student_state", "http://www.owl-ontologies.com/student_state.owl#");
			_domainDataModel.setNsPrefix ("student_profile", "http://www.owl-ontologies.com/student_profile.owl#");
			_domainDataModel.setNsPrefix ("student_trace", "http://www.owl-ontologies.com/student_trace.owl#");
			_domainDataModel.setNsPrefix ("knowledge_object", "http://www.owl-ontologies.com/knowledge_object.owl#");
			_domainDataModel.setNsPrefix ("", "http://www.owl-ontologies.com/plan_data.owl");
			//Checks if the log path exists.
			if (Directory.Exists(_logsPath)) {
				//Gets all subdirectories.
				string[] directories = Directory.GetDirectories(_logsPath);
				//Loads all owl files from subdirectories.
				foreach (string p in directories) {
					//Gets the files in the directory p.
					string[] files = Directory.GetFiles(p, "*.owl");
					//Obtains the Domain key.
					string domainKey = p.Replace(_logsPath, "");
					domainKey = domainKey.Replace(Path.DirectorySeparatorChar.ToString(), "");
					//Obtains the Student key from file path.
					foreach (string f in files) {
						//Obtains the Domain key.
						string studentKey = f.Replace(_logsPath + domainKey + Path.DirectorySeparatorChar, "");
						studentKey = studentKey.Replace(".owl", "");
						//Loads the file into an OntModel.
						CreateOntModel (domainKey, studentKey);
					}
				}
			}
		}

		/// <summary>
		/// Creates the ont model.
		/// </summary>
		/// <returns>The ont model.</returns>
		/// <param name="domainKey">Domain key.</param>
		/// <param name="studentKey">Student key.</param>
		public OntModel CreateOntModel(string domainKey, string studentKey) 
		{
			//Creates an ontology model empty instance.
			OntModel oTemp = ModelFactory.createOntologyModel ();
			//Creates an ontology model.
			Ontology ontoTemp = oTemp.createOntology ("");
			//Imports the ontology resource given by ontology URI.
			ontoTemp.addImport (oTemp.createResource (STUDENT_INFORMATION_URI));
			if (File.Exists (_logsPath + domainKey + Path.DirectorySeparatorChar + studentKey + ".owl")) {
				FileInputStream fe = new FileInputStream (_logsPath + domainKey + Path.DirectorySeparatorChar + studentKey + ".owl");
				oTemp.read (new InputStreamReader (fe, "UTF-8"), "");
				fe.close ();
			}
			/*if (System.IO.File.Exists (_logsPath + domainKey + Path.DirectorySeparatorChar + studentKey + ".owl")) {
				//Loads the log information.
				oTemp.read ("file:" + _logsPath + domainKey + Path.DirectorySeparatorChar + studentKey + ".owl");
			}*/
			//Creates a prefix in the empty ontology model.
			oTemp.setNsPrefix ("student_information", "http://www.owl-ontologies.com/student_information#");
			oTemp.setNsPrefix ("student_state", "http://www.owl-ontologies.com/student_state.owl#");
			oTemp.setNsPrefix ("student_profile", "http://www.owl-ontologies.com/student_profile.owl#");
			oTemp.setNsPrefix ("student_trace", "http://www.owl-ontologies.com/student_trace.owl#");
			oTemp.setNsPrefix ("knowledge_object", "http://www.owl-ontologies.com/knowledge_object.owl#");
			oTemp.setNsPrefix ("other_error", "http://www.owl-ontologies.com/knowledge_object.owl/other_error.owl#");
			oTemp.setNsPrefix ("world_error", "http://www.owl-ontologies.com/knowledge_object.owl/world_error.owl#");
			oTemp.setNsPrefix ("", "http://www.owl-ontologies.com/log/" + domainKey + studentKey + ".owl");
			//Add the ontology model into the dictionary.
			_logModels.Add (domainKey + "-" + studentKey, oTemp);
			//Returns the OntModel.
			return oTemp;
		}

		/// <summary>
		/// Adds the log into the ontology.
		/// </summary>
		/// <param name="log">Log.</param>
		public void AddLogIntoOnto (NoCorrectiveActionLog log, Student student, DomainActions domain) 
		{
			//Creates a auxiliar variable.
			OntModel ontM;
			//Selects the ontology model with the specific domain and student
			var oQuery = 
				from o in _logModels
				where o.Key == (domain.Key + "-" + student.Key)
				select o.Value;
			//Checks if the OntModel exists. If it does not exist, create a new OntModel.
			if (oQuery.ToList ().Count == 0)
				ontM = CreateOntModel (domain.Key, student.Key);
			else
				ontM = oQuery.First ();
			//Add directly the log into the ontology, without any kind of test.
			//Gets the resource Action_Trace.
			Resource r = ontM.getResource (STUDENT_TRACE_URI + "Action_Trace");
			//Gets the type property  of Thing.
			Property p = ontM.getProperty("http://www.w3.org/1999/02/22-rdf-syntax-ns#type");
			//Creates a new Action_Trace resource.
			Resource actionTrace = ontM.createResource (STUDENT_TRACE_URI + "Log" + domain.Key
			                       + student.Key + log.Action.Key
			                       + log.DateLog.ToString ("dd/MM/yyyy HH:mm:ss.fff").Replace (" ", null));
			//Creates a new statement.
			Statement stmt = ontM.createStatement (actionTrace, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the resource Action_Execution_State.
			r = ontM.getResource (STUDENT_STATE_URI + "Action_Execution_State");
			//Creates a new Action_Execution_State resource.
			Resource actionExeState = ontM.createResource (KNOWLEDGE_OBJECT_URI + "ActionExecutionState" + domain.Key
			                          + student.Key + log.Action.Key
			                          + log.DateLog.ToString ("dd/MM/yyyy HH:mm:ss.fff").Replace (" ", null));
			//Creates a new statement.
			stmt = ontM.createStatement (actionExeState, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the wasApplied property of Action_Execution_State resource and adds the value.
			p = ontM.getProperty (STUDENT_STATE_URI + "wasApplied");
			actionExeState.addLiteral (p, log.WasApplied);
			//Sets the associatedActionState property of Action_Trace.
			p = ontM.getProperty (STUDENT_TRACE_URI + "associatedActionState");
			//Creates a new statement.
			stmt = ontM.createStatement (actionTrace, p, actionExeState);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the Punctual_Action whose key is equals to the ActionAplication.
			p = ontM.getProperty (KNOWLEDGE_OBJECT_URI + "idObject");
			StmtIterator stmtItr = _domainDataModel.listStatements (null, p, log.Action.Key + "_" + domain.Key);
			//Gets the statement. It is assumed that there is a only statement.
			stmt = stmtItr.nextStatement ();
			//Gets the statement subject, which is the PunctualAction with the key.
			r = stmt.getSubject ();
			//Sets the associatedSpecificAction property of Action_Trace resource.
			p = ontM.getProperty (STUDENT_TRACE_URI + "associatedSpecificAction");
			//Creates a new statement.
			stmt = ontM.createStatement (actionTrace, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the Student_Profile whose key is equals to the Student.
			p = ontM.getProperty (STUDENT_PROFILE_URI + "idStudent");
			stmtItr = _studentDataModel.listStatements (null, p, student.Key);
			//Gets the statement. It is assumed that there is a only statement.
			stmt = stmtItr.nextStatement ();
			//Gets the statement subject, which is the Student_Profile with the key.
			Resource rAux = stmt.getSubject ();
			//Sets the Student_Information which has the previous Student_Profile.
			p = ontM.getProperty (STUDENT_INFORMATION_URI + "hasProfile");
			stmtItr = _studentDataModel.listStatements (null, p, rAux);
			//Gets the statement. It is assumed that there is a only statement.
			stmt = stmtItr.nextStatement ();
			//Gets the statement subject, which is the Student_Information with the key.
			r = stmt.getSubject ();
			//Sets the belongsToStudent property of Action_Trace resource and adds the value.
			//The belongsToStudent value is the previous obtained Student_Information resource.
			p = ontM.getProperty (STUDENT_TRACE_URI + "belongsToStudent");
			//Creates a new statement.
			stmt = ontM.createStatement (actionTrace, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Sets the initialTime property of Action_Trace resource and adds the value.
			p = ontM.getProperty (STUDENT_TRACE_URI + "initialTime");
			Calendar date = new GregorianCalendar ();
			date.set (log.DateLog.Year, log.DateLog.Month-1, log.DateLog.Day, log.DateLog.Hour, log.DateLog.Minute, log.DateLog.Second);
			date.set (Calendar.MILLISECOND, log.DateLog.Millisecond);
			XSDDateTime t = new XSDDateTime (date);
			actionTrace.addLiteral (p, t);

			//Saves into disk.
			string path = _logsPath + domain.Key + Path.DirectorySeparatorChar;
			//Determine whether the directory exists. If the directory does not exist, it will be created.
			if (!Directory.Exists(path)) {
				Directory.CreateDirectory(path);
			}
			//Specifies the file path.
			string file = path + student.Key + ".owl";
			//Saves the logs into files.
			using (BufferedWriter bw = new BufferedWriter (new OutputStreamWriter(new FileOutputStream(file), "UTF-8"))) {
				string b = "file:" + file;
				//using (OntModel ontTmp = ModelFactory.createOntologyModel (OntModelSpec.OWL_DL_MEM, null)) {
				//ontTmp = _ontologyModel;
				ontM.write (bw, "RDF/XML-ABBREV", b);
				//}
				bw.close ();
			}
		}

		/// <summary>
		/// Adds the log into onto.
		/// </summary>
		/// <param name="log">Log.</param>
		public void AddLogIntoOnto (CorrectiveActionLog log, Student student, DomainActions domain) 
		{
			//Creates a auxiliar variable.
			OntModel ontM;
			//Selects the ontology model with the specific domain and student
			var oQuery = 
				from o in _logModels
					where o.Key == (domain.Key + "-" + student.Key)
				select o.Value;
			//Checks if the OntModel exists. If it does not exist, create a new OntModel.
			if (oQuery.ToList ().Count == 0)
				ontM = CreateOntModel (domain.Key, student.Key);
			else
				ontM = oQuery.First ();
			//Add directly the log into the ontology, without any kind of test.
			//Gets the resource Action_Trace.
			Resource r = ontM.getResource (STUDENT_TRACE_URI + "Action_Trace");
			//Gets the type property  of Thing.
			Property p = ontM.getProperty("http://www.w3.org/1999/02/22-rdf-syntax-ns#type");
			//Creates a new Action_Trace resource.
			Resource actionTrace = ontM.createResource (STUDENT_TRACE_URI + "Log" + domain.Key
			                       + student.Key + log.Action.Key
			                       + log.DateLog.ToString ("dd/MM/yyyy HH:mm:ss.fff").Replace (" ", null));
			//Creates a new statement.
			Statement stmt = ontM.createStatement (actionTrace, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the resource Action_Execution_State.
			r = ontM.getResource (STUDENT_STATE_URI + "Action_Execution_State");
			//Creates a new Action_Execution_State resource.
			Resource actionExeState = ontM.createResource (KNOWLEDGE_OBJECT_URI + "ActionExecutionState" + domain.Key
			                          + student.Key + log.Action.Key
			                          + log.DateLog.ToString ("dd/MM/yyyy HH:mm:ss.fff").Replace (" ", null));
			//Creates a new statement.
			stmt = ontM.createStatement (actionExeState, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the resource Punctual_Action_Element.
			r = ontM.getResource (KNOWLEDGE_OBJECT_URI + "Punctual_Action_Element");
			//Creates a new Punctual_Action_Element resource.
			Resource punctualActionElement = ontM.createResource (KNOWLEDGE_OBJECT_URI + "PunctualActionElement"
			                                 + domain.Key + student.Key + log.Action.Key
			                                 + log.DateLog.ToString ("dd/MM/yyyy HH:mm:ss.fff").Replace (" ", null));
			//Creates a new statement.
			stmt = ontM.createStatement (punctualActionElement, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the resource Postcondition_State.
			r = ontM.getResource (STUDENT_STATE_URI + "Postcondition_State");
			//Creates a new Postcondition_State resource.
			Resource postCondState = ontM.createResource (STUDENT_STATE_URI + "PostconditionState"
			                         + domain.Key + student.Key + log.Action.Key
			                         + log.DateLog.ToString ("dd/MM/yyyy HH:mm:ss.fff").Replace (" ", null));
			//Creates a new statement.
			stmt = ontM.createStatement (postCondState, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the descriptorObject property and adds the value.
			//The value is x property .
			p = ontM.getProperty (KNOWLEDGE_OBJECT_URI + "descriptorObject");
			punctualActionElement.addLiteral (p, "corrected");
			//Gets the refersToCondition property of Postcondition_State resource and adds the value.
			p = ontM.getProperty (STUDENT_STATE_URI + "refersToCondition");
			//Creates a new statement.
			stmt = ontM.createStatement (postCondState, p, punctualActionElement);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the consistsOfConseqState property of Action_Execution_State resource and adds the value.
			p = ontM.getProperty (STUDENT_STATE_URI + "consistsOfConseqState");
			//Creates a new statement.
			stmt = ontM.createStatement (actionExeState, p, postCondState);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the wasApplied property of Action_Execution_State resource and adds the value.
			p = ontM.getProperty (STUDENT_STATE_URI + "wasApplied");
			actionExeState.addLiteral (p, log.WasApplied);
			//Sets the associatedActionState property of Action_Trace.
			p = ontM.getProperty (STUDENT_TRACE_URI + "associatedActionState");
			//Creates a new statement.
			stmt = ontM.createStatement (actionTrace, p, actionExeState);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the Punctual_Action whose key is equals to the ActionAplication.
			p = ontM.getProperty (KNOWLEDGE_OBJECT_URI + "idObject");
			StmtIterator stmtItr = _domainDataModel.listStatements (null, p, log.Action.Key + "_" + domain.Key);
			//Gets the statement. It is assumed that there is a only statement.
			stmt = stmtItr.nextStatement ();
			//Gets the statement subject, which is the PunctualAction with the key.
			r = stmt.getSubject ();
			//Sets the associatedSpecificAction property of Action_Trace resource.
			p = ontM.getProperty (STUDENT_TRACE_URI + "associatedSpecificAction");
			//Creates a new statement.
			stmt = ontM.createStatement (actionTrace, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the Student_Profile whose key is equals to the Student.
			p = ontM.getProperty (STUDENT_PROFILE_URI + "idStudent");
			stmtItr = _studentDataModel.listStatements (null, p, student.Key);
			//Gets the statement. It is assumed that there is a only statement.
			stmt = stmtItr.nextStatement ();
			//Gets the statement subject, which is the Student_Profile with the key.
			Resource rAux = stmt.getSubject ();
			//Sets the Student_Information which has the previous Student_Profile.
			p = ontM.getProperty (STUDENT_INFORMATION_URI + "hasProfile");
			stmtItr = _studentDataModel.listStatements (null, p, rAux);
			//Gets the statement. It is assumed that there is a only statement.
			stmt = stmtItr.nextStatement ();
			//Gets the statement subject, which is the Student_Information with the key.
			r = stmt.getSubject ();
			//Sets the belongsToStudent property of Action_Trace resource and adds the value.
			//The belongsToStudent value is the previous obtained Student_Information resource.
			p = ontM.getProperty (STUDENT_TRACE_URI + "belongsToStudent");
			//Creates a new statement.
			stmt = ontM.createStatement (actionTrace, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Sets the initialTime property of Action_Trace resource and adds the value.
			p = ontM.getProperty (STUDENT_TRACE_URI + "initialTime");
			Calendar date = new GregorianCalendar ();
			date.set (log.DateLog.Year, log.DateLog.Month-1, log.DateLog.Day, log.DateLog.Hour, log.DateLog.Minute, log.DateLog.Second);
			date.set (Calendar.MILLISECOND, log.DateLog.Millisecond);
			XSDDateTime t = new XSDDateTime (date);
			actionTrace.addLiteral (p, t);

			//Saves into disk.
			string path = _logsPath + domain.Key + Path.DirectorySeparatorChar;
			//Determine whether the directory exists. If the directory does not exist, it will be created.
			if (!Directory.Exists(path)) {
				Directory.CreateDirectory(path);
			}
			//Specifies the file path.
			string file = path + student.Key + ".owl";
			//Saves the logs into files.
			using (BufferedWriter bw = new BufferedWriter (new OutputStreamWriter(new FileOutputStream(file), "UTF-8"))) {
				string b = "file:" + file;
				//using (OntModel ontTmp = ModelFactory.createOntologyModel (OntModelSpec.OWL_DL_MEM, null)) {
				//ontTmp = _ontologyModel;
				ontM.write (bw, "RDF/XML-ABBREV", b);
				//}
				bw.close ();
			}
		}

		/// <summary>
		/// Adds the log into onto.
		/// </summary>
		/// <param name="log">Log.</param>
		public void AddLogIntoOnto (NoPlanAllowedActionLog log, Student student, DomainActions domain) 
		{
			//Creates a auxiliar variable.
			OntModel ontM;
			//Selects the ontology model with the specific domain and student
			var oQuery = 
				from o in _logModels
					where o.Key == (domain.Key + "-" + student.Key)
				select o.Value;
			//Checks if the OntModel exists. If it does not exist, create a new OntModel.
			if (oQuery.ToList ().Count == 0)
				ontM = CreateOntModel (domain.Key, student.Key);
			else
				ontM = oQuery.First ();
			//Add directly the log into the ontology, without any kind of test.
			//Gets the resource Action_Trace.
			Resource r = ontM.getResource (STUDENT_TRACE_URI + "Action_Trace");
			//Gets the type property  of Thing.
			Property p = ontM.getProperty("http://www.w3.org/1999/02/22-rdf-syntax-ns#type");
			//Creates a new Action_Trace resource.
			Resource actionTrace = ontM.createResource (STUDENT_TRACE_URI + "Log" + domain.Key
				+ student.Key + log.Action.Key
				+ log.DateLog.ToString ("dd/MM/yyyy HH:mm:ss.fff").Replace (" ", null));
			//Creates a new statement.
			Statement stmt = ontM.createStatement (actionTrace, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the resource Action_Execution_State.
			r = ontM.getResource (STUDENT_STATE_URI + "Action_Execution_State");
			//Creates a new Action_Execution_State resource.
			Resource actionExeState = ontM.createResource (KNOWLEDGE_OBJECT_URI + "ActionExecutionState" + domain.Key
				+ student.Key + log.Action.Key
				+ log.DateLog.ToString ("dd/MM/yyyy HH:mm:ss.fff").Replace (" ", null));
			//Creates a new statement.
			stmt = ontM.createStatement (actionExeState, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the resource Punctual_Action_Element.
			r = ontM.getResource (KNOWLEDGE_OBJECT_URI + "Punctual_Action_Element");
			//Creates a new Punctual_Action_Element resource.
			Resource punctualActionElement = ontM.createResource (KNOWLEDGE_OBJECT_URI + "PunctualActionElement"
				+ domain.Key + student.Key + log.Action.Key
				+ log.DateLog.ToString ("dd/MM/yyyy HH:mm:ss.fff").Replace (" ", null));
			//Creates a new statement.
			stmt = ontM.createStatement (punctualActionElement, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the resource Postcondition_State.
			r = ontM.getResource (STUDENT_STATE_URI + "Postcondition_State");
			//Creates a new Postcondition_State resource.
			Resource postCondState = ontM.createResource (STUDENT_STATE_URI + "PostconditionState"
				+ domain.Key + student.Key + log.Action.Key
				+ log.DateLog.ToString ("dd/MM/yyyy HH:mm:ss.fff").Replace (" ", null));
			//Creates a new statement.
			stmt = ontM.createStatement (postCondState, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the descriptorObject property and adds the value.
			//The value is x property .
			p = ontM.getProperty (KNOWLEDGE_OBJECT_URI + "descriptorObject");
			punctualActionElement.addLiteral (p, "noplan");
			//Gets the refersToCondition property of Postcondition_State resource and adds the value.
			p = ontM.getProperty (STUDENT_STATE_URI + "refersToCondition");
			//Creates a new statement.
			stmt = ontM.createStatement (postCondState, p, punctualActionElement);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the consistsOfConseqState property of Action_Execution_State resource and adds the value.
			p = ontM.getProperty (STUDENT_STATE_URI + "consistsOfConseqState");
			//Creates a new statement.
			stmt = ontM.createStatement (actionExeState, p, postCondState);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the wasApplied property of Action_Execution_State resource and adds the value.
			p = ontM.getProperty (STUDENT_STATE_URI + "wasApplied");
			actionExeState.addLiteral (p, log.WasApplied);
			//Sets the associatedActionState property of Action_Trace.
			p = ontM.getProperty (STUDENT_TRACE_URI + "associatedActionState");
			//Creates a new statement.
			stmt = ontM.createStatement (actionTrace, p, actionExeState);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the Punctual_Action whose key is equals to the ActionAplication.
			p = ontM.getProperty (KNOWLEDGE_OBJECT_URI + "idObject");
			StmtIterator stmtItr = _domainDataModel.listStatements (null, p, log.Action.Key + "_" + domain.Key);
			//Gets the statement. It is assumed that there is a only statement.
			stmt = stmtItr.nextStatement ();
			//Gets the statement subject, which is the PunctualAction with the key.
			r = stmt.getSubject ();
			//Sets the associatedSpecificAction property of Action_Trace resource.
			p = ontM.getProperty (STUDENT_TRACE_URI + "associatedSpecificAction");
			//Creates a new statement.
			stmt = ontM.createStatement (actionTrace, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the Student_Profile whose key is equals to the Student.
			p = ontM.getProperty (STUDENT_PROFILE_URI + "idStudent");
			stmtItr = _studentDataModel.listStatements (null, p, student.Key);
			//Gets the statement. It is assumed that there is a only statement.
			stmt = stmtItr.nextStatement ();
			//Gets the statement subject, which is the Student_Profile with the key.
			Resource rAux = stmt.getSubject ();
			//Sets the Student_Information which has the previous Student_Profile.
			p = ontM.getProperty (STUDENT_INFORMATION_URI + "hasProfile");
			stmtItr = _studentDataModel.listStatements (null, p, rAux);
			//Gets the statement. It is assumed that there is a only statement.
			stmt = stmtItr.nextStatement ();
			//Gets the statement subject, which is the Student_Information with the key.
			r = stmt.getSubject ();
			//Sets the belongsToStudent property of Action_Trace resource and adds the value.
			//The belongsToStudent value is the previous obtained Student_Information resource.
			p = ontM.getProperty (STUDENT_TRACE_URI + "belongsToStudent");
			//Creates a new statement.
			stmt = ontM.createStatement (actionTrace, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Sets the initialTime property of Action_Trace resource and adds the value.
			p = ontM.getProperty (STUDENT_TRACE_URI + "initialTime");
			Calendar date = new GregorianCalendar ();
			date.set (log.DateLog.Year, log.DateLog.Month-1, log.DateLog.Day, log.DateLog.Hour, log.DateLog.Minute, log.DateLog.Second);
			date.set (Calendar.MILLISECOND, log.DateLog.Millisecond);
			XSDDateTime t = new XSDDateTime (date);
			actionTrace.addLiteral (p, t);

			//Saves into disk.
			string path = _logsPath + domain.Key + Path.DirectorySeparatorChar;
			//Determine whether the directory exists. If the directory does not exist, it will be created.
			if (!Directory.Exists(path)) {
				Directory.CreateDirectory(path);
			}
			//Specifies the file path.
			string file = path + student.Key + ".owl";
			//Saves the logs into files.
			using (BufferedWriter bw = new BufferedWriter (new OutputStreamWriter(new FileOutputStream(file), "UTF-8"))) {
				string b = "file:" + file;
				//using (OntModel ontTmp = ModelFactory.createOntologyModel (OntModelSpec.OWL_DL_MEM, null)) {
				//ontTmp = _ontologyModel;
				ontM.write (bw, "RDF/XML-ABBREV", b);
				//}
				bw.close ();
			}
		}

		/// <summary>
		/// Adds the log into onto.
		/// </summary>
		/// <param name="log">Log.</param>
		public void AddLogIntoOnto (DepErrorLog log, Student student, DomainActions domain)
		{
			//Creates a auxiliar variable.
			OntModel ontM;
			//Selects the ontology model with the specific domain and student
			var oQuery = 
				from o in _logModels
					where o.Key == (domain.Key + "-" + student.Key)
				select o.Value;
			//Checks if the OntModel exists. If it does not exist, create a new OntModel.
			if (oQuery.ToList ().Count == 0)
				ontM = CreateOntModel (domain.Key, student.Key);
			else
				ontM = oQuery.First ();
			//Add directly the log into the ontology, without any kind of test.
			//Gets the resource Action_Trace.
			Resource r = ontM.getResource (STUDENT_TRACE_URI + "Action_Trace");
			//Gets the type property  of Thing.
			Property p = ontM.getProperty("http://www.w3.org/1999/02/22-rdf-syntax-ns#type");
			//Creates a new Action_Trace resource.
			Resource actionTrace = ontM.createResource (STUDENT_TRACE_URI + "Log" + domain.Key
			                       + student.Key + log.Action.Key + log.FailedDependence.Key
			                       + log.DateLog.ToString ("dd/MM/yyyy HH:mm:ss.fff").Replace (" ", null));
			//Creates a new statement.
			Statement stmt = ontM.createStatement (actionTrace, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the resource Action_Execution_State.
			r = ontM.getResource (STUDENT_STATE_URI + "Action_Execution_State");
			//Creates a new Action_Execution_State resource.
			Resource actionExeState = ontM.createResource (KNOWLEDGE_OBJECT_URI + "ActionExecutionState" + domain.Key
			                          + student.Key + log.Action.Key + log.FailedDependence.Key
			                          + log.DateLog.ToString ("dd/MM/yyyy HH:mm:ss.fff").Replace (" ", null));
			//Creates a new statement.
			stmt = ontM.createStatement (actionExeState, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the resource Punctual_Action_Element.
			r = ontM.getResource (KNOWLEDGE_OBJECT_URI + "Punctual_Action_Element");
			//Creates a new Punctual_Action_Element resource.
			Resource punctualActionElement = ontM.createResource (KNOWLEDGE_OBJECT_URI + "PunctualActionElement"
			                                 + domain.Key + student.Key + log.Action.Key + log.FailedDependence.Key
			                                 + log.DateLog.ToString ("dd/MM/yyyy HH:mm:ss.fff").Replace (" ", null));
			//Creates a new statement.
			stmt = ontM.createStatement (punctualActionElement, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the resource Precondition_State.
			r = ontM.getResource (STUDENT_STATE_URI + "Precondition_State");
			//Creates a new Precondition_State resource.
			Resource preCondState = ontM.createResource (STUDENT_STATE_URI + "PreconditionState"
			                        + domain.Key + student.Key + log.Action.Key + log.FailedDependence.Key
			                        + log.DateLog.ToString ("dd/MM/yyyy HH:mm:ss.fff").Replace (" ", null));
			//Creates a new statement.
			stmt = ontM.createStatement (preCondState, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the resource Condition_On_State.
			r = ontM.getResource (KNOWLEDGE_OBJECT_URI + "Condition_On_State");
			//Creates a new Condition_On_State resource.
			Resource condOnState = ontM.createResource (KNOWLEDGE_OBJECT_URI + "ConditionOnState"
			                       + domain.Key + student.Key + log.Action.Key + log.FailedDependence.Key
			                       + log.DateLog.ToString ("dd/MM/yyyy HH:mm:ss.fff").Replace (" ", null));
			//Creates a new statement.
			stmt = ontM.createStatement (condOnState, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the idObject property of Condition_On_State resource and adds the value.
			//The value is the MessageError key.
			p = ontM.getProperty (KNOWLEDGE_OBJECT_URI + "idObject");
			condOnState.addLiteral (p, log.FailedDependence.DependenceError.Message.Key);
			//Gets the descriptorObject property of Condition_On_State resource and adds the value.
			//The value is the error message.
			p = ontM.getProperty (KNOWLEDGE_OBJECT_URI + "descriptorObject");
			condOnState.addLiteral (p, log.FailedDependence.DependenceError.Message.Message);
			//Gets the conditionStateType property of Condition_On_State resource and adds the value.
			//The value is the IsOrderError value.
			p = ontM.getProperty (KNOWLEDGE_OBJECT_URI + "conditionStateType");
			condOnState.addLiteral (p, log.IsOrderError);
			//Gets the condOnState property of Punctual_Action_Element resource and adds the value.
			p = ontM.getProperty (KNOWLEDGE_OBJECT_URI + "condOnState");
			//Creates a new statement.
			stmt = ontM.createStatement (punctualActionElement, p, condOnState);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the idObject property of Punctual_Action_Element resource and adds the value..K
			//The value is the dependence key.
			p = ontM.getProperty (KNOWLEDGE_OBJECT_URI + "idObject");
			punctualActionElement.addLiteral (p, log.FailedDependence.Key);
			//Gets the descriptorObject property of Punctual_Action_Element resource and adds the value.
			//The value is the type of error.
			p = ontM.getProperty (KNOWLEDGE_OBJECT_URI + "descriptorObject");
			punctualActionElement.addLiteral (p, "dependence");
			//Gets the refersToCondition property of Precondition_State resource and adds the value.
			p = ontM.getProperty (STUDENT_STATE_URI + "refersToCondition");
			//Creates a new statement.
			stmt = ontM.createStatement (preCondState, p, punctualActionElement);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the isMet property of Precondition_State resource and adds the value.
			p = ontM.getProperty (STUDENT_STATE_URI + "isMet");
			preCondState.addLiteral (p, false);
			//Gets the consistsOfPrecondState property of Action_Execution_State resource and adds the value.
			p = ontM.getProperty (STUDENT_STATE_URI + "consistsOfPrecondState");
			//Creates a new statement.
			stmt = ontM.createStatement (actionExeState, p, preCondState);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the wasApplied property of Action_Execution_State resource and adds the value.
			p = ontM.getProperty (STUDENT_STATE_URI + "wasApplied");
			actionExeState.addLiteral (p, log.WasApplied);
			//Sets the associatedActionState property of Action_Trace.
			p = ontM.getProperty (STUDENT_TRACE_URI + "associatedActionState");
			//Creates a new statement.
			stmt = ontM.createStatement (actionTrace, p, actionExeState);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the Punctual_Action whose key is equals to the ActionAplication.
			p = ontM.getProperty (KNOWLEDGE_OBJECT_URI + "idObject");
			StmtIterator stmtItr = _domainDataModel.listStatements (null, p, log.Action.Key + "_" + domain.Key);
			//Gets the statement. It is assumed that there is a only statement.
			stmt = stmtItr.nextStatement ();
			//Gets the statement subject, which is the PunctualAction with the key.
			r = stmt.getSubject ();
			//Sets the associatedSpecificAction property of Action_Trace resource.
			p = ontM.getProperty (STUDENT_TRACE_URI + "associatedSpecificAction");
			//Creates a new statement.
			stmt = ontM.createStatement (actionTrace, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the Student_Profile whose key is equals to the Student.
			p = ontM.getProperty (STUDENT_PROFILE_URI + "idStudent");
			stmtItr = _studentDataModel.listStatements (null, p, student.Key);
			//Gets the statement. It is assumed that there is a only statement.
			stmt = stmtItr.nextStatement ();
			//Gets the statement subject, which is the Student_Profile with the key.
			Resource rAux = stmt.getSubject ();
			//Sets the Student_Information which has the previous Student_Profile.
			p = ontM.getProperty (STUDENT_INFORMATION_URI + "hasProfile");
			stmtItr = _studentDataModel.listStatements (null, p, rAux);
			//Gets the statement. It is assumed that there is a only statement.
			stmt = stmtItr.nextStatement ();
			//Gets the statement subject, which is the Student_Information with the key.
			r = stmt.getSubject ();
			//Sets the belongsToStudent property of Action_Trace resource and adds the value.
			//The belongsToStudent value is the previous obtained Student_Information resource.
			p = ontM.getProperty (STUDENT_TRACE_URI + "belongsToStudent");
			//Creates a new statement.
			stmt = ontM.createStatement (actionTrace, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Sets the initialTime property of Action_Trace resource and adds the value.
			p = ontM.getProperty (STUDENT_TRACE_URI + "initialTime");
			Calendar date = new GregorianCalendar ();
			date.set (log.DateLog.Year, log.DateLog.Month-1, log.DateLog.Day, log.DateLog.Hour, log.DateLog.Minute, log.DateLog.Second);
			date.set (Calendar.MILLISECOND, log.DateLog.Millisecond);
			XSDDateTime t = new XSDDateTime (date);
			actionTrace.addLiteral (p, t);

			//Saves into disk.
			string path = _logsPath + domain.Key + Path.DirectorySeparatorChar;
			//Determine whether the directory exists. If the directory does not exist, it will be created.
			if (!Directory.Exists(path)) {
				Directory.CreateDirectory(path);
			}
			//Specifies the file path.
			string file = path + student.Key + ".owl";
			//Saves the logs into files.
			using (BufferedWriter bw = new BufferedWriter (new OutputStreamWriter(new FileOutputStream(file), "UTF-8"))) {
				string b = "file:" + file;
				//using (OntModel ontTmp = ModelFactory.createOntologyModel (OntModelSpec.OWL_DL_MEM, null)) {
				//ontTmp = _ontologyModel;
				ontM.write (bw, "RDF/XML-ABBREV", b);
				//}
				bw.close ();
			}
		}

		/// <summary>
		/// Adds the log into onto.
		/// </summary>
		/// <param name="log">Log.</param>
		public void AddLogIntoOnto (IncompErrorLog log, Student student, DomainActions domain)
		{
			//Creates a auxiliar variable.
			OntModel ontM;
			//Selects the ontology model with the specific domain and student
			var oQuery = 
				from o in _logModels
					where o.Key == (domain.Key + "-" + student.Key)
				select o.Value;
			//Checks if the OntModel exists. If it does not exist, create a new OntModel.
			if (oQuery.ToList ().Count == 0)
				ontM = CreateOntModel (domain.Key, student.Key);
			else
				ontM = oQuery.First ();
			//Add directly the log into the ontology, without any kind of test.
			//Gets the resource Action_Trace.
			Resource r = ontM.getResource (STUDENT_TRACE_URI + "Action_Trace");
			//Gets the type property  of Thing.
			Property p = ontM.getProperty("http://www.w3.org/1999/02/22-rdf-syntax-ns#type");
			//Creates a new Action_Trace resource.
			Resource actionTrace = ontM.createResource (STUDENT_TRACE_URI + "Log" + domain.Key
			                       + student.Key + log.Action.Key + log.FailedIncompatibility.Key
			                       + log.DateLog.ToString ("dd/MM/yyyy HH:mm:ss.fff").Replace (" ", null));
			//Creates a new statement.
			Statement stmt = ontM.createStatement (actionTrace, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the resource Action_Execution_State.
			r = ontM.getResource (STUDENT_STATE_URI + "Action_Execution_State");
			//Creates a new Action_Execution_State resource.
			Resource actionExeState = ontM.createResource (KNOWLEDGE_OBJECT_URI + "ActionExecutionState" + domain.Key
			                          + student.Key + log.Action.Key + log.FailedIncompatibility.Key
			                          + log.DateLog.ToString ("dd/MM/yyyy HH:mm:ss.fff").Replace (" ", null));
			//Creates a new statement.
			stmt = ontM.createStatement (actionExeState, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the resource Punctual_Action_Element.
			r = ontM.getResource (KNOWLEDGE_OBJECT_URI + "Punctual_Action_Element");
			//Creates a new Punctual_Action_Element resource.
			Resource punctualActionElement = ontM.createResource (KNOWLEDGE_OBJECT_URI + "PunctualActionElement"
			                                 + domain.Key + student.Key + log.Action.Key + log.FailedIncompatibility.Key
			                                 + log.DateLog.ToString ("dd/MM/yyyy HH:mm:ss.fff").Replace (" ", null));
			//Creates a new statement.
			stmt = ontM.createStatement (punctualActionElement, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the resource Precondition_State.
			r = ontM.getResource (STUDENT_STATE_URI + "Precondition_State");
			//Creates a new Precondition_State resource.
			Resource preCondState = ontM.createResource (STUDENT_STATE_URI + "PreconditionState"
			                        + domain.Key + student.Key + log.Action.Key + log.FailedIncompatibility.Key
									+ log.DateLog.ToString ("dd/MM/yyyy HH:mm:ss.fff").Replace (" ", null));
			//Creates a new statement.
			stmt = ontM.createStatement (preCondState, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the resource Condition_On_State.
			r = ontM.getResource (KNOWLEDGE_OBJECT_URI + "Condition_On_State");
			//Creates a new Condition_On_State resource.
			Resource condOnState = ontM.createResource (KNOWLEDGE_OBJECT_URI + "ConditionOnState"
			                       + domain.Key + student.Key + log.Action.Key + log.FailedIncompatibility.Key
			                       + log.DateLog.ToString ("dd/MM/yyyy HH:mm:ss.fff").Replace (" ", null));
			//Creates a new statement.
			stmt = ontM.createStatement (condOnState, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the idObject property of Condition_On_State resource and adds the value.
			//The value is the MessageError key.
			p = ontM.getProperty (KNOWLEDGE_OBJECT_URI + "idObject");
			condOnState.addLiteral (p, log.FailedIncompatibility.IncompatibilityError.Message.Key);
			//Gets the descriptorObject property of Condition_On_State resource and adds the value.
			//The value is the error message.
			p = ontM.getProperty (KNOWLEDGE_OBJECT_URI + "descriptorObject");
			condOnState.addLiteral (p, log.FailedIncompatibility.IncompatibilityError.Message.Message);
			//Gets the condOnState property of Punctual_Action_Element resource and adds the value.
			p = ontM.getProperty (KNOWLEDGE_OBJECT_URI + "condOnState");
			//Creates a new statement.
			stmt = ontM.createStatement (punctualActionElement, p, condOnState);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the idObject property of Punctual_Action_Element resource and adds the value.
			//The value is the incompatibility key.
			p = ontM.getProperty (KNOWLEDGE_OBJECT_URI + "idObject");
			punctualActionElement.addLiteral (p, log.FailedIncompatibility.Key);
			//Gets the descriptorObject property of Punctual_Action_Element resource and adds the value.
			//The value is the type of error.
			p = ontM.getProperty (KNOWLEDGE_OBJECT_URI + "descriptorObject");
			punctualActionElement.addLiteral (p, "incompatibility");
			//Gets the refersToCondition property of Precondition_State resource and adds the value.
			p = ontM.getProperty (STUDENT_STATE_URI + "refersToCondition");
			//Creates a new statement.
			stmt = ontM.createStatement (preCondState, p, punctualActionElement);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the isMet property of Precondition_State resource and adds the value.
			p = ontM.getProperty (STUDENT_STATE_URI + "isMet");
			preCondState.addLiteral (p, false);
			//Gets the consistsOfPrecondState property of Action_Execution_State resource and adds the value.
			p = ontM.getProperty (STUDENT_STATE_URI + "consistsOfPrecondState");
			//Creates a new statement.
			stmt = ontM.createStatement (actionExeState, p, preCondState);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the wasApplied property of Action_Execution_State resource and adds the value.
			p = ontM.getProperty (STUDENT_STATE_URI + "wasApplied");
			actionExeState.addLiteral (p, log.WasApplied);
			//Sets the associatedActionState property of Action_Trace.
			p = ontM.getProperty (STUDENT_TRACE_URI + "associatedActionState");
			//Creates a new statement.
			stmt = ontM.createStatement (actionTrace, p, actionExeState);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the Punctual_Action whose key is equals to the ActionAplication.
			p = ontM.getProperty (KNOWLEDGE_OBJECT_URI + "idObject");
			StmtIterator stmtItr = _domainDataModel.listStatements (null, p, log.Action.Key + "_" + domain.Key);
			//Gets the statement. It is assumed that there is a only statement.
			stmt = stmtItr.nextStatement ();
			//Gets the statement subject, which is the PunctualAction with the key.
			r = stmt.getSubject ();
			//Sets the associatedSpecificAction property of Action_Trace resource.
			p = ontM.getProperty (STUDENT_TRACE_URI + "associatedSpecificAction");
			//Creates a new statement.
			stmt = ontM.createStatement (actionTrace, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the Student_Profile whose key is equals to the Student.
			p = ontM.getProperty (STUDENT_PROFILE_URI + "idStudent");
			stmtItr = _studentDataModel.listStatements (null, p, student.Key);
			//Gets the statement. It is assumed that there is a only statement.
			stmt = stmtItr.nextStatement ();
			//Gets the statement subject, which is the Student_Profile with the key.
			Resource rAux = stmt.getSubject ();
			//Sets the Student_Information which has the previous Student_Profile.
			p = ontM.getProperty (STUDENT_INFORMATION_URI + "hasProfile");
			stmtItr = _studentDataModel.listStatements (null, p, rAux);
			//Gets the statement. It is assumed that there is a only statement.
			stmt = stmtItr.nextStatement ();
			//Gets the statement subject, which is the Student_Information with the key.
			r = stmt.getSubject ();
			//Sets the belongsToStudent property of Action_Trace resource and adds the value.
			//The belongsToStudent value is the previous obtained Student_Information resource.
			p = ontM.getProperty (STUDENT_TRACE_URI + "belongsToStudent");
			//Creates a new statement.
			stmt = ontM.createStatement (actionTrace, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Sets the initialTime property of Action_Trace resource and adds the value.
			p = ontM.getProperty (STUDENT_TRACE_URI + "initialTime");
			Calendar date = new GregorianCalendar ();
			date.set (log.DateLog.Year, log.DateLog.Month-1, log.DateLog.Day, log.DateLog.Hour, log.DateLog.Minute, log.DateLog.Second);
			date.set (Calendar.MILLISECOND, log.DateLog.Millisecond);
			XSDDateTime t = new XSDDateTime (date);
			actionTrace.addLiteral (p, t);

			//Saves into disk.
			string path = _logsPath + domain.Key + Path.DirectorySeparatorChar;
			//Determine whether the directory exists. If the directory does not exist, it will be created.
			if (!Directory.Exists(path)) {
				Directory.CreateDirectory(path);
			}
			//Specifies the file path.
			string file = path + student.Key + ".owl";
			//Saves the logs into files.
			using (BufferedWriter bw = new BufferedWriter (new OutputStreamWriter(new FileOutputStream(file), "UTF-8"))) {
				string b = "file:" + file;
				//using (OntModel ontTmp = ModelFactory.createOntologyModel (OntModelSpec.OWL_DL_MEM, null)) {
				//ontTmp = _ontologyModel;
				ontM.write (bw, "RDF/XML-ABBREV", b);
				//}
				bw.close ();
			}
		}

		/// <summary>
		/// Adds the log into onto.
		/// </summary>
		/// <param name="log">Log.</param>
		public void AddLogIntoOnto (MinTimeErrorLog log, Student student, DomainActions domain)
		{
			//Creates a auxiliar variable.
			OntModel ontM;
			//Selects the ontology model with the specific domain and student
			var oQuery = 
				from o in _logModels
					where o.Key == (domain.Key + "-" + student.Key)
				select o.Value;
			//Checks if the OntModel exists. If it does not exist, create a new OntModel.
			if (oQuery.ToList ().Count == 0)
				ontM = CreateOntModel (domain.Key, student.Key);
			else
				ontM = oQuery.First ();
			//Add directly the log into the ontology, without any kind of test.
			//Gets the resource Action_Trace.
			Resource r = ontM.getResource (STUDENT_TRACE_URI + "Action_Trace");
			//Gets the type property  of Thing.
			Property p = ontM.getProperty("http://www.w3.org/1999/02/22-rdf-syntax-ns#type");
			//Creates a new Action_Trace resource.
			Resource actionTrace = ontM.createResource (STUDENT_TRACE_URI + "Log" + domain.Key
			                       + student.Key + log.Action.Key
			                       + log.DateLog.ToString ("dd/MM/yyyy HH:mm:ss.fff").Replace (" ", null));
			//Creates a new statement.
			Statement stmt = ontM.createStatement (actionTrace, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the resource Action_Execution_State.
			r = ontM.getResource (STUDENT_STATE_URI + "Action_Execution_State");
			//Creates a new Action_Execution_State resource.
			Resource actionExeState = ontM.createResource (KNOWLEDGE_OBJECT_URI + "ActionExecutionState" + domain.Key
			                          + student.Key + log.Action.Key
			                          + log.DateLog.ToString ("dd/MM/yyyy HH:mm:ss.fff").Replace (" ", null));
			//Creates a new statement.
			stmt = ontM.createStatement (actionExeState, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the resource Punctual_Action_Element.
			r = ontM.getResource (KNOWLEDGE_OBJECT_URI + "Punctual_Action_Element");
			//Creates a new Punctual_Action_Element resource.
			Resource punctualActionElement = ontM.createResource (KNOWLEDGE_OBJECT_URI + "PunctualActionElement"
			                                 + domain.Key + student.Key + log.Action.Key
			                                 + log.DateLog.ToString ("dd/MM/yyyy HH:mm:ss.fff").Replace (" ", null));
			//Creates a new statement.
			stmt = ontM.createStatement (punctualActionElement, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the resource Precondition_State.
			r = ontM.getResource (STUDENT_STATE_URI + "Precondition_State");
			//Creates a new Precondition_State resource.
			Resource preCondState = ontM.createResource (STUDENT_STATE_URI + "PreconditionState"
			                        + domain.Key + student.Key + log.Action.Key
			                        + log.DateLog.ToString ("dd/MM/yyyy HH:mm:ss.fff").Replace (" ", null));
			//Creates a new statement.
			stmt = ontM.createStatement (preCondState, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the resource Condition_On_State.
			r = ontM.getResource (KNOWLEDGE_OBJECT_URI + "Condition_On_State");
			//Creates a new Condition_On_State resource.
			Resource condOnState = ontM.createResource (KNOWLEDGE_OBJECT_URI + "ConditionOnState"
			                       + domain.Key + student.Key + log.Action.Key
			                       + log.DateLog.ToString ("dd/MM/yyyy HH:mm:ss.fff").Replace (" ", null));
			//Creates a new statement.
			stmt = ontM.createStatement (condOnState, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the idObject property of Condition_On_State resource and adds the value.
			//The value is the MessageError key.
			p = ontM.getProperty (KNOWLEDGE_OBJECT_URI + "idObject");
			condOnState.addLiteral (p, log.Action.MinTimeError.Message.Key);
			//Gets the descriptorObject property of Condition_On_State resource and adds the value.
			//The value is the error message.
			p = ontM.getProperty (KNOWLEDGE_OBJECT_URI + "descriptorObject");
			condOnState.addLiteral (p, log.Action.MinTimeError.Message.Message);
			//Gets the condOnState property of Punctual_Action_Element resource and adds the value.
			p = ontM.getProperty (KNOWLEDGE_OBJECT_URI + "condOnState");
			//Creates a new statement.
			stmt = ontM.createStatement (punctualActionElement, p, condOnState);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the idObject property of Punctual_Action_Element resource and adds the value.
			//The value is the minTimeError key.
			p = ontM.getProperty (KNOWLEDGE_OBJECT_URI + "idObject");
			punctualActionElement.addLiteral (p, log.Action.MinTimeError.Key);
			//Gets the descriptorObject property of Punctual_Action_Element resource and adds the value.
			//The value is the type of error.
			p = ontM.getProperty (KNOWLEDGE_OBJECT_URI + "descriptorObject");
			punctualActionElement.addLiteral (p, "minTime");
			//Gets the refersToCondition property of Precondition_State resource and adds the value.
			p = ontM.getProperty (STUDENT_STATE_URI + "refersToCondition");
			//Creates a new statement.
			stmt = ontM.createStatement (preCondState, p, punctualActionElement);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the isMet property of Precondition_State resource and adds the value.
			p = ontM.getProperty (STUDENT_STATE_URI + "isMet");
			preCondState.addLiteral (p, false);
			//Gets the consistsOfPrecondState property of Action_Execution_State resource and adds the value.
			p = ontM.getProperty (STUDENT_STATE_URI + "consistsOfPrecondState");
			//Creates a new statement.
			stmt = ontM.createStatement (actionExeState, p, preCondState);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the wasApplied property of Action_Execution_State resource and adds the value.
			p = ontM.getProperty (STUDENT_STATE_URI + "wasApplied");
			actionExeState.addLiteral (p, log.WasApplied);
			//Sets the associatedActionState property of Action_Trace.
			p = ontM.getProperty (STUDENT_TRACE_URI + "associatedActionState");
			//Creates a new statement.
			stmt = ontM.createStatement (actionTrace, p, actionExeState);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the Punctual_Action whose key is equals to the ActionAplication.
			p = ontM.getProperty (KNOWLEDGE_OBJECT_URI + "idObject");
			StmtIterator stmtItr = _domainDataModel.listStatements (null, p, log.Action.Key + "_" + domain.Key);
			//Gets the statement. It is assumed that there is a only statement.
			stmt = stmtItr.nextStatement ();
			//Gets the statement subject, which is the PunctualAction with the key.
			r = stmt.getSubject ();
			//Sets the associatedSpecificAction property of Action_Trace resource.
			p = ontM.getProperty (STUDENT_TRACE_URI + "associatedSpecificAction");
			//Creates a new statement.
			stmt = ontM.createStatement (actionTrace, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the Student_Profile whose key is equals to the Student.
			p = ontM.getProperty (STUDENT_PROFILE_URI + "idStudent");
			stmtItr = _studentDataModel.listStatements (null, p, student.Key);
			//Gets the statement. It is assumed that there is a only statement.
			stmt = stmtItr.nextStatement ();
			//Gets the statement subject, which is the Student_Profile with the key.
			Resource rAux = stmt.getSubject ();
			//Sets the Student_Information which has the previous Student_Profile.
			p = ontM.getProperty (STUDENT_INFORMATION_URI + "hasProfile");
			stmtItr = _studentDataModel.listStatements (null, p, rAux);
			//Gets the statement. It is assumed that there is a only statement.
			stmt = stmtItr.nextStatement ();
			//Gets the statement subject, which is the Student_Information with the key.
			r = stmt.getSubject ();
			//Sets the belongsToStudent property of Action_Trace resource and adds the value.
			//The belongsToStudent value is the previous obtained Student_Information resource.
			p = ontM.getProperty (STUDENT_TRACE_URI + "belongsToStudent");
			//Creates a new statement.
			stmt = ontM.createStatement (actionTrace, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Sets the initialTime property of Action_Trace resource and adds the value.
			p = ontM.getProperty (STUDENT_TRACE_URI + "initialTime");
			Calendar date = new GregorianCalendar ();
			date.set (log.DateLog.Year, log.DateLog.Month-1, log.DateLog.Day, log.DateLog.Hour, log.DateLog.Minute, log.DateLog.Second);
			date.set (Calendar.MILLISECOND, log.DateLog.Millisecond);
			XSDDateTime t = new XSDDateTime (date);
			actionTrace.addLiteral (p, t);

			//Saves into disk.
			string path = _logsPath + domain.Key + Path.DirectorySeparatorChar;
			//Determine whether the directory exists. If the directory does not exist, it will be created.
			if (!Directory.Exists(path)) {
				Directory.CreateDirectory(path);
			}
			//Specifies the file path.
			string file = path + student.Key + ".owl";
			//Saves the logs into files.
			using (BufferedWriter bw = new BufferedWriter (new OutputStreamWriter(new FileOutputStream(file), "UTF-8"))) {
				string b = "file:" + file;
				//using (OntModel ontTmp = ModelFactory.createOntologyModel (OntModelSpec.OWL_DL_MEM, null)) {
				//ontTmp = _ontologyModel;
				ontM.write (bw, "RDF/XML-ABBREV", b);
				//}
				bw.close ();
			}
		}

		/// <summary>
		/// Adds the log into onto.
		/// </summary>
		/// <param name="log">Log.</param>
		public void AddLogIntoOnto (MaxTimeErrorLog log, Student student, DomainActions domain)
		{
			//Creates a auxiliar variable.
			OntModel ontM;
			//Selects the ontology model with the specific domain and student
			var oQuery = 
				from o in _logModels
					where o.Key == (domain.Key + "-" + student.Key)
				select o.Value;
			//Checks if the OntModel exists. If it does not exist, create a new OntModel.
			if (oQuery.ToList ().Count == 0)
				ontM = CreateOntModel (domain.Key, student.Key);
			else
				ontM = oQuery.First ();
			//Add directly the log into the ontology, without any kind of test.
			//Gets the resource Action_Trace.
			Resource r = ontM.getResource (STUDENT_TRACE_URI + "Action_Trace");
			//Gets the type property  of Thing.
			Property p = ontM.getProperty("http://www.w3.org/1999/02/22-rdf-syntax-ns#type");
			//Creates a new Action_Trace resource.
			Resource actionTrace = ontM.createResource (STUDENT_TRACE_URI + "Log" + domain.Key
			                       + student.Key + log.Action.Key
			                       + log.DateLog.ToString ("dd/MM/yyyy HH:mm:ss.fff").Replace (" ", null));
			//Creates a new statement.
			Statement stmt = ontM.createStatement (actionTrace, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the resource Action_Execution_State.
			r = ontM.getResource (STUDENT_STATE_URI + "Action_Execution_State");
			//Creates a new Action_Execution_State resource.
			Resource actionExeState = ontM.createResource (KNOWLEDGE_OBJECT_URI + "ActionExecutionState" + domain.Key
			                          + student.Key + log.Action.Key
			                          + log.DateLog.ToString ("dd/MM/yyyy HH:mm:ss.fff").Replace (" ", null));
			//Creates a new statement.
			stmt = ontM.createStatement (actionExeState, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the resource Punctual_Action_Element.
			r = ontM.getResource (KNOWLEDGE_OBJECT_URI + "Punctual_Action_Element");
			//Creates a new Punctual_Action_Element resource.
			Resource punctualActionElement = ontM.createResource (KNOWLEDGE_OBJECT_URI + "PunctualActionElement"
			                                 + domain.Key + student.Key + log.Action.Key
			                                 + log.DateLog.ToString ("dd/MM/yyyy HH:mm:ss.fff").Replace (" ", null));
			//Creates a new statement.
			stmt = ontM.createStatement (punctualActionElement, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the resource Precondition_State.
			r = ontM.getResource (STUDENT_STATE_URI + "Precondition_State");
			//Creates a new Precondition_State resource.
			Resource preCondState = ontM.createResource (STUDENT_STATE_URI + "PreconditionState"
			                        + domain.Key + student.Key + log.Action.Key
			                        + log.DateLog.ToString ("dd/MM/yyyy HH:mm:ss.fff").Replace (" ", null));
			//Creates a new statement.
			stmt = ontM.createStatement (preCondState, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the resource Condition_On_State.
			r = ontM.getResource (KNOWLEDGE_OBJECT_URI + "Condition_On_State");
			//Creates a new Condition_On_State resource.
			Resource condOnState = ontM.createResource (KNOWLEDGE_OBJECT_URI + "ConditionOnState"
			                       + domain.Key + student.Key + log.Action.Key
			                       + log.DateLog.ToString ("dd/MM/yyyy HH:mm:ss.fff").Replace (" ", null));
			//Creates a new statement.
			stmt = ontM.createStatement (condOnState, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the idObject property of Condition_On_State resource and adds the value.
			//The value is the MessageError key.
			p = ontM.getProperty (KNOWLEDGE_OBJECT_URI + "idObject");
			condOnState.addLiteral (p, log.Action.MaxTimeError.Message.Key);
			//Gets the descriptorObject property of Condition_On_State resource and adds the value.
			//The value is the error message.
			p = ontM.getProperty (KNOWLEDGE_OBJECT_URI + "descriptorObject");
			condOnState.addLiteral (p, log.Action.MaxTimeError.Message.Message);
			//Gets the condOnState property of Punctual_Action_Element resource and adds the value.
			p = ontM.getProperty (KNOWLEDGE_OBJECT_URI + "condOnState");
			//Creates a new statement.
			stmt = ontM.createStatement (punctualActionElement, p, condOnState);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the idObject property of Punctual_Action_Element resource and adds the value.
			//The value is the maxTimeError key.
			p = ontM.getProperty (KNOWLEDGE_OBJECT_URI + "idObject");
			punctualActionElement.addLiteral (p, log.Action.MaxTimeError.Key);
			//Gets the descriptorObject property of Punctual_Action_Element resource and adds the value.
			//The value is the type of error.
			p = ontM.getProperty (KNOWLEDGE_OBJECT_URI + "descriptorObject");
			punctualActionElement.addLiteral (p, "maxTime");
			//Gets the refersToCondition property of Precondition_State resource and adds the value.
			p = ontM.getProperty (STUDENT_STATE_URI + "refersToCondition");
			//Creates a new statement.
			stmt = ontM.createStatement (preCondState, p, punctualActionElement);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the isMet property of Precondition_State resource and adds the value.
			p = ontM.getProperty (STUDENT_STATE_URI + "isMet");
			preCondState.addLiteral (p, false);
			//Gets the consistsOfPrecondState property of Action_Execution_State resource and adds the value.
			p = ontM.getProperty (STUDENT_STATE_URI + "consistsOfPrecondState");
			//Creates a new statement.
			stmt = ontM.createStatement (actionExeState, p, preCondState);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the wasApplied property of Action_Execution_State resource and adds the value.
			p = ontM.getProperty (STUDENT_STATE_URI + "wasApplied");
			actionExeState.addLiteral (p, log.WasApplied);
			//Sets the associatedActionState property of Action_Trace.
			p = ontM.getProperty (STUDENT_TRACE_URI + "associatedActionState");
			//Creates a new statement.
			stmt = ontM.createStatement (actionTrace, p, actionExeState);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the Punctual_Action whose key is equals to the ActionAplication.
			p = ontM.getProperty (KNOWLEDGE_OBJECT_URI + "idObject");
			StmtIterator stmtItr = _domainDataModel.listStatements (null, p, log.Action.Key + "_" + domain.Key);
			//Gets the statement. It is assumed that there is a only statement.
			stmt = stmtItr.nextStatement ();
			//Gets the statement subject, which is the PunctualAction with the key.
			r = stmt.getSubject ();
			//Sets the associatedSpecificAction property of Action_Trace resource.
			p = ontM.getProperty (STUDENT_TRACE_URI + "associatedSpecificAction");
			//Creates a new statement.
			stmt = ontM.createStatement (actionTrace, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the Student_Profile whose key is equals to the Student.
			p = ontM.getProperty (STUDENT_PROFILE_URI + "idStudent");
			stmtItr = _studentDataModel.listStatements (null, p, student.Key);
			//Gets the statement. It is assumed that there is a only statement.
			stmt = stmtItr.nextStatement ();
			//Gets the statement subject, which is the Student_Profile with the key.
			Resource rAux = stmt.getSubject ();
			//Sets the Student_Information which has the previous Student_Profile.
			p = ontM.getProperty (STUDENT_INFORMATION_URI + "hasProfile");
			stmtItr = _studentDataModel.listStatements (null, p, rAux);
			//Gets the statement. It is assumed that there is a only statement.
			stmt = stmtItr.nextStatement ();
			//Gets the statement subject, which is the Student_Information with the key.
			r = stmt.getSubject ();
			//Sets the belongsToStudent property of Action_Trace resource and adds the value.
			//The belongsToStudent value is the previous obtained Student_Information resource.
			p = ontM.getProperty (STUDENT_TRACE_URI + "belongsToStudent");
			//Creates a new statement.
			stmt = ontM.createStatement (actionTrace, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Sets the initialTime property of Action_Trace resource and adds the value.
			p = ontM.getProperty (STUDENT_TRACE_URI + "initialTime");
			Calendar date = new GregorianCalendar ();
			date.set (log.DateLog.Year, log.DateLog.Month-1, log.DateLog.Day, log.DateLog.Hour, log.DateLog.Minute, log.DateLog.Second);
			date.set (Calendar.MILLISECOND, log.DateLog.Millisecond);
			XSDDateTime t = new XSDDateTime (date);
			actionTrace.addLiteral (p, t);

			//Saves into disk.
			string path = _logsPath + domain.Key + Path.DirectorySeparatorChar;
			//Determine whether the directory exists. If the directory does not exist, it will be created.
			if (!Directory.Exists(path)) {
				Directory.CreateDirectory(path);
			}
			//Specifies the file path.
			string file = path + student.Key + ".owl";
			//Saves the logs into files.
			using (BufferedWriter bw = new BufferedWriter (new OutputStreamWriter(new FileOutputStream(file), "UTF-8"))) {
				string b = "file:" + file;
				//using (OntModel ontTmp = ModelFactory.createOntologyModel (OntModelSpec.OWL_DL_MEM, null)) {
				//ontTmp = _ontologyModel;
				ontM.write (bw, "RDF/XML-ABBREV", b);
				//}
				bw.close ();
			}
		}

		/// <summary>
		/// Adds the log into onto.
		/// </summary>
		/// <param name="log">Log.</param>
		public void AddLogIntoOnto (WorldErrorLog log, Student student, DomainActions domain)
		{
			//Creates a auxiliar variable.
			OntModel ontM;
			//Selects the ontology model with the specific domain and student
			var oQuery = 
				from o in _logModels
					where o.Key == (domain.Key + "-" + student.Key)
				select o.Value;
			//Checks if the OntModel exists. If it does not exist, create a new OntModel.
			if (oQuery.ToList ().Count == 0)
				ontM = CreateOntModel (domain.Key, student.Key);
			else
				ontM = oQuery.First ();
			//Add directly the log into the ontology, without any kind of test.
			//Gets the resource Action_Trace.
			Resource r = ontM.getResource (STUDENT_TRACE_URI + "Action_Trace");
			//Gets the type property  of Thing.
			Property p = ontM.getProperty("http://www.w3.org/1999/02/22-rdf-syntax-ns#type");
			//Creates a new Action_Trace resource.
			Resource actionTrace = ontM.createResource (STUDENT_TRACE_URI + "Log" + domain.Key
			                       + student.Key + log.Action.Key
			                       + log.DateLog.ToString ("dd/MM/yyyy HH:mm:ss.fff").Replace (" ", null));
			//Creates a new statement.
			Statement stmt = ontM.createStatement (actionTrace, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the resource Action_Execution_State.
			r = ontM.getResource (STUDENT_STATE_URI + "Action_Execution_State");
			//Creates a new Action_Execution_State resource.
			Resource actionExeState = ontM.createResource (KNOWLEDGE_OBJECT_URI + "ActionExecutionState" + domain.Key
			                          + student.Key + log.Action.Key
			                          + log.DateLog.ToString ("dd/MM/yyyy HH:mm:ss.fff").Replace (" ", null));
			//Creates a new statement.
			stmt = ontM.createStatement (actionExeState, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the resource Punctual_Action_Element.
			r = ontM.getResource (KNOWLEDGE_OBJECT_URI + "Punctual_Action_Element");
			//Creates a new Punctual_Action_Element resource.
			Resource punctualActionElement = ontM.createResource (KNOWLEDGE_OBJECT_URI + "PunctualActionElement"
			                                 + domain.Key + student.Key + log.Action.Key
			                                 + log.DateLog.ToString ("dd/MM/yyyy HH:mm:ss.fff").Replace (" ", null));
			//Creates a new statement.
			stmt = ontM.createStatement (punctualActionElement, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the resource Precondition_State.
			r = ontM.getResource (STUDENT_STATE_URI + "Precondition_State");
			//Creates a new Precondition_State resource.
			Resource preCondState = ontM.createResource (STUDENT_STATE_URI + "PreconditionState"
			                        + domain.Key + student.Key + log.Action.Key
			                        + log.DateLog.ToString ("dd/MM/yyyy HH:mm:ss.fff").Replace (" ", null));
			//Creates a new statement.
			stmt = ontM.createStatement (preCondState, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the resource Condition_On_State.
			r = ontM.getResource (KNOWLEDGE_OBJECT_URI + "Condition_On_State");
			//Creates a new Condition_On_State resource.
			Resource condOnState = _worldDataModel.createResource (KNOWLEDGE_OBJECT_URI + log.ErrorAssociated.Message.Key);
			//Gets the Condition_On_State whose key is equals to the ErrorMessage key.
			p = ontM.getProperty (KNOWLEDGE_OBJECT_URI + "idObject");
			StmtIterator stmtItr = _worldDataModel.listStatements (null, p, log.ErrorAssociated.Message.Key);
			//Gets the statement. It is assumed that there is a only statement.
			try {
				stmt = stmtItr.nextStatement ();
				//Gets the statement subject, which is the Condition_On_State with the key.
				condOnState = stmt.getSubject ();
			}
			//If does not exist the statement, it creates a new statement.
			catch (NoSuchElementException e) {
				//Gets the type property  of Thing.
				p = ontM.getProperty("http://www.w3.org/1999/02/22-rdf-syntax-ns#type");
				//Creates a new statement.
				stmt = _worldDataModel.createStatement (condOnState, p, r);
				//Adds the statement into the ontologyModel.
				_worldDataModel.add (stmt);
				//Gets the idObject property of Condition_On_State resource and adds the value.
				//The value is the MessageError key.
				p = ontM.getProperty (KNOWLEDGE_OBJECT_URI + "idObject");
				condOnState.addLiteral (p, log.ErrorAssociated.Message.Key);
				//Gets the descriptorObject property of Condition_On_State resource and adds the value.
				//The value is the error message.
				p = ontM.getProperty (KNOWLEDGE_OBJECT_URI + "descriptorObject");
				condOnState.addLiteral (p, log.ErrorAssociated.Message.Message);
				//Gets the conditionStateType property of Condition_On_State resource and adds the value.
				//The value is the type of world error.
				p = ontM.getProperty (KNOWLEDGE_OBJECT_URI + "conditionStateType");
				condOnState.addLiteral (p, log.Type);
			}
			//Gets the condOnState property of Punctual_Action_Element resource and adds the value.
			p = ontM.getProperty (KNOWLEDGE_OBJECT_URI + "condOnState");
			//Creates a new statement.
			stmt = ontM.createStatement (punctualActionElement, p, condOnState);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the idObject property of Punctual_Action_Element resource and adds the value.
			//The value is the error associated key.
			p = ontM.getProperty (KNOWLEDGE_OBJECT_URI + "idObject");
			punctualActionElement.addLiteral (p, log.ErrorAssociated.Key);
			//Gets the descriptorObject property of Punctual_Action_Element resource and adds the value.
			//The value is the type of error.
			p = ontM.getProperty (KNOWLEDGE_OBJECT_URI + "descriptorObject");
			punctualActionElement.addLiteral (p, "world");
			//Gets the refersToCondition property of Precondition_State resource and adds the value.
			p = ontM.getProperty (STUDENT_STATE_URI + "refersToCondition");
			//Creates a new statement.
			stmt = ontM.createStatement (preCondState, p, punctualActionElement);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the isMet property of Precondition_State resource and adds the value.
			p = ontM.getProperty (STUDENT_STATE_URI + "isMet");
			preCondState.addLiteral (p, false);
			//Gets the consistsOfPrecondState property of Action_Execution_State resource and adds the value.
			p = ontM.getProperty (STUDENT_STATE_URI + "consistsOfPrecondState");
			//Creates a new statement.
			stmt = ontM.createStatement (actionExeState, p, preCondState);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the wasApplied property of Action_Execution_State resource and adds the value.
			p = ontM.getProperty (STUDENT_STATE_URI + "wasApplied");
			actionExeState.addLiteral (p, log.WasApplied);
			//Sets the associatedActionState property of Action_Trace.
			p = ontM.getProperty (STUDENT_TRACE_URI + "associatedActionState");
			//Creates a new statement.
			stmt = ontM.createStatement (actionTrace, p, actionExeState);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the Punctual_Action whose key is equals to the ActionAplication.
			p = ontM.getProperty (KNOWLEDGE_OBJECT_URI + "idObject");
			stmtItr = _domainDataModel.listStatements (null, p, log.Action.Key  + "_" + domain.Key);
			//Gets the statement. It is assumed that there is a only statement.
			stmt = stmtItr.nextStatement ();
			//Gets the statement subject, which is the PunctualAction with the key.
			r = stmt.getSubject ();
			//Sets the associatedSpecificAction property of Action_Trace resource.
			p = ontM.getProperty (STUDENT_TRACE_URI + "associatedSpecificAction");
			//Creates a new statement.
			stmt = ontM.createStatement (actionTrace, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the Student_Profile whose key is equals to the Student.
			p = ontM.getProperty (STUDENT_PROFILE_URI + "idStudent");
			stmtItr = _studentDataModel.listStatements (null, p, student.Key);
			//Gets the statement. It is assumed that there is a only statement.
			stmt = stmtItr.nextStatement ();
			//Gets the statement subject, which is the Student_Profile with the key.
			Resource rAux = stmt.getSubject ();
			//Sets the Student_Information which has the previous Student_Profile.
			p = ontM.getProperty (STUDENT_INFORMATION_URI + "hasProfile");
			stmtItr = _studentDataModel.listStatements (null, p, rAux);
			//Gets the statement. It is assumed that there is a only statement.
			stmt = stmtItr.nextStatement ();
			//Gets the statement subject, which is the Student_Information with the key.
			r = stmt.getSubject ();
			//Sets the belongsToStudent property of Action_Trace resource and adds the value.
			//The belongsToStudent value is the previous obtained Student_Information resource.
			p = ontM.getProperty (STUDENT_TRACE_URI + "belongsToStudent");
			//Creates a new statement.
			stmt = ontM.createStatement (actionTrace, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Sets the initialTime property of Action_Trace resource and adds the value.
			p = ontM.getProperty (STUDENT_TRACE_URI + "initialTime");
			Calendar date = new GregorianCalendar ();
			date.set (log.DateLog.Year, log.DateLog.Month-1, log.DateLog.Day, log.DateLog.Hour, log.DateLog.Minute, log.DateLog.Second);
			date.set (Calendar.MILLISECOND, log.DateLog.Millisecond);
			XSDDateTime t = new XSDDateTime (date);
			actionTrace.addLiteral (p, t);

			//Saves into disk.
			string path = _logsPath + domain.Key + Path.DirectorySeparatorChar;
			//Determine whether the directory exists. If the directory does not exist, it will be created.
			if (!Directory.Exists(path)) {
				Directory.CreateDirectory(path);
			}
			//Specifies the file path.
			string file = path + student.Key + ".owl";
			//Saves the logs into files.
			using (BufferedWriter bw = new BufferedWriter (new OutputStreamWriter(new FileOutputStream(file), "UTF-8"))) {
				string b = "file:" + file;
				//using (OntModel ontTmp = ModelFactory.createOntologyModel (OntModelSpec.OWL_DL_MEM, null)) {
				//ontTmp = _ontologyModel;
				ontM.write (bw, "RDF/XML-ABBREV", b);
				//}
				bw.close ();
			}
			//Specifies the file.
			file = _ontologyPath + "world_error.owl";
			//Saves world error data into a file.
			using (BufferedWriter bw = new BufferedWriter (new OutputStreamWriter(new FileOutputStream(file), "UTF-8"))) {
				string b = "file:" + file;
				//using (OntModel ontTmp = ModelFactory.createOntologyModel (OntModelSpec.OWL_DL_MEM, null)) {
				_worldDataModel.write (bw, "RDF/XML-ABBREV", b);
				//}
				bw.close ();
			}
		}

		/// <summary>
		/// Adds the log into onto.
		/// </summary>
		/// <param name="log">Log.</param>
		public void AddLogIntoOnto (OtherErrorLog log, Student student, DomainActions domain)
		{
			//Creates a auxiliar variable.
			OntModel ontM;
			//Selects the ontology model with the specific domain and student
			var oQuery = 
				from o in _logModels
					where o.Key == (domain.Key + "-" + student.Key)
				select o.Value;
			//Checks if the OntModel exists. If it does not exist, create a new OntModel.
			if (oQuery.ToList ().Count == 0)
				ontM = CreateOntModel (domain.Key, student.Key);
			else
				ontM = oQuery.First ();
			//Add directly the log into the ontology, without any kind of test.
			//Gets the resource Action_Trace.
			Resource r = ontM.getResource (STUDENT_TRACE_URI + "Action_Trace");
			//Gets the type property  of Thing.
			Property p = ontM.getProperty("http://www.w3.org/1999/02/22-rdf-syntax-ns#type");
			//Creates a new Action_Trace resource.
			Resource actionTrace = ontM.createResource (STUDENT_TRACE_URI + "Log" + domain.Key
			                       + student.Key + log.Action.Key
			                       + log.DateLog.ToString ("dd/MM/yyyy HH:mm:ss.fff").Replace (" ", null));
			//Creates a new statement.
			Statement stmt = ontM.createStatement (actionTrace, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the resource Action_Execution_State.
			r = ontM.getResource (STUDENT_STATE_URI + "Action_Execution_State");
			//Creates a new Action_Execution_State resource.
			Resource actionExeState = ontM.createResource (KNOWLEDGE_OBJECT_URI + "ActionExecutionState" + domain.Key
			                          + student.Key + log.Action.Key
			                          + log.DateLog.ToString ("dd/MM/yyyy HH:mm:ss.fff").Replace (" ", null));
			//Creates a new statement.
			stmt = ontM.createStatement (actionExeState, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the resource Punctual_Action_Element.
			r = ontM.getResource (KNOWLEDGE_OBJECT_URI + "Punctual_Action_Element");
			//Creates a new Punctual_Action_Element resource.
			Resource punctualActionElement = ontM.createResource (KNOWLEDGE_OBJECT_URI + "PunctualActionElement"
			                                 + domain.Key + student.Key + log.Action.Key
			                                 + log.DateLog.ToString ("dd/MM/yyyy HH:mm:ss.fff").Replace (" ", null));
			//Creates a new statement.
			stmt = ontM.createStatement (punctualActionElement, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the resource Precondition_State.
			r = ontM.getResource (STUDENT_STATE_URI + "Precondition_State");
			//Creates a new Precondition_State resource.
			Resource preCondState = ontM.createResource (STUDENT_STATE_URI + "PreconditionState"
			                        + domain.Key + student.Key + log.Action.Key
			                        + log.DateLog.ToString ("dd/MM/yyyy HH:mm:ss.fff").Replace (" ", null));
			//Creates a new statement.
			stmt = ontM.createStatement (preCondState, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the Condition_On_State whose key is equals to the ErrorMessage key.
			p = ontM.getProperty (KNOWLEDGE_OBJECT_URI + "idObject");
			StmtIterator stmtItr = _otherDataModel.listStatements (null, p, log.ErrorAssociated.Message.Key);
			//Gets the statement. It is assumed that there is a only statement.
			stmt = stmtItr.nextStatement ();
			//Gets the statement subject, which is the Condition_On_State with the key.
			Resource condOnState = stmt.getSubject ();
			//Gets the condOnState property of Punctual_Action_Element resource and adds the value.
			p = ontM.getProperty (KNOWLEDGE_OBJECT_URI + "condOnState");
			//Creates a new statement.
			stmt = ontM.createStatement (punctualActionElement, p, condOnState);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the idObject property of Punctual_Action_Element resource and adds the value.
			//The value is the error associated key.
			p = ontM.getProperty (KNOWLEDGE_OBJECT_URI + "idObject");
			punctualActionElement.addLiteral (p, log.ErrorAssociated.Key);
			//Gets the descriptorObject property of Punctual_Action_Element resource and adds the value.
			//The value is the type of error.
			p = ontM.getProperty (KNOWLEDGE_OBJECT_URI + "descriptorObject");
			punctualActionElement.addLiteral (p, "other");
			//Gets the refersToCondition property of Precondition_State resource and adds the value.
			p = ontM.getProperty (STUDENT_STATE_URI + "refersToCondition");
			//Creates a new statement.
			stmt = ontM.createStatement (preCondState, p, punctualActionElement);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the isMet property of Precondition_State resource and adds the value.
			p = ontM.getProperty (STUDENT_STATE_URI + "isMet");
			preCondState.addLiteral (p, false);
			//Gets the consistsOfPrecondState property of Action_Execution_State resource and adds the value.
			p = ontM.getProperty (STUDENT_STATE_URI + "consistsOfPrecondState");
			//Creates a new statement.
			stmt = ontM.createStatement (actionExeState, p, preCondState);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the wasApplied property of Action_Execution_State resource and adds the value.
			p = ontM.getProperty (STUDENT_STATE_URI + "wasApplied");
			actionExeState.addLiteral (p, log.WasApplied);
			//Sets the associatedActionState property of Action_Trace.
			p = ontM.getProperty (STUDENT_TRACE_URI + "associatedActionState");
			//Creates a new statement.
			stmt = ontM.createStatement (actionTrace, p, actionExeState);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the Punctual_Action whose key is equals to the ActionAplication.
			p = ontM.getProperty (KNOWLEDGE_OBJECT_URI + "idObject");
			stmtItr = _domainDataModel.listStatements (null, p, log.Action.Key + "_" + domain.Key);
			//Gets the statement. It is assumed that there is a only statement.
			stmt = stmtItr.nextStatement ();
			//Gets the statement subject, which is the PunctualAction with the key.
			r = stmt.getSubject ();
			//Sets the associatedSpecificAction property of Action_Trace resource.
			p = ontM.getProperty (STUDENT_TRACE_URI + "associatedSpecificAction");
			//Creates a new statement.
			stmt = ontM.createStatement (actionTrace, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Gets the Student_Profile whose key is equals to the Student.
			p = ontM.getProperty (STUDENT_PROFILE_URI + "idStudent");
			stmtItr = _studentDataModel.listStatements (null, p, student.Key);
			//Gets the statement. It is assumed that there is a only statement.
			stmt = stmtItr.nextStatement ();
			//Gets the statement subject, which is the Student_Profile with the key.
			Resource rAux = stmt.getSubject ();
			//Sets the Student_Information which has the previous Student_Profile.
			p = ontM.getProperty (STUDENT_INFORMATION_URI + "hasProfile");
			stmtItr = _studentDataModel.listStatements (null, p, rAux);
			//Gets the statement. It is assumed that there is a only statement.
			stmt = stmtItr.nextStatement ();
			//Gets the statement subject, which is the Student_Information with the key.
			r = stmt.getSubject ();
			//Sets the belongsToStudent property of Action_Trace resource and adds the value.
			//The belongsToStudent value is the previous obtained Student_Information resource.
			p = ontM.getProperty (STUDENT_TRACE_URI + "belongsToStudent");
			//Creates a new statement.
			stmt = ontM.createStatement (actionTrace, p, r);
			//Adds the statement into the ontologyModel.
			ontM.add (stmt);
			//Sets the initialTime property of Action_Trace resource and adds the value.
			p = ontM.getProperty (STUDENT_TRACE_URI + "initialTime");
			Calendar date = new GregorianCalendar ();
			date.set (log.DateLog.Year, log.DateLog.Month-1, log.DateLog.Day, log.DateLog.Hour, log.DateLog.Minute, log.DateLog.Second);
			date.set (Calendar.MILLISECOND, log.DateLog.Millisecond);
			XSDDateTime t = new XSDDateTime (date);
			actionTrace.addLiteral (p, t);

			//Saves into disk.
			string path = _logsPath + domain.Key + Path.DirectorySeparatorChar;
			//Determine whether the directory exists. If the directory does not exist, it will be created.
			if (!Directory.Exists(path)) {
				Directory.CreateDirectory(path);
			}
			//Specifies the file path.
			string file = path + student.Key + ".owl";
			//Saves the logs into files.
			using (BufferedWriter bw = new BufferedWriter (new OutputStreamWriter(new FileOutputStream(file), "UTF-8"))) {
				string b = "file:" + file;
				//using (OntModel ontTmp = ModelFactory.createOntologyModel (OntModelSpec.OWL_DL_MEM, null)) {
				//ontTmp = _ontologyModel;
				ontM.write (bw, "RDF/XML-ABBREV", b);
				//}
				bw.close ();
			}
		}

		/// <summary>
		/// Adds the domain into onto.
		/// </summary>
		/// <param name="domain">Domain.</param>
		public void AddDomainIntoOnto(DomainActions domain) 
		{
			//Gets the resource Plan.
			Resource r = _domainDataModel.getResource (KNOWLEDGE_OBJECT_URI + "Plan");
			//Gets the type property of Thing.
			Property p = _domainDataModel.getProperty ("http://www.w3.org/1999/02/22-rdf-syntax-ns#type");
			//Creates a new Punctual_Action resource.
			Resource plan = _domainDataModel.createResource (KNOWLEDGE_OBJECT_URI + "Domain" + domain.Key);
			//Creates a new statement.
			Statement stmt = _domainDataModel.createStatement (plan, p, r);
			//Adds the statement into the ontologyModel.
			_domainDataModel.add (stmt);
			//Gets the idObject property and adds the value.
			//The value is the domain key.
			p = _domainDataModel.getProperty (KNOWLEDGE_OBJECT_URI + "idObject");
			//Creates a new statement.
			stmt = _domainDataModel.createStatement (plan, p, domain.Key);
			//Adds the statement into the ontologyModel.
			_domainDataModel.add (stmt);
			//Gets the descriptorObject property and adds the value.
			//The value is the domain description.
			p = _domainDataModel.getProperty (KNOWLEDGE_OBJECT_URI + "descriptorObject");
			//Creates a new statement.
			stmt = _domainDataModel.createStatement (plan, p, domain.Description);
			//Adds the statement into the ontologyModel.
			_domainDataModel.add (stmt);

			//Saves into disk.
			//Specifies the file.
			string file =_ontologyPath + "plan_data.owl";
			//Saves domain data into a file.
			using (BufferedWriter bw = new BufferedWriter (new OutputStreamWriter(new FileOutputStream(file), "UTF-8"))) {
				string b = "file:" + file;
				//using (OntModel ontTmp = ModelFactory.createOntologyModel (OntModelSpec.OWL_DL_MEM, null)) {
				_domainDataModel.write (bw, "RDF/XML-ABBREV", b);
				//}
				bw.close ();
			}
		}

		/// <summary>
		/// Adds the action into onto.
		/// </summary>
		/// <param name="action">Action.</param>
		/// <param name="domain">Domain.</param>
		public void AddActionIntoOnto(ActionAplication action, DomainActions domain)
		{
			//Gets the resource Punctual_Action.
			Resource r = _domainDataModel.getResource (KNOWLEDGE_OBJECT_URI + "Punctual_Action");
			//Gets the type property of Thing.
			Property p = _domainDataModel.getProperty ("http://www.w3.org/1999/02/22-rdf-syntax-ns#type");
			//Creates a new Punctual_Action resource.
			Resource pAction = _domainDataModel.createResource (KNOWLEDGE_OBJECT_URI + "ActionAplication" + action.Key +
			                   "_" + domain.Key);
			//Creates a new statement.
			Statement stmt = _domainDataModel.createStatement (pAction, p, r);
			//Adds the statement into the ontologyModel.
			_domainDataModel.add (stmt);
			//Gets the idObject property and adds the value.
			//The value is the action key.
			p = _domainDataModel.getProperty (KNOWLEDGE_OBJECT_URI + "idObject");
			//Creates a new statement.
			stmt = _domainDataModel.createStatement (pAction, p, action.Key + "_" + domain.Key);
			//Adds the statement into the ontologyModel.
			_domainDataModel.add (stmt);
			//Gets the descriptorObject property and adds the value.
			//The value is the action name.
			p = _domainDataModel.getProperty (KNOWLEDGE_OBJECT_URI + "descriptorObject");
			//Creates a new statement.
			stmt = _domainDataModel.createStatement (pAction, p, action.Name);
			//Adds the statement into the ontologyModel.
			_domainDataModel.add (stmt);
			//Gets the Plan whose key is equals to the given one.
			p = _domainDataModel.getProperty (KNOWLEDGE_OBJECT_URI + "idObject");
			StmtIterator stmtItr = _domainDataModel.listStatements (null, p, domain.Key);
			//Gets the statement. It is assumed that there is a only statement.
			stmt = stmtItr.nextStatement ();
			//Gets the statement subject, which is the Plan with the key.
			Resource plan = stmt.getSubject ();
			//Gets the belongsToPlan property and adds the value.
			//The value is the plan.
			p = _domainDataModel.getProperty (STUDENT_TRACE_URI + "belongsToPlan");
			//Creates a new statement.
			stmt = _domainDataModel.createStatement (pAction, p, plan);
			//Adds the statement into the ontologyModel.
			_domainDataModel.add (stmt);
			//Saves into disk.
			//Specifies the file.
			string file =_ontologyPath + "plan_data.owl";
			//Saves domain data into a file.
			using (BufferedWriter bw = new BufferedWriter (new OutputStreamWriter(new FileOutputStream(file), "UTF-8"))) {
				string b = "file:" + file;
				//using (OntModel ontTmp = ModelFactory.createOntologyModel (OntModelSpec.OWL_DL_MEM, null)) {
				_domainDataModel.write (bw, "RDF/XML-ABBREV", b);
				//}
				bw.close ();
			}
		}

		/// <summary>
		/// Adds the student into onto.
		/// </summary>
		/// <param name="student">Student.</param>
		public void AddStudentIntoOnto(Student student)
		{
			//Gets the resource Student.
			Resource r = _studentDataModel.getResource (STUDENT_INFORMATION_URI + "Student_Information");
			//Gets the type property  of Thing.
			Property p = _studentDataModel.getProperty("http://www.w3.org/1999/02/22-rdf-syntax-ns#type");
			//Creates a new Student_Information resource.
			Resource studentInformation = _studentDataModel.createResource (STUDENT_INFORMATION_URI + "Student" + student.Key);
			//Creates a new statement.
			Statement stmt = _studentDataModel.createStatement (studentInformation, p, r);
			//Adds the statement into the ontologyModel.
			_studentDataModel.add (stmt);
			//Gets the resource Personal_Data.
			r = _studentDataModel.getResource (STUDENT_PROFILE_URI + "Personal_Data");
			//Creates a new Personal_Data resource.
			Resource personalData = _studentDataModel.createResource (STUDENT_PROFILE_URI + "PersonalData" + student.Key);
			//Creates a new statement.
			stmt = _studentDataModel.createStatement (personalData, p, r);
			//Adds the statement into the ontologyModel.
			_studentDataModel.add (stmt);
			//Gets the resource Student_Profile.
			r = _studentDataModel.getResource (STUDENT_PROFILE_URI + "Student_Profile");
			//Creates a new Personal_Data resource.
			Resource studentProfile = _studentDataModel.createResource (STUDENT_PROFILE_URI + "StudentProfile" + student.Key);
			//Creates a new statement.
			stmt = _studentDataModel.createStatement (studentProfile, p, r);
			//Adds the statement into the ontologyModel.
			_studentDataModel.add (stmt);
			//Sets the value of name property of Personal_Data.
			p = _studentDataModel.getProperty (STUDENT_PROFILE_URI + "name");
			//Creates a new statement.
			stmt = _studentDataModel.createStatement (personalData, p, student.Name);
			//Adds the statement into the ontologyModel.
			_studentDataModel.add (stmt);
			//Sets the value of firstSurname property of Personal_Data.
			p = _studentDataModel.getProperty (STUDENT_PROFILE_URI + "firstSurname");
			//Creates a new statement.
			stmt = _studentDataModel.createStatement (personalData, p, student.Surname);
			//Adds the statement into the ontologyModel.
			_studentDataModel.add (stmt);
			//Sets the value of hasPersonalData property of Student_Profile.
			p = _studentDataModel.getProperty (STUDENT_PROFILE_URI + "hasPersonalData");
			//Creates a new statement.
			stmt = _studentDataModel.createStatement (studentProfile, p, personalData);
			//Adds the statement into the ontologyModel.
			_studentDataModel.add (stmt);
			//Sets the value of idStudent property of Student_Profile.
			p = _studentDataModel.getProperty (STUDENT_PROFILE_URI + "idStudent");
			//Creates a new statement.
			stmt = _studentDataModel.createStatement (studentProfile, p, student.Key);
			//Adds the statement into the ontologyModel.
			_studentDataModel.add (stmt);
			//Sets the value of hasProfile property of Student_Information.
			p = _studentDataModel.getProperty (STUDENT_INFORMATION_URI + "hasProfile");
			//Creates a new statement.
			stmt = _studentDataModel.createStatement (studentInformation, p, studentProfile);
			//Adds the statement into the ontologyModel.
			_studentDataModel.add (stmt);

			//Saves into disk.
			//Specifies the file.
			string file =_ontologyPath + "student_data.owl";
			//Saves student data into a file.
			using (BufferedWriter bw = new BufferedWriter (new OutputStreamWriter(new FileOutputStream(file), "UTF-8"))) {
				string b = "file:" + file;
				//using (OntModel ontTmp = ModelFactory.createOntologyModel (OntModelSpec.OWL_DL_MEM, null)) {
				_studentDataModel.write (bw, "RDF/XML-ABBREV", b);
				//}
				bw.close ();
			}
		}

		/// <summary>
		/// Saves the onto.
		/// </summary>
		public void SaveOnto()
		{
			//Determines whether the directory exists. If the directory does not exist, it will be created.
			if (!Directory.Exists(_logsPath)) {
				Directory.CreateDirectory(_logsPath);
			}
			//Specifies the file.
			string file =_ontologyPath + "plan_data.owl";
			//Saves domain data into a file.
			using (BufferedWriter bw = new BufferedWriter (new OutputStreamWriter(new FileOutputStream(file), "UTF-8"))) {
				string b = "file:" + file;
				//using (OntModel ontTmp = ModelFactory.createOntologyModel (OntModelSpec.OWL_DL_MEM, null)) {
				_domainDataModel.write (bw, "RDF/XML-ABBREV", b);
				//}
				bw.close ();
			}
			//Specifies the file.
			file =_ontologyPath + "student_data.owl";
			//Saves student data into a file.
			using (BufferedWriter bw = new BufferedWriter (new OutputStreamWriter(new FileOutputStream(file), "UTF-8"))) {
				string b = "file:" + file;
				//using (OntModel ontTmp = ModelFactory.createOntologyModel (OntModelSpec.OWL_DL_MEM, null)) {
				_studentDataModel.write (bw, "RDF/XML-ABBREV", b);
				//}
				bw.close ();
			}
			//Specifies the file.
			file = _ontologyPath + "world_error.owl";
			//Saves world error data into a file.
			using (BufferedWriter bw = new BufferedWriter (new OutputStreamWriter(new FileOutputStream(file), "UTF-8"))) {
				string b = "file:" + file;
				//using (OntModel ontTmp = ModelFactory.createOntologyModel (OntModelSpec.OWL_DL_MEM, null)) {
				_worldDataModel.write (bw, "RDF/XML-ABBREV", b);
				//}
				bw.close ();
			}
			//Gets an enumerator from the dictionary.
			Dictionary<string, OntModel>.Enumerator oEnum = _logModels.GetEnumerator ();
			//Advances the enumerator until there will be not more elements.
			while (oEnum.MoveNext ()) {
				//Selects the current element.
				KeyValuePair<string, OntModel> element = oEnum.Current;
				//Splits the element key.
				string[] k = element.Key.Split (new char[] { '-' });
				string path = _logsPath + k [0] + Path.DirectorySeparatorChar;
				//Determine whether the directory exists. If the directory does not exist, it will be created.
				if (!Directory.Exists(path)) {
					Directory.CreateDirectory(path);
				}
				//Specifies the file path.
				file = path + k [1] + ".owl";
				//Saves the logs into files.
				using (BufferedWriter bw = new BufferedWriter (new OutputStreamWriter(new FileOutputStream(file), "UTF-8"))) {
					string b = "file:" + file;
					//using (OntModel ontTmp = ModelFactory.createOntologyModel (OntModelSpec.OWL_DL_MEM, null)) {
					//ontTmp = _ontologyModel;
					element.Value.write (bw, "RDF/XML-ABBREV", b);
					//}
					bw.close ();
				}
			}
		}

		/// <summary>
		/// Gets the generic error message.
		/// </summary>
		/// <returns>The generic error message.</returns>
		/// <param name="type">Type.</param>
		/// <exception cref="NotSuchElementException"> 
		/// Thrown if does not exist any generic error message with the given key.
		/// </exception>
		public string GetGenericErrorMessage(string key)
		{
			//Gets the idObject property of knowledge_object.
			Property p = _ontModel.getProperty (KNOWLEDGE_OBJECT_URI + "idObject");
			//Lists all statements whose property idObject is the parameter key. 
			StmtIterator stmtItr = _ontModel.listStatements (null, p, key);
			//Gets the statement. It is assumed that there is a only statement.
			Statement stmt = stmtItr.nextStatement ();
			//Gets the statement subject, which is the Condition_On_State with the key.
			Resource r = stmt.getSubject ();
			//Gets the statement with the property descriptorObject of the previous resource.
			p = _ontModel.getProperty (KNOWLEDGE_OBJECT_URI + "descriptorObject");
			stmt = r.getProperty (p);
			//Gets the error message from the previous statement.
			string message = stmt.getString();
			//Returns the generic error message.
			return message;
		}

		/// <summary>
		/// Gets the generic error.
		/// </summary>
		/// <returns>The generic error.</returns>
		public List<Error> GetOtherErrors ()
		{
			//Gets the resource Condition_On_State.
			Resource r = _otherDataModel.getResource (KNOWLEDGE_OBJECT_URI + "Condition_On_State");
			//Gets the type property  of Thing.
			Property p = _otherDataModel.getProperty("http://www.w3.org/1999/02/22-rdf-syntax-ns#type");
			StmtIterator stmtItr = _otherDataModel.listStatements (null, p, r);
			//Creates the list of Error which will be returned.
			List<Error> otherErrors = new List<Error> ();
			//Creates the Error from the iterator.
			while (stmtItr.hasNext ()) {
				//Gets the statement.
				Statement stmt = stmtItr.nextStatement ();
				//Gets the statement subject, which is the Condition_On_State.
				r = stmt.getSubject ();
				//Gets the idObject property.
				p = _otherDataModel.getProperty (KNOWLEDGE_OBJECT_URI + "idObject");
				//Gets the error key.
				Statement stmtAux = r.getProperty (p);
				string key = stmtAux.getString ();
				//Gets the descriptorObject property.
				p = _otherDataModel.getProperty (KNOWLEDGE_OBJECT_URI + "descriptorObject");
				//Gets the error message.
				stmtAux = r.getProperty (p);
				string message = stmtAux.getString ();

				//Creates the ErrorMessage.
				ErrorMessage errorMessage = new ErrorMessage (key, message);
				//Creates the Error.
				Error error = new Error (key, errorMessage, true);
				//Adds into the list.
				otherErrors.Add (error);
			}
			return otherErrors;
		}

		/// <summary>
		/// Gets the world errors.
		/// </summary>
		/// <returns>The world errors.</returns>
		public List<Error> GetWorldErrors ()
		{
			//Gets the resource Condition_On_State.
			Resource r = _worldDataModel.getResource (KNOWLEDGE_OBJECT_URI + "Condition_On_State");
			//Gets the type property  of Thing.
			Property p = _worldDataModel.getProperty("http://www.w3.org/1999/02/22-rdf-syntax-ns#type");
			StmtIterator stmtItr = _worldDataModel.listStatements (null, p, r);
			//Creates the list of Error which will be returned.
			List<Error> worldErrors = new List<Error> ();
			//Creates the Error from the iterator.
			while (stmtItr.hasNext ()) {
				//Gets the statement.
				Statement stmt = stmtItr.nextStatement ();
				//Gets the statement subject, which is the Condition_On_State.
				r = stmt.getSubject ();
				//Gets the idObject property.
				p = _worldDataModel.getProperty (KNOWLEDGE_OBJECT_URI + "idObject");
				//Gets the error key.
				Statement stmtAux = r.getProperty (p);
				string key = stmtAux.getString ();
				//Gets the descriptorObject property.
				p = _worldDataModel.getProperty (KNOWLEDGE_OBJECT_URI + "descriptorObject");
				//Gets the error message.
				stmtAux = r.getProperty (p);
				string message = stmtAux.getString ();

				//Creates the ErrorMessage.
				ErrorMessage errorMessage = new ErrorMessage (key, message);
				//Creates the Error.
				Error error = new Error (key, errorMessage, true);
				//Adds into the list.
				worldErrors.Add (error);
			}
			return worldErrors;
		}

		/// <summary>
		/// Gets the students.
		/// </summary>
		/// <returns>The students.</returns>
		public List<Student> GetStudents ()
		{
			//Gets the resource Student.
			Resource r = _studentDataModel.getResource (STUDENT_INFORMATION_URI + "Student_Information");
			//Gets the type property  of Thing.
			Property p = _studentDataModel.getProperty("http://www.w3.org/1999/02/22-rdf-syntax-ns#type");
			StmtIterator stmtItr = _studentDataModel.listStatements (null, p, r);
			//Creates the list of Student which will be returned.
			List<Student> students = new List<Student> ();
			//Creates the Students from the iterator.
			while (stmtItr.hasNext ()) {
				//Gets the statement.
				Statement stmt = stmtItr.nextStatement ();
				//Gets the statement subject, which is the Student_Information.
				r = stmt.getSubject ();
				//Gets the hasProfile property.
				p = _studentDataModel.getProperty (STUDENT_INFORMATION_URI + "hasProfile");
				//Gets the value of the property.
				Resource rAux = r.getPropertyResourceValue (p);
				//Gets the idStudent property.
				p = _studentDataModel.getProperty (STUDENT_PROFILE_URI + "idStudent");
				//Gets the student's key.
				Statement stmtAux = rAux.getProperty (p);
				string key = stmtAux.getString ();
				//Gets the StudentProfile.
				r = rAux;
				//Gets the hasPersonalData property.
				p = _studentDataModel.getProperty (STUDENT_PROFILE_URI + "hasPersonalData");
				//Gets the value of the property.
				rAux = r.getPropertyResourceValue (p);
				//Gets the name property.
				p = _studentDataModel.getProperty (STUDENT_PROFILE_URI + "name");
				//Gets the student's name.
				stmtAux = rAux.getProperty (p);
				string name = stmtAux.getString ();
				//Gets the surname property.
				p = _studentDataModel.getProperty (STUDENT_PROFILE_URI + "firstSurname");
				//Gets the student's surname.
				stmtAux = rAux.getProperty (p);
				string surname = stmtAux.getString ();

				//Creates the Student.
				Student std = new Student (key, name, surname);
				//Adds the Student into the list.
				students.Add (std);
			}

			//Returns the list.
			return students;
		}

		/// <summary>
		/// Gets the log information.
		/// </summary>
		/// <returns>The log information.</returns>
		public List<object[]> GetLogInformation (DomainActions domain)
		{
			//Creates the list of log information which will be returned.
			List<object[]> logs = new List<object[]> ();
			//Creates a auxiliar list of OntModel.
			List<OntModel> ontM;
			//Selects the ontology model with the specific domain.
			var oQuery = 
				from o in _logModels
				where o.Key.Contains (domain.Key + "-")
				select o.Value;
			ontM = oQuery.ToList();
			//Gets the resource Student.
			Resource resource = _ontModel.getResource (STUDENT_TRACE_URI + "Action_Trace");
			//Gets the type property  of Thing.
			Property property = _ontModel.getProperty ("http://www.w3.org/1999/02/22-rdf-syntax-ns#type");
			//Search all log information from each OntModel.
			foreach (OntModel o in ontM) {
				//Lists all statements with logs as a subject.
				StmtIterator stmtItr = o.listStatements (null, property, resource);
				//Creates the log information from the iterator.
				while (stmtItr.hasNext ()) {
					//Gets the statement.
					Statement stmt = stmtItr.nextStatement ();
					//Gets the statement subject, which is the Action_Trace.
					Resource rSubject = stmt.getSubject ();
					//Gets the belongsToStudent property.
					Property p = o.getProperty (STUDENT_TRACE_URI + "belongsToStudent");
					//Gets the value of the property.
					Resource r = rSubject.getPropertyResourceValue (p);
					//Gets the Student id.
					string idStudent = r.getLocalName ().Replace ("Student", "");
					//Gets the associatedSpecificAction property.
					p = o.getProperty (STUDENT_TRACE_URI + "associatedSpecificAction");
					//Gets the value of the property.
					r = rSubject.getPropertyResourceValue (p);
					//Gets the ActionAplication id.
					string str = r.getLocalName ().Replace ("ActionAplication", "");
					string[] strArray = str.Split (new char[] { '_' });
					string idActionAplication = strArray [0];
					//Gets the DomainActions id.
					string idDomainActions = strArray [1];
					//Gets the initialTime property.
					p = o.getProperty (STUDENT_TRACE_URI + "initialTime");
					//Gets the value of the property.
					Statement stmtAux = rSubject.getProperty (p);
					//Gets the object.
					XSDDateTime t = (XSDDateTime)stmtAux.getLiteral ().getValue();
					//Gets the date.
					Calendar cal = t.asCalendar ();
					//Transforms in a DateTime.
					DateTime date = new DateTime (cal.get (1), cal.get (2)+1, cal.get (5), cal.get (11),
						               cal.get (12), cal.get (13), cal.get (14));
					//Gets the associatedActionState property.
					p = o.getProperty (STUDENT_TRACE_URI + "associatedActionState");
					//Gets the value of the property.
					r = rSubject.getPropertyResourceValue (p);
					//Gets the wasApplied property.
					p = o.getProperty (STUDENT_STATE_URI + "wasApplied");
					//Gets the value of the property.
					stmtAux = r.getProperty (p);
					//Gets the boolean value.
					bool wasApplied = stmtAux.getBoolean ();
					//Gets the Action_Execution_State as the subject.
					rSubject = r;
					//Gets the consistsOfConseqState property.
					p = o.getProperty (STUDENT_STATE_URI + "consistsOfConseqState");
					//Gets the value of the property.
					r = rSubject.getPropertyResourceValue (p);
					//Creates a boolean var which indicates whether the error are fixed.
					bool errorFixed = false;
					//Creates a string var which indicates the type of log.
					string type = "";
					//Creates a string var which indicates the id of the object.
					string idObj = "";
					//Creates a boolean var which indicates if it is an order error.
					string isOrderError = "";
					//Checks if the resource exists.
					if (r != null) {
						
						//Gets the refersToCondition property.
						p = o.getProperty (STUDENT_STATE_URI + "refersToCondition");
						//Gets the value of the property.
						Resource rAux = r.getPropertyResourceValue (p);
						//Gets the descriptorObject property.
						p = o.getProperty (KNOWLEDGE_OBJECT_URI + "descriptorObject");
						//Gets the value of the property.
						stmtAux = rAux.getProperty (p);
						//Gets the object.
						str = stmtAux.getString ();
						//Checks the string value.
						if (str == "corrected") {
							errorFixed = true;
							//Adds a value to type.
							type = "corrective";
						} else {
							type = str;
						}
					} else {
						//Gets the consistsOfPrecondState property.
						p = o.getProperty (STUDENT_STATE_URI + "consistsOfPrecondState");
						//Gets the value of the property.
						r = rSubject.getPropertyResourceValue (p);
						//Creates a new var.
						//Checks if the resource exists.
						if (r != null) {
							//Gets the refersToCondition property.
							p = o.getProperty (STUDENT_STATE_URI + "refersToCondition");
							//Gets the value of the property.
							Resource rAux = r.getPropertyResourceValue (p);
							//Gets the descriptorObject property.
							p = o.getProperty (KNOWLEDGE_OBJECT_URI + "descriptorObject");
							//Gets the value of the property.
							type = rAux.getProperty (p).getString ();
							//Gets the idObject property.
							p = o.getProperty (KNOWLEDGE_OBJECT_URI + "idObject");
							//Gets the value of the property.
							idObj = rAux.getProperty (p).getString ();
							//Gets the condOnState property.
							p = o.getProperty (KNOWLEDGE_OBJECT_URI + "condOnState");
							//Gets the value of the property.
							rAux = rAux.getPropertyResourceValue (p);
							//Checks if is otherError.
							if (type=="dependence"){//type != "other") {
								//Gets the conditionStateType property.
								p = o.getProperty (KNOWLEDGE_OBJECT_URI + "conditionStateType");
								//Gets the value of the property.
								isOrderError = rAux.getProperty (p).getString ();
							}
						} else {
							type = "noCorrective";
						}
					}
					//Creates the object array in which will be saved the information.
					object[] info = new object[] {idStudent, idDomainActions, idActionAplication,
						wasApplied, date, type, errorFixed, idObj, isOrderError};
					//Adds into the list.
					logs.Add (info);
				}
			}
			
			//Returns the list.
			return logs;
		}
	}
}