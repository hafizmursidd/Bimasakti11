using System;
using System.Collections.Generic;
using System.Text;

namespace Global_PMCOMMON.DTOs.Base_Param
{
    public abstract class BaseParameterDTO
    {
        public string CCOMPANY_ID { get; set; } = "";
        public string CUSER_ID { get; set; } = "";
        public string CLANG_ID { get; set; } = "";
    }
}
