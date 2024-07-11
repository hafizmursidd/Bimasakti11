using System;
using System.Collections.Generic;
using System.Text;

namespace PMT05500COMMON
{
    public class PMT05500ParamDepositDTO
    {
        public string CCHEQUE_ID { get; set; } =  "";
        public string PARAM_CALLER_ID { get; set; } = "";
        public string PARAM_CALLER_TRANS_CODE { get; set; } = "";
        public string PARAM_CALLER_REF_NO { get; set; } = ""; //send from deposit field
        public string PARAM_CALLER_ACTION { get; set; } = "";
        public string PARAM_DEPT_CODE { get; set; } = ""; //OKE
        public string PARAM_REF_NO { get; set; } = ""; //ini didapatkan saat data sudah dibuat alias id punya data yg sudah dibuat
        public string PARAM_DOC_NO { get; set; } = "";
        public string PARAM_DOC_DATE { get; set; } = "";
        public string PARAM_DESCRIPTION { get; set; } = "";
        public string PARAM_GLACCOUNT_NO { get; set; } = "";
        public string PARAM_CENTER_CODE { get; set; } = "";
        public string PARAM_CASH_FLOW_GROUP_CODE { get; set; } = "";
        public string PARAM_CASH_FLOW_CODE { get; set; } = "";
        public decimal PARAM_AMOUNT { get; set; } = 0;

        //To GLT00100
        public string PARAM_CURRENCY_CODE { get; set; } = "";

        public decimal PARAM_LC_BASE_RATE { get; set; } //belum ada fieldnya
        public decimal PARAM_LC_RATE { get; set; } //belum ada fieldnya
        public decimal PARAM_BC_BASE_RATE { get; set; } //belum ada fieldnya
        public decimal PARAM_BC_RATE { get; set; } //belum ada fieldnya

        public string PARAM_BSIS { get; set; } = "";
        public string PARAM_DBCR { get; set; } = "";

        //Param name
        public string PARAM_GLACCOUNT_NAME { get; set; } = "";
        public string PARAM_CENTER_NAME { get; set; } = "";
        public string PARAM_DEPT_NAME { get; set; } = "";
    }
}
