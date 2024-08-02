using PMB04000COMMON.Print.Utility_DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMB04000COMMON.Print
{
    public class PMB04000ResultDataDTO
    {
        public PMB04000ColumnDTO? Column { get; set; }
        public PMB04000LabelDTO? Label { get; set; }
        public PMB04000BaseHeaderDTO? Header { get; set; }
        public List<PMB04000DataReportDTO>? Data { get; set; }
    }
}
