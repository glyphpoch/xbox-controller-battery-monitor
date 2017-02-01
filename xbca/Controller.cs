using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xbca
{
    class Controller
    {        
        public int ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Charge { get; set; }

        public Controller(int t_ID, string t_device, string t_type, string t_charge)
        {
            ID = t_ID;
            Name = t_device;
            Type = t_type;
            Charge = t_charge;
        }
    }
}
