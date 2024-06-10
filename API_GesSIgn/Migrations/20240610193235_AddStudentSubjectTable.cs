using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_GesSIgn.Migrations
{
    public partial class AddStudentSubjectTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudentSubjects",
                columns: table => new
                {
                    StudentSubject_StudentId = table.Column<int>(type: "int", nullable: false),
                    StudentSubject_SubjectId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentSubjects", x => new { x.StudentSubject_StudentId, x.StudentSubject_SubjectId });
                    table.ForeignKey(
                        name: "FK_StudentSubjects_Students_StudentSubject_StudentId",
                        column: x => x.StudentSubject_StudentId,
                        principalTable: "Students",
                        principalColumn: "Student_Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_StudentSubjects_Subjects_StudentSubject_SubjectId",
                        column: x => x.StudentSubject_SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Subjects_Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentSubjects_StudentSubject_SubjectId",
                table: "StudentSubjects",
                column: "StudentSubject_SubjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentSubjects");
        }
    }
}
