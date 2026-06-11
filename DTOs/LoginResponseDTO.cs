namespace Project_StudentERP.DTOs
{
    public class LoginResponseDTO
    {
        public int StatusCode { get; set; }
        public bool Success { get; set; }
        public string? AccessToken { get; set; }
        public string Message { get; set; }
    }
}
