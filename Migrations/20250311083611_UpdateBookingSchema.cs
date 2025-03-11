using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hotel_be.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBookingSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_Employees",
                columns: table => new
                {
                    e_EmployeeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    e_FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    e_LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    e_Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    e_PhoneNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    e_Position = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    e_Salary = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tbl_Empl__C8C1ED27F2E0DF2F", x => x.e_EmployeeID);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Floors",
                columns: table => new
                {
                    f_FloorID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    f_Floor = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tbl_Floo__051B7217F729A4C1", x => x.f_FloorID);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Goods",
                columns: table => new
                {
                    g_GoodsID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    g_GoodsName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    g_Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    g_Quantity = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    g_Unit = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    g_CostPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    g_SellingPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    g_Currency = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tbl_Good__F3C565BF23D59FE8", x => x.g_GoodsID);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Guests",
                columns: table => new
                {
                    g_GuestID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    g_FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    g_LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    g_Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    g_PhoneNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tbl_Gues__246FE39FA3C461FC", x => x.g_GuestID);
                });

            migrationBuilder.CreateTable(
                name: "tbl_ImportGoods",
                columns: table => new
                {
                    ig_ImportID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    ig_SumPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ig_Currency = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ig_ImportDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    ig_Supplier = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tbl_Impo__80DAB00143D209F2", x => x.ig_ImportID);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Partner",
                columns: table => new
                {
                    p_PartnerID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    p_PartnerName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    p_PartnerType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    p_PhoneNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    p_Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    p_Address = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tbl_Part__AE7A57A2AFFBA644", x => x.p_PartnerID);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Services",
                columns: table => new
                {
                    s_ServiceID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    s_ServiceName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    s_ServiceCostPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    s_ServiceSellPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tbl_Serv__982F5085957AF391", x => x.s_ServiceID);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Rooms",
                columns: table => new
                {
                    r_RoomID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    r_RoomNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    r_FloorID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    r_RoomType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    r_PricePerHour = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    r_Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tbl_Room__7CCD5DBBE6937381", x => x.r_RoomID);
                    table.ForeignKey(
                        name: "FK__tbl_Rooms__r_Flo__3CBF0154",
                        column: x => x.r_FloorID,
                        principalTable: "tbl_Floors",
                        principalColumn: "f_FloorID");
                });

            migrationBuilder.CreateTable(
                name: "tbl_Bookings",
                columns: table => new
                {
                    b_BookingID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    b_GuestID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    b_BookingStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    b_TotalMoney = table.Column<decimal>(type: "decimal(10,2)", nullable: true, defaultValue: 0m),
                    b_Deposit = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    b_CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tbl_Book__63A328519228D519", x => x.b_BookingID);
                    table.ForeignKey(
                        name: "FK__tbl_Booki__b_Gue__7BB05806",
                        column: x => x.b_GuestID,
                        principalTable: "tbl_Guests",
                        principalColumn: "g_GuestID");
                });

            migrationBuilder.CreateTable(
                name: "tbl_ImportGoodsDetails",
                columns: table => new
                {
                    igd_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    igd_ImportID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    igd_GoodsID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    igd_Quantity = table.Column<int>(type: "int", nullable: false),
                    igd_CostPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tbl_Impo__43F44C2769D6E00D", x => x.igd_ID);
                    table.ForeignKey(
                        name: "FK__tbl_Impor__igd_G__7132C993",
                        column: x => x.igd_GoodsID,
                        principalTable: "tbl_Goods",
                        principalColumn: "g_GoodsID");
                    table.ForeignKey(
                        name: "FK__tbl_Impor__igd_I__703EA55A",
                        column: x => x.igd_ImportID,
                        principalTable: "tbl_ImportGoods",
                        principalColumn: "ig_ImportID");
                });

            migrationBuilder.CreateTable(
                name: "tbl_ServiceGoods",
                columns: table => new
                {
                    sg_ServiceGoodsID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    sg_ServiceID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    sg_GoodsID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    sg_Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tbl_Serv__1D5E6B96871F74F4", x => x.sg_ServiceGoodsID);
                    table.ForeignKey(
                        name: "FK__tbl_Servi__sg_Go__689D8392",
                        column: x => x.sg_GoodsID,
                        principalTable: "tbl_Goods",
                        principalColumn: "g_GoodsID");
                    table.ForeignKey(
                        name: "FK__tbl_Servi__sg_Se__67A95F59",
                        column: x => x.sg_ServiceID,
                        principalTable: "tbl_Services",
                        principalColumn: "s_ServiceID");
                });

            migrationBuilder.CreateTable(
                name: "tbl_BookingRooms",
                columns: table => new
                {
                    br_BookingRoomsID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    br_BookingID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    br_RoomID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    br_CheckInDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    br_CheckOutDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tbl_Book__0D1D3016859B4CCE", x => x.br_BookingRoomsID);
                    table.ForeignKey(
                        name: "FK__tbl_Booki__br_Bo__04459E07",
                        column: x => x.br_BookingID,
                        principalTable: "tbl_Bookings",
                        principalColumn: "b_BookingID");
                    table.ForeignKey(
                        name: "FK__tbl_Booki__br_Ro__0539C240",
                        column: x => x.br_RoomID,
                        principalTable: "tbl_Rooms",
                        principalColumn: "r_RoomID");
                });

            migrationBuilder.CreateTable(
                name: "tbl_BookingServices",
                columns: table => new
                {
                    bs_BookingServicesID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    bs_BookingID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    bs_ServiceID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    bs_Quantity = table.Column<int>(type: "int", nullable: false),
                    bs_CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tbl_Book__596EDF990FEA40A2", x => x.bs_BookingServicesID);
                    table.ForeignKey(
                        name: "FK__tbl_Booki__bs_Bo__0AF29B96",
                        column: x => x.bs_BookingID,
                        principalTable: "tbl_Bookings",
                        principalColumn: "b_BookingID");
                    table.ForeignKey(
                        name: "FK__tbl_Booki__bs_Se__0BE6BFCF",
                        column: x => x.bs_ServiceID,
                        principalTable: "tbl_Services",
                        principalColumn: "s_ServiceID");
                });

            migrationBuilder.CreateTable(
                name: "tbl_Payments",
                columns: table => new
                {
                    p_PaymentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    p_BookingID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    p_AmountPaid = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    p_PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    p_PaymentDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tbl_Paym__8C19796638122A06", x => x.p_PaymentID);
                    table.ForeignKey(
                        name: "FK__tbl_Payme__p_Boo__00750D23",
                        column: x => x.p_BookingID,
                        principalTable: "tbl_Bookings",
                        principalColumn: "b_BookingID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_BookingRooms_br_BookingID",
                table: "tbl_BookingRooms",
                column: "br_BookingID");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_BookingRooms_br_RoomID",
                table: "tbl_BookingRooms",
                column: "br_RoomID");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Bookings_b_GuestID",
                table: "tbl_Bookings",
                column: "b_GuestID");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_BookingServices_bs_BookingID",
                table: "tbl_BookingServices",
                column: "bs_BookingID");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_BookingServices_bs_ServiceID",
                table: "tbl_BookingServices",
                column: "bs_ServiceID");

            migrationBuilder.CreateIndex(
                name: "UQ__tbl_Empl__3908DC82E918A11D",
                table: "tbl_Employees",
                column: "e_Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__tbl_Gues__317ADEF737721475",
                table: "tbl_Guests",
                column: "g_Email",
                unique: true,
                filter: "[g_Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_ImportGoodsDetails_igd_GoodsID",
                table: "tbl_ImportGoodsDetails",
                column: "igd_GoodsID");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_ImportGoodsDetails_igd_ImportID",
                table: "tbl_ImportGoodsDetails",
                column: "igd_ImportID");

            migrationBuilder.CreateIndex(
                name: "UQ__tbl_Part__1DFC0D6D78CD48CF",
                table: "tbl_Partner",
                column: "p_Email",
                unique: true,
                filter: "[p_Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Payments_p_BookingID",
                table: "tbl_Payments",
                column: "p_BookingID");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Rooms_r_FloorID",
                table: "tbl_Rooms",
                column: "r_FloorID");

            migrationBuilder.CreateIndex(
                name: "UQ__tbl_Room__64AD69DE794F86F1",
                table: "tbl_Rooms",
                column: "r_RoomNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_ServiceGoods_sg_GoodsID",
                table: "tbl_ServiceGoods",
                column: "sg_GoodsID");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_ServiceGoods_sg_ServiceID",
                table: "tbl_ServiceGoods",
                column: "sg_ServiceID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_BookingRooms");

            migrationBuilder.DropTable(
                name: "tbl_BookingServices");

            migrationBuilder.DropTable(
                name: "tbl_Employees");

            migrationBuilder.DropTable(
                name: "tbl_ImportGoodsDetails");

            migrationBuilder.DropTable(
                name: "tbl_Partner");

            migrationBuilder.DropTable(
                name: "tbl_Payments");

            migrationBuilder.DropTable(
                name: "tbl_ServiceGoods");

            migrationBuilder.DropTable(
                name: "tbl_Rooms");

            migrationBuilder.DropTable(
                name: "tbl_ImportGoods");

            migrationBuilder.DropTable(
                name: "tbl_Bookings");

            migrationBuilder.DropTable(
                name: "tbl_Goods");

            migrationBuilder.DropTable(
                name: "tbl_Services");

            migrationBuilder.DropTable(
                name: "tbl_Floors");

            migrationBuilder.DropTable(
                name: "tbl_Guests");
        }
    }
}
