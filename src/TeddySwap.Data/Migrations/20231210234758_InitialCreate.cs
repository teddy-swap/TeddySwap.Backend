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
            migrationBuilder.EnsureSchema(
                name: "teddyswap-mainnet-v2");

            migrationBuilder.CreateTable(
                name: "Blocks",
                schema: "teddyswap-mainnet-v2",
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
                schema: "teddyswap-mainnet-v2",
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
                schema: "teddyswap-mainnet-v2",
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
                schema: "teddyswap-mainnet-v2",
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
                name: "Blocks",
                schema: "teddyswap-mainnet-v2");

            migrationBuilder.DropTable(
                name: "LiquidityByAddress",
                schema: "teddyswap-mainnet-v2");

            migrationBuilder.DropTable(
                name: "LovelaceByAddress",
                schema: "teddyswap-mainnet-v2");

            migrationBuilder.DropTable(
                name: "TransactionOutputs",
                schema: "teddyswap-mainnet-v2");
        }
    }
}
