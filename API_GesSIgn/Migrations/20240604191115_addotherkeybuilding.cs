using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_GesSIgn.Migrations
{
    /// <inheritdoc />
    public partial class addotherkeybuilding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubjectsHour_Bulding_Id",
                table: "SubjectsHour",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubjectsHour_SubjectsHour_Bulding_Id",
                table: "SubjectsHour",
                column: "SubjectsHour_Bulding_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectsHour_Buildings_SubjectsHour_Bulding_Id",
                table: "SubjectsHour",
                column: "SubjectsHour_Bulding_Id",
                principalTable: "Buildings",
                principalColumn: "Bulding_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubjectsHour_Buildings_SubjectsHour_Bulding_Id",
                table: "SubjectsHour");

            migrationBuilder.DropIndex(
                name: "IX_SubjectsHour_SubjectsHour_Bulding_Id",
                table: "SubjectsHour");

            migrationBuilder.DropColumn(
                name: "SubjectsHour_Bulding_Id",
                table: "SubjectsHour");
        }
    }
}
