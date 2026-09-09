namespace CatoriServices.Objects.Core
{
    public class cLogger
    {
        public static string LogFilePath;
        private static string lastClassName = "";  
        public static void Log(string message, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)

        {
            string classname = System.IO.Path.GetFileNameWithoutExtension(sourceFilePath);
            if (classname != lastClassName)
            {
                lastClassName = classname;
                System.IO.File.AppendAllText(LogFilePath, "--------  " + classname + "  --------" + Environment.NewLine);
            }
            else
                classname = "";
            string outputMessage = $" {memberName} #: {sourceLineNumber}" + Environment.NewLine;
            outputMessage = $"|  {message}" + Environment.NewLine;
            System.IO.File.AppendAllText(LogFilePath, outputMessage);
        }
 
    }
}

