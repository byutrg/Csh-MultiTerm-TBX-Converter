using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace MultiTermTBXMapper.Menu
{
    /// <summary>
    /// Interaction logic for VariantPicklistHandler.xaml
    /// </summary>
    public partial class VariantPicklistHandler : UserControl, ISwitchable
    {
        private MappingDict mapping;
        private List<string> datcats = new List<string>();
        private bool skip = false;

        private int index = 0;

        public VariantPicklistHandler()
        {
            InitializeComponent();
        }

        private void checkCompletion()
        {
            if(mapping.isGroupMappedToTBX(ref datcats))
            {
                vpmc.btn_submit.IsEnabled = true;
            }
        }


        private void display()
        {
            if (datcats.Count > 0)
            { 
                textblock_user_dc.Text = datcats[index];
                vpmc.clear();
                vpmc.fillListBoxes(mapping.getTBXMappingList(datcats[index]), mapping.getContentList(datcats[index]) as List<string>);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Methods.decrementIndex(ref index);
            display();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Methods.incrementIndex(ref index, datcats.Count);
            display();
        }

        private void nextPage()
        {
            Switcher.Switch(new PickListHandler(), ref mapping);
        }

        #region ISwitchable members
        public void UtilizeState<T>(T state)
        {
            throw new System.NotImplementedException();
        }

        public void UtilizeState<T>(ref T r)
        {
            throw new System.NotImplementedException();
        }

        public void UtilizeState<T1, T2>(ref T1 r, T2 state)
        {
            MappingDict mapping = r as MappingDict;
            List<string> datcats = state as List<string>;

            this.mapping = mapping;
            this.datcats = datcats;

            vpmc.map += value =>
            {
                string user_dc = datcats[index];
                string user_pl = value[0];
                string tbx_dc = value[1];

                this.mapping.setTBXContentMap(user_dc, user_pl, tbx_dc);

                checkCompletion();
            };

            vpmc.next += value =>
            {
                if (value)
                {
                    nextPage();
                }
            };

            display();
        }
        #endregion
    }
}
