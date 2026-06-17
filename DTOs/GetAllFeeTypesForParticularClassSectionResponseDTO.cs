using System.Diagnostics.Eventing.Reader;
using Project_StudentERP.Models;

namespace Project_StudentERP.DTOs
{
    public class GetAllFeeTypesForParticularClassSectionResponseDTO
    {
        public int Status { get; set; }
        public bool Success { get; set; }
        public string Message { set; get; } = "";

        public List<RowTemplateForFeeTypesOfParticularClassSection> fmLists { get; set; } =
            new List<RowTemplateForFeeTypesOfParticularClassSection>();
    }
}
