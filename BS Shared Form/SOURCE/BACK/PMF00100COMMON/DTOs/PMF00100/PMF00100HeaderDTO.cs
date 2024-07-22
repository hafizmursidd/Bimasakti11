using System;
using System.Collections.Generic;
using System.Text;

namespace PMF00100COMMON.DTOs.PMF00100
{
    public class PMF00100HeaderDTO
    {
        public string CTRANSACTION_NAME { get; set; } = "";
        public string CPROPERTY_NAME { get; set; } = "";
        public string CDEPT_CODE { get; set; } = "";
        public string CDEPT_NAME { get; set; } = "";
        public string CTENANT_ID { get; set; } = "";
        public string CTENANT_NAME { get; set; } = "";
        public string CCUSTOMER_TYPE_NAME { get; set; } = "";
        public string CCURRENCY_CODE { get; set; } = ""; 
        public string CREF_NO { get; set; } = "";
        public DateTime DREF_DATE { get; set; }
        public string CREF_DATE { get; set; } = "";
        //public string CSUPPLIER_ID { get; set; } = "";
        //public string CSUPPLIER_NAME { get; set; } = "";
        //public string CSUPPLIER_SEQ_NO { get; set; } = "";
        //public string CDOC_NO { get; set; } = "";
        //public string CDOC_DATE { get; set; } = "";
        //public DateTime? DDOC_DATE { get; set; }
        public decimal NLBASE_RATE { get; set; } = 0;
        public decimal NLCURRENCY_RATE { get; set; } = 0;
        public decimal NTAX_BASE_RATE { get; set; } = 0;
        public decimal NTAX_CURRENCY_RATE { get; set; } = 0;

    }
}
