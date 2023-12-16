using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeddySwap.Data.Migrations
{
    /// <inheritdoc />
    public partial class YieldRewardByAddressBonus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Bonus",
                schema: "teddyswap-mainnet-v2",
                table: "YieldRewardByAddress",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string[]>(
                name: "TBCs",
                schema: "teddyswap-mainnet-v2",
                table: "YieldRewardByAddress",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bonus",
                schema: "teddyswap-mainnet-v2",
                table: "YieldRewardByAddress");

            migrationBuilder.DropColumn(
                name: "TBCs",
                schema: "teddyswap-mainnet-v2",
                table: "YieldRewardByAddress");
        }
    }
}
