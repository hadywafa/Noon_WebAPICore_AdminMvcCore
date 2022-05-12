using System;
using System.Collections.Generic;
using System.Linq;

namespace BL.Helpers
{
   public static class StringHelper
    {
        public static string ToImageUrl(this string image)
        {
            //return $"http://localhost:25248/Images/{image}";
            return $"http://7699-53329.el-alt.com/Images/{image}";
        }
    }
}
