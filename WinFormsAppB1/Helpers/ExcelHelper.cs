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
        _workbook = _excelApp.Workbooks.Add();
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
        var lineCount = File.ReadLines(filePath).Count();
        List<string[]> partOfData = new List<string[]>();

        int index = 1;
        int countAddRows = 0;
        int worksheetIndex = 1;

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
                    progressCallback(countAddRows + (worksheetIndex - 1) * 1_000_000, countRowsForAddInExcel - (countAddRows + (worksheetIndex - 1) * 1_000_000), ((countAddRows + (worksheetIndex - 1) * 1_000_000) * 100) / countRowsForAddInExcel);

                    int rowsToAdd = partOfData.Count;
                    int colsToAdd = partOfData[0].Length;

                    Excel.Worksheet currentSheet = (Excel.Worksheet)_workbook.Sheets[worksheetIndex];
                    Excel.Range startCell = currentSheet.Cells[(index - 1) * limit + 1, 1];
                    Excel.Range endCell = currentSheet.Cells[index * limit, 5];
                    Excel.Range writeRange = currentSheet.Range[startCell, endCell];
                    object[,] dataArray = new object[rowsToAdd, colsToAdd];

                    for (int row = 0; row < rowsToAdd; row++)
                    {
                        for (int col = 0; col < colsToAdd; col++)
                        {
                            dataArray[row, col] = partOfData[row][col];
                        }
                    }
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

                object[,] dataArray = new object[rowsToAdd, colsToAdd];

                for (int row = 0; row < rowsToAdd; row++)
                {
                    for (int col = 0; col < colsToAdd; col++)
                    {
                        dataArray[row, col] = partOfData[row][col];
                    }
                }

                writeRange.Value2 = dataArray;
                partOfData.Clear();
            }
            _workbook.SaveAs(Environment.CurrentDirectory+"\\ImportedData.xlsx");
        }
        Console.WriteLine("Data imported to Excel successfully.");
    }

    /// <summary>
    /// Method that release memory of instance for excel obj.
    /// </summary>
    /// <param name="obj">inscance of excel component.</param>
    private static void ReleaseObject(object obj)
    {
        try
        {
            System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
            obj = null;
        }
        catch (Exception ex)
        {
            obj = null;
            Console.WriteLine("Exception Occurred while releasing object " + ex.ToString());
        }
        finally
        {
            GC.Collect();
        }
    }

    /// <summary>
    /// Dispose instance of Excel.
    /// </summary>
    public void Dispose()
    {
        try
        {
            _workbook.Close();
            _excelApp.Quit();

            ReleaseObject(_workbook);
            ReleaseObject(_excelApp);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}
