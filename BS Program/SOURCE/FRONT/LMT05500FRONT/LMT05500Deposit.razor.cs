using BlazorClientHelper;
using PMT05500COMMON.DTO;
using PMT05500Model.ViewModel;
using Microsoft.AspNetCore.Components;
using R_BlazorFrontEnd.Controls;
using R_BlazorFrontEnd.Controls.DataControls;
using R_BlazorFrontEnd.Controls.Events;
using R_BlazorFrontEnd.Controls.Tab;
using R_BlazorFrontEnd.Enums;
using R_BlazorFrontEnd.Exceptions;
using R_BlazorFrontEnd.Helpers;
using PMT05500COMMON;
using R_BlazorFrontEnd.Controls.Popup;

namespace PMT05500Front
{
    public partial class LMT05500Deposit
    {
        private LMT05500DepositViewModel _depositViewModel = new();
        private R_Grid<LMT05500DepositListDTO>? _gridDepositRef;
        private R_Grid<LMT05500DepositDetailListDTO>? _gridDepositDetailRef;

        private R_ConductorGrid? _conGridDeposit;
        private R_ConductorGrid? _conGridDepositDetail;

        private R_TabStrip? _tabStripDeposit;
        private R_TabPage? _tabPageSubDeposit;

        private bool _pageDepositOnCRUDmode;
        [Inject] private IClientHelper? _clientHelper { get; set; }
        [Inject] private R_PopupService?_popUpService { get; set; }
        protected override async Task R_Init_From_Master(object poParameter)
        {
            var loEx = new R_Exception();
            try
            {
                if ((LMT05500DBParameter)poParameter != null)
                {
                    _depositViewModel._currentDataAgreement = (LMT05500DBParameter)poParameter;

                    await R_ServiceHeaderRecord((LMT05500DBParameter)poParameter);
                    await _gridDepositRef!.R_RefreshGrid(null);
                }
                else
                {
                    _depositViewModel._currentDataAgreement = null;
                }
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            loEx.ThrowExceptionIfErrors();
        }

        #region HeaderDeposit
        private async Task R_ServiceHeaderRecord(LMT05500DBParameter poParameter)
        {
            var loEx = new R_Exception();
            try
            {
                poParameter.CCOMPANY_ID = _clientHelper!.CompanyId;
                poParameter.CUSER_ID = _clientHelper.UserId;
                await _depositViewModel.GetHeaderDeposit(poParameter);
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            await R_DisplayExceptionAsync(loEx);
        }
        #endregion

        #region Deposit List
        private async Task R_ServiceDepositListRecord(R_ServiceGetListRecordEventArgs eventArgs)
        {
            var loEx = new R_Exception();
            try
            {
                await _depositViewModel.GetAllDepositList();
                eventArgs.ListEntityResult = _depositViewModel._depositList;

                if (_depositViewModel._depositList.Count > 0)
                {
                    await _gridDepositDetailRef!.R_RefreshGrid(null);
                }
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            loEx.ThrowExceptionIfErrors();
        }
        private async Task Grid_DisplayDeposit(R_DisplayEventArgs eventArgs)
        {
            if (eventArgs.ConductorMode == R_eConductorMode.Normal)
            {
                var loParam = (LMT05500DepositListDTO)eventArgs.Data;

                _depositViewModel._currentDeposit = loParam;
                _depositViewModel._buttonOnDepositGrid = loParam.LPAYMENT;

                await _gridDepositDetailRef!.R_RefreshGrid(null);
            }
        }

        #endregion

        #region DepositDetail List
        private async Task R_ServiceDepositDetailListRecord(R_ServiceGetListRecordEventArgs eventArgs)
        {
            var loEx = new R_Exception();
            try
            {
                await _depositViewModel.GetAllDepositDetailList();
                eventArgs.ListEntityResult = _depositViewModel._depositDetailList;


            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            loEx.ThrowExceptionIfErrors();
        }
        #endregion

        #region Button Customize
        private async Task R_Before_BtnAdjustment(R_BeforeOpenDetailEventArgs eventArgs)
        {
            var loEx = new R_Exception();
            try
            {
                //  string codeSpec = "GLT00100"; //GLT00100 (Journal Entries)
                var temp = _depositViewModel._currentDeposit;

                PMT05500ParamDepositDTO loParam = new PMT05500ParamDepositDTO()
                {
                    CCHEQUE_ID = "",
                    PARAM_CALLER_ID = "PMT05500",
                    PARAM_CALLER_TRANS_CODE = _depositViewModel._currentDeposit.CTRANS_CODE!, //"802030"
                    PARAM_CALLER_REF_NO = _depositViewModel._currentDeposit.CREF_NO!, //"CheckFIN-20240400002-PM"
                    PARAM_CALLER_ACTION = "NEW",
                    PARAM_DEPT_CODE = _depositViewModel._currentDeposit.CDEPT_CODE!, //"FIN"
                    //belum tau
                    PARAM_REF_NO = "",
                    PARAM_DOC_NO = _depositViewModel._currentDeposit.CDOC_NO!, //""
                    PARAM_DOC_DATE = _depositViewModel._currentDeposit.CDOC_DATE!, //""
                    //belum tau
                    PARAM_DESCRIPTION = "Adjustmant Journal",
                    PARAM_GLACCOUNT_NO = _depositViewModel._currentDeposit.CGLACCOUNT_NO!, //"'0A155"
                    PARAM_CENTER_CODE = _depositViewModel._currentDeposit.CCENTER_CODE!, // ""
                    PARAM_CASH_FLOW_GROUP_CODE = _depositViewModel._currentDeposit.CCASH_FLOW_GROUP_CODE!,
                    PARAM_CASH_FLOW_CODE = _depositViewModel._currentDeposit.CCASH_FLOW_CODE!,
                    PARAM_AMOUNT = _depositViewModel._currentDeposit.NREMAINING_AMOUNT,
                    PARAM_CURRENCY_CODE = _depositViewModel._currentDeposit.CCURRENCY_CODE!,
                    PARAM_LC_BASE_RATE = _depositViewModel._currentDeposit.NLBASE_RATE_AMOUNT,
                    PARAM_LC_RATE = _depositViewModel._currentDeposit.NLCURRENCY_RATE_AMOUNT,
                    PARAM_BC_BASE_RATE = _depositViewModel._currentDeposit.NBBASE_RATE_AMOUNT,
                    PARAM_BC_RATE = _depositViewModel._currentDeposit.NBCURRENCY_RATE_AMOUNT,
                    PARAM_BSIS = _depositViewModel._currentDeposit.CBSIS, //"B"
                    PARAM_DBCR = _depositViewModel._currentDeposit.CDBCR,  //"D"
                    //PARAM_GLACCOUNT_NAME = _depositViewModel._currentDeposit.CGLACCOUNT_NO,
                    //PARAM_CENTER_NAME = _depositViewModel._currentDeposit.CGLACCOUNT_NO,
                    //PARAM_DEPT_NAME = _depositViewModel._currentDeposit.CGLACCOUNT_NO,
                };

                eventArgs.Parameter = loParam;
                eventArgs.PageNamespace = "UtilityFront.TryLookUp";
                // eventArgs.TargetPageType = typeof(GLT00110);
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }
            await R_DisplayExceptionAsync(loEx);
        }
        private void R_After_BtnAdjustment(R_AfterOpenDetailEventArgs eventArgs)
        {

        }
        private async Task R_Before_BtnRefund(R_BeforeOpenDetailEventArgs eventArgs)
        {
            var loEx = new R_Exception();
            try
            {
                //  string codeSpec = "GLT00100"; //GLT00100 (Journal Entries)
                var temp = _depositViewModel._currentDeposit;

                /*
                PMT05500ParamDepositDTO loParam = new PMT05500ParamDepositDTO()
                {
                    CCHEQUE_ID = "",
                    PARAM_CALLER_ID = "PMT05500",
                    PARAM_CALLER_TRANS_CODE = "802030",
                    PARAM_CALLER_REF_NO = _depositViewModel._currentDeposit.CREF_NO, //belum tau
                    PARAM_CALLER_ACTION = "VIEW",
                    PARAM_DEPT_CODE = "ACC",

                    PARAM_REF_NO = "CPJ-2024040001", //belum tau

                    PARAM_DOC_NO = "Doc-01",
                    PARAM_DOC_DATE = "20240417",

                    PARAM_DESCRIPTION = "", //belum tau

                    PARAM_GLACCOUNT_NO = _depositViewModel._currentDeposit.CGLACCOUNT_NO,
                    PARAM_CENTER_CODE = _depositViewModel._currentDeposit.CCENTER_CODE,
                    PARAM_CASH_FLOW_GROUP_CODE = _depositViewModel._currentDeposit.CCASH_FLOW_GROUP_CODE,
                    PARAM_CASH_FLOW_CODE = _depositViewModel._currentDeposit.CCASH_FLOW_CODE,
                    PARAM_AMOUNT = _depositViewModel._currentDeposit.NREMAINING_AMOUNT
                };
                */

                PMT05500ParamDepositDTO loParam = new PMT05500ParamDepositDTO()
                {
                    CCHEQUE_ID = "",
                    PARAM_CALLER_ID = "PMT05500",
                    PARAM_CALLER_TRANS_CODE = _depositViewModel._currentDeposit.CTRANS_CODE!,
                    PARAM_CALLER_REF_NO = _depositViewModel._currentDeposit.CREF_NO!,
                    PARAM_CALLER_ACTION = "NEW",
                    PARAM_DEPT_CODE = _depositViewModel._currentDeposit.CDEPT_CODE!,

                    PARAM_REF_NO = "", //belum tau

                    PARAM_DOC_NO = _depositViewModel._currentDeposit.CDOC_NO!,
                    PARAM_DOC_DATE = _depositViewModel._currentDeposit.CDOC_DATE!,

                    PARAM_DESCRIPTION = "", //belum tau

                    PARAM_GLACCOUNT_NO = _depositViewModel._currentDeposit.CGLACCOUNT_NO!,
                    PARAM_CENTER_CODE = _depositViewModel._currentDeposit.CCENTER_CODE!,
                    PARAM_CASH_FLOW_GROUP_CODE = _depositViewModel._currentDeposit.CCASH_FLOW_GROUP_CODE!,
                    PARAM_CASH_FLOW_CODE = _depositViewModel._currentDeposit.CCASH_FLOW_CODE!,
                    PARAM_AMOUNT = _depositViewModel._currentDeposit.NREMAINING_AMOUNT
                };
                eventArgs.Parameter = loParam;
                var loValue = await _popUpService!.Show(typeof(PopUpRefund), "");

                ResultPopUpDTO loResultPopup = (ResultPopUpDTO)loValue.Result;

                if (loResultPopup.IS_SUCCESS)
                {
                    string lcRefundValue = loResultPopup.CODE_PROGRAM!;

                    switch (lcRefundValue)
                    {
                        case "00200":
                            eventArgs.PageNamespace = "UtilityFront.TryLookUp";
                            //eventArgs.PageNamespace = typeof(CBT00220);
                            break;
                        case "01200":
                            eventArgs.PageNamespace = "UtilityFront.TryLookUp";
                            // eventArgs.TargetPageType = typeof(CBT01210);
                            break;
                        case "02200":
                            eventArgs.PageNamespace = "UtilityFront.TryLookUp";
                            //    eventArgs.TargetPageType = typeof(CBT02220);
                            break;
                    };
                }

            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }
            await R_DisplayExceptionAsync(loEx);
        }
        private void R_After_BtnRefund(R_AfterOpenDetailEventArgs eventArgs)
        {

        }
        #endregion

        #region CHANGE SUBTAB
        //CHANGE TAB
        private void SetOther(R_SetEventArgs eventArgs)
        {
            _pageDepositOnCRUDmode = eventArgs.Enable;
        }
        private void Before_Open_DepositInfo(R_BeforeOpenTabPageEventArgs eventArgs)
        {
            var loEx = new R_Exception();
            try
            {
                LMT05500DepositListDTO loCurrentData = _depositViewModel._currentDeposit;

                if (loCurrentData != null)
                {
                    var loTemp = (LMT05500DepositListDTO)_gridDepositRef!.GetCurrentData();
                    loTemp.CCURRENCY_CODE = _depositViewModel._headerDeposit.CCURRENCY_CODE!;

                    eventArgs.Parameter = loTemp;
                }
                else
                {
                    LMT05500DepositListDTO loParam = new LMT05500DepositListDTO()
                    {
                        CPROPERTY_ID = _depositViewModel._currentDataAgreement.CPROPERTY_ID!,
                        CCHARGE_MODE = _depositViewModel._currentDataAgreement.CCHARGE_MODE,
                        CBUILDING_ID = _depositViewModel._currentDataAgreement.CBUILDING_ID,
                        CFLOOR_ID = _depositViewModel._currentDataAgreement.CFLOOR_ID,
                        CUNIT_ID = _depositViewModel._currentDataAgreement.CUNIT_ID,
                        CDEPT_CODE = _depositViewModel._currentDataAgreement.CDEPT_CODE!,
                        CTRANS_CODE = _depositViewModel._currentDataAgreement.CTRANS_CODE!,
                        CREF_NO = _depositViewModel._currentDataAgreement.CREF_NO!,
                        CCURRENCY_CODE = _depositViewModel._headerDeposit.CCURRENCY_CODE!
                    };
                    eventArgs.Parameter = loParam;
                }

                eventArgs.TargetPageType = typeof(LMT05500DepositInfo);
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }
        }
        private async Task OnActiveTabDepositIndexChangingAsync(R_TabStripActiveTabIndexChangingEventArgs eventArgs)
        {
            //that used, when user on crud mode and changing the tab. then the command  will be cancelled

            //   eventArgs.Cancel = !_pageDepositOnCRUDmode;

            if (_pageDepositOnCRUDmode && eventArgs.TabStripTab.Id == "TabDepositList")
            {
                await _gridDepositRef!.R_RefreshGrid(null);
            }
            else if (!_pageDepositOnCRUDmode)
            {
                eventArgs.Cancel = true;
            }
        }
        //   public bool _disableTab = true;
        private void R_TabEventCallback(object poValue)
        {
            var loEx = new R_Exception();

            try
            {
                if (_depositViewModel._currentDataAgreement != null)
                {
                    _pageDepositOnCRUDmode = (bool)poValue;
                }
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }
            loEx.ThrowExceptionIfErrors();

        }
        #endregion
    }
}
