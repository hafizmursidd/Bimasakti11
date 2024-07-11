using System;

namespace PMT05500COMMON.DTO
{
    public class LMT05500DepositListDTO : LMT05500Header
    {

        public string? CSEQ_NO { get; set; }
        public string? CDEPT_CODE { get; set; }
        public string? CTRANS_CODE { get; set; }
        public string? CREF_NO { get; set; }
        public string? CCONTRACTOR_NAME { get; set; }
        public string? CDEPOSIT_ID { get; set; }
        public string? CDEPOSIT_NAME { get; set; }
        public string? CDEPOSIT_DATE { get; set; }

        public decimal NDEPOSIT_AMOUNT { get; set; }
        public decimal NREMAINING_AMOUNT { get; set; }
        public string? CINVOICE_NO { get; set; }
        public bool LPAYMENT { get; set; }
        public string? CUPDATE_BY { get; set; }
        public DateTime DUPDATE_DATE { get; set; }
        public string? CCREATE_BY { get; set; }
        public DateTime DCREATE_DATE { get; set; }
        public DateTime? DDEPOSIT_DATE { get; set; }

        
        //PARAM TO ANOTHER PROGRAM Penambahan
        //others param on the top
        public string? CCHARGE_MODE { get; set; }
        public string? CBUILDING_ID { get; set; }
        public string? CFLOOR_ID { get; set; }
        public string? CUNIT_ID { get; set; }
        public string? CDOC_NO { get; set; }
        public string? CDOC_DATE { get; set; }
        public string? CCURRENCY_CODE { get; set; }
        public string? CGLACCOUNT_NO { get; set; }
        public string? CCENTER_CODE { get; set; }
        public string? CCASH_FLOW_GROUP_CODE { get; set; }
        public string? CCASH_FLOW_CODE { get; set; }
        public string? CBSIS { get; set; }
        public string? CDBCR { get; set; }
        public string? CDESCRIPTION { get; set; }

        public decimal NBBASE_RATE_AMOUNT { get; set; }
        public decimal NBCURRENCY_RATE_AMOUNT { get; set; }
        public decimal NLBASE_RATE_AMOUNT { get; set; }
        public decimal NLCURRENCY_RATE_AMOUNT { get; set; }

    }
}