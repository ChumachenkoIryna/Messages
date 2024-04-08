namespace Messages.Models
{
    public class IndexModel
    {
        public RegisterModel RegisterModel { get; set; }
        public LoginModel LoginModel { get; set; }
        public List<Message> Messages { get; set; }
    }
}
