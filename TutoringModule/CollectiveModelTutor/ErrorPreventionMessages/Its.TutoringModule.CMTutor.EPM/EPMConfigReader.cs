using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using Excel;

namespace Its.TutoringModule.CMTutor.EPM
{
    public class EPMConfigReader
    {
	    private const string EPM_CONFIG_FILE_SUFFIX = "_EPM.xlsx";
        private static EPMConfigReader _instance;
        private Dictionary<string, List<ErrorPreventionMessage>> _messagesByDomain;

        public static EPMConfigReader Instance (string domainPath)
        {
            if (_instance == null)
            {
                _instance = new EPMConfigReader(domainPath);
            }
            return _instance;
        }

        private EPMConfigReader(string domainPath)
        {
            _messagesByDomain = new Dictionary<string, List<ErrorPreventionMessage>>();

	        string[] epmFiles = Directory.GetFiles(domainPath, "*" + EPM_CONFIG_FILE_SUFFIX);
	        Regex domainNameRegexp = new Regex("(.+)" + EPM_CONFIG_FILE_SUFFIX);

	        foreach (string filePath in epmFiles)
	        {
		        // Extract domain name
		        Match match = domainNameRegexp.Match(Path.GetFileName(filePath));
		        string domainName;

		        // Only parse xlsx file if it matches pattern (has a valid <domainName><suffix> name)
		        if (match.Success)
		        {
			        domainName = match.Groups[1].Value;
			        ReadEPMConfigFile(filePath, domainName);
		        }
	        }
        }

	    private void ReadEPMConfigFile(string filePath, string domainName)
	    {
		    _messagesByDomain.Add(domainName, new List<ErrorPreventionMessage>());
		        
			//Creates a FileStream to open the file.
			FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
			IExcelDataReader reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
			int row = 0;
			//Obtains all action information from the differents cells.
			while (reader.Read ()) {
				IDataRecord record = (IDataRecord)reader;

				// Stop reading file if empty first column found
				if (record[0] == null)
				{
					break;
				}
				
				// Skipping first (header) row
				if (row >= 1)
				{
					string stateKey 			= record[0] == null ? "" : record[0].ToString();
					string lowDetailMessage		= record[1] == null ? "" : record[1].ToString();
					string mediumDetailMessage	= record[2] == null ? "" : record[2].ToString();
					string highDetailMessage	= record[3] == null ? "" : record[3].ToString();

					// Store message object
					ErrorPreventionMessage message = new ErrorPreventionMessage(stateKey, lowDetailMessage, mediumDetailMessage, highDetailMessage);
					_messagesByDomain[domainName].Add(message);
				}
				
				//Advances the index.
				row++;
			}

			//Closes the FileStream.
			stream.Close(); 
	    }

	    public List<ErrorPreventionMessage> GetMessages(string domainName)
	    {
		    List<ErrorPreventionMessage> messages;
		    if (!_messagesByDomain.TryGetValue(domainName, out messages))
		    {
			    ArgumentException argEx = new ArgumentException ("No Error Prevention message config was found for domain " + domainName);
			    throw argEx;
		    }
		    return _messagesByDomain[domainName];
	    }
    }
}