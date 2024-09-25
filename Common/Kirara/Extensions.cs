using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace Kirara
{
    public static class FormatStringExtensions
    {
        /// <summary>
        ///
        /// </summary>
        /// <returns>"variable name=value"</returns>
        [Pure]
        public static string NameValue<T>(this T variable, [CallerArgumentExpression("variable")] string name = null)
        {
            string valueText = variable switch
            {
                null => "null",
                char => $"'{variable}'",
                string => $"\"{variable}\"",
                _ => variable.ToString(),
            };
            return name + '=' + valueText;
        }
    }
}