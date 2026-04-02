using System;

public class Calculation
{
    public static (int, bool) AttemptToConvertToInt(string stringToConvert)
    {
        int output;
        bool success;
        try
        {
            output = Convert.ToInt32(stringToConvert);
            success = true;
        }
        catch
        {
            output = -1;
            success = false;
        }
        return (output, success);
    }
    public static (double, bool) AttemptToConvertToDouble(string stringToConvert)
    {
        double output;
        bool success;
        try
        {
            output = Convert.ToDouble(stringToConvert);
            success = true;
        }
        catch
        {
            output = -1;
            success = false;
        }
        return (output, success);
    }
    public static double ReturnProductMomentCoefficientFromResultData(List<double> listOfScores, List<string> listOfDates)
    {

        List<double> listOfDatesAsDoubles = new List<double>();
        for (int i = 0; i < listOfDates.Count; i++)
        {
            listOfDatesAsDoubles.Add(ConvertDateToDouble(listOfDates[i]));
        }
        return Calculation.CalculateProductMomentCoefficient(listOfDatesAsDoubles, listOfScores);
    }
    public static double CalculateProductMomentCoefficient(List<double> xValues, List<double> yValues)
    {
        double r;
        double numPairs = xValues.Count;
        double sigmaX = 0;
        for (int i = 0; i < numPairs; i++)
        {
            sigmaX += xValues[i];
        }
        double sigmaY = 0;
        for (int i = 0; i < numPairs; i++)
        {
            sigmaY += yValues[i];
        }
        double sigmaXY = 0;
        for (int i = 0; i < numPairs; i++)
        {
            sigmaXY += xValues[i] * yValues[i];
        }
        double sigmaXSquared = 0;
        for (int i = 0; i < numPairs; i++)
        {
            sigmaXSquared += Math.Pow(xValues[i], 2);
        }
        double sigmaYSquared = 0;
        for (int i = 0; i < numPairs; i++)
        {
            sigmaYSquared += Math.Pow(yValues[i], 2);
        }
        double squaredSigmaX = Math.Pow(sigmaX, 2);
        double squaredSigmaY = Math.Pow(sigmaY, 2);
        r = (numPairs * sigmaXY - sigmaX * sigmaY) / (Math.Sqrt(numPairs * sigmaXSquared - squaredSigmaX) * Math.Sqrt(numPairs * sigmaYSquared - squaredSigmaY));
        return Convert.ToDouble(r.ToString("0.00#"));
    }
    public static double ConvertDateToDouble(string dateToConvert)
    {
        List<int> listOfMonthLengths = new List<int> { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        string year = dateToConvert.Substring(6, 4);
        string month = dateToConvert.Substring(3, 2);
        string day = dateToConvert.Substring(0, 2);
        (double yearAsDouble, _) = AttemptToConvertToDouble(year);
        (double monthAsDouble, _) = AttemptToConvertToDouble(month);
        (double dayAsDouble, _) = AttemptToConvertToDouble(day);
        dayAsDouble = (dayAsDouble - 1) / Convert.ToDouble(listOfMonthLengths[(int)monthAsDouble - 1]);
        monthAsDouble += dayAsDouble;
        monthAsDouble = (monthAsDouble - 1) / 12;
        yearAsDouble += monthAsDouble;
        return yearAsDouble;
    }
}