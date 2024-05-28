using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_GesSIgn.Migrations
{
    /// <inheritdoc />
    public partial class _282 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SubjectsHour_Date",
                table: "SubjectsHour",
                newName: "SubjectsHour_DateStart");

            migrationBuilder.AddColumn<DateTime>(
                name: "SubjectsHour_DateEnd",
                table: "SubjectsHour",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubjectsHour_DateEnd",
                table: "SubjectsHour");

            migrationBuilder.RenameColumn(
                name: "SubjectsHour_DateStart",
                table: "SubjectsHour",
                newName: "SubjectsHour_Date");
        }
    }
}
