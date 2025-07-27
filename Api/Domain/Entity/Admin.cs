using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace minimal_api.Domain.Entity
{
    public class Admin
    {
        [Key] //Marca como chave primaria a propriedade abaixo no caso o ID
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } = default!;
        [Required]
        [StringLength(255)]
        public string Email { get; set; } = default!;
        [Required]
        [StringLength(50)]
        public string Password { get; set; } = default!;
        [StringLength(10)]
        public string Role { get; set; } = default!;
    }
}