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
        
                public static string RemoverAcentos(string text)
        {
            StringBuilder sbReturn = new StringBuilder();
            var arrayText = text.Normalize(NormalizationForm.FormD).ToCharArray();
            foreach (char letter in arrayText)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(letter) != UnicodeCategory.NonSpacingMark)
                    sbReturn.Append(letter);
            }

            return sbReturn.ToString();
        }

        public static string RemoverCaracteresEspeciais(string text)
        {
            text = Regex.Replace(text, @"[^0-9a-zA-ZéúíóáÉÚÍÓÁèùìòàÈÙÌÒÀõãñÕÃÑêûîôâÊÛÎÔÂëÿüïöäËYÜÏÖÄçÇ\s]+?", " ");
            text = Regex.Replace(text, @"\s+", " ");
            return text;
        }

        public static string FormatarDescricao(string valor, int maxTamanhoCampo)
        {
            if (!string.IsNullOrWhiteSpace(valor))
            {
                var tamanhoCampo = valor.Trim().Length;
                return valor.Trim().Substring(0, (tamanhoCampo >= maxTamanhoCampo ? maxTamanhoCampo : valor.Trim().Length));
            }

            return valor;
        }

        public static string ObterIniciaisNome(string nome)
        {
            String[] palavras = nome.TrimEnd().Split(" ");
            var iniciais = string.Empty;
            for (int i = 0; i < palavras.Length; i++)
            {
                if (palavras[i].Trim() != string.Empty)
                    iniciais += palavras[i].Trim().Substring(0, 1).ToUpper();
            }
            return iniciais;
        }

        public static string SomenteNumeros(string texto)
        {
            return Regex.Replace(texto, @"[^\d]", "");
        }

        public static string RetornarNumTelefone(string telefone, string parte)
        {
            var regexTelefone = Regex.Match(telefone, @"\((\d{2})\)\s?(\d{4,5}\-?\d{4})");
            return parte switch
            {
                "DDD" => SomenteNumeros(regexTelefone.Groups[1].ToString()),
                "TELEFONE" => SomenteNumeros(regexTelefone.Groups[2].ToString()),
                _ => SomenteNumeros(telefone)
            };
        }
    }
}
