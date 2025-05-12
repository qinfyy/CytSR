using System.ComponentModel.DataAnnotations;

namespace SDKServer.Models
{
    public class User
    {
        [Key]
        public string uid { get; set; }

        [Required]
        [MaxLength(255)]
        public string username { get; set; }

        public string? password { get; set; }

        public int gameUid { get; set; }

        public string? comboToken { get; set; }

        public string SessionToken { get; set; }
    }
}