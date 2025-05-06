using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NedapKernel.Models
{
    public class CardIssueResult
    {
        public List<CardIssue> CardIssues { get; set; }
    }

    public class CardIssue
    {
        public int CarrierId { get; set; }
        public int FailedAttempts { get; set; }
        public int TotalAttempts { get; set; }
    }
}
