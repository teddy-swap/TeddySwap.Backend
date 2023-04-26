using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeddySwap.Sink.Data.Migrations.TeddySwapNftSinkDb
{
    /// <inheritdoc />
    public partial class UpdateNftSink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StakeAddress",
                table: "NftOwners",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StakeAddress",
                table: "NftOwners");
        }
    }
}
