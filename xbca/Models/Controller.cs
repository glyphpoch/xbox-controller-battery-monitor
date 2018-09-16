using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xbca.Models
{
    public class Controller
    {
        public int Id { get; set; } = -1;
        public byte BatteryType { get; set; }
        public byte BatteryValue { get; set; }
        public byte Note { get; set; }
        public string Status { get; set; } = "TODO";
        public string Name { get; set; }
        public Stopwatch NotifyTimer { get; set; } = new Stopwatch();
        public bool Notified { get; set; }
        public byte LastLevel { get; set; }
    }
}
