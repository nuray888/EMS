using System.ComponentModel.DataAnnotations;

namespace UnitedPayment.Model.DTOs.Requests;
public class RegisterRequest
{
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public required string Email { get; set; }
    public required int Age { get; set; }
    public string? Address { get; set; }
    public required string Password { get; set; }

    [Required, Compare("Password")]
    public required string ConfirmPassword { get; set; }
};

