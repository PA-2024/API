using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_GesSIgn.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Errors",
                columns: table => new
                {
                    Error_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Error_Funtion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Error_DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Error_Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Error_Solved = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Errors", x => x.Error_id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Roles_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Role_Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Roles_Id);
                });

            migrationBuilder.CreateTable(
                name: "Schools",
                columns: table => new
                {
                    School_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    School_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    School_token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    School_allowSite = table.Column<bool>(type: "bit", nullable: false),
                    School_Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schools", x => x.School_Id);
                });

            migrationBuilder.CreateTable(
                name: "Buildings",
                columns: table => new
                {
                    Bulding_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Bulding_City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bulding_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bulding_Adress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    School_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Buildings", x => x.Bulding_Id);
                    table.ForeignKey(
                        name: "FK_Buildings_Schools_School_Id",
                        column: x => x.School_Id,
                        principalTable: "Schools",
                        principalColumn: "School_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sectors",
                columns: table => new
                {
                    Sectors_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sectors_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sectors_School_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sectors", x => x.Sectors_Id);
                    table.ForeignKey(
                        name: "FK_Sectors_Schools_Sectors_School_Id",
                        column: x => x.Sectors_School_Id,
                        principalTable: "Schools",
                        principalColumn: "School_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    User_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    User_email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    User_password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    User_RoleRoles_Id = table.Column<int>(type: "int", nullable: false),
                    User_School_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.User_Id);
                    table.ForeignKey(
                        name: "FK_Users_Roles_User_RoleRoles_Id",
                        column: x => x.User_RoleRoles_Id,
                        principalTable: "Roles",
                        principalColumn: "Roles_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_Schools_User_School_Id",
                        column: x => x.User_School_Id,
                        principalTable: "Schools",
                        principalColumn: "School_Id");
                });

            migrationBuilder.CreateTable(
                name: "SubjectsHour",
                columns: table => new
                {
                    SubjectsHour_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubjectsHour_Sector_Id = table.Column<int>(type: "int", nullable: false),
                    SubjectsHour_Room = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubjectsHour_Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectsHour", x => x.SubjectsHour_Id);
                    table.ForeignKey(
                        name: "FK_SubjectsHour_Sectors_SubjectsHour_Sector_Id",
                        column: x => x.SubjectsHour_Sector_Id,
                        principalTable: "Sectors",
                        principalColumn: "Sectors_Id",
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
                    Student_User_Id = table.Column<int>(type: "int", nullable: false),
                    Student_Sector_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Student_Id);
                    table.ForeignKey(
                        name: "FK_Students_Sectors_Student_Sector_Id",
                        column: x => x.Student_Sector_Id,
                        principalTable: "Sectors",
                        principalColumn: "Sectors_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Students_Users_Student_User_Id",
                        column: x => x.Student_User_Id,
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
                    Subjects_User_Id = table.Column<int>(type: "int", nullable: false),
                    Subjects_Sector_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Subjects_Id);
                    table.ForeignKey(
                        name: "FK_Subjects_Sectors_Subjects_Sector_Id",
                        column: x => x.Subjects_Sector_Id,
                        principalTable: "Sectors",
                        principalColumn: "Sectors_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Subjects_Users_Subjects_User_Id",
                        column: x => x.Subjects_User_Id,
                        principalTable: "Users",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Presences",
                columns: table => new
                {
                    Presence_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Presence_User_Id = table.Column<int>(type: "int", nullable: false),
                    Presence_SubjectsHour_Id = table.Column<int>(type: "int", nullable: false),
                    Presence_Is = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Presences", x => x.Presence_Id);
                    table.ForeignKey(
                        name: "FK_Presences_SubjectsHour_Presence_SubjectsHour_Id",
                        column: x => x.Presence_SubjectsHour_Id,
                        principalTable: "SubjectsHour",
                        principalColumn: "SubjectsHour_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Presences_Users_Presence_User_Id",
                        column: x => x.Presence_User_Id,
                        principalTable: "Users",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Buildings_School_Id",
                table: "Buildings",
                column: "School_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Presences_Presence_SubjectsHour_Id",
                table: "Presences",
                column: "Presence_SubjectsHour_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Presences_Presence_User_Id",
                table: "Presences",
                column: "Presence_User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_QCMs_QCM_TeacherUser_Id",
                table: "QCMs",
                column: "QCM_TeacherUser_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Sectors_Sectors_School_Id",
                table: "Sectors",
                column: "Sectors_School_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Students_Student_Sector_Id",
                table: "Students",
                column: "Student_Sector_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Students_Student_User_Id",
                table: "Students",
                column: "Student_User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_Subjects_Sector_Id",
                table: "Subjects",
                column: "Subjects_Sector_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_Subjects_User_Id",
                table: "Subjects",
                column: "Subjects_User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectsHour_SubjectsHour_Sector_Id",
                table: "SubjectsHour",
                column: "SubjectsHour_Sector_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Users_User_RoleRoles_Id",
                table: "Users",
                column: "User_RoleRoles_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Users_User_School_Id",
                table: "Users",
                column: "User_School_Id");
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
                name: "Students");

            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.DropTable(
                name: "SubjectsHour");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Sectors");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Schools");
        }
    }
}
