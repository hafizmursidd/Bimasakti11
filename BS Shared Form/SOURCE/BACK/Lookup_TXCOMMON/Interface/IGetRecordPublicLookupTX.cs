using Lookup_TXCOMMON.DTOs.TXL00100;
using Lookup_TXCOMMON.DTOs.Utilities;

namespace Lookup_TXCOMMON.Interface
{
    public interface IGetRecordPublicLookupTX
    {
        TXLGenericRecord<TXL00100DTO> TXL00100BranchLookUp(TXL00100ParameterGetRecordDTO poParameter);
    }
}
