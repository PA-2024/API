using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_GesSIgn.Migrations
{
    /// <inheritdoc />
    public partial class FinalChangeDataBase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Presence_ProofAbsence_Id",
                table: "Presences",
                type: "int",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AnswerQCM",
                columns: table => new
                {
                    AnswerQCM_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnswerQCM_QCM_Id = table.Column<int>(type: "int", nullable: false),
                    AnswerQCM_Question_Id = table.Column<int>(type: "int", nullable: false),
                    AnswerQCM_Answer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnswerQCM_Student_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerQCM", x => x.AnswerQCM_Id);
                    table.ForeignKey(
                        name: "FK_AnswerQCM_QCMs_AnswerQCM_QCM_Id",
                        column: x => x.AnswerQCM_QCM_Id,
                        principalTable: "QCMs",
                        principalColumn: "QCM_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnswerQCM_Questions_AnswerQCM_Question_Id",
                        column: x => x.AnswerQCM_Question_Id,
                        principalTable: "Questions",
                        principalColumn: "Question_Id"
                        );
                    table.ForeignKey(
                        name: "FK_AnswerQCM_Students_AnswerQCM_Student_id",
                        column: x => x.AnswerQCM_Student_id,
                        principalTable: "Students",
                        principalColumn: "Student_Id");
                });

            migrationBuilder.CreateTable(
                name: "ProofAbsences",
                columns: table => new
                {
                    ProofAbsence_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProofAbsence_UrlFile = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProofAbsence_Status = table.Column<int>(type: "int", nullable: false),
                    ProofAbsence_SchoolCommentaire = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProofAbsence_ReasonAbscence = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProofAbsences", x => x.ProofAbsence_Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Presences_Presence_ProofAbsence_Id",
                table: "Presences",
                column: "Presence_ProofAbsence_Id");

            migrationBuilder.CreateIndex(
                name: "IX_AnswerQCM_AnswerQCM_QCM_Id",
                table: "AnswerQCM",
                column: "AnswerQCM_QCM_Id");

            migrationBuilder.CreateIndex(
                name: "IX_AnswerQCM_AnswerQCM_Question_Id",
                table: "AnswerQCM",
                column: "AnswerQCM_Question_Id");

            migrationBuilder.CreateIndex(
                name: "IX_AnswerQCM_AnswerQCM_Student_id",
                table: "AnswerQCM",
                column: "AnswerQCM_Student_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Presences_ProofAbsences_Presence_ProofAbsence_Id",
                table: "Presences",
                column: "Presence_ProofAbsence_Id",
                principalTable: "ProofAbsences",
                principalColumn: "ProofAbsence_Id"
                );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Presences_ProofAbsences_Presence_ProofAbsence_Id",
                table: "Presences");

            migrationBuilder.DropTable(
                name: "AnswerQCM");

            migrationBuilder.DropTable(
                name: "ProofAbsences");

            migrationBuilder.DropIndex(
                name: "IX_Presences_Presence_ProofAbsence_Id",
                table: "Presences");

            migrationBuilder.DropColumn(
                name: "Presence_ProofAbsence_Id",
                table: "Presences");
        }
    }
}
