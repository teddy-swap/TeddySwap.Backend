﻿// <auto-generated />
using System.Collections.Generic;
using Conclave.Sink.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Conclave.Sink.Migrations
{
    [DbContext(typeof(ConclaveSinkDbContext))]
    partial class ConclaveSinkDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Conclave.Sink.Models.AddressByStake", b =>
                {
                    b.Property<string>("StakeAddress")
                        .HasColumnType("text");

                    b.Property<List<string>>("PaymentAddresses")
                        .IsRequired()
                        .HasColumnType("text[]");

                    b.HasKey("StakeAddress");

                    b.ToTable("AddressByStake");
                });

            modelBuilder.Entity("Conclave.Sink.Models.BalanceByAddress", b =>
                {
                    b.Property<string>("Address")
                        .HasColumnType("text");

                    b.Property<decimal>("Balance")
                        .HasColumnType("numeric(20,0)");

                    b.HasKey("Address");

                    b.ToTable("BalanceByAddress");
                });

            modelBuilder.Entity("Conclave.Sink.Models.Block", b =>
                {
                    b.Property<string>("BlockHash")
                        .HasColumnType("text");

                    b.Property<decimal>("BlockNumber")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal>("Epoch")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal>("Slot")
                        .HasColumnType("numeric(20,0)");

                    b.HasKey("BlockHash");

                    b.ToTable("Block");
                });

            modelBuilder.Entity("Conclave.Sink.Models.DelegatorByEpoch", b =>
                {
                    b.Property<string>("StakeAddress")
                        .HasColumnType("text");

                    b.Property<string>("PoolHash")
                        .HasColumnType("text");

                    b.Property<decimal>("Slot")
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("BlockHash")
                        .HasColumnType("text");

                    b.HasKey("StakeAddress", "PoolHash", "Slot");

                    b.HasIndex("BlockHash");

                    b.ToTable("DelegatorByEpoch");
                });

            modelBuilder.Entity("Conclave.Sink.Models.RegistrationByStake", b =>
                {
                    b.Property<string>("StakeHash")
                        .HasColumnType("text");

                    b.Property<string>("TxHash")
                        .HasColumnType("text");

                    b.Property<decimal>("TxIndex")
                        .HasColumnType("numeric(20,0)");

                    b.HasKey("StakeHash", "TxHash", "TxIndex");

                    b.ToTable("RegistrationByStake");
                });

            modelBuilder.Entity("Conclave.Sink.Models.RewardAddressByPoolPerEpoch", b =>
                {
                    b.Property<string>("PoolId")
                        .HasColumnType("text");

                    b.Property<decimal>("Slot")
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("RewardAddress")
                        .HasColumnType("text");

                    b.Property<string>("BlockHash")
                        .HasColumnType("text");

                    b.HasKey("PoolId", "Slot", "RewardAddress");

                    b.HasIndex("BlockHash");

                    b.ToTable("RewardAddressByPoolPerEpoch");
                });

            modelBuilder.Entity("Conclave.Sink.Models.TxInput", b =>
                {
                    b.Property<string>("TxHash")
                        .HasColumnType("text");

                    b.Property<string>("TxInputOutputHash")
                        .HasColumnType("text");

                    b.Property<decimal>("TxInputOutputIndex")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal>("Slot")
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("BlockHash")
                        .HasColumnType("text");

                    b.HasKey("TxHash", "TxInputOutputHash", "TxInputOutputIndex", "Slot");

                    b.HasIndex("BlockHash");

                    b.ToTable("TxInput");
                });

            modelBuilder.Entity("Conclave.Sink.Models.TxOutput", b =>
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
                        .HasColumnType("text");

                    b.HasKey("TxHash", "Index");

                    b.HasIndex("BlockHash");

                    b.ToTable("TxOutput");
                });

            modelBuilder.Entity("Conclave.Sink.Models.DelegatorByEpoch", b =>
                {
                    b.HasOne("Conclave.Sink.Models.Block", "Block")
                        .WithMany()
                        .HasForeignKey("BlockHash");

                    b.Navigation("Block");
                });

            modelBuilder.Entity("Conclave.Sink.Models.RewardAddressByPoolPerEpoch", b =>
                {
                    b.HasOne("Conclave.Sink.Models.Block", "Block")
                        .WithMany()
                        .HasForeignKey("BlockHash");

                    b.Navigation("Block");
                });

            modelBuilder.Entity("Conclave.Sink.Models.TxInput", b =>
                {
                    b.HasOne("Conclave.Sink.Models.Block", "Block")
                        .WithMany()
                        .HasForeignKey("BlockHash");

                    b.Navigation("Block");
                });

            modelBuilder.Entity("Conclave.Sink.Models.TxOutput", b =>
                {
                    b.HasOne("Conclave.Sink.Models.Block", "Block")
                        .WithMany()
                        .HasForeignKey("BlockHash");

                    b.Navigation("Block");
                });
#pragma warning restore 612, 618
        }
    }
}
