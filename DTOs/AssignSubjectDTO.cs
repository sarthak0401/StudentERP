namespace Project_StudentERP.DTOs
{
    public class AssignSubjectDTO
    {
        public int classId { get; set; }
        public List<int> subjectIds { get; set; } = new();
    }
}
