using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MultiTermTBXMapper
{
    /// <summary>
    /// Interaction logic for TBXDatCatWindow.xaml
    /// </summary>
    public partial class TBXDatCatWindow : Window
    {
        private List<string[]> datcats = new List<string[]>();
        private Dictionary<string,string[]> datcat_dict = new Dictionary<string, string[]>();
        private DataRow[] sortedData;
        private DataTable dt = new DataTable();

        public event Action<string> selected;

        public TBXDatCatWindow(string selected = "None: Do Not Map")
        {
            InitializeComponent();

            datcats = TBXDatabase.getAll();
            cleanData();

            populateListBox();
            //if (selected != null)
            //{
            int index = getListItemIndex(selected);
            (dcs_tbx.Items[index] as ListBoxItem).IsSelected = true;
            //}
        }

        private void cleanData()
        {
            dt.Columns.Add("Name");
            dt.Columns.Add("XML");
            dt.Columns.Add("Descrip");

            for (int x = 1; x < datcats.Count(); x++)
            {
                DataRow row = dt.NewRow();

                row["Name"] = datcats[x][0];
                row["XML"] = datcats[x][1];
                row["Descrip"] = datcats[x][2];

                datcat_dict.Add(datcats[x][0], new string[2] { datcats[x][1], datcats[x][2] } );

                dt.Rows.Add(row);
            }

            sortedData = dt.Select("", "Name ASC");
        }

        private void populateListBox()
        {
            for (int i = 0; i < sortedData.Count(); i++)
            {
                ListBoxItem dc = new ListBoxItem();

                dc.Content = sortedData[i][0];

                dcs_tbx.Items.Add(dc);
            }
        }

        private void dcs_tbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem item = ((sender as ListBox).SelectedItem as ListBoxItem);
            item.MouseDoubleClick += ListBoxItem_MouseDoubleClick;
            dc_name_block.Text = item.Content.ToString();

            if (((sender as ListBox).SelectedItem as ListBoxItem).Content.ToString() != "None: Do Not Map")
            {
                textblock_xml.Text = datcat_dict[item.Content.ToString()][0];
                textbox_descrip.Text = datcat_dict[item.Content.ToString()][1];
            }
            else
            {
                textblock_xml.Text = "NOT A TBX DATA CATEGORY";
                textbox_descrip.Text = "Your data category will NOT be mapped to a TBX data category.  " +
                    "The data category will instead be placed in an invalid dummy /unknownTermType/ element:\n" +
                    "\n" +
                    "\tExample: \n" +
                    "\n" +
                    "\t<termNote type='unknownTermType'>CONTENT</termNote>\n" +
                    "\n" +
                    "\n" +
                    "\n" +
                    "NOTE: The dummy elements will need to be removed before the TBX file is valid.";
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsLoaded)
            {
                int index = SearchListBox((sender as TextBox).Text);
                if (index > -1)
                {
                    (dcs_tbx.Items[index] as ListBoxItem).IsSelected = true;
                }
            }
        }

        private int SearchListBox(string query)
        {
            for(int i = 0; i < dcs_tbx.Items.Count; i++)
            {
                string item = dcs_tbx.Items[i].ToString();
                if(item.Contains(query))
                {
                    return i;
                }
            }
            return -1;
        }

        private int getListItemIndex(string item)
        {
            for (int i = 0; i < dcs_tbx.Items.Count; i++)
            {
                if ((dcs_tbx.Items[i] as ListBoxItem).Content.ToString() == item)
                {
                    return i;
                }
            }

            return 0;
        }

        private void select()
        {
            if ((dcs_tbx.SelectedItem as ListBoxItem) != null)
            {
                
                selected((dcs_tbx.SelectedItem as ListBoxItem).Content.ToString());
            }

            Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            select();
        }

        private void ListBoxItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            select();
        }
    }
}
