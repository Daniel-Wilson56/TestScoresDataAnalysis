using System;
using MySql.Data.MySqlClient;

namespace TestScoresData
{
	public class DatabaseInterface
	{
        private MySqlConnection connection;
        public bool ConnectToDatabase(string password)
        {
            bool success;
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
            {
                Server = "127.0.0.1",
                UserID = "root",
                Password = password,
                Database = "TestScoresDB"
            };
            string connectionString = builder.ConnectionString;
            connection = new();
            try
            {
                connection.ConnectionString = connectionString;
                connection.Open();
                success = true;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Failed connection: {ex.Message}");
                success = false;
                connection = null;
            }
            return success;
        }
        public List<string> GetListOfSubjects()
        {
            List<string> subjects = new List<string>();
            string selectQuery = "select SubjectName from tblSubjects;";
            using (MySqlCommand selectCommand = new(selectQuery, connection))
            {
                using (MySqlDataReader selectReader = selectCommand.ExecuteReader())
                {
                    while (selectReader.Read())
                    {
                        subjects.Add(selectReader.GetString("SubjectName"));
                    }
                }
            }
            return subjects;
        }
        public void AddSubject(string subjectNameToAdd)
        {
            string insertQuery = "insert into tblSubjects (SubjectName) values (@SubjectName);";
            using (MySqlCommand insertCommand = new(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@SubjectName", subjectNameToAdd);
                insertCommand.ExecuteNonQuery();
                Console.WriteLine("Inserted new subject.");
            }
        }
        public void DeleteSubject(string subjectToDelete)
        {
            string insertQuery = "delete from tblSubjects where SubjectName = @DeleteSubject;";
            using (MySqlCommand insertCommand = new(insertQuery, connection))
            {
                try
                {
                    insertCommand.Parameters.AddWithValue("@DeleteSubject", subjectToDelete);
                    insertCommand.Prepare();
                    insertCommand.ExecuteNonQuery();
                    Console.WriteLine("Deleted subject.");
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Could not delete subject - " + ex.Message);
                }
            }
        }
        public (List<int>, List<string>, List<string>) GetListOfClasses(string subjectToFilterBy)
        {
            string? overload;
            string selectQuery;
            if (subjectToFilterBy == "")
            {
                selectQuery = "select ClassID, TeacherSurname, SubjectName from tblClasses;";
                overload = null;
            }
            else
            {
                selectQuery = "select ClassID, TeacherSurname, SubjectName from tblClasses @Overload";
                overload = $"where SubjectName = {subjectToFilterBy};";
            }
            List<int> classIDs = new List<int>();
            List<string> teacherSurnames = new List<string>();
            List<string> subjectNames = new List<string>();
            using (MySqlCommand selectCommand = new(selectQuery, connection))
            {
                if (overload != null)
                {
                    selectCommand.Parameters.AddWithValue("@Overload", overload);
                }
                using (MySqlDataReader selectReader = selectCommand.ExecuteReader())
                {
                    while (selectReader.Read())
                    {
                        classIDs.Add(selectReader.GetInt32("ClassID"));
                        teacherSurnames.Add(selectReader.GetString("TeacherSurname"));
                        subjectNames.Add(selectReader.GetString("SubjectName"));
                    }
                }
            }
            return (classIDs, teacherSurnames, subjectNames);
        }
        public void AddClass(string teacherName, string subjectName)
        {
            string insertQuery = "insert into tblClasses (TeacherSurname, SubjectName) values (@TeacherSurname, @SubjectName);";
            using (MySqlCommand insertCommand = new(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@TeacherSurname", teacherName);
                insertCommand.Parameters.AddWithValue("@SubjectName", subjectName);
                insertCommand.ExecuteNonQuery();
                Console.WriteLine("Inserted new class.");
            }
        }
        public void DeleteClass(int classToDelete)
        {
            string insertQuery = "delete from tblClasses where ClassID = @DeleteClass;";
            using (MySqlCommand insertCommand = new(insertQuery, connection))
            {
                try
                {
                    insertCommand.Parameters.AddWithValue("@DeleteClass", classToDelete);
                    insertCommand.Prepare();
                    insertCommand.ExecuteNonQuery();
                    Console.WriteLine("Deleted class.");
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Could not delete class - " + ex.Message);
                }
            }
        }
        public (List<int>, List<double>, List<string>, List<int>) GetListOfResults(string selectQuery = "select ResultID, Score, DateTaken, ClassID from tblResults order by DateTaken;", string extraInfo = "", string extraInfoType = "")
        {
            List<int> resultIDs = new List<int>();
            List<double> scores = new List<double>();
            List<string> dates = new List<string>();
            List<int> classIDs = new List<int>();
            using (MySqlCommand selectCommand = new(selectQuery, connection))
            {
                if (extraInfo != "")
                {
                    if (extraInfoType == "ClassID")
                    {
                        selectCommand.Parameters.AddWithValue("@ClassID", extraInfo);
                    }
                    else
                    {
                        selectCommand.Parameters.AddWithValue("@Subject", extraInfo);
                    }
                }
                using (MySqlDataReader selectReader = selectCommand.ExecuteReader())
                {
                    while (selectReader.Read())
                    {
                        resultIDs.Add(selectReader.GetInt32("ResultID"));
                        scores.Add(selectReader.GetDouble("Score"));
                        dates.Add(Convert.ToString(selectReader.GetDateTime("DateTaken")));
                        classIDs.Add(selectReader.GetInt32("ClassID"));
                    }
                }
            }
            //This removes the hours and minutes from the date, since it doesn't get recorded and so is always 00:00
            for (int i = 0; i < dates.Count; i++)
            {
                if (dates[i].Length > 10)
                {
                    dates[i] = dates[i].Substring(0, 10);
                }
            }
            return (resultIDs, scores, dates, classIDs);
        }
        public void AddResult(double score, string date, int classID)
        {
            string insertQuery = "insert into tblResults (Score, DateTaken, ClassID) values (@Score, @DateTaken, @ClassID);";
            using (MySqlCommand insertCommand = new(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@Score", score);
                insertCommand.Parameters.AddWithValue("@DateTaken", date);
                insertCommand.Parameters.AddWithValue("@ClassID", classID);
                insertCommand.ExecuteNonQuery();
                Console.WriteLine("Inserted new result.");
            }
        }
        public void DeleteResult(int resultIDToDelete)
        {
            string insertQuery = "delete from tblResults where ResultID = @DeleteID;";
            using (MySqlCommand insertCommand = new(insertQuery, connection))
            {
                try
                {
                    insertCommand.Parameters.AddWithValue("@DeleteID", resultIDToDelete);
                    insertCommand.Prepare();
                    insertCommand.ExecuteNonQuery();
                    Console.WriteLine("Deleted result.");
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Could not delete result - " + ex.Message);
                }
            }
        }
        public (List<int>, List<double>, List<string>, List<int>) GetListOfResultsWhereClassIDIs(int classIDToSortBy)
        {
            string query = "select ResultID, Score, DateTaken, ClassID from tblResults where ClassID = @ClassID order by DateTaken;";
            return GetListOfResults(query, Convert.ToString(classIDToSortBy), "ClassID");
        }
        public (List<int>, List<double>, List<string>, List<int>) GetListOfResultsWhereClassIDIs(int classIDToSortBy, int limit)
        {
            string query = "select ResultID, Score, DateTaken, ClassID from tblResults where ClassID = @ClassID order by DateTaken limit " + limit + ";";
            return GetListOfResults(query, Convert.ToString(classIDToSortBy), "ClassID");
        }
        public (List<int>, List<double>, List<string>, List<int>) GetListOfResultsWhereSubjectIs(string subjectToSortBy)
        {
            string query = "select ResultID, Score, DateTaken, tblResults.ClassID from tblResults, tblClasses where tblResults.ClassID = tblClasses.ClassID and tblClasses.SubjectName = \"@Subject\";";
            return GetListOfResults(query, subjectToSortBy, "Subject");
        }
        public (List<int>, List<double>, List<string>, List<int>) GetListOfResultsWhereSubjectIs(string subjectToSortBy, int limit)
        {
            string query = "select ResultID, Score, DateTaken, tblResults.ClassID from tblResults, tblClasses where tblResults.ClassID = tblClasses.ClassID and tblClasses.SubjectName = \"@Subject\" order by DateTaken desc limit " + limit + ";";
            return GetListOfResults(query, subjectToSortBy, "Subject");
        }
        public (List<int>, List<double>, List<string>, List<int>) GetListOfResultsWhereNumberOfMostRecentIs(int limit)
        {
            string query = "select ResultID, Score, DateTaken, ClassID from tblResults order by DateTaken desc limit " + limit + ";";
            return GetListOfResults(query);
        }
    }
}
