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
        private MappingDict mappingDict;

        private string firstDC;
        private string secondDC;

        public event Action<string[]> complete;

        public ConstructQueueDrainOrder(MappingDict mapping)
        {
            InitializeComponent();

            mappingDict = mapping;

            populateListBox();
        }

        private void populateListBox()
        {
            foreach(string dc in mappingDict.Keys)
            {
                ListBoxItem dcItem = new ListBoxItem();
                dcItem.MouseDoubleClick += ListBoxItem_MouseDoubleClick;
                dcItem.Content = dc;

                dcs.Items.Add(dcItem);
            }
        }

        private void select(string dc)
        {
            if (firstDC == null)
            {
                firstDC = dc;
                mainDC.Text = dc;
            }
            else if (secondDC == null)
            {
                secondDC = dc;
                subDC.Text = dc;
            }
        }


        private void ListBoxItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ListBoxItem selectedItem = (ListBoxItem)sender;
            select(selectedItem.Content.ToString());
        }

        private void Button_Submit_Click(object sender, RoutedEventArgs e)
        {
            if (firstDC != null && secondDC != null)
            {
                string[] order = new string[] { firstDC, secondDC, classificationElement.Text, level.Text };
                complete(order);

                Close();
            }
        }

        private void txtbox_search_TextChanged(object sender, TextChangedEventArgs e)
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

        private void mainDCRemove_Click(object sender, RoutedEventArgs e)
        {
            if (firstDC != null)
            {
                firstDC = null;
                mainDC.Text = null;
            }
        }

        private void subDCRemove_Click(object sender, RoutedEventArgs e)
        {
            if (secondDC != null)
            {
                secondDC = null;
                subDC.Text = null;
            }
        }
    }
}
