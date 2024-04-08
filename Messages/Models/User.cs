using Newtonsoft.Json;

namespace Messages.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Login { get; set; }

        public string? Password { get; set; }

        public string? Salt { get; set; }
        [JsonIgnore]
        public virtual ICollection<Message> Messages { get; set;}
    }
}
