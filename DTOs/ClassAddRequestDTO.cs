namespace Project_StudentERP.DTOs
{
    public class ClassAddRequestDTO
    {
        public string? ClassName { get; set; }
        public List<int> SectionIds { get; set; } = [];
        public int Capacity { get; set; }
        public int Status { get; set; }
    }
}
