using System.Collections.Generic;
using System.Numerics;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeddySwap.Sink.Data.Migrations.TeddySwapOrderSinkDb
{
    /// <inheritdoc />
    public partial class OrderInitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AddressVerifications",
                columns: table => new
                {
                    TestnetAddress = table.Column<string>(type: "text", nullable: false),
                    MainnetAddress = table.Column<string>(type: "text", nullable: true),
                    TestnetSignedData = table.Column<string>(type: "text", nullable: false),
                    MainnetSignedData = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressVerifications", x => x.TestnetAddress);
                });

            migrationBuilder.CreateTable(
                name: "Assets",
                columns: table => new
                {
                    TxOutputHash = table.Column<string>(type: "text", nullable: false),
                    TxOutputIndex = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    PolicyId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    BlockHash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => new { x.PolicyId, x.Name, x.TxOutputHash, x.TxOutputIndex });
                });

            migrationBuilder.CreateTable(
                name: "BlacklistedAddresses",
                columns: table => new
                {
                    Address = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlacklistedAddresses", x => x.Address);
                });

            migrationBuilder.CreateTable(
                name: "Blocks",
                columns: table => new
                {
                    BlockHash = table.Column<string>(type: "text", nullable: false),
                    Era = table.Column<string>(type: "text", nullable: false),
                    VrfKeyhash = table.Column<string>(type: "text", nullable: false),
                    BlockNumber = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Slot = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Epoch = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    InvalidTransactions = table.Column<IEnumerable<ulong>>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blocks", x => x.BlockHash);
                });

            migrationBuilder.CreateTable(
                name: "CollateralTxIns",
                columns: table => new
                {
                    TxHash = table.Column<string>(type: "text", nullable: false),
                    TxOutputHash = table.Column<string>(type: "text", nullable: false),
                    TxOutputIndex = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    BlockHash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollateralTxIns", x => new { x.TxHash, x.TxOutputHash, x.TxOutputIndex });
                });

            migrationBuilder.CreateTable(
                name: "CollateralTxOuts",
                columns: table => new
                {
                    TxHash = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    BlockHash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollateralTxOuts", x => new { x.Address, x.TxHash });
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    TxHash = table.Column<string>(type: "text", nullable: false),
                    Index = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Blockhash = table.Column<string>(type: "text", nullable: false),
                    OrderType = table.Column<int>(type: "integer", nullable: false),
                    PoolDatum = table.Column<byte[]>(type: "bytea", nullable: true),
                    OrderDatum = table.Column<byte[]>(type: "bytea", nullable: true),
                    UserAddress = table.Column<string>(type: "text", nullable: false),
                    BatcherAddress = table.Column<string>(type: "text", nullable: true),
                    AssetX = table.Column<string>(type: "text", nullable: false),
                    AssetY = table.Column<string>(type: "text", nullable: false),
                    AssetLq = table.Column<string>(type: "text", nullable: false),
                    PoolNft = table.Column<string>(type: "text", nullable: false),
                    OrderBase = table.Column<string>(type: "text", nullable: false),
                    ReservesX = table.Column<BigInteger>(type: "numeric", nullable: false),
                    ReservesY = table.Column<BigInteger>(type: "numeric", nullable: false),
                    Liquidity = table.Column<BigInteger>(type: "numeric", nullable: false),
                    OrderX = table.Column<BigInteger>(type: "numeric", nullable: false),
                    OrderY = table.Column<BigInteger>(type: "numeric", nullable: false),
                    OrderLq = table.Column<BigInteger>(type: "numeric", nullable: false),
                    Slot = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => new { x.TxHash, x.Index });
                });

            migrationBuilder.CreateTable(
                name: "Prices",
                columns: table => new
                {
                    TxHash = table.Column<string>(type: "text", nullable: false),
                    Index = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    PriceX = table.Column<decimal>(type: "numeric", nullable: false),
                    PriceY = table.Column<decimal>(type: "numeric", nullable: false),
                    AssetX = table.Column<string>(type: "text", nullable: false),
                    AssetY = table.Column<string>(type: "text", nullable: false),
                    AssetLq = table.Column<string>(type: "text", nullable: false),
                    PoolNft = table.Column<string>(type: "text", nullable: false),
                    Slot = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Blockhash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prices", x => new { x.TxHash, x.Index });
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Hash = table.Column<string>(type: "text", nullable: false),
                    Index = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Fee = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    BlockHash = table.Column<string>(type: "text", nullable: false),
                    HasCollateralOutput = table.Column<bool>(type: "boolean", nullable: false),
                    Metadata = table.Column<string>(type: "text", nullable: true),
                    IsValid = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Hash);
                });

            migrationBuilder.CreateTable(
                name: "TxInputs",
                columns: table => new
                {
                    TxHash = table.Column<string>(type: "text", nullable: false),
                    TxOutputHash = table.Column<string>(type: "text", nullable: false),
                    TxOutputIndex = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    BlockHash = table.Column<string>(type: "text", nullable: false),
                    InlineDatum = table.Column<byte>(type: "smallint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TxInputs", x => new { x.TxHash, x.TxOutputHash, x.TxOutputIndex, x.BlockHash });
                });

            migrationBuilder.CreateTable(
                name: "TxOutputs",
                columns: table => new
                {
                    TxHash = table.Column<string>(type: "text", nullable: false),
                    Index = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    TxIndex = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    DatumCbor = table.Column<string>(type: "text", nullable: true),
                    BlockHash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TxOutputs", x => new { x.TxHash, x.Index });
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_BatcherAddress",
                table: "Orders",
                column: "BatcherAddress");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderType",
                table: "Orders",
                column: "OrderType");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Slot",
                table: "Orders",
                column: "Slot");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserAddress",
                table: "Orders",
                column: "UserAddress");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AddressVerifications");

            migrationBuilder.DropTable(
                name: "Assets");

            migrationBuilder.DropTable(
                name: "BlacklistedAddresses");

            migrationBuilder.DropTable(
                name: "Blocks");

            migrationBuilder.DropTable(
                name: "CollateralTxIns");

            migrationBuilder.DropTable(
                name: "CollateralTxOuts");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Prices");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "TxInputs");

            migrationBuilder.DropTable(
                name: "TxOutputs");
        }
    }
}
