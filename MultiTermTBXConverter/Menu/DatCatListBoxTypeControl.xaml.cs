﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DuoVia.FuzzyStrings;

namespace MultiTermTBXMapper.Menu
{
    /// <summary>
    /// Interaction logic for DatCatListBoxTypeControl.xaml
    /// </summary>
    public partial class DatCatListBoxTypeControl : UserControl
    {
        public Action<TBXMappingList> ListBoxItems;

        private TBXDatCatWindow tbxDatCatWindow = new TBXDatCatWindow();

        public TBXDatCatWindow TBXDatCatWindow
        {
            get
            {
                if (!tbxDatCatWindow.IsClosed)
                {
                    return tbxDatCatWindow;
                }
                else
                {
                    TBXDatCatWindow = new TBXDatCatWindow();
                    return tbxDatCatWindow;
                }
            }
            set => tbxDatCatWindow = value;
        }

        public DatCatListBoxTypeControl()
        {
            InitializeComponent();
        }

        public void Clear()
        {
            Methods.RemoveListBoxItems(ref lb_tbx_dcs);
        }

        public void FillItems(List<string> tbx_dcs)
        {
            if (tbx_dcs != null)
            {
                for (int i = 0; i < tbx_dcs.Count; i++)
                {
                    ListBoxItem item = new ListBoxItem{ Content = tbx_dcs[i] };

                    lb_tbx_dcs.Items.Add(item);
                }
            }
        }

        public void FindBest(string dc)
        {
            List<string[]> datcats = TBXDatabase.GetNames();
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
                ListBoxItem tbx = new ListBoxItem
                {

                    Content = datcats[bestIndex][0]
                };

                lb_tbx_dcs.Items.Add(tbx);
            }
            else
            {
                FindBest("note");
            }

            UpdateItems();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TBXDatCatWindow.Show();

            TBXDatCatWindow.Selected += value =>
            {
                if (!lb_tbx_dcs.Items.Cast<ListBoxItem>().Any(i => i.Content.Equals(value)))
                {

                    ListBoxItem item = new ListBoxItem { Content = value };

                    lb_tbx_dcs.Items.Add(item);

                    UpdateItems();
                } else { MessageBox.Show("The chosen data category has already been added."); }
            };
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (lb_tbx_dcs.SelectedItem != null)
            {
                lb_tbx_dcs.Items.Remove(lb_tbx_dcs.SelectedItem);

                UpdateItems();
            }
        }

        private void UpdateItems()
        {
            TBXMappingList items = new TBXMappingList();
            for (int i = 0; i < lb_tbx_dcs.Items.Count; i++)
            {
                items.Add((lb_tbx_dcs.Items[i] as ListBoxItem).Content.ToString());
            }

            ListBoxItems(items);
        }

        private void Lb_tbx_dcs_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if((sender as ListBox).SelectedItem != null)
                {
                    (sender as ListBox).Items.Remove((sender as ListBox).SelectedItem);

                    UpdateItems();
                }
            }
        }
    }
}
