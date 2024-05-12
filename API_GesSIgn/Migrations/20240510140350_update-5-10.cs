using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_GesSIgn.Migrations
{
    /// <inheritdoc />
    public partial class update510 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "User_RoleRoles_Id",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "Prescence_Guid",
                table: "Presences",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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

            migrationBuilder.CreateIndex(
                name: "IX_Users_User_RoleRoles_Id",
                table: "Users",
                column: "User_RoleRoles_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_User_RoleRoles_Id",
                table: "Users",
                column: "User_RoleRoles_Id",
                principalTable: "Roles",
                principalColumn: "Roles_Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_User_RoleRoles_Id",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Users_User_RoleRoles_Id",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "User_RoleRoles_Id",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Prescence_Guid",
                table: "Presences");
        }
    }
}
