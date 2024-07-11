using PMT01700COMMON.Logs;
using R_BackEnd;
using R_Common;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PMT01700COMMON.DTO._2._LOO._1._LOO___Offer_List;
using PMT01700COMMON.DTO.Utilities.ParamDb.LOO;
using PMT01700COMMON.DTO._1._Other_Unit_List;

namespace PMT01700BACK
{
    public class PMT01700LOO_OfferListCls
    {
        private readonly LoggerPMT01700? _logger;
        private readonly ActivitySource _activitySource;


        public PMT01700LOO_OfferListCls()
        {
            _logger = LoggerPMT01700.R_GetInstanceLogger();
            _activitySource = PMT01700Activity.R_GetInstanceActivitySource();
        }

        public List<PMT01700LOO_OfferList_OfferListDTO> GetOfferListDb(PMT01700LOO_UnitUtilities_ParameterDTO poParameter)
        {
            string? lcMethodName = nameof(GetOfferListDb);
            using Activity activity = _activitySource.StartActivity(lcMethodName)!;
            _logger!.LogInfo(string.Format("START process method {0} on Cls", lcMethodName));


            R_Exception loException = new R_Exception();
            List<PMT01700LOO_OfferList_OfferListDTO>? loReturn = null;
            string lcQuery;
            DbCommand loCommand;
            R_Db loDb;
            try
            {
                loDb = new();
                DbConnection? loConn = loDb.GetConnection();
                loCommand = loDb.GetCommand();
                lcQuery = "RSP_PM_GET_AGREEMENT_LIST";
                loCommand.CommandText = lcQuery;
                loCommand.CommandType = CommandType.StoredProcedure;

                loDb.R_AddCommandParameter(loCommand, "@CCOMPANY_ID", DbType.String, 20, poParameter.CCOMPANY_ID);
                loDb.R_AddCommandParameter(loCommand, "@CUSER_ID", DbType.String, 20, poParameter.CUSER_ID);
                loDb.R_AddCommandParameter(loCommand, "@CPROPERTY_ID", DbType.String, 20, poParameter.CPROPERTY_ID);
                loDb.R_AddCommandParameter(loCommand, "@CTRANS_CODE", DbType.String, 20, poParameter.CTRANS_CODE);

                var loDbParam = loCommand.Parameters.Cast<DbParameter>()
                    .Where(x => x != null && x.ParameterName.StartsWith("@"))
                    .ToDictionary(x => x.ParameterName, x => x.Value);
                _logger.LogDebug("{@ObjectQuery} {@Parameter}", loCommand.CommandText, loDbParam);


                var loDataTable = loDb.SqlExecQuery(loConn, loCommand, true);
                loReturn = R_Utility.R_ConvertTo<PMT01700LOO_OfferList_OfferListDTO>(loDataTable).ToList();

            }
            catch (Exception ex)
            {
                loException.Add(ex);
                _logger.LogError(loException);
            }

            if (loException.Haserror)
            {
                loException.ThrowExceptionIfErrors();
            }
            _logger.LogInfo(string.Format("END process method {0} on Cls", lcMethodName));

#pragma warning disable CS8603 // Possible null reference return.
            return loReturn;
        }

        public List<PMT01700LOO_OfferList_UnitListDTO> GetUnitListDb(PMT01700LOO_UnitUtilities_ParameterDTO poParameter)
        {
            string? lcMethodName = nameof(GetUnitListDb);
            using Activity activity = _activitySource.StartActivity(lcMethodName)!;
            _logger!.LogInfo(string.Format("START process method {0} on Cls", lcMethodName));


            R_Exception loException = new R_Exception();
            List<PMT01700LOO_OfferList_UnitListDTO>? loReturn = null;
            string lcQuery;
            DbCommand loCommand;
            R_Db loDb;
            try
            {
                loDb = new();
                DbConnection? loConn = loDb.GetConnection();
                loCommand = loDb.GetCommand();
                lcQuery = "RSP_PM_GET_AGREEMENT_UNIT_INFO_LIST";
                loCommand.CommandText = lcQuery;
                loCommand.CommandType = CommandType.StoredProcedure;

                loDb.R_AddCommandParameter(loCommand, "@CCOMPANY_ID", DbType.String, 20, poParameter.CCOMPANY_ID);
                loDb.R_AddCommandParameter(loCommand, "@CPROPERTY_ID", DbType.String, 20, poParameter.CPROPERTY_ID);
                loDb.R_AddCommandParameter(loCommand, "@CDEPT_CODE", DbType.String, 20, poParameter.CDEPT_CODE);
                loDb.R_AddCommandParameter(loCommand, "@CREF_NO", DbType.String, 30, poParameter.CREF_NO);
                loDb.R_AddCommandParameter(loCommand, "@CTRANS_CODE", DbType.String, 10, poParameter.CTRANS_CODE);
                loDb.R_AddCommandParameter(loCommand, "@CUSER_ID", DbType.String, 20, poParameter.CUSER_ID);

                var loDbParam = loCommand.Parameters.Cast<DbParameter>()
                    .Where(x => x != null && x.ParameterName.StartsWith("@"))
                    .ToDictionary(x => x.ParameterName, x => x.Value);
                _logger.LogDebug("{@ObjectQuery} {@Parameter}", loCommand.CommandText, loDbParam);


                var loDataTable = loDb.SqlExecQuery(loConn, loCommand, true);
                loReturn = R_Utility.R_ConvertTo<PMT01700LOO_OfferList_UnitListDTO>(loDataTable).ToList();

            }
            catch (Exception ex)
            {
                loException.Add(ex);
                _logger.LogError(loException);
            }

            if (loException.Haserror)
            {
                loException.ThrowExceptionIfErrors();
            }
            _logger.LogInfo(string.Format("END process method {0} on Cls", lcMethodName));

#pragma warning disable CS8603 // Possible null reference return.
            return loReturn;
        }
        public bool ProcessAgreement(PMT01700LOO_ProcessOffer_DTO poEntity)
        {
            string? lcMethodName = nameof(ProcessAgreement);
            using Activity activity = _activitySource.StartActivity(lcMethodName)!;
            _logger!.LogInfo(string.Format("START process method {0} on Cls", lcMethodName));
            bool llReturn;

            R_Exception loException = new R_Exception();
            string lcQuery;
            DbCommand loCommand;
            R_Db loDb;
            try
            {
                loDb = new();
                DbConnection? loConn = loDb.GetConnection();
                loCommand = loDb.GetCommand();
                lcQuery = "RSP_PM_UPDATE_AGREEMENT_TRANS_STS";
                loCommand.CommandText = lcQuery;
                loCommand.CommandType = CommandType.StoredProcedure;

                loDb.R_AddCommandParameter(loCommand, "@CCOMPANY_ID", DbType.String, 20, poEntity.CCOMPANY_ID);
                loDb.R_AddCommandParameter(loCommand, "@CPROPERTY_ID", DbType.String, 20, poEntity.CPROPERTY_ID);
                loDb.R_AddCommandParameter(loCommand, "@CDEPT_CODE", DbType.String, 20, poEntity.CDEPT_CODE);
                loDb.R_AddCommandParameter(loCommand, "@CTRANS_CODE", DbType.String, 10, poEntity.CTRANS_CODE);
                loDb.R_AddCommandParameter(loCommand, "@CREF_NO", DbType.String, 30, poEntity.CREF_NO);
                loDb.R_AddCommandParameter(loCommand, "@CUSER_ID", DbType.String, 20, poEntity.CUSER_ID);
                loDb.R_AddCommandParameter(loCommand, "@CNEW_STATUS", DbType.String, 20, poEntity.CNEW_STATUS);

                var loDbParam = loCommand.Parameters.Cast<DbParameter>()
                    .Where(x => x != null && x.ParameterName.StartsWith("@"))
                    .ToDictionary(x => x.ParameterName, x => x.Value);
                _logger.LogDebug("{@ObjectQuery} {@Parameter}", loCommand.CommandText, loDbParam);


                var loDataTable = loDb.SqlExecNonQuery(loConn, loCommand, true);
                llReturn = true;
             //   R_Utility.R_ConvertTo<PMT01700LOO_OfferList_UnitListDTO>(loDataTable).ToList();

            }
            catch (Exception ex)
            {
                loException.Add(ex);
                 llReturn = false;
                _logger.LogError(loException);
            }
            loException.ThrowExceptionIfErrors();
            _logger.LogInfo(string.Format("END process method {0} on Cls", lcMethodName));
            return llReturn;
        }
     
    }
}
