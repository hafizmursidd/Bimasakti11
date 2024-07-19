using BlazorClientHelper;
using Microsoft.AspNetCore.Components;
using R_BlazorFrontEnd.Controls.DataControls;
using R_BlazorFrontEnd.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PMT01700MODEL.ViewModel;
using PMT01700COMMON.DTO._2._LOO._3._LOO___Unit___Charges.LOO___Unit___Charges___Charges;
using R_BlazorFrontEnd.Controls.Events;
using R_BlazorFrontEnd.Enums;
using R_BlazorFrontEnd.Exceptions;
using R_BlazorFrontEnd.Helpers;
using System.Reflection.Emit;
using PMT01700COMMON.DTO.Utilities.Front;
using PMT01700COMMON.DTO.Utilities.ParamDb.LOO;
using PMT01700COMMON.DTO._2._LOO._3._LOO___Unit___Charges.LOO___Unit___Charges___Unit___Charges;
using System.Xml.Linq;
using R_BlazorFrontEnd.Controls.Popup;
using Lookup_GSCOMMON.DTOs;
using Lookup_GSFRONT;
using Lookup_GSModel.ViewModel;
using PMT01700COMMON.DTO._2._LOO._2._LOO___Offer;
using Lookup_PMCOMMON.DTOs;
using Lookup_PMFRONT;
using Lookup_PMModel.ViewModel.LML00200;
using R_CommonFrontBackAPI;
using R_BlazorFrontEnd.Controls.MessageBox;
using System.Collections.ObjectModel;
using System.Globalization;
using PMT01700FrontResources;
using PMT01700COMMON.DTO.Utilities;

namespace PMT01700FRONT
{
    public partial class PMT01700LOO_UnitCharges_Charges
    {
        private PMT01700LOO_UnitCharges_ChargesViewModel _viewModel = new();

        private R_Conductor? _conductorCharges;
        private R_Grid<PMT01700LOO_UnitCharges_ChargesListDTO>? _gridCharges;

        private R_ConductorGrid? _conductorDetailItem;
        //for detail item
        private R_Grid<PMT01700LOO_UnitCharges_Charges_ChargesItemDTO>? _gridItemCharges;
        public bool _lControlCRUD;
        private bool _lControlChargesItem;
        private bool _lControlButton = true;
        PMT01700EventCallBackDTO _oEventCallBack = new PMT01700EventCallBackDTO();

        [Inject] IClientHelper? _clientHelper { get; set; }

        protected override async Task R_Init_From_Master(object poParameter)
        {
            var loEx = new R_Exception();
            try
            {
                _viewModel.oParameterChargeTab = R_FrontUtility.ConvertObjectToObject<PMT01700ParameterChargesTab>(poParameter);

                await _viewModel.GetFeeMethodandPeriod();

                if (!string.IsNullOrEmpty(_viewModel.oParameterChargeTab.CPROPERTY_ID))
                {
                    await _gridCharges.R_RefreshGrid(null);

                    //if (!string.IsNullOrEmpty(_viewModel.oParameter.CREF_NO))
                    //{
                    //    await _gridDetailItem!.R_RefreshGrid(null);
                    //}
                }
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            R_DisplayException(loEx);
        }

        #region CHARGES
        bool _hasDataUnit = false;
        private async Task GetListCharges(R_ServiceGetListRecordEventArgs eventArgs)
        {
            var loEx = new R_Exception();
            try
            {
                await _viewModel.GetChargesList();
                eventArgs.ListEntityResult = _viewModel.oListCharges;
                _lControlButton = _viewModel.oListCharges.Any();

                if (_viewModel.oListCharges.Any())
                {
                    _lControlButton = _hasDataUnit = true;
                }
                else
                {
                    _hasDataUnit = false;
                }

            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            loEx.ThrowExceptionIfErrors();
        }
        private async Task ServiceGetRecordCharges(R_ServiceGetRecordEventArgs eventArgs)
        {
            var loEx = new R_Exception();

            try
            {
                var loParam = R_FrontUtility.ConvertObjectToObject<PMT01700LOO_UnitCharges_ChargesDetailDTO>(eventArgs.Data);
                loParam.CDEPT_CODE = _viewModel.oParameterChargeTab.CDEPT_CODE;
                loParam.CTRANS_CODE = _viewModel.oParameterChargeTab.CTRANS_CODE;
                // var temp = _viewModel.oParameterChargeTab;
                await _viewModel.GetEntityCharges(loParam);

                eventArgs.Result = _viewModel.oEntityCharges;

            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            loEx.ThrowExceptionIfErrors();
        }
        private void ServiceAfterAdd(R_AfterAddEventArgs eventArgs)
        {
            var loEx = new R_Exception();
            var loData = (PMT01700LOO_UnitCharges_ChargesDetailDTO)eventArgs.Data;
            try
            {
                loData.DSTART_DATE = _viewModel.ConvertStringToDateTimeFormat(_viewModel.oParameterChargeTab.CSTART_DATE);
                loData.DEND_DATE = _viewModel.ConvertStringToDateTimeFormat(_viewModel.oParameterChargeTab.CEND_DATE);
                loData.IYEAR = _viewModel.oParameterChargeTab.IYEARS;
                loData.IMONTHS = _viewModel.oParameterChargeTab.IMONTHS;
                loData.IDAYS = _viewModel.oParameterChargeTab.IDAYS;
                //loData.LACTIVE = false;
                loData.CBILLING_MODE = _viewModel.loRadioGroupDataCBILLING_MODE.First().CCODE;
                loData.CFEE_METHOD = _viewModel.oComboBoxFeeMethod!.First().CCODE;
                loData.CINVOICE_PERIOD = _viewModel.oComboBoxPeriodMode!.First().CCODE;
                _viewModel.loTempListChargesItem = _viewModel.oListChargesItem;
                _viewModel.oListChargesItem = new ObservableCollection<PMT01700LOO_UnitCharges_Charges_ChargesItemDTO>();
                _lControlCRUD = true;

                _gridItemCharges!.R_RefreshGrid(null);

            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            loEx.ThrowExceptionIfErrors();
        }
        private async Task ServiceR_Display(R_DisplayEventArgs eventArgs)
        {
            var loException = new R_Exception();
            var loData = (PMT01700LOO_UnitCharges_ChargesDetailDTO)eventArgs.Data;

            try
            {
                if (loData != null)
                {
                    if (!string.IsNullOrEmpty(loData.CSEQ_NO))
                    {
                        var loTempParam = R_FrontUtility.ConvertObjectToObject<PMT01700LOO_UnitUtilities_ParameterDTO>(loData);
                        if (loData.LCAL_UNIT)
                        {
                            await _gridItemCharges!.R_RefreshGrid(loTempParam);
                        }
                        else
                        {
                            _viewModel.oListChargesItem = new();
                        }
                        // var loTempParam = _viewModel.oParameterGetItemCharges;
                        //_viewModel.oParameterChargesList.CTRANS_CODE = _viewModel.oParameterUtilitiesPage.CTRANS_CODE = _viewModel.oParameter.CTRANS_CODE;
                        //_viewModel.oParameterChargesList.CDEPT_CODE = _viewModel.oParameterUtilitiesPage.CDEPT_CODE = _viewModel.oParameter.CDEPT_CODE;
                    }

                }
            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }

            loException.ThrowExceptionIfErrors();
        }
        private async Task ServiceSaveCharges(R_ServiceSaveEventArgs eventArgs)
        {
            var loEx = new R_Exception();
            try
            {
                var loParam = (PMT01700LOO_UnitCharges_ChargesDetailDTO)eventArgs.Data;
                if (!loParam.LCAL_UNIT)
                {
                    loParam.NTOTAL_AMT = 0;
                }

                await _viewModel.ServiceSaveCharges(loParam, (eCRUDMode)eventArgs.ConductorMode);
                eventArgs.Result = _viewModel.oEntityCharges;
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            loEx.ThrowExceptionIfErrors();
        }
        public async Task ServiceDelete(R_ServiceDeleteEventArgs eventArgs)
        {
            var loEx = new R_Exception();

            try
            {
                var loParam = (PMT01700LOO_UnitCharges_ChargesDetailDTO)eventArgs.Data;
                await _viewModel.ServiceDeleteCharges(loParam);
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            loEx.ThrowExceptionIfErrors();
        }
        public async Task Validation(R_ValidationEventArgs eventArgs)
        {
            var loEx = new R_Exception();
            //PMT01700LOO_UnitCharges_ChargesDetailDTO? loData = null;
            try
            {
                var loData = (PMT01700LOO_UnitCharges_ChargesDetailDTO)eventArgs.Data;

                if (_lControlChargesItem)
                {
                    var loErr = R_FrontUtility.R_GetError(typeof(Resources_PMT01700_Class), "ValidationChargesItem");
                    loEx.Add(loErr);
                    goto EndBlock;
                }

                if (eventArgs.ConductorMode == R_eConductorMode.Edit)
                {
                    if (_viewModel.oListChargesItem.Any() && !loData.LCAL_UNIT)
                    {
                        var llFalse = await R_MessageBox.Show("", "All Fee Calculation Data will be deleted, Are you sure!",
                R_eMessageBoxButtonType.OKCancel);
                        switch (llFalse)
                        {
                            case R_eMessageBoxResult.Cancel:
                                eventArgs.Cancel = true;
                                break;
                            case R_eMessageBoxResult.OK:
                                _viewModel.Data.ChargeItemList = null;
                                //dibuat null, soalnya kalo NFEE nya pertama mati, bakal keluar ini lagi
                                break;
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(loData.CCHARGES_ID))
                {
                    var loErr = R_FrontUtility.R_GetError(typeof(Resources_PMT01700_Class), "ValidationCharge");
                    loEx.Add(loErr);
                }
                if (loData.IYEAR <= 0 && loData.IMONTHS <= 0 && loData.IDAYS <= 0)
                {
                    var loErr = R_FrontUtility.R_GetError(typeof(Resources_PMT01700_Class), "ValidationTenure");
                    loEx.Add(loErr);
                }

                if (!loData.LCAL_UNIT)
                {
                    if (loData.NFEE_AMT <= 0)
                    {
                        var loErr = R_FrontUtility.R_GetError(typeof(Resources_PMT01700_Class), "ValidationFeeAmount");
                        loEx.Add(loErr);
                    }
                }
                else
                {
                    if (!_viewModel.oListChargesItem.Any())
                    {
                        var loErr = R_FrontUtility.R_GetError(typeof(Resources_PMT01700_Class), "ValidationFeeCalculation");
                        loEx.Add(loErr);
                    }
                }
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }
        EndBlock:
            eventArgs.Cancel = _lControlCRUD = loEx.HasError;
            loEx.ThrowExceptionIfErrors();
        }
        public async Task AfterDelete()
        {
            _hasDataUnit = _lControlCRUD = _lControlButton = _viewModel.oListCharges.Any();
            await R_MessageBox.Show("", "Delete Success", R_eMessageBoxButtonType.OK);
        }
        private async Task SetOther(R_SetEventArgs eventArgs)
        {
            R_Exception loException = new R_Exception();

            try
            {
                //var x = _lControlCRUD;
                //var y = _viewModel.Data.LCAL_UNIT;

                _hasDataUnit = _viewModel.oListCharges.Any();
                _lControlCRUD = !eventArgs.Enable;
                _oEventCallBack.LCRUD_MODE = eventArgs.Enable;
                await InvokeTabEventCallbackAsync(_oEventCallBack);
            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }

            R_DisplayException(loException);
        }
        public async void BeforeCancel(R_BeforeCancelEventArgs eventArgs)
        {
            R_Exception loEx = new R_Exception();
            try
            {
                var res = await R_MessageBox.Show("", @_localizer["ValidationBeforeCancel"], R_eMessageBoxButtonType.YesNo);
                if (res == R_eMessageBoxResult.No)
                {
                    eventArgs.Cancel = true;
                }
                else
                {
                    await Close(false, false);
                    switch (eventArgs.ConductorMode)
                    {
                        case R_eConductorMode.Edit:
                            if (_viewModel.Data.LCAL_UNIT)
                            {
                                await _gridItemCharges.R_RefreshGrid(null);
                            }
                            break;
                        case R_eConductorMode.Add:
                            _viewModel.oListChargesItem = _viewModel.loTempListChargesItem.Any() ? _viewModel.loTempListChargesItem : new ObservableCollection<PMT01700LOO_UnitCharges_Charges_ChargesItemDTO>();
                            _viewModel.loTempListChargesItem = new ObservableCollection<PMT01700LOO_UnitCharges_Charges_ChargesItemDTO>();
                            break;
                        default:
                            break;
                    }
                }

            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            R_DisplayException(loEx);
        }

        #endregion


        #region CHARGES_ITEM
        private async Task GetListCharges_Item(R_ServiceGetListRecordEventArgs eventArgs)
        {
            var loEx = new R_Exception();
            try
            {
                var poParameter = (PMT01700LOO_UnitUtilities_ParameterDTO)eventArgs.Parameter;
                await _viewModel.GetChargesItemList(poParameter);
                eventArgs.ListEntityResult = _viewModel.oListChargesItem;
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            loEx.ThrowExceptionIfErrors();
        }
        private void R_ValidationItems(R_ValidationEventArgs eventArgs)
        {
            var loEx = new R_Exception();
            try
            {
                var data = (PMT01700LOO_UnitCharges_Charges_ChargesItemDTO)eventArgs.Data;

                //if (_viewModel.Data.LCAL_UNIT)
                //{
                //    if (string.IsNullOrWhiteSpace(data.CUNIT_ID))
                //    {
                //        loEx.Add("", "Unit ID is required!");
                //    }
                //}

            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            eventArgs.Cancel = loEx.HasError;
            loEx.ThrowExceptionIfErrors();
        }
        private void ServiceGetRecordCharges_Item(R_ServiceGetRecordEventArgs eventArgs)
        {
            var loEx = new R_Exception();

            try
            {
                var loParam = (PMT01700LOO_UnitCharges_Charges_ChargesItemDTO)(eventArgs.Data);
                _viewModel.GetChargesItem(loParam);
                eventArgs.Result = _viewModel.oEntityChargesItem;
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            loEx.ThrowExceptionIfErrors();
        }
        private void R_DisplayAsync(R_DisplayEventArgs eventArgs)
        {
            var loException = new R_Exception();

            try
            {
                var loData = (PMT01700LOO_UnitCharges_Charges_ChargesItemDTO)eventArgs.Data;
                var PMT01700LOO_UnitCharges_ChargesDetailDTO = (PMT01700LOO_UnitCharges_ChargesDetailDTO)_conductorCharges!.R_GetCurrentData();
                PMT01700LOO_UnitCharges_ChargesDetailDTO.NINVOICE_AMT = _viewModel.NTotalItemDetil;

                // _viewModel.oListChargesItem.Add(_viewModel.oEntityChargesItem);

                var loHeaderData = _viewModel.Data;
                if (eventArgs.ConductorMode == R_eConductorMode.Normal)
                {
                    if (_gridItemCharges!.DataSource.Count > 0)
                    {
                        loHeaderData.NINVOICE_AMT = ((_gridItemCharges.DataSource.Sum(x => x.NTOTAL_PRICE)));
                        //*  (_gridItemCharges.DataSource.Sum(x => x.)));
                    }
                }

            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }

            //EndBlocks:
            loException.ThrowExceptionIfErrors();
        }
        private void R_ServiceAfterSave()
        {
            var loEx = new R_Exception();

            try
            {
                PMT01700LOO_UnitCharges_ChargesDetailDTO PMT01700LOO_UnitCharges_ChargesDetailDTO = (PMT01700LOO_UnitCharges_ChargesDetailDTO)_conductorCharges!.R_GetCurrentData();
                // PMT01700LOO_UnitCharges_ChargesDetailDTO.NINVOICE_AMT = _viewModel.NTotalItemDetil;

                // _viewModel.oListChargesItem.Add(_viewModel.oEntityChargesItem);

            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            loEx.ThrowExceptionIfErrors();
        }
        private void SetOtherChargesItems(R_SetEventArgs eventArgs)
        {
            R_Exception loException = new R_Exception();
            try
            {
                _lControlChargesItem = !eventArgs.Enable;
            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }
            R_DisplayException(loException);
        }

        private void R_CellLostFocused(R_CellValueChangedEventArgs eventArgs)
        {
            var loException = new R_Exception();

            try
            {

                var lnTotalPrice = (PMT01700LOO_UnitCharges_Charges_ChargesItemDTO)eventArgs.CurrentRow;
                //.FirstOrDefault(x => x.Name == "Total Price");
                var liQuantity = (PMT01700LOO_UnitCharges_Charges_ChargesItemDTO)eventArgs.CurrentRow;
                //.FirstOrDefault(x => x.Name == "Qty");
                var lnUnitPrice = (PMT01700LOO_UnitCharges_Charges_ChargesItemDTO)eventArgs.CurrentRow;
                //.FirstOrDefault(x => x.Name == "Unit Price");
                var lnDiscount = (PMT01700LOO_UnitCharges_Charges_ChargesItemDTO)eventArgs.CurrentRow;
                //.FirstOrDefault(x => x.Name == "Discount");

                if (eventArgs.ColumnName != "CITEM_NAME")
                {

                    double qty = Convert.ToDouble(liQuantity.IQTY);
                    double unitPrice = Convert.ToDouble(lnUnitPrice.NUNIT_PRICE);
                    double discount = Convert.ToDouble(lnDiscount.NDISCOUNT) / 100;

                    var loData = (PMT01700LOO_UnitCharges_Charges_ChargesItemDTO)_conductorDetailItem!.R_GetCurrentData();
                    // Menghitung Harga Total dengan Diskon
                    lnTotalPrice.NTOTAL_PRICE = (decimal)(qty * unitPrice * (1 - discount));
                    _viewModel.NTotalItemDetil = (decimal)(qty * unitPrice * (1 - discount));
                }

            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }

            R_DisplayException(loException);
        }
        #endregion

        #region Lookup Button Charges Lookup
        private R_Lookup _BtnLookupCharges;

        private void BeforeOpenLookUpChargesLookup(R_BeforeOpenLookupEventArgs eventArgs)
        {
            LML00200ParameterDTO? param = null;
            if (!string.IsNullOrEmpty(_viewModel.oParameterChargeTab.CPROPERTY_ID))
            {
                param = new LML00200ParameterDTO
                {
                    CCOMPANY_ID = _clientHelper.CompanyId,
                    CUSER_ID = _clientHelper.UserId,
                    CPROPERTY_ID = _viewModel.oParameterChargeTab.CPROPERTY_ID,
                    CCHARGE_TYPE_ID = "01,02,05"
                };
            }
            eventArgs.Parameter = param;
            eventArgs.TargetPageType = typeof(LML00200);
        }

        private void AfterOpenLookUpChargesLookup(R_AfterOpenLookupEventArgs eventArgs)
        {
            R_Exception loException = new R_Exception();
            LML00200DTO? loTempResult = null;
            //PMT01100LOO_Offer_SelectedOfferDTO? loGetData = null;

            try
            {
                loTempResult = (LML00200DTO)eventArgs.Result;
                if (loTempResult == null)
                    return;
                _viewModel.Data.CCHARGES_ID = loTempResult.CCHARGES_ID;
                _viewModel.Data.CCHARGES_NAME = loTempResult.CCHARGES_NAME;
                _viewModel.Data.CCHARGES_TYPE = loTempResult.CCHARGES_TYPE;
                _viewModel.Data.CCHARGES_TYPE_DESCR = loTempResult.CCHARGES_TYPE_DESCR;
            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }

            R_DisplayException(loException);

        }

        private async Task OnLostFocusCharges()
        {
            R_Exception loEx = new();

            try
            {
                PMT01700LOO_UnitCharges_ChargesDetailDTO loGetData = _viewModel.Data;

                if (string.IsNullOrWhiteSpace(_viewModel.Data.CCHARGES_ID))
                {
                    loGetData.CCHARGES_ID = "";
                    return;
                }

                LookupLML00200ViewModel loLookupViewModel = new LookupLML00200ViewModel();
                LML00200ParameterDTO loParam = new LML00200ParameterDTO()
                {
                    CCOMPANY_ID = _clientHelper.CompanyId,
                    CUSER_ID = _clientHelper.UserId,
                    CPROPERTY_ID = _viewModel.oParameterChargeTab.CPROPERTY_ID!,
                    CCHARGE_TYPE_ID = "01,02,05",
                    CSEARCH_TEXT = loGetData.CCHARGES_ID ?? "",
                };

                var loResult = await loLookupViewModel.GetUnitCharges(loParam);

                if (loResult == null)
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                            typeof(Lookup_PMFrontResources.Resources_Dummy_Class_LookupPM),
                            "_ErrLookup01"));
                    loGetData.CCHARGES_ID = "";
                    loGetData.CCHARGES_NAME = "";
                    loGetData.CCHARGES_TYPE = "";
                    loGetData.CCHARGES_TYPE_DESCR = "";
                    //await GLAccount_TextBox.FocusAsync();
                }
                else
                {
                    loGetData.CCHARGES_ID = loResult.CCHARGES_ID;
                    loGetData.CCHARGES_NAME = loResult.CCHARGES_NAME;
                    loGetData.CCHARGES_TYPE = loResult.CCHARGES_TYPE;
                    loGetData.CCHARGES_TYPE_DESCR = loResult.CCHARGES_TYPE_DESCR;
                }
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            R_DisplayException(loEx);
        }
        #endregion

        #region Currency
        private R_Lookup R_LookupCurrencyLookup;
        private void BeforeOpenLookUpCurrencyLookup(R_BeforeOpenLookupEventArgs eventArgs)
        {
            GSL00300ParameterDTO? param = null;
            if (!string.IsNullOrEmpty(_viewModel.oParameterChargeTab.CPROPERTY_ID))
            {
                param = new GSL00300ParameterDTO
                {
                    CCOMPANY_ID = _clientHelper.CompanyId,
                    CUSER_ID = _clientHelper.UserId
                };
            }

            eventArgs.Parameter = param;
            eventArgs.TargetPageType = typeof(GSL00300);
        }

        private void AfterOpenLookUpCurrencyLookup(R_AfterOpenLookupEventArgs eventArgs)
        {
            R_Exception loException = new R_Exception();
            GSL00300DTO? loTempResult = null;
            //PMT02500FrontAgreementDetailDTO? loGetData = null;

            try
            {
                loTempResult = (GSL00300DTO)eventArgs.Result;
                if (loTempResult == null)
                    return;

                //loGetData = (PMT02500FrontAgreementDetailDTO)_conductorFullPMT02500Agreement.R_GetCurrentData();

                _viewModel.Data.CCURRENCY_CODE = loTempResult.CCURRENCY_CODE;
                _viewModel._cCurrencyCode = loTempResult.CCURRENCY_CODE;
            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }

            R_DisplayException(loException);
        }
        private async Task OnLostFocusCurrency()
        {
            R_Exception loEx = new R_Exception();

            try
            {
                var loGetData = (PMT01700LOO_UnitCharges_ChargesDetailDTO)_viewModel.Data;

                if (string.IsNullOrWhiteSpace(_viewModel.Data.CCURRENCY_CODE))
                {
                    loGetData.CCURRENCY_CODE = "";
                    return;
                }

                LookupGSL00300ViewModel loLookupViewModel = new LookupGSL00300ViewModel();
                GSL00300ParameterDTO loParam = new GSL00300ParameterDTO()
                {
                    CCOMPANY_ID = _clientHelper.CompanyId,
                    CUSER_ID = _clientHelper.UserId,
                    CSEARCH_TEXT = loGetData.CCURRENCY_CODE ?? "",
                };

                var loResult = await loLookupViewModel.GetCurrency(loParam);

                if (loResult == null)
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                        typeof(Lookup_GSFrontResources.Resources_Dummy_Class),
                        "_ErrLookup01"));
                    loGetData.CCURRENCY_CODE = "";
                    //await GLAccount_TextBox.FocusAsync();
                }
                else
                {
                    loGetData.CCURRENCY_CODE = loResult.CCURRENCY_CODE;
                    _viewModel._cCurrencyCode = loResult.CCURRENCY_CODE;
                }
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            R_DisplayException(loEx);
        }
        #endregion

        #region External Function

        private void OnChangedLCAL_UNIT(bool plParam)
        {
            _viewModel.Data.LCAL_UNIT = plParam;
            //_viewModel._nTempFEE_AMT = plParam ? _viewModel.Data.NFEE_AMT : _viewModel._nTempFEE_AMT;
            //_viewModel.Data.NFEE_AMT = plParam ? 0 : _viewModel._nTempFEE_AMT;
        }

        private void OnChangedCYEAR(Int32 poParam)
        {
            var loEx = new R_Exception();

            try
            {
                var loData = (PMT01700LOO_UnitCharges_ChargesDetailDTO)_viewModel.Data;
                PMT01700ControlYMD llControl = _viewModel._oControlYMD;
                loData.IYEAR = poParam;

                if (llControl.LYEAR || llControl.LMONTH || llControl.LDAY)
                {
                    if (loData.IYEAR == 0 && loData.IMONTHS == 0 && loData.IDAYS == 0)
                    {
                        loData.DEND_DATE = loData.DSTART_DATE;
                    }
                    else
                    {
                        loData.DEND_DATE = loData.DSTART_DATE!.Value.AddYears(loData.IYEAR).AddMonths(loData.IMONTHS).AddDays(loData.IDAYS).AddDays(-1);
                    }
                }
                else
                {
                    llControl.LYEAR = true;
                    loData.DEND_DATE = loData.DSTART_DATE!.Value.AddYears(loData.IYEAR).AddDays(-1);
                }

            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            R_DisplayException(loEx);
        }

        private void OnChangedCMONTH(Int32 poParam)
        {
            var loEx = new R_Exception();

            try
            {
                var loData = _viewModel.Data;
                var llControl = _viewModel._oControlYMD;
                loData.IMONTHS = poParam;

                if (llControl.LYEAR || llControl.LMONTH || llControl.LDAY)
                {
                    if (loData.IYEAR == 0 && loData.IMONTHS == 0 && loData.IDAYS == 0)
                    {
                        loData.DEND_DATE = loData.DSTART_DATE;
                    }
                    else
                    {
                        loData.DEND_DATE = loData.DSTART_DATE!.Value.AddYears(loData.IYEAR).AddMonths(loData.IMONTHS).AddDays(loData.IDAYS).AddDays(-1);
                    }
                }
                else
                {
                    llControl.LMONTH = true;
                    loData.DEND_DATE = loData.DSTART_DATE!.Value.AddMonths(loData.IMONTHS).AddDays(-1);
                }

            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            R_DisplayException(loEx);
        }

        private void OnChangedCDAY(Int32 poParam)
        {
            var loEx = new R_Exception();

            try
            {
                var loData = _viewModel.Data;
                var llControl = _viewModel._oControlYMD;
                loData.IDAYS = poParam;

                if (llControl.LYEAR || llControl.LMONTH || llControl.LDAY)
                {
                    if (loData.IYEAR == 0 && loData.IMONTHS == 0 && loData.IDAYS == 0)
                    {
                        loData.DEND_DATE = loData.DSTART_DATE;
                    }
                    else
                    {
                        loData.DEND_DATE = loData.DSTART_DATE!.Value.AddYears(loData.IYEAR).AddMonths(loData.IMONTHS).AddDays(loData.IDAYS).AddDays(-1);
                    }
                }
                else
                {
                    llControl.LYEAR = true;
                    loData.DEND_DATE = loData.DSTART_DATE!.Value.AddDays(loData.IDAYS).AddDays(-1);
                }

            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            R_DisplayException(loEx);
        }

        private void OnChangedDSTART_DATE(DateTime? poValue)
        {
            R_Exception loException = new R_Exception();

            try
            {
                PMT01700LOO_UnitCharges_ChargesDetailDTO loData = _viewModel.Data;
                loData.DSTART_DATE = poValue;

                if (loData.DEND_DATE == null)
                {
                    loData.DEND_DATE = loData.IYEAR == 0 && loData.IMONTHS == 0 && loData.IDAYS == 0
                        ? loData.DSTART_DATE
                        : loData.DSTART_DATE!.Value.AddYears(loData.IYEAR).AddMonths(loData.IMONTHS).AddDays(loData.IDAYS).AddDays(-1);
                }
                else
                {
                    CalculateYMD(poStartDate: loData.DSTART_DATE, poEndDate: loData.DEND_DATE, plStart: true);
                }
            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }

            R_DisplayException(loException);
        }

        private void OnChangedDEND_DATE(DateTime? poValue)
        {
            R_Exception loException = new R_Exception();

            try
            {
                PMT01700LOO_UnitCharges_ChargesDetailDTO loData = _viewModel.Data;
                loData.DEND_DATE = poValue;

                if (loData.DSTART_DATE == null)
                {
                    loData.DSTART_DATE = loData.IYEAR == 0 && loData.IMONTHS == 0 && loData.IDAYS == 0
                        ? loData.DEND_DATE
                        : loData.DEND_DATE!.Value.AddYears(loData.IYEAR).AddMonths(loData.IMONTHS).AddDays(loData.IDAYS).AddDays(-1);
                }
                else
                {
                    CalculateYMD(poStartDate: loData.DSTART_DATE, poEndDate: loData.DEND_DATE);
                }
            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }

            R_DisplayException(loException);
        }

        private void CalculateYMD(DateTime? poStartDate, DateTime? poEndDate, bool plStart = false)
        {
            R_Exception loException = new R_Exception();
            PMT01700LOO_UnitCharges_ChargesDetailDTO loData = _viewModel.Data;

            try
            {
                if (poStartDate >= poEndDate)
                {
                    if (plStart)
                    {
                        loData.DEND_DATE = poStartDate;
                    }
                    else
                    {
                        loData.DSTART_DATE = poEndDate;
                    }
                    loData.IYEAR = loData.IMONTHS = 0;
                    loData.IDAYS = 1;
                    goto EndBlocks;
                }
                DateTime dValueEndDate = poEndDate!.Value.AddDays(1);

                int liChecker = poEndDate!.Value.Day - poStartDate!.Value.Day;


                loData.IDAYS = dValueEndDate.Day - poStartDate!.Value.Day;
                if (loData.IDAYS < 0)
                {
                    DateTime dValueEndDateForHandleDay = dValueEndDate.AddMonths(-1);
                    int liTempDayinMonth = DateTime.DaysInMonth(dValueEndDateForHandleDay.Year, dValueEndDateForHandleDay.Month);
                    loData.IDAYS = liTempDayinMonth + loData.IDAYS;
                    if (loData.IDAYS < 0) { loException.Add("ErrDev", "Value is negative!"); }
                    loData.IMONTHS = dValueEndDateForHandleDay.Month - poStartDate!.Value.Month;
                    if (loData.IMONTHS < 0)
                    {
                        loData.IMONTHS = 12 + loData.IMONTHS;
                        DateTime dValueEndDateForHandleMonth = dValueEndDate.AddYears(-1);
                        loData.IYEAR = dValueEndDateForHandleMonth.Year - poStartDate!.Value.Year;
                        if (loData.IYEAR < 0)
                        {
                            loData.IYEAR = 0;
                        }
                    }

                }
                else
                {
                    loData.IMONTHS = dValueEndDate.Month - poStartDate!.Value.Month;
                    if (loData.IMONTHS < 0)
                    {
                        loData.IMONTHS = 12 + loData.IMONTHS;
                        DateTime dValueEndDateForHandleMonth = dValueEndDate.AddYears(-1);
                        loData.IYEAR = dValueEndDateForHandleMonth.Year - poStartDate!.Value.Year;
                        if (loData.IYEAR < 0)
                        {
                            loData.IYEAR = 0;
                        }
                    }
                    else
                    {
                        loData.IYEAR = dValueEndDate.Year - poStartDate!.Value.Year;
                    }
                }

            }

            //loData.IYEAR = dValueEndDate.Year - poStartDate!.Value.Year;
            //loData.IMONTH = dValueEndDate.Month - poStartDate!.Value.Month;}
            catch (Exception ex)
            {
                loException.Add(ex);
            }
        EndBlocks:

            R_DisplayException(loException);
        }
       
        #endregion
        #region utilities
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
    }
}
