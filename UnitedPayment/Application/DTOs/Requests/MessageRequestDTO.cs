using Microsoft.Data.SqlClient.DataClassification;

namespace UnitedPayment.Model.DTOs.Requests
{
    public record MessageRequestDTO(
        int toUserId,
        string Message);
}
