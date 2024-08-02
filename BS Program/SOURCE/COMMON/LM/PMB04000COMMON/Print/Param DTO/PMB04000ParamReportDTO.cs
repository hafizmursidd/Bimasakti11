using PMB04000COMMON.DTO.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMB04000COMMON.Print.Param_DTO
{
    public class PMB04000ParamReportDTO : PMB04000BaseDTO
    {
        public bool LPRINT_ONE_TIME { get; set; }
        public string CDEPT_CODE { get; set; } = "";
        public string CREF_NO { get; set; } = "";
        public bool LPRINT { get; set; }
    }
}
