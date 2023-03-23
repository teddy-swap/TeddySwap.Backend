﻿// <auto-generated />
using System;
using System.Collections.Generic;
using System.Numerics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TeddySwap.Sink.Data;

#nullable disable

namespace TeddySwap.Sink.Data.Migrations.TeddySwapFisoSinkDb
{
    [DbContext(typeof(TeddySwapFisoSinkDbContext))]
    [Migration("20230323100223_FisoAddBalanceByStakeEpoch")]
    partial class FisoAddBalanceByStakeEpoch
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("TeddySwap.Common.Models.Asset", b =>
                {
                    b.Property<string>("PolicyId")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("TxOutputHash")
                        .HasColumnType("text");

                    b.Property<decimal>("TxOutputIndex")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric(20,0)");

                    b.HasKey("PolicyId", "Name", "TxOutputHash", "TxOutputIndex");

                    b.HasIndex("TxOutputHash", "TxOutputIndex");

                    b.ToTable("Assets");
                });

            modelBuilder.Entity("TeddySwap.Common.Models.BalanceByStakeEpoch", b =>
                {
                    b.Property<string>("StakeAddress")
                        .HasColumnType("text");

                    b.Property<decimal>("Epoch")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal>("Balance")
                        .HasColumnType("numeric(20,0)");

                    b.HasKey("StakeAddress", "Epoch");

                    b.ToTable("BalanceByStakeEpoch");
                });

            modelBuilder.Entity("TeddySwap.Common.Models.Block", b =>
                {
                    b.Property<string>("BlockHash")
                        .HasColumnType("text");

                    b.Property<decimal>("BlockNumber")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal>("Epoch")
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("Era")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<IEnumerable<ulong>>("InvalidTransactions")
                        .HasColumnType("jsonb");

                    b.Property<decimal>("Slot")
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("VrfKeyhash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("BlockHash");

                    b.ToTable("Blocks");
                });

            modelBuilder.Entity("TeddySwap.Common.Models.FisoBonusDelegation", b =>
                {
                    b.Property<decimal>("EpochNumber")
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("PoolId")
                        .HasColumnType("text");

                    b.Property<string>("StakeAddress")
                        .HasColumnType("text");

                    b.Property<string>("TxHash")
                        .HasColumnType("text");

                    b.Property<decimal>("Slot")
                        .HasColumnType("numeric(20,0)");

                    b.HasKey("EpochNumber", "PoolId", "StakeAddress", "TxHash");

                    b.ToTable("FisoBonusDelegations");
                });

            modelBuilder.Entity("TeddySwap.Common.Models.FisoEpochReward", b =>
                {
                    b.Property<decimal>("EpochNumber")
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("StakeAddress")
                        .HasColumnType("text");

                    b.Property<bool>("ActiveBonus")
                        .HasColumnType("boolean");

                    b.Property<decimal>("BonusAmount")
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("PoolId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("ShareAmount")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal>("SharePercentage")
                        .HasColumnType("numeric");

                    b.Property<BigInteger>("StakeAmount")
                        .HasColumnType("numeric");

                    b.HasKey("EpochNumber", "StakeAddress");

                    b.ToTable("FisoEpochRewards");
                });

            modelBuilder.Entity("TeddySwap.Common.Models.FisoPoolActiveStake", b =>
                {
                    b.Property<decimal>("EpochNumber")
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("PoolId")
                        .HasColumnType("text");

                    b.Property<decimal>("StakeAmount")
                        .HasColumnType("numeric(20,0)");

                    b.HasKey("EpochNumber", "PoolId");

                    b.ToTable("FisoPoolActiveStakes");
                });

            modelBuilder.Entity("TeddySwap.Common.Models.Transaction", b =>
                {
                    b.Property<string>("Hash")
                        .HasColumnType("text");

                    b.Property<string>("Blockhash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("Fee")
                        .HasColumnType("numeric(20,0)");

                    b.Property<bool>("HasCollateralOutput")
                        .HasColumnType("boolean");

                    b.Property<decimal>("Index")
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("Metadata")
                        .HasColumnType("jsonb");

                    b.HasKey("Hash");

                    b.HasIndex("Blockhash");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("TeddySwap.Common.Models.TxInput", b =>
                {
                    b.Property<string>("TxHash")
                        .HasColumnType("text");

                    b.Property<string>("TxOutputHash")
                        .HasColumnType("text");

                    b.Property<decimal>("TxOutputIndex")
                        .HasColumnType("numeric(20,0)");

                    b.Property<byte?>("InlineDatum")
                        .HasColumnType("smallint");

                    b.HasKey("TxHash", "TxOutputHash", "TxOutputIndex");

                    b.HasIndex("TxOutputHash", "TxOutputIndex");

                    b.ToTable("TxInputs");
                });

            modelBuilder.Entity("TeddySwap.Common.Models.TxOutput", b =>
                {
                    b.Property<string>("TxHash")
                        .HasColumnType("text");

                    b.Property<decimal>("Index")
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("DatumCbor")
                        .HasColumnType("text");

                    b.Property<decimal>("TxIndex")
                        .HasColumnType("numeric(20,0)");

                    b.HasKey("TxHash", "Index");

                    b.ToTable("TxOutputs");
                });

            modelBuilder.Entity("TeddySwap.Common.Models.Asset", b =>
                {
                    b.HasOne("TeddySwap.Common.Models.TxOutput", "TxOutput")
                        .WithMany("Assets")
                        .HasForeignKey("TxOutputHash", "TxOutputIndex")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TxOutput");
                });

            modelBuilder.Entity("TeddySwap.Common.Models.Transaction", b =>
                {
                    b.HasOne("TeddySwap.Common.Models.Block", "Block")
                        .WithMany("Transactions")
                        .HasForeignKey("Blockhash")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Block");
                });

            modelBuilder.Entity("TeddySwap.Common.Models.TxInput", b =>
                {
                    b.HasOne("TeddySwap.Common.Models.Transaction", "Transaction")
                        .WithMany("Inputs")
                        .HasForeignKey("TxHash")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TeddySwap.Common.Models.TxOutput", "TxOutput")
                        .WithMany("Inputs")
                        .HasForeignKey("TxOutputHash", "TxOutputIndex")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Transaction");

                    b.Navigation("TxOutput");
                });

            modelBuilder.Entity("TeddySwap.Common.Models.TxOutput", b =>
                {
                    b.HasOne("TeddySwap.Common.Models.Transaction", "Transaction")
                        .WithMany("Outputs")
                        .HasForeignKey("TxHash")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Transaction");
                });

            modelBuilder.Entity("TeddySwap.Common.Models.Block", b =>
                {
                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("TeddySwap.Common.Models.Transaction", b =>
                {
                    b.Navigation("Inputs");

                    b.Navigation("Outputs");
                });

            modelBuilder.Entity("TeddySwap.Common.Models.TxOutput", b =>
                {
                    b.Navigation("Assets");

                    b.Navigation("Inputs");
                });
#pragma warning restore 612, 618
        }
    }
}
