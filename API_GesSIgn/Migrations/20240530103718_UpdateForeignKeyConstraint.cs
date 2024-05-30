using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_GesSIgn.Migrations
{
    /// <inheritdoc />
    public partial class UpdateForeignKeyConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Presences_Students_Presence_Student_Id",
                table: "Presences");

            migrationBuilder.AddForeignKey(
                name: "FK_Presences_Students_Presence_Student_Id",
                table: "Presences",
                column: "Presence_Student_Id",
                principalTable: "Students",
                principalColumn: "Student_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Presences_Students_Presence_Student_Id",
                table: "Presences");

            migrationBuilder.AddForeignKey(
                name: "FK_Presences_Students_Presence_Student_Id",
                table: "Presences",
                column: "Presence_Student_Id",
                principalTable: "Students",
                principalColumn: "Student_Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
