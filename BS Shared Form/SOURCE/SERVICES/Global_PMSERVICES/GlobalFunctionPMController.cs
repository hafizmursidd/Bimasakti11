using Global_PMBACK;
using Global_PMCOMMON.DTOs.Generic__DTO;
using Global_PMCOMMON.DTOs.User_Param_Detail;
using Global_PMCOMMON.Interface;
using Global_PMCOMMON.Logs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using R_BackEnd;
using R_Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Global_PMSERVICES
{
    [ApiController]
    [Route("api/[controller]/[action]"), AllowAnonymous]
    public class GlobalFunctionPMController : ControllerBase, IGlobalFunctionPM
    {
        private readonly LoggerGlobalFunctionPM _logger;
        private readonly ActivitySource _activitySource;
        public GlobalFunctionPMController(ILogger<GlobalFunctionPMController> logger)
        {
            LoggerGlobalFunctionPM.R_InitializeLogger(logger);
            _logger = LoggerGlobalFunctionPM.R_GetInstanceLogger();
            _activitySource = GlobalFunctionPMActivity.R_InitializeAndGetActivitySource(nameof(GlobalFunctionPMController));

        }
        [HttpPost]
        public GenericRecord<GetUserParamDetailDTO> UserParamDetail(GetUserParamDetailParameterDTO poParam)
        {
            string lcMethodName = nameof(UserParamDetail);
            using Activity activity = _activitySource.StartActivity(lcMethodName)!;
            _logger.LogInfo(string.Format("START process method {0} on Controller", lcMethodName));

            var loEx = new R_Exception();
            GenericRecord<GetUserParamDetailDTO> loReturn = new();
            try
            {
                var loCls = new GlobalFunctionPMCls();
                poParam.CCOMPANY_ID = R_BackGlobalVar.COMPANY_ID;
                poParam.CUSER_ID = R_BackGlobalVar.USER_ID;
                _logger.LogInfo($"Call method {0}", lcMethodName);
                var loTemp = loCls.GetUserParamDetailDb(poParam);

                loReturn.Data = loTemp;

            }
            catch (Exception ex)
            {

                loEx.Add(ex);
                _logger.LogError(loEx);
            }
            loEx.ThrowExceptionIfErrors();
            _logger.LogInfo(string.Format("END process method {0} on Controller", lcMethodName));
            return loReturn;
        }
    }
}
