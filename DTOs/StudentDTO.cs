using System.ComponentModel.DataAnnotations.Schema;

namespace Project_StudentERP.DTOs
{
    public class StudentDTO
    {
        public int? SId { get; set; }
        public string? SName { get; set; }
        public int SAge { get; set; }
        public DateTime SDOB { get; set; }
        public string? SContact { get; set; }
        public string? SGender { get; set; }
        public string? SBloodGrp { get; set; }
        public IFormFile? SImg { get; set; } // How actual file uploaded will be mapped to this ?
        public string? SFatherName { get; set; }
        public string? SMotherName { get; set; }
        public string? SGuardianContact { get; set; }
        public string? SGuardianAltContact { get; set; }
        public string? SAddress { get; set; }
        public string? SAddressCity { get; set; }
        public string? SAddressState { get; set; }
        public string? SAddressPincode { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal S10thPctg { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal S12thPctg { get; set; }
        public int SJeeRank { get; set; }
        public string? SJuniorClgName { get; set; }
        public int SGapYears { get; set; }
        public string? SGapJustification { get; set; }

        public string? ExistingImage { get; set; }

        public List<StudentContactNo> ContactNos { get; set; } = new();
    }

    public class StudentContactNo
    {
        public string ContactNo { get; set; }
    }
}
