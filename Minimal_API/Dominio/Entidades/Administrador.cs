using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Minimal_API.Dominio.Entidades
{
    public class Administrador
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
      
        [StringLength(20)]
        public string Perfil { get; set; }

        [Required]
        [StringLength(200)]
        public string Email { get; set; }

        [StringLength(50)]
        public string Senha { get; set; }
    }
}
