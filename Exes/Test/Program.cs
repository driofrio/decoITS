using System;
using System.Configuration;
using System.IO;
using System.Collections.Generic;
using Its.ExpertModule.ObjectModel;
using Its.ExpertModule.DataAccess;
using Its.TutoringModule.ReactiveTutor.ObjectModel;
using Its.StudentModule.DataAccess;
using Its.StudentModule.ObjectModel;
using Its.Factories;
using Its.Utils.Config;

namespace Its.Test
{

	class MainClass
	{
		private static OntologyAccess ontology;
		private static ITutorConfig config = new DefaultTutorConfig();
		private static string ontologyPath = config.OntologyPath.Replace ('\\', Path.DirectorySeparatorChar);
		private static string logsPath = config.LogsPath.Replace ('\\', Path.DirectorySeparatorChar);
		private static string expertConfPath = config.DomainConfigurationPath.Replace ('\\', Path.DirectorySeparatorChar);
		private static int initialCol = config.InitialColumn;
		private static int initialRow = config.InitialRow;

		public static void pruebaNoCorrectiveActionLog ()
		{
			Console.WriteLine ("Iniciando prueba...");
			//Charges the ontology model.
			Console.WriteLine ("Cargando ontología...");
			ontology = OntologyAccess.Instance(ontologyPath, logsPath);
			//Creates the Student which will be used into the log.
			Console.WriteLine ("Creando estudiante...");
			Console.WriteLine ("Clave: 0001");
			Console.WriteLine ("Nombre: Diego");
			Console.WriteLine ("Apellido: Riofrio");
			Student student = new Student  ("0001", "Diego", "Riofrio");
			//Adds the Student into the ontology.
			Console.WriteLine ("Añadiendo estudiante a la ontología...");
			ontology.AddStudentIntoOnto (student);
			//Creates the ActionAplication.
			Console.WriteLine ("Creando acción...");
			ActionAplication action = new ActionAplication ("f01a30", "Tirar de la cadena");
			//Creates the Domain.
			Console.WriteLine ("Creando dominio de práctica...");
			//Creates the actions list.
			List<ActionAplication> actionList = new List<ActionAplication>();
			//Adds the only action into the list.
			actionList.Add (action);
			DomainActions domain = new DomainActions ("Aseo", "Como ir al servicio", actionList, 0);
			//Adds the DomainActions into the ontology.
			Console.WriteLine ("Añadiendo dominio a la ontología...");
			ontology.AddDomainIntoOnto (domain);
			//Adds the ActionAplication into the ontology.
			Console.WriteLine ("Añadiendo acción a la ontología...");
			ontology.AddActionIntoOnto (action, domain);

			//Creates the NoCorrectiveActionLog.
			Console.WriteLine ("Creando log del tipo acción correcta que no corrige errores...");
			NoCorrectiveActionLog noCorrectiveActionLog = new NoCorrectiveActionLog (action, true);
			//Adds the log into the ontology.
			Console.WriteLine ("Añadiendo log a la ontología...");
			ontology.AddLogIntoOnto (noCorrectiveActionLog, student, domain);

			Console.WriteLine ("Escritura finalizada.");
		}

		public static void pruebaCorrectiveActionLog ()
		{
			//Charges the ontology model.
			Console.WriteLine ("Cargando ontología...");
			ontology = OntologyAccess.Instance(ontologyPath, logsPath);
			Console.WriteLine ("Iniciando prueba...");
			//Creates the Student which will be used into the log.
			Console.WriteLine ("Creando estudiante...");
			Console.WriteLine ("Clave: 0001");
			Console.WriteLine ("Nombre: Diego");
			Console.WriteLine ("Apellido: Riofrio");
			Student student = new Student ("0001", "Diego", "Riofrio");
			//Adds the Student into the ontology.
			Console.WriteLine ("Añadiendo estudiante a la ontología...");
			ontology.AddStudentIntoOnto (student);
			//Creates the ActionAplication.
			Console.WriteLine ("Creando acción...");
			ActionAplication action = new ActionAplication ("f01a30", "Tirar de la cadena");
			//Creates the Domain.
			Console.WriteLine ("Creando dominio de práctica...");
			//Creates the actions list.
			List<ActionAplication> actionList = new List<ActionAplication>();
			//Adds the only action into the list.
			actionList.Add (action);
			DomainActions domain = new DomainActions ("Aseo", "Como ir al servicio", actionList, 0);
			//Adds the DomainActions into the ontology.
			Console.WriteLine ("Añadiendo dominio a la ontología...");
			ontology.AddDomainIntoOnto (domain);
			//Adds the ActionAplication into the ontology.
			Console.WriteLine ("Añadiendo acción a la ontología...");
			ontology.AddActionIntoOnto (action, domain);

			//Creates the CorrectiveActionLog.
			Console.WriteLine ("Creando log del tipo acción correcta que corrige errores...");
			CorrectiveActionLog correctiveActionLog = new CorrectiveActionLog (action, true, true);
			//Adds the log into the ontology.
			Console.WriteLine ("Añadiendo log a la ontología...");
			ontology.AddLogIntoOnto (correctiveActionLog, student, domain);

			//Saves the ontology into disc.
			Console.WriteLine ("Guardando ontología en disco...");
			ontology.SaveOnto ();
			Console.WriteLine ("Escritura finalizada.");
		}

		public static void pruebaDepErrorLog ()
		{
			//Charges the ontology model.
			Console.WriteLine ("Cargando ontología...");
			ontology = OntologyAccess.Instance(ontologyPath, logsPath);
			Console.WriteLine ("Iniciando prueba...");
			//Creates the Student which will be used into the log.
			Console.WriteLine ("Creando estudiante...");
			Console.WriteLine ("Clave: 0001");
			Console.WriteLine ("Nombre: Diego");
			Console.WriteLine ("Apellido: Riofrio");
			Student student = new Student ("0001", "Diego", "Riofrio");
			//Adds the Student into the ontology.
			Console.WriteLine ("Añadiendo estudiante a la ontología...");
			ontology.AddStudentIntoOnto (student);
			//Creates the ActionAplication.
			Console.WriteLine ("Creando acciones de dependencia...");
			ActionAplication actionA = new ActionAplication ("f01a30", "Crear tarta");
			ActionAplication actionB = new ActionAplication ("f01a31", "Hornear tarta");
			//Creates a error messages.
			Console.WriteLine ("Creando mensajes de error...");
			ErrorMessage errorMsgA = new ErrorMessage ("001", "Se ha comido la tarta antes de crearla.");
			ErrorMessage errorMsgB = new ErrorMessage ("002", "Se ha comido la tarta antes de hornearla.");
			ErrorMessage errorMsgC = new ErrorMessage ("003", "Se ha comido la tarta sin realizar el proceso correctamente.");
			//Creates errors.
			Console.WriteLine ("Creando errores...");
			Error errorA = new Error ("001", errorMsgA, false);
			Error errorB = new Error ("002", errorMsgB, false);
			Error errorC = new Error ("003", errorMsgC, true);
			//Creates a simple dependence.
			Console.WriteLine ("Creando dependencia compleja secuencial...");
			SimpleDependence sDependenceA = new SimpleDependence ("001", actionA, errorA);
			SimpleDependence sDependenceB = new SimpleDependence ("002", actionB, errorB);
			//Creates the list with simpleDependences.
			List<Dependence> dependenceList= new List<Dependence> ();
			dependenceList.Add (sDependenceA);
			dependenceList.Add (sDependenceB);
			//Creates the sequential complex dependence.
			SeqComplexDependence complexDependence = new SeqComplexDependence ("001", dependenceList, errorC);
			//Creates the TutorMessage.
			Console.WriteLine ("Creando mensaje de tutoría...");
			TutorMessage tutorMessage = new TutorMessage ("1", "Realice la siguiente acción.");
			//Creates the action aplication.
			Console.WriteLine ("Creando acción principal...");
			ActionAplication action = new ActionAplication ("f01a32", 1, "Comer tarta", "Comerse la tarta fabricada.", 
				                          new List<string> (new string[] { "Tarta" }), false, false, false, false, false,
				                          complexDependence, null, false, true, null, "Tarta comida.", true, null, tutorMessage);
			//Creates the Domain.
			Console.WriteLine ("Creando dominio de práctica...");
			//Creates the actions list.
			List<ActionAplication> actionList = new List<ActionAplication>();
			//Adds the actions into the list.
			actionList.Add (actionA);
			actionList.Add (actionB);
			actionList.Add (action);
			DomainActions domain = new DomainActions ("Cocinar", "Como cocinar una tarta", actionList, 0);
			//Adds the DomainActions into the ontology.
			Console.WriteLine ("Añadiendo dominio a la ontología...");
			ontology.AddDomainIntoOnto (domain);
			//Adds the ActionAplications into the ontology.
			Console.WriteLine ("Añadiendo acciones a la ontología...");
			ontology.AddActionIntoOnto (action, domain);
			ontology.AddActionIntoOnto (actionA, domain);
			ontology.AddActionIntoOnto (actionB, domain);


			//Creates the DepErrorLog.
			Console.WriteLine ("Creando log del tipo error de dependencia de orden...");
			DepErrorLog depErrorLog = new DepErrorLog (action, true, complexDependence, true);
			//Adds the log into the ontology.
			Console.WriteLine ("Añadiendo log a la ontología...");
			ontology.AddLogIntoOnto (depErrorLog, student, domain);

			//Saves the ontology into disc.
			Console.WriteLine ("Guardando ontología en disco...");
			ontology.SaveOnto ();
			Console.WriteLine ("Escritura finalizada.");
		}

		public static void pruebaWorldErrorLog ()
		{
			//Charges the ontology model.
			Console.WriteLine ("Cargando ontología...");
			ontology = OntologyAccess.Instance(ontologyPath, logsPath);
			Console.WriteLine ("Iniciando prueba...");
			//Creates the Student which will be used into the log.
			Console.WriteLine ("Creando estudiante...");
			Console.WriteLine ("Clave: 0001");
			Console.WriteLine ("Nombre: Diego");
			Console.WriteLine ("Apellido: Riofrio");
			Student student = new Student ("0001", "Diego", "Riofrio");
			//Adds the Student into the ontology.
			Console.WriteLine ("Añadiendo estudiante a la ontología...");
			ontology.AddStudentIntoOnto (student);
			//Creates the ActionAplication.
			Console.WriteLine ("Creando acción...");
			ActionAplication action = new ActionAplication ("f01a30", "Tirar de la cadena");
			//Creates the Domain.
			Console.WriteLine ("Creando dominio de práctica...");
			//Creates the actions list.
			List<ActionAplication> actionList = new List<ActionAplication>();
			//Adds the only action into the list.
			actionList.Add (action);
			DomainActions domain = new DomainActions ("Aseo", "Como ir al servicio", actionList, 0);
			//Adds the DomainActions into the ontology.
			Console.WriteLine ("Añadiendo dominio a la ontología...");
			ontology.AddDomainIntoOnto (domain);
			//Adds the ActionAplication into the ontology.
			Console.WriteLine ("Añadiendo acción a la ontología...");
			ontology.AddActionIntoOnto (action, domain);
			//Creates a error messages.
			Console.WriteLine ("Creando mensajes de error...");
			ErrorMessage errorMsg = new ErrorMessage ("avataroutofrangeInodoro", "Alerta: Te encuentras lejos para usar el objeto Inodoro");
			//Creates errors.
			Console.WriteLine ("Creando errores...");
			Error error = new Error ("avataroutofrangeInodoro", errorMsg, true);

			//Creates the DepErrorLog.
			Console.WriteLine ("Creando log del tipo mundo...");
			WorldErrorLog worldErrorLog = new WorldErrorLog(action, true, error, "avataroutofrange");
			//Adds the log into the ontology.
			Console.WriteLine ("Añadiendo log a la ontología...");
			ontology.AddLogIntoOnto (worldErrorLog, student, domain);

			//Saves the ontology into disc.
			Console.WriteLine ("Guardando ontología en disco...");
			ontology.SaveOnto ();
			Console.WriteLine ("Escritura finalizada.");
		}

		public static void pruebaGenericErrorMessage ()
		{
			//Charges the ontology model.
			Console.WriteLine ("Cargando ontología...");
			ontology = OntologyAccess.Instance(ontologyPath, logsPath);
			Console.WriteLine ("Iniciando prueba...");
			Console.WriteLine ("Recuperando mensajes genéricos de error...");
			Console.WriteLine ("Buscando mensaje con clave: actionnotfound...");
			string message = ontology.GetGenericErrorMessage ("actionnotfound");
			Console.WriteLine ("Mensaje: " + message);
			Console.WriteLine ("Buscando mensaje con clave: actionalreadyperformed...");
			message = ontology.GetGenericErrorMessage ("actionalreadyperformed");
			Console.WriteLine ("Mensaje: " + message);
			Console.WriteLine ("Buscando mensaje con clave: avataroutofrange...");
			message = ontology.GetGenericErrorMessage ("avataroutofrange");
			Console.WriteLine ("Mensaje: " + message);
			Console.WriteLine ("Buscando mensaje con clave: faileddrop...");
			message = ontology.GetGenericErrorMessage ("faileddrop");
			Console.WriteLine ("Mensaje: " + message);
			Console.WriteLine ("Buscando mensaje con clave: objectblocked...");
			message = ontology.GetGenericErrorMessage ("objectblocked");
			Console.WriteLine ("Mensaje: " + message);
			Console.WriteLine ("Buscando mensaje con clave: objectoutofrange...");
			message = ontology.GetGenericErrorMessage ("objectoutofrange");
			Console.WriteLine ("Mensaje: " + message);
		}

		public static void pruebaActionAccess ()
		{
			Console.WriteLine ("Iniciando prueba...");
			ActionAccess access = ActionAccess.Instance(expertConfPath, initialCol, initialRow);

			List<object[]> lObj = access.GetActions ("Tutorial");

			foreach (object[] o in lObj)
				foreach (object u in o)
					Console.WriteLine (u.ToString ());
		}

		public static void pruebaArray ()
		{
			string[] strArray = { "A", "B", "C", "D" };
			Console.WriteLine ("El array de strings se compone de: ");
			for (int i = 0; i < strArray.Length; i++)
				Console.Write (strArray [i] + ", ");
			Console.Write ("\nSe procede a la eliminación del primer elemento.");
			//Erases the first element of the Array.
			Array.ConstrainedCopy (strArray, 1, strArray, 0, strArray.Length - 1);
			strArray [strArray.Length - 1] = "";
			Console.WriteLine ("El array de strings tras la eliminación se compone de: ");
			for (int i = 0; i < strArray.Length; i++)
				Console.Write (strArray [i] + ", ");
			Console.Write ("\n");
		}

		public static void pruebaSubString ()
		{
			string str = "([f01t14])";
			LinkedList<string> q = new LinkedList<string> ();
			q.AddLast (str);
			Console.WriteLine ("La cadena tiene valor: " + q.First.Value);
			Console.WriteLine ("Se procede a eliminar todos los elementos hasta \'(\'");
			string s = q.First.Value.Substring (q.First.Value.IndexOf ("(") + 1);
			q.RemoveFirst ();
			q.AddFirst (s);
			Console.WriteLine ("El nuevo valor de la cadena es: " + q.First.Value);
		}

		public static void recursivoA (ref Queue<string> q)
		{
			Console.WriteLine ("Entrando en recursivoA");
			Console.WriteLine ("El array de strings se compone de: ");
			foreach (string s in q)
				Console.Write (s + "-");
			Console.Write ("\n");
			while (q.Count > 0) {
				Console.WriteLine ("Valor del elemento: " + q.Peek());
				if (q.Peek() == "A") {
					Console.WriteLine ("\nSe procede a la eliminación del elemento.");
					//Erases the first element of the Array.
					q.Dequeue ();
					Console.WriteLine ("El array de strings se compone de: ");
					foreach (string s in q)
						Console.Write (s + "-");
					Console.Write ("\n");
				} else if (q.Peek() == "B") {
					recursivoB (ref q);
					Console.WriteLine ("El array de strings se compone de: ");
					foreach (string s in q)
						Console.Write (s + "-");
					Console.Write ("\n");
				}
			}
			Console.WriteLine ("Saliendo en recursivoA");
		}

		public static void recursivoB (ref Queue<string> q)
		{
			Console.WriteLine ("Entrando en recursivoB");
			Console.WriteLine ("El array de strings se compone de: ");
			foreach (string s in q)
				Console.Write (s + "-");
			Console.Write ("\n");
			while (q.Count > 0) {
				Console.WriteLine ("Valor del elemento: " + q.Peek());
				if (q.Peek() == "A") {
					recursivoA (ref q);
					Console.WriteLine ("El array de strings se compone de: ");
					foreach (string s in q)
						Console.Write (s + "-");
					Console.Write ("\n");
				} else if (q.Peek() == "B") {
					Console.WriteLine ("\nSe procede a la eliminación del elemento.");
					//Erases the first element of the Array.
					q.Dequeue ();
					Console.WriteLine ("El array de strings se compone de: ");
					foreach (string s in q)
						Console.Write (s + "-");
					Console.Write ("\n");
				}
			}
			Console.WriteLine ("Saliendo en recursivoB");
		}

		public static void recursivo (ref Queue<string> q)
		{
			if (q.Peek() == "A") {
				recursivoA (ref q);
				Console.WriteLine ("El array de strings se compone de: ");
				foreach (string s in q)
					Console.Write (s + "-");
				Console.Write ("\n");
			} else if (q.Peek() == "B") {
				recursivoB (ref q);
				Console.WriteLine ("El array de strings se compone de: ");
				foreach (string s in q)
					Console.Write (s + "-");
				Console.Write ("\n");
			}
		}

		public static void pruebaPasoRef ()
		{
			string[] strArray = { "A", "A", "B", "A" };
			Queue<string> q = new Queue<string> (strArray);
			Console.WriteLine ("El array de strings se compone de: ");
			foreach (string s in q)
				Console.Write (s + "-");
			Console.WriteLine ("\nSe procede a la llamada del método con parámetros por referencia.");
			recursivo (ref q);
		}

		public static void pruebaDomainFactory ()
		{
			//Charges the ontology model.
			Console.WriteLine ("Cargando ontología...");
			ontology = OntologyAccess.Instance(ontologyPath, logsPath);
			Console.WriteLine ("Escoja uno de los siguientes archivos:");
			string[] files = Directory.GetFiles (ConfigurationManager.AppSettings ["configurationPath"]);
			foreach (string f in files) {
				Console.WriteLine (f.Replace (ConfigurationManager.AppSettings ["configurationPath"], ""));
			}

			bool accepted = false;
			string file = "";

			while (!accepted) {

				Console.WriteLine ("Introduzca nombre del archivo:");
				string opt = Console.ReadLine ();
				file = ConfigurationManager.AppSettings ["configurationPath"] + opt;

				if (!File.Exists (file))
					Console.WriteLine ("El nombre del archivo es incorrecto, vuelva a introducirlo.");
				else
					accepted = true;
			}

			file = file.Replace (ConfigurationManager.AppSettings ["configurationPath"], "");
			file = file.Replace (".xlsx", "");

			Console.WriteLine ("Archivo escogido: " + file);

			Console.WriteLine ("\nSe procede a ejecutar la prueba.");
			Console.WriteLine ("Creando factoria...");
			DomainActionsFactory factory = DomainActionsFactory.Instance(ontologyPath, logsPath, expertConfPath, initialCol, initialRow);
			Console.WriteLine ("Creando el dominio...");
			DomainActions domain = factory.CreateDomain (file);

			Console.WriteLine ("Escriba \"Conforme\" para terminar la prueba");
			string exit = "";
			while (exit != "Conforme")
				exit = Console.ReadLine ();
		}

		public static void pruebaStudentFactory ()
		{
			//Charges the ontology model.
			Console.WriteLine ("Cargando ontología...");
			ontology = OntologyAccess.Instance(ontologyPath, logsPath);
			Console.WriteLine ("Iniciando prueba...");
			//Creates the Student which will be used into the log.
			Console.WriteLine ("Creando estudiante...");
			Console.WriteLine ("Clave: 0001");
			Console.WriteLine ("Nombre: Diego");
			Console.WriteLine ("Apellido: Riofrio");
			Student student = new Student ("0001", "Diego", "Riofrio");
			//Adds the Student into the ontology.
			Console.WriteLine ("Añadiendo estudiante a la ontología...");
			ontology.AddStudentIntoOnto (student);
			//Creates the Student which will be used into the log.
			Console.WriteLine ("Creando estudiante...");
			Console.WriteLine ("Clave: 0002");
			Console.WriteLine ("Nombre: Diego");
			Console.WriteLine ("Apellido: Dotor");
			student = new Student ("0002", "Diego", "Dotor");
			//Adds the Student into the ontology.
			Console.WriteLine ("Añadiendo estudiante a la ontología...");
			ontology.AddStudentIntoOnto (student);
			//Creates the Student which will be used into the log.
			Console.WriteLine ("Creando estudiante...");
			Console.WriteLine ("Clave: 0003");
			Console.WriteLine ("Nombre: Álvaro");
			Console.WriteLine ("Apellido: Iturmendi");
			student = new Student ("0003", "Álvaro", "Iturmendi");
			//Adds the Student into the ontology.
			Console.WriteLine ("Añadiendo estudiante a la ontología...");
			ontology.AddStudentIntoOnto (student);

			//Creates the StudentFactory.
			Console.WriteLine ("Se procede a crear la factoría...");
			StudentFactory studentFactory = StudentFactory.Instance(ontologyPath, logsPath);
			//Creates the Student list.
			Console.WriteLine ("Creando estudiantes...");
			Dictionary<string, Student> studentList = studentFactory.CreateStudents ();

			//Prints the students.
			foreach (Student std in studentList.Values)
				Console.WriteLine ("Estudiante: " + std.Key + " de nombre " + std.Name + " y apellido " + std.Surname);
		}

		public static void pruebaOtherErrorFactory () 
		{
			//Charges the ontology model.
			Console.WriteLine ("Cargando ontología...");
			ontology = OntologyAccess.Instance(ontologyPath, logsPath);
			Console.WriteLine ("Iniciando prueba...");
			//Creates the ErrorFactory.
			Console.WriteLine ("Se procede a crear la factoría...");
			ErrorFactory errorFactory = ErrorFactory.Instance(ontologyPath, logsPath);
			//Creates the Error list.
			Console.WriteLine ("Creando errores...");
			List<Error> otherList = errorFactory.CreateOtherErrors ();

			//Prints the other errors.
			foreach (Error e in otherList)
				Console.WriteLine ("Error: " + e.Key + " cuyo mensaje es \"" + e.Message.Message + "\" " +
				"de clave: " + e.Message.Key + " y con valor para isBlock = " + e.IsBlock.ToString ());
		}

		public static void pruebaWorldErrorFactory ()
		{
			//Charges the ontology model.
			Console.WriteLine ("Cargando ontología...");
			ontology = OntologyAccess.Instance(ontologyPath, logsPath);
			Console.WriteLine ("Iniciando prueba...");
			//Creates the ErrorFactory.
			Console.WriteLine ("Se procede a crear la factoría...");
			ErrorFactory errorFactory = ErrorFactory.Instance(ontologyPath, logsPath);
			//Creates the Error list.
			Console.WriteLine ("Creando errores...");
			List<Error> worldList = errorFactory.CreateWorldErrors ();

			//Prints the other errors.
			foreach (Error e in worldList)
				Console.WriteLine ("Error: " + e.Key + " cuyo mensaje es \"" + e.Message.Message + "\" " +
					"de clave: " + e.Message.Key + " y con valor para isBlock = " + e.IsBlock.ToString ());
		}

		public static void pruebaLogFactory () 
		{
			//Charges the ontology model.
			Console.WriteLine ("Cargando ontología...");
			ontology = OntologyAccess.Instance(ontologyPath, logsPath);
			Console.WriteLine ("Iniciando prueba...");
			//Creates the DomainAction.
			Console.WriteLine ("Creando dominio de Tutorial...");
			DomainActions domain = DomainActionsFactory.Instance(ontologyPath, logsPath, expertConfPath, initialCol, initialRow).CreateDomain ("Tutorial");
			//Dictionary<string, DomainActions> domains = new Dictionary<string, DomainActions> ();
			//domains.Add (domain.Key, domain);
			//Creates the students.
			Console.WriteLine ("Creando estudiantes...");
			Dictionary<string, Student> students = new Dictionary<string, Student> ();
			for (int i = 1; i <= Directory.GetFiles(config.LogsPath +
				Path.DirectorySeparatorChar + "Tutorial" + Path.DirectorySeparatorChar).Length; i++)
				students.Add (i.ToString (), new Student (i.ToString (), i.ToString () + "a", i.ToString () + "b"));
			//Creates the Error list.
			Console.WriteLine ("Creando errores...");
			List<Error> worldList = ErrorFactory.Instance(ontologyPath, logsPath).CreateWorldErrors ();
			List<Error> otherList = ErrorFactory.Instance(ontologyPath, logsPath).CreateOtherErrors ();
			//Creates the Dictionaries.
			Dictionary<string, Error> worldErrors = new Dictionary<string, Error> ();
			Dictionary<string, Error> otherErrors = new Dictionary<string, Error> ();
			//Adds the errors to the dictionaries.
			foreach (Error e in worldList)
				worldErrors.Add (e.Key, e);
			foreach (Error e in otherList)
				otherErrors.Add (e.Key, e);
			//Creates the LogFactory.
			Console.WriteLine ("Se procede a crear la factoría...");
			DomainLogFactory logFactory = DomainLogFactory.Instance(ontologyPath, logsPath);
			//Creates the Log list.
			Console.WriteLine ("Creando errores...");
			DomainLog logDomain = logFactory.CreateLogsFromOntology (domain, students, otherErrors, worldErrors);

			Console.WriteLine ("Escriba \"Conforme\" para terminar la prueba");
			string exit = "";
			while (exit != "Conforme")
				exit = Console.ReadLine ();
		}

		public static void Main (string[] args)
		{
			Console.WriteLine ("Iniciando programa de pruebas: Tratado de la ontología.");

			//Print the option to choose a test.
			Console.WriteLine ("Por favor, elija una prueba. Para ello escriba el código de la prueba.");
			Console.WriteLine ("Nombre prueba\t\tcódigo prueba");
			Console.WriteLine ("NoCorrectiveActionLog\t\t001");
			Console.WriteLine ("CorrectiveActionLog\t\t002");
			Console.WriteLine ("DepErrorLog\t\t\t003");
			Console.WriteLine ("GenericErrorMessage\t\t004");
			Console.WriteLine ("WorldErrorLog\t\t\t005");
			Console.WriteLine ("ActionAccess\t\t\t006");
			Console.WriteLine ("PruebaArray\t\t\t007");
			Console.WriteLine ("PruebaSubString\t\t\t008");
			Console.WriteLine ("PruebaPasoRef\t\t\t009");
			Console.WriteLine ("PruebaDomainFactory\t\t010");
			Console.WriteLine ("PruebaStudentFactory\t\t011");
			Console.WriteLine ("PruebaOtherErrorFactory\t\t012");
			Console.WriteLine ("PruebaWorldErrorFactory\t\t013");
			Console.WriteLine ("PruebaLogFactory\t\t014");
			bool optValida = false;
			string opt = "";
			while (optValida == false) {

				Console.WriteLine ("\nIntroducir código:");
				opt = Console.ReadLine ();
				switch (opt) {
				case "001":
					pruebaNoCorrectiveActionLog ();
					optValida = true;
					break;
				case "002":
					pruebaCorrectiveActionLog ();
					optValida = true;
					break;
				case "003":
					pruebaDepErrorLog ();
					optValida = true;
					break;
				case "004":
					pruebaGenericErrorMessage ();
					optValida = true;
					break;
				case "005":
					pruebaWorldErrorLog ();
					optValida = true;
					break;
				case "006":
					pruebaActionAccess ();
					optValida = true;
					break;
				case "007":
					pruebaArray ();
					optValida = true;
					break;
				case "008":
					pruebaSubString ();
					optValida = true;
					break;
				case "009":
					pruebaPasoRef ();
					optValida = true;
					break;
				case "010":
					pruebaDomainFactory ();
					optValida = true;
					break;
				case "011":
					pruebaStudentFactory ();
					optValida = true;
					break;
				case "012":
					pruebaOtherErrorFactory ();
					optValida = true;
					break;
				case "013":
					pruebaWorldErrorFactory ();
					optValida = true;
					break;
				case "014":
					pruebaLogFactory ();
					optValida = true;
					break;
				default:
					Console.WriteLine ("Opción no válida. Por favor, escriba bien el código.");
					break;
				}
			}

			Console.WriteLine ("Ejecución finalizada. Escriba \"Ok\" para salir.");

			string exit = "";
			while (exit != "Ok")
				exit = Console.ReadLine ();
		}
	}
}
