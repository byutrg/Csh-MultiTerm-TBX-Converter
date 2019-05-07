using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * This class file contains the classes needed to store gathered input from the user about the termbase
 */

namespace MultiTermTBXMapper
{
    public class MappingDict : Dictionary<string, object[]>
    {
        public Dictionary<string, List<string>> LevelMap { get; set; } = new Dictionary<string, List<string>>() { { "conceptGrp", new List<string>() }, { "languageGrp", new List<string>() }, { "termGrp", new List<string>() } };
        //public Dictionary<string, List<string>> queueBundlingMap = new Dictionary<string, List<string>>() { { "conceptGrp", new List<string>() }, { "languageGrp", new List<string>() }, { "termGrp", new List<string>() } };

        public void Add(string key)
        {
            Add(key, new object[4] { new TBXMappingList(), new UserDatCatContentList(), new TBXContentMap(), new PicklistMap() });
        }

        public bool HasPicklist(string key)
        {
            return (GetPicklistMap(key)?.Keys.Count > 0) ? true : false;
        }

        /// <summary>
        /// Shortcut method to see if a user data category has multiple contents which were assigned to multiple different TBX data categories.
        /// </summary>
        /// <param name="key">The name of the user data category</param>
        /// <returns>Boolean</returns>
        public bool HasSplitContents(string key)
        {
            return (GetTBXContentMap(key)?.Keys.Count > 1) ? true : false;
        }

        public PicklistMap SetPicklistMap(string key, string content_key, string value)
        {
            return GetPicklistMap(key)?.Set(content_key, value);
        }

        public PicklistMap GetPicklistMap(string key)
        {
            return ContainsKey(key) ? this[key][3] as PicklistMap : null;
        }

        public string GetPicklistMapValue(string key, string user_pl)
        {
            return ContainsKey(key) ? (this[key][3] as PicklistMap).Get(user_pl) : null;
        }

        public bool IsTBXMapped()
        {
            foreach (string key in Keys)
            {
                if (!IsKeyMappedToTBX(key))
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsTBXPicklistMapped()
        {
            foreach (string key in Keys)
            {
                if (!IsKeyMappedToTBXPicklist(key))
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsKeyMappedToTBX(string key)
        {
            foreach (string k in GetTBXContentMap(key).Keys)
            {
                if (!GetTBXContentMap(key).IsMappedToTBX(k))
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsKeyMappedToTBXPicklist(string key)
        {
            foreach (string k in GetPicklistMap(key).Keys)
            {
                if (!GetPicklistMap(key).IsMappedToTBXPicklist(k))
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsGroupMappedToTBX(List<string> user_dcs)
        {
            foreach (string dc in user_dcs)
            {
                if(!IsKeyMappedToTBX(dc))
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsGroupMappedToTBXPicklist(List<string> user_dcs)
        {
            foreach (string dc in user_dcs)
            {
                if (!IsKeyMappedToTBXPicklist(dc))
                {
                    return false;
                }
            }

            return true;
        }

        public List<string> GetDCsWithPicklists()
        {
            List<string> dcs = new List<string>();
            TBXDatabase.GetDCsWithPicklists().ForEach(delegate (string tbx_dc) {
                
                foreach(string user_dc in Keys)
                {
                    List<string> tml = GetTBXMappingList(user_dc);
                    if (tml.Contains(tbx_dc))
                    {
                        dcs.Add(user_dc);
                    }
                }
            });

            return dcs;
        }


        public List<string> GetDCsWithContent()
        {
            List<string> dcs = new List<string>();

            foreach (string key in Keys)
            {
                if(GetContentList(key)?.Count > 0)
                {
                    dcs.Add(key);
                }
            }

            return dcs;
        }

        public TBXMappingList GetTBXMappingList(string key)
        {
            return (Count > 0) ? this[key][0] as TBXMappingList : null;
        }

        public MappingDict SetTBXMappingList(string key, TBXMappingList value = null)
        {
            if (ContainsKey(key))
            {
                this[key][0] = value ?? new TBXMappingList();
            }
            else
            {
                Add(key);
            }
            return this;
        }
        

        /// <summary>
        /// Adds unique content values to the content list
        /// </summary>
        /// <param name="key">user data category</param>
        /// <param name="value">content value</param>
        /// <returns></returns>
        public MappingDict SetContentList(string key, UserDatCatContentList value)
        {
            if (ContainsKey(key))
            {
                if (!Keys.Contains(key))
                {
                    this[key][1] = value;
                }
            }
            else
            {
                Add(key);
            }

            return this;
        }

        public UserDatCatContentList GetContentList(string key)
        {
            return ContainsKey(key) ? this[key][1] as UserDatCatContentList : null;
        }

        public MappingDict AddContentMap(string dc_key, string content_key, string tbx_dc = null)
        {
            if (ContainsKey(dc_key))
            {
                if (GetTBXContentMap(dc_key).ContainsKey(content_key))
                {
                    GetTBXContentMap(dc_key)?.Add(content_key, tbx_dc);
                }
            }

            return this;
        }

        /// <summary>
        /// A shortcut to setting the content of a specific TBXContentMap.  Same as getting a specific TBXContentMap with a key and calling its "Set" method.
        /// </summary>
        /// <param name="dc_key">The name of the user data category</param>
        /// <param name="content_key">A content value of a certain user data category</param>
        /// <param name="tbx_dc">The TBX data category to which the content will be mapped</param>
        /// <returns>The MappingDict instance</returns>
        public MappingDict SetTBXContentMap(string dc_key, string content_key, string tbx_dc = null)
        {
            if (ContainsKey(dc_key))
            {
                if (GetTBXContentMap(dc_key).ContainsKey(content_key))
                {
                    GetTBXContentMap(dc_key)?.Set(content_key, tbx_dc);
                }
                else
                {
                    GetTBXContentMap(dc_key)?.Add(content_key, tbx_dc);
                }
            }

            return this;
        }

        public TBXContentMap GetTBXContentMap(string key)
        {
            return ContainsKey(key) ? this[key][2] as TBXContentMap: null;
        }
    }

    /// <summary>
    /// A list of TBX data categories to which the User's data category from MultiTerm will be mapped.
    /// </summary>
    public class TBXMappingList : List<string> {}

    /// <summary>
    /// UserDatCatContentList is a list of all of the values associated with a User data category from MultiTerm.
    /// </summary>
    public class UserDatCatContentList : List<string> {}

    /// <summary>
    /// <para>
    /// TBXContentMap is a dictionary where the keys correspond to the content values in a user's termbase, and the values are the TBX data categories to which thoses content values will be mapped.
    /// </para>
    /// <para>
    /// It is most common that all conents will be mapped to the same TBX DC, but sometimes different content values may belong to different DCs.
    /// </para>
    /// <para>
    /// Example:
    /// <code>
    ///     {
    ///         'noun' : 'partOfSpeech'
    ///     }
    /// </code>
    /// </para>
    /// </summary>
    public class TBXContentMap : Dictionary<string, string>
    {
        public bool IsMappedToTBX(string key)
        {
            if (ContainsKey(key) && this[key] != null)
            {
                return true;
            }

            return false;
        }

        public TBXContentMap Set(string key, string tbx_dc)
        {
            if (ContainsKey(key))
            {
                this[key] = tbx_dc;
            }
            else
            {
                Add(key, tbx_dc);
            }

            return this;
        }

        public string Get(string key)
        {
            return ContainsKey(key) ? this[key] : null;
        }
    }

    /// <summary>
    /// PicklistMap is dictionary mapping of User data category content values to the appropriate corresponding TBX data category value.
    /// <para>
    /// Example: <code>{ 'preferred' : 'preferredTerm-admn-sts' }</code>
    /// </para>
    /// </summary>
    public class PicklistMap : Dictionary<string, string>
    {
        public bool IsMappedToTBXPicklist(string key)
        {
            if (ContainsKey(key) && this[key] != null)
            {
                return true;
            }

            return false;
        }

        public PicklistMap Set(string key, string tbx_dc)
        {
            if (ContainsKey(key))
            {
                this[key] = tbx_dc;
            }
            else
            {
                Add(key, tbx_dc);
            }

            return this;
        }

        public string Get(string key)
        {
            return ContainsKey(key) ? this[key] : null;
        }
    }
}
