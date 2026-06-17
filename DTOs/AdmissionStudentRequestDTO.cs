namespace Project_StudentERP.DTOs
{
    public class AdmissionStudentRequestDTO
    {
        public int StdId { get; set; }

        public int CSId { get; set; }

        public List<SelectedFees> SelectedFeesList { get; set; } = new();
    }

    public class SelectedFees
    {
        public int FixedAmt { get; set; }
        public int ClassFeeTypeId { get; set; }

        public float Discount { get; set; }

        public float FinalAmount { get; set; }
    }
}
