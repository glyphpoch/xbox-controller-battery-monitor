using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xbca
{
    class Controller
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Charge { get; set; }

        public Controller(string t_device, string t_type, string t_charge)
        {
            Name = t_device;
            Type = t_type;
            Charge = t_charge;
        }
    }
}
