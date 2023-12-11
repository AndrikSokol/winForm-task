using System.Data.SqlClient;
using System.Numerics;
using WinFormsAppB1.Helpers;

namespace WinFormsAppB1;

public partial class Form1 : Form
{
    private string _folderPath = Environment.CurrentDirectory;
    private string _concatFile = "concatFiles.txt";
    private string _databaseName = "TaskB1";
    private bool _isGenerateFilesTaskActive = false;
    private bool _isConcatFilesTaskActive = false;
    private bool _isImportTaskActive = false;
    private bool _isCalculateCountRowsForAddTaskActive = false;
    private int _limit = 100_000;

    /// <summary>
    /// Constructor for the Form1 class.
    /// Initializes the form components, sets up initial settings,
    /// and hides certain labels on the form.
    /// </summary>
    public Form1()
    {
        // Initializes form components
        InitializeComponent();

        // Sets the count of rows for addition to the text box
        SetTextBoxCountRowsForAdd();

        // Hides specific labels initially
        labelStatus.Visible = false;
        labelConcatFiles.Visible = false;
        labelSumInt.Visible = false;
        labelAvgFloat.Visible = false;
    }

    /// <summary>
    /// Asynchronously sets the text in the textBoxCountRowsForAddInExcel control
    /// based on the count of lines in a file.
    /// </summary>
    private async void SetTextBoxCountRowsForAdd()
    {
        await Task.Run(() =>
        {
            _isCalculateCountRowsForAddTaskActive = true;
            string filePath = $"{_folderPath}\\{_concatFile}";
            if (File.Exists(filePath))
            {
                var lineCount = File.ReadLines(filePath).Count();
                textBoxCountRowsForAddInExcel.Invoke(new System.Action(() => textBoxCountRowsForAddInExcel.Text = lineCount.ToString()));
            }
        });
        _isCalculateCountRowsForAddTaskActive = false;
    }

    /// <summary>
    /// Handles the button click event to generate files asynchronously.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="e">The event arguments.</param>
    private async void buttonGenerateFiles_Click(object sender, EventArgs e)
    {
        await CreateFilesAsync();
    }

    /// <summary>
    /// Asynchronously initiates the process to create files.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task CreateFilesAsync()
    {
        var tasks = new List<Task>();
        tasks.Add(Task.Run(() => CreateFilesMultiThreaded()));

        // Add any other asynchronous tasks you need here

        try
        {
            _isGenerateFilesTaskActive = true;
            labelStatus.Text = "Files are generating...";
            labelStatus.Visible = true;
            await Task.WhenAll(tasks);
            _isGenerateFilesTaskActive = false;
            labelStatus.Text = "Files creation completed successfully!";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}");
            // Handle exception or log it accordingly
        }
    }

    /// <summary>
    /// Initiates the creation of files using multiple threads.
    /// </summary>
    private void CreateFilesMultiThreaded()
    {
        // get the number of processor cores
        var threadCount = Environment.ProcessorCount;

        // size of one chunk
        var chunkSize = (int)Math.Ceiling(100 / (double)threadCount);

        var finders = new FileСreator[threadCount];
        var start = 0;
        for (var i = 0; i < threadCount; i++)
        {
            finders[i] = new(start, Math.Min(start + chunkSize, 100));
            start += chunkSize;
        }

        // run threads
        foreach (var finder in finders)
        {
            finder.Start();
        }

        // wait until all threads are completed
        foreach (var finder in finders)
        {
            finder.Join();
        }

    }

    /// <summary>
    /// Creates an Excel file by adding data from a specified file, updating the UI elements during the process.
    /// </summary>
    private void CreateExcelFile()
    {
        int countRowsForAddInExcel;
        string filePath = $"{_folderPath}\\{_concatFile}";

        if (!File.Exists(filePath))
            return;

        if (_isCalculateCountRowsForAddTaskActive)
        {
            MessageBox.Show("Task for calculate count rows for add alredy running. Wait");
            return;
        }

        if (_isConcatFilesTaskActive)
        {
            MessageBox.Show("wait concat task");
            return;
        }
        if (!Int32.TryParse(textBoxCountRowsForAddInExcel.Text, out countRowsForAddInExcel))
            return;

        Action<int, int, int> updateUI = (currentCount, remainingCount, progressValue) =>
        {
            // Update your UI elements here
            labelCountOfAddedInExcelValue.Invoke(new System.Action(() => labelCountOfAddedInExcelValue.Text = currentCount.ToString()));
            labelRemainingRowsValue.Invoke(new System.Action(() => labelRemainingRowsValue.Text = remainingCount.ToString()));
            progressBarExcel.Invoke(new System.Action(() => progressBarExcel.Value = progressValue));
            labelLoader.Invoke(new System.Action(() => labelLoader.Text = progressValue.ToString()));

        };

        using (var _helper = new ExcelHelper())
        {
            _helper.AddDataInExcel(filePath, updateUI, countRowsForAddInExcel, _limit);
        }
    }

    /// <summary>
    /// Event handler for the button click to add data into an Excel file asynchronously.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="e">The event arguments.</param>
    private async void buttonAddInExcel_Click(object sender, EventArgs e)
    {
        await Task.Run(() => CreateExcelFile());
    }

    /// <summary>
    /// Event handler for importing data into SQL from an Excel file asynchronously.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="e">The event arguments.</param>
    private async void buttonImportDataInSQL_Click(object sender, EventArgs e)
    {
        _isImportTaskActive = true;
        string filePath = $"{_folderPath}\\{_concatFile}";
        int countRowsForAddInExcel;

        if (!File.Exists(filePath))
            return;

        if (_isCalculateCountRowsForAddTaskActive)
        {
            MessageBox.Show("Task for calculate count rows for add alredy running. Wait");
            return;
        }

        if (_isConcatFilesTaskActive)
        {
            MessageBox.Show("wait concat task");
            return;
        }

        if (!Int32.TryParse(textBoxCountRowsForAddInExcel.Text, out countRowsForAddInExcel))
            return;


        var lineCount = File.ReadLines(filePath).Count();
        DB.DBHelper dbHelper = new DB.DBHelper(_databaseName); // Предположим, что этот метод выполняет нужные проверки

        if (!dbHelper.IsDBExists())
            await dbHelper.CreateDBAsync();

        if (!dbHelper.IsDBTableExists("Data"))
            await dbHelper.CreateDBTableAsync("Data");

        List<string[]> partOfData = new List<string[]>();
        int limit = 10_000;
        if (lineCount > limit)
            partOfData.EnsureCapacity(limit);
        int i = 1;
        int countAddRows = 0;
        await Task.Run(() =>
        {

            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null && countAddRows < countRowsForAddInExcel)
                {
                    string[] data = line.Split("||");
                    partOfData.Add(data);
                    countAddRows++;
                    if (partOfData.Count == limit)
                    {
                        dbHelper.ImportData(partOfData, "Data");

                        partOfData.Clear();
                        this.Invoke(new System.Action(() =>
                            {

                                labelCountOfAddedInExcelValue.Text = (countAddRows).ToString();
                                labelRemainingRowsValue.Text = (countRowsForAddInExcel - countAddRows).ToString();
                                progressBarExcel.Value = (countAddRows * 100) / countRowsForAddInExcel;
                                labelLoader.Text = ((countAddRows * 100) / countRowsForAddInExcel).ToString();
                            }));
                        i++;
                    }


                }
                if (partOfData.Count > 0)
                {
                    dbHelper.ImportData(partOfData, "Data");
                    partOfData.Clear();
                    if (InvokeRequired)
                        this.Invoke(new System.Action(() =>
                        {
                            labelCountOfAddedInExcelValue.Text = (countAddRows).ToString();
                            labelRemainingRowsValue.Text = (countRowsForAddInExcel - countAddRows).ToString();
                            progressBarExcel.Value = (countAddRows * 100) / countRowsForAddInExcel;
                            labelLoader.Text = ((countAddRows * 100) / countRowsForAddInExcel).ToString();
                        }));
                }
            }
        });
        _isImportTaskActive = false;
    }

    /// <summary>
    /// Event handler for concatenating files asynchronously when the button is clicked.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="e">The event arguments.</param>
    private async void buttonConcatFiles_Click(object sender, EventArgs e)
    {
        if (_isCalculateCountRowsForAddTaskActive)
        {
            MessageBox.Show("Task for calculate count rows for add alredy running. Wait");
            return;
        }
        if (_isGenerateFilesTaskActive)
        {
            MessageBox.Show("Task for generate files alredy running. Wait");
            return;
        }
        string outputPath = $"{_folderPath}\\{_concatFile}";

        string[] fileEntries = Directory.GetFiles($"{_folderPath}\\initial", "*.txt");
        string searchText = textBoxFilter.Text; // Текст для удаления строк

        if (fileEntries.Length == 0)
            return;

        int deletedLinesCount = 0;

        await Task.Run(() =>
            {
                try
                {
                    labelConcatFiles.Invoke(new System.Action(() =>
                    {
                        labelConcatFiles.Text = $"Waiting...";
                        labelConcatFiles.Visible = true;
                        _isConcatFilesTaskActive = true;
                    }));


                    using (StreamWriter writer = File.CreateText(outputPath))
                    {
                        foreach (string fileName in fileEntries)
                        {
                            using (StreamReader reader = File.OpenText(fileName))
                            {
                                string line;
                                while ((line = reader.ReadLine()) != null)
                                {
                                    if (searchText == "" || !line.Contains(searchText))
                                    {
                                        writer.WriteLine(line);
                                    }
                                    else
                                    {
                                        deletedLinesCount++;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

            });
        labelConcatFiles.Text = $"Concatenated files: {fileEntries.Length}. Deleted lines: {deletedLinesCount}";
        _isConcatFilesTaskActive = false;
        SetTextBoxCountRowsForAdd();

    }

    /// <summary>
    /// Event handler for calculating the sum of integer values from a table in the database.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="e">The event arguments.</param>
    private void buttonCalculateSumOfInt_Click(object sender, EventArgs e)
    {
        DB.DBHelper dbHelper = new DB.DBHelper(_databaseName);

        if (_isImportTaskActive)
        {
            MessageBox.Show("wait import task");
            return;
        }

        if (!dbHelper.IsDBExists())
            MessageBox.Show("Import Data in SQL");

        if (!dbHelper.IsDBTableExists("Data"))
            MessageBox.Show("Import Data in SQL");

        try
        {
            BigInteger sumOfInt = dbHelper.CalculateSumOfInt("Data");
            labelSumInt.Text = sumOfInt.ToString();
            labelSumInt.Visible = true;
        }
        catch (SqlException ex)
        {
            MessageBox.Show("So big number. Arithmetic Overflow. Try to make less rows for import in SQL");
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }

    }

    /// <summary>
    /// Event handler for calculating the average of floating-point values from a table in the database.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="e">The event arguments.</param>
    private void buttonCalculateAvgOfFloat_Click(object sender, EventArgs e)
    {
        DB.DBHelper dbHelper = new DB.DBHelper(_databaseName);

        if (_isImportTaskActive)
        {
            MessageBox.Show("wait import task");
            return;
        }

        if (!dbHelper.IsDBExists())
            MessageBox.Show("Import Data in SQL");

        if (!dbHelper.IsDBTableExists("Data"))
            MessageBox.Show("Import Data in SQL");

        try
        {
            double avgOfFloat = dbHelper.CalculateAverageOfFloat("Data");
            labelAvgFloat.Text = avgOfFloat.ToString();
            labelAvgFloat.Visible = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }

    }
}
