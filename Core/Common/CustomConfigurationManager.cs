using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;

namespace Core.Common
{
    public class CustomConfigurationManager
    {
        public static string ReadAppSetting(string key)
        {
            var value = ConfigurationManager.AppSettings[key];

            value = ReplaceCustomKeywords(value);

            return value;
        }

        private static IEnumerable<string> ExtractCustomKeys(string value)
        {
            if (value == null)
            {
                yield break;
            }
            foreach (var match in Regex.Matches(value, @"\{\w+\}"))
            {
                yield return match.ToString().ToUpper();
            }
        }

        private static string ReplaceCustomKeywords(string value)
        {
            if (value == null)
            {
                return null;
            }

            var customKeys = ExtractCustomKeys(value).ToArray();
            if (customKeys.Any() == false)
            {
                return value;
            }

            foreach (var customKey in customKeys)
            {
                value = Regex.Replace(value, customKey, GetCustomValue(customKey), RegexOptions.IgnoreCase);
            }

            return value;
        }

        private static string GetCustomValue(string customKey)
        {
            var solutionDir = DirectoryManager.SolutionDirectory.FullName;
            if (customKey.Equals("{SOLUTIONDIR}"))
            {
                return solutionDir;
            }
            if (customKey.StartsWith("{WEB"))
            {
                return string.Format(@"{0}\WebServices\{1}", solutionDir, customKey.Replace("{", string.Empty).Replace("}", string.Empty));
            }
            return customKey;
        }
    }
}
