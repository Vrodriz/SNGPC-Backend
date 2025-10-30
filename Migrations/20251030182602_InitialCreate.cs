using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SNGPC_B.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Farmaceuticos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CRF = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CRFUF = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    CRFDataEmissao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CPF = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    LoginANVISA = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    SenhaANVISA = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Farmaceuticos", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Farmaceuticos");
        }
    }
}
