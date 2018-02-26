using System;

namespace Yahtzee.ViewModels
{
    public class GameWindowCloseRequestedEventArgs : EventArgs
    {
        public GameWindowCloseRequestedEventArgs(bool? dialogResult)
        {
            DialogResult = dialogResult;
        }
        public bool? DialogResult { get; }
    }
}
