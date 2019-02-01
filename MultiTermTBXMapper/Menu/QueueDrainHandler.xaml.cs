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
    public partial class QueueDrainHandler : UserControl
    {
        public QueueDrainHandler(MappingDict mapping)
        {
            InitializeComponent();
            mapControl.mapping = mapping;

            mapControl.submitAction += Submit;
        }

        public void Submit()
        {
            Switcher.Switch(new ConversionHandler(mapControl.mapping, mapControl.Orders));
        }

    }
}
