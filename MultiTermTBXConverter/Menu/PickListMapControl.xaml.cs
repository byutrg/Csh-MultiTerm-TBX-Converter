using System;
using System.Windows;
using System.Windows.Controls;
namespace MultiTermTBXMapper.Menu
{
    /// <summary>
    /// Interaction logic for PickListMapControl.xaml
    /// </summary>
    public partial class PickListMapControl : UserControl
    {
        public Action<string[]> select;

        public PickListMapControl(string pl_content)
        {
            InitializeComponent();

            label_picklist_item.Content = pl_content;
            HorizontalAlignment = HorizontalAlignment.Center;
            Margin = new Thickness(50, 2, 50, 2);

            combo_tbx_picklist.SelectionChanged += Item_Selected;
        }

        private void Item_Selected(object sender, RoutedEventArgs e)
        {
            select(new string[2] { label_picklist_item.Content.ToString(), ((sender as ComboBox).SelectedItem as ComboBoxItem).Content.ToString() });
        }
    }
}
