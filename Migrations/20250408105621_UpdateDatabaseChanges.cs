using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hotel_be.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabaseChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__tbl_Booki__bs_Se__0BE6BFCF",
                table: "tbl_BookingServices");

            migrationBuilder.DropForeignKey(
                name: "FK__tbl_Impor__igd_G__7132C993",
                table: "tbl_ImportGoodsDetails");

            migrationBuilder.DropTable(
                name: "tbl_ServiceGoods");

            migrationBuilder.DropTable(
                name: "tbl_Goods");

            migrationBuilder.DropTable(
                name: "tbl_Services");

            migrationBuilder.RenameColumn(
                name: "bs_BookingServicesID",
                table: "tbl_BookingServices",
                newName: "bs_ID");

            migrationBuilder.RenameColumn(
                name: "br_BookingRoomsID",
                table: "tbl_BookingRooms",
                newName: "br_ID");

            migrationBuilder.CreateTable(
                name: "tbl_Products",
                columns: table => new
                {
                    p_ProductID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    p_ProductName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    p_Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    p_Quantity = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    p_Unit = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    p_CostPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    p_SellingPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    p_Currency = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    p_IsService = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tbl_Good__F3C565BF23D59FE8", x => x.p_ProductID);
                });

            migrationBuilder.CreateTable(
                name: "tbl_ServicePackages",
                columns: table => new
                {
                    sp_PackageID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    sp_PackageName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    s_ServiceCostPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    s_ServiceSellPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tbl_Serv__982F5085957AF391", x => x.sp_PackageID);
                });

            migrationBuilder.CreateTable(
                name: "tbl_PackageDetails",
                columns: table => new
                {
                    pd_DetailID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    pd_PackageID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    pd_ProductID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    pd_Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tbl_Serv__1D5E6B96871F74F4", x => x.pd_DetailID);
                    table.ForeignKey(
                        name: "FK__tbl_Servi__sg_Go__689D8392",
                        column: x => x.pd_ProductID,
                        principalTable: "tbl_Products",
                        principalColumn: "p_ProductID");
                    table.ForeignKey(
                        name: "FK__tbl_Servi__sg_Se__67A95F59",
                        column: x => x.pd_PackageID,
                        principalTable: "tbl_ServicePackages",
                        principalColumn: "sp_PackageID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_PackageDetails_pd_PackageID",
                table: "tbl_PackageDetails",
                column: "pd_PackageID");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_PackageDetails_pd_ProductID",
                table: "tbl_PackageDetails",
                column: "pd_ProductID");

            migrationBuilder.AddForeignKey(
                name: "FK__tbl_Booki__bs_Se__0BE6BFCF",
                table: "tbl_BookingServices",
                column: "bs_ServiceID",
                principalTable: "tbl_ServicePackages",
                principalColumn: "sp_PackageID");

            migrationBuilder.AddForeignKey(
                name: "FK__tbl_Impor__igd_G__7132C993",
                table: "tbl_ImportGoodsDetails",
                column: "igd_GoodsID",
                principalTable: "tbl_Products",
                principalColumn: "p_ProductID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__tbl_Booki__bs_Se__0BE6BFCF",
                table: "tbl_BookingServices");

            migrationBuilder.DropForeignKey(
                name: "FK__tbl_Impor__igd_G__7132C993",
                table: "tbl_ImportGoodsDetails");

            migrationBuilder.DropTable(
                name: "tbl_PackageDetails");

            migrationBuilder.DropTable(
                name: "tbl_Products");

            migrationBuilder.DropTable(
                name: "tbl_ServicePackages");

            migrationBuilder.RenameColumn(
                name: "bs_ID",
                table: "tbl_BookingServices",
                newName: "bs_BookingServicesID");

            migrationBuilder.RenameColumn(
                name: "br_ID",
                table: "tbl_BookingRooms",
                newName: "br_BookingRoomsID");

            migrationBuilder.CreateTable(
                name: "tbl_Goods",
                columns: table => new
                {
                    g_GoodsID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    g_Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    g_CostPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    g_Currency = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    g_GoodsName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    g_Quantity = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    g_SellingPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    g_Unit = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tbl_Good__F3C565BF23D59FE8", x => x.g_GoodsID);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Services",
                columns: table => new
                {
                    s_ServiceID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    s_ServiceCostPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    s_ServiceName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    s_ServiceSellPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tbl_Serv__982F5085957AF391", x => x.s_ServiceID);
                });

            migrationBuilder.CreateTable(
                name: "tbl_ServiceGoods",
                columns: table => new
                {
                    sg_ServiceGoodsID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    sg_GoodsID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    sg_ServiceID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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

            migrationBuilder.CreateIndex(
                name: "IX_tbl_ServiceGoods_sg_GoodsID",
                table: "tbl_ServiceGoods",
                column: "sg_GoodsID");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_ServiceGoods_sg_ServiceID",
                table: "tbl_ServiceGoods",
                column: "sg_ServiceID");

            migrationBuilder.AddForeignKey(
                name: "FK__tbl_Booki__bs_Se__0BE6BFCF",
                table: "tbl_BookingServices",
                column: "bs_ServiceID",
                principalTable: "tbl_Services",
                principalColumn: "s_ServiceID");

            migrationBuilder.AddForeignKey(
                name: "FK__tbl_Impor__igd_G__7132C993",
                table: "tbl_ImportGoodsDetails",
                column: "igd_GoodsID",
                principalTable: "tbl_Goods",
                principalColumn: "g_GoodsID");
        }
    }
}
