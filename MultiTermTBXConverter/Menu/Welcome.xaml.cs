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
    /// Interaction logic for Welcome.xaml
    /// </summary>
    public partial class Welcome : UserControl, ISwitchable
    {
        public Welcome()
        {
            InitializeComponent();
        }

        #region ISwitchable Members

        public void UtilizeState<T>(T state)
        {
            throw new NotImplementedException();
        }

        public void UtilizeState<T>(ref T r)
        {
            throw new NotImplementedException();
        }

        public void UtilizeState<T1, T2>(ref T1 r, T2 state)
        {
            throw new NotImplementedException();
        }

        #endregion

        private void btn_import_Click(object sender, RoutedEventArgs e)
        {

            // Check Sender
            Button clickedButton = (Button)sender;

            int saveOption = 0;
            if (clickedButton.Name == "btn_import")
            {
                saveOption = 1;
            }
            else if (clickedButton.Name == "btn_has_Map")
            {
                saveOption = 2;
            }
            else if (clickedButton.Name == "btn_just_Map")
            {
                saveOption = 3;
            }

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.DefaultExt = ".xml";
            dlg.Filter = "XML Files (*.xml)|*.xml|TBX Files (*.tbx)|*.tbx|All files (*.*)|*.*";

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                string dialect = "ERROR";

                if (Core.IsChecked == true)  
                {
                    dialect = "TBX-Core";
                }
                else if (Min.IsChecked == true)
                {
                    dialect = "TBX-Min";
                }
                else if (Basic.IsChecked == true)
                {
                    dialect = "TBX-Basic";
                }

                Singleton.Instance.setDialect(dialect);
                Singleton.Instance.setSaveOption(saveOption);

                string filename = dlg.FileName;
                Singleton.Instance.setPath(filename);
                if (saveOption == 1 || saveOption == 3)
                {
                    Switcher.Switch(new DatCatHandler(), filename);
                }
                else if (saveOption == 2) // Skip to Mapping upload window
                {
                    Switcher.Switch(new MappingUpload(), filename);
                }
            }
        }
    }
}
