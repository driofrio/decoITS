using System;

namespace Its.Utils.StringUtils
{
    public class StringUtils
    {
        public static bool IsNullOrWhiteSpace(String value) {
            if (value == null) return true; 

            for(int i = 0; i < value.Length; i++) { 
                if(!Char.IsWhiteSpace(value[i])) return false; 
            }

            return true;
        }
    }
}