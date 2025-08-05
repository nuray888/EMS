namespace UnitedPayment.Model
{
    public class Chat
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int toUserId { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime Date { get; set; }

    }
}
