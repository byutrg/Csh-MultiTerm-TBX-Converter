using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace MultiTermTBXMapper
{
    public static class Methods
    {
        public static void RemoveListBoxItems(ref ListBox lb)
        {
            if (lb.Items.Count > 0)
            {
                lb.Items.RemoveAt(lb.Items.Count - 1);
                RemoveListBoxItems(ref lb);
            }
        }

        public static void DecrementIndex(ref int index)
        {
            if (index > 0)
            {
                index--;
            }
        }

        public static void IncrementIndex(ref int index, int max)
        {
            if (index + 1 < max)
            {
                index++;
            }
        }

        public static string[] GetKeyArray(Dictionary<string, List<string[]>>.KeyCollection keyCol)
        {
            string[] keys = new string[keyCol.Count];
            int i = 0;
            foreach (string key in keyCol)
            {
                keys[i] = key;
                i++;
            }

            return keys;
        }

        public static string[] GetKeyArray(Dictionary<string, Dictionary<string, string>>.KeyCollection keyCol)
        {
            string[] keys = new string[keyCol.Count];
            int i = 0;
            foreach(string key in keyCol)
            {
                keys[i] = key;
                i++;
            }
            
            return keys;
        }

        public static string RgxReplace(string pattern, string replacement, string input)
        {
            Regex rgx = new Regex(pattern);
            return rgx.Replace(input, replacement);
        }

    }
}
