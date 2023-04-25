using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeddySwap.Sink.Data.Migrations.TeddySwapNftSinkDb
{
    /// <inheritdoc />
    public partial class UpdatedNfSink2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MintTransactions",
                table: "MintTransactions");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MintTransactions",
                table: "MintTransactions",
                columns: new[] { "PolicyId", "TokenName", "TxHash" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MintTransactions",
                table: "MintTransactions");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MintTransactions",
                table: "MintTransactions",
                columns: new[] { "PolicyId", "TokenName" });
        }
    }
}
