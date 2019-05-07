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

        public void Navigate<T1,T2>(T1 nextPage, T2 state) where T1 : UserControl
        {
            Content = nextPage;

            if (nextPage is ISwitchable s)
                s.UtilizeState(state);
            else
                throw new ArgumentException($"nextPage is not ISwitchable! {nextPage.Name}");
        }

        //public void Navigate<T1, T2>(T1 nextPage, ref T2 r) where T1 : UserControl
        //{
        //    Content = nextPage;
        //    ISwitchable s = nextPage as ISwitchable;

        //    if (s != null)
        //        s.UtilizeState(ref r);
        //    else
        //        throw new ArgumentException($"nextPage is not ISwitchable! {nextPage.Name}");
        //}

        public void Navigate<T1, T2, T3>(T1 nextPage, T2 r, T3 state) where T1 : UserControl
        {
            Content = nextPage;

            if (nextPage is ISwitchable s)
                s.UtilizeState(r, state);
            else
                throw new ArgumentException($"nextPage is not ISwitchable! {nextPage.Name}");
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            App.Current.Shutdown();
        }
    }
}
