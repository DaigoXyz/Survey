using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Survey.Migrations
{
    /// <inheritdoc />
    public partial class FixForeignKeyRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentSurveyAnswerOptions_CheckboxOptions_CheckboxOptionId",
                table: "DocumentSurveyAnswerOptions");

            migrationBuilder.DropForeignKey(
                name: "FK_DocumentSurveyAnswers_SurveyItems_ItemId",
                table: "DocumentSurveyAnswers");

            migrationBuilder.DropIndex(
                name: "IX_DocumentSurveyAnswers_DocumentId_ItemId",
                table: "DocumentSurveyAnswers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DocumentSurveyAnswerOptions",
                table: "DocumentSurveyAnswerOptions");

            migrationBuilder.AlterColumn<int>(
                name: "CheckboxOptionId",
                table: "DocumentSurveyAnswerOptions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "DocumentSurveyAnswerOptions",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DocumentSurveyAnswerOptions",
                table: "DocumentSurveyAnswerOptions",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentSurveyAnswers_DocumentId",
                table: "DocumentSurveyAnswers",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentSurveyAnswerOptions_AnswerId_CheckboxOptionId",
                table: "DocumentSurveyAnswerOptions",
                columns: new[] { "AnswerId", "CheckboxOptionId" },
                unique: true,
                filter: "[CheckboxOptionId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentSurveyAnswerOptions_CheckboxOptions_CheckboxOptionId",
                table: "DocumentSurveyAnswerOptions",
                column: "CheckboxOptionId",
                principalTable: "CheckboxOptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentSurveyAnswers_SurveyItems_ItemId",
                table: "DocumentSurveyAnswers",
                column: "ItemId",
                principalTable: "SurveyItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentSurveyAnswerOptions_CheckboxOptions_CheckboxOptionId",
                table: "DocumentSurveyAnswerOptions");

            migrationBuilder.DropForeignKey(
                name: "FK_DocumentSurveyAnswers_SurveyItems_ItemId",
                table: "DocumentSurveyAnswers");

            migrationBuilder.DropIndex(
                name: "IX_DocumentSurveyAnswers_DocumentId",
                table: "DocumentSurveyAnswers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DocumentSurveyAnswerOptions",
                table: "DocumentSurveyAnswerOptions");

            migrationBuilder.DropIndex(
                name: "IX_DocumentSurveyAnswerOptions_AnswerId_CheckboxOptionId",
                table: "DocumentSurveyAnswerOptions");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "DocumentSurveyAnswerOptions");

            migrationBuilder.AlterColumn<int>(
                name: "CheckboxOptionId",
                table: "DocumentSurveyAnswerOptions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DocumentSurveyAnswerOptions",
                table: "DocumentSurveyAnswerOptions",
                columns: new[] { "AnswerId", "CheckboxOptionId" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentSurveyAnswers_DocumentId_ItemId",
                table: "DocumentSurveyAnswers",
                columns: new[] { "DocumentId", "ItemId" },
                unique: true,
                filter: "[ItemId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentSurveyAnswerOptions_CheckboxOptions_CheckboxOptionId",
                table: "DocumentSurveyAnswerOptions",
                column: "CheckboxOptionId",
                principalTable: "CheckboxOptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentSurveyAnswers_SurveyItems_ItemId",
                table: "DocumentSurveyAnswers",
                column: "ItemId",
                principalTable: "SurveyItems",
                principalColumn: "Id");
        }
    }
}
