using BlazorClientHelper;
using CBT00200COMMON;
using CBT00200FrontResources;
using CBT00200MODEL;
using Lookup_GSCOMMON.DTOs;
using Lookup_GSFRONT;
using Lookup_GSModel.ViewModel;
using Microsoft.AspNetCore.Components;
using R_BlazorFrontEnd.Controls;
using R_BlazorFrontEnd.Controls.DataControls;
using R_BlazorFrontEnd.Controls.Enums;
using R_BlazorFrontEnd.Controls.Events;
using R_BlazorFrontEnd.Controls.MessageBox;
using R_BlazorFrontEnd.Enums;
using R_BlazorFrontEnd.Exceptions;
using R_BlazorFrontEnd.Helpers;
using R_BlazorFrontEnd.Interfaces;
using R_CommonFrontBackAPI;
using R_LockingFront;
using System.Globalization;

namespace CBT00200FRONT
{
    public partial class CBT00220 : R_Page
    {
        private CBT00220ViewModel _JournalEntryDepositViewModel = new();
        private CBT00210ViewModel _JournalEntryViewModel = new();
        private CBT00200ViewModel _JournalListViewModel = new();
        private R_Conductor _conductorRef;
        private R_Grid<CBT00210DTO> _gridDetailRef;

        [Inject] IClientHelper clientHelper { get; set; }
        [Inject] private R_ILocalizer<CBT00200FrontResources.Resources_Dummy_Class> _localizer { get; set; }

        #region Private Property
        private string lcLabelSubmit = "Submit";
        private string lcLabelCommit = "Commit";
        private bool EnableEdit = false;
        private bool EnableDelete = false;
        private bool EnableSubmit = false;
        private bool EnableApprove = false;
        private bool EnableCommit = false;
        private bool EnableHaveRecId = false;
        private bool EnableDetailNormalMode;
        private bool EnableHeaderNormalMode;
        private R_TextBox _DeptCode_TextBox;
        private bool EnableCenterList = true;
        private CBT00200ParamDepositDTO _ParameterDeposit = new();
        #endregion

        protected override async Task R_Init_From_Master(object poParameter)
        {
            var loEx = new R_Exception();

            try
            {
                var loParam = R_FrontUtility.ConvertObjectToObject<CBT00200ParamDepositDTO>(poParameter);
                await _JournalEntryViewModel.GetAllUniversalData();
                _ParameterDeposit = loParam;
                if (loParam.PARAM_CALLER_ACTION == "NEW")
                {
                    await _conductorRef.Add();
                }
                else
                {
                    await _conductorRef.R_GetEntity(loParam);
                }
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            R_DisplayException(loEx);
        }

        #region Locking
        private const string DEFAULT_HTTP_NAME = "R_DefaultServiceUrlCB";
        private const string DEFAULT_MODULE_NAME = "CB";
        protected async override Task<bool> R_LockUnlock(R_LockUnlockEventArgs eventArgs)
        {
            var loEx = new R_Exception();
            var llRtn = false;
            R_LockingFrontResult loLockResult = null;

            try
            {
                var loData = (CBT00200DTO)eventArgs.Data;

                var loCls = new R_LockingServiceClient(pcModuleName: DEFAULT_MODULE_NAME,
                    plSendWithContext: true,
                    plSendWithToken: true,
                    pcHttpClientName: DEFAULT_HTTP_NAME);

                if (eventArgs.Mode == R_eLockUnlock.Lock)
                {
                    var loLockPar = new R_ServiceLockingLockParameterDTO
                    {
                        Company_Id = clientHelper.CompanyId,
                        User_Id = clientHelper.UserId,
                        Program_Id = "CBT00200",
                        Table_Name = "CBT_TRANS_HD",
                        Key_Value = string.Join("|", clientHelper.CompanyId, loData.CDEPT_CODE, loData.CTRANS_CODE, loData.CREF_NO)
                    };

                    loLockResult = await loCls.R_Lock(loLockPar);
                }
                else
                {
                    var loUnlockPar = new R_ServiceLockingUnLockParameterDTO
                    {
                        Company_Id = clientHelper.CompanyId,
                        User_Id = clientHelper.UserId,
                        Program_Id = "CBT00200",
                        Table_Name = "CBT_TRANS_HD",
                        Key_Value = string.Join("|", clientHelper.CompanyId, loData.CDEPT_CODE, loData.CTRANS_CODE, loData.CREF_NO)
                    };

                    loLockResult = await loCls.R_UnLock(loUnlockPar);
                }

                llRtn = loLockResult.IsSuccess;
                if (!loLockResult.IsSuccess && loLockResult.Exception != null)
                    throw loLockResult.Exception;
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            loEx.ThrowExceptionIfErrors();

            return llRtn;
        }
        #endregion

        #region Form
        private async Task JournalForm_GetRecord(R_ServiceGetRecordEventArgs eventArgs)
        {
            var loEx = new R_Exception();
            try
            {
                var loParam = R_FrontUtility.ConvertObjectToObject<CBT00200DTO>(eventArgs.Data);

                await _JournalEntryViewModel.GetJournal(loParam);
                eventArgs.Result = _JournalEntryViewModel.Journal;
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            loEx.ThrowExceptionIfErrors();
        }
        private async Task JournalForm_ServiceSave(R_ServiceSaveEventArgs eventArgs)
        {
            var loEx = new R_Exception();
            try
            {
                var loCRUDMode = (eCRUDMode)eventArgs.ConductorMode;
                
                var loData = (CBT00200DTO)eventArgs.Data;
                loData.CREF_NO = string.IsNullOrWhiteSpace(loData.CREF_NO) ? "" : loData.CREF_NO;
                loData.CREF_DATE = _JournalEntryViewModel.RefDate.Value.ToString("yyyyMMdd");
                loData.CDOC_DATE = _JournalEntryViewModel.DocDate.Value.ToString("yyyyMMdd");

                await _JournalEntryDepositViewModel.SaveJournalDeposit(loData, loCRUDMode);
                eventArgs.Result = _JournalEntryDepositViewModel.Journal;
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            loEx.ThrowExceptionIfErrors();
        }
        private async Task BtnDelete_OnClick()
        {
            var loEx = new R_Exception();
            try
            {
                var loValidate = await R_MessageBox.Show("", _localizer["Q03"], R_eMessageBoxButtonType.YesNo);
                if (loValidate == R_eMessageBoxResult.No)
                    goto EndBlock;

                var loData = (CBT00200DTO)_conductorRef.R_GetCurrentData();
                var loParam = R_FrontUtility.ConvertObjectToObject<CBT00200UpdateStatusDTO>(loData);
                loParam.LAUTO_COMMIT = false;
                loParam.LUNDO_COMMIT = false;
                loParam.CNEW_STATUS = "99";

                await _JournalListViewModel.UpdateJournalStatus(loParam);
                await _conductorRef.R_GetEntity(loData);
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }
        EndBlock:
            loEx.ThrowExceptionIfErrors();
        }
        private async Task JournalForm_AfterAdd(R_AfterAddEventArgs eventArgs)
        {
            var loEx = new R_Exception();
            try
            {
                eventArgs.Data = R_FrontUtility.ConvertObjectToObject<CBT00200DTO>(_ParameterDeposit);
                var data = (CBT00200DTO)eventArgs.Data;

                data.CCREATE_BY = clientHelper.UserId;
                data.CUPDATE_BY = clientHelper.UserId;
                data.DUPDATE_DATE = _JournalEntryViewModel.VAR_TODAY.DTODAY;
                data.DCREATE_DATE = _JournalEntryViewModel.VAR_TODAY.DTODAY;
                _JournalEntryViewModel.RefDate = null;
                if (DateTime.TryParseExact(_ParameterDeposit.PARAM_DOC_DATE, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var ldDocDate))
                {
                    _JournalEntryViewModel.DocDate = ldDocDate;
                }
                else
                {
                    _JournalEntryViewModel.DocDate = null;
                }
                data.NLBASE_RATE = 1;
                data.NLCURRENCY_RATE = 1;
                data.NBBASE_RATE = 1;
                data.NBCURRENCY_RATE = 1;
                data.CDEPT_CODE = _ParameterDeposit.PARAM_DEPT_CODE;
                data.NTRANS_AMOUNT = _ParameterDeposit.PARAM_AMOUNT;
                data.CDOC_NO = _ParameterDeposit.PARAM_DOC_NO;
                data.CTRANS_DESC = _ParameterDeposit.PARAM_DESCRIPTION;
                data.CDEPT_NAME = _JournalEntryViewModel.VAR_DEPT_CODE_LIST.FirstOrDefault(x => x.CDEPT_CODE == _ParameterDeposit.PARAM_DEPT_CODE).CDEPT_NAME;

                await _DeptCode_TextBox.FocusAsync();
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            R_DisplayException(loEx);
        }
        private void ValidationFormGLT00100JournalEntry(R_ValidationEventArgs eventArgs)
        {
            var loEx = new R_Exception();
            try
            {
                var loParam = (CBT00200DTO)eventArgs.Data;
                if (string.IsNullOrWhiteSpace(loParam.CDEPT_CODE))
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                        typeof(Resources_Dummy_Class),
                        "V01"));
                }

                if (string.IsNullOrWhiteSpace(loParam.CREF_NO) && _JournalEntryViewModel.VAR_GSM_TRANSACTION_CODE.LINCREMENT_FLAG == false && _JournalEntryViewModel.VAR_CB_SYSTEM_PARAM.LCB_NUMBERING == false)
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                        typeof(Resources_Dummy_Class),
                        "V02"));
                }

                if (_JournalEntryViewModel.RefDate == null)
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                        typeof(Resources_Dummy_Class),
                        "V03"));
                }
                else
                {
                    if (_JournalEntryViewModel.RefDate > _JournalEntryViewModel.VAR_TODAY.DTODAY)
                    {
                        loEx.Add(R_FrontUtility.R_GetError(
                            typeof(Resources_Dummy_Class),
                            "V04"));
                    }

                    if (int.Parse(_JournalEntryViewModel.RefDate.Value.ToString("yyyyMMdd")) < int.Parse(_JournalEntryViewModel.VAR_CB_SYSTEM_PARAM.CCB_LINK_DATE))
                    {
                        loEx.Add(R_FrontUtility.R_GetError(
                            typeof(Resources_Dummy_Class),
                            "V05"));
                    }

                    if (int.Parse(_JournalEntryViewModel.RefDate.Value.ToString("yyyyMMdd")) < int.Parse(_JournalEntryViewModel.VAR_SOFT_PERIOD_START_DATE.CEND_DATE))
                    {
                        loEx.Add(R_FrontUtility.R_GetError(
                            typeof(Resources_Dummy_Class),
                            "V06"));
                    }
                }

                if (string.IsNullOrWhiteSpace(loParam.CCB_CODE))
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                        typeof(Resources_Dummy_Class),
                        "V07"));
                }

                if (loParam.NTRANS_AMOUNT <= 0)
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                        typeof(Resources_Dummy_Class),
                        "V08"));
                }

                if (string.IsNullOrWhiteSpace(loParam.CDOC_NO))
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                        typeof(Resources_Dummy_Class),
                        "V09"));
                }

                if (_JournalEntryViewModel.DocDate.HasValue == false)
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                        typeof(Resources_Dummy_Class),
                        "V10"));
                }
                else
                {
                    int loRefDate = _JournalEntryViewModel.RefDate.HasValue ? int.Parse(_JournalEntryViewModel.RefDate.Value.ToString("yyyyMMdd")) : 0;

                    if (int.Parse(_JournalEntryViewModel.DocDate.Value.ToString("yyyyMMdd")) > loRefDate)
                    {
                        loEx.Add(R_FrontUtility.R_GetError(
                            typeof(Resources_Dummy_Class),
                            "V11"));
                    }

                    if (int.Parse(_JournalEntryViewModel.DocDate.Value.ToString("yyyyMMdd")) < int.Parse(_JournalEntryViewModel.VAR_SOFT_PERIOD_START_DATE.CEND_DATE))
                    {
                        loEx.Add(R_FrontUtility.R_GetError(
                            typeof(Resources_Dummy_Class),
                            "V12"));
                    }
                }

                if (string.IsNullOrWhiteSpace(loParam.CTRANS_DESC))
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                        typeof(Resources_Dummy_Class),
                        "V13"));
                }

                if (loParam.NLBASE_RATE <= 0)
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                        typeof(Resources_Dummy_Class),
                        "V14"));
                }

                if (loParam.NLCURRENCY_RATE <= 0)
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                        typeof(Resources_Dummy_Class),
                        "V15"));
                }

                if (loParam.NBBASE_RATE <= 0)
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                        typeof(Resources_Dummy_Class),
                        "V16"));
                }

                if (loParam.NBCURRENCY_RATE <= 0)
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                        typeof(Resources_Dummy_Class),
                        "V17"));
                }
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            loEx.ThrowExceptionIfErrors();
        }
        private async Task JournalForm_RDisplay(R_DisplayEventArgs eventArgs)
        {
            var loEx = new R_Exception();
            try
            {
                var data = (CBT00200DTO)eventArgs.Data;
                if (eventArgs.ConductorMode == R_eConductorMode.Normal)
                {
                    if (!string.IsNullOrWhiteSpace(data.CSTATUS))
                    {
                        lcLabelCommit = data.CSTATUS == "80" ? _localizer["_UndoCommit"] : _localizer["_Commit"];
                        lcLabelSubmit = data.CSTATUS == "10" ? _localizer["_UndoSubmit"] : _localizer["_Submit"];

                        EnableEdit = data.CSTATUS == "00";
                        EnableDelete = data.CSTATUS != "00";
                        EnableSubmit = data.CSTATUS == "00" || data.CSTATUS == "10";
                        EnableApprove = data.CSTATUS == "10" && _JournalEntryViewModel.VAR_GSM_TRANSACTION_CODE.LAPPROVAL_FLAG == true;
                        EnableCommit = (data.CSTATUS == "20" || data.CSTATUS == "80") &&
                                       int.Parse(data.CREF_PRD) >= int.Parse(_JournalEntryViewModel.VAR_CB_SYSTEM_PARAM.CSOFT_PERIOD);
                        EnableHaveRecId = !string.IsNullOrWhiteSpace(data.CREC_ID);

                    }
                    if (!string.IsNullOrWhiteSpace(data.CREC_ID))
                    {
                        await _gridDetailRef.R_RefreshGrid(data);
                    }
                }
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            loEx.ThrowExceptionIfErrors();

        }
        private async Task JournalForm_BeforeCancel(R_BeforeCancelEventArgs eventArgs)
        {
            var res = await R_MessageBox.Show("", _localizer["Q04"],
                R_eMessageBoxButtonType.YesNo);

            eventArgs.Cancel = res == R_eMessageBoxResult.No;
            if (res == R_eMessageBoxResult.Yes)
            {
                if (eventArgs.ConductorMode == R_eConductorMode.Add)
                {
                    await this.CloseDetail();
                }
            }
        }
        private void JournalForm_SetOther(R_SetEventArgs eventArgs)
        {
            EnableHeaderNormalMode = eventArgs.Enable;
        }
        #endregion

        #region Detail
        private async Task JournalDet_ServiceGetListRecord(R_ServiceGetListRecordEventArgs eventArgs)
        {
            var loEx = new R_Exception();
            try
            {
                var loParam = R_FrontUtility.ConvertObjectToObject<CBT00210DTO>(eventArgs.Parameter);
                await _JournalListViewModel.GetJournalDetailList(loParam);

                eventArgs.ListEntityResult = _JournalListViewModel.JournalDetailGrid;
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            loEx.ThrowExceptionIfErrors();
        }
        #endregion

        #region Refresh Currency Method
        public async Task RefreshCurrency()
        {
            var loEx = new R_Exception();
            try
            {
                var loResult = await _JournalEntryViewModel.GetLastCurrencyRate();

                if (loResult is null)
                {
                    _JournalEntryViewModel.Data.NLBASE_RATE = 1;
                    _JournalEntryViewModel.Data.NLCURRENCY_RATE = 1;
                    _JournalEntryViewModel.Data.NBBASE_RATE = 1;
                    _JournalEntryViewModel.Data.NBCURRENCY_RATE = 1;
                }
                else
                {
                    _JournalEntryViewModel.Data.NLBASE_RATE = loResult.NLBASE_RATE_AMOUNT;
                    _JournalEntryViewModel.Data.NLCURRENCY_RATE = loResult.NLCURRENCY_RATE_AMOUNT;
                    _JournalEntryViewModel.Data.NBBASE_RATE = loResult.NBBASE_RATE_AMOUNT;
                    _JournalEntryViewModel.Data.NBCURRENCY_RATE = loResult.NBCURRENCY_RATE_AMOUNT;
                }
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            loEx.ThrowExceptionIfErrors();
        }
        #endregion

        #region Ref Date OnLostFocus
        private async Task RefDate_ValueChange(DateTime? poParam)
        {
            var loEx = new R_Exception();
            try
            {
                _JournalEntryViewModel.RefDate = poParam;
                if (!string.IsNullOrWhiteSpace(_JournalEntryViewModel.Data.CCURRENCY_CODE) &&
                    (_JournalEntryViewModel.Data.CCURRENCY_CODE != _JournalEntryViewModel.VAR_GSM_COMPANY.CLOCAL_CURRENCY_CODE
                    || _JournalEntryViewModel.Data.CCURRENCY_CODE != _JournalEntryViewModel.VAR_GSM_COMPANY.CBASE_CURRENCY_CODE))
                {
                    await RefreshCurrency();
                }
                else
                {
                    _JournalEntryViewModel.Data.NLBASE_RATE = 1;
                    _JournalEntryViewModel.Data.NLCURRENCY_RATE = 1;
                    _JournalEntryViewModel.Data.NBBASE_RATE = 1;
                    _JournalEntryViewModel.Data.NBCURRENCY_RATE = 1;
                }
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            loEx.ThrowExceptionIfErrors();
        }
        #endregion

        #region Process
        private async Task ApproveJournalProcess()
        {
            var loEx = new R_Exception();
            try
            {
                var loData = (CBT00200DTO)_conductorRef.R_GetCurrentData();
                if (!loData.LALLOW_APPROVE)
                {
                    loEx.Add("", _localizer["N04"]);
                    goto EndBlock;
                }

                R_eMessageBoxResult loValidate = await R_MessageBox.Show("", _localizer["Q08"], R_eMessageBoxButtonType.YesNo);
                if (loValidate == R_eMessageBoxResult.No)
                    goto EndBlock;

                var loParam = R_FrontUtility.ConvertObjectToObject<CBT00200UpdateStatusDTO>(loData);
                loParam.LAUTO_COMMIT = false;
                loParam.LUNDO_COMMIT = false;
                loParam.CNEW_STATUS = "20";

                await _JournalListViewModel.UpdateJournalStatus(loParam);
                await _conductorRef.R_GetEntity(loData);
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }
        EndBlock:
            loEx.ThrowExceptionIfErrors();
        }
        private async Task CommitJournalProcess()
        {
            var loEx = new R_Exception();
            R_eMessageBoxResult loValidate;
            string loNewStatus = "";

            try
            {
                var loData = (CBT00200DTO)_conductorRef.R_GetCurrentData();

                if (loData.CSTATUS == "80")
                {
                    loValidate = await R_MessageBox.Show("", _localizer["Q01"], R_eMessageBoxButtonType.YesNo);
                    if (loValidate == R_eMessageBoxResult.No)
                        goto EndBlock;
                }
                else
                {
                    loValidate = await R_MessageBox.Show("", _localizer["Q02"], R_eMessageBoxButtonType.YesNo);
                    if (loValidate == R_eMessageBoxResult.No)
                        goto EndBlock;
                }

                if (loData.CSTATUS == "80")
                {
                    if (_JournalEntryViewModel.VAR_GSM_TRANSACTION_CODE.LAPPROVAL_FLAG == true)
                    {
                        loNewStatus = "10";
                    }
                    else
                    {
                        loNewStatus = "00";
                    }
                }
                else
                {
                    loNewStatus = "80";
                }

                var loParam = R_FrontUtility.ConvertObjectToObject<CBT00200UpdateStatusDTO>(loData);
                loParam.LAUTO_COMMIT = false;
                loParam.LUNDO_COMMIT = loData.CSTATUS == "80" ? true : false;
                loParam.CNEW_STATUS = loNewStatus;

                await _JournalListViewModel.UpdateJournalStatus(loParam);
                await _conductorRef.R_GetEntity(loData);
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }
        EndBlock:
            loEx.ThrowExceptionIfErrors();
        }
        private async Task SubmitJournalProcess()
        {
            var loEx = new R_Exception();
            R_eMessageBoxResult loResult;
            bool llValidate = false;
            try
            {
                var loData = (CBT00200DTO)_conductorRef.R_GetCurrentData();

                if (loData.CSTATUS == "00" && int.Parse(loData.CREF_PRD) < int.Parse(_JournalEntryViewModel.VAR_CB_SYSTEM_PARAM.CSOFT_PERIOD))
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                        typeof(Resources_Dummy_Class),
                        "V18"));
                    llValidate = true;
                }

                if ((loData.NDEBIT_AMOUNT > 0 || loData.NCREDIT_AMOUNT > 0) && loData.NCREDIT_AMOUNT != loData.NDEBIT_AMOUNT)
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                        typeof(Resources_Dummy_Class),
                        "V19"));
                    llValidate = true;
                }

                if (loData.NDEBIT_AMOUNT == 0 || loData.NCREDIT_AMOUNT == 0)
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                         typeof(Resources_Dummy_Class),
                         "V20"));
                    llValidate = true;
                }

                if (loData.NDEBIT_AMOUNT > 0 && loData.NDEBIT_AMOUNT != loData.NTRANS_AMOUNT)
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                         typeof(Resources_Dummy_Class),
                         "V21"));
                    llValidate = true;
                }

                if (llValidate == true)
                {
                    goto EndBlock;
                }
                else
                {
                    if (loData.CSTATUS == "10")
                    {
                        loResult = await R_MessageBox.Show("", _localizer["Q05"], R_eMessageBoxButtonType.YesNo);
                        if (loResult == R_eMessageBoxResult.No)
                            goto EndBlock;
                    }
                    else
                    {
                        loResult = await R_MessageBox.Show("", _localizer["Q06"], R_eMessageBoxButtonType.YesNo);
                        if (loResult == R_eMessageBoxResult.No)
                            goto EndBlock;
                    }

                    var loParam = R_FrontUtility.ConvertObjectToObject<CBT00200UpdateStatusDTO>(loData);
                    loParam.LAUTO_COMMIT = false;
                    loParam.LUNDO_COMMIT = false;
                    loParam.CNEW_STATUS = loData.CSTATUS == "00" ? "10" : "00";

                    await _JournalListViewModel.UpdateJournalStatus(loParam);
                    await _conductorRef.R_GetEntity(loData);
                }
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

        EndBlock:
            loEx.ThrowExceptionIfErrors();
        }
        #endregion

        #region Print
        private void Before_Open_lookupPrint(R_BeforeOpenLookupEventArgs eventArgs)
        {
            //var loData = (GLT00110DTO)_conductorRef.R_GetCurrentData();
            //var param = new GLTR00100DTO()
            //{
            //    CREC_ID = loData.CREC_ID
            //};
            //eventArgs.Parameter = param;
            //eventArgs.TargetPageType = typeof(GLTR00100);
        }
        #endregion

        #region lookupDept
        private async Task DeptCode_OnLostFocus(object poParam)
        {
            var loEx = new R_Exception();

            try
            {
                var loData = (CBT00200DTO)_conductorRef.R_GetCurrentData();
                if (string.IsNullOrWhiteSpace(loData.CDEPT_CODE) == false)
                {
                    GSL00700ParameterDTO loParam = new GSL00700ParameterDTO()
                    {
                        CSEARCH_TEXT = loData.CDEPT_CODE
                    };

                    LookupGSL00700ViewModel loLookupViewModel = new LookupGSL00700ViewModel();

                    var loResult = await loLookupViewModel.GetDepartment(loParam);

                    if (loResult == null)
                    {
                        loEx.Add(R_FrontUtility.R_GetError(
                                typeof(Lookup_GSFrontResources.Resources_Dummy_Class),
                                "_ErrLookup01"));
                        loData.CDEPT_NAME = "";
                        goto EndBlock;
                    }
                    loData.CDEPT_CODE = loResult.CDEPT_CODE;
                    loData.CDEPT_NAME = loResult.CDEPT_NAME;
                }
                else
                {
                    loData.CDEPT_NAME = "";
                }
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }
        EndBlock:
            R_DisplayException(loEx);
        }
        private void Before_Open_lookupDept(R_BeforeOpenLookupEventArgs eventArgs)
        {
            var param = new GSL00700ParameterDTO
            {
            };
            eventArgs.Parameter = param;
            eventArgs.TargetPageType = typeof(GSL00700);
        }
        private void After_Open_lookupDept(R_AfterOpenLookupEventArgs eventArgs)
        {
            var loTempResult = (GSL00700DTO)eventArgs.Result;
            if (loTempResult == null)
            {
                return;
            }

            var loData = (CBT00200DTO)_conductorRef.R_GetCurrentData();
            loData.CDEPT_CODE = loTempResult.CDEPT_CODE;
            loData.CDEPT_NAME = loTempResult.CDEPT_NAME;
        }
        #endregion

        #region lookupCashCode
        private async Task CashCode_OnLostFocus(object poParam)
        {
            var loEx = new R_Exception();

            try
            {
                var loData = (CBT00200DTO)_conductorRef.R_GetCurrentData();
                if (string.IsNullOrWhiteSpace(loData.CCB_CODE) == false)
                {
                    GSL02500ParameterDTO loParam = new GSL02500ParameterDTO()
                    {
                        CDEPT_CODE = loData.CDEPT_CODE,
                        CCB_TYPE = "C",
                        CBANK_TYPE = "I",
                        CSEARCH_TEXT = loData.CCB_CODE
                    };

                    LookupGSL02500ViewModel loLookupViewModel = new LookupGSL02500ViewModel();

                    var loResult = await loLookupViewModel.GetCB(loParam);

                    if (loResult == null)
                    {
                        loEx.Add(R_FrontUtility.R_GetError(
                                typeof(Lookup_GSFrontResources.Resources_Dummy_Class),
                                "_ErrLookup01"));
                        loData.CCB_NAME = "";
                        loData.CCURRENCY_CODE = "";
                        loData.CCB_ACCOUNT_NO = "";
                        goto EndBlock;
                    }
                    loData.CCB_CODE = loResult.CCB_CODE;
                    loData.CCB_NAME = loResult.CCB_NAME;
                    loData.CCURRENCY_CODE = loResult.CCURRENCY_CODE;
                    loData.CCB_ACCOUNT_NO = loResult.CCB_ACCOUNT_NO;

                    if (!string.IsNullOrWhiteSpace(loData.CCURRENCY_CODE) &&
                    (loData.CCURRENCY_CODE != _JournalEntryViewModel.VAR_GSM_COMPANY.CLOCAL_CURRENCY_CODE
                    || loData.CCURRENCY_CODE != _JournalEntryViewModel.VAR_GSM_COMPANY.CBASE_CURRENCY_CODE))
                    {
                        await RefreshCurrency();
                    }
                    else
                    {
                        loData.NLBASE_RATE = 1;
                        loData.NLCURRENCY_RATE = 1;
                        loData.NBBASE_RATE = 1;
                        loData.NBCURRENCY_RATE = 1;
                    }
                }
                else
                {
                    loData.CCB_NAME = "";
                    loData.CCURRENCY_CODE = "";
                    loData.CCB_ACCOUNT_NO = "";
                }
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }
        EndBlock:
            R_DisplayException(loEx);
        }
        private void Before_Open_lookupCashCode(R_BeforeOpenLookupEventArgs eventArgs)
        {
            var loData = (CBT00200DTO)_conductorRef.R_GetCurrentData();
            GSL02500ParameterDTO loParam = new GSL02500ParameterDTO()
            {
                CDEPT_CODE = loData.CDEPT_CODE,
                CCB_TYPE = "C",
                CBANK_TYPE = "I"
            };
            eventArgs.Parameter = loParam;
            eventArgs.TargetPageType = typeof(GSL02500);
        }
        private async Task After_Open_lookupCashCode(R_AfterOpenLookupEventArgs eventArgs)
        {
            var loTempResult = (GSL02500DTO)eventArgs.Result;
            if (loTempResult == null)
            {
                return;
            }

            var loData = (CBT00200DTO)_conductorRef.R_GetCurrentData();
            loData.CCB_CODE = loTempResult.CCB_CODE;
            loData.CCB_NAME = loTempResult.CCB_NAME;
            loData.CCURRENCY_CODE = loTempResult.CCURRENCY_CODE;
            loData.CCB_ACCOUNT_NO = loTempResult.CCB_ACCOUNT_NO;
            if (!string.IsNullOrWhiteSpace(loData.CCURRENCY_CODE) &&
                    (loData.CCURRENCY_CODE != _JournalEntryViewModel.VAR_GSM_COMPANY.CLOCAL_CURRENCY_CODE
                    || loData.CCURRENCY_CODE != _JournalEntryViewModel.VAR_GSM_COMPANY.CBASE_CURRENCY_CODE))
            {
                await RefreshCurrency();
            }
            else
            {
                loData.NLBASE_RATE = 1;
                loData.NLCURRENCY_RATE = 1;
                loData.NBBASE_RATE = 1;
                loData.NBCURRENCY_RATE = 1;
            }
        }
        #endregion

        #region Result Detail

        #endregion
    }
}
