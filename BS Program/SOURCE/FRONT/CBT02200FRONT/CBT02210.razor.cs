using BlazorClientHelper;
using CBT02200COMMON.DTO.CBT02200;
using CBT02200COMMON.DTO.CBT02210;
using CBT02200FrontResources;
using CBT02200MODEL.ViewModel;
using Lookup_GSCOMMON.DTOs;
using Lookup_GSFRONT;
using Lookup_GSModel.ViewModel;
using Microsoft.AspNetCore.Components;
using R_BlazorFrontEnd;
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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT02200FRONT
{
    public partial class CBT02210 : R_Page
    {
        [Inject] IClientHelper loClientHelper { get; set; }
        [Inject] private R_ILocalizer<CBT02200FrontResources.Resources_Dummy_Class> _localizer { get; set; }

        private CBT02210ViewModel loChequeEntryViewModel = new CBT02210ViewModel();
        
        private R_Conductor _conductorRef;
        
        private R_ConductorGrid _conductorDetailRef;
        
        private R_Grid<CBT02210DetailDTO> _gridDetailRef;

        private R_TextBox _DeptCodeRef;

        #region Private Property
        private ChequeEntryTabParameterDTO loTabParameter = null;
        private string lcLabelSubmit = "Submit";
        private string lcOldDeptCode = "";
        private string lcOldCBCode = "";
        private string lcOldCBAccountNo = "";
        private bool EnableEdit = false;
        private bool EnableDelete = false;
        private bool EnableSubmit = false;
        private bool EnableApprove = false;
        private bool EnableHaveRecId = false;
        private bool IsDetailCRUDMode = false;
        #endregion

        protected override async Task R_Init_From_Master(object poParameter)
        {
            R_Exception loEx = new R_Exception();

            try
            {
                loTabParameter = (ChequeEntryTabParameterDTO)poParameter;

                await loChequeEntryViewModel.InitialProcessAsync();
                loChequeEntryViewModel.loCompanyInfo = loChequeEntryViewModel.loInitialProcess.CompanyInfo;
                loChequeEntryViewModel.loTransCodeInfo = loChequeEntryViewModel.loInitialProcess.TransCodeInfo;
                loChequeEntryViewModel.loCBSystemParam = loChequeEntryViewModel.loInitialProcess.CBSystemParam;

                await loChequeEntryViewModel.GetCenterListStreamAsync();
                if (!string.IsNullOrWhiteSpace(loTabParameter.CCHEQUE_ID))
                {
                    await _conductorRef.R_GetEntity(new CBT02210DTO()
                    {
                        CREC_ID = loTabParameter.CCHEQUE_ID
                    });
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
            R_Exception loEx = new R_Exception();
            var llRtn = false;
            R_LockingFrontResult loLockResult = null;

            try
            {
                CBT02210DTO loData = (CBT02210DTO)eventArgs.Data;

                var loCls = new R_LockingServiceClient(pcModuleName: DEFAULT_MODULE_NAME,
                    plSendWithContext: true,
                    plSendWithToken: true,
                    pcHttpClientName: DEFAULT_HTTP_NAME);

                if (eventArgs.Mode == R_eLockUnlock.Lock)
                {
                    var loLockPar = new R_ServiceLockingLockParameterDTO
                    {
                        Company_Id = loClientHelper.CompanyId,
                        User_Id = loClientHelper.UserId,
                        Program_Id = "CBT02200",
                        Table_Name = "CBT_TRANS_HD",
                        Key_Value = string.Join("|", loClientHelper.CompanyId, loData.CDEPT_CODE, loData.CTRANS_CODE, loData.CREF_NO)
                    };

                    loLockResult = await loCls.R_Lock(loLockPar);
                }
                else
                {
                    var loUnlockPar = new R_ServiceLockingUnLockParameterDTO
                    {
                        Company_Id = loClientHelper.CompanyId,
                        User_Id = loClientHelper.UserId,
                        Program_Id = "CBT02200",
                        Table_Name = "CBT_TRANS_HD",
                        Key_Value = string.Join("|", loClientHelper.CompanyId, loData.CDEPT_CODE, loData.CTRANS_CODE, loData.CREF_NO)
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
        private async Task ChequeForm_GetRecord(R_ServiceGetRecordEventArgs eventArgs)
        {
            R_Exception loEx = new R_Exception();
            try
            {
                await loChequeEntryViewModel.GetChequeHeaderAsync((CBT02210DTO)eventArgs.Data);
                eventArgs.Result = loChequeEntryViewModel.loChequeHeader;
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            loEx.ThrowExceptionIfErrors();
        }

        private async Task ChequeForm_ServiceSave(R_ServiceSaveEventArgs eventArgs)
        {
            R_Exception loEx = new R_Exception();
            try
            {
                await loChequeEntryViewModel.SaveChequeHeaderAsync((CBT02210DTO)eventArgs.Data, (eCRUDMode)eventArgs.ConductorMode);
                eventArgs.Result = loChequeEntryViewModel.loChequeHeader;
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            loEx.ThrowExceptionIfErrors();
        }

        private async Task BtnDelete_OnClick()
        {
            R_Exception loEx = new R_Exception();
            UpdateStatusParameterDTO loParam = new UpdateStatusParameterDTO();
            try
            {
                var loValidate = await R_MessageBox.Show("", _localizer["Q003"], R_eMessageBoxButtonType.YesNo);
                if (loValidate == R_eMessageBoxResult.No)
                    goto EndBlock;

                //var loData = (CBT02200DTO)_conductorRef.R_GetCurrentData();
                //loParam = R_FrontUtility.ConvertObjectToObject<CBT02200UpdateStatusDTO>(loData);
                loParam.CREC_ID_LIST = loTabParameter.CCHEQUE_ID;
                loParam.CNEW_STATUS = "99";
                //loParam.LAUTO_COMMIT = false;
                //loParam.LUNDO_COMMIT = false;

                await loChequeEntryViewModel.UpdateStatusAsync(loParam);
                await _conductorRef.R_GetEntity(new CBT02210DTO()
                {
                    CREC_ID = loTabParameter.CCHEQUE_ID
                });
                if (!loEx.HasError)
                {
                    await R_MessageBox.Show("", _localizer["M006"], R_eMessageBoxButtonType.OK);
                }
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }
        EndBlock:
            loEx.ThrowExceptionIfErrors();
        }

        private void ChequeForm_SetAdd(R_SetEventArgs eventArgs)
        {
            var loEx = new R_Exception();
            try
            {
                if (eventArgs.Enable)
                {
                    _gridDetailRef.DataSource.Clear();
                }
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            R_DisplayException(loEx);
        }


        private async Task ChequeForm_AfterAdd(R_AfterAddEventArgs eventArgs)
        {
            var loEx = new R_Exception();
            try
            {
                await _DeptCodeRef.FocusAsync();
                CBT02210DTO loData = (CBT02210DTO)eventArgs.Data;

                //loData.CCREATE_BY = loClientHelper.UserId;
                //loData.CUPDATE_BY = loClientHelper.UserId;
                //loData.DUPDATE_DATE = loChequeEntryViewModel.VAR_TODAY.DTODAY;
                //loData.DCREATE_DATE = loChequeEntryViewModel.VAR_TODAY.DTODAY;
                loData.DREF_DATE = DateTime.Today;
                if (!loChequeEntryViewModel.loInitialProcess.CBSystemParam.LINPUT_CHEQUE_DATE)
                {
                    loData.DCHEQUE_DATE = loData.DREF_DATE;
                }
                loData.NLBASE_RATE = 1;
                loData.NLCURRENCY_RATE = 1;
                loData.NBBASE_RATE = 1;
                loData.NBCURRENCY_RATE = 1;

                if (!string.IsNullOrWhiteSpace(loData.CDEPT_CODE))
                {
                    _gridDetailRef.DataSource.Clear();
                }
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            R_DisplayException(loEx);
        }

        private void ValidationFormCBT02200ChequeEntry(R_ValidationEventArgs eventArgs)
        {
            var loEx = new R_Exception();
            try
            {
                var loParam = (CBT02210DTO)eventArgs.Data;
                if (string.IsNullOrWhiteSpace(loParam.CDEPT_CODE))
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                        typeof(Resources_Dummy_Class),
                        "V008"));
                }

                if (string.IsNullOrWhiteSpace(loParam.CREF_NO) && loChequeEntryViewModel.loInitialProcess.TransCodeInfo.LINCREMENT_FLAG == false && loChequeEntryViewModel.loInitialProcess.CBSystemParam.LCB_NUMBERING == false)
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                        typeof(Resources_Dummy_Class),
                        "V009"));
                }

                if (loParam.DREF_DATE == null)
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                        typeof(Resources_Dummy_Class),
                        "V010"));
                }
                else
                {
                    if (loParam.DREF_DATE > DateTime.Today)
                    {
                        loEx.Add(R_FrontUtility.R_GetError(
                            typeof(Resources_Dummy_Class),
                            "V011"));
                    }

                    if (int.Parse(loParam.DREF_DATE.Value.ToString("yyyyMMdd")) < int.Parse(loChequeEntryViewModel.loInitialProcess.CBSystemParam.CCB_LINK_DATE))
                    {
                        loEx.Add(R_FrontUtility.R_GetError(
                            typeof(Resources_Dummy_Class),
                            "V012"));
                    }

                    if (int.Parse(loParam.DREF_DATE.Value.ToString("yyyyMMdd")) < int.Parse(loChequeEntryViewModel.loInitialProcess.SoftPeriodStartDate.CEND_DATE))
                    {
                        loEx.Add(R_FrontUtility.R_GetError(
                            typeof(Resources_Dummy_Class),
                            "V013"));
                    }
                }

                if (string.IsNullOrWhiteSpace(loParam.CCHEQUE_NO))
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                        typeof(Resources_Dummy_Class),
                        "V014"));
                }

                if (loParam.DCHEQUE_DATE == null)
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                        typeof(Resources_Dummy_Class),
                        "V015"));
                }
                else
                {
                    if (int.Parse(loParam.DCHEQUE_DATE.Value.ToString("yyyyMMdd")) < int.Parse(loChequeEntryViewModel.loCBSystemParam.CCB_LINK_DATE))
                    {
                        loEx.Add(R_FrontUtility.R_GetError(
                            typeof(Resources_Dummy_Class),
                            "V016"));
                    }

                    if (int.Parse(loParam.DCHEQUE_DATE.Value.ToString("yyyyMMdd")) < int.Parse(loChequeEntryViewModel.loInitialProcess.SoftPeriodStartDate.CEND_DATE))
                    {
                        loEx.Add(R_FrontUtility.R_GetError(
                            typeof(Resources_Dummy_Class),
                            "V017"));
                    }
                }

                if (loParam.DCHEQUE_DATE == null)
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                        typeof(Resources_Dummy_Class),
                        "V018"));
                }
                else
                {
                    if (int.Parse(loParam.DDUE_DATE.Value.ToString("yyyyMMdd")) < int.Parse(loParam.DCHEQUE_DATE.Value.ToString("yyyyMMdd")))
                    {
                        loEx.Add(R_FrontUtility.R_GetError(
                            typeof(Resources_Dummy_Class),
                            "V019"));
                    }
                }

                if (string.IsNullOrWhiteSpace(loParam.CCB_CODE))
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                        typeof(Resources_Dummy_Class),
                        "V020"));
                }

                if (string.IsNullOrWhiteSpace(loParam.CCB_ACCOUNT_NO))
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                        typeof(Resources_Dummy_Class),
                        "V021"));
                }

                if (loParam.NTRANS_AMOUNT <= 0)
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                        typeof(Resources_Dummy_Class),
                        "V022"));
                }

                if (string.IsNullOrWhiteSpace(loParam.CTRANS_DESC))
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                        typeof(Resources_Dummy_Class),
                        "V023"));
                }

                if (loParam.NLCURRENCY_RATE <= 0)
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                        typeof(Resources_Dummy_Class),
                        "V024"));
                }

                if (loParam.NLBASE_RATE <= 0)
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                        typeof(Resources_Dummy_Class),
                        "V034"));
                }

                if (loParam.NBCURRENCY_RATE <= 0)
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                        typeof(Resources_Dummy_Class),
                        "V035"));
                }

                if (loParam.NBBASE_RATE <= 0)
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                        typeof(Resources_Dummy_Class),
                        "V026"));
                }

            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            loEx.ThrowExceptionIfErrors();
        }

        private async Task ChequeForm_Display(R_DisplayEventArgs eventArgs)
        {
            R_Exception loEx = new R_Exception();
            try
            {
                CBT02210DTO loData = (CBT02210DTO)eventArgs.Data;
                if (eventArgs.ConductorMode == R_eConductorMode.Normal)
                {
                    if (!string.IsNullOrWhiteSpace(loData.CSTATUS))
                    {

                        EnableEdit = loData.CSTATUS == "00";
                        EnableDelete = loData.CSTATUS != "00";
                        EnableSubmit = loData.CSTATUS == "00" || loData.CSTATUS == "10";
                        if (loData.CSTATUS == "10")
                        {
                            lcLabelSubmit = _localizer["_UndoSubmit"];
                        }
                        else
                        {
                            lcLabelSubmit = _localizer["_Submit"];
                        }
                        EnableApprove = loData.CSTATUS == "10" && loChequeEntryViewModel.loInitialProcess.TransCodeInfo.LAPPROVAL_FLAG == true;
                        EnableHaveRecId = !string.IsNullOrWhiteSpace(loData.CREC_ID);

                    }
                    if (!string.IsNullOrWhiteSpace(loData.CREC_ID))
                    {
                        await _gridDetailRef.R_RefreshGrid(loData);
                    }
                }
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            loEx.ThrowExceptionIfErrors();

        }

        private async Task ChequeForm_BeforeCancel(R_BeforeCancelEventArgs eventArgs)
        {
            var res = await R_MessageBox.Show("", _localizer["Q006"],
                R_eMessageBoxButtonType.YesNo);
            eventArgs.Cancel = res == R_eMessageBoxResult.No;
        }

        private async Task CopyChequeEntryProcess()
        {
            R_Exception loEx = new R_Exception();
            try
            {
                CBT02210DTO loOldData = (CBT02210DTO)_conductorRef.R_GetCurrentData();
                await _conductorRef.Add();
                if (_conductorRef.R_ConductorMode == R_eConductorMode.Add)
                {
                    CBT02210DTO loNewData = (CBT02210DTO)_conductorRef.R_GetCurrentData();

                    loNewData.CREF_PRD = loOldData.CREF_PRD;
                    loNewData.LALLOW_APPROVE = loOldData.LALLOW_APPROVE;
                    loNewData.CINPUT_TYPE = loOldData.CINPUT_TYPE;
                    loNewData.CDEPT_CODE = loOldData.CDEPT_CODE;
                    loNewData.CDEPT_NAME = loOldData.CDEPT_NAME;
                    loNewData.DREF_DATE = DateTime.ParseExact(loOldData.CREF_DATE, "yyyyMMdd", CultureInfo.InvariantCulture);
                    loNewData.CCB_CODE = loOldData.CCB_CODE;
                    loNewData.CCB_NAME = loOldData.CCB_NAME;
                    loNewData.CCHEQUE_NO = loOldData.CCHEQUE_NO;
                    loNewData.CCB_ACCOUNT_NO = loOldData.CCB_ACCOUNT_NO;
                    loNewData.CCB_ACCOUNT_NAME = loOldData.CCB_ACCOUNT_NAME;
                    loNewData.CCURRENCY_CODE = loOldData.CCURRENCY_CODE;
                    loNewData.NTRANS_AMOUNT = loOldData.NTRANS_AMOUNT;
                    loNewData.NLBASE_RATE = loOldData.NLBASE_RATE;
                    loNewData.CCURRENCY_CODE = loOldData.CCURRENCY_CODE;
                    loNewData.NLCURRENCY_RATE = loOldData.NLCURRENCY_RATE;
                    loNewData.NBBASE_RATE = loOldData.NBBASE_RATE;
                    loNewData.NBCURRENCY_RATE = loOldData.NBCURRENCY_RATE;
                    loNewData.NDEBIT_AMOUNT = loOldData.NDEBIT_AMOUNT;
                    loNewData.NCREDIT_AMOUNT = loOldData.NCREDIT_AMOUNT;
                    loNewData.CTRANS_DESC = loOldData.CTRANS_DESC;
                    loNewData.CSTATUS_NAME = loOldData.CSTATUS_NAME;
                    loNewData.CSTATUS = loOldData.CSTATUS;
                    if (!string.IsNullOrWhiteSpace(loOldData.CCLEAR_DATE))
                    {
                        loNewData.DCLEAR_DATE = DateTime.ParseExact(loOldData.CCLEAR_DATE, "yyyyMMdd", CultureInfo.InvariantCulture);
                    }
                    loNewData.DCHEQUE_DATE = DateTime.ParseExact(loOldData.CCHEQUE_DATE, "yyyyMMdd", CultureInfo.InvariantCulture);
                    loNewData.DDUE_DATE = DateTime.ParseExact(loOldData.CDUE_DATE, "yyyyMMdd", CultureInfo.InvariantCulture);

                    //loNewData.CREF_PRD = loOldData.CREF_PRD;
                    //loNewData.LALLOW_APPROVE = loOldData.LALLOW_APPROVE;
                    //loNewData.CINPUT_TYPE = loOldData.CINPUT_TYPE;
                    //loNewData.CDEPT_CODE = loOldData.CDEPT_CODE;
                    //loNewData.DREF_DATE = DateTime.ParseExact(loOldData.CREF_DATE, "yyyyMMdd", CultureInfo.InvariantCulture);
                    //loNewData.CCB_CODE = loOldData.CCB_CODE;
                    //loNewData.CCB_NAME = loOldData.CCB_NAME;
                    //loNewData.CCB_ACCOUNT_NO = loOldData.CCB_ACCOUNT_NO;
                    //loNewData.CCURRENCY_CODE = loOldData.CCURRENCY_CODE;
                    //loNewData.NTRANS_AMOUNT = loOldData.NTRANS_AMOUNT;
                    //loNewData.NLBASE_RATE = loOldData.NLBASE_RATE;
                    //loNewData.CCURRENCY_CODE = loOldData.CCURRENCY_CODE;
                    //loNewData.NLCURRENCY_RATE = loOldData.NLCURRENCY_RATE;
                    //loNewData.NBBASE_RATE = loOldData.NBBASE_RATE;
                    //loNewData.NBCURRENCY_RATE = loOldData.NBCURRENCY_RATE;
                    //loNewData.NDEBIT_AMOUNT = loOldData.NDEBIT_AMOUNT;
                    //loNewData.NCREDIT_AMOUNT = loOldData.NCREDIT_AMOUNT;
                    //loNewData.CCHEQUE_NO = loOldData.CCHEQUE_NO;
                    //loNewData.DREF_DATE = DateTime.ParseExact(loOldData.CDOC_DATE, "yyyyMMdd", CultureInfo.InvariantCulture);
                    //loNewData.CTRANS_DESC = loOldData.CTRANS_DESC;
                    //loNewData.CSTATUS_NAME = loOldData.CSTATUS_NAME;
                    //loNewData.CSTATUS = loOldData.CSTATUS;
                    //loNewData.CUPDATE_BY = loOldData.CUPDATE_BY;
                    //loNewData.DUPDATE_DATE = loOldData.DUPDATE_DATE;
                    //loNewData.CCREATE_BY = loOldData.CCREATE_BY;
                    //loNewData.DCREATE_DATE = loOldData.DCREATE_DATE;
                }
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
            R_Exception loEx = new R_Exception();
            try
            {
                await loChequeEntryViewModel.RefreshCurrencyRateAsync(new RefreshCurrencyRateParameterDTO()
                {
                    CCURRENCY_CODE = loChequeEntryViewModel.Data.CCURRENCY_CODE,
                    CRATETYPE_CODE = loChequeEntryViewModel.loInitialProcess.CBSystemParam.CRATETYPE_CODE,
                    CREF_DATE = loChequeEntryViewModel.Data.DREF_DATE.Value.ToString("yyyyMMdd")
                });

                if (loChequeEntryViewModel.loRefreshCurrencyRate is null)
                {
                    loChequeEntryViewModel.Data.NLBASE_RATE = 1;
                    loChequeEntryViewModel.Data.NLCURRENCY_RATE = 1;
                    loChequeEntryViewModel.Data.NBBASE_RATE = 1;
                    loChequeEntryViewModel.Data.NBCURRENCY_RATE = 1;
                }
                else
                {
                    loChequeEntryViewModel.Data.NLBASE_RATE = loChequeEntryViewModel.loRefreshCurrencyRate.NLBASE_RATE_AMOUNT;
                    loChequeEntryViewModel.Data.NLCURRENCY_RATE = loChequeEntryViewModel.loRefreshCurrencyRate.NLCURRENCY_RATE_AMOUNT;
                    loChequeEntryViewModel.Data.NBBASE_RATE = loChequeEntryViewModel.loRefreshCurrencyRate.NBBASE_RATE_AMOUNT;
                    loChequeEntryViewModel.Data.NBCURRENCY_RATE = loChequeEntryViewModel.loRefreshCurrencyRate.NBCURRENCY_RATE_AMOUNT;
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
                loChequeEntryViewModel.Data.DREF_DATE = poParam;
                if (!loChequeEntryViewModel.loInitialProcess.CBSystemParam.LINPUT_CHEQUE_DATE)
                {
                    loChequeEntryViewModel.Data.DCHEQUE_DATE = poParam;
                }
                if (!string.IsNullOrWhiteSpace(loChequeEntryViewModel.Data.CCURRENCY_CODE) &&
                    (loChequeEntryViewModel.Data.CCURRENCY_CODE != loChequeEntryViewModel.loInitialProcess.CompanyInfo.CLOCAL_CURRENCY_CODE
                    || loChequeEntryViewModel.Data.CCURRENCY_CODE != loChequeEntryViewModel.loInitialProcess.CompanyInfo.CBASE_CURRENCY_CODE))
                {
                    await RefreshCurrency();
                }
                else
                {
                    loChequeEntryViewModel.Data.NLBASE_RATE = 1;
                    loChequeEntryViewModel.Data.NLCURRENCY_RATE = 1;
                    loChequeEntryViewModel.Data.NBBASE_RATE = 1;
                    loChequeEntryViewModel.Data.NBCURRENCY_RATE = 1;
                }
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            loEx.ThrowExceptionIfErrors();
        }
        #endregion

        #region Detail
        private async Task ChequeDetailServiceGetListRecord(R_ServiceGetListRecordEventArgs eventArgs)
        {
            R_Exception loEx = new R_Exception();
            try
            {
                await loChequeEntryViewModel.GetChequeEntryDetailListStreamAsync();
                CBT02210DTO loHeaderData = (CBT02210DTO)_conductorRef.R_GetCurrentData();
                if (_gridDetailRef.DataSource.Count > 0)
                {
                    loHeaderData.NDEBIT_AMOUNT = _gridDetailRef.DataSource.Sum(x => x.NDEBIT);
                    loHeaderData.NCREDIT_AMOUNT = _gridDetailRef.DataSource.Sum(x => x.NCREDIT);
                }
                eventArgs.ListEntityResult = loChequeEntryViewModel.loChequeDetailList;
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            loEx.ThrowExceptionIfErrors();
        }
        private async Task ChequeDetailServiceGetRecord(R_ServiceGetRecordEventArgs eventArgs)
        {
            R_Exception loEx = new R_Exception();
            try
            {
                CBT02210DetailDTO loData = (CBT02210DetailDTO)eventArgs.Data;
                await loChequeEntryViewModel.GetChequeDetailAsync(loData);

                eventArgs.Result = loChequeEntryViewModel.loChequeDetail;
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            loEx.ThrowExceptionIfErrors();
        }
        private bool EnableCenterList = true;
        private void ChequeDetailDisplay(R_DisplayEventArgs eventArgs)
        {
            var loEx = new R_Exception();
            try
            {
                var loData = (CBT02210DetailDTO)eventArgs.Data;
                var loHeaderData = (CBT02210DTO)_conductorRef.R_GetCurrentData();

                if (loData != null)
                {
                    EnableCenterList = (loData.CBSIS == "B" && loChequeEntryViewModel.loInitialProcess.CompanyInfo.LENABLE_CENTER_BS == true) || (loData.CBSIS == "I" && loChequeEntryViewModel.loInitialProcess.CompanyInfo.LENABLE_CENTER_IS == true);
                }

                if (eventArgs.ConductorMode == R_eConductorMode.Normal)
                {
                    if (_gridDetailRef.DataSource.Count > 0)
                    {
                        loHeaderData.NDEBIT_AMOUNT = _gridDetailRef.DataSource.Sum(x => x.NDEBIT);
                        loHeaderData.NCREDIT_AMOUNT = _gridDetailRef.DataSource.Sum(x => x.NCREDIT);
                    }
                }
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            loEx.ThrowExceptionIfErrors();
        }
        private void ChequeDetail_SetOther(R_SetEventArgs eventArgs)
        {
            IsDetailCRUDMode = eventArgs.Enable;
        }

        private void ChequeDetailAfterAdd(R_AfterAddEventArgs eventArgs)
        {
            var loEx = new R_Exception();
            try
            {
                CBT02210DTO loHeaderData = (CBT02210DTO)_conductorRef.R_GetCurrentData();
                CBT02210DetailDTO loData = (CBT02210DetailDTO)eventArgs.Data;

                loData.INO = loChequeEntryViewModel.loChequeDetailList.Count() + 1;
                loData.CDETAIL_DESC = loHeaderData.CTRANS_DESC;
                loData.CDOCUMENT_NO = loHeaderData.CCHEQUE_NO;
                loData.DDOCUMENT_DATE = loHeaderData.DCHEQUE_DATE.Value;
                //loData.CDOCUMENT_NO = string.IsNullOrWhiteSpace(loHeaderData.CDOC_NO) ? "" : loHeaderData.CDOC_NO;
                //loData.CDOCUMENT_DATE = string.IsNullOrWhiteSpace(loHeaderData.CDOC_DATE) ? "" : loHeaderData.DDOC_DATE.Value.ToString("yyyyMMdd");
                //loData.DDOCUMENT_DATE = loHeaderData.DDOC_DATE.Value;
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            loEx.ThrowExceptionIfErrors();
        }
        private void Before_Open_Lookup(R_BeforeOpenGridLookupColumnEventArgs eventArgs)
        {
            if (eventArgs.ColumnName == nameof(CBT02210DetailDTO.CGLACCOUNT_NO))
            {
                eventArgs.Parameter = new GSL00500ParameterDTO()
                {
                    CPROGRAM_CODE = "GLM00100",
                };
                eventArgs.TargetPageType = typeof(GSL00500);
            }
            else if (eventArgs.ColumnName == nameof(CBT02210DetailDTO.CCASH_FLOW_CODE))
            {
                eventArgs.Parameter = new GSL01500ParameterGroupDTO()
                {
                };
                eventArgs.TargetPageType = typeof(GSL01500);
            }
        }
        private void After_Open_Lookup(R_AfterOpenGridLookupColumnEventArgs eventArgs)
        {
            CBT02210DetailDTO loGetData = (CBT02210DetailDTO)eventArgs.ColumnData;
            if (eventArgs.ColumnName == nameof(CBT02210DetailDTO.CGLACCOUNT_NO))
            {
                GSL00500DTO loTempResult = (GSL00500DTO)eventArgs.Result;
                if (loTempResult == null)
                {
                    return;
                }

                loGetData.CCASH_FLOW_CODE = loTempResult.CCASH_FLOW_CODE;
                loGetData.CCASH_FLOW_GROUP_CODE = loTempResult.CCASH_FLOW_GROUP_CODE;
                loGetData.CCASH_FLOW_NAME = loTempResult.CCASH_FLOW_NAME;
                loGetData.CGLACCOUNT_NO = loTempResult.CGLACCOUNT_NO;
                loGetData.CGLACCOUNT_NAME = loTempResult.CGLACCOUNT_NAME;
                loGetData.CBSIS = loTempResult.CBSIS;
                if (loTempResult.CBSIS == "I" && loChequeEntryViewModel.loInitialProcess.CompanyInfo.LENABLE_CENTER_IS == false || loTempResult.CBSIS == "B" && loChequeEntryViewModel.loInitialProcess.CompanyInfo.LENABLE_CENTER_BS == false)
                {
                    loGetData.CCENTER_CODE = "";
                    loGetData.CCENTER_NAME = "";
                }
            }
            else if (eventArgs.ColumnName == nameof(CBT02210DetailDTO.CCASH_FLOW_CODE))
            {
                GSL01500DTO loTempResult = R_FrontUtility.ConvertObjectToObject<GSL01500DTO>(eventArgs.Result);
                if (loTempResult == null)
                {
                    return;
                }

                loGetData.CCASH_FLOW_GROUP_CODE = loTempResult.CCASH_FLOW_GROUP_CODE;
                loGetData.CCASH_FLOW_CODE = loTempResult.CCASH_FLOW_CODE;
                loGetData.CCASH_FLOW_NAME = loTempResult.CCASH_FLOW_NAME;
            }

        }
        private async Task ChequeDetailServiceDelete(R_ServiceDeleteEventArgs eventArgs)
        {
            var loEx = new R_Exception();
            try
            {
                CBT02210DetailDTO loData = (CBT02210DetailDTO)eventArgs.Data;

                await loChequeEntryViewModel.DeleteChequeDetailAsync(loData);
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            loEx.ThrowExceptionIfErrors();
        }
        private void ChequeDetailValidation(R_ValidationEventArgs eventArgs)
        {
            var loEx = new R_Exception();
            try
            {
                var loData = (CBT02210DetailDTO)eventArgs.Data;
                if (string.IsNullOrWhiteSpace(loData.CGLACCOUNT_NO))
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                        typeof(Resources_Dummy_Class),
                        "V025"));
                }


                if (_gridDetailRef.DataSource.Any(x => x.CGLACCOUNT_NO == loData.CGLACCOUNT_NO) && eventArgs.ConductorMode == R_eConductorMode.Add)
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                        typeof(Resources_Dummy_Class),
                        "V026"));
                }

                if (string.IsNullOrWhiteSpace(loData.CCENTER_CODE) && (loData.CBSIS == "B" && loChequeEntryViewModel.loInitialProcess.CompanyInfo.LENABLE_CENTER_BS) || (loData.CBSIS == "I" && loChequeEntryViewModel.loInitialProcess.CompanyInfo.LENABLE_CENTER_IS))
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                        typeof(Resources_Dummy_Class),
                        "V027"));
                }

                if (string.IsNullOrWhiteSpace(loData.CCASH_FLOW_CODE) && loChequeEntryViewModel.loInitialProcess.CompanyInfo.LCASH_FLOW)
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                        typeof(Resources_Dummy_Class),
                        "V028"));
                }

                if (loData.NDEBIT == 0 && loData.NCREDIT == 0)
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                        typeof(Resources_Dummy_Class),
                        "V029"));
                }

                if (loData.NDEBIT > 0 && loData.NCREDIT > 0)
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                        typeof(Resources_Dummy_Class),
                        "V030"));
                }

                if (string.IsNullOrWhiteSpace(loData.CDETAIL_DESC))
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                        typeof(Resources_Dummy_Class),
                        "V031"));
                }

                if (string.IsNullOrWhiteSpace(loData.CDOCUMENT_NO))
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                        typeof(Resources_Dummy_Class),
                        "V032"));
                }

                if (loData.DDOCUMENT_DATE.HasValue == false)
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                        typeof(Resources_Dummy_Class),
                        "V033"));
                }
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            loEx.ThrowExceptionIfErrors();
        }
        private void ChequeDetailSaving(R_SavingEventArgs eventArgs)
        {
            var loEx = new R_Exception();
            try
            {
                CBT02210DTO loHeaderData = (CBT02210DTO)_conductorRef.R_GetCurrentData();
                CBT02210DetailDTO loData = (CBT02210DetailDTO)eventArgs.Data;

                loData.CDOCUMENT_DATE = loData.DDOCUMENT_DATE.Value.ToString("yyyyMMdd");
                loData.CDBCR = loData.NDEBIT > 0 && loData.NCREDIT == 0 ? "D" : loData.NCREDIT > 0 && loData.NDEBIT == 0 ? "C" : "";
                loData.NTRANS_AMOUNT = loData.NDEBIT > 0 ? loData.NDEBIT : loData.NCREDIT;
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            loEx.ThrowExceptionIfErrors();
        }

        private async Task ChequeDetailServiceSave(R_ServiceSaveEventArgs eventArgs)
        {
            R_Exception loEx = new R_Exception();
            try
            {
                CBT02210DetailDTO loData = (CBT02210DetailDTO)eventArgs.Data;
                if (string.IsNullOrEmpty(loData.CCENTER_CODE))
                {
                    loData.CCENTER_CODE = "";
                }
                await loChequeEntryViewModel.SaveChequeDetailAsync(loData, (eCRUDMode)eventArgs.ConductorMode);

                eventArgs.Result = loChequeEntryViewModel.loChequeDetail;
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            loEx.ThrowExceptionIfErrors();
        }
        private async Task ChequeDetailBeforeEdit(R_BeforeEditEventArgs eventArgs)
        {
            R_Exception loEx = new R_Exception();
            try
            {
                CBT02210DetailDTO loData = (CBT02210DetailDTO)eventArgs.Data;
                if (loData.CINPUT_TYPE == "A")
                {
                    await R_MessageBox.Show("", _localizer["M007"], R_eMessageBoxButtonType.OK);
                    eventArgs.Cancel = true;
                    return;
                }
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            loEx.ThrowExceptionIfErrors();
        }
        private async Task ChequeDetailBeforeDelete(R_BeforeDeleteEventArgs eventArgs)
        {
            var loEx = new R_Exception();
            bool llInputType = false;
            bool llValidateType = false;

            try
            {
                var loData = (CBT02210DetailDTO)eventArgs.Data;
                if (loData.CINPUT_TYPE == "A")
                {
                    await R_MessageBox.Show("", _localizer["M008"], R_eMessageBoxButtonType.OK);
                    eventArgs.Cancel = true;
                    return;
                }

                var loValidate = await R_MessageBox.Show("", _localizer["Q007"], R_eMessageBoxButtonType.YesNo);
                if (loValidate == R_eMessageBoxResult.No)
                {
                    eventArgs.Cancel = true;
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
        private async Task ApproveChequeProcess()
        {
            R_Exception loEx = new R_Exception();
            UpdateStatusParameterDTO loParam = new UpdateStatusParameterDTO();
            R_eMessageBoxResult loValidate;
            try
            {
                //var loData = (CBT02200DTO)_conductorRef.R_GetCurrentData();
                if (!loChequeEntryViewModel.loChequeHeader.LALLOW_APPROVE)
                {
                    loEx.Add("", _localizer["M002"]);
                    goto EndBlock;
                }

                loValidate = await R_MessageBox.Show("", _localizer["Q008"], R_eMessageBoxButtonType.YesNo);
                if (loValidate == R_eMessageBoxResult.No)
                    goto EndBlock;


                //var loParam = R_FrontUtility.ConvertObjectToObject<CBT02200UpdateStatusDTO>(loData);
                loParam.CREC_ID_LIST = loChequeEntryViewModel.loChequeHeader.CREC_ID;
                //loParam.LAUTO_COMMIT = false;
                //loParam.LUNDO_COMMIT = false;
                loParam.CNEW_STATUS = "20";

                await loChequeEntryViewModel.UpdateStatusAsync(loParam);
                await _conductorRef.R_GetEntity(loChequeEntryViewModel.loChequeHeader);
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }
        EndBlock:
            loEx.ThrowExceptionIfErrors();
        }
        private async Task SubmitChequeProcess()
        {
            R_Exception loEx = new R_Exception();
            R_eMessageBoxResult loResult;
            bool llValidate = false;
            UpdateStatusParameterDTO loParam = new UpdateStatusParameterDTO();
            try
            {
                CBT02210DTO loData = (CBT02210DTO)_conductorRef.R_GetCurrentData();

                if (loData.CSTATUS == "00" && int.Parse(loData.CREF_PRD) < int.Parse(loChequeEntryViewModel.loInitialProcess.CBSystemParam.CSOFT_PERIOD))
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                        typeof(Resources_Dummy_Class),
                        "V004"));
                    llValidate = true;
                }

                if ((loData.NDEBIT_AMOUNT > 0 || loData.NCREDIT_AMOUNT > 0) && loData.NCREDIT_AMOUNT != loData.NDEBIT_AMOUNT)
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                        typeof(Resources_Dummy_Class),
                        "V005"));
                    llValidate = true;
                }

                if (loData.NDEBIT_AMOUNT == 0 || loData.NCREDIT_AMOUNT == 0)
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                         typeof(Resources_Dummy_Class),
                         "V006"));
                    llValidate = true;
                }

                if (loData.NDEBIT_AMOUNT > 0 && loData.NDEBIT_AMOUNT != loData.NTRANS_AMOUNT)
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                         typeof(Resources_Dummy_Class),
                         "V007"));
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
                        loResult = await R_MessageBox.Show("", _localizer["Q004"], R_eMessageBoxButtonType.YesNo);
                        if (loResult == R_eMessageBoxResult.No)
                        {
                            return;
                        }
                    }
                    else
                    {
                        loResult = await R_MessageBox.Show("", _localizer["Q005"], R_eMessageBoxButtonType.YesNo);
                        if (loResult == R_eMessageBoxResult.No)
                        {
                            return;
                        }
                    }

                    //var loParam = R_FrontUtility.ConvertObjectToObject<CBT02200UpdateStatusDTO>(loData);
                    loParam.CREC_ID_LIST = loChequeEntryViewModel.loChequeHeader.CREC_ID;
                    //loParam.LAUTO_COMMIT = false;
                    //loParam.LUNDO_COMMIT = false;
                    loParam.CNEW_STATUS = loData.CSTATUS == "00" ? "10" : "00";

                    await loChequeEntryViewModel.UpdateStatusAsync(loParam);
                    await _conductorRef.R_GetEntity(loChequeEntryViewModel.loChequeHeader);
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
                var loData = (CBT02210DTO)_conductorRef.R_GetCurrentData();
                if (loData.CDEPT_CODE.Length > 0)
                {
                    if (lcOldDeptCode != loData.CDEPT_CODE)
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
                            loData.CDEPT_CODE = "";
                            loData.CDEPT_NAME = "";
                            loData.CCB_CODE = "";
                            loData.CCB_NAME = "";
                            loData.CCB_ACCOUNT_NO = "";
                            loData.CCB_ACCOUNT_NAME = "";
                            loData.CCURRENCY_CODE = "";
                            lcOldDeptCode = "";
                            goto EndBlock;
                        }
                        loData.CDEPT_CODE = loResult.CDEPT_CODE;
                        loData.CDEPT_NAME = loResult.CDEPT_NAME;
                        loData.CCB_CODE = "";
                        loData.CCB_NAME = "";
                        loData.CCB_ACCOUNT_NO = "";
                        loData.CCB_ACCOUNT_NAME = "";
                        loData.CCURRENCY_CODE = "";
                        lcOldDeptCode = loData.CDEPT_CODE;
                    }
                }
                else
                {
                    loData.CDEPT_NAME = "";
                    loData.CCB_CODE = "";
                    loData.CCB_NAME = "";
                    loData.CCB_ACCOUNT_NO = "";
                    loData.CCB_ACCOUNT_NAME = "";
                    loData.CCURRENCY_CODE = "";
                    lcOldDeptCode = "";
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
            GSL00700DTO loTempResult = (GSL00700DTO)eventArgs.Result;
            if (loTempResult == null)
            {
                return;
            }

            CBT02210DTO loData = (CBT02210DTO)_conductorRef.R_GetCurrentData();
            loData.CDEPT_CODE = loTempResult.CDEPT_CODE;
            loData.CDEPT_NAME = loTempResult.CDEPT_NAME;
            loData.CCB_CODE = "";
            loData.CCB_NAME = "";
            loData.CCB_ACCOUNT_NO = "";
            loData.CCB_ACCOUNT_NAME = "";
            loData.CCURRENCY_CODE = "";
        }
        #endregion

        #region lookupCashCode
        private async Task BankCode_OnLostFocus(object poParam)
        {
            R_Exception loEx = new R_Exception();

            try
            {
                CBT02210DTO loData = (CBT02210DTO)_conductorRef.R_GetCurrentData();
                if (loData.CCB_CODE.Length > 0)
                {
                    if (loData.CCB_CODE != lcOldCBCode)
                    {
                        GSL02500ParameterDTO loParam = new GSL02500ParameterDTO()
                        {
                            CDEPT_CODE = loData.CDEPT_CODE,
                            CCB_TYPE = "B",
                            CBANK_TYPE = "I",
                            CSEARCH_TEXT = loData.CCB_CODE
                        };

                        LookupGSL02500ViewModel loLookupViewModel = new LookupGSL02500ViewModel();

                        GSL02500DTO loResult = await loLookupViewModel.GetCB(loParam);

                        if (loResult == null)
                        {
                            loEx.Add(R_FrontUtility.R_GetError(
                                    typeof(Lookup_GSFrontResources.Resources_Dummy_Class),
                                    "_ErrLookup01"));
                            loData.CCB_CODE = "";
                            loData.CCB_NAME = "";
                            loData.CCURRENCY_CODE = "";
                            loData.CCB_ACCOUNT_NO = "";
                            loData.CCB_ACCOUNT_NAME = "";
                            lcOldCBCode = "";
                            goto EndBlock;
                        }
                        loData.CCB_CODE = loResult.CCB_CODE;
                        loData.CCB_NAME = loResult.CCB_NAME;
                        //lcType = loResult.CCB_TYPE;
                        lcOldCBCode = loData.CCB_CODE;

                        loData.CCURRENCY_CODE = "";
                        loData.CCB_ACCOUNT_NO = "";
                        loData.CCB_ACCOUNT_NAME = "";
                    }
                }
                else
                {
                    loData.CCB_NAME = "";
                    loData.CCURRENCY_CODE = "";
                    loData.CCB_ACCOUNT_NO = "";
                    loData.CCB_ACCOUNT_NAME = "";
                    //lcType = "";
                    lcOldCBCode = "";
                }
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }
        EndBlock:
            R_DisplayException(loEx);
        }
        private void Before_Open_lookupBankCode(R_BeforeOpenLookupEventArgs eventArgs)
        {
            var loData = (CBT02210DTO)_conductorRef.R_GetCurrentData();
            GSL02500ParameterDTO loParam = new GSL02500ParameterDTO()
            {
                CDEPT_CODE = loData.CDEPT_CODE,
                CCB_TYPE = "B",
                CBANK_TYPE = "I"
            };
            eventArgs.Parameter = loParam;
            eventArgs.TargetPageType = typeof(GSL02500);
        }
        private async Task After_Open_lookupBankCode(R_AfterOpenLookupEventArgs eventArgs)
        {
            var loTempResult = (GSL02500DTO)eventArgs.Result;
            if (loTempResult == null)
            {
                return;
            }

            CBT02210DTO loData = (CBT02210DTO)_conductorRef.R_GetCurrentData();
            loData.CCB_CODE = loTempResult.CCB_CODE;
            loData.CCB_NAME = loTempResult.CCB_NAME;
            //lcType = loTempResult.CCB_TYPE;

            loData.CCURRENCY_CODE = "";
            loData.CCB_ACCOUNT_NO = "";
            loData.CCB_ACCOUNT_NAME = "";
        }
        #endregion

        #region lookupCashCodeAccount
        private async Task BankCodeAccount_OnLostFocus(object poParam)
        {
            R_Exception loEx = new R_Exception();

            try
            {
                CBT02210DTO loData = (CBT02210DTO)_conductorRef.R_GetCurrentData();
                if (loData.CCB_ACCOUNT_NO.Length > 0)
                {
                    if (lcOldCBAccountNo != loData.CCB_ACCOUNT_NO)
                    {
                        GSL02600ParameterDTO loParam = new GSL02600ParameterDTO()
                        {
                            CDEPT_CODE = loData.CDEPT_CODE,
                            CCB_TYPE = "B",
                            CBANK_TYPE = "I",
                            CCB_CODE = loData.CCB_CODE,
                            CSEARCH_TEXT = loData.CCB_ACCOUNT_NO
                        };

                        LookupGSL02600ViewModel loLookupViewModel = new LookupGSL02600ViewModel();

                        GSL02600DTO loResult = await loLookupViewModel.GetCBAccount(loParam);

                        if (loResult == null)
                        {
                            loEx.Add(R_FrontUtility.R_GetError(
                                    typeof(Lookup_GSFrontResources.Resources_Dummy_Class),
                                    "_ErrLookup01"));
                            loData.CCB_ACCOUNT_NO = "";
                            loData.CCB_ACCOUNT_NAME = "";
                            loData.CCURRENCY_CODE = "";
                            lcOldCBAccountNo = "";
                            goto EndBlock;
                        }
                        loData.CCB_ACCOUNT_NO = loResult.CCB_ACCOUNT_NO;
                        loData.CCB_ACCOUNT_NAME = loResult.CCB_ACCOUNT_NAME;
                        loData.CCURRENCY_CODE = loResult.CCURRENCY_CODE;
                        lcOldCBAccountNo = loData.CCB_ACCOUNT_NO;

                        if (!string.IsNullOrWhiteSpace(loData.CCURRENCY_CODE) &&
                        (loData.CCURRENCY_CODE != loChequeEntryViewModel.loInitialProcess.CompanyInfo.CLOCAL_CURRENCY_CODE
                        || loData.CCURRENCY_CODE != loChequeEntryViewModel.loInitialProcess.CompanyInfo.CBASE_CURRENCY_CODE))
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
                }
                else
                {
                    loData.CCB_ACCOUNT_NO = "";
                    loData.CCB_ACCOUNT_NAME = "";
                    loData.CCURRENCY_CODE = "";
                    lcOldCBAccountNo = "";
                }
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }
        EndBlock:
            R_DisplayException(loEx);
        }
        private void Before_Open_lookupBankCodeAccount(R_BeforeOpenLookupEventArgs eventArgs)
        {
            var loData = (CBT02210DTO)_conductorRef.R_GetCurrentData();
            GSL02600ParameterDTO loParam = new GSL02600ParameterDTO()
            {
                CDEPT_CODE = loData.CDEPT_CODE,
                CCB_TYPE = "B",
                CBANK_TYPE = "I",
                CCB_CODE = loData.CCB_CODE
            };
            eventArgs.Parameter = loParam;
            eventArgs.TargetPageType = typeof(GSL02600);
        }
        private async Task After_Open_lookupBankCodeAccount(R_AfterOpenLookupEventArgs eventArgs)
        {
            var loTempResult = (GSL02600DTO)eventArgs.Result;
            if (loTempResult == null)
            {
                return;
            }

            CBT02210DTO loData = (CBT02210DTO)_conductorRef.R_GetCurrentData();
            loData.CCB_ACCOUNT_NO = loTempResult.CCB_ACCOUNT_NO;
            loData.CCB_ACCOUNT_NAME = loTempResult.CCB_ACCOUNT_NAME;
            loData.CCURRENCY_CODE = loTempResult.CCURRENCY_CODE;
            if (!string.IsNullOrWhiteSpace(loData.CCURRENCY_CODE) &&
                    (loData.CCURRENCY_CODE != loChequeEntryViewModel.loInitialProcess.CompanyInfo.CLOCAL_CURRENCY_CODE
                    || loData.CCURRENCY_CODE != loChequeEntryViewModel.loInitialProcess.CompanyInfo.CBASE_CURRENCY_CODE))
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

        //#region lookupDept
        //private async Task DeptCode_OnLostFocus(object poParam)
        //{
        //    var loEx = new R_Exception();

        //    try
        //    {
        //        var loData = (CBT02210DTO)_conductorRef.R_GetCurrentData();
        //        if (loData.CDEPT_CODE.Length > 0)
        //        {
        //            GSL00700ParameterDTO loParam = new GSL00700ParameterDTO()
        //            {
        //                CSEARCH_TEXT = loData.CDEPT_CODE
        //            };

        //            LookupGSL00700ViewModel loLookupViewModel = new LookupGSL00700ViewModel();

        //            var loResult = await loLookupViewModel.GetDepartment(loParam);

        //            if (loResult == null)
        //            {
        //                loEx.Add(R_FrontUtility.R_GetError(
        //                        typeof(Lookup_GSFrontResources.Resources_Dummy_Class),
        //                        "_ErrLookup01"));
        //                loData.CDEPT_NAME = "";
        //                goto EndBlock;
        //            }
        //            loData.CDEPT_CODE = loResult.CDEPT_CODE;
        //            loData.CDEPT_NAME = loResult.CDEPT_NAME;
        //        }
        //        else
        //        {
        //            loData.CDEPT_NAME = "";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        loEx.Add(ex);
        //    }
        //EndBlock:
        //    R_DisplayException(loEx);
        //}
        //private void Before_Open_lookupDept(R_BeforeOpenLookupEventArgs eventArgs)
        //{
        //    var param = new GSL00700ParameterDTO
        //    {
        //    };
        //    eventArgs.Parameter = param;
        //    eventArgs.TargetPageType = typeof(GSL00700);
        //}
        //private void After_Open_lookupDept(R_AfterOpenLookupEventArgs eventArgs)
        //{
        //    var loTempResult = (GSL00700DTO)eventArgs.Result;
        //    if (loTempResult == null)
        //    {
        //        return;
        //    }

        //    var loData = (CBT02210DTO)_conductorRef.R_GetCurrentData();
        //    loData.CDEPT_CODE = loTempResult.CDEPT_CODE;
        //    loData.CDEPT_NAME = loTempResult.CDEPT_NAME;
        //}
        //#endregion

        //#region lookupBankCode
        //private async Task BankCode_OnLostFocus(object poParam)
        //{
        //    var loEx = new R_Exception();

        //    try
        //    {
        //        var loData = (CBT02210DTO)_conductorRef.R_GetCurrentData();
        //        if (loData.CCB_CODE.Length > 0)
        //        {
        //            GSL02500ParameterDTO loParam = new GSL02500ParameterDTO()
        //            {
        //                CDEPT_CODE = loData.CDEPT_CODE,
        //                CCB_TYPE = "B",
        //                CBANK_TYPE = "I",
        //                CSEARCH_TEXT = loData.CCB_CODE
        //            };

        //            LookupGSL02500ViewModel loLookupViewModel = new LookupGSL02500ViewModel();

        //            GSL02500DTO loResult = await loLookupViewModel.GetCB(loParam);

        //            if (loResult == null)
        //            {
        //                loEx.Add(R_FrontUtility.R_GetError(
        //                        typeof(Lookup_GSFrontResources.Resources_Dummy_Class),
        //                        "_ErrLookup01"));
        //                loData.CCB_NAME = "";
        //                loData.CCURRENCY_CODE = "";
        //                loData.CCB_ACCOUNT_NO = "";
        //                goto EndBlock;
        //            }
        //            loData.CCB_CODE = loResult.CCB_CODE;
        //            loData.CCB_NAME = loResult.CCB_NAME;
        //            loData.CCURRENCY_CODE = loResult.CCURRENCY_CODE;
        //            loData.CCB_ACCOUNT_NO = loResult.CCB_ACCOUNT_NO;

        //            if (!string.IsNullOrWhiteSpace(loData.CCURRENCY_CODE) &&
        //            (loData.CCURRENCY_CODE != loChequeEntryViewModel.loInitialProcess.CompanyInfo.CLOCAL_CURRENCY_CODE
        //            || loData.CCURRENCY_CODE != loChequeEntryViewModel.loInitialProcess.CompanyInfo.CBASE_CURRENCY_CODE))
        //            {
        //                await RefreshCurrency();
        //            }
        //            else
        //            {
        //                loData.NLBASE_RATE = 1;
        //                loData.NLCURRENCY_RATE = 1;
        //                loData.NBBASE_RATE = 1;
        //                loData.NBCURRENCY_RATE = 1;
        //            }
        //        }
        //        else
        //        {
        //            loData.CCB_NAME = "";
        //            loData.CCURRENCY_CODE = "";
        //            loData.CCB_ACCOUNT_NO = "";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        loEx.Add(ex);
        //    }
        //EndBlock:
        //    R_DisplayException(loEx);
        //}
        //private void Before_Open_lookupBankCode(R_BeforeOpenLookupEventArgs eventArgs)
        //{
        //    var loData = (CBT02210DTO)_conductorRef.R_GetCurrentData();
        //    GSL02500ParameterDTO loParam = new GSL02500ParameterDTO()
        //    {
        //        CDEPT_CODE = loData.CDEPT_CODE,
        //        CCB_TYPE = "B",
        //        CBANK_TYPE = "I"
        //    };
        //    eventArgs.Parameter = loParam;
        //    eventArgs.TargetPageType = typeof(GSL02500);
        //}
        //private async Task After_Open_lookupBankCode(R_AfterOpenLookupEventArgs eventArgs)
        //{
        //    var loTempResult = (GSL02500DTO)eventArgs.Result;
        //    if (loTempResult == null)
        //    {
        //        return;
        //    }

        //    CBT02210DTO loData = (CBT02210DTO)_conductorRef.R_GetCurrentData();
        //    loData.CCB_CODE = loTempResult.CCB_CODE;
        //    loData.CCB_NAME = loTempResult.CCB_NAME;
        //    loData.CCURRENCY_CODE = loTempResult.CCURRENCY_CODE;
        //    loData.CCB_ACCOUNT_NO = loTempResult.CCB_ACCOUNT_NO;
        //    if (!string.IsNullOrWhiteSpace(loData.CCURRENCY_CODE) &&
        //            (loData.CCURRENCY_CODE != loChequeEntryViewModel.loInitialProcess.CompanyInfo.CLOCAL_CURRENCY_CODE
        //            || loData.CCURRENCY_CODE != loChequeEntryViewModel.loInitialProcess.CompanyInfo.CBASE_CURRENCY_CODE))
        //    {
        //        await RefreshCurrency();
        //    }
        //    else
        //    {
        //        loData.NLBASE_RATE = 1;
        //        loData.NLCURRENCY_RATE = 1;
        //        loData.NBBASE_RATE = 1;
        //        loData.NBCURRENCY_RATE = 1;
        //    }
        //}
        //#endregion

        #region Result Predifiend

        #endregion
    }
}
