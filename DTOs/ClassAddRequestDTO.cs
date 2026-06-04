namespace Project_StudentERP.DTOs
{
    public class ClassAddRequestDTO
    {
        public string ClassN { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
