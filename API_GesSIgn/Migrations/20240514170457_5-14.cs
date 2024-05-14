using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_GesSIgn.Migrations
{
    /// <inheritdoc />
    public partial class _514 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "School_Date",
                table: "School",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "School_Id",
                table: "Buildings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Buildings_School_Id",
                table: "Buildings",
                column: "School_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Buildings_School_School_Id",
                table: "Buildings",
                column: "School_Id",
                principalTable: "School",
                principalColumn: "School_Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Buildings_School_School_Id",
                table: "Buildings");

            migrationBuilder.DropIndex(
                name: "IX_Buildings_School_Id",
                table: "Buildings");

            migrationBuilder.DropColumn(
                name: "School_Date",
                table: "School");

            migrationBuilder.DropColumn(
                name: "School_Id",
                table: "Buildings");
        }
    }
}
