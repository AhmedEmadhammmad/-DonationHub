using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DonationHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCategorySoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Categories_NameAr",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_NameEn",
                table: "Categories");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                table: "Categories",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Categories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_NameAr",
                table: "Categories",
                column: "NameAr",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_NameEn",
                table: "Categories",
                column: "NameEn",
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Categories_NameAr",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_NameEn",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Categories");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_NameAr",
                table: "Categories",
                column: "NameAr",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_NameEn",
                table: "Categories",
                column: "NameEn",
                unique: true);
        }
    }
}
