using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NedapKernel.Models
{
    public class MaintenancePrediction
    {
        public int EntranceId { get; set; }
        public DateTime PredictedMaintenanceDate { get; set; }
        public double UsageTrend { get; set; }
    }

    public class MaintenancePredictionResult
    {
        public List<MaintenancePrediction> MaintenancePredictions { get; set; }
    }

}
