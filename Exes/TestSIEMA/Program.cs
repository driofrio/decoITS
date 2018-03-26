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
            List<string> messages = new List<string>();

            int codigoFinalizacion = tutor.ToTutor (actionName, domain,
                studentKey, relatedObject, out messages);

            Console.WriteLine ("Return Code: " + codigoFinalizacion);
            foreach (String m in messages) {
                Console.WriteLine (m);
            }
        }
    }
}