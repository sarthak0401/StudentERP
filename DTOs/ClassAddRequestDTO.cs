namespace Project_StudentERP.DTOs
{
    public class ClassAddRequestDTO
    {
        public int StandardId { get; set; }
        public string Section { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
