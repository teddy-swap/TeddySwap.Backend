using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeddySwap.Data.Migrations
{
    /// <inheritdoc />
    public partial class YieldRewardByAddressKeyUpdate1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_YieldRewardByAddress",
                schema: "teddyswap-mainnet-v2",
                table: "YieldRewardByAddress");

            migrationBuilder.DropColumn(
                name: "Apr",
                schema: "teddyswap-mainnet-v2",
                table: "YieldRewardByAddress");

            migrationBuilder.AlterColumn<string>(
                name: "ClaimTxId",
                schema: "teddyswap-mainnet-v2",
                table: "YieldRewardByAddress",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<decimal>(
                name: "LPAmount",
                schema: "teddyswap-mainnet-v2",
                table: "YieldRewardByAddress",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_YieldRewardByAddress",
                schema: "teddyswap-mainnet-v2",
                table: "YieldRewardByAddress",
                columns: new[] { "Address", "BlockNumber", "Slot", "PoolId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_YieldRewardByAddress",
                schema: "teddyswap-mainnet-v2",
                table: "YieldRewardByAddress");

            migrationBuilder.DropColumn(
                name: "LPAmount",
                schema: "teddyswap-mainnet-v2",
                table: "YieldRewardByAddress");

            migrationBuilder.AlterColumn<string>(
                name: "ClaimTxId",
                schema: "teddyswap-mainnet-v2",
                table: "YieldRewardByAddress",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Apr",
                schema: "teddyswap-mainnet-v2",
                table: "YieldRewardByAddress",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_YieldRewardByAddress",
                schema: "teddyswap-mainnet-v2",
                table: "YieldRewardByAddress",
                columns: new[] { "Address", "BlockNumber", "Slot" });
        }
    }
}
