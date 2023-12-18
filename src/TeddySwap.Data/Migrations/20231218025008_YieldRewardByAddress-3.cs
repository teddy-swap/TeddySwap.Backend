using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeddySwap.Data.Migrations
{
    /// <inheritdoc />
    public partial class YieldRewardByAddress3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ClaimBlockNumber",
                schema: "teddyswap-mainnet-v2",
                table: "YieldRewardByAddress",
                type: "numeric(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ClaimSlot",
                schema: "teddyswap-mainnet-v2",
                table: "YieldRewardByAddress",
                type: "numeric(20,0)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClaimBlockNumber",
                schema: "teddyswap-mainnet-v2",
                table: "YieldRewardByAddress");

            migrationBuilder.DropColumn(
                name: "ClaimSlot",
                schema: "teddyswap-mainnet-v2",
                table: "YieldRewardByAddress");
        }
    }
}
