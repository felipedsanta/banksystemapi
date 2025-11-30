using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankSystem.Api.Migrations
{
    /// <inheritdoc />
    public partial class mudancaNomeDeColunaDaTabelaConta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Ativa",
                table: "Contas",
                newName: "Ativo");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataAtualizacao",
                table: "Contas",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataAtualizacao",
                table: "Contas");

            migrationBuilder.RenameColumn(
                name: "Ativo",
                table: "Contas",
                newName: "Ativa");
        }
    }
}
