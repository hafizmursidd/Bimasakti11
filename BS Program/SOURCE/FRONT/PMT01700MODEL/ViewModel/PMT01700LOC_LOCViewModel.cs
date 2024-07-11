using PMT01700COMMON.DTO._2._LOO._2._LOO___Offer;
using PMT01700COMMON.DTO._3._LOC._2._LOC;
using PMT01700COMMON.DTO.Utilities;
using PMT01700COMMON.DTO.Utilities.Front;
using R_BlazorFrontEnd;
using R_BlazorFrontEnd.Exceptions;
using R_BlazorFrontEnd.Interfaces;
using R_CommonFrontBackAPI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace PMT01700MODEL.ViewModel
{
    public class PMT01700LOC_LOCViewModel : R_ViewModel<PMT010700_LOC_LOC_SelectedLOCDTO>
    {
        #region From Back

        private readonly PMT01700LOC_LOCModel _model = new PMT01700LOC_LOCModel();

        public PMT010700_LOC_LOC_SelectedLOCDTO oEntity = new PMT010700_LOC_LOC_SelectedLOCDTO();
        public PMT01700VarGsmTransactionCodeDTO oVarGSMTransactionCode = new PMT01700VarGsmTransactionCodeDTO();
        
        public PMT010700_LOC_LOC_SelectedLOCDTO oTempDataForAdd = new PMT010700_LOC_LOC_SelectedLOCDTO();

        #endregion


        #region For Front

        public PMT01700ParameterFrontChangePageDTO oParameter = new PMT01700ParameterFrontChangePageDTO();
        public bool lControlCRUDMode = true;
        public bool lControlExistingTenant = true;
        public PMT01700ControlYMD oControlYMD = new PMT01700ControlYMD();
        public List<PMT01700ComboBoxDTO> loComboBoxDataCLeaseMode { get; set; } = new List<PMT01700ComboBoxDTO>();
        public List<PMT01700ComboBoxDTO> loComboBoxDataCChargesMode { get; set; } = new List<PMT01700ComboBoxDTO>();

        #endregion

        #region LOC - LOC

        #region Core

        public async Task GetEntity(PMT010700_LOC_LOC_SelectedLOCDTO poEntity)
        {
            var loEx = new R_Exception();

            try
            {
                var loResult = await _model.R_ServiceGetRecordAsync(poEntity);
                Console.WriteLine(loResult);
                loResult.DREF_DATE = ConvertStringToDateTimeFormat(loResult.CREF_DATE);
                loResult.DFOLLOW_UP_DATE = ConvertStringToDateTimeFormat(loResult.CFOLLOW_UP_DATE);
                loResult.DSTART_DATE = ConvertStringToDateTimeFormat(loResult.CSTART_DATE);
                loResult.DEND_DATE = ConvertStringToDateTimeFormat(loResult.CEND_DATE);
              //  loResult.DHAND_OVER_DATE = ConvertStringToDateTimeFormat(loResult.CHAND_OVER_DATE);

                oEntity = loResult;
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            loEx.ThrowExceptionIfErrors();
        }

        public async Task ServiceSave(PMT010700_LOC_LOC_SelectedLOCDTO poNewEntity, eCRUDMode peCRUDMode)
        {
            var loEx = new R_Exception();

            try
            {
                // set Add PropertyId and Charges Type
                if (eCRUDMode.AddMode == peCRUDMode)
                {
                    poNewEntity.CPROPERTY_ID = oParameter.CPROPERTY_ID;

                }

                poNewEntity.CFOLLOW_UP_DATE = ConvertDateTimeToStringFormat(poNewEntity.DFOLLOW_UP_DATE);
                poNewEntity.CSTART_DATE = ConvertDateTimeToStringFormat(poNewEntity.DSTART_DATE);
                poNewEntity.CEND_DATE = ConvertDateTimeToStringFormat(poNewEntity.DEND_DATE);
                poNewEntity.CREF_DATE = ConvertDateTimeToStringFormat(poNewEntity.DREF_DATE);
               
                poNewEntity.CSTART_TIME = ConvertTimeToStringFormat(poNewEntity.DSTART_TIME);
                poNewEntity.CEND_TIME = ConvertTimeToStringFormat(poNewEntity.DEND_TIME);

                var loResult = await _model.R_ServiceSaveAsync(poNewEntity, peCRUDMode);

                loResult.DREF_DATE = ConvertStringToDateTimeFormat(loResult.CREF_DATE);
                loResult.DFOLLOW_UP_DATE = ConvertStringToDateTimeFormat(loResult.CFOLLOW_UP_DATE);
                loResult.DSTART_DATE = ConvertStringToDateTimeFormat(loResult.CSTART_DATE);
                loResult.DEND_DATE = ConvertStringToDateTimeFormat(loResult.CEND_DATE);
                loResult.DSTART_TIME = ConvertStringToTimeFormat(loResult.CSTART_TIME, loResult.DSTART_DATE);
                loResult.DEND_TIME = ConvertStringToTimeFormat(loResult.CEND_TIME, loResult.DEND_DATE);



                oEntity = loResult;
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            loEx.ThrowExceptionIfErrors();
        }

        public async Task ServiceDelete(PMT010700_LOC_LOC_SelectedLOCDTO poEntity)
        {
            var loEx = new R_Exception();

            try
            {
                // Validation Before Delete
                await _model.R_ServiceDeleteAsync(poEntity);
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            loEx.ThrowExceptionIfErrors();
        }

        #endregion

        public async Task GetVAR_GSM_TRANSACTION_CODE()
        {
            R_Exception loEx = new R_Exception();
            try
            {
                var loResult = await _model.GetVAR_GSM_TRANSACTION_CODEAsync();
                oVarGSMTransactionCode = loResult;
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }
            loEx.ThrowExceptionIfErrors();
        }

        #endregion
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
        private DateTime? ConvertStringToTimeFormat(string? pcEntity, DateTime? date)
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
                if (DateTime.TryParseExact(pcEntity, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
                {
                    // Mengembalikan DateTime dengan waktu yang diberikan dan tanggal hari ini
                    DateTime DDate = (DateTime)date!;
                    return new DateTime(DDate.Year, DDate.Month, DDate.Day, result.Hour, result.Minute, 0);
                }
                else
                {
                    // Jika parsing gagal, kembalikan null
                    return null;
                }

            }
        }
        private string? ConvertTimeToStringFormat(DateTime? ptEntity)
        {
            if (!ptEntity.HasValue || ptEntity.Value == null)
            {
                // Jika ptEntity adalah null atau DateTime.MinValue, kembalikan null
                return null;
            }
            else
            {
                // Format DateTime ke string "yyyyMMdd"
                return ptEntity.Value.ToString("HH:mm");
            }
        }

        #endregion
    }
}
