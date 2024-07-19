using Global_PMCOMMON.DTOs.Generic__DTO;
using Global_PMCOMMON.DTOs.User_Param_Detail;
using Global_PMCOMMON.Interface;
using R_APIClient;
using R_BlazorFrontEnd.Exceptions;
using R_BusinessObjectFront;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Global_PMModel
{
    public class GlobalFunctionModel : R_BusinessObjectServiceClientBase<GetUserParamDetailDTO>, IGlobalFunctionPM
    {
        private const string DEFAULT_HTTP = "R_DefaultServiceUrlPM";
        private const string DEFAULT_ENDPOINT = "api/GlobalFunctionPM";
        private const string DEFAULT_MODULE = "PM";

        public GlobalFunctionModel(
             string pcHttpClientName = DEFAULT_HTTP,
            string pcRequestServiceEndPoint = DEFAULT_ENDPOINT,
            string pcModuleName = DEFAULT_MODULE,
            bool plSendWithContext = true,
            bool plSendWithToken = true) :
            base(pcHttpClientName, pcRequestServiceEndPoint, pcModuleName, plSendWithContext, plSendWithToken)
        {
        }
        public async Task<GetUserParamDetailDTO> UserParamDetailAsync(GetUserParamDetailParameterDTO poParam)
        {
            var loEx = new R_Exception();
            GetUserParamDetailDTO loResult = new GetUserParamDetailDTO();

            try
            {
                R_HTTPClientWrapper.httpClientName = _HttpClientName;

                var loTempResult = await R_HTTPClientWrapper.R_APIRequestObject<GenericRecord<GetUserParamDetailDTO>, GetUserParamDetailParameterDTO>(
                    _RequestServiceEndPoint,
                    nameof(IGlobalFunctionPM.UserParamDetail),
                    poParam,
                    DEFAULT_MODULE,
                    _SendWithContext,
                    _SendWithToken);
                loResult = loTempResult.Data;
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            loEx.ThrowExceptionIfErrors();

            return loResult;
        }

        #region implements Library
        public GenericRecord<GetUserParamDetailDTO> UserParamDetail(GetUserParamDetailParameterDTO poParam)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
