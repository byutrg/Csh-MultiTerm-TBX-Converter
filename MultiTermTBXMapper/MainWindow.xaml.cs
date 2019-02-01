using System;
using System.Windows;
using System.Windows.Controls;

namespace MultiTermTBXMapper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MultiTermMapper : Window
    {
        public MultiTermMapper()
        {
            InitializeComponent();
            Switcher.pageSwitcher = this;
            Switcher.Switch(new Menu.Welcome());

            TBXDatabase.Initialize();
        }

        public void Navigate(UserControl nextPage)
        {
            Content = null;
            Content = nextPage;
        }

        public void Navigate(UserControl nextPage, object state)
        {
            Content = nextPage;
            ISwitchable s = nextPage as ISwitchable;

            if (s != null)
                s.UtilizeState(state);
            else
                throw new ArgumentException("nextPage is not ISwitchable! " + nextPage.Name.ToString());
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            App.Current.Shutdown();
        }
    }
}
