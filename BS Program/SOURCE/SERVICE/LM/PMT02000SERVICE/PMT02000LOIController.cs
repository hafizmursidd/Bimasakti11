using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PMT02000Back;
using PMT02000COMMON.Interface;
using PMT02000COMMON.Logs;
using PMT02000COMMON.LOI_List;
using PMT02000COMMON.Utility;
using R_BackEnd;
using R_Common;
using R_CommonFrontBackAPI;
using System.Diagnostics;

namespace PMT02000SERVICE
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PMT02000LOIController : ControllerBase, IPMT02000
    {
        private LoggerPMT02000 _logger;
        private readonly ActivitySource _activitySource;

        public PMT02000LOIController(ILogger<PMT02000LOIController> logger)
        {
            //Initial and Get Logger
            LoggerPMT02000.R_InitializeLogger(logger);
            _logger = LoggerPMT02000.R_GetInstanceLogger();
            _activitySource = PMT02000Activity.R_InitializeAndGetActivitySource(nameof(PMT02000LOIController));


        }
        [HttpPost]
        public R_ServiceGetRecordResultDTO<PMT02000LOIHeader_DetailDTO> R_ServiceGetRecord(R_ServiceGetRecordParameterDTO<PMT02000LOIHeader_DetailDTO> poParameter)
        {
            string lcMethodName = nameof(R_ServiceGetRecord);
            using Activity activity = _activitySource.StartActivity(lcMethodName);
            _logger.LogInfo(string.Format("START process method {0} on Controller", lcMethodName));

            var loEx = new R_Exception();
            var loRtn = new R_ServiceGetRecordResultDTO<PMT02000LOIHeader_DetailDTO>();

            try
            {
                var loCls = new PMT02000LOICls();
                poParameter.Entity.CCOMPANY_ID = R_BackGlobalVar.COMPANY_ID;
                poParameter.Entity.CUSER_ID = R_BackGlobalVar.USER_ID;
                poParameter.Entity.CLANG_ID = R_BackGlobalVar.CULTURE;
                _logger.LogInfo("Call method R_GetRecord");
                loRtn.data = loCls.R_GetRecord(poParameter.Entity);
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
                _logger.LogError(loEx);
            }
            loEx.ThrowExceptionIfErrors();
            _logger.LogInfo(string.Format("END process method {0} on Controller", lcMethodName));


            return loRtn;
        }
        [HttpPost]
        public R_ServiceSaveResultDTO<PMT02000LOIHeader_DetailDTO> R_ServiceSave(R_ServiceSaveParameterDTO<PMT02000LOIHeader_DetailDTO> poParameter)
        {
            string lcMethodName = nameof(R_ServiceSave);
            using Activity activity = _activitySource.StartActivity(lcMethodName)!;
            _logger.LogInfo(string.Format("START process method {0} on Controller", lcMethodName));

            R_Exception loException = new R_Exception();
            R_ServiceSaveResultDTO<PMT02000LOIHeader_DetailDTO>? loRtn = null;
            PMT02000LOICls loCls;

            try
            {
                loCls = new PMT02000LOICls();
                loRtn = new R_ServiceSaveResultDTO<PMT02000LOIHeader_DetailDTO>();
                poParameter.Entity.CCOMPANY_ID = R_BackGlobalVar.COMPANY_ID;
                poParameter.Entity.CUSER_ID = R_BackGlobalVar.USER_ID;
                _logger.LogInfo("Call method R_ServiceSave");
                loRtn.data = loCls.R_Save(poParameter.Entity, poParameter.CRUDMode);
            }
            catch (Exception ex)
            {
                loException.Add(ex);
                _logger.LogError(loException);
            };
            loException.ThrowExceptionIfErrors();
            _logger.LogInfo(string.Format("END process method {0} on Controller", lcMethodName));

            return loRtn;
        }
        [HttpPost]
        public PMT02000LOIHeader ProcessSubmitRedraft(PMT02000DBParameter poParam)
        {
            string lcMethodName = nameof(ProcessSubmitRedraft);
            using Activity activity = _activitySource.StartActivity(lcMethodName)!;
            _logger.LogInfo(string.Format("START process method {0} on Controller", lcMethodName));

            var loEx = new R_Exception();
            var loRtn = new PMT02000LOIHeader();

            try
            {
                var loCls = new PMT02000LOICls();
                poParam.CCOMPANY_ID = R_BackGlobalVar.COMPANY_ID;
                poParam.CUSER_ID = R_BackGlobalVar.USER_ID;
                _logger.LogInfo("Call method Submitted and Redraft");
                 loCls.ProcessSubmitRedraft(poParam);
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
                _logger.LogError(loEx);
            }
            loEx.ThrowExceptionIfErrors();
            _logger.LogInfo(string.Format("END process method {0} on Controller", lcMethodName));

            return loRtn;
        }
        [HttpPost]
        public R_ServiceDeleteResultDTO R_ServiceDelete(R_ServiceDeleteParameterDTO<PMT02000LOIHeader_DetailDTO> poParameter)
        {
            string lcMethodName = nameof(R_ServiceDelete);
            using Activity activity = _activitySource.StartActivity(lcMethodName);
            _logger.LogInfo(string.Format("START process method {0} on Controller", lcMethodName));

            R_Exception loException = new R_Exception();
            R_ServiceDeleteResultDTO loRtn = null;
            PMT02000LOICls loCls;

            try
            {
                loCls = new PMT02000LOICls();
                loRtn = new R_ServiceDeleteResultDTO();
                poParameter.Entity.CCOMPANY_ID = R_BackGlobalVar.COMPANY_ID;
                poParameter.Entity.CUSER_ID = R_BackGlobalVar.USER_ID;
                _logger.LogInfo("Call method R_Delete");
                loCls.R_Delete(poParameter.Entity);
            }
            catch (Exception ex)
            {
                loException.Add(ex);
                _logger.LogError(loException);
            };
            loException.ThrowExceptionIfErrors();
            _logger.LogInfo(string.Format("END process method {0} on Controller", lcMethodName));

            return loRtn!;
        }
        [HttpPost]
        public IAsyncEnumerable<PMT02000PropertyDTO> GetPropertyListStream()
        {
            string lcMethodName = nameof(GetPropertyListStream);
            using Activity activity = _activitySource.StartActivity(lcMethodName)!;
            _logger.LogInfo(string.Format("START process method {0} on Controller", lcMethodName));

            var loEx = new R_Exception();
            IAsyncEnumerable<PMT02000PropertyDTO> loRtn = null;
            List<PMT02000PropertyDTO> loRtnTemp;
            try
            {
                var loDbParameter = new PMT02000DBParameter();
                var loCls = new PMT02000LOICls();

                loDbParameter.CCOMPANY_ID = R_BackGlobalVar.COMPANY_ID;
                loDbParameter.CUSER_ID = R_BackGlobalVar.USER_ID;
                _logger.LogDebug("DbParameter {@Parameter} ", loDbParameter);

                _logger.LogInfo("Call method GetAllPropertyList");
                loRtnTemp = loCls.GetAllPropertyList(loDbParameter);
                _logger.LogInfo("Call method to streaming data");
                loRtn = HelperStream(loRtnTemp);
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
                _logger.LogError(loEx);
            }
            loEx.ThrowExceptionIfErrors();
            _logger.LogInfo(string.Format("END process method {0} on Controller", lcMethodName));

#pragma warning disable CS8603 // Possible null reference return.
            return loRtn;
#pragma warning restore CS8603 // Possible null reference return.
        }
        [HttpPost]
        public IAsyncEnumerable<PMT02000LOIDTO> GetLOIListStream()
        {
            string lcMethodName = nameof(GetLOIListStream);
            using Activity activity = _activitySource.StartActivity(lcMethodName);
            _logger.LogInfo(string.Format("START process method {0} on Controller", lcMethodName));

            var loEx = new R_Exception();
            IAsyncEnumerable<PMT02000LOIDTO> loRtn = null;
            List<PMT02000LOIDTO> loRtnTemp;
            try
            {
                var loDbParameter = new PMT02000DBParameter();
                var loCls = new PMT02000LOICls();

                loDbParameter.CCOMPANY_ID = R_BackGlobalVar.COMPANY_ID;
                loDbParameter.CUSER_ID = R_BackGlobalVar.USER_ID;
                loDbParameter.CPROPERTY_ID = R_Utility.R_GetStreamingContext<string>(ContextConstant.CPROPERTY_ID);
                loDbParameter.CTRANS_CODE = R_Utility.R_GetStreamingContext<string>(ContextConstant.CTRANS_CODE);
                _logger.LogDebug("DbParameter {@Parameter} ", loDbParameter);

                _logger.LogInfo("Call method GetLOIList");
                loRtnTemp = loCls.GetLOIList(loDbParameter);
                _logger.LogInfo("Call method to streaming data");
                loRtn = HelperStream(loRtnTemp);
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
                _logger.LogError(loEx);
            }

            loEx.ThrowExceptionIfErrors();
            _logger.LogInfo(string.Format("END process method {0} on Controller", lcMethodName));

#pragma warning disable CS8603 // Possible null reference return.
            return loRtn;
#pragma warning restore CS8603 // Possible null reference return.
        }
        [HttpPost]
        public PMT02000LOIHeader GetLOIHeader(PMT02000DBParameter param)
        {
            string lcMethodName = nameof(GetLOIDetailListStream);
            using Activity activity = _activitySource.StartActivity(lcMethodName)!;
            _logger.LogInfo(string.Format("START process method {0} on Controller", lcMethodName));

            var loEx = new R_Exception();
            PMT02000LOIHeader? loReturn = null;
            try
            {
                var loCls = new PMT02000LOICls();

                param.CCOMPANY_ID = R_BackGlobalVar.COMPANY_ID;
                param.CUSER_ID = R_BackGlobalVar.USER_ID;
                param.CLANG_ID = R_BackGlobalVar.CULTURE;
                _logger.LogDebug("DbParameter {@Parameter} ", param);

                _logger.LogInfo(string.Format("Call method {0} on Cls", lcMethodName));
                _logger.LogInfo("Call method GETLOIHeader");
                loReturn = loCls.GetHeader(param);
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
                _logger.LogError(loEx);
            }

            loEx.ThrowExceptionIfErrors();
            _logger.LogInfo(string.Format("END process method {0} on Controller", lcMethodName));

#pragma warning disable CS8603 // Possible null reference return.
            return loReturn;
#pragma warning restore CS8603 // Possible null reference return.

        }
        [HttpPost]
        public IAsyncEnumerable<PMT02000LOIDetailListDTO> GetLOIDetailListStream()
        {
            string lcMethodName = nameof(GetLOIDetailListStream);
            using Activity activity = _activitySource.StartActivity(lcMethodName)!;
            _logger.LogInfo(string.Format("START process method {0} on Controller", lcMethodName));

            var loEx = new R_Exception();
            IAsyncEnumerable<PMT02000LOIDetailListDTO> loRtn = null;
            List<PMT02000LOIDetailListDTO> loRtnTemp;
            try
            {
                var loDbParameter = new PMT02000DBParameter();
                var loCls = new PMT02000LOICls();

                loDbParameter.CCOMPANY_ID = R_BackGlobalVar.COMPANY_ID;
                loDbParameter.CUSER_ID = R_BackGlobalVar.USER_ID;
                loDbParameter.CLANG_ID = R_BackGlobalVar.CULTURE;
                loDbParameter.CPROPERTY_ID = R_Utility.R_GetStreamingContext<string>(ContextConstant.CPROPERTY_ID);
                loDbParameter.CDEPT_CODE = R_Utility.R_GetStreamingContext<string>(ContextConstant.CDEPT_CODE);
                loDbParameter.CTRANS_CODE = R_Utility.R_GetStreamingContext<string>(ContextConstant.CTRANS_CODE);
                loDbParameter.CREF_NO = R_Utility.R_GetStreamingContext<string>(ContextConstant.CREF_NO);
                loDbParameter.CBUILDING_ID = R_Utility.R_GetStreamingContext<string>(ContextConstant.CBUILDING_ID);
                loDbParameter.CFLOOR_ID = R_Utility.R_GetStreamingContext<string>(ContextConstant.CFLOOR_ID);
                loDbParameter.CUNIT_ID = R_Utility.R_GetStreamingContext<string>(ContextConstant.CUNIT_ID);

                _logger.LogDebug("DbParameter {@Parameter} ", loDbParameter);

                _logger.LogInfo("Call method GetLOIList");
                loRtnTemp = loCls.GetDetailListLOI(loDbParameter);
                _logger.LogInfo("Call method to streaming data");
                loRtn = HelperStream(loRtnTemp);
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
                _logger.LogError(loEx);
            }

            loEx.ThrowExceptionIfErrors();
            _logger.LogInfo(string.Format("END process method {0} on Controller", lcMethodName));

#pragma warning disable CS8603 // Possible null reference return.
            return loRtn;
#pragma warning restore CS8603 // Possible null reference return.
        }
        private async IAsyncEnumerable<T> HelperStream<T>(List<T> poParameter)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            foreach (var item in poParameter)
            {
                yield return item;
            }
        }

  
    }
}