using BlazorClientHelper;
using Lookup_GSCOMMON.DTOs;
using Lookup_GSFRONT;
using Lookup_GSModel.ViewModel;
using Lookup_PMCOMMON.DTOs;
using Lookup_PMCOMMON.DTOs.LML01000;
using Lookup_PMCOMMON.DTOs.LML01100;
using Lookup_PMFRONT;
using Lookup_PMModel.ViewModel.LML00500;
using Lookup_PMModel.ViewModel.LML00600;
using Lookup_PMModel.ViewModel.LML01000;
using Lookup_PMModel.ViewModel.LML01100;
using Microsoft.AspNetCore.Components;
using PMT01700COMMON.DTO._2._LOO._2._LOO___Offer;
using PMT01700COMMON.DTO._3._LOC._2._LOC;
using PMT01700COMMON.DTO.Utilities;
using PMT01700COMMON.DTO.Utilities.Front;
using PMT01700FrontResources;
using PMT01700MODEL.ViewModel;
using R_BlazorFrontEnd.Controls;
using R_BlazorFrontEnd.Controls.DataControls;
using R_BlazorFrontEnd.Controls.Events;
using R_BlazorFrontEnd.Enums;
using R_BlazorFrontEnd.Exceptions;
using R_BlazorFrontEnd.Helpers;
using R_BlazorFrontEnd.Interfaces;
using R_CommonFrontBackAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMT01700FRONT
{
    public partial class PMT01700LOC_LOC : R_Page
    {
        private readonly PMT01700LOC_LOCViewModel _viewModel = new();
        private R_Conductor? _conductor;
        //private R_Grid<PMT01100LOC_LOC_SelectedLOCDTO>? _gridRef;

        [Inject] IClientHelper? _clientHelper { get; set; }

        PMT01700EventCallBackDTO _oEventCallBack = new PMT01700EventCallBackDTO();
        private bool _isCheckerDataFound = false;
        private bool _lDataCREF_NO = false;

        //Tambahan
        bool _isAllDataReady = false;

        //For New Open Page
        bool LOpenAsNormalPage = false;
        private string PageWidth = "width: auto;";
        protected override async Task R_Init_From_Master(object poParameter)
        {
            var loEx = new R_Exception();

            try
            {
                //lControlChoosenData = true;
                _viewModel.oParameter = R_FrontUtility.ConvertObjectToObject<PMT01700ParameterFrontChangePageDTO>(poParameter);
                LOpenAsNormalPage = string.IsNullOrEmpty(_viewModel.oParameter.CALLER_ACTION);
                await _viewModel.GetVAR_GSM_TRANSACTION_CODE();
                //await _viewModel.GetComboBoxDataCLeaseMode();
                //await _viewModel.GetComboBoxDataCChargesMode();
                if (!LOpenAsNormalPage)
                {
                    PageWidth = "width: 1100px;";
                    var loData = R_FrontUtility.ConvertObjectToObject<PMT01700ParameterLOO_Offer_SelectedOfferDTO>(poParameter);
                    if (loData != null)
                    {
                        _viewModel.oParameter = R_FrontUtility.ConvertObjectToObject<PMT01700ParameterFrontChangePageDTO>(loData.Data);
                        _viewModel.oParameter.CTRANS_CODE = "802053";
                        _viewModel.oTempDataForAdd = loData.Data!;
                        await _conductor.Add();
                    }

                    goto EndBlock;
                }

                if (!string.IsNullOrEmpty(_viewModel.oParameter.CPROPERTY_ID))
                {
                    _isAllDataReady = true;
                    if (!string.IsNullOrEmpty(_viewModel.oParameter.CREF_NO))
                    {
                        _isCheckerDataFound = true;
                        await _conductor.R_GetEntity(null);
                    }
                }
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

        EndBlock:
            loEx.ThrowExceptionIfErrors();
        }
        #region Function Component

        #region Master HandleFunction

        private void OnChangedDFOLLOW_UP_DATE(DateTime? poParameter)
        {
            PMT010700_LOC_LOC_SelectedLOCDTO loData = _viewModel.Data;
            loData.DFOLLOW_UP_DATE = poParameter;
        }

        private void OnChangedDHAND_OVER_DATE(DateTime? poParameter)
        {
            PMT010700_LOC_LOC_SelectedLOCDTO loData = _viewModel.Data;
            //  loData.DHAND_OVER_DATE = poParameter;
        }


        private void OnChangedDREF_DATE(DateTime? poParameter)
        {
            PMT010700_LOC_LOC_SelectedLOCDTO loData = _viewModel.Data;
            loData.DREF_DATE = poParameter;
        }

        #region DateTime Function

        #endregion

        private void OnChangedCYEAR(Int32 poParam)
        {
            var loEx = new R_Exception();

            try
            {
                var loData = (PMT010700_LOC_LOC_SelectedLOCDTO)_viewModel.Data;
                var llControl = _viewModel.oControlYMD;
                loData.IYEARS = poParam;

                if (llControl.LYEAR || llControl.LMONTH || llControl.LDAY)
                {
                    if (loData.IYEARS == 0 && loData.IMONTHS == 0 && loData.IDAYS == 0)
                    {
                        loData.DEND_DATE = loData.DSTART_DATE;
                    }
                    else
                    {
                        loData.DEND_DATE = loData.DSTART_DATE!.Value.AddYears(loData.IYEARS).AddMonths(loData.IMONTHS).AddDays(loData.IDAYS).AddDays(-1);
                    }
                }
                else
                {
                    llControl.LYEAR = true;
                    loData.DEND_DATE = loData.DSTART_DATE!.Value.AddYears(loData.IYEARS).AddDays(-1);
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
                var loData = (PMT010700_LOC_LOC_SelectedLOCDTO)_viewModel.Data;
                var llControl = _viewModel.oControlYMD;
                loData.IMONTHS = poParam;

                if (llControl.LYEAR || llControl.LMONTH || llControl.LDAY)
                {
                    if (loData.IYEARS == 0 && loData.IMONTHS == 0 && loData.IDAYS == 0)
                    {
                        loData.DEND_DATE = loData.DSTART_DATE;
                    }
                    else
                    {
                        loData.DEND_DATE = loData.DSTART_DATE!.Value.AddYears(loData.IYEARS).AddMonths(loData.IMONTHS).AddDays(loData.IDAYS).AddDays(-1);
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
                var loData = (PMT010700_LOC_LOC_SelectedLOCDTO)_viewModel.Data;
                var llControl = _viewModel.oControlYMD;
                loData.IDAYS = poParam;

                if (llControl.LYEAR || llControl.LMONTH || llControl.LDAY)
                {
                    if (loData.IYEARS == 0 && loData.IMONTHS == 0 && loData.IDAYS == 0)
                    {
                        loData.DEND_DATE = loData.DSTART_DATE;
                    }
                    else
                    {
                        loData.DEND_DATE = loData.DSTART_DATE!.Value.AddYears(loData.IYEARS).AddMonths(loData.IMONTHS).AddDays(loData.IDAYS).AddDays(-1);
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

        DateTime tStartDate = DateTime.Now.AddDays(-1);

        private void OnChangedDSTART_DATE(DateTime? poValue)
        {
            R_Exception loException = new R_Exception();
            var loData = _viewModel.Data;
            try
            {
                loData.DSTART_DATE = poValue;

                tStartDate = poValue ?? DateTime.Now;

                if (loData.DEND_DATE == null)
                {
                    loData.DEND_DATE = loData.IYEARS == 0 && loData.IMONTHS == 0 && loData.IDAYS == 0
                        ? loData.DSTART_DATE
                        : loData.DSTART_DATE!.Value.AddYears(loData.IYEARS).AddMonths(loData.IMONTHS).AddDays(loData.IDAYS).AddDays(-1);
                }
                else
                {
                    CalculateYMD(poStartDate: loData.DSTART_DATE, poEndDate: loData.DEND_DATE, pcStart: "S");
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
                PMT010700_LOC_LOC_SelectedLOCDTO loData = _viewModel.Data;
                loData.DEND_DATE = poValue;

                if (loData.DSTART_DATE == null)
                {
                    loData.DSTART_DATE = loData.IYEARS == 0 && loData.IMONTHS == 0 && loData.IDAYS == 0
                        ? loData.DEND_DATE
                        : loData.DEND_DATE!.Value.AddYears(loData.IYEARS).AddMonths(loData.IMONTHS).AddDays(loData.IDAYS).AddDays(-1);
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

        private void CalculateYMD(DateTime? poStartDate, DateTime? poEndDate, string pcStart = "")
        {
            R_Exception loException = new R_Exception();
            PMT010700_LOC_LOC_SelectedLOCDTO loData = _viewModel.Data;

            try
            {
                if (poEndDate != null)
                {

                    DateTime dValueEndDate = poEndDate!.Value.AddDays(1);

                    int liChecker = poEndDate!.Value.Day - poStartDate!.Value.Day;
                    if (liChecker < 0)
                    {
                        loData.IDAYS = 1;
                        loData.IMONTHS = loData.IYEARS = 0;
                        if (string.IsNullOrEmpty(pcStart))
                            loData.DSTART_DATE = loData.DEND_DATE;
                        else
                            loData.DEND_DATE = loData.DSTART_DATE;

                    }
                    else
                    {

                        loData.IDAYS = dValueEndDate.Day - poStartDate!.Value.Day;
                        if (loData.IDAYS < 0)
                        {
                            DateTime dValueEndDateForHandleDay = dValueEndDate.AddMonths(-1);
                            int liTempDayinMonth = DateTime.DaysInMonth(dValueEndDateForHandleDay.Year, dValueEndDateForHandleDay.Month);
                            loData.IDAYS = liTempDayinMonth + loData.IDAYS;
                            if (loData.IDAYS < 0) { throw new Exception("ERROR HARINYA MINES"); }
                            loData.IMONTHS = dValueEndDateForHandleDay.Month - poStartDate!.Value.Month;
                            if (loData.IMONTHS < 0)
                            {
                                loData.IMONTHS = 12 + loData.IMONTHS;
                                DateTime dValueEndDateForHandleMonth = dValueEndDate.AddYears(-1);
                                loData.IYEARS = dValueEndDateForHandleMonth.Year - poStartDate!.Value.Year;
                                if (loData.IYEARS < 0)
                                {
                                    loData.IYEARS = 0;
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
                                loData.IYEARS = dValueEndDateForHandleMonth.Year - poStartDate!.Value.Year;
                                if (loData.IYEARS < 0)
                                {
                                    loData.IYEARS = 0;
                                }
                            }
                            else
                            {
                                loData.IYEARS = dValueEndDate.Year - poStartDate!.Value.Year;
                            }
                        }
                    }
                }
                else
                {
                    if (poStartDate != null)
                    {
                        loData.DEND_DATE = loData.DSTART_DATE!.Value.AddDays(1);
                        loData.IYEARS = loData.IMONTHS = 0;
                        loData.IDAYS = 2;
                    }


                }
            }

            //loData.IYEARS = dValueEndDate.Year - poStartDate!.Value.Year;
            //loData.IMONTHS = dValueEndDate.Month - poStartDate!.Value.Month;}
            catch (Exception ex)
            {
                loException.Add(ex);
            }

            R_DisplayException(loException);
        }
        private void OnChangedDSTART_TIME(DateTime? poValue)
        {
            R_Exception loException = new R_Exception();

            try
            {
                PMT010700_LOC_LOC_SelectedLOCDTO loData = _viewModel.Data;
                loData.DSTART_TIME = poValue;

                if (loData.DSTART_TIME == null)
                {
                    //    loData.DSTART_DATE = loData.IYEAR == 0 && loData.IMONTH == 0 && loData.IDAYS == 0
                    //        ? loData.DEND_DATE
                    //        : loData.DEND_DATE!.Value.AddYears(loData.IYEAR).AddMonths(loData.IMONTH).AddDays(loData.IDAYS).AddDays(-1);
                }
                else
                {

                    // CalculateYMD(poStartDate: loData.DSTART_DATE, poEndDate: loData.DEND_DATE);
                }
            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }

            R_DisplayException(loException);
        }
        private void OnChangedDEND_TIME(DateTime? poValue)
        {
            R_Exception loException = new R_Exception();

            try
            {
                PMT010700_LOC_LOC_SelectedLOCDTO loData = _viewModel.Data;
                loData.DEND_TIME = poValue;

                if (loData.DEND_TIME == null)
                {
                    //    loData.DSTART_DATE = loData.IYEAR == 0 && loData.IMONTH == 0 && loData.IDAYS == 0
                    //        ? loData.DEND_DATE
                    //        : loData.DEND_DATE!.Value.AddYears(loData.IYEAR).AddMonths(loData.IMONTH).AddDays(loData.IDAYS).AddDays(-1);
                }
                else
                {

                    // CalculateYMD(poStartDate: loData.DSTART_DATE, poEndDate: loData.DEND_DATE);
                }
            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }

            R_DisplayException(loException);
        }

        #endregion

        #endregion

        #region Conductor Event

        private void AfterAdd(R_AfterAddEventArgs eventArgs)
        {
            R_Exception loException = new R_Exception();
            var loData = (PMT010700_LOC_LOC_SelectedLOCDTO)eventArgs.Data;

            try
            {
                _lDataCREF_NO = !_viewModel.oVarGSMTransactionCode.LINCREMENT_FLAG;

                _oEventCallBack.LCRUD_MODE = true;
                var loTempConvertData = R_FrontUtility.ConvertObjectToObject<PMT010700_LOC_LOC_SelectedLOCDTO>(_viewModel.oTempDataForAdd);
                //loData = loTempConvertData;
                loData.CTENANT_ID = loTempConvertData.CTENANT_ID;
                loData.CTENANT_NAME = loTempConvertData.CTENANT_NAME;

                loData.CBUILDING_ID = loTempConvertData.CBUILDING_ID;
                loData.CBUILDING_NAME = loTempConvertData.CBUILDING_NAME;

                loData.CDEPT_CODE = loTempConvertData.CDEPT_CODE;
                loData.CDEPT_NAME = loTempConvertData.CDEPT_NAME;

                loData.CSALESMAN_ID = loTempConvertData.CSALESMAN_ID;
                loData.CSALESMAN_NAME = loTempConvertData.CSALESMAN_NAME;

                /* in the top is a mandatory field */

                loData.CTRANS_CODE = _viewModel.oParameter.CTRANS_CODE;

                loData.CPROPERTY_ID = _viewModel.oParameter.CPROPERTY_ID;
              //  loData.CREF_NO = _viewModel.oParameter.CREF_NO;
             //   loData.CORIGINAL_REF_NO = _viewModel.oParameter.CREF_NO;
                /* in the top is a mandatory field */
                //loData.CLEASE_MODE = _viewModel.loComboBoxDataCLeaseMode.First().CCODE;
                //loData.CCHARGE_MODE = _viewModel.loComboBoxDataCChargesMode.First().CCODE;

                loData.DREF_DATE = loTempConvertData.DREF_DATE ?? DateTime.Now;
                loData.DFOLLOW_UP_DATE = loTempConvertData.DFOLLOW_UP_DATE ?? DateTime.Now;
                loData.IYEARS = loTempConvertData.IYEARS;
                loData.IMONTHS = loTempConvertData.IMONTHS;
                loData.IDAYS = loTempConvertData.IDAYS;
                loData.DSTART_DATE = loTempConvertData.DSTART_DATE ?? DateTime.Now;
                loData.DEND_DATE = loTempConvertData.DEND_DATE ?? DateTime.Now;
                loData.CUNIT_DESCRIPTION = loTempConvertData.CUNIT_DESCRIPTION;
                loData.CBILLING_RULE_CODE = loTempConvertData.CBILLING_RULE_CODE;
                loData.NBOOKING_FEE = loTempConvertData.NBOOKING_FEE;
                loData.CTC_CODE = loTempConvertData.CTC_CODE;
                loData.CNOTES = loTempConvertData.CNOTES;

                //CR07/12/2024 //TRANSCODE ANAD CREF NO FROM LOO
                loData.CLINK_TRANS_CODE = loTempConvertData.CTRANS_CODE;
                loData.CLINK_REF_NO = loTempConvertData.CREF_NO;

            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }

            R_DisplayException(loException);
        }

        private async void AfterSave(R_AfterSaveEventArgs eventArgs)
        {
            var loException = new R_Exception();
            //Task loTask;

            try
            {
                if (!LOpenAsNormalPage)
                {
                    await Close(true, "INI SUCCESS BOSS");
                }

                _isCheckerDataFound = true;
                _oEventCallBack.LCRUD_MODE = true;
                //_oEventCallBack.LACTIVEUnitInfoHasData = true;
                _oEventCallBack.CCRUD_MODE = "A_ADD";//Meaning of Agreement Add
                _oEventCallBack.ODATA_PARAMETER = new PMT01700ParameterFrontChangePageDTO()
                {
                    CPROPERTY_ID = _viewModel.Data.CPROPERTY_ID,
                    CDEPT_CODE = _viewModel.Data.CDEPT_CODE,
                    CTRANS_CODE = _viewModel.Data.CTRANS_CODE,
                    CREF_NO = _viewModel.Data.CREF_NO,
                    CBUILDING_ID = _viewModel.Data.CBUILDING_ID,
                };
                /*
                _oEventCallBack.CDEPT_CODE = _viewModel.Data.CDEPT_CODE!;
                _oEventCallBack.CTRANS_CODE = _viewModel.Data.CTRANS_CODE!;
                _oEventCallBack.CBUILDING_ID = _viewModel.Data.CBUILDING_ID!;
                _oEventCallBack.CCHARGE_MODE = _viewModel.Data.CCHARGE_MODE!;
                _oEventCallBack.CCURRENCY_CODE = _viewModel.Data.CCURRENCY_CODE!;
                */

                // Lakukan pemanggilan async
                await InvokeTabEventCallbackAsync(_oEventCallBack);

                // Setelah pemanggilan selesai, lanjutkan dengan kode selanjutnya
                _oEventCallBack.CCRUD_MODE = "";
            }
            catch (Exception Ex)
            {
                loException.Add(Ex);
            }

            R_DisplayException(loException);
        }


        private void R_SetEdit(R_SetEventArgs eventArgs)
        {

        }

        public void AfterDelete()
        {
        }

        private async Task R_SetOther(R_SetEventArgs eventArgs)
        {
            R_Exception loException = new R_Exception();

            try
            {
                _viewModel.lControlCRUDMode = eventArgs.Enable;
                _oEventCallBack.LCRUD_MODE = _viewModel.lControlCRUDMode = eventArgs.Enable;
                await InvokeTabEventCallbackAsync(_oEventCallBack);
            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }

            R_DisplayException(loException);
        }

        private void R_Validation(R_ValidationEventArgs eventArgs)
        {
            var loException = new R_Exception();

            try
            {
                var loData = (PMT010700_LOC_LOC_SelectedLOCDTO)eventArgs.Data;
                //await LostFocusCUNIT_ID();

                if (_lDataCREF_NO)
                {
                    if (string.IsNullOrWhiteSpace(loData.CREF_NO))
                    {
                        var loErr = R_FrontUtility.R_GetError(typeof(Resources_PMT01700_Class), "ValidationAgreementNo");
                        loException.Add(loErr);
                    }


                    if (string.IsNullOrWhiteSpace(loData.CTENANT_ID))
                    {
                        var loErr = R_FrontUtility.R_GetError(typeof(Resources_PMT01700_Class), "ValidationTenant");
                        loException.Add(loErr);
                    }



                    if (string.IsNullOrWhiteSpace(loData.CBUILDING_ID))
                    {
                        var loErr = R_FrontUtility.R_GetError(typeof(Resources_PMT01700_Class), "ValidationBuilding");
                        loException.Add(loErr);
                    }

                    if (string.IsNullOrWhiteSpace(loData.CSALESMAN_ID))
                    {
                        var loErr = R_FrontUtility.R_GetError(typeof(Resources_PMT01700_Class), "ValidationSalesman");
                        loException.Add(loErr);
                    }

                    if (loData.DREF_DATE == null)
                    {
                        var loErr = R_FrontUtility.R_GetError(typeof(Resources_PMT01700_Class), "ValidationOfferDate");
                        loException.Add(loErr);
                    }

                    if (loData.DSTART_DATE > loData.DEND_DATE)
                    {
                        var loErr = R_FrontUtility.R_GetError(typeof(Resources_PMT01700_Class), "ValidationDate");
                        loException.Add(loErr);
                    }

                }
            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }

            eventArgs.Cancel = loException.HasError;


            loException.ThrowExceptionIfErrors();
        }

        #endregion

        #region Master CRUD

        private void R_Display(R_DisplayEventArgs eventArgs)
        {
            var loException = new R_Exception();

            try
            {
                //switch (eventArgs.ConductorMode)
                //{
                //    case R_eConductorMode.Edit:
                //        //await _componentCDOC_NOTextBox.FocusAsync();
                //        //OnChangedDEND_DATE(_viewModel.Data.DEND_DATE);
                //        break;
                //    default:
                //        break;
                //}

            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }

            loException.ThrowExceptionIfErrors();
        }
        public async Task R_BeforeCancel(R_BeforeCancelEventArgs eventArgs)
        {
            R_Exception loException = new R_Exception();

            try
            {
                _oEventCallBack.LCRUD_MODE = true;
                _oEventCallBack.CCRUD_MODE = "A_CANCEL";//Meaning of Agreement Add
                await InvokeTabEventCallbackAsync(_oEventCallBack);
                // Setelah pemanggilan selesai, lanjutkan dengan kode selanjutnya
                _oEventCallBack.CCRUD_MODE = "";

                //if (!string.IsNullOrEmpty(_viewModel.oParameter.CREF_NO))
                //{
                //    _isCheckerDataFound = true;
                //    await _conductor.R_GetEntity(null);
                //}
            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }

            R_DisplayException(loException);
        }

        private async Task ServiceGetRecord(R_ServiceGetRecordEventArgs eventArgs)
        {
            var loEx = new R_Exception();
            PMT010700_LOC_LOC_SelectedLOCDTO loParam;

            try
            {
                loParam = new PMT010700_LOC_LOC_SelectedLOCDTO();
                if (eventArgs.Data != null)
                {
                    loParam = R_FrontUtility.ConvertObjectToObject<PMT010700_LOC_LOC_SelectedLOCDTO>(eventArgs.Data);
                }
                else
                {
                    loParam.CREF_NO = _viewModel.oParameter.CREF_NO;
                    loParam.CPROPERTY_ID = _viewModel.oParameter.CPROPERTY_ID;
                    loParam.CDEPT_CODE = _viewModel.oParameter.CDEPT_CODE;
                    loParam.CTRANS_CODE = _viewModel.oParameter.CTRANS_CODE;
                    loParam.CCOMPANY_ID = _clientHelper.CompanyId;
                    loParam.CUSER_ID = _clientHelper.UserId;
                };
                await _viewModel.GetEntity(loParam);

                eventArgs.Result = _viewModel.oEntity;
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            loEx.ThrowExceptionIfErrors();
        }

        private async Task ServiceSave(R_ServiceSaveEventArgs eventArgs)
        {
            var loEx = new R_Exception();

            try
            {
                var loParam = R_FrontUtility.ConvertObjectToObject<PMT010700_LOC_LOC_SelectedLOCDTO>(eventArgs.Data);

                if ((R_eConductorMode)eventArgs.ConductorMode == R_eConductorMode.Add)
                {
                    loParam.CPROPERTY_ID = _viewModel.oParameter.CPROPERTY_ID;
                }

                await _viewModel.ServiceSave(loParam, (eCRUDMode)eventArgs.ConductorMode);
                eventArgs.Result = _viewModel.oEntity;
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            loEx.ThrowExceptionIfErrors();
        }

        private async Task ServiceDelete(R_ServiceDeleteEventArgs eventArgs)
        {
            var loEx = new R_Exception();

            try
            {
                var loData = (PMT010700_LOC_LOC_SelectedLOCDTO)eventArgs.Data;

                await _viewModel.GetEntity(loData);

                if (_viewModel.oEntity != null)
                    await _viewModel.ServiceDelete(loData);
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            loEx.ThrowExceptionIfErrors();
        }

        #endregion

        #region Master LookUp


        #region Lookup Button Tenant Lookup

        private R_Lookup? R_LookupTenantLookup;

        private void BeforeOpenLookUpTenantLookup(R_BeforeOpenLookupEventArgs eventArgs)
        {
            LML00600ParameterDTO? param = null;
            if (!string.IsNullOrEmpty(_viewModel.oParameter.CPROPERTY_ID))
            {
                param = new LML00600ParameterDTO()
                {
                    CCOMPANY_ID = _clientHelper.CompanyId,
                    CUSER_ID = _clientHelper.UserId,
                    CPROPERTY_ID = _viewModel.oParameter.CPROPERTY_ID,
                    CCUSTOMER_TYPE = "01",
                };
            }
            eventArgs.Parameter = param;
            eventArgs.TargetPageType = typeof(LML00600);
        }

        private void AfterOpenLookUpTenantLookup(R_AfterOpenLookupEventArgs eventArgs)
        {
            R_Exception loException = new R_Exception();
            LML00600DTO? loTempResult = null;
            //PMT01100LOC_LOC_SelectedLOCDTO? loGetData = null;

            try
            {
                loTempResult = (LML00600DTO)eventArgs.Result;
                if (loTempResult == null)
                    return;
                //loGetData = (PMT01100LOC_LOC_SelectedLOCDTO)_conductorFullPMT01500Agreement.R_GetCurrentData();

                _viewModel.Data.CTENANT_ID = loTempResult.CTENANT_ID;
                _viewModel.Data.CTENANT_NAME = loTempResult.CTENANT_NAME;
            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }

            R_DisplayException(loException);

        }

        private R_TextBox? _componentCTENANT_IDTextBox;

        private async Task OnLostFocusTenant()
        {
            R_Exception loEx = new R_Exception();

            try
            {
                PMT010700_LOC_LOC_SelectedLOCDTO loGetData = _viewModel.Data;

                if (string.IsNullOrWhiteSpace(loGetData.CTENANT_ID))
                {
                    loGetData.CTENANT_ID = "";
                    loGetData.CTENANT_NAME = "";
                    return;
                }

                LookupLML00600ViewModel loLookupViewModel = new LookupLML00600ViewModel();
                LML00600ParameterDTO loParam = new LML00600ParameterDTO()
                {

                    CCOMPANY_ID = _clientHelper.CompanyId,
                    CUSER_ID = _clientHelper.UserId,
                    CPROPERTY_ID = _viewModel.oParameter.CPROPERTY_ID,
                    CCUSTOMER_TYPE = "01",
                    CSEARCH_TEXT = loGetData.CTENANT_ID ?? "",
                };

                var loResult = await loLookupViewModel.GetTenant(loParam);

                if (loResult == null)
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                            typeof(Lookup_GSFrontResources.Resources_Dummy_Class),
                            "_ErrLookup01"));
                    loGetData.CTENANT_ID = "";
                    loGetData.CTENANT_NAME = "";
                    //await GLAccount_TextBox.FocusAsync();
                }
                else
                {
                    loGetData.CTENANT_ID = loResult.CTENANT_ID;
                    loGetData.CTENANT_NAME = loResult.CTENANT_NAME;
                }
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            R_DisplayException(loEx);
        }

        #endregion

        #region Lookup Button Building Lookup

        private R_Lookup? R_LookupBuildingLookup;

        private void BeforeOpenLookUpBuildingLookup(R_BeforeOpenLookupEventArgs eventArgs)
        {
            GSL02200ParameterDTO? param = null;
            if (!string.IsNullOrEmpty(_viewModel.oParameter.CPROPERTY_ID))
            {
                param = new GSL02200ParameterDTO
                {
                    CPROPERTY_ID = _viewModel.oParameter.CPROPERTY_ID
                };
            }
            eventArgs.Parameter = param;
            eventArgs.TargetPageType = typeof(GSL02200);
        }

        private void AfterOpenLookUpBuildingLookup(R_AfterOpenLookupEventArgs eventArgs)
        {
            R_Exception loException = new R_Exception();
            GSL02200DTO? loTempResult = null;
            //PMT01100LOC_LOC_SelectedLOCDTO? loGetData = null;

            try
            {
                loTempResult = (GSL02200DTO)eventArgs.Result;
                if (loTempResult == null)
                    return;
                //loGetData = (PMT01100LOC_LOC_SelectedLOCDTO)_conductorFullPMT01500Agreement.R_GetCurrentData();

                _viewModel.Data.CBUILDING_ID = loTempResult.CBUILDING_ID;
                _viewModel.Data.CBUILDING_NAME = loTempResult.CBUILDING_NAME;
            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }

            R_DisplayException(loException);

        }

        private async Task OnLostFocusBuilding()
        {
            R_Exception loEx = new R_Exception();

            try
            {
                PMT010700_LOC_LOC_SelectedLOCDTO loGetData = (PMT010700_LOC_LOC_SelectedLOCDTO)_viewModel.Data;

                if (string.IsNullOrWhiteSpace(_viewModel.Data.CBUILDING_ID))
                {
                    loGetData.CBUILDING_ID = "";
                    loGetData.CBUILDING_NAME = "";
                    return;
                }

                LookupGSL02200ViewModel loLookupViewModel = new LookupGSL02200ViewModel();
                GSL02200ParameterDTO loParam = new GSL02200ParameterDTO()
                {
                    CPROPERTY_ID = _viewModel.oParameter.CPROPERTY_ID!,
                    CSEARCH_TEXT = loGetData.CBUILDING_ID ?? "",
                };

                var loResult = await loLookupViewModel.GetBuilding(loParam);

                if (loResult == null)
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                            typeof(Lookup_GSFrontResources.Resources_Dummy_Class),
                            "_ErrLookup01"));
                    loGetData.CBUILDING_ID = "";
                    loGetData.CBUILDING_NAME = "";
                    //await GLAccount_TextBox.FocusAsync();
                }
                else
                {
                    loGetData.CBUILDING_ID = loResult.CBUILDING_ID;
                    loGetData.CBUILDING_NAME = loResult.CBUILDING_NAME;
                }
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            R_DisplayException(loEx);
        }

        #endregion

        #region Lookup Button Department Lookup

        private R_Lookup? R_LookupDepartmentLookup;

        private void BeforeOpenLookUpDepartmentLookup(R_BeforeOpenLookupEventArgs eventArgs)
        {
            GSL00710ParameterDTO? param = null;
            if (!string.IsNullOrEmpty(_viewModel.oParameter.CPROPERTY_ID))
            {
                param = new GSL00710ParameterDTO
                {
                    CCOMPANY_ID = _clientHelper.CompanyId,
                    CUSER_LOGIN_ID = _clientHelper.UserId,
                    CPROPERTY_ID = _viewModel.oParameter.CPROPERTY_ID,
                };
            }
            eventArgs.Parameter = param;
            eventArgs.TargetPageType = typeof(GSL00710);
        }

        private void AfterOpenLookUpDepartmentLookup(R_AfterOpenLookupEventArgs eventArgs)
        {
            R_Exception loException = new R_Exception();
            GSL00710DTO? loTempResult = null;
            //PMT01100LOC_LOC_SelectedLOCDTO? loGetData = null;


            try
            {
                loTempResult = (GSL00710DTO)eventArgs.Result;
                if (loTempResult == null)
                    return;

                //loGetData = (PMT01100LOC_LOC_SelectedLOCDTO)_conductorFullPMT01500Agreement.R_GetCurrentData();

                _viewModel.Data.CDEPT_CODE = loTempResult.CDEPT_CODE;
                _viewModel.Data.CDEPT_NAME = loTempResult.CDEPT_NAME;
            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }

            R_DisplayException(loException);

        }

        private async Task OnLostFocusDepartment()
        {
            R_Exception loEx = new R_Exception();

            try
            {
                PMT010700_LOC_LOC_SelectedLOCDTO loGetData = (PMT010700_LOC_LOC_SelectedLOCDTO)_viewModel.Data;

                if (string.IsNullOrWhiteSpace(_viewModel.Data.CDEPT_CODE))
                {
                    loGetData.CDEPT_CODE = "";
                    loGetData.CDEPT_NAME = "";
                    return;
                }

                LookupGSL00710ViewModel loLookupViewModel = new LookupGSL00710ViewModel();
                GSL00710ParameterDTO loParam = new GSL00710ParameterDTO()
                {
                    CCOMPANY_ID = _clientHelper.CompanyId,
                    CUSER_LOGIN_ID = _clientHelper.UserId,
                    CPROPERTY_ID = _viewModel.oParameter.CPROPERTY_ID!,
                    CSEARCH_TEXT = loGetData.CDEPT_CODE ?? "",
                };

                var loResult = await loLookupViewModel.GetDepartmentProperty(loParam);

                if (loResult == null)
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                            typeof(Lookup_GSFrontResources.Resources_Dummy_Class),
                            "_ErrLookup01"));
                    loGetData.CDEPT_CODE = "";
                    loGetData.CDEPT_NAME = "";
                    //await GLAccount_TextBox.FocusAsync();
                }
                else
                {
                    loGetData.CDEPT_CODE = loResult.CDEPT_CODE;
                    loGetData.CDEPT_NAME = loResult.CDEPT_NAME;
                }
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            R_DisplayException(loEx);
        }

        #endregion

        #region Lookup Button Salesman Lookup

        private R_Lookup? R_LookupSalesmanLookup;

        private void BeforeOpenLookUpSalesmanLookup(R_BeforeOpenLookupEventArgs eventArgs)
        {
            LML00500ParameterDTO? param = null;
            if (!string.IsNullOrEmpty(_viewModel.oParameter.CPROPERTY_ID))
            {
                param = new LML00500ParameterDTO
                {
                    CCOMPANY_ID = _clientHelper.CompanyId,
                    CPROPERTY_ID = _viewModel.oParameter.CPROPERTY_ID,
                    CUSER_ID = _clientHelper.UserId
                };
            }
            eventArgs.Parameter = param;
            eventArgs.TargetPageType = typeof(LML00500);
        }

        private void AfterOpenLookUpSalesmanLookup(R_AfterOpenLookupEventArgs eventArgs)
        {
            R_Exception loException = new R_Exception();
            LML00500DTO? loTempResult = null;
            //PMT01100LOC_LOC_SelectedLOCDTO? loGetData = null;


            try
            {
                loTempResult = (LML00500DTO)eventArgs.Result;
                if (loTempResult == null)
                    return;

                //loGetData = (PMT01100LOC_LOC_SelectedLOCDTO)_conductorFullPMT01500Agreement.R_GetCurrentData();

                _viewModel.Data.CSALESMAN_ID = loTempResult.CSALESMAN_ID;
                _viewModel.Data.CSALESMAN_NAME = loTempResult.CSALESMAN_NAME;
            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }

            R_DisplayException(loException);

        }

        private async Task OnLostFocusSalesman()
        {
            R_Exception loEx = new R_Exception();

            try
            {
                PMT010700_LOC_LOC_SelectedLOCDTO loGetData = (PMT010700_LOC_LOC_SelectedLOCDTO)_viewModel.Data;

                if (string.IsNullOrWhiteSpace(_viewModel.Data.CSALESMAN_ID))
                {
                    loGetData.CSALESMAN_ID = "";
                    loGetData.CSALESMAN_NAME = "";
                    return;
                }

                LookupLML00500ViewModel loLookupViewModel = new LookupLML00500ViewModel();
                LML00500ParameterDTO loParam = new LML00500ParameterDTO()
                {
                    CCOMPANY_ID = _clientHelper.CompanyId,
                    CPROPERTY_ID = _viewModel.oParameter.CPROPERTY_ID!,
                    CUSER_ID = _clientHelper.UserId,
                    CSEARCH_TEXT = loGetData.CSALESMAN_ID ?? "",
                };

                var loResult = await loLookupViewModel.GetSalesman(loParam);

                if (loResult == null)
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                            typeof(Lookup_GSFrontResources.Resources_Dummy_Class),
                            "_ErrLookup01"));
                    loGetData.CSALESMAN_ID = "";
                    loGetData.CSALESMAN_NAME = "";
                    //await GLAccount_TextBox.FocusAsync();
                }
                else
                {
                    loGetData.CSALESMAN_ID = loResult.CSALESMAN_ID;
                    loGetData.CSALESMAN_NAME = loResult.CSALESMAN_NAME;
                }
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            R_DisplayException(loEx);
        }

        #endregion

        #region Lookup Button BillingRule Lookup

        private R_Lookup? R_LookupCurrencyLookup;
        private void BeforeOpenLookUpCurrencyLookup(R_BeforeOpenLookupEventArgs eventArgs)
        {
          GSL00300ParameterDTO? param = null;
            if (!string.IsNullOrEmpty(_viewModel.oParameter.CPROPERTY_ID))
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

            try
            {
                loTempResult = (GSL00300DTO)eventArgs.Result;
                if (loTempResult == null)
                    return;


                _viewModel.Data.CCURRENCY_CODE = loTempResult.CCURRENCY_CODE;
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
                PMT010700_LOC_LOC_SelectedLOCDTO loGetData = _viewModel.Data;

                if (string.IsNullOrWhiteSpace(_viewModel.Data.CBILLING_RULE_CODE))
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
                    loGetData.CBILLING_RULE_CODE = "";
                    //await GLAccount_TextBox.FocusAsync();
                }
                else
                {
                    loGetData.CCURRENCY_CODE = loResult.CCURRENCY_CODE;
                }
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            R_DisplayException(loEx);
        }

        #endregion

        #region Lookup Button TandC Lookup

        private R_Lookup? R_LookupTandCLookup;


        private void BeforeOpenLookUpTandCLookup(R_BeforeOpenLookupEventArgs eventArgs)
        {
            LML01100ParameterDTO? param = null;
            if (!string.IsNullOrEmpty(_viewModel.oParameter.CPROPERTY_ID))
            {
                param = new LML01100ParameterDTO
                {
                    CCOMPANY_ID = _clientHelper.CompanyId,
                    CUSER_ID = _clientHelper.UserId,
                    CPROPERTY_ID = _viewModel.oParameter.CPROPERTY_ID
                };
            }
            eventArgs.Parameter = param;
            eventArgs.TargetPageType = typeof(LML01100);
        }

        private void AfterOpenLookUpTandCLookup(R_AfterOpenLookupEventArgs eventArgs)
        {
            R_Exception loException = new R_Exception();
            LML01100DTO? loTempResult = null;

            try
            {
                loTempResult = (LML01100DTO)eventArgs.Result;
                if (loTempResult == null)
                    return;

                _viewModel.Data.CTC_CODE = loTempResult.CTC_CODE;
            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }

            R_DisplayException(loException);

        }

        private async Task OnLostFocusTandC()
        {
            R_Exception loEx = new R_Exception();

            try
            {
                PMT010700_LOC_LOC_SelectedLOCDTO loGetData = _viewModel.Data;

                if (string.IsNullOrWhiteSpace(_viewModel.Data.CTC_CODE))
                {
                    loGetData.CTC_CODE = "";
                    return;
                }

                LookupLML01100ViewModel loLookupViewModel = new LookupLML01100ViewModel();
                LML01100ParameterDTO loParam = new LML01100ParameterDTO()
                {
                    CCOMPANY_ID = _clientHelper.CompanyId,
                    CUSER_ID = _clientHelper.UserId,
                    CPROPERTY_ID = _viewModel.oParameter.CPROPERTY_ID,
                    CSEARCH_TEXT = loGetData.CTC_CODE ?? "",
                };

                var loResult = await loLookupViewModel.GetTermNCondition(loParam);

                if (loResult == null)
                {
                    loEx.Add(R_FrontUtility.R_GetError(
                            typeof(Lookup_GSFrontResources.Resources_Dummy_Class),
                            "_ErrLookup01"));
                    loGetData.CTC_CODE = "";
                    //await GLAccount_TextBox.FocusAsync();
                }
                else
                {
                    loGetData.CTC_CODE = loResult.CTC_CODE;
                }
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            R_DisplayException(loEx);
        }

        #endregion

        #endregion

        #region Helper
        private void BlankFunctionButton() { }
        #endregion
    }
}
