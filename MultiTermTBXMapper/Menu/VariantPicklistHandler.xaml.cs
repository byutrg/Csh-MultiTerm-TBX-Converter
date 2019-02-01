using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace MultiTermTBXMapper.Menu
{
    /// <summary>
    /// Interaction logic for VariantPicklistHandler.xaml
    /// </summary>
    public partial class VariantPicklistHandler : UserControl
    {
        private MappingDict mapping;
        private List<string> datcats = new List<string>();
        private bool skip = false;

        private int index = 0;

        public VariantPicklistHandler(ref MappingDict mapping, List<string> datcats)
        {
            InitializeComponent();

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
            Switcher.Switch(new PickListHandler(ref mapping));
        }
    }
}
