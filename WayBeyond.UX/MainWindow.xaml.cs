using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WayBeyond.UX.Services;

namespace WayBeyond.UX
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File($"{ConfigurationManager.AppSettings["LogFile"]}BeyondLog-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
            InitializeComponent();

            MainWindowViewModel.Exit += OnExit;

            ConfigurationEncryptionService.EncryptConfiguration();
        }

        private async void OnExit()
        {
            this.Close();
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            Log.CloseAndFlushAsync();
            base.OnClosing(e);
            
        }
    }
}
