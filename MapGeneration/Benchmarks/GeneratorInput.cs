namespace MapGeneration.Benchmarks
{
    public class GeneratorInput<TMapDescription>
    {
        public string Name { get; set; }

        public TMapDescription MapDescription { get; set; }

        public GeneratorInput(string name, TMapDescription mapDescription)
        {
            Name = name;
            MapDescription = mapDescription;
        }
    }
}