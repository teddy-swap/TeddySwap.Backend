using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeddySwap.Data.Migrations
{
    /// <inheritdoc />
    public partial class YieldClaimRequests3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ProcessBlockNumber",
                schema: "teddyswap-mainnet-v2",
                table: "YieldClaimRequests",
                type: "numeric(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ProcessSlot",
                schema: "teddyswap-mainnet-v2",
                table: "YieldClaimRequests",
                type: "numeric(20,0)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProcessBlockNumber",
                schema: "teddyswap-mainnet-v2",
                table: "YieldClaimRequests");

            migrationBuilder.DropColumn(
                name: "ProcessSlot",
                schema: "teddyswap-mainnet-v2",
                table: "YieldClaimRequests");
        }
    }
}
