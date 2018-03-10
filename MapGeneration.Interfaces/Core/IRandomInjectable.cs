namespace MapGeneration.Interfaces.Core
{
	using System;

	/// <summary>
	/// Represents a class that can be injected with a random numbers generator.
	/// </summary>
	/// <remarks>
	/// It is useful when debugging algorithms based on probabilties.
	/// </remarks>
	public interface IRandomInjectable
	{
		/// <summary>
		/// Injects an instance of a random numbers generator.
		/// </summary>
		/// <param name="random"></param>
		void InjectRandomGenerator(Random random);
	}
}