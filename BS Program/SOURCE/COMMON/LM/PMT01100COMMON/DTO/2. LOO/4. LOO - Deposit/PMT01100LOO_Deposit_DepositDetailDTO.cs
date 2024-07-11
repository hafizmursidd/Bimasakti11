﻿using System;

namespace PMT01100Common.DTO._2._LOO._4._LOO___Deposit
{
    public class PMT01100LOO_Deposit_DepositDetailDTO
    {
        public string? CBUILDING_ID { get; set; }
        //For Db Parameter
        public string? CCOMPANY_ID { get; set; }
        public string? CPROPERTY_ID { get; set; }
        public string? CDEPT_CODE { get; set; }
        public string? CTRANS_CODE { get; set; }
        public string? CREF_NO { get; set; }
        public string? CUSER_ID { get; set; }
        //Real DTO : For Front
        public string? CSEQ_NO { get; set; }
        public string? CDEPOSIT_ID { get; set; }
        public string? CDEPOSIT_NAME { get; set; }
        public string? CDEPOSIT_DATE { get; set; }
        public DateTime? DDEPOSIT_DATE { get; set; }
        public string? CCURRENCY_CODE { get; set; }
        public string? CCURRENCY_NAME { get; set; }
        public decimal NDEPOSIT_AMT { get; set; }
        public string? CDESCRIPTION { get; set; } = "";
        public string? CUPDATE_BY { get; set; }
        public DateTime DUPDATE_DATE { get; set; }
        public string? CCREATE_BY { get; set; }
        public DateTime DCREATE_DATE { get; set; }
    }
}
