using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace hotel_be.ModelFromDB;

public partial class DBCnhom4 : DbContext
{
    public DBCnhom4()
    {
    }

    public DBCnhom4(DbContextOptions<DBCnhom4> options)
        : base(options)
    {
    }

    public virtual DbSet<TblBooking> TblBookings { get; set; }

    public virtual DbSet<TblBookingRoom> TblBookingRooms { get; set; }

    public virtual DbSet<TblBookingService> TblBookingServices { get; set; }

    public virtual DbSet<TblEmployee> TblEmployees { get; set; }

    public virtual DbSet<TblFloor> TblFloors { get; set; }

    public virtual DbSet<TblGood> TblGoods { get; set; }

    public virtual DbSet<TblGuest> TblGuests { get; set; }

    public virtual DbSet<TblImportGood> TblImportGoods { get; set; }

    public virtual DbSet<TblImportGoodsDetail> TblImportGoodsDetails { get; set; }

    public virtual DbSet<TblPartner> TblPartners { get; set; }

    public virtual DbSet<TblPayment> TblPayments { get; set; }

    public virtual DbSet<TblRoom> TblRooms { get; set; }

    public virtual DbSet<TblService> TblServices { get; set; }

    public virtual DbSet<TblServiceGood> TblServiceGoods { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblBooking>(entity =>
        {
            entity.HasKey(e => e.BBookingId).HasName("PK__tbl_Book__63A3285106E60874");

            entity.Property(e => e.BBookingId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.BCreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.BTotalMoney).HasDefaultValue(0m);

            entity.HasOne(d => d.BGuest).WithMany(p => p.TblBookings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_Booki__b_Gue__4460231C");
        });

        modelBuilder.Entity<TblBookingRoom>(entity =>
        {
            entity.HasKey(e => e.BrBookingRoomsId).HasName("PK__tbl_Book__0D1D3016E66847B1");

            entity.Property(e => e.BrBookingRoomsId).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.BrBooking).WithMany(p => p.TblBookingRooms)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_Booki__br_Bo__5F141958");

            entity.HasOne(d => d.BrRoom).WithMany(p => p.TblBookingRooms)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_Booki__br_Ro__60083D91");
        });

        modelBuilder.Entity<TblBookingService>(entity =>
        {
            entity.HasKey(e => e.BsBookingServicesId).HasName("PK__tbl_Book__596EDF9908F51794");

            entity.Property(e => e.BsBookingServicesId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.BsCreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.BsBooking).WithMany(p => p.TblBookingServices)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_Booki__bs_Bo__567ED357");

            entity.HasOne(d => d.BsService).WithMany(p => p.TblBookingServices)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_Booki__bs_Se__5772F790");
        });

        modelBuilder.Entity<TblEmployee>(entity =>
        {
            entity.HasKey(e => e.EEmployeeId).HasName("PK__tbl_Empl__C8C1ED27F2E0DF2F");

            entity.Property(e => e.EEmployeeId).HasDefaultValueSql("(newid())");
        });

        modelBuilder.Entity<TblFloor>(entity =>
        {
            entity.HasKey(e => e.FFloorId).HasName("PK__tbl_Floo__051B7217F729A4C1");

            entity.Property(e => e.FFloorId).HasDefaultValueSql("(newid())");
        });

        modelBuilder.Entity<TblGood>(entity =>
        {
            entity.HasKey(e => e.GGoodsId).HasName("PK__tbl_Good__F3C565BF23D59FE8");

            entity.Property(e => e.GGoodsId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.GQuantity).HasDefaultValue(0);
        });

        modelBuilder.Entity<TblGuest>(entity =>
        {
            entity.HasKey(e => e.GGuestId).HasName("PK__tbl_Gues__246FE39FA3C461FC");

            entity.Property(e => e.GGuestId).HasDefaultValueSql("(newid())");
        });

        modelBuilder.Entity<TblImportGood>(entity =>
        {
            entity.HasKey(e => e.IgImportId).HasName("PK__tbl_Impo__80DAB00143D209F2");

            entity.Property(e => e.IgImportId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.IgImportDate).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<TblImportGoodsDetail>(entity =>
        {
            entity.HasKey(e => e.IgdId).HasName("PK__tbl_Impo__43F44C2769D6E00D");

            entity.Property(e => e.IgdId).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.IgdGoods).WithMany(p => p.TblImportGoodsDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_Impor__igd_G__7132C993");

            entity.HasOne(d => d.IgdImport).WithMany(p => p.TblImportGoodsDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_Impor__igd_I__703EA55A");
        });

        modelBuilder.Entity<TblPartner>(entity =>
        {
            entity.HasKey(e => e.PPartnerId).HasName("PK__tbl_Part__AE7A57A2AFFBA644");

            entity.Property(e => e.PPartnerId).HasDefaultValueSql("(newid())");
        });

        modelBuilder.Entity<TblPayment>(entity =>
        {
            entity.HasKey(e => e.PPaymentId).HasName("PK__tbl_Paym__8C197966241815CE");

            entity.Property(e => e.PPaymentId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.PPaymentDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.PBooking).WithMany(p => p.TblPayments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_Payme__p_Boo__4A18FC72");
        });

        modelBuilder.Entity<TblRoom>(entity =>
        {
            entity.HasKey(e => e.RRoomId).HasName("PK__tbl_Room__7CCD5DBBE6937381");

            entity.Property(e => e.RRoomId).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.RFloor).WithMany(p => p.TblRooms)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_Rooms__r_Flo__3CBF0154");
        });

        modelBuilder.Entity<TblService>(entity =>
        {
            entity.HasKey(e => e.SServiceId).HasName("PK__tbl_Serv__982F5085957AF391");

            entity.Property(e => e.SServiceId).HasDefaultValueSql("(newid())");
        });

        modelBuilder.Entity<TblServiceGood>(entity =>
        {
            entity.HasKey(e => e.SgServiceGoodsId).HasName("PK__tbl_Serv__1D5E6B96871F74F4");

            entity.Property(e => e.SgServiceGoodsId).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.SgGoods).WithMany(p => p.TblServiceGoods)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_Servi__sg_Go__689D8392");

            entity.HasOne(d => d.SgService).WithMany(p => p.TblServiceGoods)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_Servi__sg_Se__67A95F59");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
