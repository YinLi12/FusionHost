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
using SinopecPumpSim.Config;

namespace SinopecPumpSim
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispenserMgr _dispenser = new DispenserMgr();
        public StationConfig StationConfig { get; set; }

        public MainWindow()
        {
            
            StationConfig = new StationConfig();

            InitializeComponent();
            DataContext = _dispenser;
        }

        private void MenuItemClick(object sender, RoutedEventArgs e)
        {
            var setting = new Setting(StationConfig);
            setting.Closed += Settings_Closed;
            setting.Show();
        }

        private void Settings_Closed(object sender, EventArgs e)
        {
           CreatePumps();
        }

        private void CreatePumps()
        {
            //for (var i = 0; i < _configurations.PumpSettings.Length; i++)
            //{
            //    var dispenser = new Dispenser();
            //    Grid.SetRow(dispenser, i+1);
            //    Grid.SetColumn(dispenser, 0);

            //    var row = new RowDefinition() { Height = new GridLength((i+1)*dispenser.Height) };
            //    MainGrid.RowDefinitions.Add(row);                
            //    MainGrid.Children.Add(dispenser);
            //}
        }
    }
}
