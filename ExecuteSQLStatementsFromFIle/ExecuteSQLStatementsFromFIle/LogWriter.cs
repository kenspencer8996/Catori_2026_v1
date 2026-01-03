namespace ExecuteSQLStatementsFromFIle
{
    internal class LogWriter
        {
            private readonly StreamWriter? _file;

            public LogWriter(string? filePath)
            {
                if (!string.IsNullOrWhiteSpace(filePath))
                {
                    _file = new StreamWriter(File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
                    {
                        AutoFlush = true
                    };
                }
                Write("INFO", "Log started");
        }

            public void Info(string msg) => Write("INFO", msg);
            public void Ok(string msg) => Write("OK", msg);
            public void Fail(string msg) => Write("FAIL", msg);

            private void Write(string level, string msg)
            {
                string line = $"{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss.fff zzz} [{level}] {msg}";
                Console.WriteLine(line);
                _file?.WriteLine(line);
            }

            public void Dispose() => _file?.Dispose();
        }
    }

