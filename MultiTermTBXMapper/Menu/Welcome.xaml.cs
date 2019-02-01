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

        public void UtilizeState(object state)
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

            dlg.DefaultExt = ".tbx";
            dlg.Filter = "XML Files (*.xml)|*.xml | TBX Files (*.tbx)|*.tbx | All files (*.*)|*.*";

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

                if (saveOption == 1 || saveOption == 3)
                {
                    string filename = dlg.FileName;
                    Switcher.Switch(new DatCatHandler(filename));
                }
                else if (saveOption == 2) // Skip to Mapping upload window
                {
                    string filename = dlg.FileName;
                    Switcher.Switch(new MappingUpload(filename));
                }
            }
        }
    }
}
