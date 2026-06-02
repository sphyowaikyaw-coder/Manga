using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Dependency;

public partial class MangaDbContext : DbContext
{
    public MangaDbContext()
    {
    }

    public MangaDbContext(DbContextOptions<MangaDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Chapter> Chapters { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Favorite> Favorites { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Manga> Mangas { get; set; }

    public virtual DbSet<MangaGenre> MangaGenres { get; set; }

    public virtual DbSet<Rating> Ratings { get; set; }

    public virtual DbSet<ReadingHistory> ReadingHistories { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-GB6LURR;Database=Manga;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.AuthorId).HasName("PK__Authors__70DAFC34A923950F");

            entity.Property(e => e.AuthorName).HasMaxLength(150);
        });

        modelBuilder.Entity<Chapter>(entity =>
        {
            entity.HasKey(e => e.ChapterId).HasName("PK__Chapters__0893A36A8B827066");

            entity.Property(e => e.ChapterNumber).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.ChapterUrl).HasMaxLength(200);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.Manga).WithMany(p => p.Chapters)
                .HasForeignKey(d => d.MangaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Chapters__MangaI__49C3F6B7");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__Comments__C3B4DFCA2195D14A");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Manga).WithMany(p => p.Comments)
                .HasForeignKey(d => d.MangaId)
                .HasConstraintName("FK__Comments__MangaI__5BE2A6F2");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Comments__UserId__5AEE82B9");
        });

        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.HasKey(e => e.FavoriteId).HasName("PK__Favorite__CE74FAD55A18A6B5");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Manga).WithMany(p => p.Favorites)
                .HasForeignKey(d => d.MangaId)
                .HasConstraintName("FK__Favorites__Manga__5165187F");

            entity.HasOne(d => d.User).WithMany(p => p.Favorites)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Favorites__UserI__5070F446");
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.GenreId).HasName("PK__Genres__0385057EFEE8E1DF");

            entity.Property(e => e.GenreName).HasMaxLength(100);
        });

        modelBuilder.Entity<Manga>(entity =>
        {
            entity.HasKey(e => e.MangaId).HasName("PK__Manga__9B040FCE7514E1E7");

            entity.ToTable("Manga");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.ViewCount).HasDefaultValue(0);

            entity.HasOne(d => d.Author).WithMany(p => p.Mangas)
                .HasForeignKey(d => d.AuthorId)
                .HasConstraintName("FK__Manga__AuthorId__4222D4EF");
        });

        modelBuilder.Entity<MangaGenre>(entity =>
        {
            entity.HasKey(e => e.MangaGenreId).HasName("PK__MangaGen__04DEB7247AB0ABBB");

            entity.HasOne(d => d.Genre).WithMany(p => p.MangaGenres)
                .HasForeignKey(d => d.GenreId)
                .HasConstraintName("FK__MangaGenr__Genre__45F365D3");

            entity.HasOne(d => d.Manga).WithMany(p => p.MangaGenres)
                .HasForeignKey(d => d.MangaId)
                .HasConstraintName("FK__MangaGenr__Manga__44FF419A");
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(e => e.RatingId).HasName("PK__Ratings__FCCDF87CC0016F29");

            entity.HasOne(d => d.Manga).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.MangaId)
                .HasConstraintName("FK__Ratings__MangaId__60A75C0F");

            entity.HasOne(d => d.User).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Ratings__UserId__5FB337D6");
        });

        modelBuilder.Entity<ReadingHistory>(entity =>
        {
            entity.HasKey(e => e.HistoryId).HasName("PK__ReadingH__4D7B4ABD10C08753");

            entity.ToTable("ReadingHistory");

            entity.Property(e => e.LastReadAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Chapter).WithMany(p => p.ReadingHistories)
                .HasForeignKey(d => d.ChapterId)
                .HasConstraintName("FK__ReadingHi__Chapt__571DF1D5");

            entity.HasOne(d => d.Manga).WithMany(p => p.ReadingHistories)
                .HasForeignKey(d => d.MangaId)
                .HasConstraintName("FK__ReadingHi__Manga__5629CD9C");

            entity.HasOne(d => d.User).WithMany(p => p.ReadingHistories)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__ReadingHi__UserI__5535A963");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C82A38E40");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534EB83DE45").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasDefaultValue("User");
            entity.Property(e => e.UserName).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
