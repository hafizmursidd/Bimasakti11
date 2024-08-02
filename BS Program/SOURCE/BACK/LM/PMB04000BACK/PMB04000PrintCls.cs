using PMB04000COMMON.DTO.DTOs;
using PMB04000COMMON.DTO.Utilities;
using PMB04000COMMON.Logs;
using PMB04000COMMON.Print;
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
using PMB04000COMMON.Print.Utility_DTO;
using PMB04000COMMON.Print.Param_DTO;

namespace PMB04000BACK
{
    public class PMB04000PrintCls
    {
        private readonly LoggerPMB04000? _logger;
        private readonly ActivitySource _activitySource;

        public PMB04000PrintCls()
        {
            _logger = LoggerPMB04000.R_GetInstanceLogger();
            _activitySource = PMB04000Activity.R_GetInstanceActivitySource();
        }

        public List<PMB04000DataReportDTO> GetReportReceiptData(PMB04000ParamReportDTO poParameter)
        {
            string? lcMethodName = nameof(GetReportReceiptData);
            using Activity activity = _activitySource.StartActivity(lcMethodName)!;
            _logger!.LogInfo(string.Format("START process method {0} on Cls", lcMethodName));

            R_Exception loException = new();
            List<PMB04000DataReportDTO>? loReturn = null;
            string lcQuery;
            DbCommand loCommand;
            R_Db loDb;
            try
            {
                loDb = new();
                DbConnection? loConn = loDb.GetConnection(R_Db.eDbConnectionStringType.ReportConnectionString);
                loCommand = loDb.GetCommand();
                lcQuery = "RSP_PM_PRINT_RECEIPT";
                loCommand.CommandText = lcQuery;
                loCommand.CommandType = CommandType.StoredProcedure;

                loDb.R_AddCommandParameter(loCommand, "@CCOMPANY_ID", DbType.String, 20, poParameter.CCOMPANY_ID);
                loDb.R_AddCommandParameter(loCommand, "@CPROPERTY", DbType.String, 20, poParameter.CPROPERTY_ID);
                loDb.R_AddCommandParameter(loCommand, "@CDEPT_CODE", DbType.String, 20, poParameter.CDEPT_CODE);
                loDb.R_AddCommandParameter(loCommand, "@CREF_NO", DbType.String, 20, poParameter.CREF_NO);
                loDb.R_AddCommandParameter(loCommand, "@CUSER_ID", DbType.String, 20, poParameter.CUSER_ID);
                loDb.R_AddCommandParameter(loCommand, "@CLANG_ID", DbType.String, 3, poParameter.CLANG_ID);
                loDb.R_AddCommandParameter(loCommand, "@LPRINT ", DbType.Boolean, 2, poParameter.LPRINT);

                var loDbParam = loCommand.Parameters.Cast<DbParameter>()
                    .Where(x => x != null && x.ParameterName.StartsWith("@"))
                    .ToDictionary(x => x.ParameterName, x => x.Value);
                _logger.LogDebug("{@ObjectQuery} {@Parameter}", loCommand.CommandText, loDbParam);

                var loDataTable = loDb.SqlExecQuery(loConn, loCommand, true);
                loReturn = R_Utility.R_ConvertTo<PMB04000DataReportDTO>(loDataTable).ToList();
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
#pragma warning restore CS8603 // Possible null reference return.
        }
        public PMB04000BaseHeaderDTO GetLogoCompany(PMB04000ParamReportDTO poParameter)
        {
            using Activity activity = _activitySource.StartActivity("GetLogoCompany")!;
            var loEx = new R_Exception();
            PMB04000BaseHeaderDTO loResult = null;

            try
            {
                var loDb = new R_Db();
                var loConn = loDb.GetConnection(R_Db.eDbConnectionStringType.ReportConnectionString);
                var loCmd = loDb.GetCommand();
                var lcQuery = "SELECT dbo.RFN_GET_COMPANY_LOGO(@CCOMPANY_ID) as CLOGO";
                loCmd.CommandText = lcQuery;
                loCmd.CommandType = CommandType.Text;
                loDb.R_AddCommandParameter(loCmd, "@CCOMPANY_ID", DbType.String, 15, poParameter.CCOMPANY_ID);

                //Debug Logs
                var loDbParam = loCmd.Parameters.Cast<DbParameter>()
                .Where(x => x != null && x.ParameterName.StartsWith("@")).Select(x => x.Value);
                _logger!.LogDebug("SELECT dbo.RFN_GET_COMPANY_LOGO({@CCOMPANY_ID}) as CLOGO", loDbParam);

                var loDataTable = loDb.SqlExecQuery(loConn, loCmd, true);
                loResult = R_Utility.R_ConvertTo<PMB04000BaseHeaderDTO>(loDataTable).FirstOrDefault()!;
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
                _logger!.LogError(loEx);
            }

            loEx.ThrowExceptionIfErrors();
            return loResult!;
        }

    }
}
