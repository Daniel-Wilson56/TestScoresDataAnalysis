using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using Org.BouncyCastle.Asn1.IsisMtt;
using System;

namespace TestScoresData
{
    public class UserInterface
    {
        private readonly DatabaseInterface database;
        private List<string> listOfMainOptions;
        public UserInterface(DatabaseInterface databaseInterface)
        {
            this.database = databaseInterface;
            listOfMainOptions = new List<string>();
            SetUpOptions(ref listOfMainOptions);
        }
        private void SetUpOptions(ref List<string> listOfOptions)
        {
            listOfOptions.Add("Add a new subject");
            listOfOptions.Add("Delete an existing subject");
            listOfOptions.Add("Add a new class");
            listOfOptions.Add("Delete an existing class");
            listOfOptions.Add("Add a result");
            listOfOptions.Add("Delete an existing result");
            listOfOptions.Add("See how you're doing");
        }
        private (int, bool) GetOptionFromList(List<string> list, bool cancelOptionNecessary)
        {
            int output;
            bool valid;
            List<string> printableList = new List<string>();
            for (int i = 0; i < list.Count; i++)
            {
                printableList.Add(list[i]);
                printableList[i] = Convert.ToString(i + 1) + ": " + (string)list[i];
            }
            Console.WriteLine();
            Console.WriteLine("Select one of the following: ");
            foreach (string item in printableList)
            {
                Console.WriteLine(item);
            }
            if (cancelOptionNecessary)
            {
                Console.WriteLine(Convert.ToString(list.Count + 1) + ": Cancel");
            }
            var (input, successfulConversion) = Calculation.AttemptToConvertToInt(Console.ReadLine());
            Console.WriteLine();
            if (successfulConversion && (input <= list.Count + 1))
            {
                output = input;
                valid = true;
            }
            else
            {
                output = -1;
                valid = false;
            }
            return (output, valid);
        }
        private (string, bool) GetDate()
        {
            string year = "0";
            string month = "0";
            int monthAsInt;
            string day = "0";
            int dayAsInt;
            bool valid = true;
            bool cancelled = false;
            List<string> listOfMonths = new List<string> { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            List<int> listOfMonthLengths = new List<int> { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            do
            {
                if (!valid)
                {
                    Console.WriteLine("Invalid input, try again");
                }
                Console.WriteLine("Enter year (4 digits). X to cancel");
                year = Console.ReadLine();
                if (year == "X")
                {
                    cancelled = true;
                }
                (_, valid) = Calculation.AttemptToConvertToInt(year);

            } while ((year.Length != 4 || !valid) && !cancelled);
            if (!cancelled)
            {
                do
                {
                    if (!valid)
                    {
                        Console.WriteLine("Invalid input, try again");
                    }
                    (monthAsInt, valid) = GetOptionFromList(listOfMonths, true);
                } while (!valid && monthAsInt != listOfMonths.Count + 1);
                if (monthAsInt == listOfMonths.Count + 1)
                {
                    cancelled = true;
                }
                else
                {
                    month = Convert.ToString(monthAsInt);
                    if (month.Length == 1)
                    {
                        month = "0" + month;
                    }
                    do
                    {
                        if (!valid)
                        {
                            Console.WriteLine("Invalid input, try again");
                        }
                        Console.WriteLine("Enter day (1 - " + listOfMonthLengths[monthAsInt - 1] + "). X to cancel");
                        string dayInput = Console.ReadLine();
                        if (dayInput != "X")
                        {
                            (dayAsInt, valid) = Calculation.AttemptToConvertToInt(dayInput);
                            if (dayAsInt < 1 || dayAsInt > listOfMonthLengths[monthAsInt - 1])
                            {
                                valid = false;
                            }
                        }
                        else
                        {
                            cancelled = true;
                            dayAsInt = 0;
                        }
                    } while (!valid && !cancelled);
                    if (!cancelled)
                    {
                        day = Convert.ToString(dayAsInt);
                        if (day.Length == 1)
                        {
                            day = "0" + day;
                        }
                    }
                }
            }
            return (year + "-" + month + "-" + day, cancelled);
        }
        public bool AskForOptions()
        {
            bool finished = false;
            (int input, bool valid) = GetOptionFromList(listOfMainOptions, true);
            switch (input)
            {
                case 1:
                    {
                        AddSubject();
                        break;
                    }
                case 2:
                    {
                        DeleteSubject();
                        break;
                    }
                case 3:
                    {
                        AddClass();
                        break;
                    }
                case 4:
                    {
                        DeleteClass();
                        break;
                    }
                case 5:
                    {
                        AddResult();
                        break;
                    }
                case 6:
                    {
                        DeleteResult();
                        break;
                    }
                case 7:
                    {
                        ShowPMC();
                        break;
                    }
                default:
                    {
                        if (!valid)
                        {
                            Console.WriteLine("Bad input, try again");
                        }
                        else
                        {
                            finished = true;
                        }
                        break;
                    }
            }
            return finished;
        }
        private void AddSubject()
        {
            Console.WriteLine("Enter subject name. X to cancel");
            string subjectNameToAdd = Console.ReadLine();
            if (subjectNameToAdd != null && subjectNameToAdd.Length <= 32 && subjectNameToAdd!="X")
            {
                database.AddSubject(subjectNameToAdd);
            }
            else if (subjectNameToAdd!="X")
            {
                Console.WriteLine("Invalid subject name");
            }
        }
        private void DeleteSubject()
        {
            (string subject, bool cancelled) = SelectSubject();
            if (!cancelled)
            {
                database.DeleteSubject(subject);
            }
        }
        private (string, bool) SelectSubject()
        {
            Console.WriteLine("Enter subject");
            List<string> listOfSubjects = database.GetListOfSubjects();
            string output = "";
            bool valid = false;
            bool cancelled = false;
            do
            {
                (int input, valid) = GetOptionFromList(listOfSubjects, true);
                if (valid && input <= listOfSubjects.Count)
                {
                    output = listOfSubjects[input - 1];
                }
                else if (input != listOfSubjects.Count + 1)
                {
                    Console.WriteLine("Your input was invalid in some way.");
                }
                else
                {
                    cancelled = true;
                    valid = true;
                }
            } while (!valid);
            return (output, cancelled);
        }
        private void AddClass()
        {
            Console.WriteLine("Enter teacher surname. X to cancel ");
            string teacherSurname = Console.ReadLine();
            if (teacherSurname != "X")
            {
                (string subject, bool cancelled) = SelectSubject();
                if (!cancelled)
                {
                    database.AddClass(teacherSurname, subject);
                }
            }
        }
        private void DeleteClass()
        {
            (int classID, bool cancelled) = SelectClassGivenSubject("");
            if (!cancelled)
            {
                database.DeleteClass(classID);
            }
        }
        private (int, bool) SelectClassGivenSubject(string subject)
        {
            (List<int> listOfIDs, List<string> listOfSurnames, List<string> listOfSubjects) = database.GetListOfClasses(subject);
            List<string> listOfClasses = new List<string>();
            int output = -1;
            bool valid = false;
            bool cancelled = false;
            for (int i = 0; i < listOfSurnames.Count; i++)
            {
                listOfClasses.Add("Teacher: " + listOfSurnames[i] + ", Subject: " + listOfSubjects[i]);
            }
            do
            {
                (int input, valid) = GetOptionFromList(listOfClasses, true);
                if (valid && input <= listOfClasses.Count)
                {
                    output = listOfIDs[input - 1];
                }
                else if (input != listOfSubjects.Count + 1)
                {
                    Console.WriteLine("Your input was invalid in some way.");
                }
                else
                {
                    cancelled = true;
                    valid = true;
                }
            } while (!valid);
            return (output, cancelled);
        }
        private void AddResult()
        {
            bool validScore;
            Console.WriteLine("Enter score. X to cancel");
            string scoreAsString = Console.ReadLine();
            if (scoreAsString != "X")
            {
                (double scoreAsDouble, validScore) = Calculation.AttemptToConvertToDouble(scoreAsString);
                if (scoreAsDouble < 0 || scoreAsDouble > 100)
                {
                    validScore = false;
                    Console.WriteLine("Score was invalid");
                }
                else
                {
                    (string date, bool cancelledGettingDate) = GetDate();
                    if (!cancelledGettingDate)
                    {
                        (int classID, bool cancelledGettingClass) = SelectClassGivenSubject("");
                        if (!cancelledGettingClass)
                        {
                            database.AddResult(scoreAsDouble, date, classID);
                        }
                    }
                }
            }
        }
        private void DeleteResult()
        {
            (List<int> listOfResultIDs, List<double> listOfScores, List<string> listOfDates, List<int> listOfForeignClassIDs) = database.GetListOfResults();
            (List<int> listOfPrimaryClassIDs, List<string> listOfSurnames, List<string> listOfSubjects) = database.GetListOfClasses("");
            List<string> listOfResults = ListBuilder.CreateListOfResults(listOfResultIDs, listOfScores, listOfDates, listOfForeignClassIDs, listOfPrimaryClassIDs, listOfSurnames, listOfSubjects);
            (int input, bool valid) = GetOptionFromList(listOfResults, true);
            if (valid && input <= listOfResults.Count)
            {
                database.DeleteResult(listOfResultIDs[input - 1]);
            }
            else if (input != listOfResultIDs.Count + 1)
            {
                Console.WriteLine("Your input was invalid in some way.");
            }
        }
        private double CalculatePMC()
        {
            List<double> listOfScores = new List<double>();
            List<string> listOfDates = new List<string>();
            List<string> filterOptions = new List<string>();
            filterOptions.Add("Subject");
            filterOptions.Add("Class");
            filterOptions.Add("No filter");
            string filter = "";
            bool valid = true;
            bool cancelled = false;
            Console.WriteLine("Enter what to filter by");
            do
            {
                (int filterNum, valid) = GetOptionFromList(filterOptions, true);
                if (valid && filterNum != filterOptions.Count + 1)
                {
                    filter = filterOptions[filterNum - 1];
                }
                else if (filterNum == filterOptions.Count + 1)
                {
                    cancelled = true;
                }
            } while (!valid && !cancelled);
            if (!cancelled)
            {
                if (filter == "No filter")
                {
                    int limit = 0;
                    (limit, cancelled) = GetLimit();
                    if (!cancelled)
                    {
                        if (limit != -1)
                        {
                            (_, listOfScores, listOfDates, _) = database.GetListOfResultsWhereNumberOfMostRecentIs(limit);
                        }
                        else
                        {
                            (_, listOfScores, listOfDates, _) = database.GetListOfResults();
                        }
                        return Calculation.ReturnProductMomentCoefficientFromResultData(listOfScores, listOfDates);
                    }
                }
                else if (filter == "Class")
                {
                    (int classID, cancelled) = SelectClassGivenSubject("");
                    int limit = 0;
                    if (!cancelled)
                    {
                        (limit, cancelled) = GetLimit();
                    }
                    if (!cancelled)
                    {
                        if (limit != -1)
                        {
                            (_, listOfScores, listOfDates, _) = database.GetListOfResultsWhereClassIDIs(classID, limit);
                        }
                        else
                        {
                            (_, listOfScores, listOfDates, _) = database.GetListOfResultsWhereClassIDIs(classID);
                        }
                        return Calculation.ReturnProductMomentCoefficientFromResultData(listOfScores, listOfDates);
                    }
                }
                else
                {
                    (string subjectName, cancelled) = SelectSubject();
                    int limit = 0;
                    if (!cancelled)
                    {
                        (limit, cancelled) = GetLimit();
                    }
                    if (!cancelled)
                    {
                        if (limit != -1)
                        {
                            (_, listOfScores, listOfDates, _) = database.GetListOfResultsWhereSubjectIs(subjectName, limit);
                        }
                        else
                        {
                            (_, listOfScores, listOfDates, _) = database.GetListOfResultsWhereSubjectIs(subjectName);
                        }
                        return Calculation.ReturnProductMomentCoefficientFromResultData(listOfScores, listOfDates);
                    }
                }
            }
            return -100;
        }
        private void ShowPMC()
        {
            double PMC = CalculatePMC();
            string positiveNegativeOrZero;
            string PMCStrength = "";
            if (PMC != -100)
            {
                if (PMC > 0)
                {
                    positiveNegativeOrZero = "positive";
                }
                else if (PMC < 0)
                {
                    positiveNegativeOrZero = "negative";
                }
                else
                {
                    positiveNegativeOrZero = "zero";
                }
                if (positiveNegativeOrZero != "zero")
                {
                    double absPMC = Math.Abs(PMC);
                    if (absPMC > 0.5)
                    {
                        PMCStrength = "strong";
                    }
                    else
                    {
                        PMCStrength = "weak";
                    }
                    Console.WriteLine("There is " + PMCStrength + " " + positiveNegativeOrZero + " correlation in your results. r = " + PMC);
                }
                else
                {
                    Console.WriteLine("There is 0 correlation - your marks are staying the same. r = 0");
                }
            }
        }
        private (int, bool) GetLimit()
        {
            bool valid = true;
            int limit = 0;
            bool cancelled = false;
            do
            {
                if (!valid)
                {
                    Console.WriteLine("Invalid input, try again");
                }
                Console.WriteLine("Enter number of results. X to cancel, ALL to show all results");
                string input = Console.ReadLine();
                if (input != "X")
                {
                    (limit, valid) = Calculation.AttemptToConvertToInt(input);
                    if (valid && limit < 0)
                    {
                        valid = false;
                    }
                    if (input == "ALL")
                    {
                        limit = -1;
                        valid = true;
                    }
                }
                else
                {
                    cancelled = true;
                }
            } while (!valid && !cancelled);
            return (limit, cancelled);
        }
    }
}