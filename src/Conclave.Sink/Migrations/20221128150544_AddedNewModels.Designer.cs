﻿// <auto-generated />
using System.Collections.Generic;
using Conclave.Sink.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Conclave.Sink.Migrations
{
    [DbContext(typeof(ConclaveSinkDbContext))]
    [Migration("20221128150544_AddedNewModels")]
    partial class AddedNewModels
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
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

                    b.HasKey("TxHash", "Index");

                    b.ToTable("TxOutput");
                });
#pragma warning restore 612, 618
        }
    }
}
