using PMT02000COMMON.LOI_List;
using PMT02000COMMON.Utility;
using R_CommonFrontBackAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMT02000COMMON.Interface
{
    public interface IPMT02000 : R_IServiceCRUDBase<PMT02000LOIHeader_DetailDTO>
    {
        IAsyncEnumerable<PMT02000PropertyDTO> GetPropertyListStream();
        IAsyncEnumerable<PMT02000LOIDTO> GetLOIListStream();
        PMT02000LOIHeader GetLOIHeader(PMT02000DBParameter poParam);
        IAsyncEnumerable<PMT02000LOIDetailListDTO> GetLOIDetailListStream();
        PMT02000LOIHeader ProcessSubmitRedraft(PMT02000DBParameter poParam);

    }
}
