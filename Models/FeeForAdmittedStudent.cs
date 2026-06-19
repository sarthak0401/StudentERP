namespace Project_StudentERP.Models
{
    public class FeeForAdmittedStudent
    {
        public int ClassFeeTypeId { get; set; }
        public int FeeTypeId { get; set; }
        public int AdmissionFeeId { get; set; }
        public int AdmissionId { get; set; }
        public int CSId { get; set; }

        public float DefaultAmount { get; set; }
        public float Discount { get; set; }
        public float FinalAmount { get; set; }
        public int Checked { get; set; }
        public string FeeTypeName { get; set; }
        public string ClassName { get; set; }
        public string SectionName { get; set; }
    }
}
