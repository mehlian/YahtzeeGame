namespace Yahtzee.ViewModels
{
    public interface IGameWindowService
    {
        void Register<TViewModel, TView>() where TViewModel : IGameWindowRequestClose
                                           where TView : IGameWindow;

        bool? ShowDialog<TViewModel>(TViewModel viewModel) where TViewModel : IGameWindowRequestClose;
    }
}
