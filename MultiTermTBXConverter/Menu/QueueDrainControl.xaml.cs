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
        public MappingDict Mapping { get; set; }
        public QueueDrainOrders Orders { get; set; } = new QueueDrainOrders();
        private ConstructQueueDrainOrder constructQueueDrainOrder;
        public ConstructQueueDrainOrder ConstructQueueDrainOrder
        {
            get
            {
                if (constructQueueDrainOrder == null)
                {
                    ConstructQueueDrainOrder = new ConstructQueueDrainOrder(Mapping);
                }
                else if (constructQueueDrainOrder.IsClosed)
                {
                    ConstructQueueDrainOrder = new ConstructQueueDrainOrder(Mapping);
                }

                return constructQueueDrainOrder;
            }
            private set => constructQueueDrainOrder = value;
        }

        public Action SubmitAction { get; set; }

        private Dictionary<string, string[]> OrderDict { get; set; } = new Dictionary<string, string[]>();

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
            ListBoxItem queueOrder = new ListBoxItem
            {
                Content = string.Format("{3}: {0} -> {1} -> {2}", order)
            };

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
            ConstructQueueDrainOrder.Show();
            ConstructQueueDrainOrder.Complete += AddToList;
        }

        /// <summary>
        /// Remove an order from the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Remove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string content = lb_queue_orders.SelectedItem.ToString();
                OrderDict.Remove(content);

                lb_queue_orders.Items.Remove(lb_queue_orders.SelectedItem);
            } catch (NullReferenceException)
            {
                return;
            }
        }
        
        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            foreach (string[] order in OrderDict.Values)
            {
                Orders.AddOrder(order);
            }

            SubmitAction();
        }
    }
}
