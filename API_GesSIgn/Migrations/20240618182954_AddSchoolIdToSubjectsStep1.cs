using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_GesSIgn.Migrations
{
    public partial class AddSchoolIdToSubjectsStep1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Subjects_School_Id",
                table: "Subjects",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_Subjects_School_Id",
                table: "Subjects",
                column: "Subjects_School_Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Subjects_Subjects_School_Id",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "Subjects_School_Id",
                table: "Subjects");
        }
    }
}
