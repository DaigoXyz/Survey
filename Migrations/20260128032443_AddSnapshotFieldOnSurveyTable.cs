using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Survey.Migrations
{
    /// <inheritdoc />
    public partial class AddSnapshotFieldOnSurveyTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentSurveyAnswers_SurveyItems_ItemId",
                table: "DocumentSurveyAnswers");

            migrationBuilder.DropIndex(
                name: "IX_DocumentSurveyAnswers_DocumentId_ItemId",
                table: "DocumentSurveyAnswers");

            migrationBuilder.AlterColumn<int>(
                name: "ItemId",
                table: "DocumentSurveyAnswers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "QuestionSnapshot",
                table: "DocumentSurveyAnswers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SortOrderSnapshot",
                table: "DocumentSurveyAnswers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TypeSnapshot",
                table: "DocumentSurveyAnswers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "OptionTextSnapshot",
                table: "DocumentSurveyAnswerOptions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentSurveyAnswers_DocumentId_ItemId",
                table: "DocumentSurveyAnswers",
                columns: new[] { "DocumentId", "ItemId" },
                unique: true,
                filter: "[ItemId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentSurveyAnswers_SurveyItems_ItemId",
                table: "DocumentSurveyAnswers",
                column: "ItemId",
                principalTable: "SurveyItems",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentSurveyAnswers_SurveyItems_ItemId",
                table: "DocumentSurveyAnswers");

            migrationBuilder.DropIndex(
                name: "IX_DocumentSurveyAnswers_DocumentId_ItemId",
                table: "DocumentSurveyAnswers");

            migrationBuilder.DropColumn(
                name: "QuestionSnapshot",
                table: "DocumentSurveyAnswers");

            migrationBuilder.DropColumn(
                name: "SortOrderSnapshot",
                table: "DocumentSurveyAnswers");

            migrationBuilder.DropColumn(
                name: "TypeSnapshot",
                table: "DocumentSurveyAnswers");

            migrationBuilder.DropColumn(
                name: "OptionTextSnapshot",
                table: "DocumentSurveyAnswerOptions");

            migrationBuilder.AlterColumn<int>(
                name: "ItemId",
                table: "DocumentSurveyAnswers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentSurveyAnswers_DocumentId_ItemId",
                table: "DocumentSurveyAnswers",
                columns: new[] { "DocumentId", "ItemId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentSurveyAnswers_SurveyItems_ItemId",
                table: "DocumentSurveyAnswers",
                column: "ItemId",
                principalTable: "SurveyItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
