using Lookup_GSCOMMON.DTOs;
using R_BlazorFrontEnd;
using R_BlazorFrontEnd.Exceptions;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Lookup_GSModel.ViewModel
{
    public class LookupGSL02700ViewModel : R_ViewModel<GSL02700DTO>
    {
        private PublicLookupModel _model = new PublicLookupModel();
        private PublicLookupRecordModel _modelRecord = new PublicLookupRecordModel();

        public ObservableCollection<GSL02700DTO> OtherUnitGrid = new ObservableCollection<GSL02700DTO>();

        public async Task GetOtherUnitList(GSL02700ParameterDTO poParameter)
        {
            var loEx = new R_Exception();

            try
            {
                var loResult = await _model.GSL02700GetOtherUnitListAsync(poParameter);

                OtherUnitGrid = new ObservableCollection<GSL02700DTO>(loResult);
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            loEx.ThrowExceptionIfErrors();
        }
        public async Task<GSL02700DTO> GetOtherUnit(GSL02700ParameterDTO poParameter)
        {
            var loEx = new R_Exception();
            GSL02700DTO loRtn = null;
            try
            {
                var loResult = await _modelRecord.GSL02700GetOtherUnitAsync(poParameter);
                loRtn = loResult;
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            loEx.ThrowExceptionIfErrors();
            return loRtn;
        }
    }
}
