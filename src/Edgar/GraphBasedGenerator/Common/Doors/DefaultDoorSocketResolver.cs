using System;

namespace Edgar.GraphBasedGenerator.Common.Doors
{
    /// <summary>
    /// The default implementation of the IDoorSocketResolver interface.
    /// </summary>
    /// <remarks>
    /// This resolver works as follows:
    /// - two null sockets are always compatible
    /// - if one socket is null, the other socket decides whether they are compatible
    /// - if no socket is null
    ///     - if both sockets return the same value: that value is used
    /// </remarks>
    public class DefaultDoorSocketResolver : IDoorSocketResolver
    {
        /// <inheritdoc />
        public bool AreCompatible(IDoorSocket socket1, IDoorSocket socket2)
        {
            if (socket1 == null && socket2 == null)
            {
                return true;
            }

            if (socket1 == null)
            {
                return socket2.IsCompatibleWith(null);
            }

            if (socket2 == null)
            {
                return socket1.IsCompatibleWith(null);
            }

            var direction1 = socket1.IsCompatibleWith(socket2);
            var direction2 = socket2.IsCompatibleWith(socket1);

            if (direction1 != direction2)
            {
                throw new InvalidOperationException(
                    $"Sockets {socket1} and {socket2} did not return the same results when asked whether they are compatible.");
            }

            return direction1;
        }
    }
}