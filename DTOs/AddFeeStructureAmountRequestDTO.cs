namespace Project_StudentERP.DTOs
{
    public class AddFeeStructureAmountRequestDTO
    {
        public List<ListOfFeeStructures> FeeStructures { get; set; } = new();
    }

    public class ListOfFeeStructures
    {
        public int StandardFeeMapId { get; set; }
        public decimal Amount { get; set; }
    }
}
