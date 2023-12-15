using System.Data;
using System.Data.SqlClient;
using System.Text;
using WinFormsAppB1.DB;
using WinFormsAppB1.Helpers;

namespace WinFormsAppB1;

public partial class Form1 : Form
{
    private string _folderPath = Environment.CurrentDirectory;
    private string _concatFile = "concatFiles.txt";
    private string _databaseNameTask1 = "TaskB1";
    private string _databaseNameTask2 = "Bank";
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

        GetFilesName();

        // Hides specific labels initially
        labelStatus.Visible = false;
        labelConcatFiles.Visible = false;
        labelSumInt.Visible = false;
        labelAvgFloat.Visible = false;
        labelOSVLoad.Visible = false;
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
        {
            MessageBox.Show("Cant find concatFile");
            return;
        }

        if (_isCalculateCountRowsForAddTaskActive)
        {
            MessageBox.Show("Task for calculate count rows for add already running. Wait");
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
    /// Create SQL query based on part of data.
    /// </summary>
    /// <param name="partOfData">part of data.</param>
    /// <returns>SQL query.</returns>
    private string CreateSQLQueryForPartData(List<string[]> partOfData)
    {
        StringBuilder queryBuilder = new StringBuilder();

        foreach (var row in partOfData)
        {
            string[] dateStr = row[0].Split(".");
            string formattedDate = $"{dateStr[2]}-{dateStr[1]}-{dateStr[0]}";

            queryBuilder.Append($"INSERT INTO [dbo].RandomData VALUES ('{formattedDate}','{row[1]}',N'{row[2]}',{row[3]},{row[4].Replace(',', '.')})");
        }
        return queryBuilder.ToString();
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
        int countRowsForAddInSQL;

        if (!File.Exists(filePath))
        {
            MessageBox.Show("Cand find concat file");
            return;
        }

        if (_isCalculateCountRowsForAddTaskActive)
        {
            MessageBox.Show("Task for calculate count rows for add already running. Wait");
            return;
        }

        if (_isConcatFilesTaskActive)
        {
            MessageBox.Show("wait concat task");
            return;
        }

        if (!Int32.TryParse(textBoxCountRowsForAddInExcel.Text, out countRowsForAddInSQL))
            return;

        try
        {
            var lineCount = File.ReadLines(filePath).Count();
            DB.DBHelper dbHelper = new DB.DBHelper(_databaseNameTask1);
            await dbHelper.InitializationTask;

            if (!dbHelper.IsDBTableExists("RandomData"))
            {
                string queryForCreateTable = $"CREATE TABLE [dbo].RandomData (" +
                $"[Date] Date NOT NULL," +
                $"[LatinSymbols] NVARCHAR(10) NOT NULL, " +
                $"[RussianSymbols] NVARCHAR(10) NOT NULL, " +
                $"[IntNumber] INT NOT NULL," +
                $"[FloatNumber] FLOAT NOT NULL)";

                await dbHelper.CreateDBTableAsync(queryForCreateTable);
            }
            else
            {
                dbHelper.ClearDBTable("RandomData");
            }

            Action<int, int, int> updateUI = (currentCount, remainingCount, progressValue) =>
            {
                // Update your UI elements here
                labelCountOfAddedInExcelValue.Invoke(new System.Action(() => labelCountOfAddedInExcelValue.Text = currentCount.ToString()));
                labelRemainingRowsValue.Invoke(new System.Action(() => labelRemainingRowsValue.Text = remainingCount.ToString()));
                progressBarExcel.Invoke(new System.Action(() => progressBarExcel.Value = progressValue));
                labelLoader.Invoke(new System.Action(() => labelLoader.Text = progressValue.ToString()));
            };

            List<string[]> partOfData = new List<string[]>();
            int limit = 10_000;
            if (lineCount > limit)
                partOfData.EnsureCapacity(limit);

            int countAddRows = 0;
            await Task.Run(async () =>
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null && countAddRows < countRowsForAddInSQL)
                    {
                        string[] data = line.Split("||");
                        partOfData.Add(data);
                        countAddRows++;
                        if (partOfData.Count == limit)
                        {
                            await dbHelper.SendDataAsync(CreateSQLQueryForPartData(partOfData));

                            partOfData.Clear();

                            updateUI(countAddRows, countRowsForAddInSQL - countAddRows, (countAddRows * 100) / countRowsForAddInSQL);
                        }
                    }

                    //Add last data that didn't achieve limit
                    if (partOfData.Count > 0)
                    {
                        await dbHelper.SendDataAsync(CreateSQLQueryForPartData(partOfData));

                        partOfData.Clear();

                        updateUI(countAddRows, countRowsForAddInSQL - countAddRows, (countAddRows * 100) / countRowsForAddInSQL);
                    }
                }
            });
            _isImportTaskActive = false;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
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
            MessageBox.Show("Task for calculate count rows for add already running. Wait");
            return;
        }
        if (_isGenerateFilesTaskActive)
        {
            MessageBox.Show("Task for generate files already running. Wait");
            return;
        }
        string outputPath = $"{_folderPath}\\{_concatFile}";

        string[] fileEntries = Directory.GetFiles($"{_folderPath}\\initial", "*.txt");
        string searchText = textBoxFilter.Text; // Текст для удаления строк

        if (fileEntries.Length == 0)
        {
            MessageBox.Show("Nothing to concat");
            return;
        }

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
    private async void buttonCalculateSumOfInt_Click(object sender, EventArgs e)
    {
        try
        {
            DB.DBHelper dbHelper = new DB.DBHelper(_databaseNameTask1);
            await dbHelper.InitializationTask;
            if (_isImportTaskActive)
            {
                MessageBox.Show("wait import task");
                return;
            }

            if (!dbHelper.IsDBTableExists("RandomData"))
                MessageBox.Show("Import Data in SQL");

            int sumOfInt = dbHelper.CalculateSumOfInt("RandomData");
            labelSumInt.Text = sumOfInt.ToString();
            labelSumInt.Visible = true;

        }
        catch (SqlException SqlEx)
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
    private async void buttonCalculateAvgOfFloat_Click(object sender, EventArgs e)
    {
        try
        {
            DB.DBHelper dbHelper = new DB.DBHelper(_databaseNameTask1);
            await dbHelper.InitializationTask;
            if (_isImportTaskActive)
            {
                MessageBox.Show("wait import task");
                return;
            }

            if (!dbHelper.IsDBTableExists("RandomData"))
            {
                MessageBox.Show("Data table doesnt exist. Import Data in SQL");
                return;
            }

            double avgOfFloat = dbHelper.CalculateAverageOfFloat("RandomData");
            labelAvgFloat.Text = avgOfFloat.ToString();
            labelAvgFloat.Visible = true;

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }

    }

    /// <summary>
    /// Get files name from database table.
    /// </summary>
    private async void GetFilesName()
    {
        try
        {
            DB.DBHelper dbHelper = new DB.DBHelper(_databaseNameTask2);
            await dbHelper.InitializationTask;

            if (!dbHelper.IsDBTableExists("FileExcel"))
                return;

            string query1 = "SELECT Name from [dbo].FileExcel";
            List<string> existingFilesPath = await dbHelper.GetColumnAsync(query1, "Name");

            for (int i = 0; i < existingFilesPath.Count; i++)
            {
                dataGridViewFilesName[i, 0].Value = existingFilesPath[i];
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
    }

    /// <summary>
    /// Event handler for the click event of the 'Load OSV' button. Loads and processes data from an Excel file into the database.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="e">The event arguments.</param>
    private async void buttonLoadOSV_Click(object sender, EventArgs e)
    {
        List<string> files = new List<string>();
        string fileName = "";
        string filePath = "";
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "xls file (*.xls)|*.xls";

        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            filePath = openFileDialog.FileName;
            fileName = openFileDialog.SafeFileName;
            if (files.Contains(fileName))
            {
                labelOSVLoad.Text = "You cant add the same file";
                labelOSVLoad.Visible = true;
                return;
            }

        }
        else
            return;

        labelOSVLoad.Text = "Waiting...";
        labelOSVLoad.Visible = true;

        using (var _excel = new ExcelHelper())
        {

            try
            {
                StringBuilder stringBuilder = new();

                DB.DBHelper dbHelper = new DB.DBHelper(_databaseNameTask2);
                await dbHelper.InitializationTask;

                string queryToGetNamesOfFile = "SELECT Name from [dbo].FileExcel";
                List<string> existingFilesName = await dbHelper.GetColumnAsync(queryToGetNamesOfFile, "Name");

                foreach (var path in existingFilesName)
                {
                    files.Add(path);
                }

                if (existingFilesName.Contains(fileName))
                {
                    labelOSVLoad.Text = "File is exists";
                    return;
                }

                var bankClasses = await _excel.ReadOSV(filePath);

                if (!dbHelper.IsDBTableExists("Class"))
                {
                    string queryForCreateTable = $"CREATE TABLE Class (" +
                    $"[Id] INT NOT NULL PRIMARY KEY IDENTITY," +
                    $"[Name] NVARCHAR(Max) NOT NULL)";

                    await dbHelper.CreateDBTableAsync(queryForCreateTable);
                }

                if (!dbHelper.IsDBTableExists("FileExcel"))
                {
                    string queryForCreateTable = $"CREATE TABLE [dbo].FileExcel (" +
                    $"[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1)," +
                    $"[Name] NVARCHAR(Max) NOT NULL)";

                    await dbHelper.CreateDBTableAsync(queryForCreateTable);
                }

                if (!dbHelper.IsDBTableExists("Bill"))
                {
                    string queryForCreateTable = $"CREATE TABLE Bill (" +
                    $"[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1)," +
                    $"[BillId] INT NOT NULL," +
                    $"[ClassId] INT NOT NULL," +
                    $"[FileId] INT NOT NULL," +
                    $"[OpeningBalanceActive] Float NOT NULL," +
                    $"[OpeningBalancePassive] Float NOT NULL," +
                    $"[Debet] Float NOT NULL," +
                    $"[Credit] Float NOT NULL," +
                    $"[CloseBalanceActive] Float NOT NULL," +
                    $"[CloseBalancePassive] Float NOT NULL," +
                    $"CONSTRAINT FK_Bill_To_Class FOREIGN KEY (ClassId)  REFERENCES Class (Id)," +
                    $"CONSTRAINT FK_Bill_To_File FOREIGN KEY (FileId)  REFERENCES FileExcel (Id))";

                    await dbHelper.CreateDBTableAsync(queryForCreateTable);
                }

                string queryToGetFromClassTableName = "SELECT Name from [dbo].Class";
                List<string> existingClassNames = await dbHelper.GetColumnAsync(queryToGetFromClassTableName, "Name");

                foreach (var item in bankClasses)
                {
                    if (!existingClassNames.Contains(item.Key.Item2))
                        stringBuilder.Append($"INSERT INTO [dbo].Class (Name) VALUES (N'{item.Key.Item2}')");
                }

                if (stringBuilder.Length != 0)
                {
                    await dbHelper.SendDataAsync(stringBuilder.ToString());
                }

                stringBuilder.Clear();

                string queryToInsertFileName = $"INSERT INTO [dbo].FileExcel  VALUES (N'{fileName}')";
                await dbHelper.SendDataAsync(queryToInsertFileName);

                string query3 = $"Select Id from FileExcel WHERE Name = N'{fileName}'";
                int idFile = await dbHelper.GetIdAsync(query3);

                for (int i = 0; i < files.Count; i++)
                {
                    dataGridViewFilesName[i, 0].Value = files[i];
                }

                if (stringBuilder.Length != 0)
                {
                    await dbHelper.SendDataAsync(stringBuilder.ToString());
                }

                stringBuilder.Clear();

                foreach (var bankClass in bankClasses)
                {
                    var bankItems = bankClass.Value;
                    foreach (var item in bankItems)
                    {
                        stringBuilder.Append($"INSERT INTO [dbo].Bill (BillId,ClassId,FileId,OpeningBalanceActive," +
                            $"OpeningBalancePassive,Debet,Credit,CloseBalanceActive,CloseBalancePassive) VALUES ({item[0]},{bankClass.Key.Item1}," +
                            $"{idFile}," +
                            $"{item[1].ToString().Replace(",", ".")}," +
                            $"{item[2].ToString().Replace(",", ".")}," +
                            $"{item[3].ToString().Replace(",", ".")}," +
                            $"{item[4].ToString().Replace(",", ".")}," +
                            $"{item[5].ToString().Replace(",", ".")}," +
                            $"{item[6].ToString().Replace(",", ".")})");
                    }
                }

                if (stringBuilder.Length != 0)
                {
                    await dbHelper.SendDataAsync(stringBuilder.ToString());
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        files.Add(fileName);

        for (int i = 0; i < files.Count; i++)
        {
            dataGridViewFilesName[i, 0].Value = files[i];
        }

        labelOSVLoad.Text = "Successful";
    }

    /// <summary>
    /// Handles the cell mouse click event in the DataGridView for file names.
    /// Retrieves and displays data in another DataGridView based on the file selected.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="e">The event arguments containing information about the mouse click.</param>
    private async void dataGridViewFilesName_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
    {
        if (e.RowIndex >= 0 && e.ColumnIndex >= 0) // Check if valid row and column indices
        {
            if (dataGridViewFilesName.Columns[e.ColumnIndex].Name == "ColumnFileName") // Check if the clicked column is "filePath"
            {
                try
                {
                    string? fileName = dataGridViewFilesName[e.ColumnIndex, e.RowIndex].Value?.ToString();

                    DB.DBHelper dbHelper = new DBHelper(_databaseNameTask2);
                    await dbHelper.InitializationTask;

                    if (!dbHelper.IsDBTableExists("FileExcel"))
                        return;

                    string query1 = $"Select Id from FileExcel WHERE Name = N'{fileName}'";

                    int idFile = await dbHelper.GetIdAsync(query1);

                    string query2 = $"SELECT * From Bill WHERE FileId = {idFile}";

                    DataSet dataSet = await dbHelper.GetTableAsync(query2);
                    DataTable table = dataSet.Tables[0];

                    table.Columns.Remove("ClassId");
                    table.Columns.Remove("FileId");
                    table.Columns.Remove("Id");

                    dataGridViewOSV.DataSource = table;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
    }
}
