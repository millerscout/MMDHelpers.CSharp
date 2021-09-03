using Dapper;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace MMDHelpers.CSharp.LocalDB
{
    public class SQLLocalDB
    {
        /// <summary>
        /// Create a new instance 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <param name="SqlWhenCreating"></param>
        /// <returns>the nearly connectionstring</returns>
        public static string CreateInstance(string name, string path, string SqlWhenCreating, string instance = "Data Source=(LocalDB)\\MSSQLLocalDB;Integrated Security=True")
        {
            String str;
            SqlConnection myConn = new SqlConnection(instance);
            bool created = File.Exists(Path.Combine(path, name)) ? false : true;
            var caminhoBanco = Path.Combine(path, $"{name}.mdf");

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(instance);

            var sttConn = $"Server={builder.DataSource}; Integrated Security={builder.IntegratedSecurity}; AttachDbFileName={caminhoBanco};";

            if (created)
            {

                str = $"CREATE DATABASE {name} ON PRIMARY " +
                 $"(NAME = {name}_Data, " +
                 $"FILENAME = '{path}\\{name}.mdf', " +
                 "SIZE = 4MB, MAXSIZE = 10GB, FILEGROWTH = 10%)" +
                 $"LOG ON (NAME = {name}_Log, " +
                 $"FILENAME = '{path}\\{name}', " +
                 "SIZE = 10MB, " +
                 "MAXSIZE = 150MB, " +
                 "FILEGROWTH = 10%)";

                SqlCommand myCommand = new SqlCommand(str, myConn);
                try
                {
                    myConn.Open();
                    myCommand.ExecuteNonQuery();

                    //MessageBox.Show("DataBase is Created Successfully", "MyProgram", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (System.Exception ex)
                {
                    //MessageBox.Show(ex.ToString(), "MyProgram", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                finally
                {
                    if (myConn.State == ConnectionState.Open)
                    {
                        myConn.Close();
                    }
                }
            }
            if (created)
            {
                using (var con = new SqlConnection(sttConn))
                {
                    con.Execute(SqlWhenCreating);
                }
            }

            return sttConn;
        }
    }
}
