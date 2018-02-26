using System;

namespace Yahtzee.ViewModels
{
    public interface IGameWindowRequestClose
    {
        event EventHandler<GameWindowCloseRequestedEventArgs> CloseRequested;
    }
}