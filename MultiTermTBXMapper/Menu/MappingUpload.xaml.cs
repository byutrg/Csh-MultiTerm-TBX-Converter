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
using System.Windows.Shapes;
using System.Xml;

namespace MultiTermTBXMapper.Menu
{
    /// <summary>
    /// Interaction logic for MappingUpload.xaml
    /// </summary>
    public partial class MappingUpload : UserControl, ISwitchable
    {
        private string multiTermXMLFile;

        public MappingUpload(string filename)
        {
            InitializeComponent();
            multiTermXMLFile = filename;
        }

        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }

        private void btn_import_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.DefaultExt = ".tbx";
            dlg.Filter = "XML Files (*.xml)|*.xml | TBX Files (*.tbx)|*.tbx | All files (*.*)|*.*";

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                string jsonUpload = dlg.FileName;
                ConverterApp mt2tbx = new ConverterApp();
                mt2tbx.deserializeFile(jsonUpload, multiTermXMLFile, Singleton.Instance.getDialect(), false);
            }
        }

    }


}
