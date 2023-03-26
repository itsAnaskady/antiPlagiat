using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace AppAntiPlagiat.Models
{
    public class ApplicationDbContext : IdentityDbContext<Utilisateur>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Utilisateur> Utilisateurs { get; set; }
        public DbSet<Rapport> Rapports { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Encadre> Encadre { get; set; }
        public DbSet<PourcentagePlagiat> pourcentagePlagiats { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Rapport>()
                .HasOne(o => o.Etudiant)
                .WithMany()
                .HasForeignKey(o => o.EtudiantId)
                .OnDelete(DeleteBehavior.NoAction);



            builder.Entity<Encadre>()
                .HasOne(er => er.Etudiant)
                .WithMany()
                .HasForeignKey(er => er.EtudiantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Encadre>()
                .HasOne(er => er.Enseignant)
                .WithMany()
                .HasForeignKey(er => er.EnseignantId)
                .OnDelete(DeleteBehavior.NoAction);

            base.OnModelCreating(builder);
        }

    }
}
