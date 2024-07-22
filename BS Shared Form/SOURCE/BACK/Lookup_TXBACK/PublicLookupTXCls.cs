using Lookup_TXCOMMON.DTOs.TXL00100;
using Lookup_TXCOMMON.DTOs.Utilities;
using Lookup_TXCOMMON.Loggers;
using R_BackEnd;
using R_Common;
using System.Data;
using System.Data.Common;
using System.Diagnostics;

namespace Lookup_TXBACK
{
    public class PublicLookupTXCls
    {
        private LoggerLookupTX _loggerLookup;
        private readonly ActivitySource _activitySource;
        public PublicLookupTXCls()
        {
            _loggerLookup = LoggerLookupTX.R_GetInstanceLogger();
            _activitySource = LookupTXActivity.R_GetInstanceActivitySource();
        }

        public List<TXL00100DTO> TXL00100BranchLookUpDb(TXLParameterCompanyAndUserDTO poParameterInternal)
        {
            string lcMethodName = nameof(TXL00100BranchLookUpDb);
            using Activity activity = _activitySource.StartActivity(lcMethodName)!;
            _loggerLookup.LogInfo(string.Format("START process method {0} on Cls", lcMethodName));

            var loEx = new R_Exception();
            List<TXL00100DTO>? loResult = null;
            R_Db loDb;
            try
            {
                loDb = new R_Db();
                var loConn = loDb.GetConnection();
                var loCmd = loDb.GetCommand();

                var lcQuery = @"RSP_TX_GET_BRANCH_LIST";
                loCmd.CommandText = lcQuery;
                loCmd.CommandType = CommandType.StoredProcedure;

                loDb.R_AddCommandParameter(loCmd, "@CCOMPANY_ID", DbType.String, 20, poParameterInternal.CCOMPANY_ID);
                loDb.R_AddCommandParameter(loCmd, "@CUSER_ID", DbType.String, 8, poParameterInternal.CUSER_ID);

                var loDbParam = loCmd.Parameters.Cast<DbParameter>()
                    .Where(x => x != null && x.ParameterName.StartsWith("@"))
                    .ToDictionary(x => x.ParameterName, x => x.Value);
                _loggerLookup.LogDebug("{@ObjectQuery} {@Parameter}", loCmd.CommandText, loDbParam);

                var loReturnTemp = loDb.SqlExecQuery(loConn, loCmd, true);
                loResult = R_Utility.R_ConvertTo<TXL00100DTO>(loReturnTemp).ToList();
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }

            loEx.ThrowExceptionIfErrors();
            _loggerLookup.LogInfo(string.Format("END process method {0} on Cls", lcMethodName));
            return loResult!;
        }


    }
}
