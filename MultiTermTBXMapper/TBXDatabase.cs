using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;

namespace MultiTermTBXMapper
{
    class TBXDatabase
    {
        protected static SQLiteConnection con;

        public static void Initialize()
        {
            con = new SQLiteConnection("Data Source=tbx_datcats.sqlite;Version=3;");
            con.Open();

            if (IsDBEmpty())
            {
                BuildDB();
            }
        }

        private static bool IsDBEmpty()
        {
            bool ret = false;

            string count_tables = "select count (*) from categories";

            SQLiteCommand command = new SQLiteCommand(count_tables, con);

            try
            {
                SQLiteDataReader results = command.ExecuteReader();
            }
            catch (SQLiteException e) { ret = true; }


            return ret;
        }

        private static void BuildDB()
        {
            string script = File.ReadAllText(@"Resources/tbx_data_categories_with_definitions_picklists.sql");
            SQLiteCommand command = new SQLiteCommand(script, con);
            command.ExecuteNonQuery();
        }

        public static void close()
        {
            con.Close();
        }

        public static void open()
        {
            con.Open();
        }

        public static List<string[]> getAll()
        {
            string query = "select name,element,description from categories";

            SQLiteCommand command = new SQLiteCommand(query, con);

            SQLiteDataReader reader = command.ExecuteReader();

            List<string[]> datcats = new List<string[]>();

            while (reader.Read())
            {
                string[] dc = new string[3] { (string)reader["name"], (string)reader["element"], (string)reader["description"] };
                datcats.Add(dc);
            }

            return datcats;
        }

        public static List<string[]> getNames()
        {
            string query = "select name from categories";

            SQLiteCommand command = new SQLiteCommand(query, con);

            SQLiteDataReader reader = command.ExecuteReader();

            List<string[]> datcats = new List<string[]>();

            while (reader.Read())
            {
                string[] dc = new string[1] { (string)reader["name"] };
                datcats.Add(dc);
            }

            return datcats;
        }

        public static Dictionary<string, List<string[]>> getPicklists()
        {
            string query = "select categories.name, picklists.value, picklists.description from picklists left join categories on picklists.category_id = categories.id";

            SQLiteCommand command = new SQLiteCommand(query, con);

            SQLiteDataReader reader = command.ExecuteReader();

            Dictionary<string, List<string[]>> picklists = new Dictionary<string, List<string[]>>();

            while (reader.Read())
            {
                string n = reader["name"].ToString();
                string v = reader["value"].ToString();
                string d = reader["description"].ToString();

                if (picklists.Keys.Contains(n))
                {
                    picklists[n].Add(new string[2] { v, d });
                }
                else
                {
                    picklists.Add(n, new List<string[]> { new string[2] { v, d } });
                }
            }

            return picklists;
        }

        public static List<string> getDCsWithPicklists()
        {
            string query = "select categories.name from picklists left join categories on picklists.category_id = categories.id";

            SQLiteCommand command = new SQLiteCommand(query, con);
            SQLiteDataReader reader = command.ExecuteReader();

            List<string> tbx_dcs = new List<string>();

            while (reader.Read())
            {
                if (!Methods.inList(ref tbx_dcs, reader["name"].ToString()))
                {
                    tbx_dcs.Add(reader["name"].ToString());
                }
            }

            return tbx_dcs;
        }

        public static Dictionary<string, Dictionary<string, string>> getDCInfo()
        {
            string query = "select name, element, level from categories";

            SQLiteCommand command = new SQLiteCommand(query, con);
            SQLiteDataReader reader = command.ExecuteReader();

            Dictionary<string, Dictionary<string, string>> dcInfo = new Dictionary<string, Dictionary<string, string>>();

            while(reader.Read())
            {
                if (reader["element"].ToString() == null || reader["element"].ToString() == "")
                {
                    continue;
                }

                Dictionary<string, string> info = new Dictionary<string, string>()
                {
                    { "element", reader["element"].ToString() },
                    { "level", reader["level"].ToString() }
                };

                dcInfo.Add(reader["name"].ToString(), info);
            }

            return dcInfo;
        }
    }
}
