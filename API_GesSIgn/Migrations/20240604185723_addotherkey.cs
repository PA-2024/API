using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_GesSIgn.Migrations
{
    /// <inheritdoc />
    public partial class addotherkey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubjectsHour_Sectors_SubjectsHour_Sector_Id",
                table: "SubjectsHour");

            migrationBuilder.DropIndex(
                name: "IX_SubjectsHour_SubjectsHour_Sector_Id",
                table: "SubjectsHour");

            migrationBuilder.AlterColumn<int>(
                name: "SubjectsHour_Sector_Id",
                table: "SubjectsHour",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "SubjectsHour_Subjects_Id",
                table: "SubjectsHour",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SubjectsHour_SubjectsHour_Subjects_Id",
                table: "SubjectsHour",
                column: "SubjectsHour_Subjects_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectsHour_Subjects_SubjectsHour_Subjects_Id",
                table: "SubjectsHour",
                column: "SubjectsHour_Subjects_Id",
                principalTable: "Subjects",
                principalColumn: "Subjects_Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubjectsHour_Subjects_SubjectsHour_Subjects_Id",
                table: "SubjectsHour");

            migrationBuilder.DropIndex(
                name: "IX_SubjectsHour_SubjectsHour_Subjects_Id",
                table: "SubjectsHour");

            migrationBuilder.DropColumn(
                name: "SubjectsHour_Subjects_Id",
                table: "SubjectsHour");

            migrationBuilder.AlterColumn<int>(
                name: "SubjectsHour_Sector_Id",
                table: "SubjectsHour",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubjectsHour_SubjectsHour_Sector_Id",
                table: "SubjectsHour",
                column: "SubjectsHour_Sector_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectsHour_Sectors_SubjectsHour_Sector_Id",
                table: "SubjectsHour",
                column: "SubjectsHour_Sector_Id",
                principalTable: "Sectors",
                principalColumn: "Sectors_Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
