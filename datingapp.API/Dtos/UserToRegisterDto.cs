using System.ComponentModel.DataAnnotations;

namespace datingapp.API.Dtos
{
    public class UserToRegisterDto
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [StringLength(8,MinimumLength = 4,ErrorMessage="Password must be btw 4 and 8 chars")]
        public string Password { get; set; }
    }
}