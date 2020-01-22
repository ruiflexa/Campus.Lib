using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Campus.Lib.Util
{
    public static class StringUtil
    {
        public static string RemoverMascara(this string inputMask)
        {
            if (string.IsNullOrWhiteSpace(inputMask))
            {
                return inputMask;
            }
            const string pattern = @"(?i)[^0-9a-z\s]";
            var replacement = "";
            var rgx = new Regex(pattern);
            return rgx.Replace(inputMask, replacement);
        }

        public static string ObterPrimeiroNome(this string nomeCompleto)
        {
            return nomeCompleto?.Split(' ').FirstOrDefault();
            
        }
    }
}
