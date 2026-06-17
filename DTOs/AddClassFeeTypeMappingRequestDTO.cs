namespace Project_StudentERP.DTOs
{
    public class AddClassFeeTypeMappingRequestDTO
    {
        public int ClassSectionId { get; set; }
        public List<FeeStructure> feeMappings { get; set; } = new();
    }

    public class FeeStructure
    {
        public float amount { get; set; }
        public int feeTypeId { get; set; }
    }
}
