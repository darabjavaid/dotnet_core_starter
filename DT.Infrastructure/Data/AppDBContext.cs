using DT.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DT.Infrastructure.Data
{
    public class AppDBContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        //public DataContext(IConfiguration configuration)
        //{
        //    Configuration = configuration;
        //}        

        //public AppDBContext(DbContextOptions<AppDBContext> options, IConfiguration configuration) : base(options)
        //{
        //    Configuration = configuration;
        //}
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }

        public DbSet<Questionnaire> Questionnaires { get; set; }
        public DbSet<QuestionnaireDetail> QuestionnaireDetails { get; set; }
        public DbSet<Subject> Subjects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Questionnaire>()
                .HasOne(b => b.QuestionnaireDetail)
                .WithOne(i => i.Questionnaire)
                .HasForeignKey<QuestionnaireDetail>(b => b.QuestionID);
        }
    }
}
