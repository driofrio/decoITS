using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Excel;
//using Excel = NetOffice.ExcelApi;

namespace Its.ExpertModule.DataAccess
{
	/// <author>
	///  Diego Dotor Jara.
	/// </author>
	/// <summary>
	/// This class will access to Google Spreadsheets to obtain the action.
	/// </summary>
	public class ActionAccess
	{
		/// <summary>
		/// The instance.
		/// </summary>
		private static ActionAccess _instance = null;
		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static ActionAccess Instance (string confPath, int initialCol, int initialRow){
			//get {
			if (_instance == null)
				_instance = new ActionAccess (confPath, initialCol, initialRow);

			return _instance;
			//}
		}
		/// <summary>
		/// The initial column.
		/// </summary>
		private static int  _initialColumn;
		/// <summary>
		/// The initial row.
		/// </summary>
		private static int _initialRow;
		/// <summary>
		/// The configuration path.
		/// </summary>
		private static string _confPath;

		/// <summary>
		/// Initializes a new instance of the <see cref="Its.Expert.DataAccess.ActionAccess"/> class.
		/// </summary>
		public ActionAccess (string confPath, int initialCol, int initialRow)
		{
			//Obtains the research parameters from AppSettings.
			_initialColumn = initialCol;
			_initialRow = initialRow;
			//Obtains the configuration path from AppSettings.
			_confPath = confPath;//ConfigurationManager.AppSettings ["domainConfigurationPath"].ToString();
			//_confPath = _confPath.Replace ('\\', Path.DirectorySeparatorChar);
		}

		/// <summary>
		/// Gets the actions.
		/// </summary>
		/// <returns>The actions.</returns>
		/// <param name="domainKey">Domain key.</param>
		/// <exception cref="ArgumentException"> 
		/// Thrown if the file <paramref name="domainKey"/> does not exist.
		/// </exception>
		public List<object[]> GetActions (string domainKey)
		{
			//Creates the list which will be returned.
			List<object[]> actions = new List<object[]> ();
			string filePath = "";

			//Obtains the file path.
			filePath = _confPath + Path.DirectorySeparatorChar + domainKey + ".xlsx";
			//Checks if the file exists.
			if (!File.Exists (filePath)) {
				ArgumentException argEx = new ArgumentException ("There is not a file with the given name.", "domainKey");
				throw argEx;
			}
			//Creates a FileStream to open the file.
			FileStream stream = File.Open(filePath.ToString(), FileMode.Open, FileAccess.Read);
			IExcelDataReader reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
			int row = 0;
			//Obtains all action information from the differents cells.
			while (reader.Read ()) {
				IDataRecord record = (IDataRecord)reader;
				//Creates the array with the action information.
				if (record [_initialColumn] != null) {
					object[] actionInfo = {record [0] == null ? "" : record [0].ToString (),//Phase. 0
						record [2] == null ? "" : record [2].ToString (), //Key. 1
						record [3] == null ? "" : record [3].ToString (), //Action name. 2
						record [4] == null ? "" : record [4].ToString (), //Ok message. 3
						record [5] == null ? "" : record [5].ToString (), //Possible next actions. 4
						record [6] == null ? "" : record [6].ToString (), //TutorMessage. 5
						record [7] == null ? "" : record [7].ToString (), //Dependencies. 6
						record [8] == null ? "" : record [8].ToString (), //Dependence error. 7
						record [9] == null ? "" : record [9].ToString (), //Order dependence error. 8
						record [10] == null ? "" : record [10].ToString (), //Incompatibilities 9
						record [11] == null ? "" : record [11].ToString (), //Incompatibility error. 10
						record [12] == null ? "" : record [12].ToString (), //Lock object. 11
						record [13] == null ? "" : record [13].ToString (), //MaxTime. 12
						record [14] == null ? "" : record [14].ToString (), //MinTime. 13
						record [15] == null ? "" : record [15].ToString (), //MaxTime error. 14
						record [16] == null ? "" : record [16].ToString (), //MinTime error. 15
						record [17] == null ? "" : record [17].ToString (), //IsRepetitive. 16
						record [18] == null ? "" : record [18].ToString (), //Validate errors. 17
						record [19] == null ? "" : record [19].ToString (), //Init phase. 18
						record [20] == null ? "" : record [20].ToString (), //Object name. 19
						record [21] == null ? "" : record [21].ToString (), //In Phase. 20
						record [22] == null ? "" : record [22].ToString (), //Is Checkpoint?. 21
						record [23] == null ? "" : record [23].ToString () //Collective Model Tutor messages. 22
					};

					//Adds the action information into the list which will be returned.
					if (row >= _initialRow)
						actions.Add (actionInfo);
					//Advances the index.
					row++;
				} else
					break;

			}

			//Closes the FileStream.
			stream.Close ();

			//Returns the actions information.
			return actions;
		}
	}
}

