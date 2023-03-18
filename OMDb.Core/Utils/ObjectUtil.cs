using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Utils
{
    public static class ObjectUtil
    {
        public static bool IsNullOrEmptyOrWhiteSpace(this Object obj)
        {
            return obj==null?true:obj.ToString().Trim().Length==0?true:false;
        }
    }
}
