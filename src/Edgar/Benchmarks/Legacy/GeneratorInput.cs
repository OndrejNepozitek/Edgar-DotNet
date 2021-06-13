﻿namespace Edgar.Benchmarks.Legacy
{
    /// <summary>
    /// Input for the generator.
    /// </summary>
    /// <typeparam name="TMapDescription"></typeparam>
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