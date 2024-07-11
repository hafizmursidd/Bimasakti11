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
using R_BlazorFrontEnd.Controls.Events;
using R_BlazorFrontEnd.Controls.MessageBox;
using R_BlazorFrontEnd.Enums;
using R_BlazorFrontEnd.Exceptions;
using R_BlazorFrontEnd.Helpers;
using R_BlazorFrontEnd.Interfaces;

namespace CBT00200FRONT
{
    public partial class CBT00200 : R_Page
    {
        private CBT00200ViewModel _viewModel = new();
        private R_Conductor _conductorRef;
        private R_ConductorGrid _conductorDetailRef;
        private R_Grid<CBT00200DTO> _gridRef;
        private R_Grid<CBT00210DTO> _gridDetailRef;

        [Inject] IClientHelper clientHelper { get; set; }
        [Inject] private R_ILocalizer<CBT00200FrontResources.Resources_Dummy_Class> _localizer { get; set; }

        protected override async Task R_Init_From_Master(object poParameter)
        {
            var loEx = new R_Exception();

            try
            {
                await _viewModel.GetAllUniversalData();

                //Set Dept Code
                if (_viewModel.VAR_DEPT_CODE_LIST.Any(x => x.CDEPT_CODE == _viewModel.VAR_GL_SYSTEM_PARAM.CCLOSE_DEPT_CODE))
                {
                    _viewModel.JornalParam.CDEPT_CODE = _viewModel.VAR_GL_SYSTEM_PARAM.CCLOSE_DEPT_CODE;
                    _viewModel.JornalParam.CDEPT_NAME = _viewModel.VAR_GL_SYSTEM_PARAM.CCLOSE_DEPT_NAME;
                }

                //Set Journal Period
                if (!string.IsNullOrWhiteSpace(_viewModel.VAR_CB_SYSTEM_PARAM.CSOFT_PERIOD_YY))
                    _viewModel.JournalPeriodYear = int.Parse(_viewModel.VAR_CB_SYSTEM_PARAM.CSOFT_PERIOD_YY);

                _viewModel.JournalPeriodMonth = _viewModel.VAR_CB_SYSTEM_PARAM.CSOFT_PERIOD_MM;
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            R_DisplayException(loEx);
        }

        public async Task OnclickSearch()
        {
            var loEx = new R_Exception();
            bool loValidate = false;

            try
            {
                if (string.IsNullOrWhiteSpace(_viewModel.JornalParam.CDEPT_CODE))
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                        typeof(Resources_Dummy_Class),
                        "V01"));
                    loValidate = true;
                }

                if (string.IsNullOrEmpty(_viewModel.JornalParam.CSEARCH_TEXT))
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                        typeof(Resources_Dummy_Class),
                        "N02"));
                    loValidate = true;
                }
                else
                {
                    if (_viewModel.JornalParam.CSEARCH_TEXT.Length < 3)
                    {
                        loEx.Add(R_FrontUtility.R_GetError(
                        typeof(Resources_Dummy_Class),
                        "N03"));
                        loValidate = true;
                    }
                }

                if (loValidate == false)
                {
                    await _gridRef.R_RefreshGrid(null);
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
            var loEx = new R_Exception();
            bool loValidate = false;

            try
            {
                if (string.IsNullOrWhiteSpace(_viewModel.JornalParam.CDEPT_CODE))
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                        typeof(Resources_Dummy_Class),
                        "V01"));
                    loValidate = true;
                }

                if (loValidate == false)
                {
                    await _gridRef.R_RefreshGrid(null);
                }
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }
            loEx.ThrowExceptionIfErrors();
        }

        #region JournalGrid
        private async Task JournalGrid_ServiceGetListRecord(R_ServiceGetListRecordEventArgs eventArgs)
        {
            var loEx = new R_Exception();
            try
            {
                await _viewModel.GetJournalList();
                eventArgs.ListEntityResult = _viewModel.JournalGrid;
                if (_viewModel.JournalGrid.Count <= 0)
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                        typeof(Resources_Dummy_Class),
                        "N01"));
                    if (_gridDetailRef.DataSource.Count > 0)
                        _gridDetailRef.DataSource.Clear();
                }
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }
            R_DisplayException(loEx);
        }
        private void JournalGrid_ServiceGetRecord(R_ServiceGetRecordEventArgs eventArgs)
        {
            eventArgs.Result = eventArgs.Data;
        }
        private string lcCommitLabel = "Commit";
        private async Task JournalGrid_Display(R_DisplayEventArgs eventArgs)
        {
            R_Exception loEx = new R_Exception();
            try
            {
                var loData = (CBT00200DTO)eventArgs.Data;
                if (eventArgs.ConductorMode == R_eConductorMode.Normal)
                {
                    lcCommitLabel = loData.CSTATUS == "80" ? _localizer["_UndoCommit"] : _localizer["_Commit"];
                    if (!string.IsNullOrWhiteSpace(loData.CREC_ID))
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

                await _viewModel.UpdateJournalStatus(loParam);
                await _gridRef.R_RefreshGrid(null);
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
                    if (_viewModel.VAR_GSM_TRANSACTION_CODE.LAPPROVAL_FLAG == true)
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

                await _viewModel.UpdateJournalStatus(loParam);
                await _gridRef.R_RefreshGrid(null);
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }
        EndBlock:
            loEx.ThrowExceptionIfErrors();
        }
        #endregion

        #region JournalGridDetail
        private async Task JournalGridDetail_ServiceGetListRecord(R_ServiceGetListRecordEventArgs eventArgs)
        {
            var loEx = new R_Exception();
            try
            {
                var loParam = R_FrontUtility.ConvertObjectToObject<CBT00210DTO>(eventArgs.Parameter);
                await _viewModel.GetJournalDetailList(loParam);
                eventArgs.ListEntityResult = _viewModel.JournalDetailGrid;
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }
            R_DisplayException(loEx);
        }
        private void JournalGridDetail_ServiceGetRecord(R_ServiceGetRecordEventArgs eventArgs)
        {
            eventArgs.Result = eventArgs.Data;
        }
        private CBT00210DTO JournalDetail { get; set; } = new CBT00210DTO();

        private void JournalGridDetail_Display(R_DisplayEventArgs eventArgs)
        {
            if (eventArgs.ConductorMode == R_eConductorMode.Normal)
            {
                JournalDetail = (CBT00210DTO)eventArgs.Data;
            }
        }
        #endregion

        #region lookupDept
        private async Task DeptCode_OnLostFocus()
        {
            var loEx = new R_Exception();

            try
            {
                if (string.IsNullOrWhiteSpace(_viewModel.JornalParam.CDEPT_CODE) == false)
                {
                    GSL00700ParameterDTO loParam = new GSL00700ParameterDTO() { CSEARCH_TEXT = _viewModel.JornalParam.CDEPT_CODE };

                    LookupGSL00700ViewModel loLookupViewModel = new LookupGSL00700ViewModel();

                    var loResult = await loLookupViewModel.GetDepartment(loParam);

                    if (loResult == null)
                    {
                        loEx.Add(R_FrontUtility.R_GetError(
                                typeof(Lookup_GSFrontResources.Resources_Dummy_Class),
                                "_ErrLookup01"));
                        _viewModel.JornalParam.CDEPT_NAME = "";
                        goto EndBlock;
                    }
                    _viewModel.JornalParam.CDEPT_CODE = loResult.CDEPT_CODE;
                    _viewModel.JornalParam.CDEPT_NAME = loResult.CDEPT_NAME;
                }
                else
                {
                    _viewModel.JornalParam.CDEPT_NAME = "";
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

            _viewModel.JornalParam.CDEPT_CODE = loTempResult.CDEPT_CODE;
            _viewModel.JornalParam.CDEPT_NAME = loTempResult.CDEPT_NAME;
        }
        #endregion

        #region Predefine Journal Entry
        private void Predef_JournalEntry(R_InstantiateDockEventArgs eventArgs)
        {
            var loData = _conductorRef.R_GetCurrentData();
            eventArgs.TargetPageType = typeof(CBT00210);
            eventArgs.Parameter = loData;
        }
        private async Task AfterPredef_JournalEntry(R_AfterOpenPredefinedDockEventArgs eventArgs)
        {
            var loEx = new R_Exception();
            try
            {
                //await _gridRef.R_RefreshGrid(null);
                await _viewModel.GetAllUniversalData();
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }
            loEx.ThrowExceptionIfErrors();
        }

        #endregion

        private void R_Before_Testing_Detail(R_BeforeOpenDetailEventArgs eventArgs)
        {
            eventArgs.Parameter = new CBT00200ParamDepositDTO()
            {
                PARAM_CALLER_ID = "PMT05500",
                PARAM_CALLER_TRANS_CODE = "802030",
                PARAM_CALLER_REF_NO = "LM-2024040001",
                PARAM_CALLER_ACTION = "VIEW_ONLY",
                PARAM_DEPT_CODE = "ACC",
                PARAM_REF_NO = "000004",
                PARAM_DOC_NO = "123",
                PARAM_DOC_DATE = "20240513",
                PARAM_DESCRIPTION = "DEPOSIT",
                PARAM_GLACCOUNT_NO = "0A111",
                PARAM_CENTER_CODE = "",
                PARAM_CASH_FLOW_GROUP_CODE = "1",
                PARAM_CASH_FLOW_CODE = "0A112",
                PARAM_AMOUNT = 1000000
            };
            eventArgs.TargetPageType = typeof(CBT00220);
        }

        private void R_After_Testing_Detail()
        {

        }
    }
}
