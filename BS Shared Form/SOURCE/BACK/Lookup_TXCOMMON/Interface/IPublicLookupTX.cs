using Lookup_TXCOMMON.DTOs.TXL00100;
using System.Collections.Generic;

namespace Lookup_TXCOMMON.Interface
{
    public interface IPublicLookupTX
    {
        IAsyncEnumerable<TXL00100DTO> TXL00100BranchLookUp();
    }
}
