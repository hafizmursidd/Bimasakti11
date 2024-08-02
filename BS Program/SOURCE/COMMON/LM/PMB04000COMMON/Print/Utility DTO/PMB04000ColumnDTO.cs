using System;
using System.Collections.Generic;
using System.Text;

namespace PMB04000COMMON.Print.Utility_DTO
{
    public class PMB04000ColumnDTO
    {
        public string? Col_CINVOICE_NO { get; set; } = "No. Invoice";
        public string? Col_CINVOICE_DATE { get; set; } = "Tanggal Invoice";
        public string? Col_CTRANS_DESC { get; set; } = "Deskripsi";
        public string? Col_NINV_AMOUNT { get; set; } = "Jumlah";
    }
}
