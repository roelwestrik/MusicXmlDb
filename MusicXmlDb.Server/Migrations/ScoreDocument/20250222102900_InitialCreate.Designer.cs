﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MusicXmlDb.Server.ScoreDocuments;

#nullable disable

namespace MusicXmlDb.Server.Migrations.ScoreDocument
{
    [DbContext(typeof(ScoreDocumentContext))]
    [Migration("20250222102900_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("ScoreDocuments")
                .HasAnnotation("ProductVersion", "8.0.13");

            modelBuilder.Entity("MusicXmlDb.Server.ScoreDocuments.ScoreDocument", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Created")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsPublic")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Modified")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Views")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("ScoreDocuments", "ScoreDocuments");
                });

            modelBuilder.Entity("MusicXmlDb.Server.ScoreDocuments.ScoreDocumentHistory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Created")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("MusicXmlId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ScoreDocumentId")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Version")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ScoreDocumentId");

                    b.ToTable("ScoreDocumentHistories", "ScoreDocuments");
                });

            modelBuilder.Entity("MusicXmlDb.Server.ScoreDocuments.ScoreDocumentHistory", b =>
                {
                    b.HasOne("MusicXmlDb.Server.ScoreDocuments.ScoreDocument", "ScoreDocument")
                        .WithMany("Versions")
                        .HasForeignKey("ScoreDocumentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ScoreDocument");
                });

            modelBuilder.Entity("MusicXmlDb.Server.ScoreDocuments.ScoreDocument", b =>
                {
                    b.Navigation("Versions");
                });
#pragma warning restore 612, 618
        }
    }
}
