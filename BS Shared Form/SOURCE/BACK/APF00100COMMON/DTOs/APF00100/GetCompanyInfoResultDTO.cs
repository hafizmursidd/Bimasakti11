﻿using R_APICommonDTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace APF00100COMMON.DTOs.APF00100
{
    public class GetCompanyInfoResultDTO : R_APIResultBaseDTO
    {
        public GetCompanyInfoDTO Data { get; set; }
    }
}