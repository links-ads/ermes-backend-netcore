using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Helpers
{
    public static class EntityCodeHelper
    {
        public static string GetNextCode(string prefix, string currentLastCode, int codeLength = 8, char paddingChar = '0')
        {
            int num = currentLastCode == null ? 0 : int.Parse(currentLastCode.Substring(prefix.Length).TrimStart(paddingChar));
            num++;
            return prefix + num.ToString().PadLeft(codeLength, paddingChar);
        }
    }
}
