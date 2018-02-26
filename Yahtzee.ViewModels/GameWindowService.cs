using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Yahtzee.ViewModels
{
    public class GameWindowService : IGameWindowService
    {
        private readonly Window _owner;
        public GameWindowService(Window owner)
        {
            _owner = owner;
            Mappings = new Dictionary<Type, Type>();
        }
        public IDictionary<Type, Type> Mappings { get; }

        public void Register<TViewModel, TView>() where TViewModel : IGameWindowRequestClose
                                                  where TView : IGameWindow
        {
            if (Mappings.ContainsKey(typeof(TViewModel)))
            {
                throw new ArgumentException($"Type {typeof(TViewModel)} is already mapped to type {typeof(TView)}.");
            }

            Mappings.Add(typeof(TViewModel), typeof(TView));
        }

        public bool? ShowDialog<TViewModel>(TViewModel viewModel) where TViewModel : IGameWindowRequestClose
        {
            Type viewType = Mappings[typeof(TViewModel)];
            IGameWindow gameWindow = (IGameWindow)Activator.CreateInstance(viewType);

            EventHandler<GameWindowCloseRequestedEventArgs> handler = null;

            handler = (sender, e) =>
            {
                viewModel.CloseRequested -= handler;

                if (e.DialogResult.HasValue)
                {
                    gameWindow.DialogResult = e.DialogResult;
                }
                else
                {
                    gameWindow.Close();
                }
            };

            viewModel.CloseRequested += handler;

            gameWindow.DataContext = viewModel;
            gameWindow.Owner = _owner;

            return gameWindow.ShowDialog();
        }
    }
}
