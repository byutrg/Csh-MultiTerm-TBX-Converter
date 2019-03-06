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

        public MappingUpload()
        {
            InitializeComponent();
        }

        #region ISwitchable members
        public void UtilizeState<T>(T state)
        {
            multiTermXMLFile = state as string;
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
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.DefaultExt = ".json";
            dlg.Filter = "JSON Files (*.json)|*.json|All files (*.*)|*.*";

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                string jsonUpload = dlg.FileName;
                //ConverterApp mt2tbx = new ConverterApp();
                //mt2tbx.deserializeFile(jsonUpload, multiTermXMLFile, Singleton.Instance.getDialect(), false);
                Switcher.Switch(new ConversionHandler(), jsonUpload);
            }
        }

    }


}
