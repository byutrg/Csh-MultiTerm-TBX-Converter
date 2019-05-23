using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MultiTermTBXMapper
{
    /// <summary>
    /// Interaction logic for ExportDatCatWidnow.xaml
    /// </summary>34
    public partial class ConstructQueueDrainOrder : Window
    {
        private MappingDict MappingDict { get; set; }

        private string FirstDC { get; set; }
        private string SecondDC { get; set; }

        public event Action<string[]> Complete;

        public static ConstructQueueDrainOrder Instance
        {
            get;
            private set;
        }

        public bool IsClosed { get; private set; } = false;

        public ConstructQueueDrainOrder(MappingDict mapping)
        {
            InitializeComponent();

            MappingDict = mapping;

            PopulateListBox();

            Instance = this;
        }

        private void PopulateListBox()
        {
            foreach(string dc in MappingDict.Keys)
            {
                ListBoxItem dcItem = new ListBoxItem();
                dcItem.MouseDoubleClick += ListBoxItem_MouseDoubleClick;
                dcItem.Content = dc;

                dcs.Items.Add(dcItem);
            }
        }

        private void Select(string dc)
        {
            if (FirstDC == null)
            {
                FirstDC = dc;
                mainDC.Text = dc;
            }
            else if (SecondDC == null)
            {
                SecondDC = dc;
                subDC.Text = dc;
            }
        }


        private void ListBoxItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ListBoxItem selectedItem = (ListBoxItem)sender;
            Select(selectedItem.Content.ToString());
        }

        private void Button_Submit_Click(object sender, RoutedEventArgs e)
        {
            if (FirstDC != null && SecondDC != null)
            {
                string[] order = new string[] { FirstDC, SecondDC, classificationElement.Text, level.Text };
                Complete(order);

                Close();
            }
        }

        private void Txtbox_search_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsLoaded)
            {
                int index = SearchListBox((sender as TextBox).Text);
                if (index > -1)
                {
                    (dcs.Items[index] as ListBoxItem).IsSelected = true;
                }
            }
        }

        private int SearchListBox(string query)
        {
            for (int i = 0; i < dcs.Items.Count; i++)
            {
                string item = dcs.Items[i].ToString();
                if (item.Contains(query))
                {
                    return i;
                }
            }
            return -1;
        }

        private void MainDCRemove_Click(object sender, RoutedEventArgs e)
        {
            if (FirstDC != null)
            {
                FirstDC = null;
                mainDC.Text = null;
            }
        }

        private void SubDCRemove_Click(object sender, RoutedEventArgs e)
        {
            if (SecondDC != null)
            {
                SecondDC = null;
                subDC.Text = null;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            IsClosed = true;
            Instance = null;
        }
    }
}
