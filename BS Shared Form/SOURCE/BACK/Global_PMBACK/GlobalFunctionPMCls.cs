using Global_PMCOMMON.Logs;
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
using Global_PMCOMMON.DTOs.User_Param_Detail;

namespace Global_PMBACK
{
    public class GlobalFunctionPMCls
    {
        private LoggerGlobalFunctionPM _logger;
        private readonly ActivitySource _activitySource;

        public GlobalFunctionPMCls()
        {
            _logger = LoggerGlobalFunctionPM.R_GetInstanceLogger();
            _activitySource = GlobalFunctionPMActivity.R_GetInstanceActivitySource();
        }

        public GetUserParamDetailDTO GetUserParamDetailDb(GetUserParamDetailParameterDTO poEntity)
        {
            string lcMethodName = nameof(GetUserParamDetailDb);
            using Activity activity = _activitySource.StartActivity(lcMethodName)!;
            _logger.LogInfo(string.Format("START process method {0} on Cls", lcMethodName));

            var loEx = new R_Exception();
            GetUserParamDetailDTO? loResult = null;
            R_Db loDb;
            try
            {
                loDb = new R_Db();
                var loConn = loDb.GetConnection();
                var loCmd = loDb.GetCommand();

                var lcQuery = @"RSP_PM_GET_USER_PARAM_DETAIL";
                loCmd.CommandText = lcQuery;
                loCmd.CommandType = CommandType.StoredProcedure;

                loDb.R_AddCommandParameter(loCmd, "@CCOMPANY_ID", DbType.String, 20, poEntity.CCOMPANY_ID);
                loDb.R_AddCommandParameter(loCmd, "@CUSER_ID", DbType.String, 8, poEntity.CUSER_ID);
                loDb.R_AddCommandParameter(loCmd, "@CCODE", DbType.String, 8, poEntity.CCODE);

                var loDbParam = loCmd.Parameters.Cast<DbParameter>()
                    .Where(x => x != null && x.ParameterName.StartsWith("@"))
                    .ToDictionary(x => x.ParameterName, x => x.Value);
                _logger.LogDebug("{@ObjectQuery} {@Parameter}", loCmd.CommandText, loDbParam);

                var loReturnTemp = loDb.SqlExecQuery(loConn, loCmd, true);
                loResult = R_Utility.R_ConvertTo<GetUserParamDetailDTO>(loReturnTemp).ToList().FirstOrDefault() != null ?
                     R_Utility.R_ConvertTo<GetUserParamDetailDTO>(loReturnTemp).ToList().FirstOrDefault() :
                     new GetUserParamDetailDTO();
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
                _logger.LogError(loEx);
            }
            loEx.ThrowExceptionIfErrors();
            _logger.LogInfo(string.Format("END process method {0} on Cls", lcMethodName));
            return loResult!;
        }

    }
}
