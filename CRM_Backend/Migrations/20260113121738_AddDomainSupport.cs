using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CRM_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddDomainSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DomainId",
                table: "users",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "domain_id",
                table: "roles",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "domains",
                columns: table => new
                {
                    domain_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    domain_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    domain_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_domains", x => x.domain_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_users_DomainId",
                table: "users",
                column: "DomainId");

            migrationBuilder.CreateIndex(
                name: "IX_roles_domain_id",
                table: "roles",
                column: "domain_id");

            migrationBuilder.AddForeignKey(
                name: "FK_roles_domains_domain_id",
                table: "roles",
                column: "domain_id",
                principalTable: "domains",
                principalColumn: "domain_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_users_domains_DomainId",
                table: "users",
                column: "DomainId",
                principalTable: "domains",
                principalColumn: "domain_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_roles_domains_domain_id",
                table: "roles");

            migrationBuilder.DropForeignKey(
                name: "FK_users_domains_DomainId",
                table: "users");

            migrationBuilder.DropTable(
                name: "domains");

            migrationBuilder.DropIndex(
                name: "IX_users_DomainId",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_roles_domain_id",
                table: "roles");

            migrationBuilder.DropColumn(
                name: "DomainId",
                table: "users");

            migrationBuilder.DropColumn(
                name: "domain_id",
                table: "roles");
        }
    }
}
