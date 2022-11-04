using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationCore.Infrastructure.Persistence.Migrations
{
    public partial class AddBaseEntidad : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreadoEn",
                table: "Products",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreadoPor",
                table: "Products",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificadoEn",
                table: "Products",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModificadoPor",
                table: "Products",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreadoEn",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CreadoPor",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ModificadoEn",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ModificadoPor",
                table: "Products");
        }
    }
}
