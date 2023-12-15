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
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GameAlias")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int?>("GenreId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("PlatformsId")
                        .HasColumnType("int");

                    b.Property<int?>("PublishersId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("GameAlias")
                        .IsUnique();

                    b.HasIndex("GenreId");

                    b.HasIndex("PlatformsId");

                    b.HasIndex("PublishersId");

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
                        .HasColumnType("nvarchar(450)");

                    b.Property<int?>("ParentId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("ParentId");

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
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("Type")
                        .IsUnique();

                    b.ToTable("Platforms");
                });

            modelBuilder.Entity("Gamestore.Database.Entities.Publisher", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CompanyName")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HomePage")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CompanyName")
                        .IsUnique();

                    b.ToTable("Publishers");
                });

            modelBuilder.Entity("Gamestore.Database.Entities.Game", b =>
                {
                    b.HasOne("Gamestore.Database.Entities.Genre", "Genre")
                        .WithMany("Games")
                        .HasForeignKey("GenreId");

                    b.HasOne("Gamestore.Database.Entities.Platform", "Platforms")
                        .WithMany()
                        .HasForeignKey("PlatformsId");

                    b.HasOne("Gamestore.Database.Entities.Publisher", "Publishers")
                        .WithMany()
                        .HasForeignKey("PublishersId");

                    b.Navigation("Genre");

                    b.Navigation("Platforms");

                    b.Navigation("Publishers");
                });

            modelBuilder.Entity("Gamestore.Database.Entities.Genre", b =>
                {
                    b.HasOne("Gamestore.Database.Entities.Genre", "ParentGenre")
                        .WithMany()
                        .HasForeignKey("ParentId");

                    b.Navigation("ParentGenre");
                });

            modelBuilder.Entity("Gamestore.Database.Entities.Genre", b =>
                {
                    b.Navigation("Games");
                });
#pragma warning restore 612, 618
        }
    }
}
