using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Survey.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldForRequesterAndCreatedBy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DocumentNo",
                table: "DocumentSurveys",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "DocumentSurveys",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "DocumentSurveys",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "DocumentSurveys",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RequesterEmployeeId",
                table: "DocumentSurveys",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RequesterEmployeeName",
                table: "DocumentSurveys",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RequesterPositionLevel",
                table: "DocumentSurveys",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RequesterPositionName",
                table: "DocumentSurveys",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SupervisorId",
                table: "DocumentSurveys",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SupervisorName",
                table: "DocumentSurveys",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TemplateCodeSnapshot",
                table: "DocumentSurveys",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TemplateNameSnapshot",
                table: "DocumentSurveys",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ThemeSnapshot",
                table: "DocumentSurveys",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "DocumentSurveys",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "DocumentSurveys");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "DocumentSurveys");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "DocumentSurveys");

            migrationBuilder.DropColumn(
                name: "RequesterEmployeeId",
                table: "DocumentSurveys");

            migrationBuilder.DropColumn(
                name: "RequesterEmployeeName",
                table: "DocumentSurveys");

            migrationBuilder.DropColumn(
                name: "RequesterPositionLevel",
                table: "DocumentSurveys");

            migrationBuilder.DropColumn(
                name: "RequesterPositionName",
                table: "DocumentSurveys");

            migrationBuilder.DropColumn(
                name: "SupervisorId",
                table: "DocumentSurveys");

            migrationBuilder.DropColumn(
                name: "SupervisorName",
                table: "DocumentSurveys");

            migrationBuilder.DropColumn(
                name: "TemplateCodeSnapshot",
                table: "DocumentSurveys");

            migrationBuilder.DropColumn(
                name: "TemplateNameSnapshot",
                table: "DocumentSurveys");

            migrationBuilder.DropColumn(
                name: "ThemeSnapshot",
                table: "DocumentSurveys");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "DocumentSurveys");

            migrationBuilder.AlterColumn<string>(
                name: "DocumentNo",
                table: "DocumentSurveys",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);
        }
    }
}
