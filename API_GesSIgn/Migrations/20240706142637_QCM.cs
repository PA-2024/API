using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_GesSIgn.Migrations
{
    /// <inheritdoc />
    public partial class QCM : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "QCMs",
                columns: table => new
                {
                    QCM_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QCM_Done = table.Column<bool>(type: "bit", nullable: false),
                    QCM_SubjectHour_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QCMs", x => x.QCM_Id);
                    table.ForeignKey(
                        name: "FK_QCMs_SubjectsHour_QCM_SubjectHour_id",
                        column: x => x.QCM_SubjectHour_id,
                        principalTable: "SubjectsHour",
                        principalColumn: "SubjectsHour_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QcmResult",
                columns: table => new
                {
                    QcmResult_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QcmResult_Student_Id = table.Column<int>(type: "int", nullable: false),
                    QcmResult_QCM_Id = table.Column<int>(type: "int", nullable: false),
                    QcmResult_Score = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QcmResult", x => x.QcmResult_Id);
                    table.ForeignKey(
                        name: "FK_QcmResult_QCMs_QcmResult_QCM_Id",
                        column: x => x.QcmResult_QCM_Id,
                        principalTable: "QCMs",
                        principalColumn: "QCM_Id");
                    table.ForeignKey(
                        name: "FK_QcmResult_Students_QcmResult_Student_Id",
                        column: x => x.QcmResult_Student_Id,
                        principalTable: "Students",
                        principalColumn: "Student_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Question_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Question_Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Question_QCM_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Question_Id);
                    table.ForeignKey(
                        name: "FK_Questions_QCMs_Question_QCM_Id",
                        column: x => x.Question_QCM_Id,
                        principalTable: "QCMs",
                        principalColumn: "QCM_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OptionQcm",
                columns: table => new
                {
                    OptionQcm_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OptionQcm_Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OptionQcm_IsCorrect = table.Column<bool>(type: "bit", nullable: false),
                    OptionQcm_Question_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OptionQcm", x => x.OptionQcm_id);
                    table.ForeignKey(
                        name: "FK_OptionQcm_Questions_OptionQcm_Question_Id",
                        column: x => x.OptionQcm_Question_Id,
                        principalTable: "Questions",
                        principalColumn: "Question_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OptionQcm_OptionQcm_Question_Id",
                table: "OptionQcm",
                column: "OptionQcm_Question_Id");

            migrationBuilder.CreateIndex(
                name: "IX_QcmResult_QcmResult_QCM_Id",
                table: "QcmResult",
                column: "QcmResult_QCM_Id");

            migrationBuilder.CreateIndex(
                name: "IX_QcmResult_QcmResult_Student_Id",
                table: "QcmResult",
                column: "QcmResult_Student_Id");

            migrationBuilder.CreateIndex(
                name: "IX_QCMs_QCM_SubjectHour_id",
                table: "QCMs",
                column: "QCM_SubjectHour_id");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_Question_QCM_Id",
                table: "Questions",
                column: "Question_QCM_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OptionQcm");

            migrationBuilder.DropTable(
                name: "QcmResult");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "QCMs");

            migrationBuilder.DropColumn(
                name: "SubjectsHour_TeacherComment",
                table: "SubjectsHour");

            migrationBuilder.DropColumn(
                name: "Presence_ScanDate",
                table: "Presences");

            migrationBuilder.DropColumn(
                name: "Presence_ScanInfo",
                table: "Presences");
        }
    }
}
