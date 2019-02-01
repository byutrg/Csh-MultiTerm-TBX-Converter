using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace MultiTermTBXMapper.Menu
{
    /// <summary>
    /// Interaction logic for VariantPicklistMapControl.xaml
    /// </summary>
    public partial class VariantPicklistMapControl : UserControl
    {
        public Action<string[]> map;
        public Action<bool> next; 
        private Dictionary<string, object[]> mapping = new Dictionary<string,object[]>();

        public VariantPicklistMapControl()
        {
            InitializeComponent();
        }

        public void clear()
        {
            Methods.removeListBoxItems(ref lb_tbx_dcs);
            Methods.removeListBoxItems(ref lb_user_picklists);
        }

        public void fillListBoxes(List<string> tbx_dcs, List<string> values)
        {
            foreach (string dc in tbx_dcs)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = dc;

                lb_tbx_dcs.Items.Add(item);
            }

            
            for (int i = 0; i < values.Count; i++)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = values[i];

                lb_user_picklists.Items.Add(item);
            }

            lb_user_picklists.SelectedIndex = 0;
            
        }

        private void lb_user_picklists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count < 1)
            {
                return;
            }

            if (lb_user_picklists.SelectedItem != null)
            {
                string picklist_value = (e.AddedItems[0] as ListBoxItem).Content.ToString();
                if (mapping.ContainsKey(picklist_value))
                {
                    lb_tbx_dcs.SelectedIndex = (int) mapping[picklist_value][0];
                }
                else
                {
                    lb_tbx_dcs.SelectedItem = lb_tbx_dcs.Items[0];
                }
            }
 
        }

        private void lb_tbx_dcs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count < 1)
            {
                return;
            }

            string picklist_value = (lb_user_picklists.SelectedItem as ListBoxItem).Content.ToString();
            string tbx_dc = (e.AddedItems[0] as ListBoxItem).Content.ToString();
            int tbx_dc_index = lb_tbx_dcs.Items.IndexOf(e.AddedItems[0]);


            string[] map_string = new string[2] { picklist_value, tbx_dc };
            map?.Invoke(map_string);

            if (mapping.ContainsKey(picklist_value))
            {
                mapping[picklist_value] = new object[2] { tbx_dc_index, tbx_dc };
            }
            else
            {
                mapping.Add(picklist_value, new object[2] { tbx_dc_index, tbx_dc });
            }
        }

        private void btn_submit_Click(object sender, RoutedEventArgs e)
        {
            next(true);
        }
    }
}
