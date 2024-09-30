using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace Kirara
{
    public static class FormatStringExtensions
    {
        /// <summary>
        ///
        /// </summary>
        /// <returns>"name=value"</returns>
        [Pure]
        public static string NameValue<T>(this T value, [CallerArgumentExpression("value")] string name = null)
        {
            string valueText = value switch
            {
                null => "null",
                char => $"'{value}'",
                string => $"\"{value}\"",
                _ => value.ToString(),
            };
            return $"{name}={valueText}";
        }
    }
}