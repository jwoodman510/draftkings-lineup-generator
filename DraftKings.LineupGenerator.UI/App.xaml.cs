using Microsoft.Maui.Controls;

namespace DraftKings.LineupGenerator.UI
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}