using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SinopecPumpSim.Config;
using SinopecPumpSim.Properties;

namespace SinopecPumpSim
{
    /// <summary>
    /// Interaction logic for Setting.xaml
    /// </summary>
    public partial class Setting : Window
    {
        //private readonly List<PumpSetting> _settings;
        private readonly StationConfig _stationConfig;

        public Setting(StationConfig stationConfig)
        {           
            InitializeComponent();
            _stationConfig = stationConfig;
            TabControl.DataContext = _stationConfig;
            StationInfoTab.DataContext = _stationConfig.StationInfo;
        }

        private void AddPumpSettings()
        {
            for (var i = 0; i < _stationConfig.PumpSettings.PumpSetting.Length; i++)
            {
                var tab = new TabItem();
                //TabControl.Items.a
            }
        }

        //public Setting(List<PumpSetting> settings)
        //{
        //    _settings = settings;
        //    InitializeComponent();
        //}

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            //    _settings.Clear();
            //    for (var i = 0; i < int.Parse(PumpNumber.Text); i ++)
            //    {
            //        var setting = new PumpSetting()
            //        {
            //            Address = i,
            //            PortName = Port.Text,
            //            Parity = (Parity)Enum.Parse(typeof(Parity), Parity.Text, true),
            //            Databits = int.Parse(Databits.Text),
            //            StopBits = (StopBits)Enum.Parse(typeof(StopBits), Stopbit.Text, true),
            //            Baudrate = int.Parse(Baudrate.Text)
            //        };

            //        _settings.Add(setting);
            //    }

            //    Close();
        }

        private void OK_Click_1(object sender, RoutedEventArgs e)
        {
            _stationConfig.UpdatePumpSetting();
        }
    }

    
}
