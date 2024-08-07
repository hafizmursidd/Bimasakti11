﻿using System;

namespace PMT01500Common.DTO._2._Agreement
{
    public class PMT01500FrontAgreementDetailDTO
    {
        // External Parameter
        public string? CCOMPANY_ID { get; set; }
        public string? CPROPERTY_ID { get; set; }
        public string? CUSER_ID { get; set; }
        //Real DTOs
        public string? CBUILDING_ID { get; set; }
        public string? CBUILDING_NAME { get; set; }
        public string? CTRANS_CODE { get; set; }
        public string? CTRANS_NAME { get; set; }
        public string? CDEPT_CODE { get; set; }
        public string? CDEPT_NAME { get; set; }
        public string? CREF_NO { get; set; } = "";
        public string? CDOC_NO { get; set; }
        public DateTime? DSTART_DATE { get; set; }
        public int IYEAR { get; set; }
        public int IMONTH { get; set; }
        public int IDAY { get; set; }
        public DateTime? DEND_DATE { get; set; }
        public string? CSALESMAN_ID { get; set; }
        public string? CSALESMAN_NAME { get; set; }
        public string? CTENANT_ID { get; set; }
        public string? CTENANT_NAME { get; set; }
        public string? CUNIT_DESCRIPTION { get; set; }
        public string? CNOTES { get; set; } = "";
        public DateTime? DREF_DATE { get; set; }
        public DateTime? DDOC_DATE { get; set; }
        public string? CTRANS_STATUS_DESCR { get; set; }
        public string? CAGREEMENT_STATUS_DESCR { get; set; }
        public string? CCURRENCY_CODE { get; set; }
        public string? CLEASE_MODE { get; set; }
        public string? CLEASE_MODE_DESCR { get; set; }
        public string? CCHARGE_MODE { get; set; }
        public string? CCHARGE_MODE_DESCR { get; set; }
    }
}
