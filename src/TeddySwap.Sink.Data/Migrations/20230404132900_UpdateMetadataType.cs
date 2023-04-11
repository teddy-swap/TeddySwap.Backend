using System.Collections.Generic;
using System.Numerics;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeddySwap.Sink.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMetadataType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "FisoBonusDelegations",
                columns: table => new
                {
                    TxHash = table.Column<string>(type: "text", nullable: false),
                    Slot = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    StakeAddress = table.Column<string>(type: "text", nullable: false),
                    EpochNumber = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    BlockNumber = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    PoolId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FisoBonusDelegations", x => new { x.StakeAddress, x.TxHash, x.Slot });
                });

            migrationBuilder.CreateTable(
                name: "FisoDelegations",
                columns: table => new
                {
                    TxHash = table.Column<string>(type: "text", nullable: false),
                    Slot = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    StakeAddress = table.Column<string>(type: "text", nullable: false),
                    EpochNumber = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    BlockNumber = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    FromPoolId = table.Column<string>(type: "text", nullable: true),
                    ToPoolId = table.Column<string>(type: "text", nullable: false),
                    LiveStake = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FisoDelegations", x => new { x.StakeAddress, x.TxHash, x.Slot });
                });

            migrationBuilder.CreateTable(
                name: "FisoDelegators",
                columns: table => new
                {
                    StakeAddress = table.Column<string>(type: "text", nullable: false),
                    PoolId = table.Column<string>(type: "text", nullable: false),
                    Epoch = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    StakeAmount = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FisoDelegators", x => new { x.StakeAddress, x.PoolId, x.Epoch });
                });

            migrationBuilder.CreateTable(
                name: "FisoEpochRewards",
                columns: table => new
                {
                    EpochNumber = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    StakeAddress = table.Column<string>(type: "text", nullable: false),
                    PoolId = table.Column<string>(type: "text", nullable: false),
                    StakeAmount = table.Column<BigInteger>(type: "numeric", nullable: false),
                    SharePercentage = table.Column<decimal>(type: "numeric", nullable: false),
                    ShareAmount = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FisoEpochRewards", x => new { x.EpochNumber, x.StakeAddress });
                });

            migrationBuilder.CreateTable(
                name: "FisoPoolActiveStakes",
                columns: table => new
                {
                    EpochNumber = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    PoolId = table.Column<string>(type: "text", nullable: false),
                    StakeAmount = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FisoPoolActiveStakes", x => new { x.EpochNumber, x.PoolId });
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Hash = table.Column<string>(type: "text", nullable: false),
                    Index = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Fee = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Blockhash = table.Column<string>(type: "text", nullable: false),
                    HasCollateralOutput = table.Column<bool>(type: "boolean", nullable: false),
                    Metadata = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Hash);
                    table.ForeignKey(
                        name: "FK_Transactions_Blocks_Blockhash",
                        column: x => x.Blockhash,
                        principalTable: "Blocks",
                        principalColumn: "BlockHash",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CollateralTxIns",
                columns: table => new
                {
                    TxHash = table.Column<string>(type: "text", nullable: false),
                    TxOutputHash = table.Column<string>(type: "text", nullable: false),
                    TxOutputIndex = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollateralTxIns", x => new { x.TxHash, x.TxOutputHash, x.TxOutputIndex });
                    table.ForeignKey(
                        name: "FK_CollateralTxIns_Transactions_TxHash",
                        column: x => x.TxHash,
                        principalTable: "Transactions",
                        principalColumn: "Hash",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CollateralTxOuts",
                columns: table => new
                {
                    TxHash = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    TxIndex = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Index = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollateralTxOuts", x => new { x.Address, x.TxHash });
                    table.ForeignKey(
                        name: "FK_CollateralTxOuts_Transactions_TxHash",
                        column: x => x.TxHash,
                        principalTable: "Transactions",
                        principalColumn: "Hash",
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
                        name: "FK_TxOutputs_Transactions_TxHash",
                        column: x => x.TxHash,
                        principalTable: "Transactions",
                        principalColumn: "Hash",
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

            migrationBuilder.CreateTable(
                name: "TxInputs",
                columns: table => new
                {
                    TxHash = table.Column<string>(type: "text", nullable: false),
                    TxOutputHash = table.Column<string>(type: "text", nullable: false),
                    TxOutputIndex = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    InlineDatum = table.Column<byte>(type: "smallint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TxInputs", x => new { x.TxHash, x.TxOutputHash, x.TxOutputIndex });
                    table.ForeignKey(
                        name: "FK_TxInputs_Transactions_TxHash",
                        column: x => x.TxHash,
                        principalTable: "Transactions",
                        principalColumn: "Hash",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TxInputs_TxOutputs_TxOutputHash_TxOutputIndex",
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
                name: "IX_CollateralTxOuts_TxHash",
                table: "CollateralTxOuts",
                column: "TxHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_Blockhash",
                table: "Transactions",
                column: "Blockhash");

            migrationBuilder.CreateIndex(
                name: "IX_TxInputs_TxOutputHash_TxOutputIndex",
                table: "TxInputs",
                columns: new[] { "TxOutputHash", "TxOutputIndex" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Assets");

            migrationBuilder.DropTable(
                name: "CollateralTxIns");

            migrationBuilder.DropTable(
                name: "CollateralTxOuts");

            migrationBuilder.DropTable(
                name: "FisoBonusDelegations");

            migrationBuilder.DropTable(
                name: "FisoDelegations");

            migrationBuilder.DropTable(
                name: "FisoDelegators");

            migrationBuilder.DropTable(
                name: "FisoEpochRewards");

            migrationBuilder.DropTable(
                name: "FisoPoolActiveStakes");

            migrationBuilder.DropTable(
                name: "TxInputs");

            migrationBuilder.DropTable(
                name: "TxOutputs");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Blocks");
        }
    }
}
