using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace MultiTermTBXMapper.Menu
{
    /// <summary>
    /// Interaction logic for PickListHandler.xaml
    /// </summary>
    public partial class PickListHandler : UserControl, ISwitchable
    {
        private MappingDict mapping = new MappingDict();
        private List<string> dcs_with_picklists = new List<string>();
        private Dictionary<string, bool> mappedPicklists = new Dictionary<string,bool>();
        private Dictionary<string, List<string[]>> tbx_picklists = TBXDatabase.getPicklists();

        private int index = 0;

        public PickListHandler(ref MappingDict mapping)
        {
            InitializeComponent();

            this.mapping = mapping;
            dcs_with_picklists = mapping.getDCsWithPicklists();

            fillMappedPicklistsDict();

            display();
        }

        private void display()
        {
            clearGrid();
            fillGrid();
            textblock_head.Text = "Picklists for " + dcs_with_picklists[index].ToString();
            editHead();
        }

        private void editHead()
        {
            string headText = "Picklist values for " + dcs_with_picklists[index];
            textblock_head.Text = headText;
        }

        private void clearGrid()
        {
            canvas_pls.Children.RemoveRange(0, canvas_pls.Children.Count);
            
        }

        /// <summary>
        /// Fills the mappedPicklists Dictionary with a key that is the combination of a data category and a content, and a boolean value.
        /// <para>
        /// Any content values which are mapped to TBX data categories which do not have picklists will be skipped.
        /// </para>
        /// <para>
        /// <code>{ 'part of speech_noun' : false }</code>
        /// </para>
        /// <para>
        /// This means that the content "noun" of "part of speech" has not been mapped to a TBX picklist value yet.
        /// </para>
        /// </summary>
        private void fillMappedPicklistsDict()
        {
            dcs_with_picklists.ForEach(delegate (string dc)
            {
                foreach (string val in mapping.getContentList(dc))
                {
                    string[] keys = Methods.getKeyArray(tbx_picklists.Keys);
                    if (mapping.getTBXMappingList(dc).Count < 2 ||  Methods.inArray(ref keys, mapping.getTBXContentMap(dc)?.Get(val)))
                    {
                        try
                        {
                            mappedPicklists.Add(dc + "_" + val, false);
                        }
                        catch (System.ArgumentException e)
                        {
                            continue;
                        }
                    }
                }
            });
        }

        private void fillGrid()
        {
            Grid grid = new Grid();
            grid.HorizontalAlignment = HorizontalAlignment.Stretch;
            grid.VerticalAlignment = VerticalAlignment.Stretch;
            canvas_pls.Children.Add(grid);
            //Cycle through each Picklist


            mapping.getContentList(dcs_with_picklists[index])?.ForEach(delegate (string content)
            {
                string[] keys = Methods.getKeyArray(tbx_picklists.Keys);
                if (mapping.getTBXMappingList(dcs_with_picklists[index]).Count < 2 || Methods.inArray(ref keys, mapping.getTBXContentMap(dcs_with_picklists[index])?.Get(content)))
                {
                    createPicklistMapControl(dcs_with_picklists[index], content, ref grid);
                }
            });
            
        }

        private void createPicklistMapControl(string user_dc, string pl_content, ref Grid grid)
        {
            PickListMapControl plmc = new PickListMapControl(pl_content);

            plmc.select += value => setMapping(value[0], value[1]);

            string tbx_dc = mapping.getTBXContentMap(user_dc)?.Get(pl_content);

            if (tbx_dc == null)
            {
                tbx_dc = mapping.getTBXMappingList(user_dc)[0];
                mapping.getTBXContentMap(user_dc).Add(pl_content, tbx_dc);
            }

            string tbx_selected = mapping.getPicklistMapValue(user_dc, pl_content);

            fillTBXComboBox(ref plmc.combo_tbx_picklist, mapping.getTBXContentMap(user_dc)?.Get(pl_content), tbx_selected);

            grid.Children.Add(plmc);

            RowDefinition rd = new RowDefinition();
            rd.Height = new GridLength(30);
            grid.RowDefinitions.Add(rd);

            plmc.SetValue(Grid.RowProperty, grid.RowDefinitions.Count - 1);
        }

        private void fillTBXComboBox(ref ComboBox box, string tbx_dc_key, string tbx_selected = null)
        {
            for (int i = 0; i < tbx_picklists[tbx_dc_key]?.Count; i++)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = tbx_picklists[tbx_dc_key][i][0];

                if (tbx_selected == tbx_picklists[tbx_dc_key][i][0])
                {
                    item.IsSelected = true;
                }
                box.Items.Add(item);
            }
        }

        private void setMapping(string user_pl, string tbx_pl)
        {
            string dc = dcs_with_picklists[index];

            mapping.setPicklistMap(dc, user_pl, tbx_pl);
            mappedPicklists[dc + "_" + user_pl] = true;
            checkCompletion();
        }

        private void checkCompletion()
        {
            foreach (bool val in mappedPicklists.Values)
            {
                if (!val)
                {
                    submit.IsEnabled = false;
                    return;
                }
            }

            submit.IsEnabled = true;
        }
        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Methods.incrementIndex(ref index, dcs_with_picklists.Count);
            display();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Methods.decrementIndex(ref index);
            display();
        }

        private void submit_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new QueueDrainHandler(mapping));
        }
    }
}
