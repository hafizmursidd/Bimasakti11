using PMB04000COMMON.DTO.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMB04000COMMON.Print
{
    public class PMB04000DataReportDTO : PMB04000BaseDTO
    { 
        public string? CREF_NO { get; set; }
        public string? CTENANT_NAME { get; set; }
        public string? CNOMINAL { get; set; }
        public string? CUNIT_DESCRIPTION { get; set; }
        public string? CINVOICE_NO { get; set; }
        public string? CINVOICE_DATE { get; set; }
        public DateTime? DINVOICE_DATE { get; set; }
        public string? CTRANS_DESC { get; set; }
        public decimal NINV_AMOUNT { get; set; }
        public string? CCURRENCY_SYMBOL { get; set; }
        public decimal NTRANS_AMOUNT { get; set; }
        public string? CCITY { get; set; }
        public string? CTODAY_DATE { get; set; }
        public DateTime? DTODAY_DATE { get; set; }
        public string? CPERSON { get; set; }
        public string? CPOSITION { get; set; }
    }
}
