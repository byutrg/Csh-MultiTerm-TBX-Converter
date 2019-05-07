using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace MultiTermTBXMapper.Menu
{
    /// <summary>
    /// Interaction logic for VariantHandler.xaml
    /// </summary>
    public partial class ConversionHandler : UserControl, ISwitchable
    {
        private Mapping fullMapping = new Mapping();
        private MappingDict mappingDict;
        private Dictionary<string, Dictionary<string, string>> tbxInfo = TBXDatabase.GetDCInfo();
        private Dictionary<string, List<string[]>> tbx_picklists = TBXDatabase.GetPicklists();

        private class InvalidLevelException : Exception
        {
            public InvalidLevelException() : base("Only levels 'concept', 'language', and 'term' are allowed as group values.") { }
        }

        #region ISwitchable members
        public void UtilizeState<T>(T state)
        {
            ConverterApp mt2tbx = new ConverterApp();
            var (outputFile, errorFile) = mt2tbx.DeserializeFile(state as string, Singleton.Instance.getPath(), Singleton.Instance.getDialect(), false);
            FinishedConversion(outputFile, errorFile);
        }

        //public void UtilizeState<T>(ref T r)
        //{
        //    throw new NotImplementedException();
        //}

        public void UtilizeState<T1, T2>(T1 r, T2 state)
        {
            mappingDict = r as MappingDict;

            Map();

            fullMapping.QueueDrainOrders = state as QueueDrainOrders;

            //Newtonsoft.Json.JsonSerializerSettings settings = new Newtonsoft.Json.JsonSerializerSettings();

            //string json = Newtonsoft.Json.JsonConvert.SerializeObject(fullMapping, settings);
            //json = fixJSON(json);


            string json = fullMapping.Serialize();

            string mappingFile = Path.GetTempFileName();

            int saveOption = Singleton.Instance.getSaveOption();

            if (saveOption == 1) // Mapping and Conversion
            {
                File.WriteAllText(mappingFile, json);

                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
                { 
                    DefaultExt = ".json",
                    Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*"
                };

                bool? result = dlg.ShowDialog();

                if (result == true)
                {
                    if (File.Exists(dlg.FileName))
                    {
                        File.Delete(dlg.FileName);
                    }

                    //File.WriteAllText(dlg.FileName, tbxOutput);
                    //File.Move(tbxOutput.Replace("/", "\\"), dlg.FileName);
                    File.Move(mappingFile, dlg.FileName);
                }

                ConverterApp mt2tbx = new ConverterApp();
                var (outputFile, errorFile) = mt2tbx.DeserializeFile(json, Singleton.Instance.getPath(), Singleton.Instance.getDialect(), true);
                FinishedConversion(outputFile, errorFile);
            }
            else if (saveOption == 2) // Will be handled in a different Dialogue
            {
                throw new Exception("Mapping not handled!!!");
            }
            else if (saveOption == 3) // Just Mapping File
            {
                File.WriteAllText(mappingFile, json);

                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
                {
                    DefaultExt = ".json",
                    Filter = "JSON Files (*.json)|*.json|All files (*.*)|*.*"
                };

                bool? result = dlg.ShowDialog();

                if (result == true)
                {
                    if (File.Exists(dlg.FileName))
                    {
                        File.Delete(dlg.FileName);
                    }

                    //File.WriteAllText(dlg.FileName, tbxOutput);
                    //File.Move(tbxOutput.Replace("/", "\\"), dlg.FileName);
                    File.Move(mappingFile, dlg.FileName);
                }
                FinishedConversion(dlg.FileName);
            }
        }
        #endregion

        public ConversionHandler()
        {
            InitializeComponent();
        }

        private void FinishedConversion(string outputFile)
        {
            textblock_conversionStatus.Text = $"Your mapping file has been created and can be found:\n {outputFile}";
        }

        private void FinishedConversion(string outputFile, string errorFile)
        {
            textblock_conversionStatus.Text = $"Your converted file has been created and can be found:\n {outputFile}\n\nExceptions to the conversion can be found:\n {errorFile}";
        }

        private void Map()
        {
            foreach (string dc in mappingDict.Keys)
            {
                if (IsConceptGrp(dc))
                {
                    HandleGrp(dc, "concept");
                }

                if (IsLanguageGrp(dc))
                {
                    HandleGrp(dc, "language");
                }

                if (IsTermGrp(dc))
                {
                    HandleGrp(dc, "term");
                }
            }


        }

        private static string FixJSON(string json)
        { 
            //json = Methods.rgxReplace("^\\{", "[", json);
            //json = Methods.rgxReplace("\\}(?!.+?)", "]", json);
            //json = Methods.rgxReplace("\\\"(dialect|xcs|catMap|empty)\\\":", "", json);
            //json = Methods.rgxReplace("\\{\\\"teasp\\\":", "", json);
            //json = Methods.rgxReplace("\\]\\}\\]\\]", "]]]", json);
            return json;
        }

        /// <summary>
        /// Handles a level.
        /// </summary>
        /// <param name="dc">User data category name</param>
        /// <param name="group">concept|language|term</param>
        private void HandleGrp(string dc, string group)
        {
            if (group != "concept" && group != "language" && group != "term")
            { 
                throw (new InvalidLevelException());
            }

            TemplateSet ts;

            
            //Handle data category with split contents and picklists
            if (mappingDict.HasPicklist(dc) && mappingDict.HasSplitContents(dc))
            {
                ts = CreateTemplateSet(dc, true);

                (List<ValueGroup> vgs, List<Teasp> teasps) = HandleSplitContents(dc, true);
                ts.AddValueGroups(vgs);
                ts.AddSpecialTeasps(teasps);
            }
            //Handle single tbx datcat with picklists
            else if (mappingDict.HasPicklist(dc) && !mappingDict.HasSplitContents(dc))
            {
                string tbx_dc = GetTBXdc(dc);
                ts = CreateTemplateSet(dc, tbx_dc);
            }
            //Handle data category with split contents (no picklists)
            else if (!mappingDict.HasPicklist(dc) && mappingDict.HasSplitContents(dc))
            {
                ts = CreateTemplateSet(dc, true);

                (List<ValueGroup> vgs, List<Teasp> teasps) = HandleSplitContents(dc);
                ts.AddValueGroups(vgs);
                ts.AddSpecialTeasps(teasps);
            }
            //Handle all simple cases: no multiple tbx datcats, no picklists
            else
            {
                ts = CreateTemplateSet(dc);
            }


            fullMapping.CatMap[group].Add(dc, ts);
        }

        /// <summary>
        /// Method for creating of list of value groups and their corresponding special teasps.
        /// </summary>
        /// <param name="dc">user data category</param>
        /// <returns>List of Value Groups</returns>
        private (List<ValueGroup>, List<Teasp>) HandleSplitContents(string dc, bool hasPicklist = false)
        {
            TBXContentMap cm = mappingDict.GetTBXContentMap(dc);
            string[] tbx_keys = Methods.GetKeyArray(tbx_picklists.Keys);

            List<ValueGroup> vgs = new List<ValueGroup>();
            List<Teasp> teasps = new List<Teasp>();

            foreach (string tbx_dc in mappingDict.GetTBXMappingList(dc))
            {
                if (Array.Exists(tbx_keys, item => item == tbx_dc))
                {
                    Teasp teasp = new Teasp();

                    (string target, string eltAtt) = GetEltTgtEltAtt(tbx_dc);

                    if (hasPicklist)
                    {
                        teasp.SetAll(target, eltAtt, mappingDict.GetPicklistMap(dc));
                    }
                    else
                    {
                        teasp.SetAll(target, eltAtt);
                    }

                    ValueGroup vg = new ValueGroup();
                    
                    foreach (string content in cm.Keys)
                    {
                        if (cm[content] == tbx_dc)
                        {
                            vg.Add(content);
                        }
                    }
                    vgs.Add(vg);
                    teasps.Add(teasp);
                }
                else
                {
                    Teasp teasp = new Teasp();
                    (string target, string eltAtt) = GetEltTgtEltAtt(tbx_dc);
                    teasp.SetAll(target, eltAtt);
                    ValueGroup vg = new ValueGroup();
                    foreach(string content in cm.Keys)
                    {
                        if (cm[content] == tbx_dc)
                        {
                            vg.Add(content);
                        }
                    }
                    vgs.Add(vg);
                    teasps.Add(teasp);
                }
            }

            return (vgs, teasps);
        }

        /// <summary>
        /// Gets the mapped TBX data category for a specific content value if provided, or the first (and should be only, if there is no content mapping) TBX data category for the user data category.
        /// </summary>
        /// <param name="dc">Name of the User Data Category</param>
        /// <param name="content_key">Optional Content of Data Category (default = null)</param>
        /// <returns>TBX data category name</returns>
        private string GetTBXdc(string dc, string content_key = null)
        {
            string tbx_dc = null;

            if (content_key == null)
            {
                tbx_dc = mappingDict.GetTBXMappingList(dc)?[0];
            }
            else
            {
                tbx_dc = mappingDict.GetTBXContentMap(dc)?[content_key];
            }

            return tbx_dc;
        }

        /// <summary>
        /// Create a default Template Set, no Picklist values. Unhandled is used for both actual unhandled cases and split content mappings.
        /// </summary>
        /// <param name="user_dc">User data category name</param>
        /// <param name="tbx_dc">TBX data category name</param>
        /// <param name="unhandled">Is this an unhandled data category</param>
        /// <returns></returns>
        private TemplateSet CreateTemplateSet(string user_dc, bool unhandled = false)
        {
            (string target, string eltAtt, TemplateSet ts) = ("", "", new TemplateSet());

            string tbx_dc = (unhandled) ? "unhandled" : GetTBXdc(user_dc);

            (target, eltAtt) = GetEltTgtEltAtt(tbx_dc);
            ((ts[0] as KeyList)[0] as Teasp).SetAll(target, eltAtt);

            return ts;
        }

        /// <summary>
        /// Create a Template Set with Picklist Values
        /// </summary>
        /// <param name="user_dc">User data category name</param>
        /// <param name="tbx_dc">User data category name</param>
        /// <returns>Template Set with Picklist values</returns>
        private TemplateSet CreateTemplateSet(string user_dc, string tbx_dc)
        {
            (string target, string eltAtt) = GetEltTgtEltAtt(tbx_dc);

            TemplateSet ts = new TemplateSet();
            Dictionary<string, string> sub = new Dictionary<string, string>();

            TBXContentMap cm = mappingDict.GetTBXContentMap(user_dc);
            PicklistMap pm = mappingDict.GetPicklistMap(user_dc);

            foreach (string content in cm?.Keys)
            {
                if (cm[content] == tbx_dc)
                {
                    sub.Add(content, pm[content]);
                }
            }

            ((ts[0] as KeyList)[0] as Teasp).SetAll(target, eltAtt, sub);
            return ts;
        }

        private (string, string) GetEltTgtEltAtt(string tbx_dc)
        {
            string elt = "";
            if (tbx_dc == "unhandled" || tbx_dc == "None: Do Not Map")
            {
                elt = "<unhandled>";
            }
            else
            {
                elt = tbxInfo[tbx_dc]?["element"];
            }

            string target = fullMapping.GetTarget(elt);
            string eltAtt = fullMapping.GetEltAtt(elt, tbx_dc);

            return (target, eltAtt);
        }

        private bool IsConceptGrp(string dc)
        {
            List<string> grp = mappingDict.LevelMap["conceptGrp"];
            return (grp.Contains(dc)) ? true : false;
        }

        private bool IsLanguageGrp(string dc)
        {
            List<string> grp = mappingDict.LevelMap["languageGrp"];
            return (grp.Contains(dc)) ? true : false;
        }

        private bool IsTermGrp(string dc)
        {
            List<string> grp = mappingDict.LevelMap["termGrp"];
            return (grp.Contains(dc)) ? true : false;   
        }
    }
}
