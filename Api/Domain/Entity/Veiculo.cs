using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace minimal_api.Domain.Entity
{
    public class Veiculo
    {
        [Key] //Marca como chave primaria a propriedade abaixo no caso o ID
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } = default!;
        [Required]
        [StringLength(155)]
        public string Name { get; set; } = default!;
        [Required]
        [StringLength(100)]
        public string Brand { get; set; } = default!;
        [Required]
        public int Year { get; set; } = default!;
    }
}