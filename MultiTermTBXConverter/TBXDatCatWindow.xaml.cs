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
        private List<string[]> Datcats { get; set; } = new List<string[]>();
        private Dictionary<string, string[]> Datcat_dict { get; set; } = new Dictionary<string, string[]>();
        private DataRow[] SortedData { get; set; }
        private DataTable dt = new DataTable();

        private static TBXDatCatWindow instance;

        public static TBXDatCatWindow Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TBXDatCatWindow();
                }
                return instance;
            }
            private set => instance = value;
        }

        public bool IsClosed { get; private set; } = false;

        public event Action<string> Selected;

        public TBXDatCatWindow()
        {
            InitializeComponent();

            Datcats = TBXDatabase.GetAll();
            CleanData();

            PopulateListBox();
            //if (selected != null)
            //{
            SelectItem("None: Do Not Map");
            //}
        }

        public void SelectItem(string selected = "None: Do Not Map")
        {
            int index = GetListItemIndex(selected);
            (dcs_tbx.Items[index] as ListBoxItem).IsSelected = true;
        }

        private void CleanData()
        {
            dt.Columns.Add("Name");
            dt.Columns.Add("XML");
            dt.Columns.Add("Descrip");

            for (int x = 1; x < Datcats.Count(); x++)
            {
                DataRow row = dt.NewRow();

                row["Name"] = Datcats[x][0];
                row["XML"] = Datcats[x][1];
                row["Descrip"] = Datcats[x][2];

                Datcat_dict.Add(Datcats[x][0], new string[2] { Datcats[x][1], Datcats[x][2] } );

                dt.Rows.Add(row);
            }

            SortedData = dt.Select("", "Name ASC");
        }

        private void PopulateListBox()
        {
            for (int i = 0; i < SortedData.Count(); i++)
            {
                ListBoxItem dc = new ListBoxItem
                {
                    Content = SortedData[i][0]
                };

                dcs_tbx.Items.Add(dc);
            }
        }

        private void Dcs_tbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem item = ((sender as ListBox).SelectedItem as ListBoxItem);
            item.MouseDoubleClick += ListBoxItem_MouseDoubleClick;
            dc_name_block.Text = item.Content.ToString();

            if (((sender as ListBox).SelectedItem as ListBoxItem).Content.ToString() != "None: Do Not Map")
            {
                textblock_xml.Text = Datcat_dict[item.Content.ToString()][0];
                textbox_descrip.Text = Datcat_dict[item.Content.ToString()][1];
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

        private int GetListItemIndex(string item)
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

        private void Select()
        {
            if ((dcs_tbx.SelectedItem as ListBoxItem) != null)
            {
                
                Selected((dcs_tbx.SelectedItem as ListBoxItem).Content.ToString());
            }

            Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Select();
        }

        private void ListBoxItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Select();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Instance = null;
            IsClosed = true;
        }
    }
}
