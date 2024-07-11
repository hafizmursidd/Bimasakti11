using BlazorClientHelper;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using PMT01700COMMON.DTO._1._Other_Unit_List;
using PMT01700COMMON.DTO.Utilities;
using PMT01700FrontResources;
using PMT01700MODEL.ViewModel;
using R_BlazorFrontEnd.Controls;
using R_BlazorFrontEnd.Controls.Base;
using R_BlazorFrontEnd.Controls.DataControls;
using R_BlazorFrontEnd.Controls.Enums;
using R_BlazorFrontEnd.Controls.Events;
using R_BlazorFrontEnd.Controls.Forms;
using R_BlazorFrontEnd.Controls.Helpers;
using R_BlazorFrontEnd.Controls.Interfaces;
using R_BlazorFrontEnd.Controls.Menu;
using R_BlazorFrontEnd.Controls.MessageBox;
using R_BlazorFrontEnd.Controls.Popup;
using R_BlazorFrontEnd.Controls.Tab;
using R_BlazorFrontEnd.Exceptions;
using R_BlazorFrontEnd.Helpers;
using R_BlazorFrontEnd.Interfaces;
using System.Text.Json;

namespace PMT01700FRONT
{
    public partial class PMT01700 : R_Page
    {
        #region Master Page 

        readonly PMT01700UnitListViewModel _viewModel = new();
        R_ConductorGrid? _conductorPMT01700otherUnit;
        R_ConductorGrid? _conductorPMT01700SelectedOtherUnit;
        R_Grid<PMT01700OtherUnitList_OtherUnitListDTO>? _gridRefPMT01700OtherUnit;
        R_Grid<PMT01700OtherUnitList_OtherSelectedUnitDTO>? _gridRefPMT01700SelectedOtherUnit;

        [Inject] IJSRuntime? JS { get; set; }
        [Inject] IClientHelper? _clientHelper { get; set; }
        #endregion

        protected override async Task R_Init_From_Master(object poParameter)
        {
            var loEx = new R_Exception();

            try
            {
                _viewModel.lControlData = false;
                _viewModel.oProperty_oDataOtherUnit.ODataOtherUnitList = null;
                await _viewModel.GetPropertyList();
                if (!string.IsNullOrEmpty(_viewModel.oProperty_oDataOtherUnit.CPROPERTY_ID))
                {
                    _viewModel.lControlData = true;
                    await _gridRefPMT01700OtherUnit!.R_RefreshGrid(null);

                }

            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }
            loEx.ThrowExceptionIfErrors();
        }
        #region Button Control
        private async Task NewOfferFunctionAsync()
        {
            var loException = new R_Exception();
            //APM00500ProductDetailDTO? poEntityAPM00500Detail = (APM00500ProductDetailDTO)_conductorAPM00500ProductDetail.R_GetCurrentData();
            List<PMT01700OtherUnitList_OtherUnitListDTO> loData;

            try
            {
                var loOtherDataUnitList = _viewModel.loOtherUnitList;

                //loData = loDataUnitList
                //    .Where(x => x.LSELECTED_UNIT == true)
                //    .ToList();

                //if (!loData.Any())
                //{
                //    var loValidate = await R_MessageBox.Show("", _localizer["ValidationNoRecord"], R_eMessageBoxButtonType.OK);
                //    goto EndBlock;
                //}

                //loData.ForEach(x => x.LSELECTED_UNIT = false);
                //_viewModel.oParameterForGetUnitList.ODataUnitList = JsonSerializer.Serialize(loData);
                //await _tabStripRef?.SetActiveTabAsync("LOO")!;
            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }
            loException.ThrowExceptionIfErrors();
        }
        #endregion

        #region Front Control Property

        private async Task PropertyDropdown_OnChange(string poParam)
        {
            var loEx = new R_Exception();

            try
            {
                _viewModel.oProperty_oDataOtherUnit.CPROPERTY_ID = _viewModel.oProperty_oDataOtherUnit.CPROPERTY_ID = poParam;
                await _gridRefPMT01700OtherUnit.R_RefreshGrid(null);

                if (_tabStripRef.ActiveTab.Id == "LOO")
                {
                    await _tabLOO.InvokeRefreshTabPageAsync(_viewModel.oProperty_oDataOtherUnit);
                }
               else if (_tabStripRef.ActiveTab.Id == "LOC")
                {
                    await _tabLOC.InvokeRefreshTabPageAsync(_viewModel.oProperty_oDataOtherUnit);
                }


                /*

                switch (_tabStripRef.ActiveTab.Id)
                {
                    case "Agreement":
                        await _tabAgreementRef.InvokeRefreshTabPageAsync(null);
                        break;

                    case "UnitInfo":
                        await _tabUnitInfoRef.InvokeRefreshTabPageAsync(null);
                        break;

                    case "ChargesInfo":
                        await _tabChargesInfoRef.InvokeRefreshTabPageAsync(null);
                        break;

                    case "InvoicePlan":
                        await _tabInvoicePlanRef.InvokeRefreshTabPageAsync(null);
                        break;

                    case "Deposit":
                        await _tabDepositRef.InvokeRefreshTabPageAsync(null);
                        break;

                    case "Document":
                        await _tabDocumentRef.InvokeRefreshTabPageAsync(null);
                        break;

                    default:
                        break;
                }
                */

            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            R_DisplayException(loEx);
        }


        #endregion

        private R_TabStrip? _tabStripRef;
        private R_TabPage? _tabLOO;
        private R_TabPage? _tabLOC;

        #region Tab Unit List
        private async Task OnActiveTabIndexChanged(R_TabStripTab eventArgs)
        {
            switch (eventArgs.Id)
            {
                case "UnitList":
                    // await _gridRefPMT01100Building.R_RefreshGrid(null);
                    break;
                case "LOO":

                    break;
                default:
                    break;
            }

        }
        private void OnActiveTabIndexChanging(R_TabStripActiveTabIndexChangingEventArgs eventArgs)
        {
            /*
            _view._lComboBoxProperty = true;
            eventArgs.Cancel = _pageAgreementListOnCRUDmode;
            */

            switch (eventArgs.TabStripTab.Id)
            {
                case "UnitList":
                    _viewModel.lControlData = true;
                    break;
                case "LOO":
                    _viewModel.lControlData = true;
                    break;
                case "LOC":
                    _viewModel.lControlData = true;
                    break;
                default:
                    break;
            }

        }

        #endregion


        private async Task BlankFunction()
        {
            var loException = new R_Exception();
            //APM00500ProductDetailDTO? poEntityAPM00500Detail = (APM00500ProductDetailDTO)_conductorAPM00500ProductDetail.R_GetCurrentData();

            try
            {
                //   var llTrue = await R_MessageBox.Show("", "This function still on Development Process!", R_eMessageBoxButtonType.OK);
            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }

            loException.ThrowExceptionIfErrors();
        }

        #region List Other unit list


        private async Task R_ServiceGetListOtherUnitRecord(R_ServiceGetListRecordEventArgs eventArgs)
        {
            var loEx = new R_Exception();

            try
            {
                await _viewModel.GetOtherUnitList();
                if (!_viewModel.loOtherUnitList.Any())
                {
                    _viewModel.loOtherUnitList.Clear();
                }
                eventArgs.ListEntityResult = _viewModel.loOtherUnitList;
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            loEx.ThrowExceptionIfErrors();
        }

        private void R_DisplayOtherUnitGetRecord(R_DisplayEventArgs eventArgs)
        {
            var loEx = new R_Exception();
            PMT01700OtherUnitList_OtherUnitListDTO loData = (PMT01700OtherUnitList_OtherUnitListDTO)eventArgs.Data;

            try
            {
                //ON DEVELOPEMENT ON RND

                if (!string.IsNullOrEmpty(loData.CBUILDING_ID))
                {

                    _viewModel.oProperty_oDataOtherUnit.CBUILDING_ID = loData.CBUILDING_ID;
                    _viewModel.oProperty_oDataOtherUnit.CBUILDING_NAME = loData.CBUILDING_NAME;
                    _viewModel.oProperty_oDataOtherUnit.COTHER_UNIT_ID = loData.COTHER_UNIT_ID;
                    //await _gridRefPMT01100Unit.R_RefreshGrid(null);
                }
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            loEx.ThrowExceptionIfErrors();
        }


        #region List Selected Unit
        private void R_ServiceGetListSelectedUnitRecord(R_ServiceGetListRecordEventArgs eventArgs)
        {
            var loEx = new R_Exception();

            try
            {
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            loEx.ThrowExceptionIfErrors();
        }

        /*
        private void R_DisplaySelectedUnitGetRecord(R_DisplayEventArgs eventArgs)
        {
            var loEx = new R_Exception();
            //PMT01500AgreementListOriginalDTO loData = (PMT01500AgreementListOriginalDTO)eventArgs.Data;

            try
            {
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            loEx.ThrowExceptionIfErrors();
        }
        */
        #endregion

        #endregion
        #region Utilities Master Tab

        private async Task R_TabEventCallbackAsync(object poValue)
        {
            var loEx = new R_Exception();

            try
            {
                var loValue = R_FrontUtility.ConvertObjectToObject<PMT01700EventCallBackDTO>(poValue);
                _viewModel.lControlData = loValue.LUSING_PROPERTY_ID;
                _viewModel.lControlTabLOC = _viewModel.lControlTabLOO = _viewModel.lControlTabUnitList = loValue.LCRUD_MODE;
                if (!string.IsNullOrEmpty(loValue.CCRUD_MODE))
                {
                    switch (loValue.CCRUD_MODE)
                    {
                        case "A_ADD":
                            _viewModel.oProperty_oDataOtherUnit.ODataOtherUnitList = null;
                            break;

                        case "A_DELETE":
                            _viewModel.oProperty_oDataOtherUnit.ODataOtherUnitList = null;
                            break;

                        case "A_CANCEL":
                            _viewModel.oProperty_oDataOtherUnit.ODataOtherUnitList = null;
                            break;
                        default:
                            _viewModel.oProperty_oDataOtherUnit.ODataOtherUnitList = null;
                            break;
                    }
                }
                //await InvokeTabEventCallbackAsync(null);
                /*
                switch (loValue.CCRUD_MODE)
                {
                    case 'A':
                        break;
                    default:
                        break;
                }
                */
                //INI MASALAH PASTI
                await Task.Delay(100);

            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }
            loEx.ThrowExceptionIfErrors();

        }

        #endregion


        #region Tab LOO

        private void General_Before_Open_LOO_TabPage(R_BeforeOpenTabPageEventArgs eventArgs)
        {
            eventArgs.Parameter = _viewModel.oProperty_oDataOtherUnit;
            eventArgs.TargetPageType = typeof(PMT01700LOO_OfferList);
        }

        #endregion


        #region Tab LOC

        private void General_Before_Open_LOC_TabPage(R_BeforeOpenTabPageEventArgs eventArgs)
        {
            eventArgs.Parameter = _viewModel.oProperty_oDataOtherUnit;
            eventArgs.TargetPageType = typeof(PMT01700LOC_LOCList);
        }

        #endregion




    }
}
