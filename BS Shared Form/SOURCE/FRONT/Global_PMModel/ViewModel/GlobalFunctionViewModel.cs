using Global_PMCOMMON.DTOs.User_Param_Detail;
using R_BlazorFrontEnd.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Global_PMModel.ViewModel
{
    public class GlobalFunctionViewModel
    {
        private readonly GlobalFunctionModel _model = new GlobalFunctionModel();

        public async Task<GetUserParamDetailDTO> GetUserParamDetail(GetUserParamDetailParameterDTO poParam)
        {
            var loEx = new R_Exception();
            GetUserParamDetailDTO loRtn = null;
            try
            {
                var loResult = await _model.UserParamDetailAsync(poParam);
                loRtn = loResult;
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }
            loEx.ThrowExceptionIfErrors();
            return loRtn!;
        }

    }
}
