using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace CrocoZooApp.CrocoZooDB;

public partial class ImportSyouDbContext : DbContext
{
    public ImportSyouDbContext()
    {
    }

    public ImportSyouDbContext(DbContextOptions<ImportSyouDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Animal> Animals { get; set; }

    public virtual DbSet<Gamesession> Gamesessions { get; set; }

    public virtual DbSet<Memorycard> Memorycards { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<Riddle> Riddles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;port=3306;user=root;database=import_syou_db", Microsoft.EntityFrameworkCore.ServerVersion.Parse("9.1.0-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Animal>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("animals")
                .UseCollation("utf8mb4_general_ci");

            entity.Property(e => e.ImagePath).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.SoundPath).HasMaxLength(255);
        });

        modelBuilder.Entity<Gamesession>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("gamesessions")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.PlayerId, "PlayerId");

            entity.Property(e => e.DatePlayed)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.GameMode).HasColumnType("enum('riddle','memo')");
            entity.Property(e => e.Score).HasDefaultValueSql("'0'");

            entity.HasOne(d => d.Player).WithMany(p => p.Gamesessions)
                .HasForeignKey(d => d.PlayerId)
                .HasConstraintName("GameSessions_ibfk_1");
        });

        modelBuilder.Entity<Memorycard>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("memorycards")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.AnimalId, "AnimalId");

            entity.Property(e => e.PairType)
                .HasDefaultValueSql("'image-image'")
                .HasColumnType("enum('image-image','image-son')");

            entity.HasOne(d => d.Animal).WithMany(p => p.Memorycards)
                .HasForeignKey(d => d.AnimalId)
                .HasConstraintName("MemoryCards_ibfk_1");
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("players")
                .UseCollation("utf8mb4_general_ci");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Riddle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("riddles")
                .UseCollation("utf8mb4_general_ci");

            entity.HasIndex(e => e.CorrectAnimalId, "CorrectAnimalId");

            entity.Property(e => e.QuestionText).HasColumnType("text");

            entity.HasOne(d => d.CorrectAnimal).WithMany(p => p.Riddles)
                .HasForeignKey(d => d.CorrectAnimalId)
                .HasConstraintName("Riddles_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
