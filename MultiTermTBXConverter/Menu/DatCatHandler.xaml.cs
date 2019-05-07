using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

namespace MultiTermTBXMapper.Menu
{
    /// <summary>
    /// Interaction logic for DatCatHandler.xaml
    /// </summary>
    public partial class DatCatHandler : UserControl, ISwitchable
    {
        public List<string> datcats = new List<string>();
        public MappingDict mapping = new MappingDict();

        private string filename;

        private int index = 0;
        private string[] indexes_visited;

        #region ISwitchable members
        public void UtilizeState<T>(T state)
        {
            filename = state as string;

            Globals.filename = filename;

            mapControl.ListBoxItems += value => SetMapping(value);

            LoadDatCats();
            indexes_visited = new string[datcats.Count];

            CycleIndexes();

            Display();
        }

        //public void UtilizeState<T>(ref T r)
        //{
        //    throw new NotImplementedException();
        //}

        public void UtilizeState<T1, T2>(T1 r, T2 state)
        {
            throw new NotImplementedException();
        }
        #endregion

        public DatCatHandler()
        {
            InitializeComponent();
        }

        private void CycleIndexes()
        {
            primary_canvas.Visibility = Visibility.Hidden;
            for (int i = 0; i < indexes_visited.Length; i++)
            {
                Methods.IncrementIndex(ref index, indexes_visited.Length);
                Display();
            }
            index = 0;
            Display();
            primary_canvas.Visibility = Visibility.Visible;
        }

        private void LoadDatCats()
        {
            XmlReaderSettings settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Ignore
            };
            XmlReader reader = XmlReader.Create(filename, settings);

            bool start = false;
            int level = 0;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && (reader.Name == "body" || reader.Name == "mtf"))
                    start = true;
                if (start == true)
                {
                    if (reader.Name == "back")
                    {
                        start = false;
                        continue;
                    }

                    if (reader.Name == "conceptGrp" || reader.Name == "languageGrp" || reader.Name == "termGrp" ||
                        reader.Name == "termEntry" || reader.Name == "langSet" || reader.Name == "tig" ||
                        reader.Name == "conceptEntry" || reader.Name == "langSec" || reader.Name == "termSec")
                    {
                        switch(reader.Name)
                        {
                            case "conceptGrp":
                            case "termEntry":
                            case "conceptEntry":
                                level = 1;
                                break;
                            case "languageGrp":
                            case "langSet":
                            case "langSec":
                                level = 2;
                                break;
                            case "termGrp":
                            case "tig":
                            case "termSec":
                                level = 3;
                                break;
                        }
                        
                    }

                    if (reader.NodeType == XmlNodeType.Element && reader.HasAttributes && null != reader.GetAttribute("type") && reader.Name != "language")
                    {
                        string dc = reader.GetAttribute("type");
                        if (!datcats.Contains(dc))
                        {
                            datcats.Add(dc);
                            mapping.Add(dc);

                            switch(level)
                            {
                                case 1:
                                    mapping.LevelMap["conceptGrp"].Add(dc);
                                    break;
                                case 2:
                                    mapping.LevelMap["languageGrp"].Add(dc);
                                    break;
                                case 3:
                                    mapping.LevelMap["termGrp"].Add(dc);
                                    break;
                            }
                        }

                        //Pull out text for use later with picklists
                        XmlReader textReader = reader.ReadSubtree();

                        while (textReader.Read())
                        {
                            if (textReader.NodeType == XmlNodeType.Text)
                            {
                                List<string> values = mapping.GetContentList(dc) as List<string>;
                                if (!values.Contains(reader.Value))
                                {
                                    mapping.GetContentList(dc).Add(textReader.Value);
                                }
                            }
                        }
                    }

                }
            }

            datcats.Sort();
            textTotal.Text = datcats.Count().ToString();
        }

        private void Display()
        {
            lbl_user_dc.Content = datcats[index];
            mapControl.clear();

            if (mapping.GetTBXMappingList(datcats[index])?.Count > 0)
            {
                mapControl.fillItems(mapping.GetTBXMappingList(datcats[index]));
            }

            if(!Array.Exists(indexes_visited, item => item == index.ToString()))
            {
                mapControl.findBest(datcats[index]);
                indexes_visited[index] = index.ToString();
            }

            UpdateCounter();
        }

        private void UpdatePercentage()
        {
            double mapped = 0;

            foreach (string key in mapping.Keys)
            {
                if (mapping.GetTBXMappingList(key)?.Count > 0)
                {
                    mapped++;
                }
            }

            double percent = Math.Round(mapped * 100 / (double)mapping.Keys.Count(), 2, MidpointRounding.AwayFromZero);

            submit.IsEnabled = (percent == 100) ? true : false; 

            textPercent.Text = percent.ToString() + "%";
        }

        private void UpdateCounter()
        {
            textIndex.Text = (index+1).ToString();
        }

        private void SetMapping(TBXMappingList datcat_tbx)
        {
            mapping.SetTBXMappingList(datcats[index], datcat_tbx );
            UpdatePercentage();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Methods.IncrementIndex(ref index, datcats.Count);
            Display();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Methods.DecrementIndex(ref index);
            Display();
        }

        private List<string> GetDatCatsWithMultiMap()
        {
            List<string> dcs_with_multi_map = new List<string>();

            foreach (string key in mapping.Keys)
            {
                if (mapping.GetTBXMappingList(key)?.Count > 1)
                {
                    dcs_with_multi_map.Add(key);
                }
            }

            return dcs_with_multi_map;
        }


        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            List<string> dcs_with_multi_map = GetDatCatsWithMultiMap();

            if (dcs_with_multi_map.Count > 0)
            {
                Switcher.Switch(new VariantPicklistHandler(), mapping, dcs_with_multi_map);
            }
            else
            {
                Switcher.Switch(new PickListHandler(), mapping);
            }
        }
    }
}
