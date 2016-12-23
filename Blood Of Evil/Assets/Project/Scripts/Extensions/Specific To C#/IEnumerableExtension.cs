using System;
using System.Collections.Generic;

namespace BloodOfEvil.Extensions
{
    /// <summary>
    /// Cette classe vient de Manzalab.
    /// </summary>
    public static class IEnumerableExtension
    {
        public static string ToString<EnumerableContent>(this IEnumerable<EnumerableContent> datas, string separator = null)
        {
            string content = "";

            foreach (var data in datas)
            {
                content += data.ToString();

                if (!string.IsNullOrEmpty(separator))
                    content += separator;
            }

            if (!string.IsNullOrEmpty(separator))
                return content.Substring(0, content.LastIndexOf(separator));

            return content;
        }

        public static int Count<EnumerableContent>(this IEnumerable<EnumerableContent> datas)
        {
            int count = 0;

            foreach (var data in datas)
                ++count;

            return count;
        }

        public static EnumerableContent GetFirstElement<EnumerableContent>(this IEnumerable<EnumerableContent> datas) where EnumerableContent : class
        {
            foreach (var data in datas)
                return data;

            return null;
        }
    }
}