

using System.ComponentModel.DataAnnotations;

namespace EWallet.DataLayer.DTO.Request
{
    public class AdminRegistrationRequestDto
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
