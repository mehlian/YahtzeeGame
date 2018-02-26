using System.Windows;

namespace Yahtzee.ViewModels
{
    public interface IGameWindow
    {
        object DataContext { get; set; }
        bool? DialogResult { get; set; }
        Window Owner { get; set; }
        void Close();
        bool? ShowDialog();
    }
}
