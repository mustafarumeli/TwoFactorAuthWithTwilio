using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserLoginWithTwiloo.Classes
{
    public class CodeGenerator
    {
        public static string GetCode()
        {
            Random rnd = new Random();
            string retVal = "";
            for (int i = 0; i < 5; i++)
            {
                retVal += Convert.ToChar(rnd.Next(48, 58));
            }
            return retVal;
        }
    }
}
