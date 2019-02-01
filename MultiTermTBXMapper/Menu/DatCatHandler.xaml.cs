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

        public DatCatHandler(string filename)
        {
            InitializeComponent();

            this.filename = filename;
            Globals.filename = filename;

            mapControl.ListBoxItems += value => setMapping(value);

            loadDatCats();
            indexes_visited = new string[datcats.Count];

            cycleIndexes();

            display();
        }

        private void cycleIndexes()
        {
            primary_canvas.Visibility = Visibility.Hidden;
            for (int i = 0; i < indexes_visited.Length; i++)
            {
                Methods.incrementIndex(ref index, indexes_visited.Length);
                display();
            }
            index = 0;
            display();
            primary_canvas.Visibility = Visibility.Visible;
        }

        private void loadDatCats()
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Ignore;
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
                        if (!Methods.inList(ref datcats, dc))
                        {
                            datcats.Add(dc);
                            mapping.Add(dc);

                            switch(level)
                            {
                                case 1:
                                    mapping.levelMap["conceptGrp"].Add(dc);
                                    break;
                                case 2:
                                    mapping.levelMap["languageGrp"].Add(dc);
                                    break;
                                case 3:
                                    mapping.levelMap["termGrp"].Add(dc);
                                    break;
                            }
                        }

                        //Pull out text for use later with picklists
                        XmlReader textReader = reader.ReadSubtree();

                        while (textReader.Read())
                        {
                            if (textReader.NodeType == XmlNodeType.Text)
                            {
                                List<string> values = mapping.getContentList(dc) as List<string>;
                                if (!Methods.inList(ref values, reader.Value))
                                {
                                    mapping.getContentList(dc).Add(textReader.Value);
                                }
                            }
                        }
                    }

                }
            }

            datcats.Sort();
            textTotal.Text = datcats.Count().ToString();
        }

        private void display()
        {
            lbl_user_dc.Content = datcats[index];
            mapControl.clear();

            if (mapping.getTBXMappingList(datcats[index])?.Count > 0)
            {
                mapControl.fillItems(mapping.getTBXMappingList(datcats[index]));
            }

            if(!Methods.inArray(ref indexes_visited,index.ToString()))
            {
                mapControl.findBest(datcats[index]);
                indexes_visited[index] = index.ToString();
            }

            updateCounter();
        }

        private void updatePercentage()
        {
            double mapped = 0;

            foreach (string key in mapping.Keys)
            {
                if (mapping.getTBXMappingList(key)?.Count > 0)
                {
                    mapped++;
                }
            }

            double percent = Math.Round(mapped * 100 / (double)mapping.Keys.Count(), 2, MidpointRounding.AwayFromZero);

            submit.IsEnabled = (percent == 100) ? true : false; 

            textPercent.Text = percent.ToString() + "%";
        }

        private void updateCounter()
        {
            textIndex.Text = (index+1).ToString();
        }

        private void setMapping(TBXMappingList datcat_tbx)
        {
            mapping.setTBXMappingList(datcats[index], datcat_tbx );
            updatePercentage();
        }

        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Methods.incrementIndex(ref index, datcats.Count);
            display();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Methods.decrementIndex(ref index);
            display();
        }

        private List<string> getDatCatsWithMultiMap()
        {
            List<string> dcs_with_multi_map = new List<string>();

            foreach (string key in mapping.Keys)
            {
                if (mapping.getTBXMappingList(key)?.Count > 1)
                {
                    dcs_with_multi_map.Add(key);
                }
            }

            return dcs_with_multi_map;
        }


        private void submit_Click(object sender, RoutedEventArgs e)
        {
            List<string> dcs_with_multi_map = getDatCatsWithMultiMap();

            if (dcs_with_multi_map.Count > 0)
            {
                Switcher.Switch(new VariantPicklistHandler(ref mapping, dcs_with_multi_map));
            }
            else
            {
                Switcher.Switch(new PickListHandler(ref mapping));
            }
        }
    }
}
