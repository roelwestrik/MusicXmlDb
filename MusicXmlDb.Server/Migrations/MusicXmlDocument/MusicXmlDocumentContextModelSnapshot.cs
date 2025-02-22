﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MusicXmlDb.Server.MusicXmlDocuments;

#nullable disable

namespace MusicXmlDb.Server.Migrations
{
    [DbContext(typeof(MusicXmlDocumentContext))]
    partial class MusicXmlDocumentContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("MusicXml")
                .HasAnnotation("ProductVersion", "8.0.13");

            modelBuilder.Entity("MusicXmlDb.Server.MusicXml.MusicXmlDocument", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("MusicXmlDocuments", "MusicXml");
                });
#pragma warning restore 612, 618
        }
    }
}
