using System;

public class ListBuilder
{
    public static List<string> CreateListOfResults(List<int> listOfResultIDs, List<double> listOfScores, List<string> listOfDates, List<int> listOfForeignClassIDs, List<int> listOfPrimaryClassIDs, List<string> listOfSurnames, List<string> listOfSubjects)
        {
            List<string> listOfClasses = new List<string>();
            List<string> listOfResults = new List<string>();
            for (int i = 0; i < listOfPrimaryClassIDs.Count; i++)
            {
                listOfClasses.Add("Teacher: " + listOfSurnames[i] + ", Subject: " + listOfSubjects[i]);
            }
            for (int i = 0; i < listOfResultIDs.Count; i++)
            {
                int foreignClassID = listOfForeignClassIDs[i];
                int classIndex = 0;
                for (int j = 0; j < listOfPrimaryClassIDs.Count; j++)
                {
                    if (listOfPrimaryClassIDs[j] == foreignClassID)
                    {
                        classIndex = j;
                    }
                }
                listOfResults.Add("Score: " + listOfScores[i] + "%, Date: " + listOfDates[i] + ", " + listOfClasses[classIndex]);
            }
            return listOfResults;
        }
}
