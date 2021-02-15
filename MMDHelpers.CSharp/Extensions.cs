﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Reflection;

namespace MMDHelpers.CSharp.Extensions
{
    public static class Extensions
    {
        public static string ToCurrentPath(this string path) => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), path);
        public static void WriteToFile(this string Path, List<string> logs)
        {
            using (FileStream file = new FileStream(Path, FileMode.Append, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(file))
            {
                foreach (var message in logs)
                {
                    sw.WriteLine(message);
                }

            }
        }
        /// <summary>
        /// Helper to transform an IEnumerable to Datatable, Useful to Bulk insert 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DataTable AsDataTable<T>(this IEnumerable<T> data)
        {
            var properties = TypeDescriptor.GetProperties(typeof(T));
            var table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }
    }
}
