namespace WinFormsAppB1
{
    public class FileСreator
    {
        private readonly int _start;
        private readonly int _end;
        private readonly Thread _th;
        private readonly Random random = new();
        public bool Done { get; set; }

        /// <summary>
        /// Initializes a new instance of the FileСreator class with optional start and end parameters.
        /// </summary>
        /// <param name="start">The start value (optional). Default is 0.</param>
        /// <param name="end">The end value (optional). Default is 1.</param>
        public FileСreator(int start = 0, int end = 1)
        {
            _start = start;
            _end = end;
            _th = new Thread(CreateFile);
        }

        /// <summary>
        /// Starts the thread execution.
        /// </summary>
        public void Start() => _th.Start();

        /// <summary>
        /// Blocks the calling thread until the thread represented by this instance terminates.
        /// </summary>
        public void Join() => _th.Join();

        /// <summary>
        /// Creates files with random data within a specified range and saves them in the 'initial' directory.
        /// </summary>
        private void CreateFile()
        {
            string dir = Environment.CurrentDirectory;
            string directoryPath = $"{dir}\\initial";
            // If directory does not exist, create it
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            for (int i = _start; i < _end; i++)
            {
                string filePath = $"{directoryPath}\\file_{i}.txt";
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    for (int j = 0; j < 100000; j++)
                    {
                        DateTime startDate = DateTime.Now.AddYears(-5);
                        DateTime randomDate = startDate.AddDays(random.Next((DateTime.Today - startDate).Days));

                        string date = randomDate.ToString("dd.MM.yyyy");
                        string latinChars = GenerateRandomString("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz", 10);
                        string russianChars = GenerateRandomString("АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюя", 10);
                        int evenPositiveInteger = random.Next(1, 50000000) * 2;
                        double positiveDouble = random.NextDouble() * 19 + 1;

                        writer.WriteLine($"{date}||{latinChars}||{russianChars}||{evenPositiveInteger}||{positiveDouble:F8}||");
                    }
                }
            }
            Done = true;
        }

        /// <summary>
        /// Generates a random string of the specified length using the given set of characters.
        /// </summary>
        /// <param name="characters">The set of characters from which to generate the random string.</param>
        /// <param name="length">The length of the random string to generate.</param>
        /// <returns>A randomly generated string based on the provided characters and length.</returns>
        private string GenerateRandomString(string characters, int length)
        {
            char[] result = new char[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = characters[random.Next(characters.Length)];
            }
            return new string(result);
        }

    }
}

