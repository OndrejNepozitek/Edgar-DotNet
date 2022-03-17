using System;

namespace Edgar.Legacy.Core.LayoutGenerators.Interfaces
{
    /// <inheritdoc />
    /// <summary>
    /// Layout generators that can be observed for example from GUI.
    /// </summary>
    public interface IObservableGenerator<out TLayout> : ILayoutGenerator<TLayout>
    {
        /// <summary>
        /// Invoked whenever a layout is perturbed.
        /// </summary>
        event Action<TLayout> OnPerturbed;

        /// <summary>
        /// Invoked whenever a partial valid layout is found.
        /// </summary>
        event Action<TLayout> OnPartialValid;

        /// <summary>
        /// Invoked whenever a valid layout is found.
        /// </summary>
        event Action<TLayout> OnValid;
    }
}