using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.Entities
{
    public class UsersEntity
    {
        
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Rol { get; set; }
        [Required]
        [MaxLength(100)]
        public string Email { get; set; }
        [Required] 
        public string PasswordHash { get; set; }
    }
}
