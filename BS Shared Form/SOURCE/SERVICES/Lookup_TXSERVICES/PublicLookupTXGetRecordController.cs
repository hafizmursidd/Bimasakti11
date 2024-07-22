﻿using Lookup_TXBACK;
using Lookup_TXCOMMON.DTOs.TXL00100;
using Lookup_TXCOMMON.DTOs.Utilities;
using Lookup_TXCOMMON.Interface;
using Lookup_TXCOMMON.Loggers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using R_BackEnd;
using R_Common;
using System.Diagnostics;

namespace Lookup_TXSERVICES
{
    [ApiController]
    [Route("api/[controller]/[action]"), AllowAnonymous]
    public class PublicLookupTXGetRecordController : ControllerBase, IGetRecordPublicLookupTX
    {
        private LoggerLookupTX _loggerLookup;
        private readonly ActivitySource _activitySource;

        public PublicLookupTXGetRecordController(ILogger<LoggerLookupTX> logger)
        {
            //Initial and Get Logger
            LoggerLookupTX.R_InitializeLogger(logger);
            _loggerLookup = LoggerLookupTX.R_GetInstanceLogger();
            _activitySource = LookupTXActivity.R_InitializeAndGetActivitySource(nameof(PublicLookupTXGetRecordController));
        }

        [HttpPost]
        public TXLGenericRecord<TXL00100DTO> TXL00100BranchLookUp(TXL00100ParameterGetRecordDTO poParameter)
        {
            string lcMethodName = nameof(TXL00100BranchLookUp);
            using Activity activity = _activitySource.StartActivity(lcMethodName)!;
            _loggerLookup.LogInfo(string.Format("START process method {0} on Controller", lcMethodName));

            TXLParameterCompanyAndUserDTO loDbParameterInternal;
            var loEx = new R_Exception();
            TXLGenericRecord<TXL00100DTO> loReturn = new();
            try
            {
                var loCls = new PublicLookupTXCls();
                _loggerLookup.LogInfo("Call method TXL00100BranchLookUp");
                loDbParameterInternal = new TXLParameterCompanyAndUserDTO()
                {
                    CCOMPANY_ID = R_BackGlobalVar.COMPANY_ID,
                    CUSER_ID = R_BackGlobalVar.USER_ID,
                };
                var loTempList = loCls.TXL00100BranchLookUpDb(loDbParameterInternal);

                _loggerLookup.LogInfo("Filter Search by text");
                loReturn.Data = loTempList
                    .Find(x => x.CBRANCH_CODE!
                        .Equals(poParameter.CSEARCH_TEXT!
                        .Trim(),
                        StringComparison.OrdinalIgnoreCase))!;

            }
            catch (Exception ex)
            {

                loEx.Add(ex);
                _loggerLookup.LogError(loEx);
            }
            loEx.ThrowExceptionIfErrors();
            _loggerLookup.LogInfo(string.Format("END process method {0} on Controller", lcMethodName));
            return loReturn;
        }
    }
}
