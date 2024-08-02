using BlazorClientHelper;
using Microsoft.AspNetCore.Components;
using PMB04000COMMON.DTO.DTOs;
using PMB04000COMMON.DTO.Utilities;
using PMB04000COMMON.Print.Param_DTO;
using PMB04000MODEL.ViewModel;
using R_BlazorFrontEnd.Controls;
using R_BlazorFrontEnd.Controls.DataControls;
using R_BlazorFrontEnd.Controls.Events;
using R_BlazorFrontEnd.Controls.MessageBox;
using R_BlazorFrontEnd.Controls.Tab;
using R_BlazorFrontEnd.Exceptions;
using R_BlazorFrontEnd.Helpers;
using R_BlazorFrontEnd.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMB04000FRONT
{
    public partial class PMB04000Receipt : R_Page, R_ITabPage
    {
        readonly PMB04000ViewModel _viewModel = new();
        private R_ConductorGrid? _conductorRef;
        private R_Grid<PMB04000DTO>? _grid;
        [Inject] IClientHelper? _clientHelper { get; set; }
        [Inject] private R_IReport? _reportService { get; set; }
        protected override async Task R_Init_From_Master(object poParameter)
        {
            var loEx = new R_Exception();
            try
            {
                _viewModel.oParameterInvoiceReceipt = R_FrontUtility.ConvertObjectToObject<PMB04000ParamDTO>(poParameter);
                _viewModel.oParameterInvoiceReceipt.CTRANS_CODE = "940010";
                //  await _grid!.R_RefreshGrid(null)!;
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }
            loEx.ThrowExceptionIfErrors();
        }

        #region Master Tab
        private async Task R_ServiceGetList(R_ServiceGetListRecordEventArgs eventArgs)
        {
            var loEx = new R_Exception();

            try
            {
                await _viewModel.GetOfficialReceiptList();
                if (!_viewModel.loOfficialReceipt.Any())
                {
                    _viewModel.loOfficialReceipt.Clear();
                }
                eventArgs.ListEntityResult = _viewModel.loOfficialReceipt;
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            loEx.ThrowExceptionIfErrors();
        }
        private void R_Display(R_DisplayEventArgs eventArgs)
        {
            var loEx = new R_Exception();
            //PMT01700OtherUnitList_OtherUnitListDTO loData = (PMT01700OtherUnitList_OtherUnitListDTO)eventArgs.Data;

            //try
            //{
            //    //ON DEVELOPEMENT ON RND

            //    if (!string.IsNullOrEmpty(loData.CBUILDING_ID))
            //    {

            //        _viewModel.oProperty_oDataOtherUnit.CBUILDING_ID = loData.CBUILDING_ID;
            //        _viewModel.oProperty_oDataOtherUnit.CBUILDING_NAME = loData.CBUILDING_NAME;
            //        _viewModel.oProperty_oDataOtherUnit.COTHER_UNIT_ID = loData.COTHER_UNIT_ID;
            //        //await _gridRefPMT01100Unit.R_RefreshGrid(null);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    loEx.Add(ex);
            //}

            loEx.ThrowExceptionIfErrors();
        }
        #endregion
        #region Button
        private async Task BtnCancel()
        {
            var loEx = new R_Exception();
            try
            {
                _viewModel.pcTYPE_PROCESS = "CANCEL_RECEIPT";
                await _conductorRef.R_SaveBatch();
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }
            R_DisplayException(loEx);
        }
        private async Task BtnPrint()
        {
            var loEx = new R_Exception();
            try
            {
                _viewModel.pcTYPE_PROCESS = "PRINT";
                await _conductorRef.R_SaveBatch();
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }
            R_DisplayException(loEx);
        }

        public async Task RefreshTabPageAsync(object poParam)
        {
            R_Exception loException = new R_Exception();
            try
            {
                _viewModel.oParameterInvoiceReceipt = R_FrontUtility.ConvertObjectToObject<PMB04000ParamDTO>(poParam);
                _viewModel.oParameterInvoiceReceipt.CTRANS_CODE = "940010";
                await _grid!.R_RefreshGrid(null);
            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }
            R_DisplayException(loException);
        }
        #endregion
        #region Save Batch
        private async Task ServiceSaveBatch(R_ServiceSaveBatchEventArgs eventArgs)
        {
            var loEx = new R_Exception();
            try
            {
                var loList = (List<PMB04000DTO>)eventArgs.Data;

            //    List<PMB04000DTO> poDataSelected = _viewModel.ValidationProcessData(loList);

                if (_viewModel.pcTYPE_PROCESS == "CANCEL_RECEIPT")
                {
                    if (await R_MessageBox.Show("Confirmation",
                        $"Are you sure want to cancel receipt selected Data?",
                         R_eMessageBoxButtonType.YesNo) == R_eMessageBoxResult.Yes)
                    {
                        var loParam = new PMB04000ParamDTO
                        {
                            CCOMPANY_ID = _clientHelper!.CompanyId,
                            CUSER_ID = _clientHelper!.UserId,
                            CTYPE_PROCESS = _viewModel.pcTYPE_PROCESS
                        };
                       // await _viewModel.ProcessDataSelected(poParam: loParam, poListData: poDataSelected);
                        //CLEAR OLD DATA
                        _grid!.DataSource.Clear();
                        //_viewModel.BankInChequeInfo = new();
                    }
                }
                else if (_viewModel.pcTYPE_PROCESS == "PRINT")
                {
                   // var lcRefNoData = string.Join(",", poDataSelected.Where(x => x.LSELECTED).Select(x => x.CREF_NO));
                    var loParam = new PMB04000ParamReportDTO
                    {
                        CCOMPANY_ID = _clientHelper!.CompanyId,
                        CPROPERTY_ID = _viewModel.oParameterInvoiceReceipt.CPROPERTY_ID,
                        CDEPT_CODE = _viewModel.oParameterInvoiceReceipt.CDEPT_CODE,
                        CREF_NO = "",
                        CUSER_ID = _clientHelper!.UserId,
                        CLANG_ID =_clientHelper.ReportCulture,
                        LPRINT = true
                    };
                    await _reportService!.GetReport(
                                "R_DefaultServiceUrlPM",
                                "PM",
                                "rpt/PMB04000Report/ReceiptReportPost",
                                "rpt/PMB04000Report/ReceiptReportGet",
                                loParam);
                }
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }
            loEx.ThrowExceptionIfErrors();
        }
        private async Task AfterSaveBatch(R_AfterSaveBatchEventArgs eventArgs)
        {
            var loEx = new R_Exception();
            try
            {
                //GET LIST DATA
                await _grid!.R_RefreshGrid(null);
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
