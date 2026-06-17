namespace Project_StudentERP.DTOs
{
    public class AssignSubjectDTO
    {
        public int classSectionId { get; set; }
        public List<int> subjectIds { get; set; } = new();
    }
}
