using BlazorClientHelper;
using CBT02200COMMON.DTO;
using CBT02200COMMON.DTO.CBT02200;
using CBT02200COMMON.DTO.CBT02210;
using CBT02200FrontResources;
using CBT02200MODEL.ViewModel;
using Lookup_GSCOMMON.DTOs;
using Lookup_GSFRONT;
using Lookup_GSModel.ViewModel;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.VisualBasic;
using R_BlazorFrontEnd;
using R_BlazorFrontEnd.Controls;
using R_BlazorFrontEnd.Controls.DataControls;
using R_BlazorFrontEnd.Controls.Enums;
using R_BlazorFrontEnd.Controls.Events;
using R_BlazorFrontEnd.Controls.MessageBox;
using R_BlazorFrontEnd.Controls.Popup;
using R_BlazorFrontEnd.Controls.Tab;
using R_BlazorFrontEnd.Enums;
using R_BlazorFrontEnd.Exceptions;
using R_BlazorFrontEnd.Helpers;
using R_BlazorFrontEnd.Interfaces;
using R_CommonFrontBackAPI;
using R_LockingFront;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CBT02200FRONT
{
    public partial class CBT02200 : R_Page
    {
        [Inject] IClientHelper clientHelper { get; set; }
        [Inject] private R_ILocalizer<CBT02200FrontResources.Resources_Dummy_Class> _localizer { get; set; }

        private CBT02200ViewModel loViewModel = new();

        private R_Conductor _conductorHeaderRef;

        private R_ConductorGrid _conductorDetailRef;

        private R_Grid<CBT02200GridDTO> _gridHeaderRef;

        private R_Grid<CBT02200GridDetailDTO> _gridDetailRef;

        private List<PeriodMonth> loPeriodMonth = new List<PeriodMonth>()
        {
            new PeriodMonth() { CCODE = "01" },
            new PeriodMonth() { CCODE = "02" },
            new PeriodMonth() { CCODE = "03" },
            new PeriodMonth() { CCODE = "04" },
            new PeriodMonth() { CCODE = "05" },
            new PeriodMonth() { CCODE = "06" },
            new PeriodMonth() { CCODE = "07" },
            new PeriodMonth() { CCODE = "08" },
            new PeriodMonth() { CCODE = "09" },
            new PeriodMonth() { CCODE = "10" },
            new PeriodMonth() { CCODE = "11" },
            new PeriodMonth() { CCODE = "12" }
        };

        private string lcOldDeptCode = "";
        private string lcOldCBCode = "";
        private string lcOldCBAccountNo = "";

        private bool llEnableApprove = false;

        private bool IsDebitVisible = false;

        private bool IsCreditVisible = false;


        protected override async Task R_Init_From_Master(object poParameter)
        {
            R_Exception loEx = new R_Exception();

            try
            {
                await loViewModel.InitialProcessAsync();
                await loViewModel.GetStatusListStreamAsync();

                loViewModel.loTransCodeInfo = loViewModel.loInitialProcess.TransCodeInfo;

                loViewModel.loHeader.CDEPT_CODE = loViewModel.loInitialProcess.GLSystemParam.CCLOSE_DEPT_CODE;
                loViewModel.loHeader.CDEPT_NAME = loViewModel.loInitialProcess.GLSystemParam.CCLOSE_DEPT_NAME;
                loViewModel.loHeader.IPERIOD_YY = int.Parse(loViewModel.loInitialProcess.CBSystemParam.CSOFT_PERIOD_YY);
                loViewModel.loHeader.CPERIOD_MM = loViewModel.loInitialProcess.CBSystemParam.CSOFT_PERIOD_MM;
                loViewModel.loHeader.CSTATUS = "";
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            loEx.ThrowExceptionIfErrors();
        }

        public async Task OnclickSearch()
        {
            R_Exception loEx = new R_Exception();
            try
            {
                if (string.IsNullOrEmpty(loViewModel.loHeader.CDEPT_CODE))
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                        typeof(Resources_Dummy_Class),
                        "V001"));
                }
                if (string.IsNullOrEmpty(loViewModel.loHeader.CSEARCH_TEXT))
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                        typeof(Resources_Dummy_Class),
                        "V002"));
                }
                else
                {
                    if (loViewModel.loHeader.CSEARCH_TEXT.Length < 3)
                    {
                        loEx.Add(R_FrontUtility.R_GetError(
                        typeof(Resources_Dummy_Class),
                        "V003"));
                    }
                }
                if (!loEx.HasError)
                {
                    loViewModel.IsSearchFunction = true;
                    await _gridHeaderRef.R_RefreshGrid(null);
                }
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }
            loEx.ThrowExceptionIfErrors();
        }

        public async Task OnClickShowAll()
        {
            R_Exception loEx = new R_Exception();
            try
            {
                loViewModel.IsSearchFunction = false;
                await _gridHeaderRef.R_RefreshGrid(null);
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }
            loEx.ThrowExceptionIfErrors();

        }

        #region Cheque Header 

        private async Task ChequeHeaderGrid_ServiceGetListRecord(R_ServiceGetListRecordEventArgs eventArgs)
        {
            R_Exception loEx = new R_Exception();
            try
            {
                await loViewModel.GetChequeHeaderListStreamAsync();
                eventArgs.ListEntityResult = loViewModel.loChequeHeaderList;
                if (loViewModel.loChequeHeaderList.Count == 0)
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                        typeof(Resources_Dummy_Class),
                        "M001"));
                    if (_gridDetailRef.DataSource.Count > 0)
                    {
                        _gridDetailRef.DataSource.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }
            R_DisplayException(loEx);
        }

        private void ChequeHeaderGrid_ServiceGetRecord(R_ServiceGetRecordEventArgs eventArgs)
        {
            eventArgs.Result = eventArgs.Data;
        }

        private async Task ChequeHeaderGrid_Display(R_DisplayEventArgs eventArgs)
        {
            R_Exception loEx = new R_Exception();
            try
            {
                loViewModel.loChequeHeader = (CBT02200GridDTO)eventArgs.Data;
                if (eventArgs.ConductorMode == R_eConductorMode.Normal)
                {
                    if (loViewModel.loChequeHeader.CSTATUS == "10" && loViewModel.loInitialProcess.TransCodeInfo.LAPPROVAL_FLAG)
                    {
                        llEnableApprove = true;
                    }
                    else
                    {
                        llEnableApprove = false;
                    }

                    if (!string.IsNullOrWhiteSpace(loViewModel.loChequeHeader.CREC_ID))
                    {
                        await _gridDetailRef.R_RefreshGrid(eventArgs.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }
            R_DisplayException(loEx);
        }
        private async Task ApproveChequeProcess()
        {
            R_Exception loEx = new R_Exception();
            UpdateStatusParameterDTO loParam = new UpdateStatusParameterDTO();
            try
            {
                //var loData = (CBT02200GridDTO)_conductorHeaderRef.R_GetCurrentData();
                if (!loViewModel.loChequeHeader.LALLOW_APPROVE)
                {
                    loEx.Add("", _localizer["M002"]);
                    goto EndBlock;
                }

                loParam.CREC_ID_LIST = loViewModel.loChequeHeader.CREC_ID;
                loParam.CNEW_STATUS = "20";
                //loParam.LAUTO_COMMIT = false;
                //loParam.LUNDO_COMMIT = false;

                await loViewModel.UpdateStatusAsync(loParam);
                await _gridHeaderRef.R_RefreshGrid(null);

                if (!loEx.HasError)
                {
                    await R_MessageBox.Show("", _localizer["M003"], R_eMessageBoxButtonType.OK);
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

        #region ChequeDetailGrid
        private async Task ChequeDetailGrid_ServiceGetListRecord(R_ServiceGetListRecordEventArgs eventArgs)
        {
            R_Exception loEx = new R_Exception();
            try
            {
                await loViewModel.GetChequeDetailListStreamAsync();
                eventArgs.ListEntityResult = loViewModel.loChequeDetailList;
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }
            R_DisplayException(loEx);
        }
        private void ChequeDetailGrid_ServiceGetRecord(R_ServiceGetRecordEventArgs eventArgs)
        {
            eventArgs.Result = eventArgs.Data;
        }

        private void ChequeDetailGrid_Display(R_DisplayEventArgs eventArgs)
        {
            if (eventArgs.ConductorMode == R_eConductorMode.Normal)
            {
                loViewModel.loChequeDetail = (CBT02200GridDetailDTO)eventArgs.Data;
            }
        }
        #endregion

        #region lookupDept
        private async Task DeptCode_OnLostFocus(object poParam)
        {
            var loEx = new R_Exception();

            try
            {
                if (loViewModel.loHeader.CDEPT_CODE.Length > 0)
                {
                    if (lcOldDeptCode != loViewModel.loHeader.CDEPT_CODE)
                    {
                        GSL00700ParameterDTO loParam = new GSL00700ParameterDTO()
                        {
                            CSEARCH_TEXT = loViewModel.loHeader.CDEPT_CODE
                        };

                        LookupGSL00700ViewModel loLookupViewModel = new LookupGSL00700ViewModel();

                        var loResult = await loLookupViewModel.GetDepartment(loParam);

                        if (loResult == null)
                        {
                            loEx.Add(R_FrontUtility.R_GetError(
                                    typeof(Lookup_GSFrontResources.Resources_Dummy_Class),
                                    "_ErrLookup01"));
                            loViewModel.loHeader.CDEPT_CODE = "";
                            loViewModel.loHeader.CDEPT_NAME = "";
                            loViewModel.loHeader.CCB_CODE = "";
                            loViewModel.loHeader.CCB_NAME = "";
                            loViewModel.loHeader.CCB_ACCOUNT_NO = "";
                            loViewModel.loHeader.CCB_ACCOUNT_NAME = "";
                            //loViewModel.loHeader.CCURRENCY_CODE = "";
                            lcOldDeptCode = "";
                            goto EndBlock;
                        }
                        loViewModel.loHeader.CDEPT_CODE = loResult.CDEPT_CODE;
                        loViewModel.loHeader.CDEPT_NAME = loResult.CDEPT_NAME;
                        loViewModel.loHeader.CCB_CODE = "";
                        loViewModel.loHeader.CCB_NAME = "";
                        loViewModel.loHeader.CCB_ACCOUNT_NO = "";
                        loViewModel.loHeader.CCB_ACCOUNT_NAME = "";
                        //loViewModel.loHeader.CCURRENCY_CODE = "";
                        lcOldDeptCode = loViewModel.loHeader.CDEPT_CODE;
                    }
                }
                else
                {
                    loViewModel.loHeader.CDEPT_NAME = "";
                    loViewModel.loHeader.CCB_CODE = "";
                    loViewModel.loHeader.CCB_NAME = "";
                    loViewModel.loHeader.CCB_ACCOUNT_NO = "";
                    loViewModel.loHeader.CCB_ACCOUNT_NAME = "";
                    //loViewModel.loHeader.CCURRENCY_CODE = "";
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

            loViewModel.loHeader.CDEPT_CODE = loTempResult.CDEPT_CODE;
            loViewModel.loHeader.CDEPT_NAME = loTempResult.CDEPT_NAME;
            loViewModel.loHeader.CCB_CODE = "";
            loViewModel.loHeader.CCB_NAME = "";
            loViewModel.loHeader.CCB_ACCOUNT_NO = "";
            loViewModel.loHeader.CCB_ACCOUNT_NAME = "";
            //loViewModel.loHeader.CCURRENCY_CODE = "";
        }
        #endregion

        #region lookupCashCode
        private async Task BankCode_OnLostFocus(object poParam)
        {
            R_Exception loEx = new R_Exception();

            try
            {
                if (loViewModel.loHeader.CCB_CODE.Length > 0)
                {
                    if (loViewModel.loHeader.CCB_CODE != lcOldCBCode)
                    {
                        GSL02500ParameterDTO loParam = new GSL02500ParameterDTO()
                        {
                            CDEPT_CODE = loViewModel.loHeader.CDEPT_CODE,
                            CCB_TYPE = "B",
                            CBANK_TYPE = "I",
                            CSEARCH_TEXT = loViewModel.loHeader.CCB_CODE
                        };

                        LookupGSL02500ViewModel loLookupViewModel = new LookupGSL02500ViewModel();

                        GSL02500DTO loResult = await loLookupViewModel.GetCB(loParam);

                        if (loResult == null)
                        {
                            loEx.Add(R_FrontUtility.R_GetError(
                                    typeof(Lookup_GSFrontResources.Resources_Dummy_Class),
                                    "_ErrLookup01"));
                            loViewModel.loHeader.CCB_CODE = "";
                            loViewModel.loHeader.CCB_NAME = "";
                            //loViewModel.loHeader.CCURRENCY_CODE = "";
                            loViewModel.loHeader.CCB_ACCOUNT_NO = "";
                            loViewModel.loHeader.CCB_ACCOUNT_NAME = "";
                            lcOldCBCode = "";
                            goto EndBlock;
                        }
                        loViewModel.loHeader.CCB_CODE = loResult.CCB_CODE;
                        loViewModel.loHeader.CCB_NAME = loResult.CCB_NAME;
                        lcOldCBCode = loViewModel.loHeader.CCB_CODE;

                        //if (loResult.CCB_TYPE == "C")
                        //{
                        //    EnableAccountNo = false;
                        //    loViewModel.loHeader.CCURRENCY_CODE = loResult.CCURRENCY_CODE;
                        //    loViewModel.loHeader.CCB_ACCOUNT_NO = loResult.CCB_ACCOUNT_NO;
                        //    loViewModel.loHeader.CCB_ACCOUNT_NAME = loResult.CCB_ACCOUNT_NAME;
                        //    if (!string.IsNullOrWhiteSpace(loViewModel.loHeader.CCURRENCY_CODE) &&
                        //            (loViewModel.loHeader.CCURRENCY_CODE != loViewModel.loTabParameter.InitialProcess.CompanyInfo.CLOCAL_CURRENCY_CODE
                        //            || loViewModel.loHeader.CCURRENCY_CODE != loViewModel.loTabParameter.InitialProcess.CompanyInfo.CBASE_CURRENCY_CODE))
                        //    {
                        //        await RefreshCurrency();
                        //    }
                        //    else
                        //    {
                        //        loViewModel.loHeader.NLBASE_RATE = 1;
                        //        loViewModel.loHeader.NLCURRENCY_RATE = 1;
                        //        loViewModel.loHeader.NBBASE_RATE = 1;
                        //        loViewModel.loHeader.NBCURRENCY_RATE = 1;
                        //    }
                        //}
                        //else
                        //{
                        //    EnableAccountNo = true;
                        //    loViewModel.loHeader.CCURRENCY_CODE = "";
                        //    loViewModel.loHeader.CCB_ACCOUNT_NO = "";
                        //    loViewModel.loHeader.CCB_ACCOUNT_NAME = "";
                        //}
                    }
                }
                else
                {
                    loViewModel.loHeader.CCB_NAME = "";
                    //loViewModel.loHeader.CCURRENCY_CODE = "";
                    loViewModel.loHeader.CCB_ACCOUNT_NO = "";
                    loViewModel.loHeader.CCB_ACCOUNT_NAME = "";
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
        private void Before_Open_LookUpBank(R_BeforeOpenLookupEventArgs eventArgs)
        {
            GSL02500ParameterDTO loParam = new GSL02500ParameterDTO()
            {
                CDEPT_CODE = loViewModel.loHeader.CDEPT_CODE,
                CCB_TYPE = "B",
                CBANK_TYPE = "I"
            };
            eventArgs.Parameter = loParam;
            eventArgs.TargetPageType = typeof(GSL02500);
        }
        private async Task After_Open_LookUpBank(R_AfterOpenLookupEventArgs eventArgs)
        {
            var loTempResult = (GSL02500DTO)eventArgs.Result;
            if (loTempResult == null)
            {
                return;
            }

            loViewModel.loHeader.CCB_CODE = loTempResult.CCB_CODE;
            loViewModel.loHeader.CCB_NAME = loTempResult.CCB_NAME;

            //if (loTempResult.CCB_TYPE == "C")
            //{
            //    //loViewModel.loHeader.CCURRENCY_CODE = loTempResult.CCURRENCY_CODE;
            //    loViewModel.loHeader.CCB_ACCOUNT_NO = loTempResult.CCB_ACCOUNT_NO;
            //    loViewModel.loHeader.CCB_ACCOUNT_NAME = loTempResult.CCB_ACCOUNT_NAME;
            //    if (!string.IsNullOrWhiteSpace(loViewModel.loHeader.CCURRENCY_CODE) &&
            //            (loViewModel.loHeader.CCURRENCY_CODE != loViewModel.loTabParameter.InitialProcess.CompanyInfo.CLOCAL_CURRENCY_CODE
            //            || loViewModel.loHeader.CCURRENCY_CODE != loViewModel.loTabParameter.InitialProcess.CompanyInfo.CBASE_CURRENCY_CODE))
            //    {
            //        await RefreshCurrency();
            //    }
            //    else
            //    {
            //        loViewModel.loHeader.NLBASE_RATE = 1;
            //        loViewModel.loHeader.NLCURRENCY_RATE = 1;
            //        loViewModel.loHeader.NBBASE_RATE = 1;
            //        loViewModel.loHeader.NBCURRENCY_RATE = 1;
            //    }
            //}
            //else
            //{
            //    EnableAccountNo = true;
            //    loViewModel.loHeader.CCURRENCY_CODE = "";
            //    loViewModel.loHeader.CCB_ACCOUNT_NO = "";
            //    loViewModel.loHeader.CCB_ACCOUNT_NAME = "";
            //}
        }
        #endregion

        #region lookupCashCodeAccount
        private async Task AccountNo_OnLostFocus(object poParam)
        {
            R_Exception loEx = new R_Exception();

            try
            {
                if (loViewModel.loHeader.CCB_ACCOUNT_NO.Length > 0)
                {
                    if (lcOldCBAccountNo != loViewModel.loHeader.CCB_ACCOUNT_NO)
                    {
                        GSL02600ParameterDTO loParam = new GSL02600ParameterDTO()
                        {
                            CDEPT_CODE = loViewModel.loHeader.CDEPT_CODE,
                            CCB_TYPE = "B",
                            CBANK_TYPE = "I",
                            CCB_CODE = loViewModel.loHeader.CCB_CODE,
                            CSEARCH_TEXT = loViewModel.loHeader.CCB_ACCOUNT_NO
                        };

                        LookupGSL02600ViewModel loLookupViewModel = new LookupGSL02600ViewModel();

                        GSL02600DTO loResult = await loLookupViewModel.GetCBAccount(loParam);

                        if (loResult == null)
                        {
                            loEx.Add(R_FrontUtility.R_GetError(
                                    typeof(Lookup_GSFrontResources.Resources_Dummy_Class),
                                    "_ErrLookup01"));
                            loViewModel.loHeader.CCB_ACCOUNT_NO = "";
                            loViewModel.loHeader.CCB_ACCOUNT_NAME = "";
                            //loViewModel.loHeader.CCURRENCY_CODE = "";
                            lcOldCBAccountNo = "";
                            goto EndBlock;
                        }
                        loViewModel.loHeader.CCB_ACCOUNT_NO = loResult.CCB_ACCOUNT_NO;
                        loViewModel.loHeader.CCB_ACCOUNT_NAME = loResult.CCB_ACCOUNT_NAME;
                        //loViewModel.loHeader.CCURRENCY_CODE = loResult.CCURRENCY_CODE;
                        lcOldCBAccountNo = loViewModel.loHeader.CCB_ACCOUNT_NO;

                        //if (!string.IsNullOrWhiteSpace(loViewModel.loHeader.CCURRENCY_CODE) &&
                        //(loViewModel.loHeader.CCURRENCY_CODE != loViewModel.loTabParameter.InitialProcess.CompanyInfo.CLOCAL_CURRENCY_CODE
                        //|| loViewModel.loHeader.CCURRENCY_CODE != loViewModel.loTabParameter.InitialProcess.CompanyInfo.CBASE_CURRENCY_CODE))
                        //{
                        //    await RefreshCurrency();
                        //}
                        //else
                        //{
                        //    loViewModel.loHeader.NLBASE_RATE = 1;
                        //    loViewModel.loHeader.NLCURRENCY_RATE = 1;
                        //    loViewModel.loHeader.NBBASE_RATE = 1;
                        //    loViewModel.loHeader.NBCURRENCY_RATE = 1;
                        //}
                    }
                }
                else
                {
                    loViewModel.loHeader.CCB_ACCOUNT_NO = "";
                    loViewModel.loHeader.CCB_ACCOUNT_NAME = "";
                    //loViewModel.loHeader.CCURRENCY_CODE = "";
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

        private void TestValueChange(string poParam)
        {
            loViewModel.loHeader.CSTATUS = poParam;
        }

        private void Before_Open_lookupAccountNo(R_BeforeOpenLookupEventArgs eventArgs)
        {
            GSL02600ParameterDTO loParam = new GSL02600ParameterDTO()
            {
                CDEPT_CODE = loViewModel.loHeader.CDEPT_CODE,
                CCB_TYPE = "B",
                CBANK_TYPE = "I",
                CCB_CODE = loViewModel.loHeader.CCB_CODE
            };
            eventArgs.Parameter = loParam;
            eventArgs.TargetPageType = typeof(GSL02600);
        }
        private async Task After_Open_lookupAccountNo(R_AfterOpenLookupEventArgs eventArgs)
        {
            var loTempResult = (GSL02600DTO)eventArgs.Result;
            if (loTempResult == null)
            {
                return;
            }

            loViewModel.loHeader.CCB_ACCOUNT_NO = loTempResult.CCB_ACCOUNT_NO;
            loViewModel.loHeader.CCB_ACCOUNT_NAME = loTempResult.CCB_ACCOUNT_NAME;
            //loViewModel.loHeader.CCURRENCY_CODE = loTempResult.CCURRENCY_CODE;
            //if (!string.IsNullOrWhiteSpace(loViewModel.loHeader.CCURRENCY_CODE) &&
            //        (loViewModel.loHeader.CCURRENCY_CODE != loViewModel.loTabParameter.InitialProcess.CompanyInfo.CLOCAL_CURRENCY_CODE
            //        || loViewModel.loHeader.CCURRENCY_CODE != loViewModel.loTabParameter.InitialProcess.CompanyInfo.CBASE_CURRENCY_CODE))
            //{
            //    await RefreshCurrency();
            //}
            //else
            //{
            //    loViewModel.loHeader.NLBASE_RATE = 1;
            //    loViewModel.loHeader.NLCURRENCY_RATE = 1;
            //    loViewModel.loHeader.NBBASE_RATE = 1;
            //    loViewModel.loHeader.NBCURRENCY_RATE = 1;
            //}
        }
        #endregion

        //#region lookupDept
        //private async Task DeptCode_OnLostFocus()
        //{
        //    var loEx = new R_Exception();

        //    try
        //    {
        //        if (loViewModel.loHeader.CDEPT_CODE.Length > 0)
        //        {
        //            GSL00700ParameterDTO loParam = new GSL00700ParameterDTO() { CSEARCH_TEXT = loViewModel.loHeader.CDEPT_CODE };

        //            LookupGSL00700ViewModel loLookupViewModel = new LookupGSL00700ViewModel();

        //            var loResult = await loLookupViewModel.GetDepartment(loParam);

        //            if (loResult == null)
        //            {
        //                loEx.Add(R_FrontUtility.R_GetError(
        //                        typeof(Lookup_GSFrontResources.Resources_Dummy_Class),
        //                        "_ErrLookup01"));
        //                loViewModel.loHeader.CDEPT_NAME = "";
        //                goto EndBlock;
        //            }
        //            loViewModel.loHeader.CDEPT_CODE = loResult.CDEPT_CODE;
        //            loViewModel.loHeader.CDEPT_NAME = loResult.CDEPT_NAME;
        //        }
        //        else
        //        {
        //            loViewModel.loHeader.CDEPT_NAME = "";
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
        //    GSL00700ParameterDTO param = new GSL00700ParameterDTO
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

        //    loViewModel.loHeader.CDEPT_CODE = loTempResult.CDEPT_CODE;
        //    loViewModel.loHeader.CDEPT_NAME = loTempResult.CDEPT_NAME;
        //}
        //#endregion

        //#region LookUpBankCode
        //private async Task BankCode_OnLostFocus()
        //{
        //    var loEx = new R_Exception();

        //    try
        //    {
        //        if (loViewModel.loHeader.CCB_CODE.Length > 0)
        //        {
        //            GSL02500ParameterDTO loParam = new GSL02500ParameterDTO() { CSEARCH_TEXT = loViewModel.loHeader.CCB_CODE };

        //            LookupGSL02500ViewModel loLookupViewModel = new LookupGSL02500ViewModel();

        //            var loResult = await loLookupViewModel.GetCB(loParam);

        //            if (loResult == null)
        //            {
        //                loEx.Add(R_FrontUtility.R_GetError(
        //                        typeof(Lookup_GSFrontResources.Resources_Dummy_Class),
        //                        "_ErrLookup01"));
        //                loViewModel.loHeader.CCB_NAME = "";
        //                goto EndBlock;
        //            }
        //            loViewModel.loHeader.CCB_CODE = loResult.CCB_CODE;
        //            loViewModel.loHeader.CCB_NAME = loResult.CCB_NAME;
        //            loViewModel.loHeader.CCB_ACCOUNT_NO = "";
        //            loViewModel.loHeader.CCB_ACCOUNT_NAME = "";
        //        }
        //        else
        //        {
        //            loViewModel.loHeader.CCB_NAME = "";
        //            loViewModel.loHeader.CCB_ACCOUNT_NO = "";
        //            loViewModel.loHeader.CCB_ACCOUNT_NAME = "";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        loEx.Add(ex);
        //    }
        //EndBlock:
        //    R_DisplayException(loEx);
        //}
        //private void Before_Open_LookUpBank(R_BeforeOpenLookupEventArgs eventArgs)
        //{
        //    GSL02500ParameterDTO param = new GSL02500ParameterDTO
        //    {
        //        CDEPT_CODE = loViewModel.loHeader.CDEPT_CODE,
        //        CCB_TYPE = "B",
        //        CBANK_TYPE = "I"
        //    };
        //    eventArgs.Parameter = param;
        //    eventArgs.TargetPageType = typeof(GSL02500);
        //}
        //private void After_Open_LookUpBank(R_AfterOpenLookupEventArgs eventArgs)
        //{
        //    var loTempResult = (GSL02500DTO)eventArgs.Result;
        //    if (loTempResult == null)
        //    {
        //        return;
        //    }

        //    loViewModel.loHeader.CCB_CODE = loTempResult.CCB_CODE;
        //    loViewModel.loHeader.CCB_NAME = loTempResult.CCB_NAME;
        //    loViewModel.loHeader.CCB_ACCOUNT_NO = "";
        //    loViewModel.loHeader.CCB_ACCOUNT_NAME = "";
        //}
        //#endregion

        //#region lookupAccountNo
        //private async Task AccountNo_OnLostFocus(object poParam)
        //{
        //    var loEx = new R_Exception();

        //    try
        //    {
        //        if (loViewModel.loHeader.CCB_ACCOUNT_NO.Length > 0)
        //        {
        //            GSL02600ParameterDTO loParam = new GSL02600ParameterDTO()
        //            {
        //                CDEPT_CODE = loViewModel.loHeader.CDEPT_CODE,
        //                CCB_TYPE = "B",
        //                CBANK_TYPE = "I",
        //                CCB_CODE = loViewModel.loHeader.CCB_CODE,
        //                CSEARCH_TEXT = loViewModel.loHeader.CCB_ACCOUNT_NO
        //            };

        //            LookupGSL02600ViewModel loLookupViewModel = new LookupGSL02600ViewModel();

        //            GSL02600DTO loResult = await loLookupViewModel.GetCBAccount(loParam);

        //            if (loResult == null)
        //            {
        //                loEx.Add(R_FrontUtility.R_GetError(
        //                        typeof(Lookup_GSFrontResources.Resources_Dummy_Class),
        //                        "_ErrLookup01"));
        //                loViewModel.loHeader.CCB_ACCOUNT_NAME = "";
        //                goto EndBlock;
        //            }
        //            loViewModel.loHeader.CCB_ACCOUNT_NO = loResult.CCB_ACCOUNT_NO;
        //            loViewModel.loHeader.CCB_ACCOUNT_NAME = loResult.CCB_ACCOUNT_NAME;
        //        }
        //        else
        //        {
        //            loViewModel.loHeader.CCB_ACCOUNT_NAME = "";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        loEx.Add(ex);
        //    }
        //EndBlock:
        //    R_DisplayException(loEx);
        //}
        //private void Before_Open_lookupAccountNo(R_BeforeOpenLookupEventArgs eventArgs)
        //{
        //    GSL02600ParameterDTO loParam = new GSL02600ParameterDTO()
        //    {
        //        CDEPT_CODE = loViewModel.loHeader.CDEPT_CODE,
        //        CCB_TYPE = "B",
        //        CBANK_TYPE = "I",
        //        CCB_CODE = loViewModel.loHeader.CCB_CODE,
        //    };
        //    eventArgs.Parameter = loParam;
        //    eventArgs.TargetPageType = typeof(GSL02600);
        //}
        //private async Task After_Open_lookupAccountNo(R_AfterOpenLookupEventArgs eventArgs)
        //{
        //    var loTempResult = (GSL02600DTO)eventArgs.Result;
        //    if (loTempResult == null)
        //    {
        //        return;
        //    }

        //    loViewModel.loHeader.CCB_ACCOUNT_NO = loTempResult.CCB_ACCOUNT_NO;
        //    loViewModel.loHeader.CCB_ACCOUNT_NAME = loTempResult.CCB_ACCOUNT_NAME;
        //}
        //#endregion

        #region Predefine Cheque Entry
        private void Predef_ChequeEntry(R_InstantiateDockEventArgs eventArgs)
        {
            eventArgs.Parameter = new ChequeEntryTabParameterDTO()
            {
                CCHEQUE_ID = loViewModel.loChequeHeader.CREC_ID
            };
            eventArgs.TargetPageType = typeof(CBT02210);
        }
        private async Task AfterPredef_ChequeEntry(R_AfterOpenPredefinedDockEventArgs eventArgs)
        {
            var loEx = new R_Exception();
            try
            {
                await _gridHeaderRef.R_RefreshGrid(null);
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }
            loEx.ThrowExceptionIfErrors();
        }

        #endregion

    }
    public class PeriodMonth
    {
        public string CCODE { get; set; }
    }
}
