using Microsoft.Data.SqlClient.DataClassification;

namespace UnitedPayment.Model.DTOs.Requests
{
    public record MessageRequestDTO(
        int userId,
        int toUserId,
        string Message);
}
