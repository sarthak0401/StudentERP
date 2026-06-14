namespace Project_StudentERP.DTOs
{
    public class AddStdFeeTypeMappingRequestDTO
    {
        public int StdId { get; set; }
        public List<int> FeeTypes { get; set; } = new();
    }
}
