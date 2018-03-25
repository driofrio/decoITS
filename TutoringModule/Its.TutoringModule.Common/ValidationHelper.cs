using System.Collections.Generic;
using Its.ExpertModule;
using Its.ExpertModule.ObjectModel;
using Its.StudentModule;
using Its.StudentModule.ObjectModel;
using Its.TutoringModule.TutoringCoordinator.ReactiveTutor.ObjectModel;
using Its.WorldModule;

namespace Its.TutoringModule.Common
{
    public class ValidationHelper
    {
        /// <summary>
        /// The world control.
        /// </summary>
        private Dictionary<string, WorldControl> _worldControl;
        /// <summary>
        /// The student control.
        /// </summary>
        private StudentControl _studentControl;
        /// <summary>
        /// The expert control.
        /// </summary>
        private ExpertControl _expertControl;

        private int lastValidationCode;
        private List<Error> lastValidationErrors;
        
        public ValidationHelper(Dictionary<string, WorldControl> worldControl, StudentControl studentControl, ExpertControl expertControl)
        {
            _worldControl = worldControl;
            _studentControl = studentControl;
            _expertControl = expertControl;
        }

        /// <summary>
        /// Performs validation and stored valiation results
        /// </summary>
        /// <param name="actionName"></param>
        /// <param name="domainName"></param>
        /// <param name="studentKey"></param>
        /// <param name="objectName"></param>
        /// <param name="outputError"></param>
        /// <returns></returns>
        public int ValidateAction(string actionName, string domainName, string studentKey, string objectName,
            out List<Error> outputError)
        {
            lastValidationCode = ValidateActionHelper(actionName, domainName, studentKey, objectName, out outputError);
            lastValidationErrors = new List<Error>(outputError);

            return lastValidationCode;
        }

        public int GetLastValidationResults(out List<Error> outputError)
        {
            outputError = new List<Error>(lastValidationErrors);
            return lastValidationCode;
        }
        
        private int ValidateActionHelper (string actionName, string domainName, string studentKey, string objectName, out List<Error> outputError)
        {
            //Creates the result variable.
            int result = 1;
            //Creates a bool variable to get if the action block or not the object.
            bool blockObj;
            //Gets the student with the given key.
            Student student = _studentControl.GetStudent (studentKey);
            //Validates if the object is block.
            bool blockRes = _worldControl[domainName].ObjectBlockValidate(objectName, student);
            //Checks the value returned.
            if (blockRes == true) {
                //Creates a WorldError.
                List<string> objectNameList = new List<string> ();
                objectNameList.Add (objectName);
                Error worldError = _worldControl [domainName].GetWorldError ("objectblocked", objectNameList);
                //Gets the action aplication.
                ActionAplication action = _expertControl.GetActionByName (domainName, actionName, studentKey);
                //Gets the domain.
                DomainActions domain = _expertControl.GetDomainActions (domainName);
                //Registers the error.
                _studentControl.CreateWorldErrorLog (action, domain, student, false, worldError, "objectblocked");
                //Adds the error into the list.
                outputError = new List<Error>();
                outputError.Add (worldError);
                //Sets the result.
                result = 0;
            } else {
                //Calls the expert action validation and gets the result.
                result = _expertControl.ActionValidation (actionName, domainName, studentKey, objectName, out outputError, out blockObj);
                //Checks if the action blocks or not the object.
                if (blockObj == true)
                    //Blocks the object.
                    _worldControl[domainName].BlockObject(objectName, student);
            }
            //Returns the result.
            return result;
        }
    }
}
