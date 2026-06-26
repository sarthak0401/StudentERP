using Project_StudentERP.DTOs;
using Project_StudentERP.Models;

namespace Project_StudentERP.Interfaces
{
    public interface IAdministrationService
    {
        Task AddStudent(StudentDTO dto, string filename, int tId);
        Task<byte[]> GenerateReceipt(int receiptId);
        List<string> getAllContacts(int id);
        GetFeeForAlreadyAdmittedStudentResponseDTO GetAllFeeInfoForAlreadyAdmittedStudent(int id);
        GetAllFeeTypesForParticularClassSectionResponseDTO GetAllFeeTypesForParticularClassSection(
            int id
        );
        GetAllReceiptsResponseDTO GetAllReceipts(int id);
        List<Student> GetAllStudents();
        FeeBalanceForAdmittedStudentResponseDTO GetBalanceFeeForAdmittedStudent(int id);
        Student GetStudentById(int id);
        List<SearchStudentResponseDTO> GetStudentsOnSearch(StudentSearchDTO studentSearchDTO);
        AdmissionStudentResponseDTO StudentAdmission(AdmissionStudentRequestDTO dto);
        Task<UpdateFeeBalanceResponseDTO> UpdateFeeBalanceForAdmittedStudent(
            UpdateFeeBalanceRequestDTO dto
        );
        UpdateFeeDetailsStudentResponseDTO UpdateFeeDetailsOfAlreadyAdmittedStudent(
            UpdateFeeDetailsStudentRequestDTO dto
        );
    }
}
