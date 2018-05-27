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
#if USING_XINPUT
        public string Type { get; set; }
#else
        public string Status { get; set; }
#endif
        public string Charge { get; set; }
        public string Level { get; set; }

        public Controller(int t_ID, string t_device, string t_type, string t_charge, string t_level, string t_status)
        {
            ID = t_ID;
            Name = t_device;
#if USING_XINPUT
            Type = t_type;
#else
            Status = t_status;
#endif
            Charge = t_charge;
            Level = t_level;
        }
    }
}
