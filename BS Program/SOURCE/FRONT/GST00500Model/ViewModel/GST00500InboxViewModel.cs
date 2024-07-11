using R_BlazorFrontEnd;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using GST00500Common;
using R_APICommonDTO;
using R_BlazorFrontEnd.Exceptions;
using R_BlazorFrontEnd.Helpers;
using R_CommonFrontBackAPI;
using R_ProcessAndUploadFront;
using System.Linq;
using System.Runtime.CompilerServices;
using GST00500FrontResources;
using System.Globalization;

namespace GST00500Model.ViewModel
{
    public class GST00500InboxViewModel : R_ViewModel<GST00500DTO>, R_IProcessProgressStatus
    {
        private GST00500InboxModel _modelGST00500 = new GST00500InboxModel();
        public ObservableCollection<GST00500DTO> InboxTransactionList = new ObservableCollection<GST00500DTO>();
        public GST00500UserNameDTO UserName = new GST00500UserNameDTO();

        public List<GST00500DTO> loInboxApprovaltBatchList = new List<GST00500DTO>();
        public List<GST00500RejectDTO> ReasonOfRejectList = new List<GST00500RejectDTO>();
        public GST00500ParamToOthersProgramDTO _currentRecord = new GST00500ParamToOthersProgramDTO();

        //Assign on RejectPopUp.razor
        public GST00500ReasonRejectDTO ParamRejectTransactionStatus = new GST00500ReasonRejectDTO();
        public Action<R_Exception>? DisplayErrorAction { get; set; }
        public bool BtnReject = true;
        public bool BtnApprove = true;
        public string CCOMPANYID = "";
        public string CUSERID = "";
        public string Message = "";
        public int Percentage = 0;
        private enum TYPE_APPROVE { Approve, Reject }

        private TYPE_APPROVE TipeApprove;
        public Action? StateChangeAction { get; set; }
        public async Task GetAllInboxTransaction()
        {
            R_Exception loException = new R_Exception();
            try
            {
                var loResult = await _modelGST00500.GetInboxListAsyncModel();
                InboxTransactionList = new ObservableCollection<GST00500DTO>(loResult);

                if (InboxTransactionList.Count > 0)
                {
                    BtnReject = true;
                    BtnApprove = true;
                    _currentRecord = R_FrontUtility.ConvertObjectToObject<GST00500ParamToOthersProgramDTO>(InboxTransactionList.FirstOrDefault());
                  
                    foreach (var item in InboxTransactionList)
                    {
                        item.DREF_DATE = ConvertStringToDateTimeFormat(item.CREF_DATE);
                    }
                }
                else
                {
                    BtnReject = false;
                    BtnApprove = false;
                }
            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }
        }

        public async Task GetUserName()
        {
            R_Exception loException = new R_Exception();
            try
            {
                var loResult = await _modelGST00500.GetUserNameAsyncModel();
                UserName = loResult;
            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }
        }
        public async Task GetReasonRejectTransaction()
        {
            R_Exception loException = new R_Exception();
            try
            {
                var loResult = await _modelGST00500.GetReasonRejectListAsyncModel();
                ReasonOfRejectList = loResult.Data;
            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }
        }

        #region Seprated Data Selected
        public bool IsDataSelectedExist()
        {
            R_Exception loException = new R_Exception();
            bool isSelected = false;
            try
            {
                var loTempListToCheck = InboxTransactionList.ToList();
                isSelected = loTempListToCheck.Any(item => item.LSELECTED == true);
            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }
        EndBlock:
            loException.ThrowExceptionIfErrors();

            return isSelected;
        }
        #endregion

        #region Seprated Data Selected and Process (Approve and Reject)
        public async Task ProcessApproval()
        {
            R_Exception loException = new R_Exception();
            List<GST00500DTO> loInboxApprovaltBatchListSelected = new List<GST00500DTO>();

            try
            {
                loInboxApprovaltBatchListSelected = loInboxApprovaltBatchList.Where(x => x.LSELECTED == true).ToList();

                await ProcessApproveTransaction(loInboxApprovaltBatchListSelected);
            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }
        EndBlock:
            loException.ThrowExceptionIfErrors();
        }

        public async Task GetSelectedDataToRejectApproval()
        {
            R_Exception loException = new R_Exception();
            List<GST00500DataRejectDTO> loInboxRejectListSelected = null;

            try
            {
                var loTEmpInboxRejectListSelected = loInboxApprovaltBatchList.Where(x => x.LSELECTED == true).ToList();

                var loTempSelected =
                    R_FrontUtility.ConvertCollectionToCollection<GST00500DataRejectDTO>(loTEmpInboxRejectListSelected).ToList();

                //Add Reason Rejected code and reason_reject_notes
                loInboxRejectListSelected = loTempSelected.Select(item =>
                {
                    item.CREASON_CODE = ParamRejectTransactionStatus.CREASON_CODE;
                    item.TNOTES = ParamRejectTransactionStatus.TNOTES;
                    return item;
                }).ToList();

                await ProcessRejectTransaction(loInboxRejectListSelected);
            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }
            loException.ThrowExceptionIfErrors();
        }
        #endregion

        #region Approve
        public async Task ProcessApproveTransaction(List<GST00500DTO> loInboxApprovaltBatchListSelected)
        {
            var loEx = new R_Exception();
            R_BatchParameter loUploadPar;
            R_ProcessAndUploadClient loCls;
            List<R_KeyValue> loUserParameters;

            try
            {
                loUserParameters = new List<R_KeyValue>();

                //Instantiate ProcessClient
                loCls = new R_ProcessAndUploadClient(
                    pcModuleName: "GS",
                    plSendWithContext: true,
                    plSendWithToken: true,
                    pcHttpClientName: "R_DefaultServiceUrl",
                    poProcessProgressStatus: this);

                //prepare Batch Parameter
                loUploadPar = new R_BatchParameter();
                loUploadPar.COMPANY_ID = CCOMPANYID;
                loUploadPar.USER_ID = CUSERID;
                loUploadPar.UserParameters = loUserParameters;
                loUploadPar.ClassName = "GST00500Back.GST00500ProcessApproveCls";
                loUploadPar.BigObject = loInboxApprovaltBatchListSelected;

                TipeApprove = TYPE_APPROVE.Approve;
                await loCls.R_BatchProcess<List<GST00500DTO>>(loUploadPar, loInboxApprovaltBatchListSelected.Count);
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }
        }
        #endregion

        #region REJECT
        public async Task ProcessRejectTransaction(List<GST00500DataRejectDTO> loInboxRejectBatchListSelected)
        {
            var loEx = new R_Exception();
            R_BatchParameter loUploadPar;
            R_ProcessAndUploadClient loCls;
            List<R_KeyValue> loUserParameters;

            try
            {
                //Instantiate ProcessClient
                loCls = new R_ProcessAndUploadClient(
                    pcModuleName: "GS",
                    plSendWithContext: true,
                    plSendWithToken: true,
                    pcHttpClientName: "R_DefaultServiceUrl",
                    poProcessProgressStatus: this);

                //prepare Batch Parameter
                loUploadPar = new R_BatchParameter();
                loUploadPar.COMPANY_ID = CCOMPANYID;
                loUploadPar.USER_ID = CUSERID;
                loUploadPar.ClassName = "GST00500Back.GST00500ProcessRejectCls";
                loUploadPar.BigObject = loInboxRejectBatchListSelected;

                TipeApprove = TYPE_APPROVE.Reject;
                await loCls.R_BatchProcess<List<GST00500DataRejectDTO>>(loUploadPar, loInboxRejectBatchListSelected.Count);
            }
            catch (Exception ex)
            {
                loEx.Add(ex);
            }
        }
        #endregion

        #region ProgressStatus

        public async Task ProcessComplete(string pcKeyGuid, eProcessResultMode poProcessResultMode)
        {
            if (TipeApprove == TYPE_APPROVE.Approve)
            {
                if (poProcessResultMode == eProcessResultMode.Success)
                {
                    Message = string.Format("Finish Process Approving!” ");
                }

                if (poProcessResultMode == eProcessResultMode.Fail)
                {
                    Message = ("Process Approve Complete but fail with");
                    try
                    {
                         await ServiceGetError(pcKeyGuid);
                    }
                    catch (R_Exception ex)
                    {
                          DisplayErrorAction.Invoke(ex);
                    }
                }
            }
            else if (TipeApprove == TYPE_APPROVE.Reject)
            {
                if (poProcessResultMode == eProcessResultMode.Success)
                {
                    Message = string.Format("Finish Process Reject!” ");
                }

                if (poProcessResultMode == eProcessResultMode.Fail)
                {
                    Message = string.Format("Process Reject Complete but fail");
                    try
                    {
                         await ServiceGetError(pcKeyGuid);
                    }
                    catch (R_Exception ex)
                    {
                          DisplayErrorAction.Invoke(ex);
                    }
                }
            }
            StateChangeAction();
            await Task.CompletedTask;
        }
        public async Task ProcessError(string pcKeyGuid, R_APIException ex)
        {
            R_Exception loException = new R_Exception();
            Message = string.Format("Process Error with GUID {0}", pcKeyGuid);
            if (TipeApprove == TYPE_APPROVE.Approve)
            {
                ex.ErrorList.ForEach(x => loException.Add(x.ErrNo, x.ErrDescp));
            }
            else if (TipeApprove == TYPE_APPROVE.Reject)
            {
                ex.ErrorList.ForEach(x => loException.Add(x.ErrNo, x.ErrDescp));
            }

            DisplayErrorAction.Invoke(loException);
            StateChangeAction();
            await Task.CompletedTask;
        }
        public Task ReportProgress(int pnProgress, string pcStatus)
        {
            if (TipeApprove == TYPE_APPROVE.Approve)
            {
                Message = string.Format("Process Progress {0} with status {1}", pnProgress, pcStatus);
            }
            else if (TipeApprove == TYPE_APPROVE.Reject)
            {
                Message = string.Format("Process Progress {0} with status {1}", pnProgress, pcStatus);
            }
            StateChangeAction();
            return Task.CompletedTask;
        }
        private async Task ServiceGetError(string pcKeyGuid)
        {
            R_Exception loException = new R_Exception();

            List<R_ErrorStatusReturn> loResultData;
            R_GetErrorWithMultiLanguageParameter loParameterData;
            R_ProcessAndUploadClient loCls;
            try
            {
                loParameterData = new R_GetErrorWithMultiLanguageParameter()
                {
                    COMPANY_ID = CCOMPANYID,
                    USER_ID = CUSERID,
                    KEY_GUID = pcKeyGuid,
                    RESOURCE_NAME = "RSP_GS_MAINTAIN_APPROVALResources"
                };
                loCls = new R_ProcessAndUploadClient(
                    pcModuleName: "GS",
                    plSendWithContext: true,
                    plSendWithToken: true,
                    pcHttpClientName: "R_DefaultServiceUrl",
                    poProcessProgressStatus: this);

                loResultData = await loCls.R_GetStreamErrorProcess(loParameterData);

                loResultData.ForEach(x => loException.Add(x.SeqNo.ToString(), x.ErrorMessage));
                goto EndBlock;
            }
            catch (Exception ex)
            {
                loException.Add(ex);
            }
        EndBlock:;
            loException.ThrowExceptionIfErrors();

        }
        #endregion

        #region Utilities
        public DateTime? ConvertStringToDateTimeFormat(string? pcEntity)
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

        #endregion

    }
}
