using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using sistema_padron_electoral.Models;

namespace sistema_padron_electoral.Data
{
    public class sistema_padron_electoralContext : DbContext
    {
        public sistema_padron_electoralContext (DbContextOptions<sistema_padron_electoralContext> options)
            : base(options)
        {
        }

        public DbSet<sistema_padron_electoral.Models.Votante> Votante { get; set; } = default!;
    }
}
