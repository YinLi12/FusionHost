using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Windows;
using SinopecPumpSim.Properties;

namespace SinopecPumpSim
{
    /// <summary>
    /// Interaction logic for Setting.xaml
    /// </summary>
    public partial class Setting : Window
    {
        private readonly List<PumpSetting> _settings;

        public Setting(List<PumpSetting> settings)
        {
            _settings = settings;
            InitializeComponent();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            _settings.Clear();
            for (var i = 0; i < int.Parse(PumpNumber.Text); i ++)
            {
                var setting = new PumpSetting()
                {
                    Address = i,
                    PortName = Port.Text,
                    Parity = (Parity)Enum.Parse(typeof(Parity), Parity.Text, true),
                    Databits = int.Parse(Databits.Text),
                    StopBits = (StopBits)Enum.Parse(typeof(StopBits), Stopbit.Text, true),
                    Baudrate = int.Parse(Baudrate.Text)
                };

                _settings.Add(setting);
            }

            Close();
        }
    }

    
}
