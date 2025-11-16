using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddObjectivesAndTestCases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestCase_Objectives_ObjectiveId",
                table: "TestCase");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TestCase",
                table: "TestCase");

            migrationBuilder.RenameTable(
                name: "TestCase",
                newName: "TestCases");

            migrationBuilder.RenameIndex(
                name: "IX_TestCase_ObjectiveId",
                table: "TestCases",
                newName: "IX_TestCases_ObjectiveId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestCases",
                table: "TestCases",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TestCases_Objectives_ObjectiveId",
                table: "TestCases",
                column: "ObjectiveId",
                principalTable: "Objectives",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestCases_Objectives_ObjectiveId",
                table: "TestCases");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TestCases",
                table: "TestCases");

            migrationBuilder.RenameTable(
                name: "TestCases",
                newName: "TestCase");

            migrationBuilder.RenameIndex(
                name: "IX_TestCases_ObjectiveId",
                table: "TestCase",
                newName: "IX_TestCase_ObjectiveId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestCase",
                table: "TestCase",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TestCase_Objectives_ObjectiveId",
                table: "TestCase",
                column: "ObjectiveId",
                principalTable: "Objectives",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
