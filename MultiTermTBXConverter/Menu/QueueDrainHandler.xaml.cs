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

namespace MultiTermTBXMapper.Menu
{
    /// <summary>
    /// Interaction logic for QueueDrainHandler.xaml
    /// </summary>
    public partial class QueueDrainHandler : UserControl, ISwitchable
    {
        public QueueDrainHandler()
        {
            InitializeComponent();
        }

        public void Submit()
        {
            Switcher.Switch(new ConversionHandler(), mapControl.Mapping, mapControl.Orders);
        }

        #region ISwitchable members
        public void UtilizeState<T>(T state)
        {
            mapControl.Mapping = state as MappingDict;

            mapControl.SubmitAction += Submit;
        }

        //public void UtilizeState<T>(ref T r)
        //{
            
        //}

        public void UtilizeState<T1, T2>(T1 r, T2 state)
        {
            throw new NotImplementedException();
        }
        #endregion

        private void Whats_this_btn_Click(object sender, RoutedEventArgs e)
        {
            QueueBundlingHelpWindow helpWindow = QueueBundlingHelpWindow.Instance;
            helpWindow.Show();
        }
    }
}
