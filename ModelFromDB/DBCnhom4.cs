using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace hotel_be.ModelFromDB;

public partial class DBCnhom4 : IdentityDbContext<IdentityUser>

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

    public virtual DbSet<TblGuest> TblGuests { get; set; }

    public virtual DbSet<TblImportGood> TblImportGoods { get; set; }

    public virtual DbSet<TblImportGoodsDetail> TblImportGoodsDetails { get; set; }

    public virtual DbSet<TblPackageDetail> TblPackageDetails { get; set; }

    public virtual DbSet<TblPartner> TblPartners { get; set; }

    public virtual DbSet<TblPayment> TblPayments { get; set; }

    public virtual DbSet<TblProduct> TblProducts { get; set; }

    public virtual DbSet<TblRoom> TblRooms { get; set; }

    public virtual DbSet<TblServicePackage> TblServicePackages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TblBooking>(entity =>
        {
            entity.HasKey(e => e.BBookingId).HasName("PK__tbl_Book__63A328519228D519");

            entity.ToTable("tbl_Bookings");

            entity.Property(e => e.BBookingId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("b_BookingID");
            entity.Property(e => e.BBookingStatus)
                .HasMaxLength(20)
                .HasColumnName("b_BookingStatus");
            entity.Property(e => e.BCreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("b_CreatedAt");
            entity.Property(e => e.BDeposit)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("b_Deposit");
            entity.Property(e => e.BGuestId).HasColumnName("b_GuestID");
            entity.Property(e => e.BTotalMoney)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("b_TotalMoney");

            entity.HasOne(d => d.BGuest).WithMany(p => p.TblBookings)
                .HasForeignKey(d => d.BGuestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_Booki__b_Gue__7BB05806");
        });

        modelBuilder.Entity<TblBookingRoom>(entity =>
        {
            entity.HasKey(e => e.BrId).HasName("PK__tbl_Book__0D1D3016859B4CCE");

            entity.ToTable("tbl_BookingRooms");

            entity.Property(e => e.BrId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("br_ID");
            entity.Property(e => e.BrBookingId).HasColumnName("br_BookingID");
            entity.Property(e => e.BrCheckInDate)
                .HasColumnType("datetime")
                .HasColumnName("br_CheckInDate");
            entity.Property(e => e.BrCheckOutDate)
                .HasColumnType("datetime")
                .HasColumnName("br_CheckOutDate");
            entity.Property(e => e.BrRoomId).HasColumnName("br_RoomID");

            entity.HasOne(d => d.BrBooking).WithMany(p => p.TblBookingRooms)
                .HasForeignKey(d => d.BrBookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_Booki__br_Bo__04459E07");

            entity.HasOne(d => d.BrRoom).WithMany(p => p.TblBookingRooms)
                .HasForeignKey(d => d.BrRoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_Booki__br_Ro__0539C240");
        });

        modelBuilder.Entity<TblBookingService>(entity =>
        {
            entity.HasKey(e => e.BsId).HasName("PK__tbl_Book__596EDF990FEA40A2");

            entity.ToTable("tbl_BookingServices");

            entity.Property(e => e.BsId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("bs_ID");
            entity.Property(e => e.BsBookingId).HasColumnName("bs_BookingID");
            entity.Property(e => e.BsCreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("bs_CreatedAt");
            entity.Property(e => e.BsQuantity).HasColumnName("bs_Quantity");
            entity.Property(e => e.BsServiceId).HasColumnName("bs_ServiceID");

            entity.HasOne(d => d.BsBooking).WithMany(p => p.TblBookingServices)
                .HasForeignKey(d => d.BsBookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_Booki__bs_Bo__0AF29B96");

            entity.HasOne(d => d.BsService).WithMany(p => p.TblBookingServices)
                .HasForeignKey(d => d.BsServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_Booki__bs_Se__0BE6BFCF");
        });

        modelBuilder.Entity<TblEmployee>(entity =>
        {
            entity.HasKey(e => e.EEmployeeId).HasName("PK__tbl_Empl__C8C1ED27F2E0DF2F");

            entity.ToTable("tbl_Employees");

            entity.HasIndex(e => e.EEmail, "UQ__tbl_Empl__3908DC82E918A11D").IsUnique();

            entity.Property(e => e.EEmployeeId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("e_EmployeeID");
            entity.Property(e => e.EEmail)
                .HasMaxLength(100)
                .HasColumnName("e_Email");
            entity.Property(e => e.EFirstName)
                .HasMaxLength(50)
                .HasColumnName("e_FirstName");
            entity.Property(e => e.ELastName)
                .HasMaxLength(50)
                .HasColumnName("e_LastName");
            entity.Property(e => e.EPhoneNumber)
                .HasMaxLength(15)
                .HasColumnName("e_PhoneNumber");
            entity.Property(e => e.EPosition)
                .HasMaxLength(50)
                .HasColumnName("e_Position");
            entity.Property(e => e.ESalary)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("e_Salary");
        });

        modelBuilder.Entity<TblFloor>(entity =>
        {
            entity.HasKey(e => e.FFloorId).HasName("PK__tbl_Floo__051B7217F729A4C1");

            entity.ToTable("tbl_Floors");

            entity.Property(e => e.FFloorId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("f_FloorID");
            entity.Property(e => e.FFloor)
                .HasMaxLength(10)
                .HasColumnName("f_Floor");
        });

        modelBuilder.Entity<TblGuest>(entity =>
        {
            entity.HasKey(e => e.GGuestId).HasName("PK__tbl_Gues__246FE39FA3C461FC");

            entity.ToTable("tbl_Guests");

            entity.HasIndex(e => e.GEmail, "UQ__tbl_Gues__317ADEF737721475").IsUnique();

            entity.Property(e => e.GGuestId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("g_GuestID");
            entity.Property(e => e.GEmail)
                .HasMaxLength(100)
                .HasColumnName("g_Email");
            entity.Property(e => e.GFirstName)
                .HasMaxLength(50)
                .HasColumnName("g_FirstName");
            entity.Property(e => e.GLastName)
                .HasMaxLength(50)
                .HasColumnName("g_LastName");
            entity.Property(e => e.GPhoneNumber)
                .HasMaxLength(15)
                .HasColumnName("g_PhoneNumber");
        });

        modelBuilder.Entity<TblImportGood>(entity =>
        {
            entity.HasKey(e => e.IgImportId).HasName("PK__tbl_Impo__80DAB00143D209F2");

            entity.ToTable("tbl_ImportGoods");

            entity.Property(e => e.IgImportId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ig_ImportID");
            entity.Property(e => e.IgCurrency)
                .HasMaxLength(30)
                .HasColumnName("ig_Currency");
            entity.Property(e => e.IgImportDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("ig_ImportDate");
            entity.Property(e => e.IgSumPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("ig_SumPrice");
            entity.Property(e => e.IgSupplier)
                .HasMaxLength(200)
                .HasColumnName("ig_Supplier");
        });

        modelBuilder.Entity<TblImportGoodsDetail>(entity =>
        {
            entity.HasKey(e => e.IgdId).HasName("PK__tbl_Impo__43F44C2769D6E00D");

            entity.ToTable("tbl_ImportGoodsDetails");

            entity.Property(e => e.IgdId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("igd_ID");
            entity.Property(e => e.IgdCostPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("igd_CostPrice");
            entity.Property(e => e.IgdGoodsId).HasColumnName("igd_GoodsID");
            entity.Property(e => e.IgdImportId).HasColumnName("igd_ImportID");
            entity.Property(e => e.IgdQuantity).HasColumnName("igd_Quantity");

            entity.HasOne(d => d.IgdGoods).WithMany(p => p.TblImportGoodsDetails)
                .HasForeignKey(d => d.IgdGoodsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_Impor__igd_G__7132C993");

            entity.HasOne(d => d.IgdImport).WithMany(p => p.TblImportGoodsDetails)
                .HasForeignKey(d => d.IgdImportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_Impor__igd_I__703EA55A");
        });

        modelBuilder.Entity<TblPackageDetail>(entity =>
        {
            entity.HasKey(e => e.PdDetailId).HasName("PK__tbl_Serv__1D5E6B96871F74F4");

            entity.ToTable("tbl_PackageDetails");

            entity.Property(e => e.PdDetailId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("pd_DetailID");
            entity.Property(e => e.PdPackageId).HasColumnName("pd_PackageID");
            entity.Property(e => e.PdProductId).HasColumnName("pd_ProductID");
            entity.Property(e => e.PdQuantity).HasColumnName("pd_Quantity");

            entity.HasOne(d => d.PdPackage).WithMany(p => p.TblPackageDetails)
                .HasForeignKey(d => d.PdPackageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_Servi__sg_Se__67A95F59");

            entity.HasOne(d => d.PdProduct).WithMany(p => p.TblPackageDetails)
                .HasForeignKey(d => d.PdProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_Servi__sg_Go__689D8392");
        });

        modelBuilder.Entity<TblPartner>(entity =>
        {
            entity.HasKey(e => e.PPartnerId).HasName("PK__tbl_Part__AE7A57A2AFFBA644");

            entity.ToTable("tbl_Partner");

            entity.HasIndex(e => e.PEmail, "UQ__tbl_Part__1DFC0D6D78CD48CF").IsUnique();

            entity.Property(e => e.PPartnerId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("p_PartnerID");
            entity.Property(e => e.PAddress)
                .HasMaxLength(50)
                .HasColumnName("p_Address");
            entity.Property(e => e.PEmail)
                .HasMaxLength(255)
                .HasColumnName("p_Email");
            entity.Property(e => e.PPartnerName)
                .HasMaxLength(255)
                .HasColumnName("p_PartnerName");
            entity.Property(e => e.PPartnerType)
                .HasMaxLength(100)
                .HasColumnName("p_PartnerType");
            entity.Property(e => e.PPhoneNumber)
                .HasMaxLength(15)
                .HasColumnName("p_PhoneNumber");
        });

        modelBuilder.Entity<TblPayment>(entity =>
        {
            entity.HasKey(e => e.PPaymentId).HasName("PK__tbl_Paym__8C19796638122A06");

            entity.ToTable("tbl_Payments");

            entity.Property(e => e.PPaymentId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("p_PaymentID");
            entity.Property(e => e.PAmountPaid)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("p_AmountPaid");
            entity.Property(e => e.PBookingId).HasColumnName("p_BookingID");
            entity.Property(e => e.PPaymentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("p_PaymentDate");
            entity.Property(e => e.PPaymentMethod)
                .HasMaxLength(50)
                .HasColumnName("p_PaymentMethod");

            entity.HasOne(d => d.PBooking).WithMany(p => p.TblPayments)
                .HasForeignKey(d => d.PBookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_Payme__p_Boo__00750D23");
        });

        modelBuilder.Entity<TblProduct>(entity =>
        {
            entity.HasKey(e => e.PProductId).HasName("PK__tbl_Good__F3C565BF23D59FE8");

            entity.ToTable("tbl_Products");

            entity.Property(e => e.PProductId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("p_ProductID");
            entity.Property(e => e.PCategory)
                .HasMaxLength(100)
                .HasColumnName("p_Category");
            entity.Property(e => e.PCostPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("p_CostPrice");
            entity.Property(e => e.PCurrency)
                .HasMaxLength(30)
                .HasColumnName("p_Currency");
            entity.Property(e => e.PIsService)
                .HasDefaultValue(false)
                .HasColumnName("p_IsService");
            entity.Property(e => e.PProductName)
                .HasMaxLength(255)
                .HasColumnName("p_ProductName");
            entity.Property(e => e.PQuantity)
                .HasDefaultValue(0)
                .HasColumnName("p_Quantity");
            entity.Property(e => e.PSellingPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("p_SellingPrice");
            entity.Property(e => e.PUnit)
                .HasMaxLength(30)
                .HasColumnName("p_Unit");
        });

        modelBuilder.Entity<TblRoom>(entity =>
        {
            entity.HasKey(e => e.RRoomId).HasName("PK__tbl_Room__7CCD5DBBE6937381");

            entity.ToTable("tbl_Rooms");

            entity.HasIndex(e => e.RRoomNumber, "UQ__tbl_Room__64AD69DE794F86F1").IsUnique();

            entity.Property(e => e.RRoomId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("r_RoomID");
            entity.Property(e => e.RFloorId).HasColumnName("r_FloorID");
            entity.Property(e => e.RPricePerHour)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("r_PricePerHour");
            entity.Property(e => e.RRoomNumber)
                .HasMaxLength(10)
                .HasColumnName("r_RoomNumber");
            entity.Property(e => e.RRoomType)
                .HasMaxLength(50)
                .HasColumnName("r_RoomType");
            entity.Property(e => e.RStatus)
                .HasMaxLength(20)
                .HasColumnName("r_Status");

            entity.HasOne(d => d.RFloor).WithMany(p => p.TblRooms)
                .HasForeignKey(d => d.RFloorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbl_Rooms__r_Flo__3CBF0154");
        });

        modelBuilder.Entity<TblServicePackage>(entity =>
        {
            entity.HasKey(e => e.SpPackageId).HasName("PK__tbl_Serv__982F5085957AF391");

            entity.ToTable("tbl_ServicePackages");

            entity.Property(e => e.SpPackageId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("sp_PackageID");
            entity.Property(e => e.SServiceCostPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("s_ServiceCostPrice");
            entity.Property(e => e.SServiceSellPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("s_ServiceSellPrice");
            entity.Property(e => e.SpPackageName)
                .HasMaxLength(100)
                .HasColumnName("sp_PackageName");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
