using System.ComponentModel.DataAnnotations;

namespace UserRE.API.Dtos
{
    public class UserForRegisterDto
    {
        [Required]
        public string username { get; set; }
        [StringLength(8,MinimumLength=4,ErrorMessage="Password must be at least 4")]
        public string password { get; set; }
    }
}