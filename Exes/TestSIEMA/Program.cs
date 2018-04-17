using System;
using System.Collections.Generic;
using Its.TutoringModule.Common;
using Its.TutoringModule.Factories;
using Its.Utils.Config;

namespace TestSIEMA
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello!!!");
            
//            string[] keys = new string[] { "Tutorial" }; //Tutorial must refer to Domain and world configuration files with same name
            
            TutorFactory tf = TutorFactory.Instance();
            ITutor tutor = tf.CreateReactiveTutor("Tutorial", new DefaultTutorConfig());
            //Carga los estudiantes registrados en el sistema
//            tutor.LoadStudents();
//
//            foreach (string k in keys)
//            {
//                tutor.LoadWorld(k);
//                tutor.LoadDomain(k);
//            }


            //Para registrar estudiantes
            //string key, string name, string
/*            middleName, string surname, string lastName
            tutor.CreateStudent(argumentos[1], argumentos[2], argumentos[3],
                argumentos[4], argumentos[5]);
*/

            //Así se hacen las llamadas al tutor

            string actionName = "crearLog";
            string domain = "Tutorial";
            string studentKey = "22";
            string relatedObject = "SeleccionPractica";
            Dictionary<string, List<string>> messages;
            List<string> errors;
            
            Console.WriteLine("Before validation: " + DateTime.Now.ToString("hh:mm:ss.fff"));

            int codigoFinalizacion = tutor.Validate(actionName, domain,
                studentKey, relatedObject, out errors);
            
            Console.WriteLine("After validation: " + DateTime.Now.ToString("hh:mm:ss.fff"));
            
            Console.WriteLine ("Return Code: " + codigoFinalizacion);
            Console.WriteLine ("Errors: ");
            foreach (string error in errors) {
                Console.WriteLine (error);
            }

            messages = tutor.GetTutorMessages(actionName, domain, studentKey);
            
            Console.WriteLine("After get messages: " + DateTime.Now.ToString("hh:mm:ss.fff"));

            foreach (string key in messages.Keys) {
                Console.WriteLine (key + ":");
                foreach (string msg in messages[key])
                {
                    Console.WriteLine(msg);    
                }
                Console.WriteLine ("");
            }
        }
    }
}