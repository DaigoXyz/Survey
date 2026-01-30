using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Survey.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldOtherAnswer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OtherText",
                table: "DocumentSurveyAnswers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsOther",
                table: "CheckboxOptions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OtherText",
                table: "DocumentSurveyAnswers");

            migrationBuilder.DropColumn(
                name: "IsOther",
                table: "CheckboxOptions");
        }
    }
}
