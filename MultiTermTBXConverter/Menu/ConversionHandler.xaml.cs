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
        private Dictionary<string, Dictionary<string, string>> tbxInfo = TBXDatabase.getDCInfo();
        private Dictionary<string, List<string[]>> tbx_picklists = TBXDatabase.getPicklists();

        private class InvalidLevelException : Exception
        {
            public InvalidLevelException() : base("Only levels 'concept', 'language', and 'term' are allowed as group values.") { }
        }

        #region ISwitchable members
        public void UtilizeState<T>(T state)
        {
            ConverterApp mt2tbx = new ConverterApp();
            var (outputFile, errorFile) = mt2tbx.deserializeFile(state as string, Singleton.Instance.getPath(), Singleton.Instance.getDialect(), false);
            FinishedConversion(outputFile, errorFile);
        }

        public void UtilizeState<T>(ref T r)
        {
            throw new NotImplementedException();
        }

        public void UtilizeState<T1, T2>(ref T1 r, T2 state)
        {
            mappingDict = r as MappingDict;

            map();

            fullMapping.queueDrainOrders = state as QueueDrainOrders;

            //Newtonsoft.Json.JsonSerializerSettings settings = new Newtonsoft.Json.JsonSerializerSettings();

            //string json = Newtonsoft.Json.JsonConvert.SerializeObject(fullMapping, settings);
            //json = fixJSON(json);


            string json = fullMapping.Serialize();

            string mappingFile = Path.GetTempFileName();

            int saveOption = Singleton.Instance.getSaveOption();

            if (saveOption == 1) // Mapping and Conversion
            {
                File.WriteAllText(mappingFile, json);

                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

                dlg.DefaultExt = ".json";
                dlg.Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*";

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
                var (outputFile, errorFile) = mt2tbx.deserializeFile(json, Singleton.Instance.getPath(), Singleton.Instance.getDialect(), true);
                FinishedConversion(outputFile, errorFile);
            }
            else if (saveOption == 2) // Will be handled in a different Dialogue
            {
                throw new Exception("Mapping not handled!!!");
            }
            else if (saveOption == 3) // Just Mapping File
            {
                File.WriteAllText(mappingFile, json);

                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

                dlg.DefaultExt = ".json";
                dlg.Filter = "JSON Files (*.json)|*.json|All files (*.*)|*.*";

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

        private void map()
        {
            foreach (string dc in mappingDict.Keys)
            {
                if (isConceptGrp(dc))
                {
                    handleGrp(dc, "concept");
                }

                if (isLanguageGrp(dc))
                {
                    handleGrp(dc, "language");
                }

                if (isTermGrp(dc))
                {
                    handleGrp(dc, "term");
                }
            }


        }

        private static string fixJSON(string json)
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
        private void handleGrp(string dc, string group)
        {
            if (group != "concept" && group != "language" && group != "term")
            { 
                throw (new InvalidLevelException());
            }

            TemplateSet ts;

            
            //Handle data category with split contents and picklists
            if (mappingDict.hasPicklist(dc) && mappingDict.hasSplitContents(dc))
            {
                ts = createTemplateSet(dc, true);

                (List<ValueGroup> vgs, List<Teasp> teasps) = handleSplitContents(dc, true);
                ts.addValueGroups(vgs);
                ts.addSpecialTeasps(teasps);
            }
            //Handle single tbx datcat with picklists
            else if (mappingDict.hasPicklist(dc) && !mappingDict.hasSplitContents(dc))
            {
                string tbx_dc = getTBXdc(dc);
                ts = createTemplateSet(dc, tbx_dc);
            }
            //Handle data category with split contents (no picklists)
            else if (!mappingDict.hasPicklist(dc) && mappingDict.hasSplitContents(dc))
            {
                ts = createTemplateSet(dc, true);

                (List<ValueGroup> vgs, List<Teasp> teasps) = handleSplitContents(dc);
                ts.addValueGroups(vgs);
                ts.addSpecialTeasps(teasps);
            }
            //Handle all simple cases: no multiple tbx datcats, no picklists
            else
            {
                ts = createTemplateSet(dc);
            }


            fullMapping.catMap[group].Add(dc, ts);
        }

        /// <summary>
        /// Method for creating of list of value groups and their corresponding special teasps.
        /// </summary>
        /// <param name="dc">user data category</param>
        /// <returns>List of Value Groups</returns>
        private (List<ValueGroup>, List<Teasp>) handleSplitContents(string dc, bool hasPicklist = false)
        {
            TBXContentMap cm = mappingDict.getTBXContentMap(dc);
            string[] tbx_keys = Methods.getKeyArray(tbx_picklists.Keys);

            List<ValueGroup> vgs = new List<ValueGroup>();
            List<Teasp> teasps = new List<Teasp>();

            foreach (string tbx_dc in mappingDict.getTBXMappingList(dc))
            {
                if (Methods.inArray(ref tbx_keys, tbx_dc))
                {
                    Teasp teasp = new Teasp();

                    (string target, string eltAtt) = getEltTgtEltAtt(tbx_dc);

                    if (hasPicklist)
                    {
                        teasp.setAll(target, eltAtt, mappingDict.getPicklistMap(dc));
                    }
                    else
                    {
                        teasp.setAll(target, eltAtt);
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
                    (string target, string eltAtt) = getEltTgtEltAtt(tbx_dc);
                    teasp.setAll(target, eltAtt);
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
        private string getTBXdc(string dc, string content_key = null)
        {
            string tbx_dc = null;

            if (content_key == null)
            {
                tbx_dc = mappingDict.getTBXMappingList(dc)?[0];
            }
            else
            {
                tbx_dc = mappingDict.getTBXContentMap(dc)?[content_key];
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
        private TemplateSet createTemplateSet(string user_dc, bool unhandled = false)
        {
            (string target, string eltAtt, TemplateSet ts) = ("", "", new TemplateSet());

            string tbx_dc = (unhandled) ? "unhandled" : getTBXdc(user_dc);

            (target, eltAtt) = getEltTgtEltAtt(tbx_dc);
            ((ts[0] as KeyList)[0] as Teasp).setAll(target, eltAtt);

            return ts;
        }

        /// <summary>
        /// Create a Template Set with Picklist Values
        /// </summary>
        /// <param name="user_dc">User data category name</param>
        /// <param name="tbx_dc">User data category name</param>
        /// <returns>Template Set with Picklist values</returns>
        private TemplateSet createTemplateSet(string user_dc, string tbx_dc)
        {
            (string target, string eltAtt) = getEltTgtEltAtt(tbx_dc);

            TemplateSet ts = new TemplateSet();
            Dictionary<string, string> sub = new Dictionary<string, string>();

            TBXContentMap cm = mappingDict.getTBXContentMap(user_dc);
            PicklistMap pm = mappingDict.getPicklistMap(user_dc);

            foreach (string content in cm?.Keys)
            {
                if (cm[content] == tbx_dc)
                {
                    sub.Add(content, pm[content]);
                }
            }

            ((ts[0] as KeyList)[0] as Teasp).setAll(target, eltAtt, sub);
            return ts;
        }

        private (string, string) getEltTgtEltAtt(string tbx_dc)
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

            string target = fullMapping.getTarget(elt);
            string eltAtt = fullMapping.getEltAtt(elt, tbx_dc);

            return (target, eltAtt);
        }

        private bool isConceptGrp(string dc)
        {
            List<string> grp = mappingDict.levelMap["conceptGrp"];
            return (Methods.inList(ref grp, dc)) ? true : false;
        }

        private bool isLanguageGrp(string dc)
        {
            List<string> grp = mappingDict.levelMap["languageGrp"];
            return (Methods.inList(ref grp, dc)) ? true : false;
        }

        private bool isTermGrp(string dc)
        {
            List<string> grp = mappingDict.levelMap["termGrp"];
            return (Methods.inList(ref grp, dc)) ? true : false;   
        }
    }
}
