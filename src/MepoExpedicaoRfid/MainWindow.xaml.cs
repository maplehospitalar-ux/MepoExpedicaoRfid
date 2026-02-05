using System.Windows;
using Serilog;

namespace MepoExpedicaoRfid
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Log.Information("MainWindow inicializado");
        }
    }
}

