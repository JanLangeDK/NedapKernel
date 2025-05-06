using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NedapKernel.Models
{
    public class FlaggedEvent
    {
        public string carrierid { get; set; }
        public DateTime servertimestamp { get; set; }
        public string entranceid { get; set; }
        public string reason { get; set; }
    }

    public class FlaggedEventsResult
    {
        public List<FlaggedEvent> FlaggedEvents { get; set; }
    }

}
