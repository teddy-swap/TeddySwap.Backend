﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TeddySwap.Sink.Data;

#nullable disable

namespace TeddySwap.Sink.Data.Migrations.TeddySwapBadgerAddressSinkDb
{
    [DbContext(typeof(TeddySwapBadgerAddressSinkDbContext))]
    partial class TeddySwapBadgerAddressSinkDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
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

                    b.Property<string>("BlockHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("PolicyId", "Name", "TxOutputHash", "TxOutputIndex");

                    b.ToTable("Assets");
                });

            modelBuilder.Entity("TeddySwap.Common.Models.BadgerAddressVerification", b =>
                {
                    b.Property<string>("Address")
                        .HasColumnType("text");

                    b.Property<string>("TxHash")
                        .HasColumnType("text");

                    b.Property<string>("BlockHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LinkAddress")
                        .HasColumnType("text");

                    b.Property<decimal>("Slot")
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("StakeAddress")
                        .HasColumnType("text");

                    b.HasKey("Address", "TxHash");

                    b.ToTable("BadgerAddressVerifications");
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

            modelBuilder.Entity("TeddySwap.Common.Models.CollateralTxIn", b =>
                {
                    b.Property<string>("TxHash")
                        .HasColumnType("text");

                    b.Property<string>("TxOutputHash")
                        .HasColumnType("text");

                    b.Property<decimal>("TxOutputIndex")
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("BlockHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("TxHash", "TxOutputHash", "TxOutputIndex");

                    b.ToTable("CollateralTxIns");
                });

            modelBuilder.Entity("TeddySwap.Common.Models.CollateralTxOut", b =>
                {
                    b.Property<string>("Address")
                        .HasColumnType("text");

                    b.Property<string>("TxHash")
                        .HasColumnType("text");

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("BlockHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Address", "TxHash");

                    b.ToTable("CollateralTxOuts");
                });

            modelBuilder.Entity("TeddySwap.Common.Models.Transaction", b =>
                {
                    b.Property<string>("Hash")
                        .HasColumnType("text");

                    b.Property<string>("BlockHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("Fee")
                        .HasColumnType("numeric(20,0)");

                    b.Property<bool>("HasCollateralOutput")
                        .HasColumnType("boolean");

                    b.Property<decimal>("Index")
                        .HasColumnType("numeric(20,0)");

                    b.Property<bool>("IsValid")
                        .HasColumnType("boolean");

                    b.Property<string>("Metadata")
                        .HasColumnType("text");

                    b.HasKey("Hash");

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

                    b.Property<string>("BlockHash")
                        .HasColumnType("text");

                    b.Property<byte?>("InlineDatum")
                        .HasColumnType("smallint");

                    b.HasKey("TxHash", "TxOutputHash", "TxOutputIndex", "BlockHash");

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

                    b.Property<string>("BlockHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("DatumCbor")
                        .HasColumnType("text");

                    b.Property<decimal>("TxIndex")
                        .HasColumnType("numeric(20,0)");

                    b.HasKey("TxHash", "Index");

                    b.ToTable("TxOutputs");
                });
#pragma warning restore 612, 618
        }
    }
}
