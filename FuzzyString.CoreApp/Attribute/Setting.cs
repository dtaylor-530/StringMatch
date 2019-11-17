using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace StringMatch
{
    public class SettingAttribute : Attribute
    {
        public string Name { get; set; }
        public SettingAttribute([CallerMemberName] string name = null)
        {
            Name = name;
        }
    }
}
