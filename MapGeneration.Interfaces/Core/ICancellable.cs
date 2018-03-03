namespace MapGeneration.Interfaces.Core
{
	using System.Threading;

	public interface ICancellable
	{
		void SetCancellationToken(CancellationToken? cancellationToken);
	}
}