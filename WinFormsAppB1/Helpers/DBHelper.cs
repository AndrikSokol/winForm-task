using System.Data;
using System.Data.SqlClient;

namespace WinFormsAppB1.DB;

public class DBHelper
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; }

    private Task _initializationTask;
    public Task InitializationTask => _initializationTask;

    /// <summary>
    /// Constructor for create instance of DBHelper for local MSSQL.
    /// If database wasnt exists will create database.
    /// "Data Source=(LocalDB)\MSSQLLocalDB;Integrated Security=True;".
    /// </summary>
    /// <param name="databaseName">This database name will be used in current instance.</param>
    public DBHelper(string databaseName)
    {
        DatabaseName = databaseName;
        _initializationTask = InitializeAsync();
    }

    /// <summary>
    /// Check valid connection.
    /// Check Database.
    /// Create if doesn't  find.
    /// </summary>
    /// <returns>async Task</returns>
    private async Task InitializeAsync()
    {
        try
        {
            ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Integrated Security=True;";

            if (IsServerConnected())
            {
                Console.WriteLine("Connection successful");
                if (!IsDatabaseExists())
                {
                    Console.WriteLine($"DB '{DatabaseName}' doesn't exist. Trying to create db");
                    await CreateDBAsync();
                    Console.WriteLine($"DB '{DatabaseName}' is created.");
                }
                ConnectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;Integrated Security=True;Initial Catalog={DatabaseName};";
            }
            else
            {
                Console.WriteLine($"Can't access the connection with the server {DatabaseName}");
                throw new Exception($"Can't establish connection to {DatabaseName}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred during initialization: {ex.Message}");
            throw; // Rethrow the exception to signal initialization failure
        }
    }

    /// <summary>
    /// Check connection to local MSSQL.
    /// </summary>
    /// <returns>boolean variable that has successful or failure access.</returns>
    private bool IsServerConnected()
    {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (SqlException)
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Check for exists database.
    /// </summary>
    /// <returns>boolean variable that has successful or failure result.</returns>
    private bool IsDatabaseExists()
    {
        string query = $"SELECT database_id FROM sys.databases WHERE Name = '{DatabaseName}'";
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            connection.Open();
            SqlCommand command = new SqlCommand(query, connection);

            object result = command.ExecuteScalar();
            return (result != null);
        }
    }

    /// <summary>
    /// Creates a new database asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task CreateDBAsync()
    {
        string query = $"CREATE DATABASE {DatabaseName}";
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }
    }

    /// <summary>
    /// Checks if a table exists in the database.
    /// </summary>
    /// <param name="tableName">The name of the table to check.</param>
    /// <returns>True if the table exists; otherwise, false.</returns>
    public bool IsDBTableExists(string tableName)
    {
        string query = $"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{tableName}' AND TABLE_SCHEMA = 'dbo'";
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            int count = Convert.ToInt32(command.ExecuteScalar());
            return (count > 0);
        }
    }

    /// <summary>
    /// Create a table in the database.
    /// </summary>
    /// <param name="queryForCreateTable">SQL query.</param>
    /// <returns>Async task</returns>
    public async Task CreateDBTableAsync(string queryForCreateTable)
    {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            SqlCommand command = new SqlCommand(queryForCreateTable, connection);
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }
    }

    /// <summary>
    /// Clears all data from the specified database table.
    /// </summary>
    /// <param name="tableName">The name of the table to clear.</param>
    public void ClearDBTable(string tableName)
    {
        string query = $"DELETE FROM {tableName}";

        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            command.ExecuteNonQuery();
        }
    }

    /// <summary>
    /// Send data to SQL.
    /// </summary>
    /// <param name="query">SQL query.</param>
    /// <returns>async Task</returns>
    public async Task SendDataAsync(string query)
    {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            command.CommandTimeout = 0;
            await connection.OpenAsync();
            await command.ExecuteReaderAsync();
        }
    }

    /// <summary>
    /// Get first common id from query.
    /// </summary>
    /// <param name="query">SQL query.</param>
    /// <returns>id or default 0</returns>
    public async Task<int> GetIdAsync(string query)
    {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            command.CommandTimeout = 0;
            await connection.OpenAsync();
            using (SqlDataReader reader = await command.ExecuteReaderAsync())
            {
                if (reader.Read())
                {
                    int id = reader.GetInt32(0); // Assuming the ID is in the first column (index 0)
                    return id;
                }
            }
        }
        return 0;
    }

    /// <summary>
    /// Gets Columns from table.
    /// </summary>
    /// <param name="query">sql query</param>
    /// <returns>List with names from table</returns>
    public async Task<List<string>> GetColumnAsync(string query, string column)
    {
        List<string> data = new();
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            command.CommandTimeout = 0;
            await connection.OpenAsync();
            using (SqlDataReader dataReader = command.ExecuteReader())
            {
                while (dataReader.Read())
                {
                    data.Add(dataReader[column].ToString());
                }
            }
        }
        return data;
    }

    /// <summary>
    /// Get a table.
    /// </summary>
    /// <param name="query">SQL query.</param>
    /// <returns>Async Task with instance of Dataset</returns>
    public async Task<DataSet> GetTableAsync(string query)
    {
        DataSet dataSet = new();
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            command.CommandTimeout = 0;
            await connection.OpenAsync();
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
            sqlDataAdapter.Fill(dataSet);
        }
        return dataSet;
    }

    /// <summary>
    /// Retrieves the count of rows imported into the database.
    /// </summary>
    /// <returns>The count of rows that have been imported into the database.</returns>
    public int GetImportedRowCount()
    {
        int importedRows = 0;
        string queryForGetCountOfImportedRows = $"SELECT COUNT(*) AS 'ImportedRows' FROM {DatabaseName}";

        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            SqlCommand command = new SqlCommand(queryForGetCountOfImportedRows, connection);
            connection.Open();
            object result = command.ExecuteScalar();

            if (result != null && result != DBNull.Value)
            {
                importedRows = Convert.ToInt32(result);
            }
        }

        return importedRows;
    }

    /// <summary>
    /// Calculates the sum of integer values from a specified table in the database.
    /// </summary>
    /// <param name="table">The name of the table containing integer values for summation.</param>
    /// <returns>The sum of integer values from the specified table.</returns>
    public int CalculateSumOfInt(string table)
    {
        int sumInt = 0;
        string queryForGetCountOfImportedRows = $"SELECT SUM(IntNumber) AS SumOfInt FROM {table}";

        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            SqlCommand command = new SqlCommand(queryForGetCountOfImportedRows, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                sumInt = reader.GetFieldValue<Int32>(0);
            }
            reader.Close();
        }

        return sumInt;
    }

    /// <summary>
    /// Calculates the average of floating-point values from a specified table in the database.
    /// </summary>
    /// <param name="table">The name of the table containing floating-point values for averaging.</param>
    /// <returns>The average of floating-point values from the specified table.</returns>
    public double CalculateAverageOfFloat(string table)
    {
        double averageFloat = 0f;
        string queryForGetCountOfImportedRows = $"SELECT AVG(FloatNumber) AS AvgOfFloat FROM {table}";

        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            SqlCommand command = new SqlCommand(queryForGetCountOfImportedRows, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                averageFloat = Convert.ToSingle(reader.GetDouble(0));
            }
            reader.Close();
        }
        return averageFloat;
    }

}

