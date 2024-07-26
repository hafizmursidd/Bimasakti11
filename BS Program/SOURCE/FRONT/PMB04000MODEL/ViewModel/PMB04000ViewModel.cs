using Global_PMCOMMON.DTOs.Response.Invoice_Type;
using Global_PMCOMMON.DTOs.Response.Property;
using Global_PMModel;
using PMB04000COMMON.DTO.DTOs;
using PMB04000COMMON.DTO.Utilities;
using R_BlazorFrontEnd.Exceptions;
using R_BlazorFrontEnd.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMB04000MODEL.ViewModel
{
    public class PMB04000ViewModel
    {
        #region From Back

        private readonly PMB04000Model _model = new PMB04000Model();
        private readonly GlobalFunctionModel _modelGlobalPM = new GlobalFunctionModel();
        public List<PropertyDTO> loPropertyList  = new List<PropertyDTO>();
        public List<InvoiceTypeDTO> loInvoiceType  = new List<InvoiceTypeDTO>();
        public ObservableCollection<PMB04000DTO> loOfficialReceipt = new ObservableCollection<PMB04000DTO>();
        #endregion

        #region For Front
        public PMB04000ParamDTO oParameterInvoiceReceipt = new PMB04000ParamDTO();
        public List<PeriodMonthDTO>? GetMonthList;
        public PeriodYearDTO oPeriodYearRange = new PeriodYearDTO();
        public string? cPeriodMonth;
        public int iPeriodYear = DateTime.Now.Year;
        public string? cPeriodParam;
        #endregion

        #region Program
        public async Task GetPropertyList()
        {
            R_Exception loEx = new R_Exception();
            try
            {
                var loResult = await _modelGlobalPM.PropertyListAsync();
                if (loResult.Data.Any())
                {
                    loPropertyList = new List<PropertyDTO>(loResult.Data);
                    if (string.IsNullOrEmpty(oParameterInvoiceReceipt.CPROPERTY_ID))
                    {
                        oParameterInvoiceReceipt.CPROPERTY_ID = loResult.Data.First().CPROPERTY_ID!;
                    }
                }
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }
            loEx.ThrowExceptionIfErrors();
        }
        public async Task GetInvoiceTypeList()
        {
            R_Exception loEx = new R_Exception();
            try
            {
                var loParameter = new InvoiceTypeParameterDTO
                {
                    CCLASS_APPLICATION = "BIMASAKTI",
                    CCLASS_ID = "_BS_INVOICE_TYPE",
                    CCLASS_RECID = "01,02,08,09"
                };

                var loResult = await _modelGlobalPM.InvoiceTypeAsync(loParameter);
                if (loResult.Data.Any())
                {
                    loInvoiceType = new List<InvoiceTypeDTO>(loResult.Data);
                    if (string.IsNullOrEmpty(oParameterInvoiceReceipt.CINVOICE_TYPE))
                    {
                        oParameterInvoiceReceipt.CINVOICE_TYPE = loResult.Data.First().CCODE!;
                    }
                }
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }
            loEx.ThrowExceptionIfErrors();
        }
        public async Task GetIOfficialReceiptList()
        {
            R_Exception loEx = new R_Exception();
            try
            {
                if (!string.IsNullOrEmpty(oParameterInvoiceReceipt.CPROPERTY_ID))
                {
                    var loResult = await _model.GetInvReceiptListAsync();
                    if (loResult.Data.Any())
                    {
                        foreach (var item in loResult.Data!)
                        {
                            item.DREF_DATE = ConvertStringToDateTimeFormat(item.CREF_DATE!)!;
                        }
                    }
                    loOfficialReceipt = new ObservableCollection<PMB04000DTO>(loResult.Data);
                }
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }
            loEx.ThrowExceptionIfErrors();
        }
        #endregion
        public void GetMonth()
        {
            GetMonthList = new List<PeriodMonthDTO>();
            for (int i = 1; i <= 12; i++)
            {
                string monthId = i.ToString("D2");
                PeriodMonthDTO month = new PeriodMonthDTO { Id = monthId };
                GetMonthList.Add(month);
            }
            cPeriodMonth = DateTime.Now.Month.ToString("D2");
        }
        public async Task GetPeriodRangeYear()
        {
            var loEx = new R_Exception();
            try
            {
                var loReturn = await _model.GetPeriodYearRangeAsync();
                if (loReturn != null)
                {
                    oPeriodYearRange = loReturn;
                }
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }
            loEx.ThrowExceptionIfErrors();
        }
        #region Utilities
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
