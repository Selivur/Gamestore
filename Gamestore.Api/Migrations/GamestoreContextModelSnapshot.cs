﻿// <auto-generated />
using System;
using Gamestore.Database.Dbcontext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Gamestore.Api.Migrations
{
    [DbContext(typeof(GamestoreContext))]
    partial class GamestoreContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Gamestore.Database.Entities.Game", b =>
                {
                    b.Property<string>("GameAlias")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("GenreId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("PlatformsId")
                        .HasColumnType("int");

                    b.HasKey("GameAlias");

                    b.HasIndex("GenreId");

                    b.HasIndex("PlatformsId");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("Gamestore.Database.Entities.Genre", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Genres");
                });

            modelBuilder.Entity("Gamestore.Database.Entities.Platform", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Platforms");
                });

            modelBuilder.Entity("Gamestore.Database.Entities.Game", b =>
                {
                    b.HasOne("Gamestore.Database.Entities.Genre", "Genre")
                        .WithMany()
                        .HasForeignKey("GenreId");

                    b.HasOne("Gamestore.Database.Entities.Platform", "Platforms")
                        .WithMany()
                        .HasForeignKey("PlatformsId");

                    b.Navigation("Genre");

                    b.Navigation("Platforms");
                });
#pragma warning restore 612, 618
        }
    }
}
