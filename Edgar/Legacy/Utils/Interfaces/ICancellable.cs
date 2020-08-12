using System.Threading;

namespace Edgar.Legacy.Utils.Interfaces
{
    /// <summary>
	/// Represents types that can be cancelled by a cancellation token.
	/// </summary>
	public interface ICancellable
	{
		/// <summary>
		/// Sets a cancellation token.
		/// </summary>
		/// <param name="cancellationToken"></param>
		void SetCancellationToken(CancellationToken? cancellationToken);
	}
}