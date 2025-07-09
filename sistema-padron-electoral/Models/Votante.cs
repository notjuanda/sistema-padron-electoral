using System;
using System.ComponentModel.DataAnnotations;

namespace sistema_padron_electoral.Models
{
    public class Votante
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(20)]
        public string CI { get; set; }

        [Required]
        [MaxLength(100)]
        public string NombreCompleto { get; set; }

        [Required]
        [MaxLength(200)]
        public string Direccion { get; set; }

        [MaxLength(200)]
        public string FotoCarnetAnverso { get; set; }

        [MaxLength(200)]
        public string FotoCarnetReverso { get; set; }

        [MaxLength(200)]
        public string FotoVotante { get; set; }

        [Required]
        public int RecintoId { get; set; }

        [MaxLength(100)]
        public string RecintoNombre { get; set; }
    }
}
