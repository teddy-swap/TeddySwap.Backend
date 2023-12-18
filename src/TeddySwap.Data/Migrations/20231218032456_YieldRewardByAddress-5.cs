using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeddySwap.Data.Migrations
{
    /// <inheritdoc />
    public partial class YieldRewardByAddress5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClaimTxId",
                schema: "teddyswap-mainnet-v2",
                table: "YieldRewardByAddress",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClaimTxId",
                schema: "teddyswap-mainnet-v2",
                table: "YieldRewardByAddress");
        }
    }
}
