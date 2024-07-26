using PMB04000COMMON.DTO.DTOs;
using System;
using System.Text;

namespace PMB04000COMMON.DTO.Utilities
{
    public class PMB04000ParamDTO : PMB04000BaseDTO
    {
        public string? CDEPT_CODE { get; set; }
        public string? CDEPT_NAME { get; set; }
        public string? CTENANT_ID { get; set; }
        public string? CTENANT_NAME { get; set; }
        public bool LALL_TENANT { get; set; }
        public string? CINVOICE_TYPE { get; set; }
        public string? CPERIOD { get; set; }
        public string? CTRANS_CODE { get; set; }
    }
}
