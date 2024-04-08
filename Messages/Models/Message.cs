namespace Messages.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string? Text { get; set; }

        public virtual User User { get; set; }

        public DateTime Date { get; set; }

    }
}
