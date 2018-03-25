using System;
using System.Collections.Generic;
using System.Linq;
using Its.TutoringModule.StudentBehaviorPredictor;
using Its.ExpertModule;
using Its.ExpertModule.ObjectModel;
using System.IO;
using Its.StudentModule;
using Its.TutoringModule.ReactiveTutor.ObjectModel;
using Its.StudentModule.ObjectModel;
using System.Data;
using Its.TutoringModule.StudentBehaviorPredictor.ObjectModel;
using Its.Utils.Math;
using Its.WorldModule;
using System.Globalization;
using System.Data.SqlClient;
using Microsoft.AnalysisServices.AdomdClient;
using System.Xml;
using System.Data.OleDb;
using Excel;
using System.Text;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using Its.Utils.Config;

namespace TestSBP
{
	class MainClass
	{
		private static Dictionary<string, Dictionary<int,PredictiveStudentModel>> validationModels=new Dictionary<string, Dictionary<int, PredictiveStudentModel>>();
		private static Dictionary<string, Dictionary<int,List<StudentLog>>> validationLogs=new Dictionary<string, Dictionary<int, List<StudentLog>>>();
		private static ITutorConfig config = new DefaultTutorConfig();

		public static void Main (string[] args)
		{
			//DateTime now=DateTime.Now;
			//java.util.Calendar cal = java.util.GregorianCalendar.getInstance();
			//cal.set (now.Year, now.Month - 1, now.Day, now.Hour, now.Minute, now.Second);
			//cal.set (java.util.Calendar.MILLISECOND, now.Millisecond);
			//cal.setTimeInMillis (long.Parse(now.ToString ("HHmmssfff")));
			Console.WriteLine (DateTime.Now.ToString());
			Console.WriteLine ("Domain name: ");
			string strDomainName = strDomainName=Console.ReadLine();
			//CreateModel(strDomainName, ClusterMethod.NoClusters,true, false);
			//CreateValStuByStu ();
		    Console.WriteLine ("Option:\n1. CreateValPertage.\n2. CreateValPertageNoCluSeq.\n3. MeansDispertions." +
			                   "\n4. MeansDispertionsNoCluSeq.\n5. MeansSum.\n6. TotalModelData.\n7. Data by cluster." +
			                   "\n8. Data by sequence cluster.\n9. Save SQL.\n10. DiffNoclusterByCluster.\n11. DiffNoclusterBySequences" +
			                   "\n12. Diff Time two Actions.\n13. Media de transiciones y varianza.\n14. Frecuencias por eventos y estados.");
			string option=Console.ReadLine();
			bool includeNoPlanActions = false;
			bool inPhases = true;
			switch (option)
			{
				case "1":
					//Console.WriteLine("includeNoPlanActions? Y/N");
					//includeNoPlanActions = Console.ReadLine().ToUpper() == "Y";
					//Console.WriteLine("in phases? Y/N");
					//inPhases = Console.ReadLine().ToUpper() == "Y";
					CreateValPertage(strDomainName, includeNoPlanActions, inPhases);
					break;
				case "2":
					//Console.WriteLine("includeNoPlanActions? Y/N");
					//includeNoPlanActions = Console.ReadLine().ToUpper() == "Y";
					//Console.WriteLine("in phases? Y/N");
					//inPhases = Console.ReadLine().ToUpper() == "Y";
					CreateValPertageNoCluSeq(strDomainName, includeNoPlanActions, inPhases);
					break;
				case "3":
					MeansDispertions();
					break;
				case "4":
					MeansDispertionsNoCluSeq();
					break;
				case "5":
					MeansSum();
					break;
				case "6":
					TotalModelData(strDomainName);
					break;
				case "7":
					StudentsByCluster(strDomainName);
					break;
				case "8":
					StudentsByCluster(strDomainName, ClusterMethod.Sequences);
					break;
				case "9":
                    Console.WriteLine("in phases? Y/N");
                    inPhases = Console.ReadLine().ToUpper() == "Y";
                    SaveLogsSql(strDomainName, false, inPhases);
					break;
				case "10":
                    Console.WriteLine("in phases? Y/N");
                    inPhases = Console.ReadLine().ToUpper() == "Y";
                    DiffNoclusterByCluster(strDomainName, inPhases);
					break;
				case "11":
                    Console.WriteLine("in phases? Y/N");
                    inPhases = Console.ReadLine().ToUpper() == "Y";
                    PredictiveStudentModel model = CreateModel(strDomainName, ClusterMethod.NoClusters, false, inPhases);
					string filepath = "Similitud.xlsx";
					if (File.Exists(filepath))
						File.Delete(filepath);

					File.Copy("default.xlsx", filepath, true);
					using (SpreadsheetDocument spreadSheet = SpreadsheetDocument.Open(filepath, true))
					{
						Sheet firstSheet = spreadSheet.WorkbookPart.Workbook.Sheets.GetFirstChild<Sheet>();
						firstSheet.Remove();
						DiffNoclusterByCluster(strDomainName, ClusterMethod.Sequences, model, spreadSheet.WorkbookPart, inPhases);
					}
					break;
				case "12":
					Console.WriteLine("Action1: ");
					string act1 = Console.ReadLine();
					Console.WriteLine("Action2: ");
					string act2 = Console.ReadLine();
					TimeBetweenActions(strDomainName, act1, act2);
					break;
				case "13":
					MediaVarianzaTrans(strDomainName);
					break;
				case "14":
					CreateWorkbooksFreq(strDomainName, ClusterMethod.ErrorsAndTime);
					break;
			}

			//TotalModelData();
			//CreateStuClu();
			//StudentsByCluster();
			//SaveLogsSql(strDomainName,true);
			//TestModelDate (strDomainName, ClusterMethod.ErrorsAndTime);
			Console.WriteLine (DateTime.Now.ToString());
			Console.WriteLine ("Done!");
		}

		public static void MediaVarianzaTrans(string strDomainName)
		{
			PredictiveStudentModel model = CreateModel(strDomainName, ClusterMethod.NoClusters, false, false);
			List<Node<State, Event>> states = model.DefaultCluster.StudentActionsModel.GetListStates();
			int minTrans = 0;
			int maxTrans = 0;
			double sumaTrans = 0;
			double meanTrans = 0;
			double varTrans = 0;
			int counMinMedia = 0;
			int counMaxMedia = 0;
			int counUno = 0;
			int counEstado = 0;
			int countunoIrr = 0;
			int countunoCorr = 0;
			int countunoRel = 0;
			int countMas3 = 0;
			int countMas4 = 0;
			int countMas5 = 0;
			int countMas6 = 0;
			int countMas7 = 0;
			foreach (Node<State, Event> state in states)
			{
				if (!((state.Specification.GetType() == typeof(DependenceErrorState) && state.Specification.Area == Area.IrrelevantErrors) ||
							(state.Specification.GetType() == typeof(IncompatibilityErrorState) && state.Specification.Area == Area.IrrelevantErrors) ||
						   (state.Specification.GetType() == typeof(OtherErrorState) && state.Specification.Area == Area.IrrelevantErrors) ||
				     (state.Specification.GetType() == typeof(WorldErrorState) && state.Specification.Area == Area.IrrelevantErrors)))
				{
					int sumaTransState = 0;
					foreach (Arc<State, Event> arc in state.OutArcs.Values)
					{
						if ((arc.NodeIn.Specification.GetType() == typeof(DependenceErrorState) && arc.NodeIn.Specification.Area != Area.RelevantErrors) ||
							(arc.NodeIn.Specification.GetType() == typeof(IncompatibilityErrorState) && arc.NodeIn.Specification.Area != Area.RelevantErrors) ||
						   (arc.NodeIn.Specification.GetType() == typeof(OtherErrorState) && arc.NodeIn.Specification.Area != Area.RelevantErrors) ||
							  (arc.NodeIn.Specification.GetType() == typeof(WorldErrorState) && arc.NodeIn.Specification.Area != Area.RelevantErrors))
						{
							sumaTrans++;
							sumaTransState++;
						}
					}
					if (minTrans == 0 || (sumaTransState < minTrans && sumaTransState != 0))
					{
						minTrans = sumaTransState;
					}
					if (maxTrans == 0 || (sumaTransState > maxTrans))
					{
						maxTrans = sumaTransState;
					}
					counEstado++;
				}
			}
			meanTrans = sumaTrans / counEstado;

			sumaTrans = 0;
			foreach (Node<State, Event> state in states)
			{
				if (!((state.Specification.GetType() == typeof(DependenceErrorState) && state.Specification.Area == Area.IrrelevantErrors) ||
							(state.Specification.GetType() == typeof(IncompatibilityErrorState) && state.Specification.Area == Area.IrrelevantErrors) ||
						   (state.Specification.GetType() == typeof(OtherErrorState) && state.Specification.Area == Area.IrrelevantErrors) ||
					 (state.Specification.GetType() == typeof(WorldErrorState) && state.Specification.Area == Area.IrrelevantErrors)))
				{
					int sumaTransState = 0;
					foreach (Arc<State, Event> arc in state.OutArcs.Values)
					{
						if ((arc.NodeIn.Specification.GetType() == typeof(DependenceErrorState) && arc.NodeIn.Specification.Area != Area.RelevantErrors) ||
							(arc.NodeIn.Specification.GetType() == typeof(IncompatibilityErrorState) && arc.NodeIn.Specification.Area != Area.RelevantErrors) ||
						   (arc.NodeIn.Specification.GetType() == typeof(OtherErrorState) && arc.NodeIn.Specification.Area != Area.RelevantErrors) ||
							  (arc.NodeIn.Specification.GetType() == typeof(WorldErrorState) && arc.NodeIn.Specification.Area != Area.RelevantErrors))
						{
							sumaTransState++;
						}
					}
					if (sumaTransState < meanTrans)
					{
						counMinMedia++;
						if (sumaTransState == 1)
						{
							counUno++;
							if ((state.Specification.GetType() == typeof(DependenceErrorState) && state.Specification.Area == Area.IrrelevantErrors) ||
								(state.Specification.GetType() == typeof(IncompatibilityErrorState) && state.Specification.Area == Area.IrrelevantErrors) ||
							   (state.Specification.GetType() == typeof(OtherErrorState) && state.Specification.Area == Area.IrrelevantErrors) ||
								  (state.Specification.GetType() == typeof(WorldErrorState) && state.Specification.Area == Area.IrrelevantErrors))
								countunoIrr++;
							else if ((state.Specification.GetType() == typeof(DependenceErrorState) && state.Specification.Area == Area.RelevantErrors) ||
								(state.Specification.GetType() == typeof(IncompatibilityErrorState) && state.Specification.Area == Area.RelevantErrors) ||
							   (state.Specification.GetType() == typeof(OtherErrorState) && state.Specification.Area == Area.RelevantErrors) ||
								  (state.Specification.GetType() == typeof(WorldErrorState) && state.Specification.Area == Area.RelevantErrors))
								countunoRel++;
							else if (state.Specification.GetType() == typeof(CorrectState))
								countunoCorr++;
						}


					}
					else
						counMaxMedia++;
					if (sumaTransState >= 3)
					{
						countMas3++;
					}
					if (sumaTransState >= 4)
					{
						countMas4++;
					}
					if (sumaTransState >= 5)
					{
						countMas5++;
					}
					if (sumaTransState >= 6)
					{
						countMas6++;
					}
					if (sumaTransState >= 7)
					{
						countMas7++;
					}
					sumaTrans += Math.Pow(sumaTransState - meanTrans, 2);
				}
			}
			varTrans = sumaTrans / counEstado;
			Console.WriteLine("Min: " + minTrans);
			Console.WriteLine("Max: " + maxTrans);
			Console.WriteLine("Media: " + meanTrans);
			Console.WriteLine("Var: " + varTrans);
			Console.WriteLine("Num Min: " + counMinMedia);
			Console.WriteLine("Num Max: " + counMaxMedia);
			Console.WriteLine("Num Estados: " + counEstado);
			Console.WriteLine("Num 1: " + counUno);
			Console.WriteLine("Estados de errores Irrelevantes con una transición: "+countunoIrr);
			Console.WriteLine("Estados de errores Relevantes con una transición: "+countunoRel);
			Console.WriteLine("Estados correctos con una transición: " + countunoCorr);
			Console.WriteLine("> 3: " + countMas3);
			Console.WriteLine("> 4: " + countMas4);
			Console.WriteLine("> 5: " + countMas5);
			Console.WriteLine("> 6: " + countMas6);
			Console.WriteLine("> 7: " + countMas7);
		}

		public static void TimeBetweenActions(string strDomainName, string act1, string act2)
		{
			string ontologyPath = config.OntologyPath.Replace('\\', Path.DirectorySeparatorChar);
			string logsPath = config.LogsPath.Replace('\\', Path.DirectorySeparatorChar);
			string expertConfPath = config.DomainConfigurationPath.Replace('\\', Path.DirectorySeparatorChar);
			int initialCol = config.InitialColumn;
			int intialRow = config.InitialRow;
			ExpertControl expert = ExpertControl.Instance(ontologyPath, logsPath, expertConfPath, initialCol, intialRow);
			DomainActions domain = expert.GetDomainActions(strDomainName);
			if (domain == null)
				domain = expert.CreateDomain(strDomainName);
			WorldControl world = WorldControl.Instance(ontologyPath, logsPath);
			StudentControl studentcont = StudentControl.Instance(ontologyPath, logsPath, expertConfPath);
			DomainLog logs = studentcont.GetDomainLogsFromOntology(domain, expert.OtherErrors, world.WorldErrors);
			List<Student> students = logs.GetStudents();
			foreach (Student stu in students)
			{
				StudentLog stuLog = logs.GetStudentLog(stu.Key);
				/*if (stu.Key == "15" || stu.Key == "22" || stu.Key == "40" || stu.Key == "44" || stu.Key == "53" || stu.Key == "73" || stu.Key == "74" || stu.Key == "79")
				{
					Console.WriteLine(stu.Name + " " + stu.Surname + " " + stu.LastName);
				}*/
				LogEntry log1 = stuLog.Logs.Find(y => y.GetType() == typeof(NoCorrectiveActionLog) && y.Action.Key == act1);
				LogEntry log2 = stuLog.Logs.Find(y => y.GetType() == typeof(NoCorrectiveActionLog) && y.Action.Key == act2);
				if (log1 != null && log2 != null)
				{
					double diff = (log2.DateLog - log1.DateLog).TotalSeconds;
					Console.WriteLine(stu.Key + "\t" + log1.DateLog.ToShortTimeString() + "\t" + log2.DateLog.ToShortTimeString() + "\t"+diff);
				}
			}
		}

		public static void StudentsByCluster(string strDomainName){
			StudentsByCluster (strDomainName, ClusterMethod.Errors);
			Dispose ();
			StudentsByCluster (strDomainName, ClusterMethod.ErrorsAndTime);
			Dispose ();
			StudentsByCluster (strDomainName, ClusterMethod.EventsByZone);
			Dispose ();
		}

		public static void DiffNoclusterByCluster(string strDomainName, bool inPhase)
		{
			PredictiveStudentModel model = CreateModel(strDomainName, ClusterMethod.NoClusters, false, inPhase);
			string filepath = "Similitud.xlsx";
			if (File.Exists(filepath))
				File.Delete(filepath);

			File.Copy("default.xlsx", filepath, true);
			using (SpreadsheetDocument spreadSheet = SpreadsheetDocument.Open(filepath, true))
			{
				Sheet firstSheet = spreadSheet.WorkbookPart.Workbook.Sheets.GetFirstChild<Sheet>();
				firstSheet.Remove();
				spreadSheet.WorkbookPart.Workbook.Save();
				DiffNoclusterByCluster(strDomainName, ClusterMethod.Errors, model, spreadSheet.WorkbookPart, inPhase);
				//Dispose();
				DiffNoclusterByCluster(strDomainName, ClusterMethod.ErrorsAndTime, model, spreadSheet.WorkbookPart, inPhase);
				//Dispose();
				DiffNoclusterByCluster(strDomainName, ClusterMethod.EventsByZone, model, spreadSheet.WorkbookPart, inPhase);
				//Dispose();
			}
		}

		public static void DiffNoclusterByCluster(string strDomainName, ClusterMethod clumet, PredictiveStudentModel noclusterModel, WorkbookPart workbookPart, bool inPhase)
		{
			SharedStringTablePart shareStringPart;
			if (workbookPart.GetPartsOfType<SharedStringTablePart>().Count() > 0)
			{
				shareStringPart = workbookPart.GetPartsOfType<SharedStringTablePart>().First();
			}
			else {
				shareStringPart = workbookPart.AddNewPart<SharedStringTablePart>();
			}

			WorksheetPart worksheetPart = InsertWorksheet(workbookPart, clumet.ToString());

			int indexSharedString = InsertSharedStringItem("Cluster", shareStringPart);
			DocumentFormat.OpenXml.Spreadsheet.Cell cell = InsertCellInWorksheet("A", 1, worksheetPart);
			cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
			cell.CellValue = new CellValue(indexSharedString.ToString());
			indexSharedString = InsertSharedStringItem("Supp/Conf", shareStringPart);
			cell = InsertCellInWorksheet("B", 1, worksheetPart);
			cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
			cell.CellValue = new CellValue(indexSharedString.ToString());
			indexSharedString = InsertSharedStringItem("Count", shareStringPart);
			cell = InsertCellInWorksheet("C", 1, worksheetPart);
			cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
			cell.CellValue = new CellValue(indexSharedString.ToString());
			indexSharedString = InsertSharedStringItem("CountClu", shareStringPart);
			cell = InsertCellInWorksheet("D", 1, worksheetPart);
			cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
			cell.CellValue = new CellValue(indexSharedString.ToString());
			indexSharedString = InsertSharedStringItem("CountNoClu", shareStringPart);
			cell = InsertCellInWorksheet("E", 1, worksheetPart);
			cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
			cell.CellValue = new CellValue(indexSharedString.ToString());
			indexSharedString = InsertSharedStringItem("Similarity", shareStringPart);
			cell = InsertCellInWorksheet("F", 1, worksheetPart);
			cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
			cell.CellValue = new CellValue(indexSharedString.ToString());
			indexSharedString = InsertSharedStringItem("OutArcsSimilarity", shareStringPart);
			cell = InsertCellInWorksheet("G", 1, worksheetPart);
			cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
			cell.CellValue = new CellValue(indexSharedString.ToString());

			cell.CellValue = new CellValue(indexSharedString.ToString());
			indexSharedString = InsertSharedStringItem("Count", shareStringPart);
			cell = InsertCellInWorksheet("J", 1, worksheetPart);
			cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
			cell.CellValue = new CellValue(indexSharedString.ToString());
			indexSharedString = InsertSharedStringItem("CountClu", shareStringPart);
			cell = InsertCellInWorksheet("K", 1, worksheetPart);
			cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
			cell.CellValue = new CellValue(indexSharedString.ToString());
			indexSharedString = InsertSharedStringItem("CountNoClu", shareStringPart);
			cell = InsertCellInWorksheet("L", 1, worksheetPart);
			cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
			cell.CellValue = new CellValue(indexSharedString.ToString());
			indexSharedString = InsertSharedStringItem("Similarity", shareStringPart);
			cell = InsertCellInWorksheet("M", 1, worksheetPart);
			cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
			cell.CellValue = new CellValue(indexSharedString.ToString());
			/*
			cell = InsertCellInWorksheet("A", Convert.ToUInt32(currentExcelRow), worksheetPart);
			cell.CellValue = new CellValue(clu.Number.ToString());
			cell.DataType = new EnumValue<CellValues>(CellValues.Number);
*/
			Dictionary<string, Node<State, Event>> statesNoCluster = noclusterModel.DefaultCluster.StudentActionsModel.GetDicStates();
			PredictiveStudentModel model = CreateModel(strDomainName, clumet, false, inPhase);
			double[] supconfs = { 0, 0.1,0.2, 0.25,0.3,0.4, 0.5,0.6,0.7, 0.75,0.8,0.81,0.82,0.83,0.84,0.85,0.86,0.87,0.88,0.89, 0.9 };
			Console.WriteLine(clumet.ToString());
			Console.WriteLine("");
			int currentExcelRow = 2;
			foreach (StudentsCluster clu in model.Clusters.Values)
			{
				Console.WriteLine("Cluster Number: " + clu.Number);
				cell = InsertCellInWorksheet("A", Convert.ToUInt32(currentExcelRow), worksheetPart);
				cell.CellValue = new CellValue(clu.Number.ToString());
				cell.DataType = new EnumValue<CellValues>(CellValues.Number);

				List<Node<State, Event>> totalStates = clu.StudentActionsModel.GetListStates();
				foreach (double supconf in supconfs)
				{
					cell = InsertCellInWorksheet("B", Convert.ToUInt32(currentExcelRow), worksheetPart);
					cell.CellValue = new CellValue(supconf.ToString(new CultureInfo("en-US")));
					cell.DataType = new EnumValue<CellValues>(CellValues.Number);

					List<Node<State, Event>> cluSup = totalStates.Where(y => ((double)y.Specification.StudentFrequency / clu.NumberOfStudents) > supconf).ToList();
					List<Node<State, Event>> noCluSup = statesNoCluster.Values.Where(y => ((double)y.Specification.StudentFrequency / noclusterModel.DefaultCluster.NumberOfStudents) > supconf).ToList();
					int count = 0;
					double perArcs = 0;
					foreach (Node<State, Event> state in cluSup)
					{
						if (noCluSup.Any(x => x.Key == state.Key))
						{
							Node<State, Event> stateNoClu = noCluSup.Find(x => x.Key == state.Key);
							int countArc = 0;
							if (state.OutArcs.Count > 0)
							{
								foreach (Arc<State, Event> arc in state.OutArcs.Values)
								{
									if (stateNoClu.OutArcs.Values.Any(x => x.Key.Equals(arc.Key)))
										countArc++;
								}
								perArcs += countArc / stateNoClu.OutArcs.Count;
							}
							count++;
						}
					}
					double per = (double)count / (cluSup.Count +  noCluSup.Count - count);

					cell = InsertCellInWorksheet("C", Convert.ToUInt32(currentExcelRow), worksheetPart);
					cell.CellValue = new CellValue(count.ToString(new CultureInfo("en-US")));
					cell.DataType = new EnumValue<CellValues>(CellValues.Number);
					cell = InsertCellInWorksheet("D", Convert.ToUInt32(currentExcelRow), worksheetPart);
					cell.CellValue = new CellValue(cluSup.Count.ToString(new CultureInfo("en-US")));
					cell.DataType = new EnumValue<CellValues>(CellValues.Number);
					cell = InsertCellInWorksheet("E", Convert.ToUInt32(currentExcelRow), worksheetPart);
					cell.CellValue = new CellValue(noCluSup.Count.ToString(new CultureInfo("en-US")));
					cell.DataType = new EnumValue<CellValues>(CellValues.Number);
					cell = InsertCellInWorksheet("F", Convert.ToUInt32(currentExcelRow), worksheetPart);
					cell.CellFormula = new CellFormula("C" + currentExcelRow + "/(D" + currentExcelRow + "+E"+ currentExcelRow + "-C"+ currentExcelRow+")");
					cell.CellValue = new CellValue(per.ToString(new CultureInfo("en-US")));
					cell.DataType = new EnumValue<CellValues>(CellValues.Number);
					cell = InsertCellInWorksheet("G", Convert.ToUInt32(currentExcelRow), worksheetPart);
					cell.CellValue = new CellValue((perArcs/count).ToString(new CultureInfo("en-US")));
					cell.DataType = new EnumValue<CellValues>(CellValues.Number);

					Console.WriteLine("states_"+supconf.ToString(new CultureInfo("en-US")) +"\t"+per.ToString(new CultureInfo("en-US")));
					//Console.WriteLine("Equal States:\t" + count);
					//Console.WriteLine("States in clu no in total:\t" + (totalStates.Count - count));
					//Console.WriteLine("States in total no in clu:\t" + (statesNoCluster.Count - count));

					List<Arc<State, Event>> eventsNoCluster = noclusterModel.DefaultCluster.StudentActionsModel.GetEventsConf(supconf);
					List<Arc<State, Event>> eventsClu = clu.StudentActionsModel.GetEventsConf(supconf);
					count = 0;
					foreach (Arc<State, Event> evt in eventsClu)
					{
						if (eventsNoCluster.Any(x => x.Key.Equals(evt.Key)))
							count++;
					}
					per = (double)count / (eventsClu.Count + eventsNoCluster.Count - count);

					cell = InsertCellInWorksheet("J", Convert.ToUInt32(currentExcelRow), worksheetPart);
					cell.CellValue = new CellValue(count.ToString(new CultureInfo("en-US")));
					cell.DataType = new EnumValue<CellValues>(CellValues.Number);
					cell = InsertCellInWorksheet("K", Convert.ToUInt32(currentExcelRow), worksheetPart);
					cell.CellValue = new CellValue(eventsClu.Count.ToString(new CultureInfo("en-US")));
					cell.DataType = new EnumValue<CellValues>(CellValues.Number);
					cell = InsertCellInWorksheet("L", Convert.ToUInt32(currentExcelRow), worksheetPart);
					cell.CellValue = new CellValue(eventsNoCluster.Count.ToString(new CultureInfo("en-US")));
					cell.DataType = new EnumValue<CellValues>(CellValues.Number);
					cell = InsertCellInWorksheet("M", Convert.ToUInt32(currentExcelRow), worksheetPart);
					cell.CellFormula = new CellFormula("J" + currentExcelRow + "/(K" + currentExcelRow + "+L" + currentExcelRow + "-J" + currentExcelRow+ ")");
					cell.CellValue = new CellValue(per.ToString(new CultureInfo("en-US")));
					cell.DataType = new EnumValue<CellValues>(CellValues.Number);

					Console.WriteLine("events_" + supconf.ToString(new CultureInfo("en-US")) + "\t" + per.ToString(new CultureInfo("en-US")));
					//Console.WriteLine("Equal Events:\t" + count);
					//Console.WriteLine("Events in clu no in total:\t" + (eventsClu.Count - count));
					//Console.WriteLine("Events in total no in clu:\t" + (eventsNoCluster.Count - count));
					currentExcelRow++;
				}
			}

			worksheetPart.Worksheet.Save();
		}

		public static void StudentsByCluster(string strDomainName, ClusterMethod clumet){
			CreateModel (strDomainName, clumet, 1, 0, true, false, false);
			Console.WriteLine ("Cluster Method: "+clumet.ToString());
			PredictiveStudentModel model = StudentBehaviorPredictorControl.Instance(config).GetModel (strDomainName+"0", clumet);
			foreach (StudentsCluster clu in model.Clusters.Values) {
				Console.WriteLine ("Cluster Number: " + clu.Number);
				Console.WriteLine ("Number Students: " + clu.NumberOfStudents);
				Console.WriteLine("Number of states: " + clu.StudentActionsModel.NumberOfStates);
				List<Node<State, Event>> states = clu.StudentActionsModel.GetListStates();
				Console.WriteLine("Number of states > 0 supp: " + states.Count);
				Console.WriteLine("Number of states > 0.1 supp: " + states.Count(y => ((double)y.Specification.StudentFrequency / clu.NumberOfStudents) > 0.1));
				Console.WriteLine("Number of states > 0.25 supp: " + states.Count(y => ((double)y.Specification.StudentFrequency / clu.NumberOfStudents) > 0.25));
				Console.WriteLine("Number of states > 0.5 supp: " + states.Count(y => ((double)y.Specification.StudentFrequency / clu.NumberOfStudents) > 0.5));
				Console.WriteLine("Number of states > 0.75 supp: " + states.Count(y => ((double)y.Specification.StudentFrequency / clu.NumberOfStudents) > 0.75));
				Console.WriteLine("Number of states > 0.9 supp: " + states.Count(y => ((double)y.Specification.StudentFrequency / clu.NumberOfStudents) > 0.9));
				Console.WriteLine("Number of events > 0 conf: " + clu.StudentActionsModel.GetEventsConf(0).Count);
				Console.WriteLine("Number of events > 0.1 conf: " + clu.StudentActionsModel.GetEventsConf(0.1).Count);
				Console.WriteLine("Number of events > 0.25 conf: " + clu.StudentActionsModel.GetEventsConf(0.25).Count);
				Console.WriteLine("Number of events > 0.5 conf: " + clu.StudentActionsModel.GetEventsConf(0.5).Count);
				Console.WriteLine("Number of events > 0.75 conf: " + clu.StudentActionsModel.GetEventsConf(0.75).Count);
				Console.WriteLine("Number of events > 0.9 conf: " + clu.StudentActionsModel.GetEventsConf(0.9).Count);
				foreach (Area area in Enum.GetValues(typeof(Area))) {
					Console.WriteLine ("Number of states in " + area.ToString () + " cluster " + clu.Number+": " + clu.StudentActionsModel.GetStatesByArea (area).Count);
				}
			}
		}

		public static void TotalModelData(string strDomainName){
			CreateModel (strDomainName, ClusterMethod.NoClusters, 1, 0, true, false, false);
			PredictiveStudentModel model = StudentBehaviorPredictorControl.Instance(config).GetModel (strDomainName+"0", ClusterMethod.NoClusters);
			Console.WriteLine ("Total Number of students: "+model.DefaultCluster.NumberOfStudents);
			Console.WriteLine ("Total Number of states: "+model.DefaultCluster.StudentActionsModel.NumberOfStates);
			Console.WriteLine ("Total Number of events: "+model.DefaultCluster.StudentActionsModel.NumberOfEvents);
			Console.WriteLine ("Total Number of normal events: "+model.DefaultCluster.StudentActionsModel.NumberOfNormalEvents);
			Console.WriteLine ("Total Number of vector events: "+model.DefaultCluster.StudentActionsModel.NumberOfVectorEvents);
			Console.WriteLine ("Total Number of log entries: "+model.DefaultCluster.StudentActionsModel.LogEntriesNumber);
			List<Node<State, Event>> izStates = new List<Node<State, Event>> ();
			List<Node<State, Event>> rzStates = new List<Node<State, Event>> ();
			foreach (Area area in Enum.GetValues(typeof(Area))) {
				List<Node<State, Event>> tempStates = model.DefaultCluster.StudentActionsModel.GetStatesByArea (area);
				if (area == Area.IrrelevantErrors)
					izStates = tempStates;
				else if (area == Area.RelevantErrors)
					rzStates = tempStates;
				Console.WriteLine ("Number of states in " + area.ToString () + ": " + tempStates.Count);
			}
			List<Node<State, Event>> states = model.DefaultCluster.StudentActionsModel.GetListStates();
			Console.WriteLine("Number of states > 0 supp: " + states.Count);
			Console.WriteLine("Number of states > 0.1 supp: " + states.Count(y => ((double)y.Specification.StudentFrequency / model.DefaultCluster.NumberOfStudents) > 0.1));
			Console.WriteLine("Number of states > 0.25 supp: " + states.Count(y => ((double)y.Specification.StudentFrequency / model.DefaultCluster.NumberOfStudents) > 0.25));
			Console.WriteLine("Number of states > 0.5 supp: " + states.Count(y => ((double)y.Specification.StudentFrequency / model.DefaultCluster.NumberOfStudents) > 0.5));
			Console.WriteLine("Number of states > 0.75 supp: " + states.Count(y => ((double)y.Specification.StudentFrequency / model.DefaultCluster.NumberOfStudents) > 0.75));
			Console.WriteLine("Number of states > 0.9 supp: " + states.Count(y => ((double)y.Specification.StudentFrequency / model.DefaultCluster.NumberOfStudents) > 0.9));
			Console.WriteLine("Number of events > 0 conf: " + model.DefaultCluster.StudentActionsModel.GetEventsConf(0).Count);
			Console.WriteLine("Number of events > 0.1 conf: " + model.DefaultCluster.StudentActionsModel.GetEventsConf(0.1).Count);
			Console.WriteLine("Number of events > 0.25 conf: " + model.DefaultCluster.StudentActionsModel.GetEventsConf(0.25).Count);
			Console.WriteLine ("Number of events > 0.5 conf: "+model.DefaultCluster.StudentActionsModel.GetEventsConf(0.5).Count);
			Console.WriteLine ("Number of events > 0.75 conf: "+model.DefaultCluster.StudentActionsModel.GetEventsConf(0.75).Count);
			Console.WriteLine ("Number of events > 0.9 conf: "+model.DefaultCluster.StudentActionsModel.GetEventsConf(0.9).Count);

			Console.WriteLine ("");
			Console.WriteLine ("Student freq in each error state:");
			Console.WriteLine ("RelevantErrors");
			foreach (Node<State, Event> state in rzStates) {
				if (state.Specification.GetType () != typeof(CorrectState)) {
					Console.WriteLine (state.Key + ": " + state.Specification.StudentFrequency);
				}
			}
			Console.WriteLine ("");
			Console.WriteLine ("IrrelevantErrors");
			foreach (Node<State, Event> state in izStates) {
				if (state.Specification.GetType () != typeof(CorrectState)) {
					Console.WriteLine (state.Key + ": " + state.Specification.StudentFrequency);
				}
			}
		}

		public static void CreateValPertage(string strDomainName, bool includeNoPlanActions, bool inPhases){
			//Console.WriteLine ("Op por año: "); //valPerYear==0 valPerYear==1 && Year>2013 && Year<2015 valPerYear==2 && Year>2014)
			//int opYear=int.Parse(Console.ReadLine());
			Console.WriteLine ("\nIterations: ");
			int iteration=int.Parse(Console.ReadLine());
			CreateWorkbooksVal (strDomainName, 10, iteration, ClusterMethod.ErrorsAndTime, true, includeNoPlanActions, inPhases);
			Dispose ();
			CreateWorkbooksVal (strDomainName, 10, iteration, ClusterMethod.EventsByZone, true, includeNoPlanActions, inPhases);
			Dispose ();
			CreateWorkbooksVal (strDomainName, 10, iteration, ClusterMethod.Errors, true, includeNoPlanActions, inPhases);
			Dispose ();
		}

		public static void CreateValPertageNoCluSeq(string strDomainName, bool includeNoPlanActions, bool inPhases){
			Console.WriteLine ("\nIterations: ");
			int iteration=int.Parse(Console.ReadLine());
			CreateWorkbooksVal (strDomainName, 10, iteration, ClusterMethod.Sequences, true, includeNoPlanActions, inPhases);
			Dispose ();
			CreateWorkbooksVal (strDomainName, 10, iteration, ClusterMethod.NoClusters, true, includeNoPlanActions, inPhases);
			Dispose ();
		}

		/*public static void CreateValStuByStu(string strDomainName){
			Console.WriteLine ("Year: ");
			int year=int.Parse(Console.ReadLine());
			CreateWorkbooksValStudentByStudent (strDomainName, ClusterMethod.ErrorsAndTime, year, false);
			Dispose ();
			CreateWorkbooksValStudentByStudent (strDomainName, ClusterMethod.Sequences, year, false);
			Dispose ();
			CreateWorkbooksValStudentByStudent (strDomainName, ClusterMethod.EventsByZone, year, false);
			Dispose ();
			CreateWorkbooksValStudentByStudent (strDomainName, ClusterMethod.NoClusters, year, false);
			Dispose ();
			CreateWorkbooksValStudentByStudent (strDomainName, ClusterMethod.Errors, year, false);
			Dispose ();
		}*/

		public static void CreateStuClu(string strDomainName){
			SaveStudentsByCluster (strDomainName, ClusterMethod.ErrorsAndTime);
			Dispose ();
			SaveStudentsByCluster (strDomainName, ClusterMethod.EventsByZone);
			Dispose ();
			SaveStudentsByCluster (strDomainName, ClusterMethod.Errors);
			Dispose ();
		}

		private static void SaveStudentsByCluster(string strDomainName, ClusterMethod cluMet){
			CreateModel (strDomainName, cluMet, 1, 0, true, false, false);
			PredictiveStudentModel model = StudentBehaviorPredictorControl.Instance(config).GetModel (strDomainName+"0", cluMet);
			string path = "DataFor"+cluMet + ".csv";
			if (File.Exists (path))
				File.Delete (path);

			System.IO.StreamWriter sw = new System.IO.StreamWriter(path,true,System.Text.Encoding.Unicode);
			sw.Write(model._clusteredData);
			sw.Close ();
		}

		public static void MeansDispertions(){
			MeansDispersion (ClusterMethod.ErrorsAndTime);
			MeansDispersion (ClusterMethod.EventsByZone);
			MeansDispersion (ClusterMethod.Errors);
		}

		public static void MeansDispertionsNoCluSeq(){
			MeansDispersion (ClusterMethod.NoClusters);
			MeansDispersion (ClusterMethod.Sequences);
		}

		public static void CreateSpreadsheetWorkbook(string filepath)
		{
			File.Copy("default.xlsx", filepath, true);
			using (SpreadsheetDocument spreadSheet = SpreadsheetDocument.Open (filepath, true)) {
				SharedStringTablePart shareStringPart;
				if (spreadSheet.WorkbookPart.GetPartsOfType<SharedStringTablePart>().Count() > 0)
				{
					shareStringPart = spreadSheet.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First();
				}
				else
				{
					shareStringPart = spreadSheet.WorkbookPart.AddNewPart<SharedStringTablePart>();
				}

				// Insert the text into the SharedStringTablePart.
				int index = InsertSharedStringItem("hola", shareStringPart);

				Sheet firstSheet = spreadSheet.WorkbookPart.Workbook.Sheets.GetFirstChild<Sheet> ();
				firstSheet.Remove ();
				/*string sheetId = firstSheet.Id.ToString ();
				firstSheet.Name = "SheetData1";
				WorksheetPart worksheetPart = (WorksheetPart)(spreadSheet.WorkbookPart.GetPartById (sheetId));
				// Insert cell A1 into the new worksheet.
				DocumentFormat.OpenXml.Spreadsheet.Cell cell = InsertCellInWorksheet("A", 1, worksheetPart);

				// Set the value of cell A1.
				cell.CellValue = new CellValue(index.ToString());
				cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

				DocumentFormat.OpenXml.Spreadsheet.Cell cell1 = InsertCellInWorksheet("B", 1, worksheetPart);

				cell1.CellValue = new CellValue("1.2");
				cell1.DataType = new EnumValue<CellValues>(CellValues.Number);
				index = InsertSharedStringItem("prueba", shareStringPart);
				cell = InsertCellInWorksheet("C", 1, worksheetPart);
				cell.CellValue = new CellValue(index.ToString());
				cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
				// Save the new worksheet.
				worksheetPart.Worksheet.Save();*/
				spreadSheet.WorkbookPart.Workbook.Save ();

				// Insert a new worksheet.
				WorksheetPart worksheetPart = InsertWorksheet(spreadSheet.WorkbookPart, "SheetData2");

				DocumentFormat.OpenXml.Spreadsheet.Cell cell1 = InsertCellInWorksheet("A", 1, worksheetPart);

				cell1.CellValue = new CellValue("2.2");
				cell1.DataType = new EnumValue<CellValues>(CellValues.Number);
				DocumentFormat.OpenXml.Spreadsheet.Cell cell2 = InsertCellInWorksheet("B", 1, worksheetPart);

				cell2.CellValue = new CellValue("4.2");
				cell2.DataType = new EnumValue<CellValues>(CellValues.Number);
				index = InsertSharedStringItem("prueba2", shareStringPart);
				DocumentFormat.OpenXml.Spreadsheet.Cell cell = InsertCellInWorksheet("C", 1, worksheetPart);
				cell.CellValue = new CellValue(index.ToString());
				cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
				// Save the new worksheet.
				worksheetPart.Worksheet.Save();
			}
		}

		private static List<DataSet> LoadExcel(ClusterMethod cluMet){
			List<DataSet> excels = new List<DataSet> ();
			foreach (string file in Directory.GetFiles("./")) {
				if (file.Contains (cluMet.ToString ()+"_")) {
					FileStream stream = File.Open(file, FileMode.Open, FileAccess.Read);
					IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
					DataSet result = excelReader.AsDataSet();
					excels.Add (result);
				}
			}
			return excels;
		}

		private static Dictionary<string,DataSet> LoadExcelMeans(string iterations){
			Dictionary<string,DataSet> excels = new Dictionary<string, DataSet> ();
			foreach (string file in Directory.GetFiles("./")) {
				if (file.Contains ("Means")) {
					FileStream stream = File.Open(file, FileMode.Open, FileAccess.Read);
					IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
					DataSet result = excelReader.AsDataSet();
					int index1 = file.IndexOf ("Means");
					int index2 = file.IndexOf (iterations);
					excels.Add (file.Substring(index1+5,index2-5-index1),result);
				}
			}
			return excels;
		}

		// Given a WorkbookPart, inserts a new worksheet.
		private static WorksheetPart InsertWorksheet(WorkbookPart workbookPart, string sheetName)
		{
			// Add a new worksheet part to the workbook.
			WorksheetPart newWorksheetPart = workbookPart.AddNewPart<WorksheetPart>();
			newWorksheetPart.Worksheet = new Worksheet(new SheetData());
			newWorksheetPart.Worksheet.Save();

			Sheets sheets = workbookPart.Workbook.GetFirstChild<Sheets>();
			string relationshipId = workbookPart.GetIdOfPart(newWorksheetPart);

			// Get a unique ID for the new sheet.
			uint sheetId = 1;
			if (sheets.Elements<Sheet>().Count() > 0)
			{
				sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
			}

			// Append the new worksheet and associate it with the workbook.
			Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = sheetName };
			sheets.Append(sheet);
			workbookPart.Workbook.Save();

			return newWorksheetPart;
		}

		// Given text and a SharedStringTablePart, creates a SharedStringItem with the specified text 
		// and inserts it into the SharedStringTablePart. If the item already exists, returns its index.
		private static int InsertSharedStringItem(string text, SharedStringTablePart shareStringPart)
		{
			// If the part does not contain a SharedStringTable, create one.
			if (shareStringPart.SharedStringTable == null)
			{
				shareStringPart.SharedStringTable = new SharedStringTable();
			}

			int i = 0;

			// Iterate through all the items in the SharedStringTable. If the text already exists, return its index.
			foreach (SharedStringItem item in shareStringPart.SharedStringTable.Elements<SharedStringItem>())
			{
				if (item.InnerText == text)
				{
					return i;
				}

				i++;
			}

			// The text does not exist in the part. Create the SharedStringItem and return its index.
			shareStringPart.SharedStringTable.AppendChild(new SharedStringItem(new DocumentFormat.OpenXml.Spreadsheet.Text(text)));
			shareStringPart.SharedStringTable.Save();

			return i;
		}

		// Given a column name, a row index, and a WorksheetPart, inserts a cell into the worksheet. 
		// If the cell already exists, returns it. 
		private static DocumentFormat.OpenXml.Spreadsheet.Cell InsertCellInWorksheet(string columnName, uint rowIndex, WorksheetPart worksheetPart)
		{
			Worksheet worksheet = worksheetPart.Worksheet;
			SheetData sheetData = worksheet.GetFirstChild<SheetData>();
			string cellReference = columnName + rowIndex;

			// If the worksheet does not contain a row with the specified row index, insert one.
			Row row;
			if (sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).Count() != 0)
			{
				row = sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
			}
			else
			{
				row = new Row() { RowIndex = rowIndex };
				sheetData.Append(row);
			}

			// If there is not a cell with the specified column name, insert one.  
			/*if (row.Elements<DocumentFormat.OpenXml.Spreadsheet.Cell>().Where(c => c.CellReference.Value == columnName + rowIndex).Count() > 0)
			{
				return row.Elements<DocumentFormat.OpenXml.Spreadsheet.Cell>().Where(c => c.CellReference.Value == cellReference).First();
			}
			else
			{
				// Cells must be in sequential order according to CellReference. Determine where to insert the new cell.
				DocumentFormat.OpenXml.Spreadsheet.Cell refCell = null;
				foreach (DocumentFormat.OpenXml.Spreadsheet.Cell cell in row.Elements<DocumentFormat.OpenXml.Spreadsheet.Cell>())
				{
					if (string.Compare(cell.CellReference.Value, cellReference, true) > 0)
					{
						refCell = cell;
						break;
					}
				}*/

				DocumentFormat.OpenXml.Spreadsheet.Cell newCell = new DocumentFormat.OpenXml.Spreadsheet.Cell() { CellReference = cellReference };
				//row.InsertBefore(newCell, refCell);
			row.Append(newCell);
				//worksheet.Save();
				return newCell;
			//}
		}

		private static string GetValue(SpreadsheetDocument doc, DocumentFormat.OpenXml.Spreadsheet.Cell cell)
		{
			string value = cell.CellValue.InnerText;
			if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
			{
				return doc.WorkbookPart.SharedStringTablePart.SharedStringTable.ChildElements.GetItem(int.Parse(value)).InnerText;
			}
			return value;
		}

		private static List<DataSet> GetSubtotals(List<DataSet> excels){
			List<DataSet> subtotals = new List<DataSet> ();

			foreach (DataSet ds in excels) {
				DataSet dsnew = new DataSet ();
				if (ds != null) {
					foreach (DataTable dt in ds.Tables) {
						DataTable dtnew = new DataTable (dt.TableName);
						DataColumn dcarea = new DataColumn ("Area");
						DataColumn dccluster = new DataColumn ("Cluster");
						DataColumn dcfreq = new DataColumn ("Freq", typeof(int));
						DataColumn dcen = new DataColumn ("EN", typeof(double));
						DataColumn dcepn = new DataColumn ("EpN", typeof(double));
						DataColumn dcmen = new DataColumn ("Media_E", typeof(double));
						DataColumn dcmepn = new DataColumn ("Media_Ep", typeof(double));
						dtnew.Columns.Add (dcarea);
						dtnew.Columns.Add (dccluster);
						dtnew.Columns.Add (dcfreq);
						dtnew.Columns.Add (dcen);
						dtnew.Columns.Add (dcepn);
						dtnew.Columns.Add (dcmen);
						dtnew.Columns.Add (dcmepn);

						string area = "";
						Dictionary<int,int> clusters = new Dictionary<int, int> ();
						Dictionary<int,double> freq = new Dictionary<int, double> ();
						Dictionary<int,double> en = new Dictionary<int, double> ();
						Dictionary<int,double> epn = new Dictionary<int, double> ();
						Dictionary<int,double> men = new Dictionary<int, double> ();
						Dictionary<int,double> mepn = new Dictionary<int, double> ();

						for (int i = 0; i < dt.Rows.Count; i++) {
							switch (dt.Rows [i].ItemArray [0].ToString ()) {
							case "TOTAL":
								area = dt.Rows [i].ItemArray [0].ToString ();
								clusters.Add (-1, 0);
								for (int j = 0; j < dt.Columns.Count; j++) {
									if (dt.Rows [i].ItemArray [j].ToString ().Contains ("CLUSTER")) {
										clusters.Add (int.Parse (dt.Rows [i].ItemArray [j].ToString ().Split (' ') [1]), j);
									}
								}

								freq = new Dictionary<int, double> ();
								en = new Dictionary<int, double> ();
								epn = new Dictionary<int, double> ();
								men = new Dictionary<int, double> ();
								mepn = new Dictionary<int, double> ();
								break;
							case "Suma FREQ":
								for (int j = 0; j < clusters.Count; j++) {
									int cluster = clusters.Keys.ElementAt (j);
									if (dt.Rows [i].ItemArray [clusters [cluster] + 1].ToString () != "#DIV/0!")
										freq.Add (cluster, (double)dt.Rows [i].ItemArray [clusters [cluster] + 1]);
									else
										men.Add (cluster, 0);
								}
								break;
							case "Suma EN":
								for (int j = 0; j < clusters.Count; j++) {
									int cluster = clusters.Keys.ElementAt (j);
									if (dt.Rows [i].ItemArray [clusters [cluster] + 1].ToString () != "#DIV/0!")
										en.Add (cluster, (double)dt.Rows [i].ItemArray [clusters [cluster] + 1]);
									else
										men.Add (cluster, 0);
								}
								break;
							case "Suma E'N":
								for (int j = 0; j < clusters.Count; j++) {
									int cluster = clusters.Keys.ElementAt (j);
									if (dt.Rows [i].ItemArray [clusters [cluster] + 1].ToString () != "#DIV/0!")
										epn.Add (cluster, (double)dt.Rows [i].ItemArray [clusters [cluster] + 1]);
									else
										men.Add (cluster, 0);
								}
								break;
							case "Media E":
								for (int j = 0; j < clusters.Count; j++) {
									int cluster = clusters.Keys.ElementAt (j);
									if (dt.Rows [i].ItemArray [clusters [cluster] + 1].ToString () != "#DIV/0!")
										men.Add (cluster, (double)dt.Rows [i].ItemArray [clusters [cluster] + 1]);
									else
										men.Add (cluster, 0);
								}
								break;
							case "Media E'":
								for (int j = 0; j < clusters.Count; j++) {
									int cluster = clusters.Keys.ElementAt (j);
									DataRow dr = dtnew.NewRow ();
									dr [0] = area;
									dr [1] = cluster;
									dr [2] = freq [cluster];
									dr [3] = en [cluster];
									dr [4] = epn [cluster];
									dr [5] = men [cluster];
									if (dt.Rows [i].ItemArray [clusters [cluster] + 1].ToString () != "#DIV/0!")
										dr [6] = (double)dt.Rows [i].ItemArray [clusters [cluster] + 1];
									else
										dr [6] = 0;
									dtnew.Rows.Add (dr);
								}
								break;
							case "CorrectFlow":
								area = dt.Rows [i].ItemArray [0].ToString ();
								freq = new Dictionary<int, double> ();
								en = new Dictionary<int, double> ();
								epn = new Dictionary<int, double> ();
								men = new Dictionary<int, double> ();
								mepn = new Dictionary<int, double> ();
								break;
							case "IrrelevantErrors":
								area = dt.Rows [i].ItemArray [0].ToString ();
								freq = new Dictionary<int, double> ();
								en = new Dictionary<int, double> ();
								epn = new Dictionary<int, double> ();
								men = new Dictionary<int, double> ();
								mepn = new Dictionary<int, double> ();
								break;
							case "RelevantErrors":
								area = dt.Rows [i].ItemArray [0].ToString ();
								freq = new Dictionary<int, double> ();
								en = new Dictionary<int, double> ();
								epn = new Dictionary<int, double> ();
								men = new Dictionary<int, double> ();
								mepn = new Dictionary<int, double> ();
								break;
							default:
								break;
							}
						}
						dsnew.Tables.Add (dtnew);
					}
				}
				subtotals.Add (dsnew);
			}
			return subtotals;
		}

		private static void MeansSum(){
			Console.WriteLine ("\nIterations: ");
			string iterations=Console.ReadLine();
			if (File.Exists ("MeansSum"+iterations+".xlsx"))
				File.Delete ("MeansSum"+iterations+".xlsx");
			Dictionary<string, DataSet> excels = LoadExcelMeans (iterations);
			File.Copy("default.xlsx", "MeansSum"+iterations+".xlsx", true);
			using (SpreadsheetDocument spreadSheet = SpreadsheetDocument.Open ("MeansSum"+iterations+".xlsx", true)) {
				Sheet firstSheet = spreadSheet.WorkbookPart.Workbook.Sheets.GetFirstChild<Sheet> ();
				firstSheet.Remove ();
				spreadSheet.WorkbookPart.Workbook.Save ();
				SharedStringTablePart shareStringPart;
				if (spreadSheet.WorkbookPart.GetPartsOfType<SharedStringTablePart> ().Count () > 0) {
					shareStringPart = spreadSheet.WorkbookPart.GetPartsOfType<SharedStringTablePart> ().First ();
				} else {
					shareStringPart = spreadSheet.WorkbookPart.AddNewPart<SharedStringTablePart> ();
				}
				int indexSharedString = 0;
				foreach (string cluMeth in excels.Keys) {
					WorksheetPart worksheetPart = InsertWorksheet (spreadSheet.WorkbookPart,cluMeth);
					indexSharedString = InsertSharedStringItem ("Conf", shareStringPart);
					DocumentFormat.OpenXml.Spreadsheet.Cell cell = InsertCellInWorksheet ("A", 1, worksheetPart);
					cell.CellValue = new CellValue (indexSharedString.ToString ());
					cell.DataType = new EnumValue<CellValues> (CellValues.SharedString);

					indexSharedString = InsertSharedStringItem ("Sup", shareStringPart);
					cell = InsertCellInWorksheet ("B", 1, worksheetPart);
					cell.CellValue = new CellValue (indexSharedString.ToString ());
					cell.DataType = new EnumValue<CellValues> (CellValues.SharedString);

					indexSharedString = InsertSharedStringItem ("Error Mean", shareStringPart);
					cell = InsertCellInWorksheet ("C", 1, worksheetPart);
					cell.CellValue = new CellValue (indexSharedString.ToString ());
					cell.DataType = new EnumValue<CellValues> (CellValues.SharedString);

					for (int i=0;i<excels [cluMeth].Tables.Count;i++) {
						DataTable dt = excels [cluMeth].Tables [i];
						string[] tabName = dt.TableName.Split (' ');
						string conf = tabName [0];
						string sup = tabName [1];
						cell = InsertCellInWorksheet ("A", Convert.ToUInt32 (i+2), worksheetPart);
						cell.CellValue = new CellValue (conf);
						cell.DataType = new EnumValue<CellValues> (CellValues.Number);

						cell = InsertCellInWorksheet ("B", Convert.ToUInt32 (i+2), worksheetPart);
						cell.CellValue = new CellValue (sup);
						cell.DataType = new EnumValue<CellValues> (CellValues.Number);

						cell = InsertCellInWorksheet ("C", Convert.ToUInt32 (i+2), worksheetPart);
						cell.CellValue = new CellValue (double.Parse(dt.Rows[1].ItemArray[2].ToString()).ToString(new CultureInfo ("en-US")));
						cell.DataType = new EnumValue<CellValues> (CellValues.Number);
					}

					worksheetPart.Worksheet.Save ();
				}
			}
		}

		private static void MeansDispersion(ClusterMethod cluMet){
			List<DataSet> excels = LoadExcel(cluMet);
			List<DataSet> subtotals = GetSubtotals(excels);


			StringBuilder strExcelXml = new StringBuilder();
				
			#region Header
			strExcelXml.Append ("<?xml version=\"1.0\"?>");
			strExcelXml.Append ("<?mso-application progid=\"Excel.Sheet\"?>");
			strExcelXml.Append ("<Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\" ");
			strExcelXml.Append ("xmlns:o=\"urn:schemas-microsoft-com:office:office\" ");
			strExcelXml.Append ("xmlns:x=\"urn:schemas-microsoft-com:office:excel\" ");
			strExcelXml.Append ("xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\" ");
			strExcelXml.Append ("xmlns:html=\"http://www.w3.org/TR/REC-html40\">");
			strExcelXml.Append ("<DocumentProperties xmlns=\"urn:schemas-microsoft-com:office:office\">");
			strExcelXml.Append ("</DocumentProperties>");
			strExcelXml.Append ("<ExcelWorkbook xmlns=\"urn:schemas-microsoft-com:office:excel\">");
			strExcelXml.Append ("<ProtectStructure>False</ProtectStructure>");
			strExcelXml.Append ("<ProtectWindows>False</ProtectWindows>");
			strExcelXml.Append ("</ExcelWorkbook>");

			strExcelXml.Append ("<Styles>");
			strExcelXml.Append ("<Style ss:ID=\"Default\">");
			strExcelXml.Append ("<Alignment ss:Vertical=\"Bottom\"/>");
			strExcelXml.Append ("<Borders/>");
			strExcelXml.Append ("<Font/>");
			strExcelXml.Append ("<Interior/>");
			strExcelXml.Append ("<NumberFormat/>");
			strExcelXml.Append ("<Protection/>");
			strExcelXml.Append ("</Style>");
			strExcelXml.Append ("<Style ss:ID=\"bold\">");
			strExcelXml.Append ("<Font ss:Bold=\"1\"/>");
			strExcelXml.Append ("</Style>");
			strExcelXml.Append ("</Styles>");
			#endregion

			int iteractions = excels.Count;



			int numtables = subtotals [0].Tables.Count;

			for (int table = 0; table < numtables; table++) {
				strExcelXml.Append("<Worksheet ss:Name=\""+subtotals[0].Tables [table].TableName+"\">\n");
				strExcelXml.Append ("<Table>");
				#region headersheet
				strExcelXml.Append ("<column ss:autofitwidth=\"1\"/>\n");
				strExcelXml.Append ("<Row ss:AutoFitHeight=\"1\" >\n");
				strExcelXml.Append ("<Cell ss:StyleID=\"bold\"><Data ss:Type=\"String\">");
				strExcelXml.Append ("Iteraction");
				strExcelXml.Append ("</Data></Cell>\n");
				strExcelXml.Append ("<Cell ss:StyleID=\"bold\"><Data ss:Type=\"String\">");
				strExcelXml.Append ("Error");
				strExcelXml.Append ("</Data></Cell>\n");
				strExcelXml.Append ("<Cell ss:StyleID=\"bold\"><Data ss:Type=\"String\">");
				strExcelXml.Append ("Mean");
				strExcelXml.Append ("</Data></Cell>\n");
				strExcelXml.Append ("</Row>\n");
				#endregion

				int iter = 1;
				foreach (DataSet ds in subtotals) {
					DataTable dttemp = ds.Tables [table];
					strExcelXml.Append ("<Row ss:AutoFitHeight=\"1\" >\n");
					strExcelXml.Append ("<Cell><Data ss:Type=\"Number\">");
					strExcelXml.Append (iter);
					strExcelXml.Append ("</Data></Cell>\n");
					strExcelXml.Append ("<Cell><Data ss:Type=\"Number\">");
					strExcelXml.Append (((double)dttemp.Select ("Area='TOTAL' AND Cluster='-1'") [0].ItemArray [5]).ToString (new CultureInfo ("en-US")));
					strExcelXml.Append ("</Data></Cell>\n");
					strExcelXml.Append ("<Cell ss:Formula=\"SUM(R2C2:R" + (iteractions + 1).ToString () + "C2)/" + iteractions.ToString () + "\">");
					strExcelXml.Append ("</Cell>\n");
					strExcelXml.Append ("<Cell ss:Formula=\"VAR.S(R2C2:R" + (iteractions + 1).ToString () + "C2)\">");
					strExcelXml.Append ("</Cell>\n");
					strExcelXml.Append ("</Row>\n");
					iter++;
				}

				strExcelXml.Append("</Table>\n");
				strExcelXml.Append("</Worksheet>\n");
			}
			strExcelXml.Append ("</Workbook>\n");
			if (File.Exists ("Means"+cluMet+iteractions+".xls"))
				File.Delete ("Means"+cluMet+iteractions+".xls");
			File.WriteAllText("Means"+cluMet+iteractions+".xls", strExcelXml.ToString());
		}

		private static void IterationsMean(ClusterMethod cluMet){
			List<DataSet> excels = LoadExcel(cluMet);
			List<DataSet> subtotals = GetSubtotals(excels);

			StringBuilder strExcelXml = new StringBuilder ();
			#region Header
			strExcelXml.Append ("<?xml version=\"1.0\"?>");
			strExcelXml.Append ("<?mso-application progid=\"Excel.Sheet\"?>");
			strExcelXml.Append ("<Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\" ");
			strExcelXml.Append ("xmlns:o=\"urn:schemas-microsoft-com:office:office\" ");
			strExcelXml.Append ("xmlns:x=\"urn:schemas-microsoft-com:office:excel\" ");
			strExcelXml.Append ("xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\" ");
			strExcelXml.Append ("xmlns:html=\"http://www.w3.org/TR/REC-html40\">");
			strExcelXml.Append ("<DocumentProperties xmlns=\"urn:schemas-microsoft-com:office:office\">");
			strExcelXml.Append ("</DocumentProperties>");
			strExcelXml.Append ("<ExcelWorkbook xmlns=\"urn:schemas-microsoft-com:office:excel\">");
			strExcelXml.Append ("<ProtectStructure>False</ProtectStructure>");
			strExcelXml.Append ("<ProtectWindows>False</ProtectWindows>");
			strExcelXml.Append ("</ExcelWorkbook>");

			strExcelXml.Append ("<Styles>");
			strExcelXml.Append ("<Style ss:ID=\"Default\">");
			strExcelXml.Append ("<Alignment ss:Vertical=\"Bottom\"/>");
			strExcelXml.Append ("<Borders/>");
			strExcelXml.Append ("<Font/>");
			strExcelXml.Append ("<Interior/>");
			strExcelXml.Append ("<NumberFormat/>");
			strExcelXml.Append ("<Protection/>");
			strExcelXml.Append ("</Style>");
			strExcelXml.Append ("<Style ss:ID=\"bold\">");
			strExcelXml.Append ("<Font ss:Bold=\"1\"/>");
			strExcelXml.Append ("</Style>");
			strExcelXml.Append ("</Styles>");
			#endregion
			if(subtotals.Count>0){
				int numIterations = subtotals.Count;
				for (int i = 0; i < subtotals [0].Tables.Count; i++) {
					strExcelXml.Append("<Worksheet ss:Name=\""+subtotals [0].Tables[i].TableName+"\">\n");
					strExcelXml.Append("<Table>");
					strExcelXml.Append("<column ss:autofitwidth=\"1\"/>\n");
					strExcelXml.Append("<Row ss:AutoFitHeight=\"1\" >\n");
					strExcelXml.Append("<Cell ss:StyleID=\"bold\"><Data ss:Type=\"String\">");
					strExcelXml.Append("Area");
					strExcelXml.Append("</Data></Cell>\n");
					strExcelXml.Append("<Cell ss:StyleID=\"bold\"><Data ss:Type=\"String\">");
					strExcelXml.Append("Cluster");
					strExcelXml.Append("</Data></Cell>\n");
					strExcelXml.Append("<Cell ss:StyleID=\"bold\"><Data ss:Type=\"String\">");
					strExcelXml.Append("Freq");
					strExcelXml.Append("</Data></Cell>\n");
					strExcelXml.Append("<Cell ss:StyleID=\"bold\"><Data ss:Type=\"String\">");
					strExcelXml.Append("EN");
					strExcelXml.Append("</Data></Cell>\n");
					strExcelXml.Append("<Cell ss:StyleID=\"bold\"><Data ss:Type=\"String\">");
					strExcelXml.Append("E'N");
					strExcelXml.Append("</Data></Cell>\n");
					strExcelXml.Append("<Cell ss:StyleID=\"bold\"><Data ss:Type=\"String\">");
					strExcelXml.Append("ME");
					strExcelXml.Append("</Data></Cell>\n");
					strExcelXml.Append("<Cell ss:StyleID=\"bold\"><Data ss:Type=\"String\">");
					strExcelXml.Append("ME'");
					strExcelXml.Append("</Data></Cell>\n");
					strExcelXml.Append("</Row>\n");

					DataTable dtTemp = new DataTable ();
					DataColumn dcarea = new DataColumn ("Area");
					DataColumn dccluster = new DataColumn ("Cluster");
					DataColumn dcfreq = new DataColumn ("Freq",typeof(int));
					DataColumn dcen = new DataColumn ("EN",typeof(double));
					DataColumn dcepn = new DataColumn ("EpN",typeof(double));
					DataColumn dcmen = new DataColumn ("Media_E",typeof(double));
					DataColumn dcmepn = new DataColumn ("Media_Ep",typeof(double));
					dtTemp.Columns.Add (dcarea);
					dtTemp.Columns.Add (dccluster);
					dtTemp.Columns.Add (dcfreq);
					dtTemp.Columns.Add (dcen);
					dtTemp.Columns.Add (dcepn);
					dtTemp.Columns.Add (dcmen);
					dtTemp.Columns.Add (dcmepn);

					foreach (DataSet ds in subtotals) {
						foreach(DataRow dr in ds.Tables[i].Rows){
							string area = dr ["Area"].ToString();
							int cluster = int.Parse(dr ["Cluster"].ToString());
							int freq = int.Parse (dr ["Freq"].ToString());
							double en = int.Parse (dr ["EN"].ToString());
							double epn = int.Parse (dr ["EpN"].ToString());
							double men = int.Parse (dr ["Media_E"].ToString());
							double mepn = int.Parse (dr ["Media_Ep"].ToString());
							DataRow[] drSelect = dtTemp.Select ("Area='" + area + "' AND Cluster=" + cluster);
							if (drSelect.Length == 0) {
								DataRow drnew = dtTemp.NewRow ();
								drnew [0] = area;
								drnew [1] = cluster;
								drnew [2] = freq;
								drnew [3] = en;
								drnew [4] = epn;
								drnew [5] = men;
								drnew [6] = mepn;
								dtTemp.Rows.Add (drnew);
							} else {
								int rowIndex = dtTemp.Rows.IndexOf (drSelect [0]);
								dtTemp.Rows [rowIndex] [2] = (int)dtTemp.Rows [rowIndex] [2] + freq;
								dtTemp.Rows [rowIndex] [3] = (double)dtTemp.Rows [rowIndex] [3] + en;
								dtTemp.Rows [rowIndex] [4] = (double)dtTemp.Rows [rowIndex] [4] + epn;
								dtTemp.Rows [rowIndex] [5] = (double)dtTemp.Rows [rowIndex] [5] + men;
								dtTemp.Rows [rowIndex] [6] = (double)dtTemp.Rows [rowIndex] [6] + mepn;
							}
						}

					}

					foreach (DataRow dr in dtTemp.Rows) {
						strExcelXml.Append("<Row ss:AutoFitHeight=\"1\" >\n");
						strExcelXml.Append("<Cell><Data ss:Type=\"String\">");
						strExcelXml.Append(dr[0].ToString());
						strExcelXml.Append("</Data></Cell>\n");
						strExcelXml.Append("<Cell><Data ss:Type=\"Number\">");
						strExcelXml.Append(dr[1]);
						strExcelXml.Append("</Data></Cell>\n");
						strExcelXml.Append("<Cell><Data ss:Type=\"Number\">");
						strExcelXml.Append(((double)dr[2]/numIterations).ToString(new CultureInfo ("en-US")));
						strExcelXml.Append("</Data></Cell>\n");
						strExcelXml.Append("<Cell><Data ss:Type=\"Number\">");
						strExcelXml.Append(((double)dr[3]/numIterations).ToString(new CultureInfo ("en-US")));
						strExcelXml.Append("</Data></Cell>\n");
						strExcelXml.Append("<Cell><Data ss:Type=\"Number\">");
						strExcelXml.Append(((double)dr[4]/numIterations).ToString(new CultureInfo ("en-US")));
						strExcelXml.Append("</Data></Cell>\n");
						strExcelXml.Append("<Cell><Data ss:Type=\"Number\">");
						strExcelXml.Append(((double)dr[5]/numIterations).ToString(new CultureInfo ("en-US")));
						strExcelXml.Append("</Data></Cell>\n");
						strExcelXml.Append("<Cell><Data ss:Type=\"Number\">");
						strExcelXml.Append(((double)dr[6]/numIterations).ToString(new CultureInfo ("en-US")));
						strExcelXml.Append("</Data></Cell>\n");
						strExcelXml.Append("</Row>\n");
					}

					strExcelXml.Append("</Table>\n");
					strExcelXml.Append("</Worksheet>\n");
					if (i == 0)
						strExcelXml.Append ("<WorksheetOptions xmlns=\"urn:schemas-microsoft-com:office:excel\"><Selected/></WorksheetOptions>");
				}
			}

			strExcelXml.Append("</Workbook>\n");

			string path = "IterationsMean.xls";
			if (File.Exists (path))
				File.Delete (path);

			System.IO.StreamWriter sw = new System.IO.StreamWriter(path,true,System.Text.Encoding.Unicode);
			sw.Write(strExcelXml.ToString());
			sw.Close ();
		}

		#region readExcelXML
		private static ColumnType getDefaultType() {
			return new ColumnType(typeof(String));
		}

		struct ColumnType {
			public Type type;
			private string name;
			public ColumnType(Type type) { this.type = type; this.name = type.ToString().ToLower(); }
			public object ParseString(string input) {
				if (String.IsNullOrEmpty(input))
					return DBNull.Value;
				switch (type.ToString()) {
				case "system.datetime":
					return DateTime.Parse(input);
				case "system.decimal":
					return decimal.Parse(input);
				case "system.boolean":
					return bool.Parse(input);
				default:
					return input;
				}
			}
		}


		private static ColumnType getType(XmlNode data) {
			string type = null;
			if (data.Attributes["ss:Type"] == null || data.Attributes["ss:Type"].Value == null)
				type = "";
			else
				type = data.Attributes["ss:Type"].Value;

			switch (type) {
			case "DateTime":
				return new ColumnType(typeof(DateTime));
			case "Boolean":
				return new ColumnType(typeof(Boolean));
			case "Number":
				return new ColumnType(typeof(Decimal));
			case "":
				decimal test2;
				if (data == null || String.IsNullOrEmpty(data.InnerText) || decimal.TryParse(data.InnerText, out test2)) {
					return new ColumnType(typeof(Decimal));
				} else {
					return new ColumnType(typeof(String));
				}
			default://"String"
				return new ColumnType(typeof(String));
			}
		}

		public static DataSet ImportExcelXML (string fileName, bool hasHeaders, bool autoDetectColumnType) {
			StreamReader sr = new StreamReader( fileName);
			Stream st = (Stream) sr.BaseStream;
			return ImportExcelXML( st, hasHeaders, autoDetectColumnType);
		}

		private static DataSet ImportExcelXML(Stream inputFileStream, bool hasHeaders, bool autoDetectColumnType) {
			XmlDocument doc = new XmlDocument();
			doc.Load(new XmlTextReader(inputFileStream));
			XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);

			nsmgr.AddNamespace("o", "urn:schemas-microsoft-com:office:office");
			nsmgr.AddNamespace("x", "urn:schemas-microsoft-com:office:excel");
			nsmgr.AddNamespace("ss", "urn:schemas-microsoft-com:office:spreadsheet");

			DataSet ds = new DataSet();

			foreach (XmlNode node in 
				doc.DocumentElement.SelectNodes("//ss:Worksheet", nsmgr)) {
				DataTable dt = new DataTable(node.Attributes["ss:Name"].Value);
				ds.Tables.Add(dt);
				XmlNodeList rows = node.SelectNodes("ss:Table/ss:Row", nsmgr);
				if (rows.Count > 0) {

					//*************************
					//Add Columns To Table from header row
					//*************************
					List<ColumnType> columns = new List<ColumnType>();
					int startIndex = 0;
					if (hasHeaders) {
						foreach (XmlNode data in rows[0].SelectNodes("ss:Cell/ss:Data", nsmgr)) {
							columns.Add(new ColumnType(typeof(string)));//default to text
							dt.Columns.Add(data.InnerText, typeof(string));
						}
						startIndex++;
					}
					//*************************
					//Update Data-Types of columns if Auto-Detecting
					//*************************
					if (autoDetectColumnType && rows.Count > 0) {
						XmlNodeList cells = rows[startIndex].SelectNodes("ss:Cell", nsmgr);
						int actualCellIndex = 0;
						for (int cellIndex = 0; cellIndex < cells.Count; cellIndex++) {
							XmlNode cell = cells[cellIndex];
							if (cell.Attributes["ss:Index"] != null)
								actualCellIndex = 
									int.Parse(cell.Attributes["ss:Index"].Value) - 1;

							ColumnType autoDetectType = 
								getType(cell.SelectSingleNode("ss:Data", nsmgr));

							if (actualCellIndex >= dt.Columns.Count) {
								dt.Columns.Add("Column" + 
									actualCellIndex.ToString(), autoDetectType.type);
								columns.Add(autoDetectType);
							} else {
								dt.Columns[actualCellIndex].DataType = autoDetectType.type;
								columns[actualCellIndex] = autoDetectType;
							}

							actualCellIndex++;
						}
					}
					//*************************
					//Load Data
					//*************************
					for (int i = startIndex; i < rows.Count; i++) {
						DataRow row = dt.NewRow();
						XmlNodeList cells = rows[i].SelectNodes("ss:Cell", nsmgr);
						int actualCellIndex = 0;
						for (int cellIndex = 0; cellIndex < cells.Count; cellIndex++) {
							XmlNode cell = cells[cellIndex];
							if (cell.Attributes["ss:Index"] != null)
								actualCellIndex = int.Parse(cell.Attributes["ss:Index"].Value) - 1;

							XmlNode data = cell.SelectSingleNode("ss:Data", nsmgr);

							if (actualCellIndex >= dt.Columns.Count) {
								for (int ii = dt.Columns.Count; ii < actualCellIndex; ii++) {
									dt.Columns.Add("Column" + actualCellIndex.ToString(), typeof(string));columns.Add(getDefaultType());
								} // ii
								ColumnType autoDetectType = 
									getType(cell.SelectSingleNode("ss:Data", nsmgr));
								dt.Columns.Add("Column" + actualCellIndex.ToString(), 
									typeof(string));
								columns.Add(autoDetectType);
							}
							if (data != null)
								row[actualCellIndex] = data.InnerText;

							actualCellIndex++;
						}

						dt.Rows.Add(row);
					}
				}
			}
			return ds;
		}
		#endregion



		private static void SaveLogsSql(string strDomainName, bool includeNoPlanActions, bool inPhases){
			string ontologyPath = config.OntologyPath.Replace ('\\', Path.DirectorySeparatorChar);
			string logsPath = config.LogsPath.Replace ('\\', Path.DirectorySeparatorChar);
			string expertConfPath = config.DomainConfigurationPath.Replace ('\\', Path.DirectorySeparatorChar);
			int initialCol = config.InitialColumn;
			int intialRow = config.InitialRow;
			ExpertControl expert = ExpertControl.Instance(ontologyPath, logsPath, expertConfPath, initialCol, intialRow);
			DomainActions domain = expert.CreateDomain (strDomainName);
			WorldControl world = WorldControl.Instance (ontologyPath, logsPath);
			DomainLog logs = StudentControl.Instance (ontologyPath, logsPath, expertConfPath).GetDomainLogsFromOntology (domain, expert.OtherErrors, world.WorldErrors);
			string sql = "";
			string sql2 = "";
			int temp = 0;

			string table1 = "Logs";
			string table2 = "Students";
            
            foreach (Student stu in logs.GetStudents())
			{
				StudentLog stuLog = logs.GetStudentLog (stu.Key);
				/*if (strDomainName.Contains ("Val")) {
					table1 = "LogsVal";
					table2 = "StudentsVal";
				}*/
				int seq = 1;
                int phase = 0;
                if (inPhases)
                    table1 = "LogsPhase0";
                foreach (LogEntry log in stuLog.Logs) {
                    if (log.GetType().BaseType != typeof(NoPlanAllowedActionLog) || includeNoPlanActions)
                    {
                        string action = "";
                        if (log.GetType().BaseType == typeof(ActionLog))
                        {
                            action = log.Action.Key;
                        }
                        else if (log.GetType() == typeof(DepErrorLog))
                        {
                            Dependence fail = ((DepErrorLog)log).FailedDependence;
                            action = log.Action.Key + "_" + fail.Key;
                        }
                        else if (log.GetType() == typeof(IncompErrorLog))
                        {
                            Incompatibility fail = ((IncompErrorLog)log).FailedIncompatibility;
                            action = log.Action.Key + "_" + fail.Key;
                        }
                        else if (log.GetType().BaseType == typeof(TimeErrorLog))
                        {
                            Error fail = (log.GetType() == typeof(MinTimeErrorLog)) ? log.Action.MinTimeError : log.Action.MaxTimeError;
                            action = log.Action.Key + "_" + fail.Key;
                        }
                        else if (log.GetType() == typeof(WorldErrorLog))
                        {
                            Error fail = ((WorldErrorLog)log).ErrorAssociated;
                            action = log.Action.Key + "_" + fail.Key;
                        }
                        else if (log.GetType() == typeof(OtherErrorLog))
                        {
                            Error fail = ((OtherErrorLog)log).ErrorAssociated;
                            action = log.Action.Key + "_" + fail.Key;
                        }

                        if (inPhases && log.GetType().BaseType == typeof(ActionLog) && log.Action.InitPhase)
                        {
                            phase++;
                            table1 = "LogsPhase" + phase;
                        }

                        sql = sql + "insert into " + table1 + " (studentid, domain, actionkey, sequence, date) values('"
                    + stu.Key + "','"
                    + strDomainName + "','"
                    + action + "',"
                    + seq + ",'"
                    + log.DateLog.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                        seq++;
                    }
				}

				sql2 = sql2 + "insert into "+table2+" (id) values('"
					+ stu.Key +"') ";
				temp++;
			}
			SqlConnection con = new SqlConnection (config.BDDConString);
			SqlCommand cmd = new SqlCommand ("delete from Logs; delete from LogsPhase0; delete from LogsPhase1; delete from LogsPhase2; delete from LogsPhase3; delete from Students;", con);
            con.Open ();
			cmd.ExecuteNonQuery ();

			cmd = new SqlCommand (sql, con);
			cmd.ExecuteNonQuery ();
			cmd.CommandText=sql2;
			cmd.ExecuteNonQuery ();
			con.Close ();

			/*AdomdConnection conad = new AdomdConnection (ConfigurationManager.AppSettings ["BDDConString"].ToString ());
			conad.Open ();
			AdomdCommand command = new AdomdCommand("INSERT INTO MINING MODEL [EntireLab]",conad); 
			command.ExecuteNonQuery ();
			conad.Close ();*/
		}

		private static void Dispose ()
		{
			ExpertControl.DisposeInstance ();
			WorldControl.DisposeInstance ();
			StudentControl.DisposeInstance ();
			StudentBehaviorPredictorControl.DisposeInstance ();
			validationModels.Clear ();
			validationLogs.Clear ();
		}

		private static void TestModelDate(string strDomainName, ClusterMethod cluMet){
			string ontologyPath = config.OntologyPath.Replace ('\\', Path.DirectorySeparatorChar);
			string logsPath = config.LogsPath.Replace ('\\', Path.DirectorySeparatorChar);
			string expertConfPath = config.DomainConfigurationPath.Replace ('\\', Path.DirectorySeparatorChar);
			int initialCol = config.InitialColumn;
			int intialRow = config.InitialRow;
			ExpertControl expert = ExpertControl.Instance(ontologyPath, logsPath, expertConfPath, initialCol, intialRow);
			DomainActions domain = expert.CreateDomain (strDomainName);
			WorldControl world = WorldControl.Instance (ontologyPath, logsPath);
			DomainLog logs = StudentControl.Instance (ontologyPath, logsPath, expertConfPath).GetDomainLogsFromOntology (domain, expert.OtherErrors, world.WorldErrors);
			StudentBehaviorPredictorControl.Instance(config).GetModelFromDate (logs, new DateTime (2010, 1, 1),false);
		}

		private static PredictiveStudentModel CreateModel(string strDomainName, ClusterMethod cluMet, bool includeNoPlanActions, bool inPhases)
		{
			string ontologyPath = config.OntologyPath.Replace('\\', Path.DirectorySeparatorChar);
			string logsPath = config.LogsPath.Replace('\\', Path.DirectorySeparatorChar);
			string expertConfPath = config.DomainConfigurationPath.Replace('\\', Path.DirectorySeparatorChar);
			int initialCol = config.InitialColumn;
			int intialRow = config.InitialRow;
			ExpertControl expert = ExpertControl.Instance(ontologyPath, logsPath, expertConfPath, initialCol, intialRow);
			DomainActions domain = expert.GetDomainActions(strDomainName);
			if (domain == null)
				domain = expert.CreateDomain(strDomainName);
			WorldControl world = WorldControl.Instance(ontologyPath, logsPath);
			DomainLog logs = StudentControl.Instance(ontologyPath, logsPath, expertConfPath).GetDomainLogsFromOntology(domain, expert.OtherErrors, world.WorldErrors);
			StudentBehaviorPredictorControl.Instance(config).AddModel(logs, cluMet, includeNoPlanActions, inPhases);
			PredictiveStudentModel model = StudentBehaviorPredictorControl.Instance(config).GetModel(domain.Key, cluMet);
			return model;
		}

		private static int CreateModel (string strDomainName, ClusterMethod cluMet, int iterations, int perValidation, bool logByLog, bool includeNoPlanActions, bool inPhases)
		{
			string ontologyPath = config.OntologyPath.Replace ('\\', Path.DirectorySeparatorChar);
			string logsPath = config.LogsPath.Replace ('\\', Path.DirectorySeparatorChar);
			string expertConfPath = config.DomainConfigurationPath.Replace ('\\', Path.DirectorySeparatorChar);
			int initialCol = config.InitialColumn;
			int intialRow = config.InitialRow;
			ExpertControl expert = ExpertControl.Instance(ontologyPath, logsPath, expertConfPath, initialCol, intialRow);
			DomainActions domain = expert.CreateDomain (strDomainName);
			WorldControl world = WorldControl.Instance (ontologyPath, logsPath);
			DomainLog logs = StudentControl.Instance (ontologyPath, logsPath, expertConfPath).GetDomainLogsFromOntology (domain, expert.OtherErrors, world.WorldErrors);
			List<Student> students = logs.GetStudents ();
			int numberStudents = students.Count;
			Dictionary<string, string> orStudentsVsNew = new Dictionary<string, string> ();
			int rtPhases = 0;

			for (int it = 0; it < iterations; it++) {
				List<int> studentsVal = new List<int> ();
				if (perValidation > 0){ //&& valPerYear==0) {
					decimal maxRandomNumber = Math.Ceiling ((decimal)(perValidation * numberStudents) / 100);// * mult) / 100);
					Random rnd = new Random ();
					for (int i = 0; i < maxRandomNumber; i++) {
						int random = rnd.Next (1, (int)numberStudents);// * mult);
						while (studentsVal.Contains (random)) {
							random = rnd.Next (1, (int)numberStudents);// * mult);
						}
						studentsVal.Add (random);
					}
				}

				if (inPhases)
				{
					//separar los logs de los alumnos por fases
					Dictionary<int, Dictionary<string, StudentLog>> phaseLogs = new Dictionary<int, Dictionary<string, StudentLog>>();
					Dictionary<int, Dictionary<string, StudentLog>> phaseLogsVal = new Dictionary<int, Dictionary<string, StudentLog>>();
					int phase = 0;
					foreach (Student std in students)
					{
						string sdtKey = std.Key;
						if (orStudentsVsNew.Keys.Contains(sdtKey))
						{
							sdtKey = orStudentsVsNew[sdtKey];
						}

						Dictionary<string, StudentLog> studentLogs = new Dictionary<string, StudentLog>();
						Dictionary<string, StudentLog> studentLogsVal = new Dictionary<string, StudentLog>();

						List<LogEntry> tempStuPhaseLogs = logs.GetStudentLog(sdtKey).Logs;
						if (studentsVal.Contains(int.Parse(std.Key)))
						{
							List<LogEntry> tempStuLogs = new List<LogEntry>();
							phase = 0;
							if (phaseLogsVal.ContainsKey(phase))
								studentLogsVal = phaseLogsVal[phase];
							bool phase0 = tempStuPhaseLogs.Any(x => x.GetType().BaseType == typeof(ActionLog) && x.Action.Key == "f0t1");
							bool phase1 = tempStuPhaseLogs.Any(x => x.GetType().BaseType == typeof(ActionLog) && x.Action.Key == "f1t46");
							bool phase2 = tempStuPhaseLogs.Any(x => x.GetType().BaseType == typeof(ActionLog) && x.Action.Key == "f2t61");
							bool phase3 = tempStuPhaseLogs.Any(x => x.GetType().BaseType == typeof(ActionLog) && x.Action.Key == "f3t61");

							foreach (LogEntry log in tempStuPhaseLogs)
							{
								if (log.GetType().BaseType == typeof(ActionLog) && log.Action.InitPhase && tempStuLogs.Count > 0)
								{
									if ((phase == 0 && phase0) ||
										(phase == 1 && phase1) ||
										(phase == 2 && phase2))
									{

										studentLogsVal.Add(std.Key, new StudentLog(std, tempStuLogs));
										if (phaseLogsVal.ContainsKey(phase))
											phaseLogsVal[phase] = studentLogsVal;
										else
											phaseLogsVal.Add(phase, studentLogsVal);
										tempStuLogs = new List<LogEntry>();
										phase++;
										if (phaseLogsVal.ContainsKey(phase))
											studentLogsVal = phaseLogsVal[phase];
										else
											studentLogsVal = new Dictionary<string, StudentLog>();
									}
									else
										break;
								}

								tempStuLogs.Add(log);
							}
							if (phase == 3 && phase3)
							{
								studentLogsVal.Add(std.Key, new StudentLog(std, tempStuLogs));
								if (phaseLogsVal.ContainsKey(phase))
									phaseLogsVal[phase] = studentLogsVal;
								else
									phaseLogsVal.Add(phase, studentLogsVal);
							}
						}
						else {
							List<LogEntry> tempStuLogs = new List<LogEntry>();
							phase = 0;
							if (phaseLogs.ContainsKey(phase))
								studentLogs = phaseLogs[phase];
							foreach (LogEntry log in tempStuPhaseLogs)
							{
								if (log.GetType().BaseType == typeof(ActionLog) && log.Action.InitPhase && tempStuLogs.Count > 0)
								{
									studentLogs.Add(std.Key, new StudentLog(std, tempStuLogs));
									if (phaseLogs.ContainsKey(phase))
										phaseLogs[phase] = studentLogs;
									else
										phaseLogs.Add(phase, studentLogs);
									tempStuLogs = new List<LogEntry>();
									phase++;
									if (phaseLogs.ContainsKey(phase))
										studentLogs = phaseLogs[phase];
									else
										studentLogs = new Dictionary<string, StudentLog>();
								}
								tempStuLogs.Add(log);
							}
							studentLogs.Add(std.Key, new StudentLog(std, tempStuLogs));
							if (phaseLogs.ContainsKey(phase))
								phaseLogs[phase] = studentLogs;
							else
								phaseLogs.Add(phase, studentLogs);
						}
					}

					//separar las acciones por fases
					Dictionary<int, List<ActionAplication>> phasesActions = new Dictionary<int, List<ActionAplication>>();
					List<ActionAplication> phaseActs = new List<ActionAplication>();
					phase = 0;
					foreach (ActionAplication act in domain.Actions)
					{
						if (act.InitPhase && phaseActs.Count > 0)
						{
							phasesActions.Add(phase, phaseActs);
							phaseActs = new List<ActionAplication>();
							phase++;
						}
						phaseActs.Add(act);
					}
					phasesActions.Add(phase, phaseActs);

					foreach (int ph in phasesActions.Keys)
					{
						phaseActs = phasesActions[ph];
						DomainActions newDomain = new DomainActions(ph+"_"+strDomainName + it, domain.Description, phaseActs, domain.EstimatedTime);
						DomainLog newLogs = new DomainLog(newDomain, phaseLogs[ph]);
						DomainActions newDomainVal = new DomainActions(ph+"_" +strDomainName + it + "Val", domain.Description, phaseActs, domain.EstimatedTime);
						DomainLog newLogsVal = new DomainLog(newDomainVal, phaseLogsVal[ph]);

						StudentBehaviorPredictorControl.Instance(config).AddModel(newLogs, cluMet, includeNoPlanActions, inPhases);
						if (perValidation > 0)
						{
							PredictiveStudentModel model = StudentBehaviorPredictorControl.Instance(config).GetModel(newDomain.Key, cluMet);
							if (!logByLog)
							{
								validationModels.Add(newDomainVal.Key, CreateModelVal(newDomainVal, newLogsVal, model, cluMet, logByLog, includeNoPlanActions, inPhases));
							}
							else {
								validationLogs.Add(newDomainVal.Key, CreateLogsVal(newDomainVal, newLogsVal, model, cluMet, logByLog, includeNoPlanActions, inPhases));
							}
						}
						rtPhases = ph;
					}
				}
				else {
					Dictionary<string, StudentLog> studentLogs = new Dictionary<string, StudentLog>();
					Dictionary<string, StudentLog> studentLogsVal = new Dictionary<string, StudentLog>();
					foreach (Student std in students)
					{
						string sdtKey = std.Key;
						if (orStudentsVsNew.Keys.Contains(sdtKey))
						{
							sdtKey = orStudentsVsNew[sdtKey];
						}
						StudentLog stdlogs = new StudentLog(std, logs.GetStudentLog(sdtKey).Logs);
						if (studentsVal.Contains(int.Parse(std.Key)))
						{
							studentLogsVal.Add(std.Key, stdlogs);
						}
						else {
							studentLogs.Add(std.Key, stdlogs);
						}
					}

					DomainActions newDomain = new DomainActions(strDomainName + it, domain.Description, domain.Actions, domain.EstimatedTime);
					DomainLog newLogs = new DomainLog(newDomain, studentLogs);
					DomainActions newDomainVal = new DomainActions(strDomainName + it + "Val", domain.Description, domain.Actions, domain.EstimatedTime);
					DomainLog newLogsVal = new DomainLog(newDomainVal, studentLogsVal);

					StudentBehaviorPredictorControl.Instance(config).AddModel(newLogs, cluMet, includeNoPlanActions, inPhases);
					if (perValidation > 0)
					{
						PredictiveStudentModel model = StudentBehaviorPredictorControl.Instance(config).GetModel(newDomain.Key, cluMet);
						if (!logByLog)
						{
							validationModels.Add(newDomainVal.Key, CreateModelVal(newDomainVal, newLogsVal, model, cluMet, logByLog, includeNoPlanActions, inPhases));
						}
						else {
							validationLogs.Add(newDomainVal.Key, CreateLogsVal(newDomainVal, newLogsVal, model, cluMet, logByLog, includeNoPlanActions, inPhases));
						}
					}
				}
			}

			return rtPhases;
		}

		private static void CreateModel (DomainActions domain, DomainLog logs, Student stuVal, ClusterMethod cluMet, int year, bool includeNoPlanActions, bool inPhases)
		{
			List<Student> students = logs.GetStudents ();
			Dictionary<string, StudentLog> studentLogs = new Dictionary<string, StudentLog> ();
			Dictionary<string, StudentLog> studentLogsVal = new Dictionary<string, StudentLog> ();

			foreach (Student std in students) {
				string sdtKey = std.Key;
				StudentLog stdlogs = new StudentLog (std, logs.GetStudentLog (sdtKey).Logs);
				if (std == stuVal) {
					studentLogsVal.Add (std.Key, stdlogs);
				} else if(year==0 || stdlogs.Logs[0].DateLog.Year==year){
					studentLogs.Add (std.Key, stdlogs);
				}
			}

			DomainActions newDomain = new DomainActions (domain.Key, domain.Description, domain.Actions, domain.EstimatedTime);
			DomainLog newLogs = new DomainLog (newDomain, studentLogs);
			DomainActions newDomainVal = new DomainActions (domain.Key + "Val", domain.Description, domain.Actions, domain.EstimatedTime);
			DomainLog newLogsVal = new DomainLog (newDomainVal, studentLogsVal);

			StudentBehaviorPredictorControl.Instance(config).AddModel (newLogs, cluMet, includeNoPlanActions, inPhases);
			PredictiveStudentModel model = StudentBehaviorPredictorControl.Instance(config).GetModel (newDomain.Key, cluMet);
			validationLogs.Add (newDomainVal.Key, CreateLogsVal (newDomainVal, newLogsVal, model, cluMet, false, includeNoPlanActions, false));
		}

		private static void createWorksheetValidation(double conf, double sup, Dictionary<int,PredictiveStudentModel> models, Dictionary<string, Dictionary<int, List<StudentLog>>> modelLogsVal, WorkbookPart workbookPart, bool includeNoPlanActions){
			SharedStringTablePart shareStringPart;
			if (workbookPart.GetPartsOfType<SharedStringTablePart> ().Count () > 0) {
				shareStringPart = workbookPart.GetPartsOfType<SharedStringTablePart> ().First ();
			} else {
				shareStringPart = workbookPart.AddNewPart<SharedStringTablePart> ();
			}

			WorksheetPart worksheetPart = InsertWorksheet (workbookPart, conf.ToString (new CultureInfo ("en-US")) + " " + sup.ToString (new CultureInfo ("en-US")));

			#region headersheet
			int indexSharedString = InsertSharedStringItem ("Cluster", shareStringPart);
			DocumentFormat.OpenXml.Spreadsheet.Cell cell = InsertCellInWorksheet ("A", 1, worksheetPart);
			cell.CellValue = new CellValue (indexSharedString.ToString ());
			cell.DataType = new EnumValue<CellValues> (CellValues.SharedString);

			indexSharedString = InsertSharedStringItem ("StudentsCluster", shareStringPart);
			cell = InsertCellInWorksheet ("B", 1, worksheetPart);
			cell.CellValue = new CellValue (indexSharedString.ToString ());
			cell.DataType = new EnumValue<CellValues> (CellValues.SharedString);

			indexSharedString = InsertSharedStringItem ("State", shareStringPart);
			cell = InsertCellInWorksheet ("C", 1, worksheetPart);
			cell.CellValue = new CellValue (indexSharedString.ToString ());
			cell.DataType = new EnumValue<CellValues> (CellValues.SharedString);

			indexSharedString = InsertSharedStringItem ("Area", shareStringPart);
			cell = InsertCellInWorksheet ("D", 1, worksheetPart);
			cell.CellValue = new CellValue (indexSharedString.ToString ());
			cell.DataType = new EnumValue<CellValues> (CellValues.SharedString);

			indexSharedString = InsertSharedStringItem ("StateFreq", shareStringPart);
			cell = InsertCellInWorksheet ("E", 1, worksheetPart);
			cell.CellValue = new CellValue (indexSharedString.ToString ());
			cell.DataType = new EnumValue<CellValues> (CellValues.SharedString);

			indexSharedString = InsertSharedStringItem ("StateValFreq", shareStringPart);
			cell = InsertCellInWorksheet ("F", 1, worksheetPart);
			cell.CellValue = new CellValue (indexSharedString.ToString ());
			cell.DataType = new EnumValue<CellValues> (CellValues.SharedString);

			indexSharedString = InsertSharedStringItem ("Event", shareStringPart);
			cell = InsertCellInWorksheet ("G", 1, worksheetPart);
			cell.CellValue = new CellValue (indexSharedString.ToString ());
			cell.DataType = new EnumValue<CellValues> (CellValues.SharedString);

			indexSharedString = InsertSharedStringItem ("EventVal", shareStringPart);
			cell = InsertCellInWorksheet ("H", 1, worksheetPart);
			cell.CellValue = new CellValue (indexSharedString.ToString ());
			cell.DataType = new EnumValue<CellValues> (CellValues.SharedString);

			indexSharedString = InsertSharedStringItem ("EventFreq", shareStringPart);
			cell = InsertCellInWorksheet ("I", 1, worksheetPart);
			cell.CellValue = new CellValue (indexSharedString.ToString ());
			cell.DataType = new EnumValue<CellValues> (CellValues.SharedString);

			indexSharedString = InsertSharedStringItem ("EventFreqVal", shareStringPart);
			cell = InsertCellInWorksheet ("J", 1, worksheetPart);
			cell.CellValue = new CellValue (indexSharedString.ToString ());
			cell.DataType = new EnumValue<CellValues> (CellValues.SharedString);

			indexSharedString = InsertSharedStringItem ("E", shareStringPart);
			cell = InsertCellInWorksheet ("K", 1, worksheetPart);
			cell.CellValue = new CellValue (indexSharedString.ToString ());
			cell.DataType = new EnumValue<CellValues> (CellValues.SharedString);

			indexSharedString = InsertSharedStringItem ("E'", shareStringPart);
			cell = InsertCellInWorksheet ("L", 1, worksheetPart);
			cell.CellValue = new CellValue (indexSharedString.ToString ());
			cell.DataType = new EnumValue<CellValues> (CellValues.SharedString);

			indexSharedString = InsertSharedStringItem ("COF", shareStringPart);
			cell = InsertCellInWorksheet ("M", 1, worksheetPart);
			cell.CellValue = new CellValue (indexSharedString.ToString ());
			cell.DataType = new EnumValue<CellValues> (CellValues.SharedString);

			indexSharedString = InsertSharedStringItem ("ECOF", shareStringPart);
			cell = InsertCellInWorksheet ("N", 1, worksheetPart);
			cell.CellValue = new CellValue (indexSharedString.ToString ());
			cell.DataType = new EnumValue<CellValues> (CellValues.SharedString);

			indexSharedString = InsertSharedStringItem ("EN", shareStringPart);
			cell = InsertCellInWorksheet ("O", 1, worksheetPart);
			cell.CellValue = new CellValue (indexSharedString.ToString ());
			cell.DataType = new EnumValue<CellValues> (CellValues.SharedString);

			indexSharedString = InsertSharedStringItem ("E'N", shareStringPart);
			cell = InsertCellInWorksheet ("P", 1, worksheetPart);
			cell.CellValue = new CellValue (indexSharedString.ToString ());
			cell.DataType = new EnumValue<CellValues> (CellValues.SharedString);
			#endregion

			int currentExcelRow = 1;
			DataTable dtData = new DataTable ("Data");
			dtData.Columns.Add (new DataColumn ("Cluster", typeof(int)));
			dtData.Columns.Add (new DataColumn ("StudentsCluster", typeof(int)));
			dtData.Columns.Add (new DataColumn ("State", typeof(string)));
			dtData.Columns.Add (new DataColumn ("Area", typeof(string)));
			dtData.Columns.Add (new DataColumn ("Event", typeof(string)));
			dtData.Columns.Add (new DataColumn ("EventVal", typeof(string)));
			dtData.Columns.Add (new DataColumn ("EventFreq", typeof(long)));
			dtData.Columns.Add (new DataColumn ("EvtFreqVal", typeof(long)));
			dtData.Columns.Add (new DataColumn ("P1", typeof(double)));
			dtData.Columns.Add (new DataColumn ("E", typeof(double)));
			dtData.Columns.Add (new DataColumn ("Ep", typeof(double)));
			dtData.Columns.Add (new DataColumn ("EvtsFromNode", typeof(int)));

			List<int[]> clusterRows = new List<int[]> ();
			Dictionary<int,List<SubtotalArea>> datosPorArea = new Dictionary<int, List<SubtotalArea>> ();
			datosPorArea.Add (-1, new List<SubtotalArea> ());
			datosPorArea [-1].Add (new SubtotalArea {Area = Area.CorrectFlow,
				Freq = 0, EN = 0, EpN = 0
			});
			datosPorArea [-1].Add (new SubtotalArea {Area = Area.IrrelevantErrors,
				Freq = 0, EN = 0, EpN = 0
			});
			datosPorArea [-1].Add (new SubtotalArea {Area = Area.RelevantErrors,
				Freq = 0, EN = 0, EpN = 0
			});

			foreach (PredictiveStudentModel model in models.Values)
			{
				Dictionary<int, List<StudentLog>> cluLogsVal = modelLogsVal[model.Key+"Val"];
				foreach (StudentsCluster clu in model.Clusters.Values)
				{
					StudentActionsModel automata = clu.StudentActionsModel;

					int numStudents = clu.NumberOfStudents;
					long numEvents = clu.NumberOfEvents;
					List<StudentLog> stuLogsVal = cluLogsVal[clu.Number];

					foreach (StudentLog stuLod in stuLogsVal)
					{
						Node<State, Event> previousState = automata.InitState;
						List<Node<State, Event>> pastNodes = new List<Node<State, Event>>();
						pastNodes.Add(automata.InitState);
						bool hasPreviousNode = true;
						int countVector = 0;
						foreach (LogEntry log in stuLod.Logs)
						{
							if (log.GetType() != typeof(NoPlanAllowedActionLog) || includeNoPlanActions)
							{
								Node<State, Event> node = null;
								string stateKey = "";
								string stateArea = "";
								double supNodeOut = 0;
								int evtsFromNode = previousState.OutArcs.Count;

								if (hasPreviousNode)
								{
									node = automata.FindState(log, previousState, pastNodes);

									if (node != null)
									{
										hasPreviousNode = true;
										stateArea = node.Specification.Area.ToString();
										stateKey = node.Key;
										//cálculo del soporte del nodo de salida del arco
										if (stateArea == Area.IrrelevantErrors.ToString())
											//para estados en la zona de errores irrelevantes se calcula en base al número de eventos totales en el cluster
											supNodeOut = (double)node.Specification.EventFrequency / numEvents;
										else
											supNodeOut = (double)node.Specification.EventFrequency / numStudents;
										bool evtAdded = false;
										foreach (Arc<State, Event> evt in previousState.OutArcs.Values)
										{
											if (node == evt.NodeIn)
											{
												evtAdded = true;
												string evtKey = evt.Key.ToString();
												string evtKeyVal = "";
												double p1 = 0;
												double E = 0;
												double Ep = 0;

												Type evtType = evt.Specification.GetType();
												double supEvt = 0;
												double confEvt = 0;
												long evtFreqVal = 0;
												long evtFreq = 0;
												DataRow dr = null;
												DataRow[] drsTemp = dtData.Select("Cluster=" + clu.Number + " AND State='" + stateKey +
																   "' AND Area='" + stateArea + "' AND Event='" + evtKey + "'");
												if (drsTemp.Length > 0)
												{
													dr = drsTemp[0];
													evtFreq = (long)dr.ItemArray[6];
													evtFreqVal = (long)dr.ItemArray[7];
													E = (double)dr.ItemArray[9];
													Ep = (double)dr.ItemArray[10];
												}
												else
													dr = dtData.NewRow();

												if (evtType == typeof(NormalEvent))
												{
													//asignación de frecuencia del modelo y cálculo de la probabilidad p1

													//strEvtFreq = ((NormalEvent)evt.Specification).Frequency.ToString (new CultureInfo ("en-US"));
													evtFreq = ((NormalEvent)evt.Specification).Frequency;
													p1 = 1 - (double)evtFreq / previousState.Specification.EventFrequency;
													//cálculo del soporte del evento del modelo
													supEvt = (double)evtFreq / numStudents;
													//cálculo de la confianza de la regla X=>Y (evento) del modelo
													confEvt = supNodeOut == 0 ? 0 : supEvt / supNodeOut;

													if (confEvt >= conf && supNodeOut >= sup)
													{
														//cálculo del error
														E += p1;///evtsFromNode;
														Ep += (double)1;///evtsFromNode;
														evtKeyVal = evtKey;

														if (drsTemp.Length == 0)
														{
															dr[0] = clu.Number;
															dr[1] = clu.NumberOfStudents;
															dr[2] = stateKey;
															dr[3] = stateArea;
															dr[4] = evtKey;
															dr[5] = evtKeyVal;
															dr[6] = evtFreq;
															dr[7] = evtFreqVal + 1;
															dr[8] = p1;
															dr[9] = E;
															dr[10] = Ep;
															dr[11] = evtsFromNode;
															dtData.Rows.Add(dr);
														}
														else {
															dr[5] = evtKeyVal;
															dr[6] = evtFreq;
															dr[7] = evtFreqVal + 1;
															dr[8] = p1;
															dr[9] = E;
															dr[10] = Ep;
															dr[11] = evtsFromNode;
														}
													}

												}
												else if (evtType == typeof(VectorEvent))
												{
													//se asigna a un contador el número de elementos de la lista de frecuencias mayor para iterar por este
													int count = ((VectorEvent)evt.Specification).Frequency.Count;
													double Etmp = 0;

													bool lastVector = false;
													lastVector = previousState != node;
													if (lastVector)
													{
														if (countVector < count)
														{
															evtFreq = ((VectorEvent)evt.Specification).Frequency.Sum();
															//cálculo del soporte del evento del modelo en base al número de eventos totales en el cluster
															supEvt = (double)evtFreq / numEvents;
															if (countVector == 0)
															{
																Etmp = (1 - (double)((VectorEvent)evt.Specification).Frequency[countVector] / previousState.Specification.EventFrequency) / evtsFromNode;
															}
															else {
																Etmp = (1 - (double)((VectorEvent)evt.Specification).Frequency.Skip(1).Sum() / previousState.Specification.EventFrequency) / evtsFromNode;
															}
														}
														else
															Etmp = (double)1;///evtsFromNode;

														//asignación de frecuencia del modelo
														//strEvtFreq = String.Join ("|", ((VectorEvent)evt.Specification).Frequency.ToArray ());
														//cálculo de la confianza de la regla X=>Y (evento) del modelo
														confEvt = supNodeOut == 0 ? 0 : supEvt / supNodeOut;

														if (confEvt >= conf && supNodeOut >= sup)
														{
															E += Etmp;
															Ep += (double)1;///evtsFromNode;
															evtKeyVal = evtKey;

															if (drsTemp.Length == 0)
															{
																dr[0] = clu.Number;
																dr[1] = clu.NumberOfStudents;
																dr[2] = stateKey;
																dr[3] = stateArea;
																dr[4] = evtKey;
																dr[5] = evtKeyVal;
																dr[6] = evtFreq;
																dr[7] = evtFreqVal + 1;
																dr[8] = p1;
																dr[9] = E;
																dr[10] = Ep;
																dr[11] = evtsFromNode;
																dtData.Rows.Add(dr);
															}
															else {
																dr[5] = evtKeyVal;
																dr[6] = evtFreq;
																dr[7] = evtFreqVal + 1;
																dr[8] = p1;
																dr[9] = E;
																dr[10] = Ep;
																dr[11] = evtsFromNode;
															}
														}
														countVector = 0;
													}
													else
														countVector++;
												}

											}
										}
										if (!evtAdded && previousState != node)
										{
											evtsFromNode = evtsFromNode == 0 ? 1 : evtsFromNode;
											if (supNodeOut >= sup)
											{
												DataRow[] drsTemp = dtData.Select("Cluster=" + clu.Number + " AND State='" + node.Key +
																   "' AND Area='" + node.Specification.Area + "' AND Event='" + previousState.Key + " " + node.Key + "'");
												DataRow dr = null;
												if (drsTemp.Length == 0)
												{
													dr = dtData.NewRow();
													dr[0] = clu.Number;
													dr[1] = clu.NumberOfStudents;
													dr[2] = stateKey;
													dr[3] = stateArea;
													dr[4] = "";
													dr[5] = previousState.Key + " " + node.Key;
													dr[6] = 0;
													dr[7] = 1;
													dr[8] = 0;
													dr[9] = (double)1;///evtsFromNode;
													dr[10] = (double)1;///evtsFromNode;
													dr[11] = evtsFromNode;
													dtData.Rows.Add(dr);
												}
												else {
													dr[5] = previousState.Key + " " + node.Key;
													dr[6] = 0;
													dr[7] = (long)dr[7] + 1;
													dr[8] = 0;
													dr[9] = (double)dr[9] + 1;///evtsFromNode;	
													dr[10] = (double)dr[10] + 1;///evtsFromNode;
													dr[11] = evtsFromNode;
												}
											}
										}
										if (previousState == node)
											countVector++;
										else
											countVector = 0;
									}
									else {
										Area tmpArea = GetArea(log, previousState, pastNodes, clu.StudentActionsModel.InitState);
										node = CreateState(log, tmpArea);
										hasPreviousNode = false;
										DataRow[] drsTemp = dtData.Select("Cluster=" + clu.Number + " AND State='" + node.Key +
														   "' AND Area='" + node.Specification.Area + "' AND Event='" + previousState.Key + " " + node.Key + "'");
										DataRow dr = null;
										if (drsTemp.Length == 0)
										{
											dr = dtData.NewRow();
											dr[0] = clu.Number;
											dr[1] = clu.NumberOfStudents;
											dr[2] = node.Key;
											dr[3] = node.Specification.Area;
											dr[4] = "";
											dr[5] = previousState.Key + " " + node.Key;
											dr[6] = 0;
											dr[7] = 1;
											dr[8] = 0;
											dr[9] = (double)1;///(evtsFromNode+1);
											dr[10] = (double)1;///(evtsFromNode+1);
											dr[11] = evtsFromNode;
											dtData.Rows.Add(dr);
										}
										else {
											dr = drsTemp[0];
											dr[5] = previousState.Key + " " + node.Key;
											dr[6] = 0;
											dr[7] = (long)dr[7] + 1;
											dr[8] = 0;
											dr[9] = (double)dr[9] + 1;///(evtsFromNode+1);	
											dr[10] = (double)dr[10] + 1;///(evtsFromNode+1);
											dr[11] = evtsFromNode;
										}
									}
								}
								else {
									node = automata.FindState(log, previousState, pastNodes);

									if (node != null)
									{
										hasPreviousNode = true;
										evtsFromNode = 1;
									}
									else {
										Area tmpArea = GetArea(log, previousState, pastNodes, clu.StudentActionsModel.InitState);
										node = CreateState(log, tmpArea);
										hasPreviousNode = false;
										evtsFromNode += 1;
									}
									DataRow[] drsTemp = dtData.Select("Cluster=" + clu.Number + " AND State='" + node.Key +
													   "' AND Area='" + node.Specification.Area + "' AND Event='" + previousState.Key + " " + node.Key + "'");
									DataRow dr = null;
									if (drsTemp.Length == 0)
									{
										dr = dtData.NewRow();
										dr[0] = clu.Number;
										dr[1] = clu.NumberOfStudents;
										dr[2] = node.Key;
										dr[3] = node.Specification.Area;
										dr[4] = "";
										dr[5] = previousState.Key + " " + node.Key;
										dr[6] = 0;
										dr[7] = 1;
										dr[8] = 0;
										dr[9] = (double)1;///evtsFromNode;
										dr[10] = (double)1;///evtsFromNode;
										dr[11] = evtsFromNode;
										dtData.Rows.Add(dr);
									}
									else {
										dr = drsTemp[0];
										dr[5] = previousState.Key + " " + node.Key;
										dr[6] = 0;
										dr[7] = (long)dr[7] + 1;
										dr[8] = 0;
										dr[9] = (double)dr[9] + 1;///evtsFromNode);
										dr[10] = (double)dr[10] + 1;///evtsFromNode;
										dr[11] = evtsFromNode;
									}
								}

								pastNodes.Add(node);
								previousState = node;
							}
						}

					}
				}
			}

			/*foreach (StudentsCluster clu in model.Clusters.Values) {
				datosPorArea.Add (clu.Number, new List<SubtotalArea> ());
				datosPorArea [clu.Number].Add (new SubtotalArea {Area = Area.CorrectFlow,
					Freq = 0, EN = 0, EpN = 0
				});
				datosPorArea [clu.Number].Add (new SubtotalArea {Area = Area.IrrelevantErrors,
					Freq = 0, EN = 0, EpN = 0
				});
				datosPorArea [clu.Number].Add (new SubtotalArea {Area = Area.RelevantErrors,
					Freq = 0, EN = 0, EpN = 0
				});
				StudentActionsModel automata = clu.StudentActionsModel;

				int numStudents = clu.NumberOfStudents;
				long numEvents = clu.NumberOfEvents;
				List<StudentLog> stuLogsVal = cluLogsVal [clu.Number];

				foreach (StudentLog stuLod in stuLogsVal) {
					Node<State, Event> previousState = automata.InitState;
					List<Node<State, Event>> pastNodes = new List<Node<State, Event>> ();
					pastNodes.Add (automata.InitState);
					bool hasPreviousNode = true;
					int countVector = 0;
					foreach (LogEntry log in stuLod.Logs) {
					if (log.GetType () != typeof(NoPlanAllowedActionLog) || includeNoPlanActions) {
							Node<State,Event> node = null;
							string stateKey = "";
							string stateArea = "";
							double supNodeOut = 0;
							int evtsFromNode = previousState.OutArcs.Count;

							if (hasPreviousNode) {
								node = automata.FindState (log, previousState, pastNodes);

								if (node != null) {
									hasPreviousNode = true;
									stateArea = node.Specification.Area.ToString ();
									stateKey = node.Key;
									//cálculo del soporte del nodo de salida del arco
									if (stateArea == Area.IrrelevantErrors.ToString ())
									//para estados en la zona de errores irrelevantes se calcula en base al número de eventos totales en el cluster
									supNodeOut = (double)node.Specification.EventFrequency / numEvents;
									else
										supNodeOut = (double)node.Specification.EventFrequency / numStudents;
									bool evtAdded = false;
									foreach (Arc<State,Event> evt in previousState.OutArcs.Values) {
										if (node == evt.NodeIn) {
											evtAdded = true;
											string evtKey = evt.Key.ToString ();
											string evtKeyVal = "";
											double p1 = 0;
											double E = 0;
											double Ep = 0;

											Type evtType = evt.Specification.GetType ();
											double supEvt = 0;
											double confEvt = 0;
											long evtFreqVal = 0;
											long evtFreq = 0;
											DataRow dr = null;
											DataRow[] drsTemp = dtData.Select ("Cluster=" + clu.Number + " AND State='" + stateKey +
											                   "' AND Area='" + stateArea + "' AND Event='" + evtKey + "'");
											if (drsTemp.Length > 0) {
												dr = drsTemp [0];
												evtFreq = (long)dr.ItemArray [6];
												evtFreqVal = (long)dr.ItemArray [7];
												E = (double)dr.ItemArray [9];
												Ep = (double)dr.ItemArray [10];
											} else
												dr = dtData.NewRow ();

											if (evtType == typeof(NormalEvent)) {
												//asignación de frecuencia del modelo y cálculo de la probabilidad p1

												//strEvtFreq = ((NormalEvent)evt.Specification).Frequency.ToString (new CultureInfo ("en-US"));
												evtFreq = ((NormalEvent)evt.Specification).Frequency;
												p1 = 1 - (double)evtFreq / previousState.Specification.EventFrequency;
												//cálculo del soporte del evento del modelo
												supEvt = (double)evtFreq / numStudents;
												//cálculo de la confianza de la regla X=>Y (evento) del modelo
												confEvt = supNodeOut == 0 ? 0 : supEvt / supNodeOut;

												if (confEvt >= conf && supNodeOut >= sup) {
													//cálculo del error
													E += p1;///evtsFromNode;
													Ep += (double)1;///evtsFromNode;
													evtKeyVal = evtKey;

													if (drsTemp.Length == 0) {
														dr [0] = clu.Number;
														dr [1] = clu.NumberOfStudents;
														dr [2] = stateKey;
														dr [3] = stateArea;
														dr [4] = evtKey;
														dr [5] = evtKeyVal;
														dr [6] = evtFreq;
														dr [7] = evtFreqVal + 1;
														dr [8] = p1;
														dr [9] = E;
														dr [10] = Ep;
														dr [11] = evtsFromNode;
														dtData.Rows.Add (dr);
													} else {
														dr [5] = evtKeyVal;
														dr [6] = evtFreq;
														dr [7] = evtFreqVal + 1;
														dr [8] = p1;
														dr [9] = E;
														dr [10] = Ep;
														dr [11] = evtsFromNode;
													}
												}

											} else if (evtType == typeof(VectorEvent)) {
												//se asigna a un contador el número de elementos de la lista de frecuencias mayor para iterar por este
												int count = ((VectorEvent)evt.Specification).Frequency.Count;
												double Etmp = 0;

												bool lastVector = false;
												lastVector = previousState != node;
												if (lastVector) {
													if (countVector < count) {
														evtFreq = ((VectorEvent)evt.Specification).Frequency.Sum ();
														//cálculo del soporte del evento del modelo en base al número de eventos totales en el cluster
														supEvt = (double)evtFreq / numEvents;
														if (countVector == 0) {
															Etmp = (1 - (double)((VectorEvent)evt.Specification).Frequency [countVector] / previousState.Specification.EventFrequency) / evtsFromNode;
														} else {
															Etmp = (1 - (double)((VectorEvent)evt.Specification).Frequency.Skip (1).Sum () / previousState.Specification.EventFrequency) / evtsFromNode;
														}
													} else
														Etmp = (double)1;///evtsFromNode;

													//asignación de frecuencia del modelo
													//strEvtFreq = String.Join ("|", ((VectorEvent)evt.Specification).Frequency.ToArray ());
													//cálculo de la confianza de la regla X=>Y (evento) del modelo
													confEvt = supNodeOut == 0 ? 0 : supEvt / supNodeOut;

													if (confEvt >= conf && supNodeOut >= sup) {
														E += Etmp;
														Ep += (double)1;///evtsFromNode;
														evtKeyVal = evtKey;

														if (drsTemp.Length == 0) {
															dr [0] = clu.Number;
															dr [1] = clu.NumberOfStudents;
															dr [2] = stateKey;
															dr [3] = stateArea;
															dr [4] = evtKey;
															dr [5] = evtKeyVal;
															dr [6] = evtFreq;
															dr [7] = evtFreqVal + 1;
															dr [8] = p1;
															dr [9] = E;
															dr [10] = Ep;
															dr [11] = evtsFromNode;
															dtData.Rows.Add (dr);
														} else {
															dr [5] = evtKeyVal;
															dr [6] = evtFreq;
															dr [7] = evtFreqVal + 1;
															dr [8] = p1;
															dr [9] = E;
															dr [10] = Ep;
															dr [11] = evtsFromNode;
														}
													}
													countVector = 0;
												} else
													countVector++;
											}

										}
									}
									if (!evtAdded && previousState != node) {
										evtsFromNode = evtsFromNode == 0 ? 1 : evtsFromNode;
										if (supNodeOut >= sup) {
											DataRow[] drsTemp = dtData.Select ("Cluster=" + clu.Number + " AND State='" + node.Key +
											                   "' AND Area='" + node.Specification.Area + "' AND Event='" + previousState.Key + " " + node.Key + "'");
											DataRow dr = null;
											if (drsTemp.Length == 0) {
												dr = dtData.NewRow ();
												dr [0] = clu.Number;
												dr [1] = clu.NumberOfStudents;
												dr [2] = stateKey;
												dr [3] = stateArea;
												dr [4] = "";
												dr [5] = previousState.Key + " " + node.Key;
												dr [6] = 0;
												dr [7] = 1;
												dr [8] = 0;
												dr [9] = (double)1;///evtsFromNode;
												dr [10] = (double)1;///evtsFromNode;
												dr [11] = evtsFromNode;
												dtData.Rows.Add (dr);
											} else {
												dr [5] = previousState.Key + " " + node.Key;
												dr [6] = 0;
												dr [7] = (long)dr [7] + 1;
												dr [8] = 0;
												dr [9] = (double)dr [9] + 1;///evtsFromNode;	
												dr [10] = (double)dr [10] + 1;///evtsFromNode;
												dr [11] = evtsFromNode;
											}
										}
									}
									if (previousState == node)
										countVector++;
									else
										countVector = 0;
								} else {
									Area tmpArea = GetArea (log, previousState, pastNodes, clu.StudentActionsModel.InitState);
									node = CreateState (log, tmpArea);
									hasPreviousNode = false;
									DataRow[] drsTemp = dtData.Select ("Cluster=" + clu.Number + " AND State='" + node.Key +
									                   "' AND Area='" + node.Specification.Area + "' AND Event='" + previousState.Key + " " + node.Key + "'");
									DataRow dr = null;
									if (drsTemp.Length == 0) {
										dr = dtData.NewRow ();
										dr [0] = clu.Number;
										dr [1] = clu.NumberOfStudents;
										dr [2] = node.Key;
										dr [3] = node.Specification.Area;
										dr [4] = "";
										dr [5] = previousState.Key + " " + node.Key;
										dr [6] = 0;
										dr [7] = 1;
										dr [8] = 0;	
										dr [9] = (double)1;///(evtsFromNode+1);
										dr [10] = (double)1;///(evtsFromNode+1);
										dr [11] = evtsFromNode;
										dtData.Rows.Add (dr);
									} else {
										dr = drsTemp [0];
										dr [5] = previousState.Key + " " + node.Key;
										dr [6] = 0;
										dr [7] = (long)dr [7] + 1;
										dr [8] = 0;
										dr [9] = (double)dr [9] + 1;///(evtsFromNode+1);	
										dr [10] = (double)dr [10] + 1;///(evtsFromNode+1);
										dr [11] = evtsFromNode;
									}
								}
							} else {
								node = automata.FindState (log, previousState, pastNodes);

								if (node != null) {
									hasPreviousNode = true;
									evtsFromNode = 1;
								} else {
									Area tmpArea = GetArea (log, previousState, pastNodes, clu.StudentActionsModel.InitState);
									node = CreateState (log, tmpArea);
									hasPreviousNode = false;
									evtsFromNode += 1;
								}
								DataRow[] drsTemp = dtData.Select ("Cluster=" + clu.Number + " AND State='" + node.Key +
								                   "' AND Area='" + node.Specification.Area + "' AND Event='" + previousState.Key + " " + node.Key + "'");
								DataRow dr = null;
								if (drsTemp.Length == 0) {
									dr = dtData.NewRow ();
									dr [0] = clu.Number;
									dr [1] = clu.NumberOfStudents;
									dr [2] = node.Key;
									dr [3] = node.Specification.Area;
									dr [4] = "";
									dr [5] = previousState.Key + " " + node.Key;
									dr [6] = 0;
									dr [7] = 1;
									dr [8] = 0;	
									dr [9] = (double)1;///evtsFromNode;
									dr [10] = (double)1;///evtsFromNode;
									dr [11] = evtsFromNode;
									dtData.Rows.Add (dr);
								} else {
									dr = drsTemp [0];
									dr [5] = previousState.Key + " " + node.Key;
									dr [6] = 0;
									dr [7] = (long)dr [7] + 1;
									dr [8] = 0;	
									dr [9] = (double)dr [9] + 1;///evtsFromNode);
									dr [10] = (double)dr [10] + 1;///evtsFromNode;
									dr [11] = evtsFromNode;
								}
							}

							pastNodes.Add (node);
							previousState = node;
						}
					}

				}
			}*/
			foreach (PredictiveStudentModel model in models.Values)
			{
				foreach (StudentsCluster clu in model.Clusters.Values)
				{
					DataView dvTemp = new DataView(dtData);
					DataTable dtStates = dvTemp.ToTable(true, "State", "Area");

					foreach (DataRow drState in dtStates.Rows)
					{
						string stateKey = (string)drState[0];
						string stateArea = (string)drState[1];
						DataRow[] drsTemp = dtData.Select("Cluster=" + clu.Number + " AND State='" + stateKey +
											"' AND Area='" + stateArea + "'");
						long stateFreqVal = 0;
						bool printNode = false;
						if (drsTemp.Length > 0)
						{
							long stateFreq = drsTemp.Sum(row => (long)row.ItemArray[6]);
							double supNodeOut = 0;
							if (stateArea == Area.IrrelevantErrors.ToString())
								//para estados en la zona de errores irrelevantes se calcula en base al número de eventos totales en el cluster
								supNodeOut = (double)stateFreq / clu.NumberOfEvents;
							else
								supNodeOut = (double)stateFreq / clu.NumberOfStudents;
							int evtsFromNode = 0;
							foreach (DataRow drEvent in drsTemp)
							{
								int studentsCluster = clu.Number;
								string evtKey = (string)drEvent[4];
								string evtKeyVal = (string)drEvent[5];
								long evtFreq = (long)drEvent[6];
								long eventFreqVal = (long)drEvent[7];
								double E = (double)drEvent[9];
								double Ep = (double)drEvent[10];
								evtsFromNode = (int)drEvent[11];

								double supEvt = (double)evtFreq / clu.NumberOfStudents;
								double confEvt = supNodeOut == 0 ? 0 : supEvt / supNodeOut;

								if (confEvt >= conf && supNodeOut >= sup)
								{
									printNode = true;
									stateFreqVal += eventFreqVal;
									currentExcelRow++;
									cell = InsertCellInWorksheet("A", Convert.ToUInt32(currentExcelRow), worksheetPart);
									cell.CellValue = new CellValue(clu.Number.ToString());
									cell.DataType = new EnumValue<CellValues>(CellValues.Number);
									indexSharedString = InsertSharedStringItem(stateKey, shareStringPart);
									cell = InsertCellInWorksheet("C", Convert.ToUInt32(currentExcelRow), worksheetPart);
									cell.CellValue = new CellValue(indexSharedString.ToString());
									cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
									indexSharedString = InsertSharedStringItem(evtKey, shareStringPart);
									cell = InsertCellInWorksheet("G", Convert.ToUInt32(currentExcelRow), worksheetPart);
									cell.CellValue = new CellValue(indexSharedString.ToString());
									cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
									indexSharedString = InsertSharedStringItem(evtKeyVal, shareStringPart);
									cell = InsertCellInWorksheet("H", Convert.ToUInt32(currentExcelRow), worksheetPart);
									cell.CellValue = new CellValue(indexSharedString.ToString());
									cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
									cell = InsertCellInWorksheet("I", Convert.ToUInt32(currentExcelRow), worksheetPart);
									cell.CellValue = new CellValue(evtFreq.ToString());
									cell.DataType = new EnumValue<CellValues>(CellValues.Number);
									cell = InsertCellInWorksheet("J", Convert.ToUInt32(currentExcelRow), worksheetPart);
									cell.CellValue = new CellValue(eventFreqVal.ToString());
									cell.DataType = new EnumValue<CellValues>(CellValues.Number);
									cell = InsertCellInWorksheet("K", Convert.ToUInt32(currentExcelRow), worksheetPart);
									cell.CellValue = new CellValue(E.ToString(new CultureInfo("en-US")));
									cell.DataType = new EnumValue<CellValues>(CellValues.Number);
									cell = InsertCellInWorksheet("L", Convert.ToUInt32(currentExcelRow), worksheetPart);
									cell.CellValue = new CellValue("1");//Ep.ToString(new CultureInfo("en-US")));
									cell.DataType = new EnumValue<CellValues>(CellValues.Number);
									double cof = (double)1 / eventFreqVal;
									cell = InsertCellInWorksheet("M", Convert.ToUInt32(currentExcelRow), worksheetPart);
									cell.CellValue = new CellValue(cof.ToString(new CultureInfo("en-US")));
									cell.DataType = new EnumValue<CellValues>(CellValues.Number);
									cell = InsertCellInWorksheet("N", Convert.ToUInt32(currentExcelRow), worksheetPart);
									cell.CellValue = new CellValue((E * cof).ToString(new CultureInfo("en-US")));
									cell.DataType = new EnumValue<CellValues>(CellValues.Number);
									cell = InsertCellInWorksheet("O", Convert.ToUInt32(currentExcelRow), worksheetPart);
									cell.CellFormula = new CellFormula("N" + currentExcelRow + "*E" + currentExcelRow);
									cell.DataType = new EnumValue<CellValues>(CellValues.Number);
									cell = InsertCellInWorksheet("P", Convert.ToUInt32(currentExcelRow), worksheetPart);
									cell.CellFormula = new CellFormula("L" + currentExcelRow + "*E" + currentExcelRow);
									cell.DataType = new EnumValue<CellValues>(CellValues.Number);
								}
							}

							if (printNode)
							{
								double E = drsTemp.Sum(row => (double)row.ItemArray[9]);
								double Ep = drsTemp.Sum(row => (double)row.ItemArray[10]);
								SubtotalArea tempSubTot = datosPorArea[-1].Single(x => x.Area.ToString() == stateArea);
								tempSubTot.Freq += (int)stateFreq;
								double cof = (double)1 / stateFreqVal;
								tempSubTot.EN += E * cof * stateFreq;
								tempSubTot.EpN += (int)stateFreq;
								/*SubtotalArea tempSub = datosPorArea[clu.Number].Single(x => x.Area.ToString() == stateArea);
								tempSub.Freq += (int)stateFreq;
								tempSub.EN += E * cof * stateFreq;
								tempSub.EpN += Ep;*/

								currentExcelRow++;
								cell = InsertCellInWorksheet("A", Convert.ToUInt32(currentExcelRow), worksheetPart);
								cell.CellValue = new CellValue(clu.Number.ToString());
								cell.DataType = new EnumValue<CellValues>(CellValues.Number);
								cell = InsertCellInWorksheet("B", Convert.ToUInt32(currentExcelRow), worksheetPart);
								cell.CellValue = new CellValue(clu.NumberOfStudents.ToString());
								cell.DataType = new EnumValue<CellValues>(CellValues.Number);
								cell = new DocumentFormat.OpenXml.Spreadsheet.Cell()
								{
									CellReference = "C" + currentExcelRow,
									DataType = CellValues.String,
									CellValue = new CellValue(stateKey)
								};
								indexSharedString = InsertSharedStringItem(stateKey, shareStringPart);
								cell = InsertCellInWorksheet("C", Convert.ToUInt32(currentExcelRow), worksheetPart);
								cell.CellValue = new CellValue(indexSharedString.ToString());
								cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
								cell = new DocumentFormat.OpenXml.Spreadsheet.Cell()
								{
									CellReference = "D" + currentExcelRow,
									DataType = CellValues.String,
									CellValue = new CellValue(stateArea)
								};
								indexSharedString = InsertSharedStringItem(stateArea, shareStringPart);
								cell = InsertCellInWorksheet("D", Convert.ToUInt32(currentExcelRow), worksheetPart);
								cell.CellValue = new CellValue(indexSharedString.ToString());
								cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
								cell = InsertCellInWorksheet("E", Convert.ToUInt32(currentExcelRow), worksheetPart);
								cell.CellValue = new CellValue(stateFreq.ToString(new CultureInfo("en-US")));
								cell.DataType = new EnumValue<CellValues>(CellValues.Number);
								cell = InsertCellInWorksheet("F", Convert.ToUInt32(currentExcelRow), worksheetPart);
								cell.CellValue = new CellValue(stateFreqVal.ToString(new CultureInfo("en-US")));
								cell.DataType = new EnumValue<CellValues>(CellValues.Number);
								cell = InsertCellInWorksheet("K", Convert.ToUInt32(currentExcelRow), worksheetPart);
								cell.CellValue = new CellValue(E.ToString(new CultureInfo("en-US")));
								cell.DataType = new EnumValue<CellValues>(CellValues.Number);
								cell = InsertCellInWorksheet("L", Convert.ToUInt32(currentExcelRow), worksheetPart);
								cell.CellValue = new CellValue("1");
								cell.DataType = new EnumValue<CellValues>(CellValues.Number);
								cell = InsertCellInWorksheet("M", Convert.ToUInt32(currentExcelRow), worksheetPart);
								cell.CellValue = new CellValue(cof.ToString(new CultureInfo("en-US")));
								cell.DataType = new EnumValue<CellValues>(CellValues.Number);
								cell = InsertCellInWorksheet("N", Convert.ToUInt32(currentExcelRow), worksheetPart);
								cell.CellValue = new CellValue((E * cof).ToString(new CultureInfo("en-US")));
								cell.DataType = new EnumValue<CellValues>(CellValues.Number);
								cell = InsertCellInWorksheet("O", Convert.ToUInt32(currentExcelRow), worksheetPart);
								cell.CellFormula = new CellFormula("N" + currentExcelRow + "*E" + currentExcelRow);
								cell.DataType = new EnumValue<CellValues>(CellValues.Number);
								cell = InsertCellInWorksheet("P", Convert.ToUInt32(currentExcelRow), worksheetPart);
								cell.CellFormula = new CellFormula("L" + currentExcelRow + "*E" + currentExcelRow);
								cell.DataType = new EnumValue<CellValues>(CellValues.Number);

								if (!clusterRows.Any(c => c[0] == clu.Number))
									clusterRows.Add(new int[] { clu.Number, currentExcelRow });
								else {
									clusterRows.Single(c => c[0] == clu.Number)[1] = currentExcelRow;
								}
							}
						}
					}
				}
			}

			#region subtotales
			currentExcelRow++;
			cell = InsertCellInWorksheet ("A", Convert.ToUInt32 (currentExcelRow), worksheetPart);
			cell.CellValue = new CellValue ("");
			cell.DataType = new EnumValue<CellValues> (CellValues.String);

			currentExcelRow++;
			cell = InsertCellInWorksheet ("A", Convert.ToUInt32 (currentExcelRow), worksheetPart);
			cell.CellValue = new CellValue ("TOTAL");
			cell.DataType = new EnumValue<CellValues> (CellValues.String);
			/*int clusterCol = 1;
			foreach (int[] cr in clusterRows) {
				cell = InsertCellInWorksheet (Convert.ToChar (65 + 3 * clusterCol).ToString (), Convert.ToUInt32 (currentExcelRow), worksheetPart);
				cell.CellValue = new CellValue ("CLUSTER " + cr [0].ToString ());
				cell.DataType = new EnumValue<CellValues> (CellValues.String);
				cell = InsertCellInWorksheet (Convert.ToChar (66 + 3 * clusterCol).ToString (), Convert.ToUInt32 (currentExcelRow), worksheetPart);
				
				if (model.Clusters [cr [0]].GetType () == typeof(StudentsClusterByErrorTime)) {
					StudentsClusterByErrorTime stTemp = (StudentsClusterByErrorTime)model.Clusters [cr [0]];
					cell.CellValue = new CellValue ("E: " + Math.Round (stTemp.AvgErrors, 2).ToString (new CultureInfo ("en-US")) +
					" T: " + Math.Round (stTemp.AvgTime, 2).ToString (new CultureInfo ("en-US")));
				} else if (model.Clusters [cr [0]].GetType () == typeof(StudentsClusterByErrors)) {
					StudentsClusterByErrors stTemp = (StudentsClusterByErrors)model.Clusters [cr [0]];
					cell.CellValue = new CellValue ("E: " + Math.Round (stTemp.AvgErrors, 2).ToString (new CultureInfo ("en-US")));
				} else if (model.Clusters [cr [0]].GetType () == typeof(StudentsClusterByEventsZone)) {
					StudentsClusterByEventsZone stTemp = (StudentsClusterByEventsZone)model.Clusters [cr [0]];
					cell.CellValue = new CellValue ("CZ: " + Math.Round (stTemp.AvgCorrectEvents, 2).ToString (new CultureInfo ("en-US")) +
					" IZ: " + Math.Round (stTemp.AvgIrrelevantErrorEvents, 2).ToString (new CultureInfo ("en-US")) +
					" RZ: " + Math.Round (stTemp.AvgRelevantErrorEvents, 2).ToString (new CultureInfo ("en-US")));
				}
				cell.DataType = new EnumValue<CellValues> (CellValues.String);
				clusterCol++;
			}
*/
			currentExcelRow++;
			cell = InsertCellInWorksheet ("A", Convert.ToUInt32 (currentExcelRow), worksheetPart);
			cell.CellValue = new CellValue ("Suma FREQ");
			cell.DataType = new EnumValue<CellValues> (CellValues.String);

			int lastCluRow = 0;
			if (clusterRows.Count > 0)
				lastCluRow = clusterRows [clusterRows.Count - 1] [1];
			cell = InsertCellInWorksheet ("B", Convert.ToUInt32 (currentExcelRow), worksheetPart);
			cell.CellFormula = new CellFormula ("SUM(E2:E" + lastCluRow + ")");
			cell.CellValue = new CellValue (datosPorArea [-1].Sum (x => x.Freq).ToString (new CultureInfo ("en-US")));
			cell.DataType = new EnumValue<CellValues> (CellValues.Number);

			/*int currentLastCluRow = 0;
			lastCluRow = 1;
			clusterCol = 1;
			foreach (int[] cr in clusterRows) {
				currentLastCluRow = cr [1];
				cell = InsertCellInWorksheet (Convert.ToChar (65 + 3 * clusterCol).ToString (), Convert.ToUInt32 (currentExcelRow), worksheetPart);
				cell.CellValue = new CellValue ("Suma FREQ");
				cell.DataType = new EnumValue<CellValues> (CellValues.String);
				cell = InsertCellInWorksheet (Convert.ToChar (66 + 3 * clusterCol).ToString (), Convert.ToUInt32 (currentExcelRow), worksheetPart);
				cell.CellFormula = new CellFormula ("SUM(E" + (lastCluRow + 1).ToString () + ":E" + currentLastCluRow + ")");
				cell.CellValue = new CellValue (datosPorArea [cr [0]].Sum (x => x.Freq).ToString (new CultureInfo ("en-US")));
				cell.DataType = new EnumValue<CellValues> (CellValues.Number);
				lastCluRow = currentLastCluRow;
				clusterCol++;
			}*/
			int rowFreq = currentExcelRow;

			currentExcelRow++;
			cell = InsertCellInWorksheet ("A", Convert.ToUInt32 (currentExcelRow), worksheetPart);
			cell.CellValue = new CellValue ("Suma EN");
			cell.DataType = new EnumValue<CellValues> (CellValues.String);

			lastCluRow = 0;
			if (clusterRows.Count > 0)
				lastCluRow = clusterRows [clusterRows.Count - 1] [1];
			cell = InsertCellInWorksheet ("B", Convert.ToUInt32 (currentExcelRow), worksheetPart);
			cell.CellFormula = new CellFormula ("SUM(O2:O" + lastCluRow + ")");
			cell.CellValue = new CellValue (datosPorArea [-1].Sum (x => x.EN).ToString (new CultureInfo ("en-US")));
			cell.DataType = new EnumValue<CellValues> (CellValues.Number);

			/*currentLastCluRow = 0;
			lastCluRow = 1;
			clusterCol = 1;
			foreach (int[] cr in clusterRows) {
				currentLastCluRow = cr [1];
				cell = InsertCellInWorksheet (Convert.ToChar (65 + 3 * clusterCol).ToString (), Convert.ToUInt32 (currentExcelRow), worksheetPart);
				cell.CellValue = new CellValue ("Suma EN");
				cell.DataType = new EnumValue<CellValues> (CellValues.String);
				cell = InsertCellInWorksheet (Convert.ToChar (66 + 3 * clusterCol).ToString (), Convert.ToUInt32 (currentExcelRow), worksheetPart);
				cell.CellFormula = new CellFormula ("SUM(O" + (lastCluRow + 1).ToString () + ":O" + currentLastCluRow + ")");
				cell.CellValue = new CellValue (datosPorArea [cr [0]].Sum (x => x.EN).ToString (new CultureInfo ("en-US")));
				cell.DataType = new EnumValue<CellValues> (CellValues.Number);
				lastCluRow = currentLastCluRow;
				clusterCol++;
			}*/
			int rowE = currentExcelRow;

			currentExcelRow++;
			cell = InsertCellInWorksheet ("A", Convert.ToUInt32 (currentExcelRow), worksheetPart);
			cell.CellValue = new CellValue ("Suma E'N");
			cell.DataType = new EnumValue<CellValues> (CellValues.String);

			lastCluRow = 0;
			if (clusterRows.Count > 0)
				lastCluRow = clusterRows [clusterRows.Count - 1] [1];
			cell = InsertCellInWorksheet ("B", Convert.ToUInt32 (currentExcelRow), worksheetPart);
			cell.CellFormula = new CellFormula ("SUM(P2:P" + lastCluRow + ")");
			cell.CellValue = new CellValue (datosPorArea [-1].Sum (x => x.EpN).ToString (new CultureInfo ("en-US")));
			cell.DataType = new EnumValue<CellValues> (CellValues.Number);

			/*currentLastCluRow = 0;
			lastCluRow = 1;
			clusterCol = 1;
			foreach (int[] cr in clusterRows) {
				currentLastCluRow = cr [1];
				cell = InsertCellInWorksheet (Convert.ToChar (65 + 3 * clusterCol).ToString (), Convert.ToUInt32 (currentExcelRow), worksheetPart);
				cell.CellValue = new CellValue ("Suma E'N");
				cell.DataType = new EnumValue<CellValues> (CellValues.String);
				cell = InsertCellInWorksheet (Convert.ToChar (66 + 3 * clusterCol).ToString (), Convert.ToUInt32 (currentExcelRow), worksheetPart);
				cell.CellFormula = new CellFormula ("SUM(P" + (lastCluRow + 1).ToString () + ":P" + currentLastCluRow + ")");
				cell.CellValue = new CellValue (datosPorArea [cr [0]].Sum (x => x.EpN).ToString (new CultureInfo ("en-US")));
				cell.DataType = new EnumValue<CellValues> (CellValues.Number);
				lastCluRow = currentLastCluRow;
				clusterCol++;
			}*/
			int rowEp = currentExcelRow;

			currentExcelRow++;
			cell = InsertCellInWorksheet ("A", Convert.ToUInt32 (currentExcelRow), worksheetPart);
			cell.CellValue = new CellValue ("Media E");
			cell.DataType = new EnumValue<CellValues> (CellValues.String);
			cell = InsertCellInWorksheet ("B", Convert.ToUInt32 (currentExcelRow), worksheetPart);
			cell.CellFormula = new CellFormula ("B" + rowE + "/B" + rowFreq);
			if (datosPorArea [-1].Sum (x => x.Freq) > 0)
				cell.CellValue = new CellValue ((datosPorArea [-1].Sum (x => x.EN) / datosPorArea [-1].Sum (x => x.Freq)).ToString (new CultureInfo ("en-US")));
			else
				cell.CellValue = new CellValue ("0");
			cell.DataType = new EnumValue<CellValues> (CellValues.Number);

			/*clusterCol = 1;
			foreach (int[] cr in clusterRows) {
				cell = InsertCellInWorksheet (Convert.ToChar (65 + 3 * clusterCol).ToString (), Convert.ToUInt32 (currentExcelRow), worksheetPart);
				cell.CellValue = new CellValue ("Media E");
				cell.DataType = new EnumValue<CellValues> (CellValues.String);
				cell = InsertCellInWorksheet (Convert.ToChar (66 + 3 * clusterCol).ToString (), Convert.ToUInt32 (currentExcelRow), worksheetPart);
				cell.CellFormula = new CellFormula (Convert.ToChar (66 + 3 * clusterCol) + rowE.ToString () + "/" + Convert.ToChar (66 + 3 * clusterCol) + rowFreq.ToString ());
				if (datosPorArea [cr [0]].Sum (x => x.Freq) > 0)
					cell.CellValue = new CellValue ((datosPorArea [cr [0]].Sum (x => x.EN) / datosPorArea [cr [0]].Sum (x => x.Freq)).ToString (new CultureInfo ("en-US")));
				else
					cell.CellValue = new CellValue ("0");
				cell.DataType = new EnumValue<CellValues> (CellValues.Number);
				clusterCol++;
			}*/
			int rowMedE = currentExcelRow;

			currentExcelRow++;
			cell = InsertCellInWorksheet ("A", Convert.ToUInt32 (currentExcelRow), worksheetPart);
			cell.CellValue = new CellValue ("Media E'");
			cell.DataType = new EnumValue<CellValues> (CellValues.String);
			cell = InsertCellInWorksheet ("B", Convert.ToUInt32 (currentExcelRow), worksheetPart);
			cell.CellFormula = new CellFormula ("B" + rowEp + "/B" + rowFreq);
			if (datosPorArea [-1].Sum (x => x.Freq) > 0)
				cell.CellValue = new CellValue ((datosPorArea [-1].Sum (x => x.EpN) / datosPorArea [-1].Sum (x => x.Freq)).ToString (new CultureInfo ("en-US")));
			else
				cell.CellValue = new CellValue ("0");
			cell.DataType = new EnumValue<CellValues> (CellValues.Number);

			/*clusterCol = 1;
			foreach (int[] cr in clusterRows) {
				cell = InsertCellInWorksheet (Convert.ToChar (65 + 3 * clusterCol).ToString (), Convert.ToUInt32 (currentExcelRow), worksheetPart);
				cell.CellValue = new CellValue ("Media E'");
				cell.DataType = new EnumValue<CellValues> (CellValues.String);
				cell = InsertCellInWorksheet (Convert.ToChar (66 + 3 * clusterCol).ToString (), Convert.ToUInt32 (currentExcelRow), worksheetPart);
				cell.CellFormula = new CellFormula (Convert.ToChar (66 + 3 * clusterCol) + rowEp.ToString () + "/" + Convert.ToChar (66 + 3 * clusterCol) + rowFreq.ToString ());
				if (datosPorArea [cr [0]].Sum (x => x.Freq) > 0)
					cell.CellValue = new CellValue ((datosPorArea [cr [0]].Sum (x => x.EpN) / datosPorArea [cr [0]].Sum (x => x.Freq)).ToString (new CultureInfo ("en-US")));
				else
					cell.CellValue = new CellValue ("0");
				cell.DataType = new EnumValue<CellValues> (CellValues.Number);
				clusterCol++;
			}*/
			int rowMedEp = currentExcelRow;

			currentExcelRow++;
			cell = InsertCellInWorksheet ("A", Convert.ToUInt32 (currentExcelRow), worksheetPart);
			cell.CellValue = new CellValue ("DIF");
			cell.DataType = new EnumValue<CellValues> (CellValues.String);
			cell = InsertCellInWorksheet ("B", Convert.ToUInt32 (currentExcelRow), worksheetPart);
			cell.CellFormula = new CellFormula ("B" + rowMedEp + "-B" + rowMedE);
			if (datosPorArea [-1].Sum (x => x.Freq) > 0)
				cell.CellValue = new CellValue (((datosPorArea [-1].Sum (x => x.EpN) - datosPorArea [-1].Sum (x => x.EN)) / datosPorArea [-1].Sum (x => x.Freq)).ToString (new CultureInfo ("en-US")));
			else
				cell.CellValue = new CellValue ("0");
			cell.DataType = new EnumValue<CellValues> (CellValues.Number);

			/*clusterCol = 1;
			foreach (int[] cr in clusterRows) {
				cell = InsertCellInWorksheet (Convert.ToChar (65 + 3 * clusterCol).ToString (), Convert.ToUInt32 (currentExcelRow), worksheetPart);
				cell.CellValue = new CellValue ("DIF");
				cell.DataType = new EnumValue<CellValues> (CellValues.String);
				cell = InsertCellInWorksheet (Convert.ToChar (66 + 3 * clusterCol).ToString (), Convert.ToUInt32 (currentExcelRow), worksheetPart);
				cell.CellFormula = new CellFormula (Convert.ToChar (66 + 3 * clusterCol) + rowMedEp.ToString () + "-" + Convert.ToChar (66 + 3 * clusterCol) + rowMedE.ToString ());
				if (datosPorArea [cr [0]].Sum (x => x.Freq) > 0)
					cell.CellValue = new CellValue (((datosPorArea [cr [0]].Sum (x => x.EpN) - datosPorArea [cr [0]].Sum (x => x.EN)) / datosPorArea [cr [0]].Sum (x => x.Freq)).ToString (new CultureInfo ("en-US")));
				else
					cell.CellValue = new CellValue ("0");
				cell.DataType = new EnumValue<CellValues> (CellValues.Number);
				clusterCol++;
			}*/
			#endregion

			#region totalesporarea
			for (int j = 0; j < 3; j++) {
				currentExcelRow++;
				cell = InsertCellInWorksheet ("A", Convert.ToUInt32 (currentExcelRow), worksheetPart);
				cell.CellValue = new CellValue ("");
				cell.DataType = new EnumValue<CellValues> (CellValues.String);

				currentExcelRow++;
				cell = InsertCellInWorksheet ("A", Convert.ToUInt32 (currentExcelRow), worksheetPart);
				cell.CellValue = new CellValue (datosPorArea [-1] [j].Area.ToString ());
				cell.DataType = new EnumValue<CellValues> (CellValues.String);

				/*clusterCol = 1;
				foreach (int[] cr in clusterRows) {
					cell = InsertCellInWorksheet (Convert.ToChar (65 + 3 * clusterCol).ToString (), Convert.ToUInt32 (currentExcelRow), worksheetPart);
					cell.CellValue = new CellValue (datosPorArea [cr [0]] [j].Area.ToString () + " " + cr [0].ToString ());
					cell.DataType = new EnumValue<CellValues> (CellValues.String);
					clusterCol++;
				}
*/
				currentExcelRow++;
				cell = InsertCellInWorksheet ("A", Convert.ToUInt32 (currentExcelRow), worksheetPart);
				cell.CellValue = new CellValue ("Suma FREQ");
				cell.DataType = new EnumValue<CellValues> (CellValues.String);
				cell = InsertCellInWorksheet ("B", Convert.ToUInt32 (currentExcelRow), worksheetPart);
				cell.CellValue = new CellValue (datosPorArea [-1] [j].Freq.ToString (new CultureInfo ("en-US")));
				cell.DataType = new EnumValue<CellValues> (CellValues.Number);

				/*clusterCol = 1;
				foreach (int[] cr in clusterRows) {
					cell = InsertCellInWorksheet (Convert.ToChar (65 + 3 * clusterCol).ToString (), Convert.ToUInt32 (currentExcelRow), worksheetPart);
					cell.CellValue = new CellValue ("Suma FREQ");
					cell.DataType = new EnumValue<CellValues> (CellValues.String);
					cell = InsertCellInWorksheet (Convert.ToChar (66 + 3 * clusterCol).ToString (), Convert.ToUInt32 (currentExcelRow), worksheetPart);
					cell.CellValue = new CellValue (datosPorArea [cr [0]] [j].Freq.ToString (new CultureInfo ("en-US")));
					cell.DataType = new EnumValue<CellValues> (CellValues.Number);
					clusterCol++;
				}*/
				rowFreq = currentExcelRow;

				currentExcelRow++;
				cell = InsertCellInWorksheet ("A", Convert.ToUInt32 (currentExcelRow), worksheetPart);
				cell.CellValue = new CellValue ("Suma EN");
				cell.DataType = new EnumValue<CellValues> (CellValues.String);
				cell = InsertCellInWorksheet ("B", Convert.ToUInt32 (currentExcelRow), worksheetPart);
				cell.CellValue = new CellValue (datosPorArea [-1] [j].EN.ToString (new CultureInfo ("en-US")));
				cell.DataType = new EnumValue<CellValues> (CellValues.Number);

				/*clusterCol = 1;
				foreach (int[] cr in clusterRows) {
					cell = InsertCellInWorksheet (Convert.ToChar (65 + 3 * clusterCol).ToString (), Convert.ToUInt32 (currentExcelRow), worksheetPart);
					cell.CellValue = new CellValue ("Suma EN");
					cell.DataType = new EnumValue<CellValues> (CellValues.String);
					cell = InsertCellInWorksheet (Convert.ToChar (66 + 3 * clusterCol).ToString (), Convert.ToUInt32 (currentExcelRow), worksheetPart);
					cell.CellValue = new CellValue (datosPorArea [cr [0]] [j].EN.ToString (new CultureInfo ("en-US")));
					cell.DataType = new EnumValue<CellValues> (CellValues.Number);
					clusterCol++;
				}*/
				rowE = currentExcelRow;

				currentExcelRow++;
				cell = InsertCellInWorksheet ("A", Convert.ToUInt32 (currentExcelRow), worksheetPart);
				cell.CellValue = new CellValue ("Suma E'N");
				cell.DataType = new EnumValue<CellValues> (CellValues.String);
				cell = InsertCellInWorksheet ("B", Convert.ToUInt32 (currentExcelRow), worksheetPart);
				cell.CellValue = new CellValue (datosPorArea [-1] [j].EpN.ToString (new CultureInfo ("en-US")));
				cell.DataType = new EnumValue<CellValues> (CellValues.Number);

				/*clusterCol = 1;
				foreach (int[] cr in clusterRows) {
					cell = InsertCellInWorksheet (Convert.ToChar (65 + 3 * clusterCol).ToString (), Convert.ToUInt32 (currentExcelRow), worksheetPart);
					cell.CellValue = new CellValue ("Suma E'N");
					cell.DataType = new EnumValue<CellValues> (CellValues.String);
					cell = InsertCellInWorksheet (Convert.ToChar (66 + 3 * clusterCol).ToString (), Convert.ToUInt32 (currentExcelRow), worksheetPart);
					cell.CellValue = new CellValue (datosPorArea [cr [0]] [j].EpN.ToString (new CultureInfo ("en-US")));
					cell.DataType = new EnumValue<CellValues> (CellValues.Number);
					clusterCol++;
				}*/
				rowEp = currentExcelRow;

				currentExcelRow++;
				cell = InsertCellInWorksheet ("A", Convert.ToUInt32 (currentExcelRow), worksheetPart);
				cell.CellValue = new CellValue ("Media E");
				cell.DataType = new EnumValue<CellValues> (CellValues.String);
				cell = InsertCellInWorksheet ("B", Convert.ToUInt32 (currentExcelRow), worksheetPart);
				cell.CellFormula = new CellFormula ("B" + rowE + "/B" + rowFreq);
				if (datosPorArea [-1] [j].Freq > 0)
					cell.CellValue = new CellValue ((datosPorArea [-1] [j].EN / datosPorArea [-1] [j].Freq).ToString (new CultureInfo ("en-US")));
				else
					cell.CellValue = new CellValue ("0");
				cell.DataType = new EnumValue<CellValues> (CellValues.Number);

				/*clusterCol = 1;
				foreach (int[] cr in clusterRows) {
					cell = InsertCellInWorksheet (Convert.ToChar (65 + 3 * clusterCol).ToString (), Convert.ToUInt32 (currentExcelRow), worksheetPart);
					cell.CellValue = new CellValue ("Media E");
					cell.DataType = new EnumValue<CellValues> (CellValues.String);
					cell = InsertCellInWorksheet (Convert.ToChar (66 + 3 * clusterCol).ToString (), Convert.ToUInt32 (currentExcelRow), worksheetPart);
					cell.CellFormula = new CellFormula (Convert.ToChar (66 + 3 * clusterCol) + rowE.ToString () + "/" + Convert.ToChar (66 + 3 * clusterCol) + rowFreq.ToString ());
					if (datosPorArea [cr [0]] [j].Freq > 0)
						cell.CellValue = new CellValue ((datosPorArea [cr [0]] [j].EN / datosPorArea [cr [0]] [j].Freq).ToString (new CultureInfo ("en-US")));
					else
						cell.CellValue = new CellValue ("0");
					cell.DataType = new EnumValue<CellValues> (CellValues.Number);
					clusterCol++;
				}*/
				rowMedE = currentExcelRow;

				currentExcelRow++;
				cell = InsertCellInWorksheet ("A", Convert.ToUInt32 (currentExcelRow), worksheetPart);
				cell.CellValue = new CellValue ("Media E'");
				cell.DataType = new EnumValue<CellValues> (CellValues.String);
				cell = InsertCellInWorksheet ("B", Convert.ToUInt32 (currentExcelRow), worksheetPart);
				cell.CellFormula = new CellFormula ("B" + rowEp + "/B" + rowFreq);
				if (datosPorArea [-1] [j].Freq > 0)
					cell.CellValue = new CellValue ((datosPorArea [-1] [j].EpN / datosPorArea [-1] [j].Freq).ToString (new CultureInfo ("en-US")));
				else
					cell.CellValue = new CellValue ("0");
				cell.DataType = new EnumValue<CellValues> (CellValues.Number);

				/*clusterCol = 1;
				foreach (int[] cr in clusterRows) {
					cell = InsertCellInWorksheet (Convert.ToChar (65 + 3 * clusterCol).ToString (), Convert.ToUInt32 (currentExcelRow), worksheetPart);
					cell.CellValue = new CellValue ("Media E'");
					cell.DataType = new EnumValue<CellValues> (CellValues.String);
					cell = InsertCellInWorksheet (Convert.ToChar (66 + 3 * clusterCol).ToString (), Convert.ToUInt32 (currentExcelRow), worksheetPart);
					cell.CellFormula = new CellFormula (Convert.ToChar (66 + 3 * clusterCol) + rowEp.ToString () + "/" + Convert.ToChar (66 + 3 * clusterCol) + rowFreq.ToString ());
					if (datosPorArea [cr [0]] [j].Freq > 0)
						cell.CellValue = new CellValue ((datosPorArea [cr [0]] [j].EpN / datosPorArea [cr [0]] [j].Freq).ToString (new CultureInfo ("en-US")));
					else
						cell.CellValue = new CellValue ("0");
					cell.DataType = new EnumValue<CellValues> (CellValues.Number);
					clusterCol++;
				}*/
				rowMedEp = currentExcelRow;

				currentExcelRow++;
				cell = InsertCellInWorksheet ("A", Convert.ToUInt32 (currentExcelRow), worksheetPart);
				cell.CellValue = new CellValue ("DIF");
				cell.DataType = new EnumValue<CellValues> (CellValues.String);
				cell = InsertCellInWorksheet ("B", Convert.ToUInt32 (currentExcelRow), worksheetPart);
				cell.CellFormula = new CellFormula ("B" + rowMedEp + "-B" + rowMedE);
				if (datosPorArea [-1] [j].Freq > 0)
					cell.CellValue = new CellValue (((datosPorArea [-1] [j].EpN - datosPorArea [-1] [j].EN) / datosPorArea [-1] [j].Freq).ToString (new CultureInfo ("en-US")));
				else
					cell.CellValue = new CellValue ("0");
				cell.DataType = new EnumValue<CellValues> (CellValues.Number);

				/*clusterCol = 1;
				foreach (int[] cr in clusterRows) {
					cell = InsertCellInWorksheet (Convert.ToChar (65 + 3 * clusterCol).ToString (), Convert.ToUInt32 (currentExcelRow), worksheetPart);
					cell.CellValue = new CellValue ("DIF");
					cell.DataType = new EnumValue<CellValues> (CellValues.String);
					cell = InsertCellInWorksheet (Convert.ToChar (66 + 3 * clusterCol).ToString (), Convert.ToUInt32 (currentExcelRow), worksheetPart);
					cell.CellFormula = new CellFormula (Convert.ToChar (66 + 3 * clusterCol) + rowMedEp.ToString () + "-" + Convert.ToChar (66 + 3 * clusterCol) + rowMedE.ToString ());
					if (datosPorArea [cr [0]] [j].Freq > 0)
						cell.CellValue = new CellValue (((datosPorArea [cr [0]] [j].EpN - datosPorArea [cr [0]] [j].EN) / datosPorArea [cr [0]] [j].Freq).ToString (new CultureInfo ("en-US")));
					else
						cell.CellValue = new CellValue ("0");
					cell.DataType = new EnumValue<CellValues> (CellValues.Number);
					clusterCol++;
				}*/
			}
			#endregion

			worksheetPart.Worksheet.Save ();
		}

private static Dictionary<int,PredictiveStudentModel> CreateModelVal (DomainActions domainVal, DomainLog validationLogs, PredictiveStudentModel model, ClusterMethod cluMet, bool logByLog, bool includeNoPlanActions, bool inPhases)
		{
			Dictionary<int,List<StudentLog>> logsbyCluster = new Dictionary<int, List<StudentLog>> ();
			foreach (int cluNum in model.Clusters.Keys) {
				logsbyCluster.Add (cluNum, new List<StudentLog> ());
			}
				
			//separar logs por cada cluster identificando el estudiante en que log está en model
			switch (cluMet) {
			case ClusterMethod.ErrorsAndTime:
			case ClusterMethod.Errors:
			case ClusterMethod.EventsByZone:
				foreach (Student student in validationLogs.GetStudents()) {
					StudentLog log = validationLogs.GetStudentLog (student.Key);
					int cluNum = model.MatchClusterForNewStudent (log, cluMet, includeNoPlanActions, inPhases, domainVal.Key);
					logsbyCluster [cluNum].Add (log);
				}
				break;
			case ClusterMethod.Sequences:
				AdomdConnection con = new AdomdConnection (config.ASSConString);
				con.Open ();
				AdomdDataAdapter da = new AdomdDataAdapter ("SELECT Cluster(),t.[id] From [EntireLab] PREDICTION JOIN  SHAPE { OPENQUERY([Lab Biotecnologia], 'SELECT [id] FROM [dbo].[StudentsVal] ORDER BY [id]')} APPEND ({OPENQUERY([Lab Biotecnologia], 'SELECT [actionkey], [sequence], [studentid] FROM [dbo].[LogsVal] ORDER BY [studentid]')} RELATE [id] TO [studentid]) AS [LogsVal] AS t ON [EntireLab].[Logs].[Actionkey] = t.[LogsVal].[actionkey] AND [EntireLab].[Logs].[Sequence] = t.[LogsVal].[sequence]", con);
				DataTable dtlogstemp = new DataTable ();
				da.Fill (dtlogstemp);
				con.Close ();
				foreach (DataRow dr in dtlogstemp.Rows) {
					StudentLog log = validationLogs.GetStudentLog (dr [1].ToString ());
					int cluNum = int.Parse (dr [0].ToString ().Split (' ') [1]);
					logsbyCluster [cluNum].Add (log);
				}
				break;
			case ClusterMethod.NoClusters:
				foreach (Student student in validationLogs.GetStudents()) {
					StudentLog log = validationLogs.GetStudentLog (student.Key);
					logsbyCluster [0].Add (log);
				}
				break;
			}

			Dictionary<int,PredictiveStudentModel> modelsValByCluster = new Dictionary<int, PredictiveStudentModel> ();
			if (!logByLog) {
				//por cada logsByCluster crear autómata sin clusterizar y agregar a modelsValByCluster
				foreach (int cluNum in logsbyCluster.Keys) {
					modelsValByCluster [cluNum] = StudentBehaviorPredictorControl.Instance(config).GetAutomataForValidation (logsbyCluster [cluNum], domainVal, cluMet,false);
				}
			}

			return modelsValByCluster;
		}

        private static Dictionary<int, List<StudentLog>> CreateLogsVal(DomainActions domainVal, DomainLog valLogs, PredictiveStudentModel model, ClusterMethod cluMet, bool logByLog, bool includeNoPlanActions, bool inPhases)
        {
            Dictionary<int, List<StudentLog>> logsbyCluster = new Dictionary<int, List<StudentLog>>();
            foreach (int cluNum in model.Clusters.Keys)
            {
                logsbyCluster.Add(cluNum, new List<StudentLog>());
            }

            //separar logs por cada cluster identificando el estudiante en que log está en model
            switch (cluMet)
            {
                case ClusterMethod.ErrorsAndTime:
                case ClusterMethod.Errors:
                case ClusterMethod.EventsByZone:
                    List<Student> students = valLogs.GetStudents();
                    foreach (Student student in students)
                    {
                        StudentLog log = valLogs.GetStudentLog(student.Key);
                        int cluNum = model.MatchClusterForNewStudent(log, cluMet, includeNoPlanActions, inPhases, domainVal.Key);
                        logsbyCluster[cluNum].Add(log);
                    }
                    break;
                case ClusterMethod.Sequences:
                    if (!logByLog)
                    {
                        AdomdConnection con = new AdomdConnection(config.ASSConString);
                        con.Open();
                        AdomdDataAdapter da = new AdomdDataAdapter();
                        DataTable dtlogstemp = new DataTable();
                        if (!inPhases)
                        {
                            da = new AdomdDataAdapter("SELECT Cluster(),t.[id] From [EntireLab] PREDICTION JOIN  SHAPE { OPENQUERY([Lab Biotecnologia], 'SELECT [id] FROM [dbo].[StudentsVal] ORDER BY [id]')} APPEND ({OPENQUERY([Lab Biotecnologia], 'SELECT [actionkey], [sequence], [studentid] FROM [dbo].[LogsVal] ORDER BY [studentid]')} RELATE [id] TO [studentid]) AS [LogsVal] AS t ON [EntireLab].[Logs].[Actionkey] = t.[LogsVal].[actionkey] AND [EntireLab].[Logs].[Sequence] = t.[LogsVal].[sequence]", con);
                        }
                        else
                        {
                            string miningStruct = "Phase" + domainVal.Key.Split('_')[0];
                            string table1 = "LogsVal";
                            string table2 = "LogsValPhase" + domainVal.Key.Split('_')[0];
                            string table3 = "Logs Phase" + domainVal.Key.Split('_')[0];
                            da = new AdomdDataAdapter("SELECT Cluster(),t.[id] From [" + miningStruct + "] PREDICTION JOIN  SHAPE { OPENQUERY([Lab Biotecnologia], 'SELECT [id] FROM [dbo].[StudentsVal] ORDER BY [id]')} APPEND ({OPENQUERY([Lab Biotecnologia], 'SELECT [actionkey], [sequence], [studentid] FROM [dbo].["+ table2 + "] ORDER BY [studentid]')} RELATE [id] TO [studentid]) AS ["+ table2 + "] AS t ON [" + miningStruct + "].["+ table3 + "].[Actionkey] = t.["+table2+"].[actionkey] AND [" + miningStruct + "].["+ table3 + "].[Sequence] = t.["+ table2 + "].[sequence]", con);
                        }
                        da.Fill(dtlogstemp);
                        con.Close();
                        foreach (DataRow dr in dtlogstemp.Rows)
                        {
                            StudentLog log = valLogs.GetStudentLog(dr[1].ToString());
                            int cluNum = int.Parse(dr[0].ToString().Split(' ')[1]);
                            logsbyCluster[cluNum].Add(log);
                        }
                    }
                    else
                    {
                        SqlConnection con = new SqlConnection(config.BDDConString);
                        con.Open();
                        string sql = "delete from LogsVal; delete from LogsValPhase0; delete from LogsValPhase1; delete from LogsValPhase2; delete from LogsValPhase3; delete from StudentsVal ";
                        SqlCommand cmd = new SqlCommand(sql, con);
                        cmd.ExecuteNonQuery();

                        foreach (Student student in valLogs.GetStudents())
                        {
                            StudentLog stuLog = valLogs.GetStudentLog(student.Key);
                            sql = "insert into StudentsVal values ('" + student.Key + "') ";

                            int seq = 1;
                            for (int i = 0; i < stuLog.Logs.Count; i++)
                            {
                                LogEntry log = stuLog.Logs[i];
                                if (log.GetType().BaseType != typeof(NoPlanAllowedActionLog) || includeNoPlanActions)
                                {
                                    string action = "";
                                    if (log.GetType().BaseType == typeof(ActionLog))
                                    {
                                        action = log.Action.Key;
                                    }
                                    else if (log.GetType() == typeof(DepErrorLog))
                                    {
                                        Dependence fail = ((DepErrorLog)log).FailedDependence;
                                        action = log.Action.Key + "_" + fail.Key;
                                    }
                                    else if (log.GetType() == typeof(IncompErrorLog))
                                    {
                                        Incompatibility fail = ((IncompErrorLog)log).FailedIncompatibility;
                                        action = log.Action.Key + "_" + fail.Key;
                                    }
                                    else if (log.GetType().BaseType == typeof(TimeErrorLog))
                                    {
                                        Error fail = (log.GetType() == typeof(MinTimeErrorLog)) ? log.Action.MinTimeError : log.Action.MaxTimeError;
                                        action = log.Action.Key + "_" + fail.Key;
                                    }
                                    else if (log.GetType() == typeof(WorldErrorLog))
                                    {
                                        Error fail = ((WorldErrorLog)log).ErrorAssociated;
                                        action = log.Action.Key + "_" + fail.Key;
                                    }
                                    else if (log.GetType() == typeof(OtherErrorLog))
                                    {
                                        Error fail = ((OtherErrorLog)log).ErrorAssociated;
                                        action = log.Action.Key + "_" + fail.Key;
                                    }

                                    string table = "LogsVal";
                                    if (inPhases)
                                    {
                                        table = "LogsValPhase" + domainVal.Key.Split('_')[0];
                                    }

                                    sql = sql + "insert into "+ table + " (studentid, domain, actionkey, sequence, date) values('"
                            + student.Key + "','"
                            + domainVal.Key + "','"
                            + action + "',"
                            + seq + ",'"
                            + log.DateLog.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                                    seq++;
                                }
                            }
                            cmd = new SqlCommand(sql, con);
                            cmd.ExecuteNonQuery();

                            int cluNum = model.MatchClusterForNewStudent(stuLog, cluMet, includeNoPlanActions, inPhases, domainVal.Key);
                            logsbyCluster[cluNum].Add(stuLog);
                        }
                        con.Close();
                    }
                    break;
                case ClusterMethod.NoClusters:
                    foreach (Student student in valLogs.GetStudents())
                    {
                        StudentLog log = valLogs.GetStudentLog(student.Key);
                        logsbyCluster[0].Add(log);
                    }
                    break;
            }

            return logsbyCluster;
        }

		private static void CreateWorkbooksFreq(string strDomainName, ClusterMethod cluMet)
		{
			int phases = CreateModel(strDomainName, cluMet, 1, 0, true, false, false);

			for (int it = 0; it < 1; it++)
			{
				//modelo predictivo
				Dictionary<int, PredictiveStudentModel> models = new Dictionary<int, PredictiveStudentModel>();
				models.Add(0, StudentBehaviorPredictorControl.Instance(config).GetModel(strDomainName + it, cluMet));



				string filepath = cluMet + "_freq.xlsx";
				if (File.Exists(filepath))
					File.Delete(filepath);

				File.Copy("default.xlsx", filepath, true);
				using (SpreadsheetDocument spreadSheet = SpreadsheetDocument.Open(filepath, true))
				{
					Sheet firstSheet = spreadSheet.WorkbookPart.Workbook.Sheets.GetFirstChild<Sheet>();
					firstSheet.Remove();
					spreadSheet.WorkbookPart.Workbook.Save();
					createWorksheetEvtsFreq(0, 0, models, spreadSheet.WorkbookPart, false);
					createWorksheetStatesFreq(0, 0, models, spreadSheet.WorkbookPart, false);
				}
			}
		}

		private static void createWorksheetEvtsFreq(double conf, double sup, Dictionary<int, PredictiveStudentModel> models, WorkbookPart workbookPart, bool includeNoPlanActions)
		{
			SharedStringTablePart shareStringPart;
			if (workbookPart.GetPartsOfType<SharedStringTablePart>().Count() > 0)
			{
				shareStringPart = workbookPart.GetPartsOfType<SharedStringTablePart>().First();
			}
			else {
				shareStringPart = workbookPart.AddNewPart<SharedStringTablePart>();
			}

			WorksheetPart worksheetPart = InsertWorksheet(workbookPart, "Events" + conf.ToString(new CultureInfo("en-US")) + " " + sup.ToString(new CultureInfo("en-US")));

			#region headersheet
			int indexSharedString = InsertSharedStringItem("Cluster", shareStringPart);
			DocumentFormat.OpenXml.Spreadsheet.Cell cell = InsertCellInWorksheet("A", 1, worksheetPart);
			cell.CellValue = new CellValue(indexSharedString.ToString());
			cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

			indexSharedString = InsertSharedStringItem("StudentsCluster", shareStringPart);
			cell = InsertCellInWorksheet("B", 1, worksheetPart);
			cell.CellValue = new CellValue(indexSharedString.ToString());
			cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

			indexSharedString = InsertSharedStringItem("Event", shareStringPart);
			cell = InsertCellInWorksheet("C", 1, worksheetPart);
			cell.CellValue = new CellValue(indexSharedString.ToString());
			cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

			indexSharedString = InsertSharedStringItem("EventFreq", shareStringPart);
			cell = InsertCellInWorksheet("D", 1, worksheetPart);
			cell.CellValue = new CellValue(indexSharedString.ToString());
			cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
			#endregion

			int currentExcelRow = 1;

			foreach (PredictiveStudentModel model in models.Values)
			{
				foreach (StudentsCluster clu in model.Clusters.Values)
				{
					List<Arc<State, Event>> evts = clu.StudentActionsModel.GetListEvents();

					foreach (Arc<State, Event> evt in evts)
					{
						long stateFreq = evt.NodeOut.Specification.StudentFrequency;
						double supNodeOut = (double)stateFreq / clu.NumberOfStudents;
						long evtFreq = evt.Specification.GetType() == typeof(VectorEvent) ? ((VectorEvent)evt.Specification).Frequency.Sum() : ((NormalEvent)evt.Specification).Frequency;
						double supEvt = (double)evtFreq / clu.NumberOfStudents;
						double confEvt = supNodeOut == 0 ? 0 : supEvt / supNodeOut;
						string evtKey = evt.Key.ToString();

						if (confEvt >= conf && supNodeOut >= sup)
						{
							currentExcelRow++;
							cell = InsertCellInWorksheet("A", Convert.ToUInt32(currentExcelRow), worksheetPart);
							cell.CellValue = new CellValue(clu.Number.ToString());
							cell.DataType = new EnumValue<CellValues>(CellValues.Number);
							cell = InsertCellInWorksheet("B", Convert.ToUInt32(currentExcelRow), worksheetPart);
							cell.CellValue = new CellValue(clu.NumberOfStudents.ToString());
							cell.DataType = new EnumValue<CellValues>(CellValues.Number);
							cell = InsertCellInWorksheet("C", Convert.ToUInt32(currentExcelRow), worksheetPart);
							cell.CellValue = new CellValue(indexSharedString.ToString());
							cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
							indexSharedString = InsertSharedStringItem(evtKey, shareStringPart);
							cell = InsertCellInWorksheet("D", Convert.ToUInt32(currentExcelRow), worksheetPart);
							cell.CellValue = new CellValue(evtFreq.ToString());
							cell.DataType = new EnumValue<CellValues>(CellValues.Number);
						}
					}
				}
			}
			worksheetPart.Worksheet.Save();
		}

		private static void createWorksheetStatesFreq(double conf, double sup, Dictionary<int, PredictiveStudentModel> models, WorkbookPart workbookPart, bool includeNoPlanActions)
		{
			SharedStringTablePart shareStringPart;
			if (workbookPart.GetPartsOfType<SharedStringTablePart>().Count() > 0)
			{
				shareStringPart = workbookPart.GetPartsOfType<SharedStringTablePart>().First();
			}
			else {
				shareStringPart = workbookPart.AddNewPart<SharedStringTablePart>();
			}

			WorksheetPart worksheetPart = InsertWorksheet(workbookPart, "States"+conf.ToString(new CultureInfo("en-US")) + " " + sup.ToString(new CultureInfo("en-US")));

			#region headersheet
			int indexSharedString = InsertSharedStringItem("Cluster", shareStringPart);
			DocumentFormat.OpenXml.Spreadsheet.Cell cell = InsertCellInWorksheet("A", 1, worksheetPart);
			cell.CellValue = new CellValue(indexSharedString.ToString());
			cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

			indexSharedString = InsertSharedStringItem("StudentsCluster", shareStringPart);
			cell = InsertCellInWorksheet("B", 1, worksheetPart);
			cell.CellValue = new CellValue(indexSharedString.ToString());
			cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

			indexSharedString = InsertSharedStringItem("State", shareStringPart);
			cell = InsertCellInWorksheet("C", 1, worksheetPart);
			cell.CellValue = new CellValue(indexSharedString.ToString());
			cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

			indexSharedString = InsertSharedStringItem("Area", shareStringPart);
			cell = InsertCellInWorksheet("D", 1, worksheetPart);
			cell.CellValue = new CellValue(indexSharedString.ToString());
			cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

			indexSharedString = InsertSharedStringItem("StateFreq", shareStringPart);
			cell = InsertCellInWorksheet("E", 1, worksheetPart);
			cell.CellValue = new CellValue(indexSharedString.ToString());
			cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
			#endregion

			int currentExcelRow = 1;

			foreach (PredictiveStudentModel model in models.Values)
			{
				foreach (StudentsCluster clu in model.Clusters.Values)
				{
					List<Node<State, Event>> states = clu.StudentActionsModel.GetListStates();

					foreach (Node<State, Event> state in states)
					{
						long stateFreq = state.Specification.StudentFrequency;
						double supNode = (double)stateFreq / clu.NumberOfStudents;
						string stKey = state.Key;

						if (supNode >= sup)
						{
							currentExcelRow++;
							cell = InsertCellInWorksheet("A", Convert.ToUInt32(currentExcelRow), worksheetPart);
							cell.CellValue = new CellValue(clu.Number.ToString());
							cell.DataType = new EnumValue<CellValues>(CellValues.Number);
							cell = InsertCellInWorksheet("B", Convert.ToUInt32(currentExcelRow), worksheetPart);
							cell.CellValue = new CellValue(clu.NumberOfStudents.ToString());
							cell.DataType = new EnumValue<CellValues>(CellValues.Number);
							indexSharedString = InsertSharedStringItem(stKey, shareStringPart);
							cell = InsertCellInWorksheet("C", Convert.ToUInt32(currentExcelRow), worksheetPart);
							cell.CellValue = new CellValue(indexSharedString.ToString());
							cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
							indexSharedString = InsertSharedStringItem(state.Specification.Area.ToString(), shareStringPart);
							cell = InsertCellInWorksheet("D", Convert.ToUInt32(currentExcelRow), worksheetPart);
							cell.CellValue = new CellValue(indexSharedString.ToString());
							cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
							cell = InsertCellInWorksheet("E", Convert.ToUInt32(currentExcelRow), worksheetPart);
							cell.CellValue = new CellValue(stateFreq.ToString());
							cell.DataType = new EnumValue<CellValues>(CellValues.Number);
						}
					}
				}
			}
			worksheetPart.Worksheet.Save();
		}

		private static void CreateWorkbooksVal (string strDomainName, int perValidation, int iterations, ClusterMethod cluMet, bool logByLog, bool includeNoPlanActions, bool inPhases){
			int phases = CreateModel(strDomainName, cluMet, iterations, perValidation, logByLog, includeNoPlanActions, inPhases);

			for (int it = 0; it < iterations; it++)
			{
				//modelo predictivo
				Dictionary<int, PredictiveStudentModel> models = new Dictionary<int, PredictiveStudentModel>();
				if (inPhases)
				{
					for (int i = 0; i <= phases; i++)
					{
						models.Add(i, StudentBehaviorPredictorControl.Instance(config).GetModel(i +"_"+ strDomainName + it, cluMet));
					}
				}
				else
					models.Add(0,StudentBehaviorPredictorControl.Instance(config).GetModel(strDomainName + it, cluMet));
						
				

				string filepath = cluMet + "_val" + perValidation + "_" + it + ".xlsx";
				if (File.Exists (filepath))
					File.Delete (filepath);

				File.Copy("default.xlsx", filepath, true);
				using (SpreadsheetDocument spreadSheet = SpreadsheetDocument.Open (filepath, true)) {
					Sheet firstSheet = spreadSheet.WorkbookPart.Workbook.Sheets.GetFirstChild<Sheet> ();
					firstSheet.Remove ();
					spreadSheet.WorkbookPart.Workbook.Save ();

					if (logByLog) {
						//Dictionary<int, List<StudentLog>> logsVal = validationLogs[strDomainName + it + "Val"];
						createWorksheetValidation (0, 0, models, validationLogs, spreadSheet.WorkbookPart, includeNoPlanActions);
						createWorksheetValidation (0, 0.1, models, validationLogs, spreadSheet.WorkbookPart, includeNoPlanActions);
						createWorksheetValidation (0, 0.25, models, validationLogs, spreadSheet.WorkbookPart, includeNoPlanActions);
						createWorksheetValidation (0, 0.5, models, validationLogs, spreadSheet.WorkbookPart, includeNoPlanActions);
						createWorksheetValidation (0, 0.75, models, validationLogs, spreadSheet.WorkbookPart, includeNoPlanActions);
						createWorksheetValidation (0, 0.9, models, validationLogs, spreadSheet.WorkbookPart, includeNoPlanActions);
						createWorksheetValidation (0.1, 0, models, validationLogs, spreadSheet.WorkbookPart, includeNoPlanActions);
						createWorksheetValidation (0.1, 0.1, models, validationLogs, spreadSheet.WorkbookPart, includeNoPlanActions);
						createWorksheetValidation (0.1, 0.25, models, validationLogs, spreadSheet.WorkbookPart, includeNoPlanActions);
						createWorksheetValidation (0.1, 0.5, models, validationLogs, spreadSheet.WorkbookPart, includeNoPlanActions);
						createWorksheetValidation (0.1, 0.75, models, validationLogs, spreadSheet.WorkbookPart, includeNoPlanActions);
						createWorksheetValidation (0.1, 0.9, models, validationLogs, spreadSheet.WorkbookPart, includeNoPlanActions);
						createWorksheetValidation (0.25, 0, models, validationLogs, spreadSheet.WorkbookPart, includeNoPlanActions);
						createWorksheetValidation (0.25, 0.1, models, validationLogs, spreadSheet.WorkbookPart, includeNoPlanActions);
						createWorksheetValidation (0.25, 0.25, models, validationLogs, spreadSheet.WorkbookPart, includeNoPlanActions);
						createWorksheetValidation (0.25, 0.5, models, validationLogs, spreadSheet.WorkbookPart, includeNoPlanActions);
						createWorksheetValidation (0.25, 0.75, models, validationLogs, spreadSheet.WorkbookPart, includeNoPlanActions);
						createWorksheetValidation (0.25, 0.9, models, validationLogs, spreadSheet.WorkbookPart, includeNoPlanActions);
						createWorksheetValidation (0.5, 0, models, validationLogs, spreadSheet.WorkbookPart, includeNoPlanActions);
						createWorksheetValidation (0.5, 0.1, models, validationLogs, spreadSheet.WorkbookPart, includeNoPlanActions);
						createWorksheetValidation (0.5, 0.25, models, validationLogs, spreadSheet.WorkbookPart, includeNoPlanActions);
						createWorksheetValidation (0.5, 0.5, models, validationLogs, spreadSheet.WorkbookPart, includeNoPlanActions);
						createWorksheetValidation (0.5, 0.75, models, validationLogs, spreadSheet.WorkbookPart, includeNoPlanActions);
						createWorksheetValidation (0.5, 0.9, models, validationLogs, spreadSheet.WorkbookPart, includeNoPlanActions);
						createWorksheetValidation (0.75, 0, models, validationLogs, spreadSheet.WorkbookPart, includeNoPlanActions);
						createWorksheetValidation (0.75, 0.1, models, validationLogs, spreadSheet.WorkbookPart, includeNoPlanActions);
						createWorksheetValidation (0.75, 0.25, models, validationLogs, spreadSheet.WorkbookPart, includeNoPlanActions);
						createWorksheetValidation (0.75, 0.5, models, validationLogs, spreadSheet.WorkbookPart, includeNoPlanActions);
						createWorksheetValidation (0.75, 0.75, models, validationLogs, spreadSheet.WorkbookPart, includeNoPlanActions);
						createWorksheetValidation (0.75, 0.9, models, validationLogs, spreadSheet.WorkbookPart, includeNoPlanActions);
						createWorksheetValidation (0.9, 0, models, validationLogs, spreadSheet.WorkbookPart, includeNoPlanActions);
						createWorksheetValidation (0.9, 0.1, models, validationLogs, spreadSheet.WorkbookPart, includeNoPlanActions);
						createWorksheetValidation (0.9, 0.25, models, validationLogs, spreadSheet.WorkbookPart, includeNoPlanActions);
						createWorksheetValidation (0.9, 0.5, models, validationLogs, spreadSheet.WorkbookPart, includeNoPlanActions);
						createWorksheetValidation (0.9, 0.75, models, validationLogs, spreadSheet.WorkbookPart, includeNoPlanActions);
						createWorksheetValidation (0.9, 0.9, models, validationLogs, spreadSheet.WorkbookPart, includeNoPlanActions);
					}
				}
			}
		}

		/*private static void CreateWorkbooksValStudentByStudent (string strDomainName, ClusterMethod cluMet, int year, bool includeNoPlanActions){
			string ontologyPath = ConfigurationManager.AppSettings ["ontologyPath"].ToString ().Replace ('\\', Path.DirectorySeparatorChar);
			string logsPath = ConfigurationManager.AppSettings ["logsPath"].ToString ().Replace ('\\', Path.DirectorySeparatorChar);
			string expertConfPath = ConfigurationManager.AppSettings ["domainConfigurationPath"].Replace ('\\', Path.DirectorySeparatorChar);
			ExpertControl expert = ExpertControl.Instance (ontologyPath, logsPath, expertConfPath);
			DomainActions domain = expert.CreateDomain (strDomainName);
			WorldControl world = WorldControl.Instance (ontologyPath, logsPath);
			DomainLog logs = StudentControl.Instance (ontologyPath, logsPath).GetDomainLogsFromOntology (domain, expert.OtherErrors, world.WorldErrors);
			List<Student> students = logs.GetStudents ();

			foreach (Student stu in students) {
				CreateModel (domain, logs, stu, cluMet, year, includeNoPlanActions);
				//modelo predictivo
				PredictiveStudentModel model = StudentBehaviorPredictorControl.Instance(config).GetModel (strDomainName, cluMet);

				string filepath = cluMet + "_valStu"+stu.Key+".xlsx";
				if (File.Exists (filepath))
					File.Delete (filepath);

				File.Copy ("default.xlsx", filepath, true);
				using (SpreadsheetDocument spreadSheet = SpreadsheetDocument.Open (filepath, true)) {
					Sheet firstSheet = spreadSheet.WorkbookPart.Workbook.Sheets.GetFirstChild<Sheet> ();
					firstSheet.Remove ();
					spreadSheet.WorkbookPart.Workbook.Save ();

					Dictionary<int,List<StudentLog>> logsVal = validationLogs [strDomainName + "Val"];
					createWorksheetValidation (0, 0, model, logsVal, spreadSheet.WorkbookPart, includeNoPlanActions);
					createWorksheetValidation (0, 0.1, model, logsVal, spreadSheet.WorkbookPart, includeNoPlanActions);
					createWorksheetValidation (0, 0.25, model, logsVal, spreadSheet.WorkbookPart, includeNoPlanActions);
					createWorksheetValidation (0, 0.5, model, logsVal, spreadSheet.WorkbookPart, includeNoPlanActions);
					createWorksheetValidation (0, 0.75, model, logsVal, spreadSheet.WorkbookPart, includeNoPlanActions);
					createWorksheetValidation (0, 0.9, model, logsVal, spreadSheet.WorkbookPart, includeNoPlanActions);
					createWorksheetValidation (0.1, 0, model, logsVal, spreadSheet.WorkbookPart, includeNoPlanActions);
					createWorksheetValidation (0.1, 0.1, model, logsVal, spreadSheet.WorkbookPart, includeNoPlanActions);
					createWorksheetValidation (0.1, 0.25, model, logsVal, spreadSheet.WorkbookPart, includeNoPlanActions);
					createWorksheetValidation (0.1, 0.5, model, logsVal, spreadSheet.WorkbookPart, includeNoPlanActions);
					createWorksheetValidation (0.1, 0.75, model, logsVal, spreadSheet.WorkbookPart, includeNoPlanActions);
					createWorksheetValidation (0.1, 0.9, model, logsVal, spreadSheet.WorkbookPart, includeNoPlanActions);
					createWorksheetValidation (0.25, 0, model, logsVal, spreadSheet.WorkbookPart, includeNoPlanActions);
					createWorksheetValidation (0.25, 0.1, model, logsVal, spreadSheet.WorkbookPart, includeNoPlanActions);
					createWorksheetValidation (0.25, 0.25, model, logsVal, spreadSheet.WorkbookPart, includeNoPlanActions);
					createWorksheetValidation (0.25, 0.5, model, logsVal, spreadSheet.WorkbookPart, includeNoPlanActions);
					createWorksheetValidation (0.25, 0.75, model, logsVal, spreadSheet.WorkbookPart, includeNoPlanActions);
					createWorksheetValidation (0.25, 0.9, model, logsVal, spreadSheet.WorkbookPart, includeNoPlanActions);
					createWorksheetValidation (0.5, 0, model, logsVal, spreadSheet.WorkbookPart, includeNoPlanActions);
					createWorksheetValidation (0.5, 0.1, model, logsVal, spreadSheet.WorkbookPart, includeNoPlanActions);
					createWorksheetValidation (0.5, 0.25, model, logsVal, spreadSheet.WorkbookPart, includeNoPlanActions);
					createWorksheetValidation (0.5, 0.5, model, logsVal, spreadSheet.WorkbookPart, includeNoPlanActions);
					createWorksheetValidation (0.5, 0.75, model, logsVal, spreadSheet.WorkbookPart, includeNoPlanActions);
					createWorksheetValidation (0.5, 0.9, model, logsVal, spreadSheet.WorkbookPart, includeNoPlanActions);
					createWorksheetValidation (0.75, 0, model, logsVal, spreadSheet.WorkbookPart, includeNoPlanActions);
					createWorksheetValidation (0.75, 0.1, model, logsVal, spreadSheet.WorkbookPart, includeNoPlanActions);
					createWorksheetValidation (0.75, 0.25, model, logsVal, spreadSheet.WorkbookPart, includeNoPlanActions);
					createWorksheetValidation (0.75, 0.5, model, logsVal, spreadSheet.WorkbookPart, includeNoPlanActions);
					createWorksheetValidation (0.75, 0.75, model, logsVal, spreadSheet.WorkbookPart, includeNoPlanActions);
					createWorksheetValidation (0.75, 0.9, model, logsVal, spreadSheet.WorkbookPart, includeNoPlanActions);
					createWorksheetValidation (0.9, 0, model, logsVal, spreadSheet.WorkbookPart, includeNoPlanActions);
					createWorksheetValidation (0.9, 0.1, model, logsVal, spreadSheet.WorkbookPart, includeNoPlanActions);
					createWorksheetValidation (0.9, 0.25, model, logsVal, spreadSheet.WorkbookPart, includeNoPlanActions);
					createWorksheetValidation (0.9, 0.5, model, logsVal, spreadSheet.WorkbookPart, includeNoPlanActions);
					createWorksheetValidation (0.9, 0.75, model, logsVal, spreadSheet.WorkbookPart, includeNoPlanActions);
					createWorksheetValidation (0.9, 0.9, model, logsVal, spreadSheet.WorkbookPart, includeNoPlanActions);
				}
				Dispose ();
			}
		}
*/
		/*private static void validarModelosConf (string strDomainName, int perValidation, int iterations, ClusterMethod cluMet, bool logByLog, int mult){
			CreateModel (strDomainName, cluMet, iterations, perValidation, logByLog, mult);

			for (int it = 0; it < iterations; it++) {
				//modelo predictivo
				PredictiveStudentModel model = StudentBehaviorPredictorControl.Instance(config).GetModel (strDomainName + it);
				StringBuilder strExcelXml = new StringBuilder ();
				// Excel header
				#region Header
				strExcelXml.Append ("<?xml version=\"1.0\"?>");
				strExcelXml.Append ("<?mso-application progid=\"Excel.Sheet\"?>");
				strExcelXml.Append ("<Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\" ");
				strExcelXml.Append ("xmlns:o=\"urn:schemas-microsoft-com:office:office\" ");
				strExcelXml.Append ("xmlns:x=\"urn:schemas-microsoft-com:office:excel\" ");
				strExcelXml.Append ("xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\" ");
				strExcelXml.Append ("xmlns:html=\"http://www.w3.org/TR/REC-html40\">");
				strExcelXml.Append ("<DocumentProperties xmlns=\"urn:schemas-microsoft-com:office:office\">");
				strExcelXml.Append ("</DocumentProperties>");
				strExcelXml.Append ("<ExcelWorkbook xmlns=\"urn:schemas-microsoft-com:office:excel\">");
				strExcelXml.Append ("<ProtectStructure>False</ProtectStructure>");
				strExcelXml.Append ("<ProtectWindows>False</ProtectWindows>");
				strExcelXml.Append ("</ExcelWorkbook>");

				strExcelXml.Append ("<Styles>");
				strExcelXml.Append ("<Style ss:ID=\"Default\">");
				strExcelXml.Append ("<Alignment ss:Vertical=\"Bottom\"/>");
				strExcelXml.Append ("<Borders/>");
				strExcelXml.Append ("<Font/>");
				strExcelXml.Append ("<Interior/>");
				strExcelXml.Append ("<NumberFormat/>");
				strExcelXml.Append ("<Protection/>");
				strExcelXml.Append ("</Style>");
				strExcelXml.Append ("<Style ss:ID=\"bold\">");
				strExcelXml.Append ("<Font ss:Bold=\"1\"/>");
				strExcelXml.Append ("</Style>");
				strExcelXml.Append ("</Styles>");
				#endregion

				if (!logByLog) {
					//modelo para validar
					Dictionary<int,PredictiveStudentModel> modelsVal = validationModels [strDomainName + it + "Val"];

					strExcelXml.Append (validarModelo (0, 0, model, modelsVal));
					strExcelXml.Append ("<WorksheetOptions xmlns=\"urn:schemas-microsoft-com:office:excel\"><Selected/></WorksheetOptions>");
					strExcelXml.Append (validarModelo (0.1, 0, model, modelsVal));
					strExcelXml.Append (validarModelo (0.1, 0.1, model, modelsVal));
					strExcelXml.Append (validarModelo (0.25, 0, model, modelsVal));
					strExcelXml.Append (validarModelo (0.25, 0.25, model, modelsVal));
					strExcelXml.Append (validarModelo (0.3, 0, model, modelsVal));
					strExcelXml.Append (validarModelo (0.3, 0.3, model, modelsVal));
					strExcelXml.Append (validarModelo (0.5, 0, model, modelsVal));
					strExcelXml.Append (validarModelo (0.5, 0.25, model, modelsVal));
					strExcelXml.Append (validarModelo (0.5, 0.5, model, modelsVal));
					strExcelXml.Append (validarModelo (0.5, 0.75, model, modelsVal));
					strExcelXml.Append (validarModelo (0.75, 0, model, modelsVal));
					strExcelXml.Append (validarModelo (0.75, 0.25, model, modelsVal));
					strExcelXml.Append (validarModelo (0.75, 0.5, model, modelsVal));
					strExcelXml.Append (validarModelo (0.75, 0.75, model, modelsVal));
					strExcelXml.Append (validarModelo (0.75, 0.9, model, modelsVal));
					strExcelXml.Append (validarModelo (0.8, 0, model, modelsVal));
					strExcelXml.Append (validarModelo (0.8, 0.25, model, modelsVal));
					strExcelXml.Append (validarModelo (0.8, 0.5, model, modelsVal));
					strExcelXml.Append (validarModelo (0.8, 0.75, model, modelsVal));
					strExcelXml.Append (validarModelo (0.8, 0.9, model, modelsVal));
					strExcelXml.Append (validarModelo (0.9, 0, model, modelsVal));
					strExcelXml.Append (validarModelo (0.9, 0.25, model, modelsVal));
					strExcelXml.Append (validarModelo (0.9, 0.5, model, modelsVal));
					strExcelXml.Append (validarModelo (0.9, 0.75, model, modelsVal));
					strExcelXml.Append (validarModelo (0.9, 0.9, model, modelsVal));
				} else {
					//logs para validar
					Dictionary<int,List<StudentLog>> logsVal = validationLogs [strDomainName + it + "Val"];

					strExcelXml.Append (validarModelo (0, 0, model, logsVal));
					strExcelXml.Append ("<WorksheetOptions xmlns=\"urn:schemas-microsoft-com:office:excel\"><Selected/></WorksheetOptions>");
					strExcelXml.Append (validarModelo (0.1, 0, model, logsVal));
					strExcelXml.Append (validarModelo (0.1, 0.1, model, logsVal));
					strExcelXml.Append (validarModelo (0.25, 0, model, logsVal));
					strExcelXml.Append (validarModelo (0.25, 0.25, model, logsVal));
					strExcelXml.Append (validarModelo (0.3, 0, model, logsVal));
					strExcelXml.Append (validarModelo (0.3, 0.3, model, logsVal));
					strExcelXml.Append (validarModelo (0.5, 0, model, logsVal));
					strExcelXml.Append (validarModelo (0.5, 0.25, model, logsVal));
					strExcelXml.Append (validarModelo (0.5, 0.5, model, logsVal));
					strExcelXml.Append (validarModelo (0.5, 0.75, model, logsVal));
					strExcelXml.Append (validarModelo (0.75, 0, model, logsVal));
					strExcelXml.Append (validarModelo (0.75, 0.25, model, logsVal));
					strExcelXml.Append (validarModelo (0.75, 0.5, model, logsVal));
					strExcelXml.Append (validarModelo (0.75, 0.75, model, logsVal));
					strExcelXml.Append (validarModelo (0.75, 0.9, model, logsVal));
					strExcelXml.Append (validarModelo (0.8, 0, model, logsVal));
					strExcelXml.Append (validarModelo (0.8, 0.25, model, logsVal));
					strExcelXml.Append (validarModelo (0.8, 0.5, model, logsVal));
					strExcelXml.Append (validarModelo (0.8, 0.75, model, logsVal));
					strExcelXml.Append (validarModelo (0.8, 0.9, model, logsVal));
					strExcelXml.Append (validarModelo (0.9, 0, model, logsVal));
					strExcelXml.Append (validarModelo (0.9, 0.25, model, logsVal));
					strExcelXml.Append (validarModelo (0.9, 0.5, model, logsVal));
					strExcelXml.Append (validarModelo (0.9, 0.75, model, logsVal));
					strExcelXml.Append (validarModelo (0.9, 0.9, model, logsVal));
				}
			
				strExcelXml.Append ("</Workbook>\n");

				string path = cluMet + "_val" + perValidation + "_" + it + ".xls";
				if (File.Exists (path))
					File.Delete (path);

				System.IO.StreamWriter sw = new System.IO.StreamWriter (path, true, System.Text.Encoding.Unicode);
				sw.Write (strExcelXml.ToString ());
				sw.Close ();
			}
		}
*/
		
		private static string GetStateKey (LogEntry log, Area area)
		{
			string stateKey = "";
			if (log.GetType ().BaseType == typeof(ActionLog)) {
				stateKey = log.Action.Key+"_"+area.ToString();
			} else if (log.GetType () == typeof(DepErrorLog)) {
				Dependence fail = ((DepErrorLog)log).FailedDependence;
				stateKey = log.Action.Key + "_" + fail.Key+"_"+area.ToString();
			} else if (log.GetType () == typeof(IncompErrorLog)) {
				Incompatibility fail = ((IncompErrorLog)log).FailedIncompatibility;
				stateKey = log.Action.Key + "_" + fail.Key+"_"+area.ToString();
			} else if (log.GetType ().BaseType == typeof(TimeErrorLog)) {
				Error fail = (log.GetType () == typeof(MinTimeErrorLog)) ? log.Action.MinTimeError : log.Action.MaxTimeError;
				stateKey = log.Action.Key + "_" + fail.Key+"_"+area.ToString();
			} else if (log.GetType () == typeof(WorldErrorLog)) {
				Error fail = ((WorldErrorLog)log).ErrorAssociated;
				stateKey = log.Action.Key + "_" + fail.Key+"_"+area.ToString();
			} else if (log.GetType () == typeof(OtherErrorLog)) {
				Error fail = ((OtherErrorLog)log).ErrorAssociated;
				stateKey = log.Action.Key + "_" + fail.Key+"_"+area.ToString();
			}
			return stateKey;
		}

		private static Area GetArea(LogEntry log, Node<State,Event> previousState, List<Node<State,Event>> pastNodes, Node<State,Event> initState){
			Area area=previousState.Specification.Area;
			if (log.GetType ().BaseType == typeof(ActionLog)) {
				if (area == Area.CorrectFlow || area == Area.IrrelevantErrors) {
					if ((((ActionLog)log).Action.Dependence != null)) {
						if (IsDependeceRelevantError (((ActionLog)log).Action.Dependence.DependenciesList, pastNodes, initState) || HasPastNodeRelevantError(pastNodes))
							area = Area.RelevantErrors;
						else 
							area = Area.CorrectFlow;
					} else
						if (HasPastNodeRelevantError(pastNodes))
							area = Area.RelevantErrors;
						else 
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
			return area;
		}

		private static bool IsDependeceRelevantError (List<Dependence> dependences, List<Node<State,Event>> pastNodes, Node<State,Event> initState)
		{
			bool isRelevantError = false;
			List<Node<State,Event>> tempPasNodes = pastNodes.Where (x => x.Specification.GetType () == typeof(CorrectState) && x != initState).ToList();
			foreach (Dependence dep in dependences) {
				if (dep.GetType () == typeof(SimpleDependence)) {
					if (tempPasNodes.Count (x => ((CorrectState)x.Specification).Action.Key == ((SimpleDependence)dep).ActionDependence.Key) > 0) {
						if (tempPasNodes.Single (y => ((CorrectState)y.Specification).Action.Key == ((SimpleDependence)dep).ActionDependence.Key).Specification.Area == Area.RelevantErrors) {
							isRelevantError = true;
							break;
						}
					}
				} else {
					isRelevantError = IsDependeceRelevantError (((ComplexDependence)dep).DependenciesList, pastNodes, initState);
					if (isRelevantError)
						break;
				}
			}
			return isRelevantError;
		}

private static bool HasPastNodeRelevantError (List<Node<State,Event>> pastNodes)
{
	bool isRelevantError = false;
	for (int i = pastNodes.Count - 1; i >= 0 ; i--) {
		if (pastNodes [i].Specification.Area == Area.RelevantErrors) {
			isRelevantError = true;
			break;
		}
	}
	return isRelevantError;
}

		private static Node<State,Event> CreateState(LogEntry log, Area area){
			State state=null;
			Node<State,Event> newState = null;

			if (log.GetType ().BaseType == typeof(ActionLog)) {
				state = new CorrectState (area, log.Action, log.GetType () == typeof(CorrectiveActionLog));
				newState = new Node<State,Event> (log.Action.Key+"_"+area.ToString(), log.Action.Description, state);
			} else if (log.GetType () == typeof(DepErrorLog)) {
				Dependence fail = ((DepErrorLog)log).FailedDependence;
				state = new DependenceErrorState (area, fail);
				newState = new Node<State,Event> (log.Action.Key + "_" + fail.Key+"_"+area.ToString(), log.Action.Description, state);
			} else if (log.GetType () == typeof(IncompErrorLog)) {
				Incompatibility fail = ((IncompErrorLog)log).FailedIncompatibility;
				state = new IncompatibilityErrorState (area, fail);
				newState = new Node<State,Event> (log.Action.Key + "_" + fail.Key+"_"+area.ToString(), log.Action.Description, state);
			} else if (log.GetType ().BaseType == typeof(TimeErrorLog)) {
				Error fail = (log.GetType () == typeof(MinTimeErrorLog)) ? log.Action.MinTimeError : log.Action.MaxTimeError;
				state = new TimeErrorState (area, ((TimeErrorLog)log).Time, fail);
				newState = new Node<State,Event> (log.Action.Key + "_" + fail.Key+"_"+area.ToString(), log.Action.Description, state);
			} else if (log.GetType () == typeof(WorldErrorLog)) {
				Error fail = ((WorldErrorLog)log).ErrorAssociated;
				state = new WorldErrorState (area, fail, ((WorldErrorLog)log).Type);
				newState = new Node<State,Event> (log.Action.Key + "_" + fail.Key+"_"+area.ToString(), log.Action.Description, state);
			} else if (log.GetType () == typeof(OtherErrorLog)) {
				Error fail = ((OtherErrorLog)log).ErrorAssociated;
				state = new OtherErrorState (area, fail);
				newState = new Node<State,Event> (log.Action.Key + "_" + fail.Key+"_"+area.ToString(), log.Action.Description, state);
			}

			return newState;
		}
	}

	public class SubtotalArea{
		public Area Area{ get; set;}
		public int Freq{ get; set;}
		public double EN{ get; set;}
		public double EpN{ get; set;}
	}

	public class SubtotalCluster{
		public int Cluster{ get; set;}
		public int Freq{ get; set;}
		public double EN{ get; set;}
		public double EpN{ get; set;}
	}
}