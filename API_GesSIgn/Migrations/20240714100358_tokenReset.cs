﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_GesSIgn.Migrations
{
    /// <inheritdoc />
    public partial class tokenReset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<string>(
                name: "User_tokenReset",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                defaultValue: "");

           
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
          

            migrationBuilder.DropColumn(
                name: "User_tokenReset",
                table: "Users");
         
        }
    }
}
