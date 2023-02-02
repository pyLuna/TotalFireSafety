using System.Collections.Generic;
using System.Linq;

namespace TotalFireSafety.Models
{
    public class Dictionary
    {
        public IDictionary<string, string> GetValues(object obj)
        {
            return obj
                    .GetType()
                    .GetProperties()
                    .ToDictionary(p => p.Name, p => p.GetValue(obj).ToString());
        }
    }
}