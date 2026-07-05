using System;
using System.Windows;

namespace MultimeterDisplay
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Application initialization
            MainWindow window = new MainWindow();
            window.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Cleanup on exit
            base.OnExit(e);
        }
    }
}
