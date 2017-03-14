using System;
using System.Collections.Generic;
using System.IO.Ports;
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

namespace SinopecPumpSim
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Configurations _configurations;
        public List<Dispenser> Dispensers;

        public MainWindow()
        {
            _configurations = new Configurations();
            _configurations.Load();
            InitializeComponent();
        }

        private void MenuItemClick(object sender, RoutedEventArgs e)
        {
            var setting = new Setting(_configurations.PumpSettings);
            setting.Closed += Settings_Closed;
            setting.Show();
        }

        private void Settings_Closed(object sender, EventArgs e)
        {
           CreatePumps();
        }

        private void CreatePumps()
        {
            for (var i = 0; i < _configurations.PumpSettings.Length; i++)
            {
                var dispenser = new Dispenser();
                Grid.SetRow(dispenser, i+1);
                Grid.SetColumn(dispenser, 0);

                var row = new RowDefinition() { Height = new GridLength((i+1)*dispenser.Height) };
                MainGrid.RowDefinitions.Add(row);                
                MainGrid.Children.Add(dispenser);
            }
        }
    }
}
