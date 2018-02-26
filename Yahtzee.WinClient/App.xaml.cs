using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Yahtzee.ViewModels;

namespace Yahtzee.WinClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            IGameWindowService gameWindowService = new GameWindowService(MainWindow);
            gameWindowService.Register<GameWindowViewModel, GameWindow>();

            var viewModel = new MainWindowViewModel(gameWindowService);
            var view = new MainWindow { DataContext = viewModel };
            view.ShowDialog();
        }
    }
}
