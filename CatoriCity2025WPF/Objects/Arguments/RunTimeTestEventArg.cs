namespace CatoriCity2025WPF.Objects.Arguments
{
    public class RunTimeTestEventArg
    {
        public string TestName { get; set; }
        public string TestData { get; set; }
        public RunTimeTestEventArg(string testName, string testData)
        {
            TestName = testName;
            TestData = testData;
        }
    }
}
