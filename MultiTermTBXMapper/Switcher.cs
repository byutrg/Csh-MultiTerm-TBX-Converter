using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiTermTBXMapper
{
    public static class Switcher
    {
        public static MultiTermMapper pageSwitcher;

        public static void Switch(UserControl newPage)
        {
            pageSwitcher.Navigate(newPage);
        }

        public static void Switch<T1,T2>(T1 newPage, T2 state) where T1 : UserControl
        {
            pageSwitcher.Navigate(newPage, state);
        }

        public static void Switch<T1, T2>(T1 newPage, ref T2 r) where T1 : UserControl
        {
            pageSwitcher.Navigate(newPage, ref r);
        }

        public static void Switch<T1,T2,T3>(T1 newPage, ref T2 r, T3 state) where T1 : UserControl
        {
            pageSwitcher.Navigate(newPage, ref r, state);
        }
    }
}
