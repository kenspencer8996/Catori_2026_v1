namespace CatoriServices.Objects
{
    public class cLogger
    {
        public static string LogFilePath;
        public static void Log(string message, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)

        {
            string classname = System.IO.Path.GetFileNameWithoutExtension(sourceFilePath);
            string outputMessage = $"{classname}: {message} | MemberName: {memberName} | SourceLineNumber: {sourceLineNumber}" + Environment.NewLine;
            System.IO.File.AppendAllText(LogFilePath, outputMessage);
        }
 
    }
}
