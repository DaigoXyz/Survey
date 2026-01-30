using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Survey.Migrations
{
    /// <inheritdoc />
    public partial class Versioning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StructuralVersion",
                table: "SurveyHeaders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StructuralVersionSnapshot",
                table: "DocumentSurveys",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TemplateHeaderId",
                table: "DocumentSurveys",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StructuralVersion",
                table: "SurveyHeaders");

            migrationBuilder.DropColumn(
                name: "StructuralVersionSnapshot",
                table: "DocumentSurveys");

            migrationBuilder.DropColumn(
                name: "TemplateHeaderId",
                table: "DocumentSurveys");
        }
    }
}
