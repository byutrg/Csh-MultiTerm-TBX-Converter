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
using CefSharp;
using CefSharp.Wpf;

namespace MultiTermTBXMapper
{
    /// <summary>
    /// Interaction logic for QueueBundlingHelpWindow.xaml
    /// </summary>
    public partial class QueueBundlingHelpWindow : Window
    {
        private static QueueBundlingHelpWindow instance;

        public static QueueBundlingHelpWindow Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new QueueBundlingHelpWindow();
                }
                return instance;
            }
        }

        public QueueBundlingHelpWindow()
        {
            InitializeComponent();
            ChromeBrowser.Address = "https://www.tbxinfo.net/queue-bundling-orders/";
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            instance = null;
        }
    }
}
