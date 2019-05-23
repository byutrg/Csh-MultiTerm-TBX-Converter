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
        private Dictionary<string, List<string[]>> tbx_picklists = TBXDatabase.GetPicklists();

        private int index = 0;

        #region ISwitchable members
        public void UtilizeState<T>(T state)
        {
            this.mapping = state as MappingDict;
            dcs_with_picklists = mapping.GetDCsWithPicklists();

            FillMappedPicklistsDict();

            Display();
        }

        //public void UtilizeState<T>(ref T r)
        //{
            
        //}

        public void UtilizeState<T1, T2>(T1 r, T2 state)
        {
            throw new NotImplementedException();
        }
        #endregion

        public PickListHandler()
        {
            InitializeComponent();
        }

        private void Display()
        {
            ClearGrid();
            FillGrid();
            textblock_head.Text = "Picklists for " + dcs_with_picklists[index].ToString();
            EditHead();
        }

        private void EditHead()
        {
            string headText = "Picklist values for " + dcs_with_picklists[index];
            textblock_head.Text = headText;
        }

        private void ClearGrid()
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
        private void FillMappedPicklistsDict()
        {
            dcs_with_picklists.ForEach(delegate (string dc)
            {
                foreach (string val in mapping.GetContentList(dc))
                {
                    string[] keys = Methods.GetKeyArray(tbx_picklists.Keys);
                    if (mapping.GetTBXMappingList(dc).Count < 2 ||  Array.Exists(keys, item => item == mapping.GetTBXContentMap(dc)?.Get(val)))
                    {
                        try
                        {
                            mappedPicklists.Add(dc + "_" + val, false);
                        }
                        catch (ArgumentException)
                        {
                            continue;
                        }
                    }
                }
            });
        }

        private void FillGrid()
        {
            Grid grid = new Grid{
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            canvas_pls.Children.Add(grid);
            //Cycle through each Picklist


            mapping.GetContentList(dcs_with_picklists[index])?.ForEach(delegate (string content)
            {
                string[] keys = Methods.GetKeyArray(tbx_picklists.Keys);
                if (mapping.GetTBXMappingList(dcs_with_picklists[index]).Count < 2 || Array.Exists(keys, item => item == mapping.GetTBXContentMap(dcs_with_picklists[index])?.Get(content)))
                {
                    CreatePicklistMapControl(dcs_with_picklists[index], content, ref grid);
                }
            });
            
        }

        private void CreatePicklistMapControl(string user_dc, string pl_content, ref Grid grid)
        {
            PickListMapControl plmc = new PickListMapControl(pl_content);

            plmc.Selected += value => SetMapping(value[0], value[1]);

            string tbx_dc = mapping.GetTBXContentMap(user_dc)?.Get(pl_content);

            if (tbx_dc == null)
            {
                tbx_dc = mapping.GetTBXMappingList(user_dc)[0];
                mapping.GetTBXContentMap(user_dc).Add(pl_content, tbx_dc);
            }

            string tbx_selected = mapping.GetPicklistMapValue(user_dc, pl_content);
            if (tbx_selected == null)
            {
                tbx_selected = pl_content;
            }
            FillTBXComboBox(ref plmc.combo_tbx_picklist, mapping.GetTBXContentMap(user_dc)?.Get(pl_content), tbx_selected);

            grid.Children.Add(plmc);

            RowDefinition rd = new RowDefinition{
                Height = new GridLength(30)
            };
            grid.RowDefinitions.Add(rd);

            plmc.SetValue(Grid.RowProperty, grid.RowDefinitions.Count - 1);
        }

        private void FillTBXComboBox(ref ComboBox box, string tbx_dc_key, string tbx_selected = null)
        {
            for (int i = 0; i < tbx_picklists[tbx_dc_key]?.Count; i++)
            {
                ComboBoxItem item = new ComboBoxItem{
                    Content = tbx_picklists[tbx_dc_key][i][0]
                };
                

                if (tbx_selected == tbx_picklists[tbx_dc_key][i][0])
                {
                    item.IsSelected = true;
                }
                box.Items.Add(item);
            }
        }

        private void SetMapping(string user_pl, string tbx_pl)
        {
            string dc = dcs_with_picklists[index];

            mapping.SetPicklistMap(dc, user_pl, tbx_pl);
            mappedPicklists[dc + "_" + user_pl] = true;
            CheckCompletion();
        }

        private void CheckCompletion()
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
            Methods.IncrementIndex(ref index, dcs_with_picklists.Count);
            Display();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Methods.DecrementIndex(ref index);
            Display();
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new QueueDrainHandler(), mapping);
        }
    }
}
