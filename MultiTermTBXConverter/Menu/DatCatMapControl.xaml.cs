using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using DuoVia.FuzzyStrings;
using System.Collections.Generic;

namespace MultiTermTBXMapper.Menu
{
    /// <summary>
    /// Interaction logic for DatCatMapControl.xaml
    /// </summary>
    public partial class DatCatMapControl : UserControl
    {
        public event Action<string> mapping;
        public event Action<bool> convert;

        public DatCatMapControl()
        {
            InitializeComponent();
        }

        public void setContent(string datcat_user, string datcat_tbx = null)
        {
            dc_user.Text = datcat_user;

            if (datcat_tbx != null)
            {
                updateButtonMapped(datcat_tbx);
            }
            else
            {
                updateButtonUnmapped();
                findBest(datcat_user);
            }
        }

        private void findBest(string dc)
        {
            List<string[]> datcats = TBXDatabase.getNames();
            int bestDistance = 99999;
            int bestIndex = -1;

            for (int i = 0; i < datcats.Count(); i++)
            {
                int distance = datcats[i][0].LevenshteinDistance(dc);
                
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestIndex = i;
                }
            }

            if (bestDistance < dc.ToCharArray().Count() / 2)
            {
                updateButtonSuggested(datcats[bestIndex][0]);
            }
            else
            {
                updateButtonUnmapped();
            }

        }

        private void dc_tbx_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            TBXDatCatWindow tbxWindow;
            if (dc_tbx.Content.ToString() != "???")
            {
                tbxWindow = new TBXDatCatWindow(dc_tbx.Content.ToString());
            }
            else
            {
                tbxWindow = new TBXDatCatWindow();
            }

            tbxWindow.Show();
            tbxWindow.selected += value =>
            {
                if(value == null)
                {
                    value = "NONE";
                }
                
                setMapping(value);
            };

        }

        private bool isYellow()
        {
            return isColor("#FFFAFF99");
        }

        private void setYellow()
        {
            setColor("#FFFAFF99");
        }

        private bool isRed()
        {
            return isColor("#FFFF9999");
        }

        private void setRed()
        {
            setColor("#FFFF9999");
        }

        private bool isGreen()
        {
            return isColor("#FF99FFA7");
        }

        private void setGreen()
        {
            setColor("#FF99FFA7");
        }

        private bool isColor(string color)
        {
            if (dc_tbx.Background != (SolidColorBrush)new BrushConverter().ConvertFromString(color))
            {
                return true;
            } else
            {
                return false;
            }
        }

        private void setColor(string color)
        {
            dc_tbx.Background = (SolidColorBrush)new BrushConverter().ConvertFromString(color);
        }

        private void updateButtonMapped(string datcat_tbx)
        {
            dc_tbx.Content = datcat_tbx;
            setGreen();
        }

        private void updateButtonUnmapped()
        {
            dc_tbx.Content = "???";
            setRed();
        }

        private void updateButtonSuggested(string datcat_tbx)
        {
            dc_tbx.Content = datcat_tbx;
            setYellow();
        }

        private void setMapping(string datcat_tbx)
        {
            mapping(datcat_tbx);
            updateButtonMapped(datcat_tbx);
        }

        private void button_convert_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            convert(true);
        }
    }
}
