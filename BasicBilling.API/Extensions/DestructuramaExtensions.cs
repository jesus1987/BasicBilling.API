using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;
using Destructurama.Attributed;
using Newtonsoft.Json.Linq;

namespace BasicBilling.API.Extensions
{
    public static class DestructuramaExtensions
    {

        static DestructuramaExtensions()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => assembly?.FullName?.Contains("BasicBilling") ?? false);

            MaskedProperties = assemblies?.SelectMany(assembly => assembly.GetTypes()
                    .SelectMany(x => x.GetProperties())
                    .Where(property => Attribute.GetCustomAttribute(property, typeof(LogMaskedAttribute)) is LogMaskedAttribute)).AsQueryable()
                .DistinctBy(x => x.Name);
        }

        public static IEnumerable<PropertyInfo> MaskedProperties { get; }

        private static string ToCamelCase(string value)
        {
            return $"{char.ToLowerInvariant(value[0])}{value.Substring(1)}";
        }

        public static Dictionary<string, string> ApplyAttributesToDictionary(Dictionary<string, string> dictionary)
        {
            var clonedDictionary = new Dictionary<string, string>(dictionary);

            foreach (var keyValuePair in clonedDictionary)
            {
                var maskedProperty = MaskedProperties.FirstOrDefault(x => x.Name.ToLower() == keyValuePair.Key.ToLower());

                if (maskedProperty == null)
                    continue;

                var attribute = (LogMaskedAttribute)Attribute.GetCustomAttribute(maskedProperty, typeof(LogMaskedAttribute));
                var isDefaultValue = attribute.Text == "***";

                var val = keyValuePair.Value;

                if (string.IsNullOrEmpty(keyValuePair.Value))
                {
                    dictionary[keyValuePair.Key] = keyValuePair.Value.Replace(keyValuePair.Value, val);
                    continue;
                }

                if (attribute.ShowFirst == 0 && attribute.ShowLast == 0)
                {
                    if (attribute.PreserveLength)
                    {
                        dictionary[keyValuePair.Key] = dictionary[keyValuePair.Key].Replace(keyValuePair.Value, new string(attribute.Text[0], val.Length));
                        continue;
                    }

                    dictionary[keyValuePair.Key] = dictionary[keyValuePair.Key].Replace(keyValuePair.Value, attribute.Text);
                    continue;
                }

                if (attribute.ShowFirst > 0 && attribute.ShowLast == 0)
                {
                    var first = val.Substring(0, Math.Min(attribute.ShowFirst, val.Length));

                    if (!attribute.PreserveLength || !isDefaultValue)
                    {
                        dictionary[keyValuePair.Key] = dictionary[keyValuePair.Key].Replace(keyValuePair.Value, first + attribute.Text);
                        continue;
                    }

                    var mask = string.Empty;
                    if (attribute.ShowFirst <= val.Length)
                        mask = new string(attribute.Text[0], val.Length - attribute.ShowFirst);

                    dictionary[keyValuePair.Key] = dictionary[keyValuePair.Key].Replace(keyValuePair.Value, first + mask);
                    continue;
                }

                if (attribute.ShowFirst == 0 && attribute.ShowLast > 0)
                {
                    var last = attribute.ShowLast > val.Length ? val : val.Substring(val.Length - attribute.ShowLast);

                    if (!attribute.PreserveLength || !isDefaultValue)
                    {
                        dictionary[keyValuePair.Key] = dictionary[keyValuePair.Key].Replace(keyValuePair.Value, attribute.Text + last);
                        continue;
                    }

                    var mask = string.Empty;
                    if (attribute.ShowLast <= val.Length)
                        mask = new string(attribute.Text[0], val.Length - attribute.ShowLast);

                    dictionary[keyValuePair.Key] = dictionary[keyValuePair.Key].Replace(keyValuePair.Value, mask + last);
                }
            }

            return dictionary;
        }

        public static string ApplyAttributesToJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return json;

            if (!JsonExtensions.IsJson(json))
                return json;

            json = JToken.Parse(json).ToString();

            var regexTemplate = "(?<=\\s*\"{propertyName}\": \"\\s*)[^\"\\\\]*(?:\\\\.[^\"\\\\]*)*";

            foreach (var property in MaskedProperties)
            {
                var pattern = regexTemplate.Replace("{propertyName}", ToCamelCase(property.Name));
                var matchedValues = Regex.Matches(json, pattern);

                foreach (Match matchedValue in matchedValues)
                {
                    var attribute = (LogMaskedAttribute)Attribute.GetCustomAttribute(property, typeof(LogMaskedAttribute));
                    var isDefaultValue = attribute.Text == "***";

                    var val = matchedValue.Value;

                    if (string.IsNullOrEmpty(val))
                    {
                        //if the value is null, json should be empty
                        json = string.Empty;
                        continue;
                    }

                    if (attribute.ShowFirst == 0 && attribute.ShowLast == 0)
                    {
                        if (attribute.PreserveLength)
                        {
                            json = json.Replace(matchedValue.Value, new string(attribute.Text[0], val.Length));
                            continue;
                        }

                        json = json.Replace(matchedValue.Value, attribute.Text);
                        continue;
                    }

                    if (attribute.ShowFirst > 0 && attribute.ShowLast == 0)
                    {
                        var first = val.Substring(0, Math.Min(attribute.ShowFirst, val.Length));

                        if (!attribute.PreserveLength || !isDefaultValue)
                        {
                            json = json.Replace(matchedValue.Value, first + attribute.Text);
                            continue;
                        }

                        var mask = string.Empty;
                        if (attribute.ShowFirst <= val.Length)
                            mask = new string(attribute.Text[0], val.Length - attribute.ShowFirst);

                        json = json.Replace(matchedValue.Value, first + mask);
                        continue;
                    }

                    if (attribute.ShowFirst == 0 && attribute.ShowLast > 0)
                    {
                        var last = attribute.ShowLast > val.Length ? val : val.Substring(val.Length - attribute.ShowLast);

                        if (!attribute.PreserveLength || !isDefaultValue)
                        {
                            json = json.Replace(matchedValue.Value, attribute.Text + last);
                            continue;
                        }

                        var mask = string.Empty;
                        if (attribute.ShowLast <= val.Length)
                            mask = new string(attribute.Text[0], val.Length - attribute.ShowLast);

                        json = json.Replace(matchedValue.Value, mask + last);
                    }
                }
            }

            return json;
        }
    }
}
