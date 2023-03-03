﻿using System.Collections.Generic;
using System.Numerics;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeddySwap.Sink.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AddressVerifications",
                columns: table => new
                {
                    TestnetAddress = table.Column<string>(type: "text", nullable: false),
                    MainnetAddress = table.Column<string>(type: "text", nullable: false),
                    TestnetSignedData = table.Column<string>(type: "text", nullable: false),
                    MainnetSignedData = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressVerifications", x => x.TestnetAddress);
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
                    BlockNumber = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    VrfKeyhash = table.Column<string>(type: "text", nullable: false),
                    Slot = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Epoch = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    InvalidTransactions = table.Column<IEnumerable<ulong>>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blocks", x => x.BlockHash);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    TxHash = table.Column<string>(type: "text", nullable: false),
                    Index = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Blockhash = table.Column<string>(type: "text", nullable: true),
                    OrderType = table.Column<int>(type: "integer", nullable: false),
                    PoolDatum = table.Column<string>(type: "text", nullable: true),
                    OrderDatum = table.Column<string>(type: "text", nullable: true),
                    UserAddress = table.Column<string>(type: "text", nullable: false),
                    BatcherAddress = table.Column<string>(type: "text", nullable: false),
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
                    table.ForeignKey(
                        name: "FK_Orders_Blocks_Blockhash",
                        column: x => x.Blockhash,
                        principalTable: "Blocks",
                        principalColumn: "BlockHash",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Hash = table.Column<string>(type: "text", nullable: false),
                    Index = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Fee = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Blockhash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => new { x.Hash, x.Index });
                    table.ForeignKey(
                        name: "FK_Transactions_Blocks_Blockhash",
                        column: x => x.Blockhash,
                        principalTable: "Blocks",
                        principalColumn: "BlockHash",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Prices",
                columns: table => new
                {
                    TxHash = table.Column<string>(type: "text", nullable: false),
                    Index = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    PriceX = table.Column<decimal>(type: "numeric", nullable: false),
                    PriceY = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prices", x => new { x.TxHash, x.Index });
                    table.ForeignKey(
                        name: "FK_Prices_Orders_TxHash_Index",
                        columns: x => new { x.TxHash, x.Index },
                        principalTable: "Orders",
                        principalColumns: new[] { "TxHash", "Index" },
                        onDelete: ReferentialAction.Cascade);
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
                    DatumCbor = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TxOutputs", x => new { x.TxHash, x.Index });
                    table.ForeignKey(
                        name: "FK_TxOutputs_Transactions_TxHash_TxIndex",
                        columns: x => new { x.TxHash, x.TxIndex },
                        principalTable: "Transactions",
                        principalColumns: new[] { "Hash", "Index" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Assets",
                columns: table => new
                {
                    TxOutputHash = table.Column<string>(type: "text", nullable: false),
                    TxOutputIndex = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    PolicyId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => new { x.PolicyId, x.Name, x.TxOutputHash, x.TxOutputIndex });
                    table.ForeignKey(
                        name: "FK_Assets_TxOutputs_TxOutputHash_TxOutputIndex",
                        columns: x => new { x.TxOutputHash, x.TxOutputIndex },
                        principalTable: "TxOutputs",
                        principalColumns: new[] { "TxHash", "Index" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assets_TxOutputHash_TxOutputIndex",
                table: "Assets",
                columns: new[] { "TxOutputHash", "TxOutputIndex" });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Blockhash",
                table: "Orders",
                column: "Blockhash");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_Blockhash",
                table: "Transactions",
                column: "Blockhash");

            migrationBuilder.CreateIndex(
                name: "IX_TxOutputs_TxHash_TxIndex",
                table: "TxOutputs",
                columns: new[] { "TxHash", "TxIndex" });
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
                name: "Prices");

            migrationBuilder.DropTable(
                name: "TxOutputs");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Blocks");
        }
    }
}
