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
        private  string connectionstring;

        public string DbLocation { get; private set; }

        public DataService Setup(string DBLocation = null, List<string> commands = null)
        {
            DBLocation = string.IsNullOrWhiteSpace(DBLocation) ? "cachedb.sqlite".ToCurrentPath() : DBLocation;
            DbLocation = DBLocation;
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
                    return this;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            throw new ArgumentException("check the file path / Connection.");

        }

        public IEnumerable<T> Query<T>(string query, object param = null, int timeout = 0)
        {
            using (var con = new SQLiteConnection(connectionstring))
            {
                if (timeout > 0) con.DefaultTimeout = timeout;
                return con.Query<T>(query, param);
            }
        }
        public T FirstOrDefault<T>(string query, object param = null, int timeout = 0)
        {
            using (var con = new SQLiteConnection(connectionstring))
            {
                if (timeout > 0) con.DefaultTimeout = timeout;
                return con.QueryFirstOrDefault<T>(query, param);
            }
        }
        public bool Execute(string query, object param = null, int timeout = 0)
        {
            using (var con = new SQLiteConnection(connectionstring))
            {
                if (timeout > 0) con.DefaultTimeout = timeout;
                return con.Execute(query, param) == 1;
            }
        }

        public int InsertBatch(string query, IEnumerable<Dictionary<string, object>> items, int timeout = 0)
        {
            var executed = 0;

            if (items.Count() == 0 || items.Sum(c => c.Count) == 0) return 0;

            using (var connection = new SQLiteConnection(connectionstring))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                using (var command = connection.CreateCommand())
                {
                    command.Prepare();
                    command.CommandText = query;

                    foreach (var item in items)
                    {
                        command.Parameters.Clear();
                        foreach (var dict in item)
                        {

                            var param = command.CreateParameter();
                            param.ParameterName = dict.Key;
                            param.Value = dict.Value;
                            command.Parameters.Add(param);
                        }
                        if (timeout > 0) command.CommandTimeout = timeout;

                        executed += command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
                connection.Close();
            }
            return executed;
        }
    }
}
