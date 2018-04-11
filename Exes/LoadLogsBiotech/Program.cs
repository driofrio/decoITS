using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using Its.ExpertModule.ObjectModel;
using Its.Factories;
using Its.StudentModule.DataAccess;
using Its.StudentModule.ObjectModel;
using Its.TutoringModule.ReactiveTutor.ObjectModel;
using Its.Utils.Config;

namespace LoadLogsBiotech
{
	class MainClass
	{
		private static ITutorConfig config = new DefaultTutorConfig();

		public static void Main (string[] args)
		{
			LoadLogs ();
		}

		private static void LoadLogs()
		{
			List<string> noPlanActions = new List<string>() {"f0t5","f0t6","f1t2","f1t3","f1t17","f1t18","f1t19","f2t2","f2t3","f2t15","f2t16","f2t17","f2t18","f2t19","f2t20","f2t21","f2t22","f2t23","f2t24","f3t2","f3t3","f3t12","f3t13","f3t14","f3t15","f3t16","f3t17","f3t18","f3t19","f3t20","f3t21","f3t22","f3t23","f3t24","f3t25"};

			Console.WriteLine ("Domain name: ");
			string strDomainName=Console.ReadLine();
			Console.WriteLine ("Loading log files into Ontology...");

			Console.WriteLine ("Creating domain...");

			/*Console.WriteLine ("Iteration: ");
			string iteration=Console.ReadLine();

			Console.WriteLine ("Multiply (0): ");
			int mult = 0;
			string tempmult = Console.ReadLine ();
			mult = tempmult.Trim ().Length == 0 || tempmult.Trim ()=="0" ? 1 : int.Parse (tempmult);*/

			string ontologyPath = config.OntologyPath.Replace ('\\', Path.DirectorySeparatorChar);
			string logsPath = config.LogsPath.Replace ('\\', Path.DirectorySeparatorChar);
			string expertConfPath = config.DomainConfigurationPath.Replace ('\\', Path.DirectorySeparatorChar);
			int initialCol = config.InitialColumn;
			int initialRow = config.InitialRow;

			/*if (iteration != "0") {
				string ontologyPathvacia = ontologyPath;
				ontologyPath=ontologyPath.Replace ("Its.StudentModule.Ontology", "Its.StudentModule.Ontology" + iteration);
				logsPath=logsPath.Replace ("Its.StudentModule.Ontology", "Its.StudentModule.Ontology" + iteration);
				if (!Directory.Exists (ontologyPath)) {
					Directory.CreateDirectory (ontologyPath);
					foreach (string file in Directory.GetFiles(ontologyPathvacia)) {
						File.Copy (file, file.Replace ("Its.StudentModule.Ontology", "Its.StudentModule.Ontology" + iteration), true);
					}
				}
			}
*/

			DomainActions domain = null;
			DomainActions domainVal = null;
			DomainActionsFactory temp = DomainActionsFactory.Instance (ontologyPath, logsPath, expertConfPath, initialCol, initialRow);
			domain = temp.CreateDomain (strDomainName);

			/*Console.WriteLine ("Create Validation (y/n): ");
			bool ifValidation = Console.ReadLine ().ToUpper () == "Y";
			if (ifValidation) {
				Console.WriteLine ("Creating domain validation...");
				domainVal = temp.CreateDomain (strDomainName + "Val");
			}*/

			string folderPath = "../../Logs/"+strDomainName;
			//List < LogEntry > logs = new List<LogEntry> ();
			List<Error> otherErrors=ErrorFactory.Instance(ontologyPath, logsPath).CreateOtherErrors();
			List<Error> worldErrors=ErrorFactory.Instance(ontologyPath, logsPath).CreateWorldErrors();

			/*List<int> studentsVal = new List<int> ();
			if (ifValidation) {
				Console.WriteLine ("% of validation: ");
				int intNumberValidation = int.Parse (Console.ReadLine ());
				int numberStudents = Directory.GetFiles (folderPath).Count (path => path.Contains ("_2"));//*mult;
				decimal maxRandomNumber = Math.Ceiling ((decimal)(intNumberValidation * numberStudents) / 100);
				Random rnd = new Random ();
				for (int i = 0; i < maxRandomNumber; i++) {
					int random = rnd.Next (1, (int)numberStudents);
					while (studentsVal.Contains (random)) {
						random = rnd.Next (1,(int)numberStudents);
					}
					studentsVal.Add (random);
				}
			}*/
			Console.WriteLine ("Getting logs...");

			Dictionary<int,Student> students = new Dictionary<int,Student> ();
			DataTable filesbyStudents = new DataTable ();
			filesbyStudents.Columns.Add (new DataColumn ("StudentKey"));
			filesbyStudents.Columns.Add (new DataColumn ("FilePath"));
			Dictionary<string,string[]> files = new Dictionary<string,string[]> ();

			int studentKey=1;
			int studentAct = studentKey;
			if (File.GetAttributes (folderPath) == FileAttributes.Directory) {
				Dictionary<string,Student> tmpstudents = new Dictionary<string,Student> ();
				foreach (string file in Directory.GetFiles(folderPath, "*.log")) {
					string[] fileName = Path.GetFileNameWithoutExtension (file).Split (' ');
					string studentName = fileName [0];
					string studentLastName = fileName [1].Substring(0,fileName [1].IndexOf('_'));
					Student student = null;
					if (tmpstudents.ContainsKey (studentName + studentLastName)) {
						student = tmpstudents [studentName + studentLastName];
						studentAct = int.Parse (student.Key);
					}else {
						student = new Student (studentKey.ToString (), studentName, studentLastName);
						tmpstudents.Add (studentName + studentLastName, student);
						students.Add (studentKey, student);
						studentAct = studentKey;
						studentKey++;
					}

					filesbyStudents.Rows.Add (new object[2]{ studentAct, file });
					files.Add (file, File.ReadAllLines (file));
				}

				foreach(int stdkey in students.Keys){
					Student student = students [stdkey];
					StudentLog stdLog = new StudentLog (student);

					DomainActions tmpDomain = null;
						tmpDomain = domain;
					
					OntologyAccess ontology = OntologyAccess.Instance (ontologyPath, logsPath);
					ontology.AddStudentIntoOnto (student);

					DataRow[] drs = filesbyStudents.Select ("StudentKey='"+stdkey+"'");
					Dictionary<string, List<LogEntry>> tempDepLogs = new Dictionary<string, List<LogEntry>> ();
					Dictionary<string, List<LogEntry>> tempImpLogs = new Dictionary<string, List<LogEntry>> ();

					foreach (DataRow dr in drs) {
						string file = dr [1].ToString ();
						string[] content = files [file];
						LogEntry log = null;
						if (!file.Contains ("_errores")) {
							Console.WriteLine (file);
							foreach (string line in content) {
								if (line.Length > 0) {
									string[] lineParts = line.Split ('|');
									string taskKey = lineParts [0].Trim ();
									string taskCode = lineParts [1].Trim ();
									string strtime = lineParts [5].Trim ().Replace (".0000000Z", "");
									//if (!taskCode.Contains ("releer")) {
									DateTime time = DateTime.ParseExact (strtime, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
									ActionAplication action = tmpDomain.Actions.Where (act => act.Key == taskKey).ToList () [0];

									if (!noPlanActions.Contains (taskKey)) {
										log = new NoCorrectiveActionLog (action, true, time);
										stdLog.AddLog (log);
											//ontology.AddLogIntoOnto ((NoCorrectiveActionLog)log, student, tmpDomain);
									} else {
										log = new NoPlanAllowedActionLog (action, true, time);
										stdLog.AddLog (log);
											//ontology.AddLogIntoOnto ((NoPlanAllowedActionLog)log, student, tmpDomain);
									}
									//}
								}
							}
						} else if (!file.Contains ("_erroresnotutor")) {
							Console.WriteLine (file);
							foreach (string line in content) {
								if (line.Length > 0) {
									string[] lineParts = line.Split ('|');
									string errorType = lineParts [0].Trim ();
									string taskKey = lineParts [1].Trim ();
									string taskCode = lineParts [2].Trim ();
									string[] errorTaskKey = lineParts [3].Trim ().Split ('-');
									string strtime = lineParts [4].Trim ().Replace (".0000000Z", "");

									DateTime time = DateTime.ParseExact (strtime, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);

									ActionAplication action=null;
									if (errorType=="accionnoencontrada")  {
										int phase = 0;
										/*List<LogEntry> tmpLogs = stdLog.Logs.Where (x => x.DateLog > time).ToList();
										LogEntry logDesp = null;
										if (tmpLogs.Count > 0)
											logDesp = tmpLogs.OrderBy (y => y.DateLog).First ();
										else {
											tmpLogs = stdLog.Logs.Where (x => x.DateLog < time).ToList();
											logDesp = tmpLogs.OrderBy (y => y.DateLog).Last ();
										}*/

										List<LogEntry> logsDesp = stdLog.Logs.Where (x => x.DateLog > time).OrderBy(y=>y.DateLog).ToList();
										if (logsDesp.Count == 0) {
											logsDesp = stdLog.Logs.Where (x => x.DateLog < time).OrderByDescending(y=>y.DateLog).ToList();
										}

										foreach (LogEntry logDesp in logsDesp) {
											if (logDesp.GetType () == typeof(NoCorrectiveActionLog)) {
												phase = ((NoCorrectiveActionLog)logDesp).Action.Phase;
												if (((NoCorrectiveActionLog)logDesp).Action.InitPhase)
													phase--;
												break;
											}else if (logDesp.GetType () == typeof(DepErrorLog)) {
												phase = ((DepErrorLog)logDesp).Action.Phase;
												if (((DepErrorLog)logDesp).Action.InitPhase)
													phase--;
												break;
											}else if (logDesp.GetType () == typeof(IncompErrorLog)) {
												phase = ((IncompErrorLog)logDesp).Action.Phase;
												if (((IncompErrorLog)logDesp).Action.InitPhase)
													phase--;
												break;
											}
										}

										IEnumerable<ActionAplication> enumAction = tmpDomain.Actions.Where (act => act.Name == taskCode && act.Phase==phase);
										if (enumAction.ToList ().Count == 0) {
											int tempphase = phase;
											int totalphases = tmpDomain.Actions.Max (x => x.Phase);

											tempphase--;
											enumAction = tmpDomain.Actions.Where (act => act.Name == taskCode && act.Phase==tempphase);
											if (enumAction.ToList ().Count == 0) {
												for (int i = phase+1; i <= totalphases; i++) {
													enumAction = tmpDomain.Actions.Where (act => act.Name == taskCode && act.Phase==i);
													if (enumAction.ToList ().Count > 0)
														break;
												}
												if (enumAction.ToList ().Count > 0)
													action = enumAction.ToList () [0];
												else {
													for (int i = phase-1; i >= 0; i--) {
														enumAction = tmpDomain.Actions.Where (act => act.Name == taskCode && act.Phase==i);
														if (enumAction.ToList ().Count > 0)
															break;
													}
													if (enumAction.ToList ().Count > 0)
														action = enumAction.ToList () [0];
													else {
														action = new ActionAplication (taskCode, taskCode);
														ontology.AddActionIntoOnto (action, tmpDomain);
													}
												}
											}else
												action = enumAction.ToList () [0];
										}else
											action = enumAction.ToList () [0];
									}else
										action = tmpDomain.Actions.Where (act => act.Key == taskKey).ToList () [0];
										
									Error error = null;
									switch (errorType) {
									case "accionrealizada":
								//actionalreadyperformed
										error = otherErrors.Where (err => err.Key == "actionalreadyperformed").ToList () [0];
										log = new OtherErrorLog (action, false, time, error);
										stdLog.AddLog (log);
										//ontology.AddLogIntoOnto ((OtherErrorLog)log, student, tmpDomain);
										break;
									case "accionnoencontrada":
								//actionnotfound
										error = otherErrors.Where (err => err.Key == "actionnotfound").ToList () [0];
										log = new OtherErrorLog (action, false, time, error);
										stdLog.AddLog (log);
										//ontology.AddLogIntoOnto ((OtherErrorLog)log, student, tmpDomain);
										break;
									case "dependenciabloq":
									case "dependenciasnobloq":
										foreach (string errorKey in errorTaskKey) {
											Dependence dependende;
											bool isOrderError = false;
											if (errorKey.Contains ("ord_")) {
												dependende = (SeqComplexDependence)action.Dependence;
												isOrderError = true;
											} else {
												dependende = action.Dependence.GetSimpleDependenceByActionKey (errorKey);
											}
											log = new DepErrorLog (action, true, time, dependende, isOrderError);

											stdLog.AddLog (log);
											//ontology.AddLogIntoOnto ((DepErrorLog)log, student, tmpDomain);

											if (errorType == "dependenciasnobloq") {
												if (tempDepLogs.ContainsKey (action.Key))
													tempDepLogs [action.Key].Add (log);
												else
													tempDepLogs.Add (action.Key, new List<LogEntry> (){ log });
											}
										}
										break;
									case "incompatibilidadbloq":
									case "incompatibilidadnobloq":
										foreach (string errorKey in errorTaskKey) {
											Incompatibility incomp = action.Incompatibilities.Where (inc => inc.Key == errorKey).ToList () [0];
											log = new IncompErrorLog (action, false, time, incomp);
											stdLog.AddLog (log);
											//ontology.AddLogIntoOnto ((IncompErrorLog)log, student, tmpDomain);

											if (errorType == "incompatibilidadnobloq") {
												if (tempImpLogs.ContainsKey (action.Key))
													tempImpLogs [action.Key].Add (log);
												else
													tempImpLogs.Add (action.Key, new List<LogEntry> (){ log });
											}
										}
										break;

									}
								}
							}
						} else {
							Console.WriteLine (file);
							int errMesKey = 0;
							foreach (string line in content) {
								if (line.Length > 0) {
									errMesKey++;
									string[] lineParts = line.Split ('|');
									string taskCode = lineParts [0].Trim ();
									string errorDesc = lineParts [1].Trim ();
									string objectName1 = lineParts [2].Trim ();
									string strtime = lineParts [3].Trim ().Replace (".0000000Z", "");

									int phase = 0;

									DateTime time = DateTime.ParseExact (strtime, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);

									List<LogEntry> logsDesp = stdLog.Logs.Where (x => x.DateLog > time).OrderBy(y=>y.DateLog).ToList();
									if (logsDesp.Count == 0) {
										logsDesp = stdLog.Logs.Where (x => x.DateLog < time).OrderByDescending(y=>y.DateLog).ToList();
									}

									foreach (LogEntry logDesp in logsDesp) {
										if (logDesp.GetType () == typeof(NoCorrectiveActionLog)) {
											phase = ((NoCorrectiveActionLog)logDesp).Action.Phase;
											if (((NoCorrectiveActionLog)logDesp).Action.InitPhase)
												phase--;
											break;
										}else if (logDesp.GetType () == typeof(DepErrorLog)) {
											phase = ((DepErrorLog)logDesp).Action.Phase;
											if (((DepErrorLog)logDesp).Action.InitPhase)
												phase--;
											break;
										}else if (logDesp.GetType () == typeof(IncompErrorLog)) {
											phase = ((IncompErrorLog)logDesp).Action.Phase;
											if (((IncompErrorLog)logDesp).Action.InitPhase)
												phase--;
											break;
										}
									}

									ActionAplication action = null;

									IEnumerable<ActionAplication> enumAction = tmpDomain.Actions.Where (act => act.Name == taskCode && act.Phase==phase);
									if (enumAction.ToList ().Count == 0) {
										int tempphase = phase;
										int totalphases = tmpDomain.Actions.Max (x => x.Phase);

										tempphase--;
										enumAction = tmpDomain.Actions.Where (act => act.Name == taskCode && act.Phase==tempphase);
										if (enumAction.ToList ().Count == 0) {
											for (int i = phase+1; i <= totalphases; i++) {
												enumAction = tmpDomain.Actions.Where (act => act.Name == taskCode && act.Phase==i);
												if (enumAction.ToList ().Count > 0)
													break;
											}
											if (enumAction.ToList ().Count > 0)
												action = enumAction.ToList () [0];
											else {
												for (int i = phase-1; i >= 0; i--) {
													enumAction = tmpDomain.Actions.Where (act => act.Name == taskCode && act.Phase==i);
													if (enumAction.ToList ().Count > 0)
														break;
												}
												if (enumAction.ToList ().Count > 0)
													action = enumAction.ToList () [0];
												else {
													action = new ActionAplication (taskCode, taskCode);
													ontology.AddActionIntoOnto (action, tmpDomain);
												}
											}
										}else
											action = enumAction.ToList () [0];
									}else
										action = enumAction.ToList () [0];

									Error error = null;
									string objectName2 = "";
									string type = "";
									if (errorDesc.Contains ("avatarlejosdebase") || errorDesc.Contains ("noseencuentrabase") || errorDesc == "noseencuentrapoyata"
									   || errorDesc == "noseencuentracabina" || errorDesc == "noseencuentraelagitador" || errorDesc == "noseencuentraautoclave"
									   || errorDesc == "avatarlejos") {
										//faileddrop:Alerta: El objeto @objectName no ha encontrado una superficie cercana. Acércate mas al lugar deseado.
										type = "faileddrop";
										List<Error> tempErrors = worldErrors.Where (err => err.Key == "faileddrop" + objectName1.ToLower ()).ToList ();
										if (tempErrors.Count > 0)
											error = tempErrors.ToList () [0];
										else {
											Error tempError = worldErrors.Where (err => err.Key == "faileddrop").ToList () [0];
											error = new Error (tempError.Key + objectName1.ToLower (), tempError.Message.Clone (), tempError.IsBlock);
											error.Message.Key = error.Key;
											string tempmessage = error.Message.Message;
											tempmessage = tempmessage.Replace ("@objectName", char.ToUpper (objectName1 [0]) + objectName1.Substring (1));
											error.Message.Message = tempmessage;
											worldErrors.Add (error);
										}
									} else if (errorDesc.Contains ("noseencuentra")) {
										//objectoutofrange:Alerta: El objeto @objectName1 necesita que este cerca el objeto @objectName2 para realizar la acción solicitada.
										type = "objectoutofrange";
										objectName2 = errorDesc.Replace ("noseencuentra", "");
										List<Error> tempErrors = worldErrors.Where (err => err.Key == "objectoutofrange" + objectName1 + objectName1.ToLower ()).ToList ();
										if (tempErrors.Count > 0)
											error = tempErrors.ToList () [0];
										else {
											Error tempError = worldErrors.Where (err => err.Key == "objectoutofrange").ToList () [0];
											error = new Error (tempError.Key + objectName1.ToLower () + objectName2.ToLower (), tempError.Message.Clone (), tempError.IsBlock);
											error.Message.Key = error.Key;
											string tempmessage = error.Message.Message;
											tempmessage = tempmessage.Replace ("@objectName1", char.ToUpper (objectName1 [0]) + objectName1.Substring (1));
											tempmessage = tempmessage.Replace ("@objectName2", char.ToUpper (objectName2 [0]) + objectName2.Substring (1));
											error.Message.Message = tempmessage;
											worldErrors.Add (error);
										}
									} else if (errorDesc.Contains ("avatarlejosde") && !errorDesc.Contains ("avatarlejosdebase")) {
										//avataroutofrange:Alerta: Te encuentras lejos para usar el objeto @objectName.
										type = "avataroutofrange";
										List<Error> tempErrors = worldErrors.Where (err => err.Key == "avataroutofrange" + objectName1.ToLower ()).ToList ();
										if (tempErrors.Count > 0)
											error = tempErrors.ToList () [0];
										else {
											Error tempError = worldErrors.Where (err => err.Key == "avataroutofrange").ToList () [0];
											error = new Error (tempError.Key + objectName1.ToLower (), tempError.Message.Clone (), tempError.IsBlock);
											error.Message.Key = error.Key;
											string tempmessage = error.Message.Message;
											tempmessage = tempmessage.Replace ("@objectName", char.ToUpper (objectName1 [0]) + objectName1.Substring (1));
											error.Message.Message = tempmessage;
											worldErrors.Add (error);
										}
									} else if (errorDesc.Contains ("ocupado")) {
										//objectblocked:Alerta: El objeto @objectName está siendo utilizado por otro avatar.
										type = "objectblocked";
										List<Error> tempError = worldErrors.Where (err => err.Key == "objectblocked" + objectName1.ToLower ()).ToList ();
										if (tempError.Count > 0)
											error = tempError.ToList () [0];
										else {
											error = worldErrors.Where (err => err.Key == "objectblocked").ToList () [0].Clone ();
											error.Key = error.Key + objectName1.ToLower ();
											error.Message.Key = error.Key;
											string tempmessage = error.Message.Message;
											tempmessage = tempmessage.Replace ("@objectName", char.ToUpper (objectName1 [0]) + objectName1.Substring (1));
											error.Message.Message = tempmessage;
											worldErrors.Add (error);
										}
									}

									log = new WorldErrorLog (action, false, time, error, type);
									stdLog.AddLog (log);
									//ontology.AddLogIntoOnto ((WorldErrorLog)log, student, tmpDomain);
								}
							}
						}
					}

					if (student.Name == "Alvaro" && student.Surname == "Miguel") {
						int x = 0;
					}

					foreach (string actionKey in tempImpLogs.Keys) {
						List<LogEntry> logs = tempImpLogs [actionKey];
						DateTime dtPreAct = new DateTime ();
						LogEntry preAction = stdLog.Logs.FirstOrDefault (x => x.Action.Key == actionKey && x.GetType()==typeof(NoCorrectiveActionLog));
						if (preAction != null) {
							dtPreAct = preAction.DateLog;
							preAction.DateLog = logs.First ().DateLog;
							int seq = 1;
							DateTime iniDate = preAction.DateLog;
							foreach (LogEntry log in logs) {
								log.DateLog = iniDate.AddMilliseconds (seq);
								seq++;
							}
								
							if (tempDepLogs.ContainsKey (actionKey)) {
								logs = tempDepLogs [actionKey];
								foreach (LogEntry log in logs) {
									log.DateLog = iniDate.AddMilliseconds (seq);
									seq++;
								}
								tempDepLogs.Remove (actionKey);
							}
						}
					}

					foreach (string actionKey in tempDepLogs.Keys) {
						List<LogEntry> logs = tempDepLogs [actionKey];
						DateTime dtPreAct = new DateTime ();
						LogEntry preAction = stdLog.Logs.FirstOrDefault (x => x.Action.Key == actionKey && x.GetType () == typeof(NoCorrectiveActionLog));
						if (preAction != null) {
							dtPreAct = preAction.DateLog;
							preAction.DateLog = logs.First ().DateLog;
							int seq = 1;
							DateTime iniDate = preAction.DateLog;
							foreach (LogEntry log in logs) {
								log.DateLog = iniDate.AddMilliseconds (seq);
								seq++;
							}
						}
					}

					foreach (LogEntry log in stdLog.Logs) {
						if (log.GetType () == typeof(NoCorrectiveActionLog))
							ontology.AddNoCorrectiveActionLogIntoOnto(log, student, tmpDomain);
						else if (log.GetType () == typeof(NoPlanAllowedActionLog))
							ontology.AddNoPlanAllowedActionLogIntoOnto(log, student, tmpDomain);
						else if (log.GetType () == typeof(OtherErrorLog))
							ontology.AddOtherErrorLogIntoOnto(log, student, tmpDomain);
						else if (log.GetType () == typeof(OtherErrorLog))
							ontology.AddOtherErrorLogIntoOnto(log, student, tmpDomain);
						else if (log.GetType () == typeof(DepErrorLog))
							ontology.AddDepErrorLogIntoOnto(log, student, tmpDomain);
						else if (log.GetType () == typeof(IncompErrorLog))
							ontology.AddIncompErrorLogIntoOnto(log, student, tmpDomain);
						else if (log.GetType () == typeof(WorldErrorLog))
							ontology.AddWorldErrorLogIntoOnto(log, student, tmpDomain);
					}

				}
			}
			Console.WriteLine ("Log files loaded.");
		}
	}
}
