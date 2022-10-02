using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using DropDown.Models;

namespace DropDown.Data
{
    public partial class DropDownContext : DbContext
    {
        public DropDownContext()
        {
        }

        public DropDownContext(DbContextOptions<DropDownContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ActionProj> ActionProjs { get; set; } = null!;
        public virtual DbSet<Dd> Dds { get; set; } = null!;
        public virtual DbSet<Detail> Details { get; set; } = null!;
        public virtual DbSet<Dr> Drs { get; set; } = null!;
        public virtual DbSet<Exercice> Exercices { get; set; } = null!;
        public virtual DbSet<Objectif> Objectifs { get; set; } = null!;
        public virtual DbSet<Programme> Programmes { get; set; } = null!;
        public virtual DbSet<Projet> Projets { get; set; } = null!;
        public virtual DbSet<Prévision> Prévisions { get; set; } = null!;
        public virtual DbSet<Réalisation> Réalisations { get; set; } = null!;
        public virtual DbSet<Stock> Stocks { get; set; } = null!;
        

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=DESKTOP-8CCLSBT\\SQL_EXPRESS;Database=DropDown;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        public DbSet<DropDown.Models.User> User { get; set; }

        
    }
}
