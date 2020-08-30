namespace Edgar.Examples
{
    public class ExampleOptions
    {
        public string Name { get; set; }

        public string DocsFileName { get; set; }

        public string EntryPointMethod { get; set; }

        public bool IncludeResults { get; set; } = true;

        public bool IncludeSourceCode { get; set; } = true;
    }
}