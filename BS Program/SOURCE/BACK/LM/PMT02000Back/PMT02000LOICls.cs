using PMT02000COMMON.Logs;
using R_BackEnd;
using R_Common;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PMT02000COMMON.Utility;
using PMT02000COMMON.LOI_List;
using R_CommonFrontBackAPI;

namespace PMT02000Back
{
    public class PMT02000LOICls : R_BusinessObject<PMT02000LOIHeader_DetailDTO>
    {
        private LoggerPMT02000 _logger;
        private readonly ActivitySource _activitySource;

        public PMT02000LOICls()
        {
            _logger = LoggerPMT02000.R_GetInstanceLogger();
            _activitySource = PMT02000Activity.R_GetInstanceActivitySource();
        }

        public List<PMT02000PropertyDTO> GetAllPropertyList(PMT02000DBParameter poParameter)
        {
            string lcMethodName = nameof(GetAllPropertyList);
            using Activity activity = _activitySource.StartActivity(lcMethodName)!;
            _logger.LogInfo(string.Format("START process method {0} on Cls", lcMethodName));
            R_Exception loException = new R_Exception();
            List<PMT02000PropertyDTO>? loReturn = null;
            R_Db loDb;
            DbCommand loCommand;

            try
            {
                loDb = new R_Db();
                var loConn = loDb.GetConnection();
                loCommand = loDb.GetCommand();

                var lcQuery = @"RSP_GS_GET_PROPERTY_LIST";
                loCommand.CommandText = lcQuery;
                loCommand.CommandType = CommandType.StoredProcedure;
                loDb.R_AddCommandParameter(loCommand, "@CCOMPANY_ID", DbType.String, 50, poParameter.CCOMPANY_ID);
                loDb.R_AddCommandParameter(loCommand, "@CUSER_ID", DbType.String, 50, poParameter.CUSER_ID);

                var loDbParam = loCommand.Parameters.Cast<DbParameter>()
                    .Where(x => x != null && x.ParameterName.StartsWith("@"))
                    .ToDictionary(x => x.ParameterName, x => x.Value);
                _logger.LogDebug("{@ObjectQuery} {@Parameter}", loCommand.CommandText, loDbParam);

                var loReturnTemp = loDb.SqlExecQuery(loConn, loCommand, true);
                loReturn = R_Utility.R_ConvertTo<PMT02000PropertyDTO>(loReturnTemp).ToList();
            }
            catch (Exception ex)
            {
                loException.Add(ex);
                _logger.LogError(loException);
            }
            loException.ThrowExceptionIfErrors();
            _logger.LogInfo(string.Format("END process method {0} on Cls", lcMethodName));

#pragma warning disable CS8603 // Possible null reference return.
            return loReturn;
#pragma warning restore CS8603 // Possible null reference return.
        }

        public List<PMT02000LOIDTO> GetLOIList(PMT02000DBParameter poParameter)
        {
            string? lcMethodName = nameof(GetLOIList);
            using Activity activity = _activitySource.StartActivity(lcMethodName)!;
            _logger.LogInfo(string.Format("START process method {0} on Cls", lcMethodName));


            R_Exception loException = new R_Exception();
            List<PMT02000LOIDTO>? loReturn = null;
            string lcQuery;
            DbCommand loCommand;
            R_Db loDb;
            try
            {
                loDb = new();
                DbConnection? loConn = loDb.GetConnection();
                loCommand = loDb.GetCommand();
                lcQuery = "RSP_PM_GET_HANDOVER_LIST";
                loCommand.CommandText = lcQuery;
                loCommand.CommandType = CommandType.StoredProcedure;

                loDb.R_AddCommandParameter(loCommand, "@CCOMPANY_ID", DbType.String, 20, poParameter.CCOMPANY_ID);
                loDb.R_AddCommandParameter(loCommand, "@CPROPERTY_ID", DbType.String, 20, poParameter.CPROPERTY_ID);
                loDb.R_AddCommandParameter(loCommand, "@CTRANS_CODE", DbType.String, 6, poParameter.CTRANS_CODE);
                loDb.R_AddCommandParameter(loCommand, "@CLANG_ID", DbType.String, 3, poParameter.CLANG_ID);

                var loDbParam = loCommand.Parameters.Cast<DbParameter>()
                    .Where(x => x != null && x.ParameterName.StartsWith("@"))
                    .ToDictionary(x => x.ParameterName, x => x.Value);
                _logger.LogDebug("{@ObjectQuery} {@Parameter}", loCommand.CommandText, loDbParam);


                var loDataTable = loDb.SqlExecQuery(loConn, loCommand, true);
                loReturn = R_Utility.R_ConvertTo<PMT02000LOIDTO>(loDataTable).ToList();

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
        public PMT02000LOIHeader GetHeader(PMT02000DBParameter poParameter)
        {
            string lcMethodName = nameof(GetHeader);
            using Activity activity = _activitySource.StartActivity(lcMethodName)!;
            _logger.LogInfo(string.Format("START process method {0} on Cls", lcMethodName));

            R_Exception loException = new R_Exception();
            PMT02000LOIHeader? loReturn = null;
            DbConnection? loConn = null;
            DbCommand? loCommand = null;
            R_Db loDb;
            try
            {
                var lcQuery = "RSP_PM_GET_HANDOVER_DETAIL";
                loDb = new R_Db();
                loCommand = loDb.GetCommand();
                loConn = loDb.GetConnection();
                loCommand.CommandText = lcQuery;
                loCommand.CommandType = CommandType.StoredProcedure;

                loDb.R_AddCommandParameter(loCommand, "@CCOMPANY_ID", DbType.String, 20, poParameter.CCOMPANY_ID);
                loDb.R_AddCommandParameter(loCommand, "@CPROPERTY_ID", DbType.String, 20, poParameter.CPROPERTY_ID);
                loDb.R_AddCommandParameter(loCommand, "@CTRANS_CODE", DbType.String, 6, poParameter.CTRANS_CODE);
                loDb.R_AddCommandParameter(loCommand, "@CLANG_ID", DbType.String, 3, poParameter.CLANG_ID);
                loDb.R_AddCommandParameter(loCommand, "@CDEPT_CODE", DbType.String, 20, poParameter.CDEPT_CODE);
                loDb.R_AddCommandParameter(loCommand, "@CREF_NO", DbType.String, 30, poParameter.CREF_NO);
                loDb.R_AddCommandParameter(loCommand, "@CBUILDING_ID", DbType.String, 20, poParameter.CBUILDING_ID);
                loDb.R_AddCommandParameter(loCommand, "@CFLOOR_ID", DbType.String, 20, poParameter.CFLOOR_ID);
                loDb.R_AddCommandParameter(loCommand, "@CUNIT_ID ", DbType.String, 20, poParameter.CUNIT_ID);
                loDb.R_AddCommandParameter(loCommand, "@CSAVEMODE ", DbType.String, 7, poParameter.CSAVEMODE);

                var loDbParam = loCommand.Parameters.Cast<DbParameter>()
                    .Where(x => x != null && x.ParameterName.StartsWith("@"))
                    .ToDictionary(x => x.ParameterName, x => x.Value);
                _logger.LogDebug("{@ObjectQuery} {@Parameter}", loCommand.CommandText, loDbParam);

                var loReturnTemp = loDb.SqlExecQuery(loConn, loCommand, false);
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.

                loReturn = R_Utility.R_ConvertTo<PMT02000LOIHeader>(loReturnTemp).ToList().FirstOrDefault() ?? new PMT02000LOIHeader();
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.


            }
            catch (Exception ex)
            {
                loException.Add(ex);
                _logger.LogError(loException);
            }
            _logger.LogInfo("End process method GetHeader on Cls");
            return loReturn!;
        }

        #region GETDETAIL LOI LIST
        public List<PMT02000LOIDetailListDTO> GetDetailListLOI(PMT02000DBParameter poParameter)
        {
            string? lcMethodName = nameof(GetDetailListLOI);
            using Activity activity = _activitySource.StartActivity(lcMethodName)!;
            _logger.LogInfo(string.Format("START process method {0} on Cls", lcMethodName));

            R_Exception loException = new R_Exception();
            List<PMT02000LOIDetailListDTO>? loReturn = null;
            string lcQuery;
            DbCommand loCommand;
            R_Db loDb;
            try
            {
                loDb = new();
                DbConnection? loConn = loDb.GetConnection();
                loCommand = loDb.GetCommand();
                lcQuery = "RSP_PM_GET_HANDOVER_UTILITY";
                loCommand.CommandText = lcQuery;
                loCommand.CommandType = CommandType.StoredProcedure;

                loDb.R_AddCommandParameter(loCommand, "@CCOMPANY_ID", DbType.String, 20, poParameter.CCOMPANY_ID);
                loDb.R_AddCommandParameter(loCommand, "@CPROPERTY_ID", DbType.String, 20, poParameter.CPROPERTY_ID);
                loDb.R_AddCommandParameter(loCommand, "@CDEPT_CODE", DbType.String, 20, poParameter.CDEPT_CODE);
                loDb.R_AddCommandParameter(loCommand, "@CTRANS_CODE", DbType.String, 6, poParameter.CTRANS_CODE);
                loDb.R_AddCommandParameter(loCommand, "@CREF_NO", DbType.String, 30, poParameter.CREF_NO);
                loDb.R_AddCommandParameter(loCommand, "@CBUILDING_ID", DbType.String, 20, poParameter.CBUILDING_ID);
                loDb.R_AddCommandParameter(loCommand, "@CFLOOR_ID", DbType.String, 20, poParameter.CFLOOR_ID);
                loDb.R_AddCommandParameter(loCommand, "@CUNIT_ID ", DbType.String, 20, poParameter.CUNIT_ID);
                loDb.R_AddCommandParameter(loCommand, "@CLANG_ID", DbType.String, 3, poParameter.CLANG_ID);

                var loDbParam = loCommand.Parameters.Cast<DbParameter>()
                     .Where(x => x != null && x.ParameterName.StartsWith("@"))
                     .ToDictionary(x => x.ParameterName, x => x.Value);
                _logger.LogDebug("{@ObjectQuery} {@Parameter}", loCommand.CommandText, loDbParam);


                var loDataTable = loDb.SqlExecQuery(loConn, loCommand, false);
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                loReturn = R_Utility.R_ConvertTo<PMT02000LOIDetailListDTO>(loDataTable).ToList();
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

            }
            catch (Exception ex)
            {
                loException.Add(ex);
                _logger.LogError(loException);
            }
            loException.ThrowExceptionIfErrors();
            _logger.LogInfo("End process method GetDetailListLOI on Cls");

            return loReturn!;
        }
        #endregion
        protected override PMT02000LOIHeader_DetailDTO R_Display(PMT02000LOIHeader_DetailDTO poEntity)
        {

            string lcMethodName = nameof(R_Display);
            using Activity activity = _activitySource.StartActivity(lcMethodName)!;
            _logger.LogInfo(string.Format("START process method {0} on Cls", lcMethodName));

            R_Exception loException = new R_Exception();
            PMT02000LOIHeader_DetailDTO? loReturn = null;
            DbConnection? loConn = null;
            DbCommand? loCommand = null;
            R_Db loDb;
            try
            {
                var lcQuery = "RSP_PM_GET_HANDOVER_DETAIL";
                loDb = new R_Db();
                loCommand = loDb.GetCommand();
                loConn = loDb.GetConnection();
                loCommand.CommandText = lcQuery;
                loCommand.CommandType = CommandType.StoredProcedure;

                loDb.R_AddCommandParameter(loCommand, "@CCOMPANY_ID", DbType.String, 20, poEntity.CCOMPANY_ID);
                loDb.R_AddCommandParameter(loCommand, "@CPROPERTY_ID", DbType.String, 20, poEntity.CPROPERTY_ID);
                loDb.R_AddCommandParameter(loCommand, "@CTRANS_CODE", DbType.String, 6, poEntity.CTRANS_CODE);
                loDb.R_AddCommandParameter(loCommand, "@CLANG_ID", DbType.String, 3, poEntity.CLANG_ID);
                loDb.R_AddCommandParameter(loCommand, "@CDEPT_CODE", DbType.String, 20, poEntity.CDEPT_CODE);
                loDb.R_AddCommandParameter(loCommand, "@CREF_NO", DbType.String, 30, poEntity.CREF_NO);
                loDb.R_AddCommandParameter(loCommand, "@CBUILDING_ID", DbType.String, 20, poEntity.CBUILDING_ID);
                loDb.R_AddCommandParameter(loCommand, "@CFLOOR_ID", DbType.String, 20, poEntity.CFLOOR_ID);
                loDb.R_AddCommandParameter(loCommand, "@CUNIT_ID ", DbType.String, 20, poEntity.CUNIT_ID);
                loDb.R_AddCommandParameter(loCommand, "@CSAVEMODE ", DbType.String, 7, poEntity.CSAVEMODE);

                var loDbParam = loCommand.Parameters.Cast<DbParameter>()
                    .Where(x => x != null && x.ParameterName.StartsWith("@"))
                    .ToDictionary(x => x.ParameterName, x => x.Value);
                _logger.LogDebug("{@ObjectQuery} {@Parameter}", loCommand.CommandText, loDbParam);

                var loReturnTemp = loDb.SqlExecQuery(loConn, loCommand, false);
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.

                loReturn = R_Utility.R_ConvertTo<PMT02000LOIHeader_DetailDTO>(loReturnTemp).ToList().FirstOrDefault() ?? new PMT02000LOIHeader_DetailDTO();
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.


            }
            catch (Exception ex)
            {
                loException.Add(ex);
                _logger.LogError(loException);
            }
            _logger.LogInfo("End process method GetHeader on Cls");
            return loReturn!;
        }

        protected override void R_Saving(PMT02000LOIHeader_DetailDTO poNewEntity, eCRUDMode poCRUDMode)
        {
            string lcMethodName = nameof(R_Saving);
            using Activity activity = _activitySource.StartActivity(lcMethodName)!;
            _logger.LogInfo(string.Format("START process method {0} on Cls", lcMethodName));

            R_Exception loException = new R_Exception();
            string lcQuery = null;
            R_Db loDb;
            DbCommand loCommand;
            DbConnection loConn = null;
            string lcAction = null;
            try
            {
                loDb = new R_Db();
                loConn = loDb.GetConnection();
                R_ExternalException.R_SP_Init_Exception(loConn);
                loCommand = loDb.GetCommand();

                switch (poCRUDMode)
                {
                    case eCRUDMode.AddMode:
                        lcAction = "ADD";
                        break;

                    case eCRUDMode.EditMode:
                        lcAction = "EDIT";
                        break;
                }
                var loListDetail = poNewEntity.ListDetail;

                List<PMT02000LOIDetailListTempTableDTO> loListTempTable = null;
                #region Convert to Tem Table
                if (loListDetail != null)
                {
                    loListTempTable = loListDetail
                         .Select(x => new PMT02000LOIDetailListTempTableDTO
                         {
                             CCHARGES_TYPE = x.CCHARGES_TYPE,
                             CCHARGES_ID = x.CCHARGES_ID,
                             CMETER_NO = x.CMETER_NO,
                             IMETER_START = x.IMETER_START,
                             CSTART_MONTH = x.CMONTH,
                             CSTART_YEAR = x.CYEAR
                         }).ToList();
                }
                #endregion

                #region TEMP TABLE

                lcQuery = $"CREATE TABLE #GRID_UTILITY " +
                          $"(CCHARGES_TYPE VARCHAR(2), " +
                          $"CCHARGES_ID VARCHAR(20), " +
                          $"CMETER_NO VARCHAR(50), " +
                          $"IMETER_START INT, " +
                          $"CSTART_YEAR VARCHAR(4), " +
                          $"CSTART_MONTH VARCHAR(50) )";

                _logger.LogDebug("{@ObjectQuery} ", lcQuery);

                loDb.SqlExecNonQuery(lcQuery, loConn, false);

                loDb.R_BulkInsert((SqlConnection)loConn, "#GRID_UTILITY", loListTempTable);

                #endregion
                lcQuery = "RSP_PM_MAINTAIN_HANDOVER";
                loCommand.CommandText = lcQuery;
                loCommand.CommandType = CommandType.StoredProcedure;

                loDb.R_AddCommandParameter(loCommand, "@CCOMPANY_ID", DbType.String, 8, poNewEntity.CCOMPANY_ID);
                loDb.R_AddCommandParameter(loCommand, "@CPROPERTY_ID", DbType.String, 20, poNewEntity.CPROPERTY_ID);
                loDb.R_AddCommandParameter(loCommand, "@CTRANS_CODE", DbType.String, 10, poNewEntity.VAR_TRANS_CODE); //
                loDb.R_AddCommandParameter(loCommand, "@CREF_NO", DbType.String, 30, poNewEntity.CHO_REF_NO);
                loDb.R_AddCommandParameter(loCommand, "@CREF_DATE", DbType.String, 8, poNewEntity.CHO_REF_DATE);
                loDb.R_AddCommandParameter(loCommand, "@CBUILDING_ID", DbType.String, 20, poNewEntity.CBUILDING_ID);
                loDb.R_AddCommandParameter(loCommand, "@CUNIT_ID ", DbType.String, 20, poNewEntity.CUNIT_ID);
                loDb.R_AddCommandParameter(loCommand, "@CFLOOR_ID", DbType.String, 20, poNewEntity.CFLOOR_ID);
                loDb.R_AddCommandParameter(loCommand, "@CHAND_OVER_DATE", DbType.String, 8, poNewEntity.CHO_ACTUAL_DATE);
                loDb.R_AddCommandParameter(loCommand, "@CSTART_DATE", DbType.String, 8, poNewEntity.CHO_PLAN_START_DATE);
                loDb.R_AddCommandParameter(loCommand, "@CEND_DATE", DbType.String, 8, poNewEntity.CHO_PLAN_END_DATE);
                loDb.R_AddCommandParameter(loCommand, "@CACTION", DbType.String, 10, lcAction);
                loDb.R_AddCommandParameter(loCommand, "@CUSER_ID", DbType.String, 8, poNewEntity.CUSER_ID);
                loDb.R_AddCommandParameter(loCommand, "@CLINK_DEPT", DbType.String, 20, poNewEntity.CDEPT_CODE);
                loDb.R_AddCommandParameter(loCommand, "@CLINK_TRANSCODE ", DbType.String, 10, poNewEntity.VAR_LOI_TRANS_CODE);
                loDb.R_AddCommandParameter(loCommand, "@CLINK_REFNO", DbType.String, 30, poNewEntity.CREF_NO);
                loDb.R_AddCommandParameter(loCommand, "@NACTUAL_SIZE", DbType.Int32, 20, poNewEntity.NHO_ACTUAL_SIZE);

                var loDbParam = loCommand.Parameters.Cast<DbParameter>()
                    .Where(x => x != null && x.ParameterName.StartsWith("@"))
                    .ToDictionary(x => x.ParameterName, x => x.Value);
                _logger.LogDebug("{@ObjectQuery} {@Parameter}", loCommand.CommandText, loDbParam);

                try
                {
                    var loDataTable = loDb.SqlExecQuery(loConn, loCommand, false);
                    var loEntity = R_Utility.R_ConvertTo<PMT02000LOIHeader_DetailDTO>(loDataTable).FirstOrDefault();
                    _logger.LogInfo(string.Format("END process method {0} on Cls", lcMethodName));
                }
                catch (Exception ex)
                {
                    loException.Add(ex);
                    _logger.LogError(loException);
                }
                loException.Add(R_ExternalException.R_SP_Get_Exception(loConn));


            }
            catch (Exception ex)
            {
                loException.Add(ex);
                _logger.LogError(loException);
            }
            finally
            {
                if (loConn != null)
                {
                    if (loConn.State != ConnectionState.Closed)
                    {
                        loConn.Close();
                    }
                    loConn.Dispose();
                }
            }
            loException.ThrowExceptionIfErrors();
        }

        protected override void R_Deleting(PMT02000LOIHeader_DetailDTO poEntity)
        {

            string lcMethodName = nameof(R_Deleting);
            using Activity activity = _activitySource.StartActivity(lcMethodName)!;
            _logger.LogInfo(string.Format("START process method {0} on Cls", lcMethodName));

            R_Exception loException = new R_Exception();
            string? lcQuery = null;
            R_Db loDb;
            DbCommand loCommand;
            DbConnection loConn = null;
            string? lcAction = null;
            try
            {
                loDb = new R_Db();
                loConn = loDb.GetConnection();
                R_ExternalException.R_SP_Init_Exception(loConn);
                loCommand = loDb.GetCommand();
                lcAction = "DELETE";

                lcQuery = "RSP_PM_MAINTAIN_HANDOVER";
                loCommand.CommandText = lcQuery;
                loCommand.CommandType = CommandType.StoredProcedure;

                loDb.R_AddCommandParameter(loCommand, "@CCOMPANY_ID", DbType.String, 8, poEntity.CCOMPANY_ID);
                loDb.R_AddCommandParameter(loCommand, "@CPROPERTY_ID", DbType.String, 20, poEntity.CPROPERTY_ID);
                loDb.R_AddCommandParameter(loCommand, "@CLINK_DEPT", DbType.String, 20, poEntity.CDEPT_CODE);
                loDb.R_AddCommandParameter(loCommand, "@CTRANS_CODE", DbType.String, 10, poEntity.VAR_TRANS_CODE); //
                loDb.R_AddCommandParameter(loCommand, "@CREF_NO", DbType.String, 30, poEntity.CHO_REF_NO); //
                loDb.R_AddCommandParameter(loCommand, "@CACTION", DbType.String, 10, lcAction);
                loDb.R_AddCommandParameter(loCommand, "@CUSER_ID", DbType.String, 8, poEntity.CUSER_ID);

                loDb.R_AddCommandParameter(loCommand, "@CREF_DATE", DbType.String, 8,"");
                loDb.R_AddCommandParameter(loCommand, "@CBUILDING_ID", DbType.String, 20,"");
                loDb.R_AddCommandParameter(loCommand, "@CUNIT_ID ", DbType.String, 20, "");
                loDb.R_AddCommandParameter(loCommand, "@CFLOOR_ID", DbType.String, 20, "");
                loDb.R_AddCommandParameter(loCommand, "@CHAND_OVER_DATE", DbType.String, 8, ""); //
                loDb.R_AddCommandParameter(loCommand, "@CSTART_DATE", DbType.String, 8, "");
                loDb.R_AddCommandParameter(loCommand, "@CEND_DATE", DbType.String, 8, "");
                loDb.R_AddCommandParameter(loCommand, "@CLINK_TRANSCODE ", DbType.String, 10, "");
                loDb.R_AddCommandParameter(loCommand, "@CLINK_REFNO", DbType.String, 30, "");
                loDb.R_AddCommandParameter(loCommand, "@NACTUAL_SIZE", DbType.Int32, 20, "");


                var loDbParam = loCommand.Parameters.Cast<DbParameter>()
                    .Where(x => x != null && x.ParameterName.StartsWith("@"))
                    .ToDictionary(x => x.ParameterName, x => x.Value);
                _logger.LogDebug("{@ObjectQuery} {@Parameter}", loCommand.CommandText, loDbParam);

                try
                {
                    loDb.SqlExecNonQuery(loConn, loCommand, false);
                    _logger.LogInfo(string.Format("END process method {0} on Cls", lcMethodName));
                }
                catch (Exception ex)
                {
                    loException.Add(ex);
                    _logger.LogError(loException);
                }
                loException.Add(R_ExternalException.R_SP_Get_Exception(loConn));
            }
            catch (Exception ex)
            {
                loException.Add(ex);
                _logger.LogError(loException);
            }
            finally
            {
                if (loConn != null)
                {
                    if (loConn.State != ConnectionState.Closed)
                    {
                        loConn.Close();
                    }
                    loConn.Dispose();
                }
            }
            loException.ThrowExceptionIfErrors();
        }

        #region Process SubmitRedraft
        public void ProcessSubmitRedraft(PMT02000DBParameter poParameter)
        {
            string lcMethodName = nameof(ProcessSubmitRedraft);
            using Activity activity = _activitySource.StartActivity(lcMethodName)!;
            _logger.LogInfo(string.Format("START process method {0} on Cls", lcMethodName));

            R_Exception loException = new R_Exception();
            DbConnection? loConn = null;
            DbCommand? loCommand = null;
            R_Db loDb;
            try
            {
                var lcQuery = "RSP_PM_UPDATE_HANDOVER";
                loDb = new R_Db();
                loCommand = loDb.GetCommand();
                loConn = loDb.GetConnection();
                loCommand.CommandText = lcQuery;
                loCommand.CommandType = CommandType.StoredProcedure;

                loDb.R_AddCommandParameter(loCommand, "@CCOMPANY_ID", DbType.String, 20, poParameter.CCOMPANY_ID);
                loDb.R_AddCommandParameter(loCommand, "@CPROPERTY_ID", DbType.String, 20, poParameter.CPROPERTY_ID);
                loDb.R_AddCommandParameter(loCommand, "@CTRANS_CODE", DbType.String, 6, poParameter.CTRANS_CODE);
                loDb.R_AddCommandParameter(loCommand, "@CDEPT_CODE", DbType.String, 20, poParameter.CDEPT_CODE);
                loDb.R_AddCommandParameter(loCommand, "@CREF_NO", DbType.String, 30, poParameter.CREF_NO);
                loDb.R_AddCommandParameter(loCommand, "@CUSER_ID", DbType.String, 8, poParameter.CUSER_ID); ;
                loDb.R_AddCommandParameter(loCommand, "@CNEW_STATUS", DbType.String, 2, poParameter.VAR_NEW_STATUS);

                var loDbParam = loCommand.Parameters.Cast<DbParameter>()
                    .Where(x => x != null && x.ParameterName.StartsWith("@"))
                    .ToDictionary(x => x.ParameterName, x => x.Value);
                _logger.LogDebug("{@ObjectQuery} {@Parameter}", loCommand.CommandText, loDbParam);

                loDb.SqlExecNonQuery(loConn, loCommand, false);
            }
            catch (Exception ex)
            {
                loException.Add(ex);
                _logger.LogError(loException);
            }
            _logger.LogInfo("End process method GetHeader on Cls");
        }
        #endregion
        /*
        protected override PMT02000LOIHeader_DetailDTO R_Display(PMT02000LOIHeader_DetailDTO poEntity)
        {

            string lcMethodName = nameof(R_Display);
            using Activity activity = _activitySource.StartActivity(lcMethodName)!;
            _logger.LogInfo(string.Format("START process method {0} on Cls", lcMethodName));

            R_Exception loException = new R_Exception();
            PMT02000LOIHeader_DetailDTO? loReturn = null;
            DbConnection? loConn = null;
            DbCommand? loCommand = null;
            R_Db loDb;
            try
            {
                var lcQuery = "RSP_PM_GET_HANDOVER_DETAIL";
                loDb = new R_Db();
                loCommand = loDb.GetCommand();
                loConn = loDb.GetConnection();
                loCommand.CommandText = lcQuery;
                loCommand.CommandType = CommandType.StoredProcedure;

                loDb.R_AddCommandParameter(loCommand, "@CCOMPANY_ID", DbType.String, 20, poEntity.CCOMPANY_ID);
                loDb.R_AddCommandParameter(loCommand, "@CPROPERTY_ID", DbType.String, 20, poEntity.CPROPERTY_ID);
                loDb.R_AddCommandParameter(loCommand, "@CTRANS_CODE", DbType.String, 6, poEntity.CTRANS_CODE);
                loDb.R_AddCommandParameter(loCommand, "@CLANG_ID", DbType.String, 3, poEntity.CLANG_ID);
                loDb.R_AddCommandParameter(loCommand, "@CDEPT_CODE", DbType.String, 20, poEntity.CDEPT_CODE);
                loDb.R_AddCommandParameter(loCommand, "@CREF_NO", DbType.String, 30, poEntity.CREF_NO);
                loDb.R_AddCommandParameter(loCommand, "@CBUILDING_ID", DbType.String, 20, poEntity.CBUILDING_ID);
                loDb.R_AddCommandParameter(loCommand, "@CFLOOR_ID", DbType.String, 20, poEntity.CFLOOR_ID);
                loDb.R_AddCommandParameter(loCommand, "@CUNIT_ID ", DbType.String, 20, poEntity.CUNIT_ID);
                loDb.R_AddCommandParameter(loCommand, "@CSAVEMODE ", DbType.String, 7, poEntity.CSAVEMODE);

                var loDbParam = loCommand.Parameters.Cast<DbParameter>()
                    .Where(x => x != null && x.ParameterName.StartsWith("@"))
                    .ToDictionary(x => x.ParameterName, x => x.Value);
                _logger.LogDebug("{@ObjectQuery} {@Parameter}", loCommand.CommandText, loDbParam);

                var loReturnTemp = loDb.SqlExecQuery(loConn, loCommand, false);
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                loReturn = R_Utility.R_ConvertTo<PMT02000LOIHeader_DetailDTO>(loReturnTemp).ToList().FirstOrDefault();
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

                //GET DETAIL LIST
                List<PMT02000LOIDetailListDTO> LOIDetailList = GetDetailListLOI(loConn, poEntity);

                if (LOIDetailList != null)
                {
                    loReturn!.ListDetail = LOIDetailList;
                }
            }
            catch (Exception ex)
            {
                loException.Add(ex);
                _logger.LogError(loException);
            }
            finally
            {
                if (loConn != null)
                {
                    if (!(loConn.State == ConnectionState.Closed))
                        loConn.Close();
                    loConn.Dispose();
                    loConn = null;
                }

                if (loCommand != null)
                {
                    loCommand.Dispose();
                    loCommand = null;
                }
            }
            _logger.LogInfo("End process method R_Display on Cls");
            return loReturn!;
        }
        */
    }
}
