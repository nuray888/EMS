namespace UnitedPayment.Model
{
    public class PasswordHistory
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string PasswordHash { get; set; }
        public DateTime ChangeDate { get; set; }

    }
}
