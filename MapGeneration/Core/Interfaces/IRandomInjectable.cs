namespace MapGeneration.Core.Interfaces
{
	using System;

	public interface IRandomInjectable
	{
		void InjectRandomGenerator(Random random);
	}
}