using System.Collections.Generic;
using System.Collections.Specialized;

namespace BasicBilling.API.Extensions
{
    internal static class NameValueCollectionExtensions
    {
        public static Dictionary<string, string> ToStringDictionary(this NameValueCollection value)
        {
            var dict = new Dictionary<string, string>();

            if (value != null)
                foreach (var key in value.AllKeys)
                    dict.Add(key, value[key]);

            return dict;
        }
    }
}
