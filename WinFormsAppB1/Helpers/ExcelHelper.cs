using Excel = Microsoft.Office.Interop.Excel;

namespace WinFormsAppB1.Helpers;

public class ExcelHelper : IDisposable
{
    private Excel.Application _excelApp;
    private Excel.Workbook _workbook;
    private Excel.Worksheet _worksheet;

    /// <summary>
    /// Constuctor for create instance of ExcelHelper.
    /// </summary>
    public ExcelHelper()
    {
        _excelApp = new Excel.Application();
    }

    /// <summary>
    /// Convert Data to object for write in Excel.
    /// </summary>
    /// <param name="rowsToAdd">count of row to add in Excel.</param>
    /// <param name="colsToAdd">count of col to add in Excel.</param>
    /// <param name="partOfData">part of data to write.</param>
    /// <returns>Array of objects</returns>
    private object[,] ConvertDataToObject(int rowsToAdd, int colsToAdd, List<string[]> partOfData)
    {
        object[,] dataArray = new object[rowsToAdd, colsToAdd];

        for (int row = 0; row < rowsToAdd; row++)
        {
            for (int col = 0; col < colsToAdd; col++)
            {
                dataArray[row, col] = partOfData[row][col];
            }
        }
        return dataArray;
    }

    /// <summary>
    /// Method that divide data on chunks with some limit and record in Excel.
    /// </summary>
    /// <param name="filePath">get data by this file path.</param>
    /// <param name="progressCallback">delegate that use for update UI components.</param>
    /// <param name="countRowsForAddInExcel">count of rows that will add in excel.</param>
    /// <param name="limit">limit that used to divide data on chunks.</param>
    public void AddDataInExcel(string filePath, Action<int, int, int> progressCallback, int countRowsForAddInExcel, int limit)
    {
        _workbook = _excelApp.Workbooks.Add();
        var lineCount = File.ReadLines(filePath).Count();
        List<string[]> partOfData = new List<string[]>();

        int index = 1;
        int countAddRows = 0;
        int worksheetIndex = 1;
        int countAddRowsInWoorkSheets = 0;

        if (lineCount > limit)
            partOfData.EnsureCapacity(limit);

        using (StreamReader reader = new StreamReader(filePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null && countAddRows + (worksheetIndex - 1) * 1_000_000 < countRowsForAddInExcel)
            {
                string[] data = line.Split("||");
                data[^2] = data[^2].Replace(',', '.');
                partOfData.Add(data);
                countAddRows++;

                if (partOfData.Count == limit || countAddRows == 1_000_000)
                {
                    countAddRowsInWoorkSheets = countAddRows + (worksheetIndex - 1) * 1_000_000;
                    progressCallback(countAddRowsInWoorkSheets, countRowsForAddInExcel - countAddRowsInWoorkSheets, (countAddRowsInWoorkSheets * 100) / countRowsForAddInExcel);

                    Excel.Worksheet currentSheet = (Excel.Worksheet)_workbook.Sheets[worksheetIndex];
                    Excel.Range startCell = currentSheet.Cells[(index - 1) * limit + 1, 1];
                    Excel.Range endCell = currentSheet.Cells[index * limit, 5];
                    Excel.Range writeRange = currentSheet.Range[startCell, endCell];

                    int rowsToAdd = partOfData.Count;
                    int colsToAdd = partOfData[0].Length;

                    object[,] dataArray = ConvertDataToObject(rowsToAdd, colsToAdd, partOfData);
                    writeRange.Value2 = dataArray;
                    partOfData.Clear();
                    index++;

                    if (countAddRows == 1_000_000)
                    {
                        _worksheet = (Excel.Worksheet)_workbook.Worksheets.Add(After: _workbook.Sheets[_workbook.Sheets.Count]);
                        worksheetIndex++;
                        countAddRows = 0;
                        index = 1;
                    }
                }
            }
            if (partOfData.Count != 0)
            {
                progressCallback(countRowsForAddInExcel, 0, 100);

                int rowsToAdd = partOfData.Count;
                int colsToAdd = partOfData[0].Length;

                Excel.Worksheet currentSheet = (Excel.Worksheet)_workbook.Sheets[worksheetIndex];
                Excel.Range startCell = currentSheet.Cells[(index - 1) * limit + 1, 1];
                Excel.Range endCell = currentSheet.Cells[(index - 1) * limit + partOfData.Count, 5];
                Excel.Range writeRange = currentSheet.Range[startCell, endCell];

                object[,] dataArray = ConvertDataToObject(rowsToAdd, colsToAdd, partOfData);

                writeRange.Value2 = dataArray;
                partOfData.Clear();
            }
            _workbook.SaveAs(Environment.CurrentDirectory + "\\ImportedData.xlsx");
        }
        _workbook.Close();

        Console.WriteLine("Data imported to Excel successfully.");
    }

    /// <summary>
    /// Read OSV excel file.
    /// </summary>
    /// <param name="filePath">FilePath to excel file.</param>
    /// <returns>Dictionary</returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<Dictionary<(int, string), List<List<double>>>> ReadOSV(string filePath)
    {
        string localFilePath = filePath;

        return await Task.Run(() =>
        {
            Excel.Workbook xlWorkbook = _excelApp.Workbooks.Open(localFilePath);
            Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
            Excel.Range xlRange = xlWorksheet.UsedRange;

            int rowCount = xlRange.Rows.Count;
            int colCount = xlRange.Columns.Count;
            Dictionary<(int, string), List<List<double>>> bankClasses = new();
            int indexClass = -1;
            string ClassName = "";
            List<double> values = new();
            values.EnsureCapacity(7);

            for (int i = 1; i <= rowCount; i++)
            {
                for (int j = 1; j <= colCount; j++)
                {

                    if (xlRange.Cells[i, j] == null || xlRange.Cells[i, j].Value2 == null)
                        break;

                    string text = xlRange.Cells[i, j].Value2.ToString();

                    if (text.ToUpper().Contains("КЛАСС"))
                    {
                        string[] textPart = text.Split(" ");
                        if (textPart.Length <= 2)
                            break;

                        if (!Int32.TryParse(textPart[2], out indexClass))
                            throw new ArgumentException(textPart[1], "not is int");

                        ClassName = String.Join(" ", textPart[4..]);

                        continue;
                    }

                    if (indexClass == -1)
                        break;

                    if (!double.TryParse(xlRange.Cells[i, j].Value2.ToString(), out double result))
                        break;

                    values.Add(result);
                }
                if (values.Count == 7)
                {
                    var key = (indexClass, ClassName);

                    if (!bankClasses.ContainsKey(key))
                    {
                        bankClasses[key] = new List<List<double>>(); // Initialize the key if it doesn't exist
                    }

                    bankClasses[key].Add(new List<double>(values)); // Add values to the corresponding key
                }
                values.Clear();
            }

            return bankClasses;
        });
    }

    /// <summary>
    /// Dispose instance of Excel.
    /// </summary>
    public void Dispose()
    {
        try
        {
            _excelApp.Quit();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}
