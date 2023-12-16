using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeddySwap.Data.Migrations
{
    /// <inheritdoc />
    public partial class LedgerStateByAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LedgerStateByAddress",
                schema: "teddyswap-mainnet-v2",
                columns: table => new
                {
                    Address = table.Column<string>(type: "text", nullable: false),
                    BlockNumber = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Slot = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    OutputsJson = table.Column<JsonElement>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LedgerStateByAddress", x => new { x.Address, x.BlockNumber, x.Slot });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LedgerStateByAddress",
                schema: "teddyswap-mainnet-v2");
        }
    }
}
