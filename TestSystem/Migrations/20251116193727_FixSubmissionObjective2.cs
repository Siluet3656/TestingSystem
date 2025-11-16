using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestSystem.Migrations
{
    /// <inheritdoc />
    public partial class FixSubmissionObjective2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Objectives_TaskId",
                table: "Submissions");

            migrationBuilder.DropIndex(
                name: "IX_Submissions_TaskId",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "TaskId",
                table: "Submissions");

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_ObjectiveId",
                table: "Submissions",
                column: "ObjectiveId");

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_Objectives_ObjectiveId",
                table: "Submissions",
                column: "ObjectiveId",
                principalTable: "Objectives",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Objectives_ObjectiveId",
                table: "Submissions");

            migrationBuilder.DropIndex(
                name: "IX_Submissions_ObjectiveId",
                table: "Submissions");

            migrationBuilder.AddColumn<int>(
                name: "TaskId",
                table: "Submissions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_TaskId",
                table: "Submissions",
                column: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_Objectives_TaskId",
                table: "Submissions",
                column: "TaskId",
                principalTable: "Objectives",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
