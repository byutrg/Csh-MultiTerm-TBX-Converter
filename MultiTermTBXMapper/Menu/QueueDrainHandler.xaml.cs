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
            Switcher.Switch(new ConversionHandler(), ref mapControl.mapping, mapControl.Orders);
        }

        #region ISwitchable members
        public void UtilizeState<T>(T state)
        {
            throw new NotImplementedException();
        }

        public void UtilizeState<T>(ref T r)
        {
            mapControl.mapping = r as MappingDict;

            mapControl.submitAction += Submit;
        }

        public void UtilizeState<T1, T2>(ref T1 r, T2 state)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
