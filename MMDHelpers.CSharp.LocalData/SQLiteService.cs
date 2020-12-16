using Dapper;
using MMDHelpers.CSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace MMDHelpers.CSharp.LocalData
{
    public class DataService
    {
        private static string connectionstring;

        public static string DbLocation { get; private set; }

        public static void Setup(string DBLocation = null, List<string> commands = null)
        {
            DBLocation = string.IsNullOrWhiteSpace(DBLocation) ? "cachedb.sqlite".ToCurrentPath() : DBLocation;
            connectionstring = $"Data Source={DbLocation}; Version=3;";
            if (!File.Exists(DbLocation))
            {
                try
                {
                    SQLiteConnection.CreateFile(DbLocation);

                    if (commands != null && commands.Any())
                    {
                        using (var conn = new SQLiteConnection(connectionstring))
                        using (var cmd = conn.CreateCommand())
                        {
                            conn.Open();

                            foreach (var item in commands)
                            {
                                cmd.CommandText = item;
                                cmd.ExecuteNonQuery();
                            }

                            conn.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public IEnumerable<T> Query<T>(string query, object param = null)
        {
            using (var con = new SQLiteConnection(connectionstring))
                return con.Query<T>(query, param);
        }
        public T FirstOrDefault<T>(string query, object param = null)
        {
            using (var con = new SQLiteConnection(connectionstring))
                return con.QueryFirstOrDefault<T>(query, param);
        }
        public bool Execute(string query, object param = null)
        {
            using (var con = new SQLiteConnection(connectionstring))
                return con.Execute(query, param) == 1;
        }
    }
}
