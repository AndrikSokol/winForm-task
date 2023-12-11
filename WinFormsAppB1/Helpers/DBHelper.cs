using System.Data.SqlClient;
using System.Numerics;
using System.Text;

namespace WinFormsAppB1.DB;

public class DBHelper
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; }

    /// <summary>
    /// Constructor for create instance of DBHelper for local MSSQL.Default connection 
    /// "Data Source=(LocalDB)\MSSQLLocalDB;Integrated Security=True;".
    /// </summary>
    /// <param name="databaseName">This database name will be used in current instance.</param>
    public DBHelper(string databaseName)
    {
        ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Integrated Security=True;";
        DatabaseName = databaseName;
    }

    /// <summary>
    /// Check connection and is exists database.
    /// </summary>
    /// <returns>A bool that has successful or failure access.</returns>
    public bool IsDBExists()
    {

        if (IsServerConnected())
        {
            Console.WriteLine("Connection successfull");

            if (!IsDatabaseExists())
            {
                Console.WriteLine($"DB'{DatabaseName}' isnt exist.");
                return false;
            }
            else
            {
                ConnectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;Integrated Security=True;Initial Catalog={DatabaseName};";
                Console.WriteLine($"DB '{DatabaseName}' is exist.");
                return true;
            }
        }
        else
        {
            Console.WriteLine("cannot access connection with server");
        }
        return false;
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
        ConnectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;Integrated Security=True;Initial Catalog={DatabaseName};";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tableName"></param>
    /// <returns></returns>
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
    /// Checks if a table exists in the database.
    /// </summary>
    /// <param name="tableName">The name of the table to check.</param>
    /// <returns>True if the table exists; otherwise, false.</returns>
    public async Task CreateDBTableAsync(string tableName)
    {
        string queryForCreateTable = $"CREATE TABLE {tableName} (" +
            $"[Date] Date NOT NULL," +
            $"[LatinSymbols] NVARCHAR(10) NOT NULL, " +
            $"[RussianSymbols] NVARCHAR(10) NOT NULL, " +
            $"[IntNumber] INT NOT NULL," +
            $"[FloatNumber] FLOAT NOT NULL)";

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
    /// Imports data into the specified database table.
    /// </summary>
    /// <param name="dataToImport">The data to be imported as a list of string arrays, where each array represents a row of data.</param>
    /// <param name="tableName">The name of the table where the data will be imported.</param>
    public void ImportData(List<string[]> dataToImport, string tableName)
    {
        ClearDBTable(tableName);
        // Assuming dataToImport is a list of string arrays where each array represents a row of data

        StringBuilder queryBuilder = new StringBuilder();
        foreach (var data in dataToImport)
        {
            string[] dateStr = data[0].Split(".");
            string formattedDate = $"{dateStr[2]}-{dateStr[1]}-{dateStr[0]}";

            queryBuilder.Append($"INSERT INTO {tableName} VALUES ('{formattedDate}','{data[1]}','{data[2]}',{data[3]},{data[4].Replace(',', '.')})");
        }

        string query = queryBuilder.ToString();

        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            command.CommandTimeout = 0;
            connection.Open();
            command.ExecuteNonQuery();
        }
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

