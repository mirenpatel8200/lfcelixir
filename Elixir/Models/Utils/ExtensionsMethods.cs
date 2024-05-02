using System;
using System.Data.OleDb;

namespace Elixir.Models.Utils
{
    public static class ExtensionsMethods
    {
        public static TValue GetTableValue<TValue>(this OleDbDataReader dataReader, int rowNumber)
        {
            return dataReader.IsDBNull(rowNumber) ? default(TValue) : (TValue)dataReader.GetValue(rowNumber);
        }

        //public static string Clean(this string query)
        //{
        //    char[] toRemove = new char[] { '\'', '"', '<', '>', '&', ',', ';', '.' };
            
        //    string[] temp = query.Split(toRemove, StringSplitOptions.RemoveEmptyEntries);
        //    query = string.Join("", temp);

        //    query = query.Replace("-", " ");

        //    return query;
        //}
        public static string CleanAndMarkPhrases(this string query)
        {
            int quoteStart = -1, quoteEnd = -1;
            char[] toRemove = new char[] { '\'', '<', '>', '&', ',', ';', '.' };

            string[] temp = query.Split(toRemove, StringSplitOptions.RemoveEmptyEntries);
            query = string.Join("", temp);

            quoteStart = query.IndexOf("\"");
            if (quoteStart >= 0)
                quoteEnd = query.IndexOf("\"", quoteStart + 1);
            if (quoteEnd < 0 && quoteStart >= 0)
                throw new Exception("Url has some invalid format - a quote was started but was not closed");
            if (quoteStart < 0 && quoteEnd < 0)
                return query;
            while (quoteStart >= 0 && quoteEnd >= 0)
            {
                string inQuotes = query.Substring(quoteStart + 1,
                    quoteEnd - quoteStart - 1);
                inQuotes = inQuotes.Replace(" ", "+");

                string beforeQuotes = query.Substring(0, quoteStart);
                string afterQuotes = query.Substring(quoteEnd + 1);

                query = $"{beforeQuotes} {inQuotes} {afterQuotes}";

                quoteStart = query.IndexOf("\"", quoteEnd + 1);
                quoteEnd = query.IndexOf("\"", quoteStart + 1);
            }
            return query;
        }
    }
}