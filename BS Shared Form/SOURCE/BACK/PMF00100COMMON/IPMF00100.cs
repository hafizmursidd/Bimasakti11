using PMF00100COMMON.DTOs.PMF00100;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMF00100COMMON
{
    public interface IPMF00100
    {
        GetCompanyInfoResultDTO GetCompanyInfo();
        GetGLSystemParamResultDTO GetGLSystemParam();
        GetCallerTrxInfoResultDTO GetCallerTrxInfo(GetCallerTrxInfoParameterDTO poParam);
        GetPeriodResultDTO GetPeriod(GetPeriodParameterDTO poParam);
        GetTransactionFlagResultDTO GetTransactionFlag(GetTransactionFlagParameterDTO poParam);
        PMF00100HeaderResultDTO GetHeader(PMF00100HeaderParameterDTO poParam);
        IAsyncEnumerable<PMF00100ListDTO> GetAllocationList();
    }
}
