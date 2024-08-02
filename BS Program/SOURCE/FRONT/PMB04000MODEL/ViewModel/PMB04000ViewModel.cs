using Global_PMCOMMON.DTOs.Response.Invoice_Type;
using Global_PMCOMMON.DTOs.Response.Property;
using Global_PMModel;
using PMB04000COMMON.Context;
using PMB04000COMMON.DTO.DTOs;
using PMB04000COMMON.DTO.Utilities;
using PMB04000COMMON.Print.Param_DTO;
using PMB04000FrontResources;
using R_APICommonDTO;
using R_BlazorFrontEnd;
using R_BlazorFrontEnd.Exceptions;
using R_BlazorFrontEnd.Helpers;
using R_CommonFrontBackAPI;
using R_ProcessAndUploadFront;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMB04000MODEL.ViewModel
{
    public class PMB04000ViewModel : R_ViewModel<PMB04000DTO>, R_IProcessProgressStatus
    {
        #region From Back

        private readonly PMB04000Model _model = new PMB04000Model();
        private readonly GlobalFunctionModel _modelGlobalPM = new GlobalFunctionModel();
        public List<PropertyDTO> loPropertyList = new List<PropertyDTO>();
        public List<InvoiceTypeDTO> loInvoiceType = new List<InvoiceTypeDTO>();
        public ObservableCollection<PMB04000DTO> loOfficialReceipt = new ObservableCollection<PMB04000DTO>();
        #endregion

        #region For Front
        public PMB04000ParamDTO oParameterInvoiceReceipt = new PMB04000ParamDTO();
        public PMB04000ParamReportDTO oParameterPrintReceipt = new PMB04000ParamReportDTO();
        public List<PeriodMonthDTO>? GetMonthList;
        public PeriodYearDTO oPeriodYearRange = new PeriodYearDTO();
        public string? cPeriodParam;
        public DateTime? DCreateReceipt;
        public List<PMB04000DTO> loDataProcess = new List<PMB04000DTO>();
        public Action? StateChangeAction { get; set; }
        public Action<R_Exception>? DisplayErrorAction { get; set; }
        public Action? ShowSuccessAction { get; set; }
        public bool llEnableBtn;
        public string Message = "";
        public int Percentage = 0;
        public bool _isSuccess;
        public string? COMPANYID;
        public string? USERID;
        public string? pcTYPE_PROCESS;
        #endregion

        #region Program
        public async Task GetPropertyList()
        {
            R_Exception loEx = new R_Exception();
            try
            {
                var loResult = await _modelGlobalPM.PropertyListAsync();
                if (loResult.Data.Any())
                {
                    loPropertyList = new List<PropertyDTO>(loResult.Data);
                    if (string.IsNullOrEmpty(oParameterInvoiceReceipt.CPROPERTY_ID))
                    {
                        oParameterInvoiceReceipt.CPROPERTY_ID = loResult.Data.First().CPROPERTY_ID!;
                    }
                }
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }
            loEx.ThrowExceptionIfErrors();
        }
        public async Task GetInvoiceTypeList()
        {
            R_Exception loEx = new R_Exception();
            try
            {
                var loParameter = new InvoiceTypeParameterDTO
                {
                    CCLASS_APPLICATION = "BIMASAKTI",
                    CCLASS_ID = "_BS_INVOICE_TYPE",
                    CCLASS_RECID = "01,02,08,09"
                };

                var loResult = await _modelGlobalPM.InvoiceTypeAsync(loParameter);
                if (loResult.Data.Any())
                {
                    loInvoiceType = new List<InvoiceTypeDTO>(loResult.Data);
                    if (string.IsNullOrEmpty(oParameterInvoiceReceipt.CINVOICE_TYPE))
                    {
                        oParameterInvoiceReceipt.CINVOICE_TYPE = loResult.Data.First().CCODE!;
                    }
                }
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }
            loEx.ThrowExceptionIfErrors();
        }
        public async Task GetOfficialReceiptList()
        {
            R_Exception loEx = new R_Exception();
            try
            {
                var tempPeriod = (oParameterInvoiceReceipt.IPERIOD_YEAR.ToString()) + oParameterInvoiceReceipt.CPERIOD_MONTH;
                oParameterInvoiceReceipt.CPERIOD = tempPeriod == "0" ? "" : tempPeriod;
                R_FrontContext.R_SetStreamingContext(PMB04000ContextDTO.CPROPERTY_ID, oParameterInvoiceReceipt.CPROPERTY_ID);
                R_FrontContext.R_SetStreamingContext(PMB04000ContextDTO.CDEPT_CODE, oParameterInvoiceReceipt.CDEPT_CODE);
                R_FrontContext.R_SetStreamingContext(PMB04000ContextDTO.CTENANT_ID, oParameterInvoiceReceipt.CTENANT_ID);
                R_FrontContext.R_SetStreamingContext(PMB04000ContextDTO.LALL_TENANT, oParameterInvoiceReceipt.LALL_TENANT);
                R_FrontContext.R_SetStreamingContext(PMB04000ContextDTO.CINVOICE_TYPE, oParameterInvoiceReceipt.CINVOICE_TYPE);
                R_FrontContext.R_SetStreamingContext(PMB04000ContextDTO.CPERIOD, oParameterInvoiceReceipt.CPERIOD);
                R_FrontContext.R_SetStreamingContext(PMB04000ContextDTO.CTRANS_CODE, oParameterInvoiceReceipt.CTRANS_CODE);

                var loResult = await _model.GetInvReceiptListAsync();
                if (loResult.Data.Any())
                {
                    llEnableBtn = true;
                    foreach (var item in loResult.Data!)
                    {
                        item.DREF_DATE = ConvertStringToDateTimeFormat(item.CREF_DATE!)!;
                    }
                }
                else
                {
                    llEnableBtn = true;
                }
                loOfficialReceipt = new ObservableCollection<PMB04000DTO>(loResult.Data);
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }
            loEx.ThrowExceptionIfErrors();
        }
        public void ValidationParamToGetList()
        {
            var loEx = new R_Exception();
            try
            {
                if (string.IsNullOrWhiteSpace(oParameterInvoiceReceipt.CPROPERTY_ID))
                {
                    var loErr = R_FrontUtility.R_GetError(typeof(Resources_PMB0400_Class), "_validationProperty");
                    loEx.Add(loErr);
                }
                if (string.IsNullOrWhiteSpace(oParameterInvoiceReceipt.CDEPT_CODE))
                {
                    var loErr = R_FrontUtility.R_GetError(typeof(Resources_PMB0400_Class), "_validationDept");
                    loEx.Add(loErr);
                }
                if (string.IsNullOrWhiteSpace(oParameterInvoiceReceipt.CTENANT_ID) && !oParameterInvoiceReceipt.LALL_TENANT)
                {
                    var loErr = R_FrontUtility.R_GetError(typeof(Resources_PMB0400_Class), "_validationTenant");
                    loEx.Add(loErr);
                }
                if (string.IsNullOrWhiteSpace(oParameterInvoiceReceipt.CINVOICE_TYPE))
                {
                    var loErr = R_FrontUtility.R_GetError(typeof(Resources_PMB0400_Class), "_validationInvoiceType");
                    loEx.Add(loErr);
                }
                if ((!oParameterInvoiceReceipt.LALL_PERIOD) &&
                    (oParameterInvoiceReceipt.IPERIOD_YEAR < oPeriodYearRange.IMIN_YEAR
                    || string.IsNullOrWhiteSpace(oParameterInvoiceReceipt.CPERIOD_MONTH)))
                {
                    var loErr = R_FrontUtility.R_GetError(typeof(Resources_PMB0400_Class), "_validationPeriod");
                    loEx.Add(loErr);
                }
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }
            if (loEx.HasError)
            {
                loEx.ThrowExceptionIfErrors();
            }
        }
        #endregion
        #region Process Batch
        public List<PMB04000DTO> ValidationProcessData(List<PMB04000DTO> poListData)
        {
            R_Exception loException = new R_Exception();
            List<PMB04000DTO> loReturn = new List<PMB04000DTO>();
            try
            {
                string lcResourceMessage =
                    pcTYPE_PROCESS == "CREATE_RECEIPT"
                    ? "ValidationDataSelectedCreate"
                    : "ValidationDataSelectedCancel";

                switch (pcTYPE_PROCESS)
                {
                    case "CREATE_RECEIPT":
                        lcResourceMessage = "ValidationDataSelectedCreate";
                        break;
                    case "CANCEL_RECEIPT":
                        lcResourceMessage = "ValidationDataSelectedCancel";
                        break;
                    case "PRINT":
                        lcResourceMessage = "ValidationDataSelectedPrint";
                        break;
                    default:
                        break;
                }

                if (pcTYPE_PROCESS == "CREATE_RECEIPT")
                {
                    if (DCreateReceipt == null)
                    {
                        var loErr = R_FrontUtility.R_GetError(typeof(Resources_PMB0400_Class), "ValidationCreateDate");
                        loException.Add(loErr);
                        goto EndBlock;
                    }
                }

                List<PMB04000DTO> tempDataSelected =
                    poListData.Where(x => x.LSELECTED == true)
                    .Select((item, index) =>
                    {
                        item.INO = index + 1; // index + 1 karena kita ingin memulai No dari 1, bukan 0
                        return item;
                    })
                    .ToList();

                if (tempDataSelected.Count < 1)
                {
                    var loErr = R_FrontUtility.R_GetError(typeof(Resources_PMB0400_Class), lcResourceMessage);
                    loException.Add(loErr);
                    goto EndBlock;
                }
                loReturn = tempDataSelected;
            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }
        EndBlock:
            loException.ThrowExceptionIfErrors();
            return loReturn;
        }
        #endregion
        #region Utilities
        public void GetMonth()
        {
            GetMonthList = new List<PeriodMonthDTO>();
            for (int i = 1; i <= 12; i++)
            {
                string monthId = i.ToString("D2");
                PeriodMonthDTO month = new PeriodMonthDTO { Id = monthId };
                GetMonthList.Add(month);
            }
            oParameterInvoiceReceipt.CPERIOD_MONTH = DateTime.Now.Month.ToString("D2");
        }
        public async Task GetPeriodRangeYear()
        {
            var loEx = new R_Exception();
            try
            {
                var loReturn = await _model.GetPeriodYearRangeAsync();
                if (loReturn != null)
                {
                    oPeriodYearRange = loReturn;
                }
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }
            loEx.ThrowExceptionIfErrors();
        }
        private DateTime? ConvertStringToDateTimeFormat(string? pcEntity)
        {
            if (string.IsNullOrWhiteSpace(pcEntity))
            {
                // Jika string kosong atau null, kembalikan DateTime.MinValue atau nilai default yang sesuai
                //return DateTime.MinValue; // atau DateTime.MinValue atau DateTime.Now atau nilai default yang sesuai dengan kebutuhan Anda
                return null;
            }
            else
            {
                // Parse string ke DateTime
                DateTime result;
                if (DateTime.TryParseExact(pcEntity, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
                {
                    return result;
                }
                else
                {
                    // Jika parsing gagal, kembalikan DateTime.MinValue atau nilai default yang sesuai
                    //return DateTime.MinValue; // atau DateTime.MinValue atau DateTime.Now atau nilai default yang sesuai dengan kebutuhan Anda
                    return null;
                }
            }
        }
        private string? ConvertDateTimeToStringFormat(DateTime? ptEntity)
        {
            if (!ptEntity.HasValue || ptEntity.Value == null)
            {
                // Jika ptEntity adalah null atau DateTime.MinValue, kembalikan null
                return null;
            }
            else
            {
                // Format DateTime ke string "yyyyMMdd"
                return ptEntity.Value.ToString("yyyyMMdd");
            }
        }
        #endregion
        #region "Process"
        public async Task ProcessDataSelected(PMB04000ParamDTO poParam, List<PMB04000DTO> poListData)
        {
            var loEx = new R_Exception();
            try
            {
                var loUserParameters = new List<R_KeyValue>();
                // set Param
                loUserParameters = new List<R_KeyValue>
                {
                    new R_KeyValue()
                    { Key = PMB04000ContextDTO.CCREATE_RECEIPT_DATE, Value = ConvertDateTimeToStringFormat(DCreateReceipt)!},
                     new R_KeyValue()
                    { Key = PMB04000ContextDTO.CTYPE_PROCESS, Value = poParam.CTYPE_PROCESS!},
                };
                //CONVERT DATA DTO
                List<PMB04000ProcessDTO> loParamListData = R_FrontUtility.ConvertCollectionToCollection<PMB04000ProcessDTO>(poListData).ToList();

                //Instantiate ProcessClient
                R_ProcessAndUploadClient loCls = new R_ProcessAndUploadClient(
                    pcModuleName: "PM",
                    plSendWithContext: true,
                    plSendWithToken: true,
                    pcHttpClientName: "R_DefaultServiceUrlPM",
                    poProcessProgressStatus: this);

                //prepare Batch Parameter
                R_BatchParameter loUploadPar = new R_BatchParameter();
                loUploadPar.COMPANY_ID = COMPANYID = poParam.CCOMPANY_ID!;
                loUploadPar.USER_ID = USERID = poParam.CUSER_ID!; ;
                loUploadPar.UserParameters = loUserParameters;
                loUploadPar.ClassName = "PMB04000BACK.PMB04000ProcessCreateCls";
                loUploadPar.BigObject = loParamListData;
                await loCls.R_BatchProcess<List<string>>(loUploadPar, 10);
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }
        }
        #endregion
        #region ProgressBar
        public async Task ProcessComplete(string pcKeyGuid, eProcessResultMode poProcessResultMode)
        {
            if (pcTYPE_PROCESS == "CREATE_RECEIPT")
            {
                if (poProcessResultMode == eProcessResultMode.Success)
                {
                    Message = string.Format("Finish Process Create Receipt!");
                    ShowSuccessAction!();
                    _isSuccess = true;
                }
                if (poProcessResultMode == eProcessResultMode.Fail)
                {
                    _isSuccess = false;

                    Message = "Process Completed With Fail";
                    try
                    {
                        await ServiceGetError(pcKeyGuid);
                    }
                    catch (R_Exception ex)
                    {
                        DisplayErrorAction?.Invoke(ex);
                    }
                }
            }
            else if (pcTYPE_PROCESS == "CANCEL_RECEIPT")
            {
                if (poProcessResultMode == eProcessResultMode.Success)
                {
                    Message = string.Format("Finish Process Cancel Receipt!");
                    ShowSuccessAction!();
                    _isSuccess = true;
                }
                if (poProcessResultMode == eProcessResultMode.Fail)
                {
                    _isSuccess = false;

                    Message = "Process Completed With Fail";
                    try
                    {
                        await ServiceGetError(pcKeyGuid);
                    }
                    catch (R_Exception ex)
                    {
                        DisplayErrorAction?.Invoke(ex);
                    }
                }
            }
            StateChangeAction!();
            await Task.CompletedTask;
        }

        public async Task ProcessError(string pcKeyGuid, R_APIException ex)
        {
            R_Exception loException = new R_Exception();
            Message = string.Format("Process Error with GUID {0}", pcKeyGuid);

            if (pcTYPE_PROCESS == "CREATE_RECEIPT")
            {
                ex.ErrorList.ForEach(x => loException.Add(x.ErrNo, x.ErrDescp));
            }
            else if (pcTYPE_PROCESS == "CANCEL_RECEIPT")
            {
                ex.ErrorList.ForEach(x => loException.Add(x.ErrNo, x.ErrDescp));
            }

            DisplayErrorAction!.Invoke(loException);
            StateChangeAction!();
            await Task.CompletedTask;
        }

        public async Task ReportProgress(int pnProgress, string pcStatus)
        {
            if (pcTYPE_PROCESS == "CREATE_RECEIPT")
            {
                Percentage = pnProgress;
                Message = string.Format("Process Progress {0} with status {1}", pnProgress, pcStatus);
            }
            else if (pcTYPE_PROCESS == "CANCEL_RECEIPT")
            {
                Percentage = pnProgress;
                Message = string.Format("Process Progress {0} with status {1}", pnProgress, pcStatus);
            }
            StateChangeAction!();
            await Task.CompletedTask;
        }
        private async Task ServiceGetError(string pcKeyGuid)
        {
            R_Exception loException = new R_Exception();

            List<R_ErrorStatusReturn> loResultData;
            R_GetErrorWithMultiLanguageParameter loParameterData;
            R_ProcessAndUploadClient loCls;
            try
            {
                loParameterData = new R_GetErrorWithMultiLanguageParameter()
                {
                    COMPANY_ID = COMPANYID!,
                    USER_ID = USERID!,
                    KEY_GUID = pcKeyGuid,
                    RESOURCE_NAME = "RSP_PM_CREATE_OFFICIAL_RECEIPT"
                };
                loCls = new R_ProcessAndUploadClient(
                   pcModuleName: "PM",
                   plSendWithContext: true,
                   plSendWithToken: true,
                   pcHttpClientName: "R_DefaultServiceUrlPM",
                   poProcessProgressStatus: this);
                loResultData = await loCls.R_GetStreamErrorProcess(loParameterData);
                loResultData.ForEach(x => loException.Add(x.SeqNo.ToString(), x.ErrorMessage));
                goto EndBlock;
            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }
        EndBlock:
            loException.ThrowExceptionIfErrors();

        }
        #endregion

    }
}
