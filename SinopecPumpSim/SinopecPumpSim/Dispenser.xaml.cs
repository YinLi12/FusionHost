using System;
using System.Collections.Generic;
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
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class Dispenser : UserControl
    {
        private readonly Queue<string> _keyBuffer;
        private PumpSetting _pumpSetting;

        public Dispenser()
        { 
            _keyBuffer = new Queue<string>();
            InitializeComponent();
        }    

        private void AmountBoxTextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void VolumetBoxTextChanged(object sender, TextChangedEventArgs e)
        {

        }       

        private void KeypadClick(object sender, RoutedEventArgs e)
        {
            var btnClicked = sender as Button;

            if (btnClicked != null)
            {
                _keyBuffer.Enqueue(btnClicked.Content as string);
            }

            ProcessKeyBuffer();
        }

        private void ProcessKeyBuffer()
        {
            var key = _keyBuffer.Dequeue();
            int number;

            switch (key)
            {
                case "预设金额":
                    break;
                case "预设油量":
                    break;
                case "清除":
                    break;
                case "取消":
                    _keyBuffer.Clear();
                    break;
                case "确定":
                    break;
                default:
                    _keyBuffer.Enqueue(key);
                    return;
            }
        }
    }
}
