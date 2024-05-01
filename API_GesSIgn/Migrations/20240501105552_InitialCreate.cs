using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_GesSIgn.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Buildings",
                columns: table => new
                {
                    Bulding_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Bulding_City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bulding_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bulding_Adress = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Buildings", x => x.Bulding_Id);
                });

            migrationBuilder.CreateTable(
                name: "Errors",
                columns: table => new
                {
                    Error_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Error_Funtion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Error_DateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Errors", x => x.Error_id);
                });

            migrationBuilder.CreateTable(
                name: "School",
                columns: table => new
                {
                    School_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    School_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    School_token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    School_allowSite = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_School", x => x.School_Id);
                });

            migrationBuilder.CreateTable(
                name: "Sectors",
                columns: table => new
                {
                    Sectors_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sectors_Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sectors", x => x.Sectors_Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    User_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    User_email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    User_password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.User_Id);
                });

            migrationBuilder.CreateTable(
                name: "SubjectsHour",
                columns: table => new
                {
                    SubjectsHour_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubjectsHour_SectorsSectors_Id = table.Column<int>(type: "int", nullable: false),
                    SubjectsHour_Rooom = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubjectsHour_Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectsHour", x => x.SubjectsHour_Id);
                    table.ForeignKey(
                        name: "FK_SubjectsHour_Sectors_SubjectsHour_SectorsSectors_Id",
                        column: x => x.SubjectsHour_SectorsSectors_Id,
                        principalTable: "Sectors",
                        principalColumn: "Sectors_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Presences",
                columns: table => new
                {
                    Presence_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Presence_UserUser_Id = table.Column<int>(type: "int", nullable: false),
                    Presence_SubjectsHourUser_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Presences", x => x.Presence_Id);
                    table.ForeignKey(
                        name: "FK_Presences_Users_Presence_SubjectsHourUser_Id",
                        column: x => x.Presence_SubjectsHourUser_Id,
                        principalTable: "Users",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Presences_Users_Presence_UserUser_Id",
                        column: x => x.Presence_UserUser_Id,
                        principalTable: "Users",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QCMs",
                columns: table => new
                {
                    QCM_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QCM_TeacherUser_Id = table.Column<int>(type: "int", nullable: false),
                    QCM_Done = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QCMs", x => x.QCM_Id);
                    table.ForeignKey(
                        name: "FK_QCMs_Users_QCM_TeacherUser_Id",
                        column: x => x.QCM_TeacherUser_Id,
                        principalTable: "Users",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Student_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Student_FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Student_LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Student_UserUser_Id = table.Column<int>(type: "int", nullable: false),
                    Student_sectorsSectors_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Student_Id);
                    table.ForeignKey(
                        name: "FK_Students_Sectors_Student_sectorsSectors_Id",
                        column: x => x.Student_sectorsSectors_Id,
                        principalTable: "Sectors",
                        principalColumn: "Sectors_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Students_Users_Student_UserUser_Id",
                        column: x => x.Student_UserUser_Id,
                        principalTable: "Users",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    Subjects_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Subjects_UserUser_Id = table.Column<int>(type: "int", nullable: false),
                    Subjects_SectorsSectors_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Subjects_Id);
                    table.ForeignKey(
                        name: "FK_Subjects_Sectors_Subjects_SectorsSectors_Id",
                        column: x => x.Subjects_SectorsSectors_Id,
                        principalTable: "Sectors",
                        principalColumn: "Sectors_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Subjects_Users_Subjects_UserUser_Id",
                        column: x => x.Subjects_UserUser_Id,
                        principalTable: "Users",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Presences_Presence_SubjectsHourUser_Id",
                table: "Presences",
                column: "Presence_SubjectsHourUser_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Presences_Presence_UserUser_Id",
                table: "Presences",
                column: "Presence_UserUser_Id");

            migrationBuilder.CreateIndex(
                name: "IX_QCMs_QCM_TeacherUser_Id",
                table: "QCMs",
                column: "QCM_TeacherUser_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Students_Student_sectorsSectors_Id",
                table: "Students",
                column: "Student_sectorsSectors_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Students_Student_UserUser_Id",
                table: "Students",
                column: "Student_UserUser_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_Subjects_SectorsSectors_Id",
                table: "Subjects",
                column: "Subjects_SectorsSectors_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_Subjects_UserUser_Id",
                table: "Subjects",
                column: "Subjects_UserUser_Id");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectsHour_SubjectsHour_SectorsSectors_Id",
                table: "SubjectsHour",
                column: "SubjectsHour_SectorsSectors_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Buildings");

            migrationBuilder.DropTable(
                name: "Errors");

            migrationBuilder.DropTable(
                name: "Presences");

            migrationBuilder.DropTable(
                name: "QCMs");

            migrationBuilder.DropTable(
                name: "School");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.DropTable(
                name: "SubjectsHour");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Sectors");
        }
    }
}
