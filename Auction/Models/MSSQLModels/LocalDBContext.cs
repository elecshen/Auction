using System;
using System.Collections.Generic;
using Auction.Models.MSSQLModels.Entities;
using Microsoft.EntityFrameworkCore;

namespace Auction.Models.MSSQLModels;

public partial class LocalDBContext : DbContext
{
    public LocalDBContext()
    {
    }

    public LocalDBContext(DbContextOptions<LocalDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ActiveLot> ActiveLots { get; set; }

    public virtual DbSet<Bid> Bids { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Lot> Lots { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=LAPTOP-4AHQIJI5\\SQLEXPRESS;Database=Auction;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActiveLot>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("ActiveLots");

            entity.Property(e => e.ExpiresOn).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Bid>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Bid__3214EC279023AA21");

            entity.ToTable("Bid");

            entity.HasIndex(e => e.Value, "IX_BidValue");

            entity.HasIndex(e => e.BidderId, "IX_Bidder");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.BidDate).HasColumnType("datetime");
            entity.Property(e => e.BidderId).HasColumnName("BidderID");
            entity.Property(e => e.LotId).HasColumnName("LotID");

            entity.HasOne(d => d.Bidder).WithMany(p => p.Bids)
                .HasForeignKey(d => d.BidderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BidOwner");

            entity.HasOne(d => d.Lot).WithMany(p => p.Bids)
                .HasForeignKey(d => d.LotId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BidToLot");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Category__3214EC277E3466E8");

            entity.ToTable("Category");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Comment__3214EC27E29B7116");

            entity.ToTable("Comment");

            entity.HasIndex(e => e.CommentatorId, "IX_CommentAuthor");

            entity.HasIndex(e => e.LotId, "IX_CommentToLot");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.CommentatorId).HasColumnName("CommentatorID");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.LotId).HasColumnName("LotID");
            entity.Property(e => e.Text).HasColumnType("text");

            entity.HasOne(d => d.Commentator).WithMany(p => p.Comments)
                .HasForeignKey(d => d.CommentatorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CommentAuthor");

            entity.HasOne(d => d.Lot).WithMany(p => p.Comments)
                .HasForeignKey(d => d.LotId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CommentToLot");
        });

        modelBuilder.Entity<Lot>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Lot__3214EC270277B31E");

            entity.ToTable("Lot");

            entity.HasIndex(e => e.CategoryId, "IX_LotCategory");

            entity.HasIndex(e => e.OwnerId, "IX_LotOwner");

            entity.HasIndex(e => e.StatusId, "IX_LotStatus");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.ExpiresOn).HasColumnType("datetime");
            entity.Property(e => e.IsClosed).HasDefaultValue(true);
            entity.Property(e => e.LastBidId).HasColumnName("LastBidID");
            entity.Property(e => e.OwnerId).HasColumnName("OwnerID");
            entity.Property(e => e.PublicId).ValueGeneratedOnAdd();
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.StatusId)
                .HasDefaultValue(1)
                .HasColumnName("StatusID");
            entity.Property(e => e.Title).HasMaxLength(128);

            entity.HasOne(d => d.Category).WithMany(p => p.Lots)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LotCategory");

            entity.HasOne(d => d.LastBid).WithMany(p => p.Lots)
                .HasForeignKey(d => d.LastBidId)
                .HasConstraintName("FK_LotLastBid");

            entity.HasOne(d => d.Owner).WithMany(p => p.Lots)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LotOwner");

            entity.HasOne(d => d.Status).WithMany(p => p.Lots)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LotStatus");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Status__3214EC2739B6A836");

            entity.ToTable("Status");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC27D675023C");

            entity.ToTable("User");

            entity.HasIndex(e => e.Username, "UQ__User__536C85E430421527").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ID");
            entity.Property(e => e.Hash)
                .HasMaxLength(128)
                .IsUnicode(false);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.Salt)
                .HasMaxLength(128)
                .IsUnicode(false);
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
