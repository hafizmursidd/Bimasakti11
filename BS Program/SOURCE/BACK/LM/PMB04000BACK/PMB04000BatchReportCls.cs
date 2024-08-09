using PMB04000BackResources;
using PMB04000COMMON.Context;
using PMB04000COMMON.Logs;
using PMB04000COMMON.Print;
using PMB04000COMMON.Print.Distribute;
using PMB04000COMMON.Print.Param_DTO;
using PMB04000COMMON.Print.Utility_DTO;
using R_BackEnd;
using R_Common;
using R_CommonFrontBackAPI;
using R_Processor;
using R_ProcessorJob;
using R_ReportServerClient;
using R_ReportServerClient.DTO;
using R_ReportServerCommon;
using R_StorageCommon;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Input;

namespace PMB04000BACK
{
    public class PMB04000BatchReportCls : R_IBatchProcess, R_IProcessJob<PMB04000DataReportDTO, R_Exception>
    {
        private readonly LoggerPMB04000? _logger;
        private readonly ActivitySource _activitySource;
        public PMB04000BatchReportCls()
        {
            _logger = LoggerPMB04000.R_GetInstanceLogger();
            _activitySource = PMB04000Activity.R_GetInstanceActivitySource();
        }
        private R_UploadAndProcessKey _oKey;
        private int _nMaxData;
        private int _nCountProcess;
        private int _nCountError;
        private R_UploadAndProcessKey _olock = new R_UploadAndProcessKey(); // For locking multithread
        private R_ProcessorClass<PMB04000DataReportDTO, R_Exception> _oProcessor;

        PMB04000ParamReportDTO poParamReport;

        public void R_BatchProcess(R_BatchProcessPar poBatchProcessPar)
        {
            string lcMethodName = nameof(R_BatchProcess);
            using Activity activity = _activitySource.StartActivity(lcMethodName);
            _logger.LogInfo(string.Format("START process method {0} on Cls", lcMethodName));

            R_Exception loException = new R_Exception();
            R_Db loDb = new R_Db();
            try
            {
                if (loDb.R_TestConnection() == false)
                {
                    //cara 1
                    // throw new Exception("Connection to database failed");

                    //cara2
                    loException.Add("", "Error where Connection to database");
                    _logger.LogError(loException);
                    goto EndBlock;
                }
                var loTask = Task.Run(() =>
                {
                    _BatchProcess(poBatchProcessPar);
                });

                while (!loTask.IsCompleted)
                {
                    Thread.Sleep(100);
                }

                if (loTask.IsFaulted)
                {
                    loException.Add(loTask.Exception.InnerException != null ?
                        loTask.Exception.InnerException :
                        loTask.Exception);

                    goto EndBlock;
                }
            }
            catch (Exception ex)
            {
                loException.Add(ex);
                _logger.LogError(loException);
            }
        EndBlock:
            loException.ThrowExceptionIfErrors();
            _logger.LogInfo(string.Format("END process method {0} on Cls", lcMethodName));

        }

        private async Task _BatchProcess(R_BatchProcessPar poBatchProcessPar)
        {
            string lcMethodName = nameof(_BatchProcess);
            using Activity activity = _activitySource.StartActivity(lcMethodName)!;
            _logger.LogInfo(string.Format("START process method {0} on Cls", lcMethodName));

            R_Exception loException = new R_Exception();
            var loDb = new R_Db();
            DbCommand loCommand = null;
            DbConnection loConnection = null;
            string lcCompany;
            string lcUserId;
            string lcLangId;
            string lcGuidId;
            int Var_Total;

            var loTempListForProcess =
                R_NetCoreUtility.R_DeserializeObjectFromByte<List<string>>(poBatchProcessPar.BigObject);

            R_ReportServerClientConfiguration loReportConfiguration;
            try
            {
                await Task.Delay(100);

                #region GetParameter

                lcCompany = poBatchProcessPar.Key.COMPANY_ID;
                lcUserId = poBatchProcessPar.Key.USER_ID;
                lcLangId = R_BackGlobalVar.CULTURE;
                lcGuidId = poBatchProcessPar.Key.KEY_GUID;
                Var_Total = loTempListForProcess.Count;

                //get parameter
                var loVar = poBatchProcessPar.UserParameters.Where((x) => x.Key.Equals(PMB04000ContextDTO.CPROPERTY_ID)).FirstOrDefault()!.Value;
                string paramPropertyId = ((System.Text.Json.JsonElement)loVar).GetString()!;

                loVar = poBatchProcessPar.UserParameters.Where((x) => x.Key.Equals(PMB04000ContextDTO.CDEPT_CODE)).FirstOrDefault()!.Value;
                string paramDeptCode = ((System.Text.Json.JsonElement)loVar).GetString()!;

                loVar = poBatchProcessPar.UserParameters.Where((x) => x.Key.Equals(PMB04000ContextDTO.LIST_REFNO)).FirstOrDefault()!.Value;
                string paramlistRefNo = ((System.Text.Json.JsonElement)loVar).GetString()!;

                poParamReport = new PMB04000ParamReportDTO
                {
                    CCOMPANY_ID = lcCompany,
                    CPROPERTY_ID = paramPropertyId,
                    CDEPT_CODE = paramDeptCode,
                    CREF_NO = paramlistRefNo,
                    CUSER_ID = lcUserId,
                    CLANG_ID = lcLangId,
                    LPRINT = false //0
                };
                #endregion

                loCommand = loDb.GetCommand();
                loConnection = loDb.GetConnection();
                _oKey = poBatchProcessPar.Key;

                _nCountError = 0;
                loReportConfiguration = R_ReportServerClientService.R_GetReportServerConfiguration();
                _oProcessor = new R_ProcessorClass<PMB04000DataReportDTO, R_Exception>(this);
                _oProcessor.R_ProcessAsync("key1", pnMinimumBatchThread: loReportConfiguration.MinimumBatchThread, pnMaximumBatchThread: loReportConfiguration.MaximumBatchThread);

            }
            catch (Exception ex)
            {
                loException.Add(ex);
                _logger.LogError(loException);
            }
            finally
            {
                if (loConnection != null)
                {
                    if (!(loConnection.State == ConnectionState.Closed))
                        loConnection.Close();
                    loConnection.Dispose();
                    loConnection = null;
                }

                if (loCommand != null)
                {
                    loCommand.Dispose();
                    loCommand = null;
                }
            }
        }

        public List<PMB04000DataReportDTO> R_InitData(string poKey)
        {
            string lcMethodName = nameof(R_InitData);
            using Activity activity = _activitySource.StartActivity(lcMethodName)!;
            _logger!.LogInfo(string.Format("START method {0}", lcMethodName));

            var loException = new R_Exception();
            List<PMB04000DataReportDTO> loReturn = null;
            CultureInfo loCultureInfo = new CultureInfo(R_BackGlobalVar.REPORT_CULTURE);
            try
            {
                var loCls = new PMB04000PrintCls();
                _logger.LogInfo("Get data from DB");
                loReturn = loCls.GetReportReceiptData(poParamReport);
            }
            catch (Exception ex)
            {
                loException.Add(ex);
                _logger.LogError(loException);
            }
            loException.ThrowExceptionIfErrors();
            _logger.LogInfo("END Method GenerateDataPrint on Controller");
            return loReturn!;
        }

        public void R_InitDataErrorStatus(string poKey, R_Exception poException)
        {
            R_Exception loException = new R_Exception();
            string lcMethodName = nameof(R_InitDataErrorStatus);
            using Activity activity = _activitySource.StartActivity(lcMethodName)!;
            try
            {
                string lcCmd;
                R_Db loDb = new R_Db();
                string lcErrMsg;
                int lnCountError;

                lock (_olock)
                    lnCountError = _nCountError;
                lcErrMsg = "ERROR run method R_InitDataErrorStatus";

                if (poException != null)
                {
                    if (poException.ErrorList.Count > 0)
                    {
                        lcErrMsg = string.Format("{0} with message {1}", lcErrMsg, poException.ErrorList.FirstOrDefault()!.ErrDescp);
                        lcErrMsg = lcErrMsg.Replace("'", "").Replace(((char)34).ToString(), "");
                    }
                }
                lcCmd = "insert into GST_UPLOAD_ERROR_STATUS(CCOMPANY_ID, CUSER_ID, CKEY_GUID, ISEQ_NO, CERROR_MESSAGE) ";
                lcCmd += string.Format("values('{0}', '{1}', '{2}', {3}, '{4}')", _oKey.COMPANY_ID.Trim(), _oKey.USER_ID.Trim(), _oKey.KEY_GUID.Trim(), lnCountError, lcErrMsg.Trim());
                loDb.SqlExecNonQuery(lcCmd);
            }
            catch (Exception ex)
            {
                loException.Add(ex);
                _logger!.LogError(loException);
            }
        }

        public void R_ProcessCompleteStatus(string poKey, List<R_Exception> poExceptions)
        {
            R_Exception loException = new R_Exception();
            string lcMethodName = nameof(R_SingleErrorStatus);
            using Activity activity = _activitySource.StartActivity(lcMethodName)!;

            try
            {
                string lcCmd;
                R_Db loDb = new R_Db();
                int lnFlag;

                if (poExceptions.Count > 0)
                    lnFlag = 9;
                else
                    lnFlag = 1;
                lcCmd = string.Format("exec RSP_WriteUploadProcessStatus '{0}','{1}','{2}',{3},'{4}',{5}",
                    _oKey.COMPANY_ID.Trim(), _oKey.USER_ID.Trim(), _oKey.KEY_GUID.Trim(), _GetPropCount(_nMaxData), "Simulation Finish", lnFlag);
                loDb.SqlExecNonQuery(lcCmd);
            }
            catch (Exception ex)
            {
                loException.Add(ex);
                _logger!.LogError(loException);
            }
        }

        public void R_SingleErrorStatus(string poKey, PMB04000DataReportDTO poParameter, R_Exception poException)
        {
            R_Exception loException = new R_Exception();
            string lcMethodName = nameof(R_SingleErrorStatus);
            using Activity activity = _activitySource.StartActivity(lcMethodName)!;

            try
            {
                string lcCmd;
                R_Db loDb = new R_Db();
                string lcErrMsg;
                int lnCountError;
                string lcError = "";

                lock (_olock)
                {
                    lnCountError = _nCountError;
                    _nCountError += 1;
                }
                if (poException.Haserror)
                {
                    lcError = poException.ErrorList.FirstOrDefault().ErrDescp;
                    lcError.Replace("\"", "").Replace("'", "");
                }
                lcCmd = "INSERT INTO GST_UPLOAD_ERROR_STATUS(CCOMPANY_ID, CUSER_ID, CKEY_GUID, ISEQ_NO, CERROR_MESSAGE) ";
                lcCmd += string.Format("values('{0}', '{1}', '{2}', {3}, 'Error Invoice Id {4}')", _oKey.COMPANY_ID.Trim(), _oKey.USER_ID.Trim(), _oKey.KEY_GUID.Trim(), lnCountError, lcError.ToString());
                loDb.SqlExecNonQuery(lcCmd);
            }
            catch (Exception ex)
            {
                loException.Add(ex);
                _logger!.LogError(loException);
            }
        }

        public async Task R_SingleProcessAsync(string poKey, PMB04000DataReportDTO poParameter)
        {
            R_Exception loException = new R_Exception();
            string lcMethodName = nameof(R_SingleProcessAsync);
            using Activity activity = _activitySource.StartActivity(lcMethodName)!;

            string lcCmd;
            R_Db loDb = null;
            DbConnection loConn = null;
            R_ConnectionAttribute loConnAttr;
            int lnCountProcess;
            PMB04000ResultDataDTO loData = null;
            R_GenerateReportParameter loParameter;
            R_ReportServerRule loReportRule;
            string lcReportFileName;
            string lcMime;
            byte[] loRtnInByte;
            R_FileType leReportOutputType;
            string lcExtension;
            R_SaveResult loSaveResult;
            try
            {
                lock (_olock)
                {
                    lnCountProcess = _nCountProcess;
                    _nCountProcess += 1;
                }
                loDb = new R_Db();

                _logger!.LogInfo("Start Single process");
                lcCmd = string.Format("exec RSP_WriteUploadProcessStatus '{0}','{1}','{2}',{3},'{4}'", _oKey.COMPANY_ID.Trim(), _oKey.USER_ID.Trim(), _oKey.KEY_GUID.Trim(), _GetPropCount(lnCountProcess), string.Format("Process data Ref NO {0}", poParameter.CREF_NO));
                loDb.SqlExecNonQuery(lcCmd);

                loData = GenerateDataPrint(poParamReport);
                //Set Report Rule and Name
                loReportRule = new R_ReportServerRule("t01", "c01");
                lcReportFileName = "PMB04000.frx";

                //Prepare Parameter
                leReportOutputType = R_FileType.PDF;
                lcExtension = Enum.GetName(typeof(R_FileType), leReportOutputType)!;
                loParameter = new R_GenerateReportParameter()
                {
                    ReportRule = loReportRule,
                    ReportFileName = lcReportFileName,
                    ReportData = JsonSerializer.Serialize<PMB04000ResultDataDTO>(loData),
                    ReportDataSourceName = "ResponseDataModel",
                    ReportFormat = new R_ReportFormatDTO()
                    {
                        DecimalPlaces = R_BackGlobalVar.REPORT_FORMAT_DECIMAL_PLACES,
                        DecimalSeparator = R_BackGlobalVar.REPORT_FORMAT_DECIMAL_SEPARATOR,
                        GroupSeparator = R_BackGlobalVar.REPORT_FORMAT_GROUP_SEPARATOR,
                        ShortDate = R_BackGlobalVar.REPORT_FORMAT_SHORT_DATE,
                        ShortTime = R_BackGlobalVar.REPORT_FORMAT_SHORT_TIME,
                        LongDate = R_BackGlobalVar.REPORT_FORMAT_LONG_DATE,
                        LongTime = R_BackGlobalVar.REPORT_FORMAT_LONG_TIME
                    },
                    ReportDataType = typeof(PMB04000ResultDataDTO).ToString(),
                    ReportOutputType = leReportOutputType,
                    ReportAssemblyName = "PMB04000COMMON.dll", 
                    ReportParameter = null
                };
                //GENERATE REPORT
                loRtnInByte = await R_ReportServerUtility.R_GenerateReportByte(R_ReportServerClientService.R_GetHttpClient(), 
                    "api/ReportServer/GetReport", loParameter); ///menghubungkan ke API dari engine yang sudah ada


                #region Transaction block
                //gunakan transaction block
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.))
                {

                }

                    loConn = loDb.GetConnection();
                loConnAttr = loDb.GetConnectionAttribute();
                
                #endregion

            }
            catch (Exception ex)
            {
                loException.Add(ex);
                _logger!.LogError(loException);
            }
            finally
            {
                if (loConn != null)
                {
                    if ((loConn.State == ConnectionState.Closed) == false)
                    {
                        loConn.Close();
                    }
                    loConn = null;
                }
                if (loDb != null)
                {
                    loDb = null;
                }
            }
        EndBlock:
            loException.ThrowExceptionIfErrors();
            await Task.CompletedTask;
        }

        public void R_SingleSuccessStatus(string poKey, PMB04000DataReportDTO poParameter)
        {
            R_Exception loException = new R_Exception();
            string lcMethodName = nameof(R_SingleSuccessStatus);
            using Activity activity = _activitySource.StartActivity(lcMethodName)!;
            try
            {
                string lcCmd;
                R_Db loDb = new R_Db();
                int lnCountProcess;
                lock (_olock)
                    lnCountProcess = _nCountProcess;

                lcCmd = string.Format("exec RSP_WriteUploadProcessStatus '{0}','{1}','{2}',{3},'{4}'",
                    _oKey.COMPANY_ID.Trim(), _oKey.USER_ID.Trim(), _oKey.KEY_GUID.Trim(), _GetPropCount(lnCountProcess), string.Format("Process data CREF {0}", poParameter.CREF_NO));
                loDb.SqlExecNonQuery(lcCmd);
            }
            catch (Exception ex)
            {
                loException.Add(ex);
                _logger!.LogError(loException);
            }

        }

        public void R_Status(string poKey, string pcMessage)
        {
            R_Exception loException = new R_Exception();
            string lcMethodName = nameof(R_Status);
            using Activity activity = _activitySource.StartActivity(lcMethodName)!;

            try
            {
                string lcCmd;
                R_Db loDb = new R_Db();
                int lnCountProcess;

                lock (_olock)
                    lnCountProcess = _nCountProcess;

                lcCmd = string.Format("exec RSP_WriteUploadProcessStatus '{0}','{1}','{2}',{3},'{4}'",
                    _oKey.COMPANY_ID.Trim(), _oKey.USER_ID.Trim(), _oKey.KEY_GUID.Trim(), _GetPropCount(lnCountProcess), pcMessage.Trim());
                loDb.SqlExecNonQuery(lcCmd);
            }
            catch (Exception ex)
            {
                loException.Add(ex);
                _logger!.LogError(loException);

            }
        }




        #region Region PRINT AND SEND RECEIPT
        public List<PMB04000SendReceiptDTO> SendReceiptCls(PMB04000ParamReportDTO poParameter)
        {
            string? lcMethodName = nameof(SendReceiptCls);
            using Activity activity = _activitySource.StartActivity(lcMethodName)!;
            _logger!.LogInfo(string.Format("START process method {0} on Cls", lcMethodName));


            R_Exception loException = new();
            List<PMB04000SendReceiptDTO>? loReturn = null;
            string lcQuery;
            DbCommand loCommand;
            R_Db loDb;
            try
            {
                loDb = new();
                DbConnection? loConn = loDb.GetConnection();
                loCommand = loDb.GetCommand();
                lcQuery = "RSP_PM_SEND_RECEIPT";
                loCommand.CommandText = lcQuery;
                loCommand.CommandType = CommandType.StoredProcedure;

                loDb.R_AddCommandParameter(loCommand, "@CCOMPANY_ID", DbType.String, 20, poParameter.CCOMPANY_ID);
                loDb.R_AddCommandParameter(loCommand, "@CPROPERTY", DbType.String, 20, poParameter.CPROPERTY_ID);
                loDb.R_AddCommandParameter(loCommand, "@CDEPT_CODE", DbType.String, 20, poParameter.CDEPT_CODE);
                loDb.R_AddCommandParameter(loCommand, "@CREF_NO", DbType.String, int.MaxValue, poParameter.LIST_REFNO);
                loDb.R_AddCommandParameter(loCommand, "@CUSER_ID", DbType.String, 20, poParameter.CUSER_ID);
                loDb.R_AddCommandParameter(loCommand, "@CLANG_ID", DbType.String, 3, poParameter.CLANG_ID);

                var loDbParam = loCommand.Parameters.Cast<DbParameter>()
                    .Where(x => x != null && x.ParameterName.StartsWith("@"))
                    .ToDictionary(x => x.ParameterName, x => x.Value);
                _logger.LogDebug("{@ObjectQuery} {@Parameter}", loCommand.CommandText, loDbParam);

                var loDataTable = loDb.SqlExecQuery(loConn, loCommand, true);
                loReturn = R_Utility.R_ConvertTo<PMB04000SendReceiptDTO>(loDataTable).ToList();
            }
            catch (Exception ex)
            {
                loException.Add(ex);
                _logger.LogError(loException);
            }
            if (loException.Haserror)
            {
                loException.ThrowExceptionIfErrors();
            }
            _logger.LogInfo(string.Format("END process method {0} on Cls", lcMethodName));
#pragma warning disable CS8603 // Possible null reference return.
            return loReturn;
#pragma warning restore CS8603 // Possible null reference return.
        }
        private List<PMB04000DataReportDTO> ConvertResultToFormatPrint(List<PMB04000DataReportDTO> poCollectionDataRaw)
        {
            var loException = new R_Exception();
            string lcMethodName = nameof(ConvertResultToFormatPrint);
            using Activity activity = _activitySource.StartActivity(lcMethodName)!;
            _logger.LogInfo(string.Format("START method {0} ", lcMethodName));
            List<PMB04000DataReportDTO> loReturn = poCollectionDataRaw;

            try
            {
                foreach (var item in loReturn)
                {
                    item.DINVOICE_DATE = ConvertStringToDateTimeFormat(item.CINVOICE_DATE);
                    item.DTODAY_DATE = ConvertStringToDateTimeFormat(item.CTODAY_DATE);
                }
            }
            catch (Exception ex)
            {
                loException.Add(ex);
                _logger.LogError(loException);
            }
            loException.ThrowExceptionIfErrors();
            _logger.LogInfo(string.Format("END process method {0} on Cls", lcMethodName));
            return loReturn;

        }

        #endregion
        #region Helper
        private PMB04000ResultDataDTO GenerateDataPrint(PMB04000ParamReportDTO poParam)
        {
            string lcMethodName = nameof(GenerateDataPrint);
            using Activity activity = _activitySource.StartActivity(lcMethodName)!;
            _logger.LogInfo(string.Format("START method {0}", lcMethodName));

            var loException = new R_Exception();
            PMB04000ResultDataDTO loRtn = new PMB04000ResultDataDTO();
            PMB04000PrintCls? loCls = null;
            //PMR00150SummaryResultDTO? loData = null;
            CultureInfo loCultureInfo = new CultureInfo(R_BackGlobalVar.REPORT_CULTURE);
            try
            {
                loCls = new PMB04000PrintCls();
                var loCollectionFromDb = loCls.GetReportReceiptData(poParam);
                _logger.LogInfo("Set BaseHeader Report");

                PMB04000DataReportDTO? loDataFirst = loCollectionFromDb.Any() == true ? loCollectionFromDb.FirstOrDefault() : new PMB04000DataReportDTO()!;

                PMB04000BaseHeaderDTO loHeader = new();
                var loGetLogo = loCls.GetLogoCompany(poParam);
                //SET LOGO DAN HEADER
                _logger.LogInfo("Set LOGO AND HEADER Report");

                loHeader.KWITANSI = R_Utility.R_GetMessage(typeof(Resources_PMB04000), "Kwitansi", loCultureInfo);
                loHeader.CLOGO = loGetLogo.CLOGO!;

                _logger.LogInfo("Set Column and Label Report");
                var loColumn = AssignValuesWithMessages(typeof(Resources_PMB04000), loCultureInfo, new PMB04000ColumnDTO());
                var loLabel = AssignValuesWithMessages(typeof(Resources_PMB04000), loCultureInfo, new PMB04000LabelDTO());


                //CONVERT DATA TO DISPLAY IF DATA EXIST
                var oListData = loCollectionFromDb.Any() ? ConvertResultToFormatPrint(loCollectionFromDb) : new List<PMB04000DataReportDTO>();

                //Assign DATA
                _logger.LogInfo("Set Data Report");
                loRtn.Header = loHeader;
                loRtn.Column = (PMB04000ColumnDTO)loColumn;
                loRtn.Label = (PMB04000LabelDTO)loLabel;
                loRtn.Data = oListData;
            }
            catch (Exception ex)
            {
                loException.Add(ex);
                _logger.LogError(loException);
            }
            loException.ThrowExceptionIfErrors();
            _logger.LogInfo("END Method GenerateDataPrint on Controller");

            return loRtn;
        }
        #endregion
        #region Utilities
        private int _GetPropCount(int pnCount)
        {
            if (_nMaxData == 0)
                return 0;
            return Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(100) * pnCount / _nMaxData));
        }
        private static DateTime? ConvertStringToDateTimeFormat(string? pcEntity)
        {
            if (string.IsNullOrWhiteSpace(pcEntity))
            {
                return null;
            }
            else
            {
                DateTime result;
                if (pcEntity.Length == 6)
                {
                    pcEntity += "01";
                }

                if (DateTime.TryParseExact(pcEntity, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
                {
                    return result;
                }
                else
                {
                    return null;
                }
            }
        }
        //Helper Assign Object
        private object AssignValuesWithMessages(Type poResourceType, CultureInfo poCultureInfo, object poObject)
        {
            object loObj = Activator.CreateInstance(poObject.GetType())!;
            var loGetPropertyObject = poObject.GetType().GetProperties();

            foreach (var property in loGetPropertyObject)
            {
                string propertyName = property.Name;
                string message = R_Utility.R_GetMessage(poResourceType, propertyName, poCultureInfo);
                property.SetValue(loObj, message);
            }

            return loObj;
        }

        #endregion
    }

}
