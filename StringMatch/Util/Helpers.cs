using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringMatch
{
    using System.IO;
    using LumenWorks.Framework.IO.Csv;
    using System.Data;

    public static class CsvHelpers
    {
        public static DataTable ReadCsv(string path)
        {
            DataTable csvTable = new DataTable();
            // open the file "data.csv" which is a CSV file with headers
            using (CsvReader csvReader = new CsvReader(
                                   new StreamReader(path), true))
            {
                csvTable.Load(csvReader);
            }
            return csvTable;

        }


        public static string ToCsv( this DataTable dt)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                int count = 1;
                int totalColumns = dt.Columns.Count;
                foreach (DataColumn dr in dt.Columns)
                {
                    sb.Append(dr.ColumnName);

                    if (count != totalColumns)
                    {
                        sb.Append(",");
                    }

                    count++;
                }

                sb.AppendLine();

                string value = String.Empty;
                foreach (DataRow dr in dt.Rows)
                {
                    for (int x = 0; x < totalColumns; x++)
                    {
                        value = dr[x].ToString();

                        if (value.Contains(",") || value.Contains("\""))
                        {
                            value = '"' + value.Replace("\"", "\"\"") + '"';
                        }

                        sb.Append(value);

                        if (x != (totalColumns - 1))
                        {
                            sb.Append(",");
                        }
                    }

                    sb.AppendLine();
                }
            }
            catch (Exception ex)
            {
                // Do something
            }

            return sb.ToString();
        }


    }
}
