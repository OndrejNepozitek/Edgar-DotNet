namespace MapGeneration.Interfaces.Core
{
	using System;

	public interface IRandomInjectable
	{
		void InjectRandomGenerator(Random random);
	}
}