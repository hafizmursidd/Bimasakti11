using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PMT05500COMMON.DTO;
using R_BlazorFrontEnd;
using R_BlazorFrontEnd.Exceptions;
using R_BlazorFrontEnd.Helpers;

namespace PMT05500Model.ViewModel
{
    public class LMT05500AgreementViewModel : R_ViewModel<LMT05500DepositInfoFrontDTO>
    {
        private LMT05500AgreementModel _model = new LMT05500AgreementModel();
        public List<LMT05500PropertyDTO> PropertyList { get; set; } = new List<LMT05500PropertyDTO>();
        public ObservableCollection<LMT05500AgreementDTO> AgreementList =
            new ObservableCollection<LMT05500AgreementDTO>();
        public ObservableCollection<LMT05500UnitDTO> DepositUnitList =
            new ObservableCollection<LMT05500UnitDTO>();

        public string PropertyValueContext = "";
        public string? UnitDescValue { get; set; }

        public LMT05500DBParameter poParamTabDeposit = new LMT05500DBParameter();

        public bool _dropdownProperty = true;
        public bool _enabledTabDeposit;
        public async Task GetPropertyList()
        {
            var loEx = new R_Exception();

            try
            {
                var loResult = await _model.GetPropertyListStreamAsyncModel();
                PropertyList = loResult.Data;
                if (PropertyList.Count > 0)
                {
                    PropertyValueContext = PropertyList[0].CPROPERTY_ID;
                }
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }
            loEx.ThrowExceptionIfErrors();
        }

        public async Task GetAllAgreementList()
        {
            R_Exception loException = new R_Exception();
            try
            {
                R_FrontContext.R_SetStreamingContext(ContextConstant.CPROPERTY_ID, PropertyValueContext);
                var loResult = await _model.GetAgreementListStreamAsyncModel();

                // var tempAgreementList = new List<LMT05500AgreementDTO>();

                //CONVERT TO DATETIME
                loResult.Data = loResult.Data!
                    .Select(item =>
                    {
                        item.DSTART_DATE = ConvertStringToDateTimeFormat(item.CSTART_DATE!);
                        item.DEND_DATE = ConvertStringToDateTimeFormat(item.CEND_DATE!);
                        item.DDOC_DATE = ConvertStringToDateTimeFormat(item.CDOC_DATE!);
                        return item;
                    }).ToList();

                AgreementList = new ObservableCollection<LMT05500AgreementDTO>(loResult.Data);

                if (AgreementList.Count > 0)
                {
                    var loParam = R_FrontUtility.ConvertObjectToObject<LMT05500DBParameter>(AgreementList[0]);
                    poParamTabDeposit = loParam;
                    _enabledTabDeposit = true;
                }
                else
                {
                    _enabledTabDeposit = false;
                }
            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }
            loException.ThrowExceptionIfErrors();
        }

        public async Task GetAllDepositUnitList()
        {
            R_Exception loException = new R_Exception();
            try
            {
                var temp = poParamTabDeposit;

                if (poParamTabDeposit != null)
                {
                    UnitDescValue = temp.CUNIT_DESCRIPTION;
                    R_FrontContext.R_SetStreamingContext(ContextConstant.CPROPERTY_ID, poParamTabDeposit.CPROPERTY_ID);
                    R_FrontContext.R_SetStreamingContext(ContextConstant.CDEPT_CODE, poParamTabDeposit.CDEPT_CODE);
                    R_FrontContext.R_SetStreamingContext(ContextConstant.CTRANS_CODE, poParamTabDeposit.CTRANS_CODE);
                    R_FrontContext.R_SetStreamingContext(ContextConstant.CREF_NO, poParamTabDeposit.CREF_NO);
                    var loResult = await _model.GetDepositUnitStreamAsyncModel();

                    DepositUnitList = new ObservableCollection<LMT05500UnitDTO>(loResult.Data);

                    if (DepositUnitList.Count > 0)
                    {
                        poParamTabDeposit.CFLOOR_ID = DepositUnitList[0].CFLOOR_ID;
                        poParamTabDeposit.CUNIT_ID = DepositUnitList[0].CUNIT_ID;
                        _enabledTabDeposit = true;
                    }
                    else
                    {
                        _enabledTabDeposit = false;
                    }
                }
            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }
            loException.ThrowExceptionIfErrors();
        }

        private DateTime? ConvertStringToDateTimeFormat(string pcEntity)
        {
            if (string.IsNullOrWhiteSpace(pcEntity))
            {
                // Jika string kosong atau null, kembalikan DateTime.MinValue atau nilai default yang sesuai
                return null; // atau DateTime.MinValue atau DateTime.Now atau nilai default yang sesuai dengan kebutuhan Anda
            }
            // Parse string ke DateTime
            DateTime result;
            if (DateTime.TryParseExact(pcEntity, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            {
                return result;
            }

            // Jika parsing gagal, kembalikan DateTime.MinValue atau nilai default yang sesuai
            return null; // atau DateTime.MinValue atau DateTime.Now atau nilai default yang sesuai dengan kebutuhan Anda
        }
    }
}
