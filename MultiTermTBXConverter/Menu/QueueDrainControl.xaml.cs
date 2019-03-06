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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MultiTermTBXMapper.Menu
{
    /// <summary>
    /// Interaction logic for QueueDrainControl.xaml
    /// </summary>
    public partial class QueueDrainControl : UserControl
    {
        public MappingDict mapping;
        public QueueDrainOrders Orders = new QueueDrainOrders();

        public Action submitAction;

        private Dictionary<string, string[]> OrderDict = new Dictionary<string, string[]>();

        public QueueDrainControl()
        {
            InitializeComponent();
        }

        public QueueDrainOrders GetOrders()
        {
            return Orders;
        }

        private void AddToList(string[] order)
        {
            ListBoxItem queueOrder = new ListBoxItem();
            queueOrder.Content = string.Format("{3}: {0} -> {1} -> {2}", order);

            OrderDict.Add(queueOrder.Content.ToString(), order);
            lb_queue_orders.Items.Add(queueOrder);
        }

        /// <summary>
        /// Add an order to the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            ConstructQueueDrainOrder constructQeueDrainOrder = new ConstructQueueDrainOrder(mapping);
            constructQeueDrainOrder.Show();
            constructQeueDrainOrder.complete += AddToList;
        }

        /// <summary>
        /// Remove an order from the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Remove_Click(object sender, RoutedEventArgs e)
        {
            string content = lb_queue_orders.SelectedItem.ToString();
            OrderDict.Remove(content);

            lb_queue_orders.Items.Remove(lb_queue_orders.SelectedItem);
        }
        
        private void submit_Click(object sender, RoutedEventArgs e)
        {
            foreach (string[] order in OrderDict.Values)
            {
                Orders.AddOrder(order);
            }

            submitAction();
        }
    }
}
