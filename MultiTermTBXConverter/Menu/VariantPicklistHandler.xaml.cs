using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace MultiTermTBXMapper.Menu
{
    /// <summary>
    /// Interaction logic for VariantPicklistHandler.xaml
    /// </summary>
    public partial class VariantPicklistHandler : UserControl, ISwitchable
    {
        private MappingDict Mapping { get; set; }
        private List<string> Datcats { get; set; } = new List<string>();

        private int index = 0;

        public VariantPicklistHandler()
        {
            InitializeComponent();
        }

        private void CheckCompletion()
        {
            if(Mapping.IsGroupMappedToTBX(Datcats))
            {
                vpmc.btn_submit.IsEnabled = true;
            }
        }


        private void Display()
        {
            if (Datcats.Count > 0)
            { 
                textblock_user_dc.Text = Datcats[index];
                vpmc.clear();
                vpmc.fillListBoxes(Mapping.GetTBXMappingList(Datcats[index]), Mapping.GetContentList(Datcats[index]) as List<string>);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Methods.DecrementIndex(ref index);
            Display();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Methods.IncrementIndex(ref index, Datcats.Count);
            Display();
        }

        private void NextPage()
        {
            Switcher.Switch(new PickListHandler(), Mapping);
        }

        #region ISwitchable members
        public void UtilizeState<T>(T state)
        {
            throw new System.NotImplementedException();
        }

        //public void UtilizeState<T>(ref T r)
        //{
        //    throw new System.NotImplementedException();
        //}

        public void UtilizeState<T1, T2>(T1 r, T2 state)
        {
            MappingDict mapping = r as MappingDict;
            List<string> datcats = state as List<string>;

            this.Mapping = mapping;
            this.Datcats = datcats;

            vpmc.map += value =>
            {
                string user_dc = datcats[index];
                string user_pl = value[0];
                string tbx_dc = value[1];

                this.Mapping.SetTBXContentMap(user_dc, user_pl, tbx_dc);

                CheckCompletion();
            };

            vpmc.next += value =>
            {
                if (value)
                {
                    NextPage();
                }
            };

            Display();
        }
        #endregion
    }
}
