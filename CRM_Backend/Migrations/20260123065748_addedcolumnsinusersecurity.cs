using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM_Backend.Migrations
{
    /// <inheritdoc />
    public partial class addedcolumnsinusersecurity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "password_reset_expires_at",
                table: "user_security",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "password_reset_token_hash",
                table: "user_security",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "password_reset_expires_at",
                table: "user_security");

            migrationBuilder.DropColumn(
                name: "password_reset_token_hash",
                table: "user_security");
        }
    }
}
