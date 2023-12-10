using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeddySwap.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Blocks",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Slot = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Number = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blocks", x => new { x.Id, x.Slot });
                });

            migrationBuilder.CreateTable(
                name: "LiquidityByAddress",
                columns: table => new
                {
                    Address = table.Column<string>(type: "text", nullable: false),
                    BlockNumber = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Slot = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    AssetsJson = table.Column<JsonElement>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiquidityByAddress", x => new { x.Address, x.BlockNumber, x.Slot });
                });

            migrationBuilder.CreateTable(
                name: "LovelaceByAddress",
                columns: table => new
                {
                    Address = table.Column<string>(type: "text", nullable: false),
                    BlockNumber = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Slot = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LovelaceByAddress", x => new { x.Address, x.BlockNumber, x.Slot });
                });

            migrationBuilder.CreateTable(
                name: "TransactionOutputs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Index = table.Column<long>(type: "bigint", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    Amount_Coin = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Amount_MultiAssetJson = table.Column<JsonElement>(type: "jsonb", nullable: false),
                    Slot = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionOutputs", x => new { x.Id, x.Index });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Blocks");

            migrationBuilder.DropTable(
                name: "LiquidityByAddress");

            migrationBuilder.DropTable(
                name: "LovelaceByAddress");

            migrationBuilder.DropTable(
                name: "TransactionOutputs");
        }
    }
}
