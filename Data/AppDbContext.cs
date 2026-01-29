using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Survey.Entities.SurveyEntities;
using Survey.Entities;
using Survey.Entities.DocumentEntities;

namespace Survey.Data
{
    public class AppDbContext : DbContext 
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}
        //User Sections
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Position> Positions { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<UserRelation> UserRelations { get; set; } = null!;

        //Survey Sections
        public DbSet<SurveyHeader> SurveyHeaders { get; set; } = null!;
        public DbSet<SurveyItem> SurveyItems { get; set; } = null!;
        public DbSet<CheckboxOption> CheckboxOptions { get; set; } = null!;

        //Document Sections
        public DbSet<DocumentSurvey> DocumentSurveys { get; set; } = null!;
        public DbSet<DocumentSurveyAnswer> DocumentSurveyAnswers { get; set; } = null!;
        public DbSet<DocumentSurveyAnswerOption> DocumentSurveyAnswerOptions { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserRelation>(e =>
            {
                e.HasKey(x => new { x.SupervisorId, x.UserId });

                e.HasIndex(x => x.UserId).IsUnique();

                e.HasOne(x => x.Supervisor)
                    .WithMany(u => u.Subordinates)
                    .HasForeignKey(x => x.SupervisorId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.User)
                    .WithMany(u => u.Supervisors)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<SurveyItem>()
                .HasIndex(x => new { x.HeaderId, x.SortOrder })
                .IsUnique();

            modelBuilder.Entity<DocumentSurvey>()
                .HasIndex(x => x.DocumentNo)
                .IsUnique();

            modelBuilder.Entity<DocumentSurveyAnswer>(e =>
            {
                e.HasOne(x => x.Item)
                    .WithMany()
                    .HasForeignKey(x => x.ItemId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<SurveyHeader>()
                .HasIndex(x => x.TemplateCode)
                .IsUnique();

            modelBuilder.Entity<DocumentSurveyAnswerOption>(e =>
            {
                e.HasKey(x => x.Id);

                e.HasOne(x => x.Answer)
                    .WithMany(a => a.DocumentSurveyAnswerOptions)
                    .HasForeignKey(x => x.AnswerId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.CheckboxOption)
                    .WithMany(o => o.DocumentSurveyAnswerOptions)
                    .HasForeignKey(x => x.CheckboxOptionId)
                    .OnDelete(DeleteBehavior.SetNull);

                e.HasIndex(x => new { x.AnswerId, x.CheckboxOptionId }).IsUnique();
            });
            modelBuilder.Entity<CheckboxOption>(e =>
            {
                e.HasOne(x => x.Item)
                    .WithMany(i => i.CheckboxOptions)
                    .HasForeignKey(x => x.ItemId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}