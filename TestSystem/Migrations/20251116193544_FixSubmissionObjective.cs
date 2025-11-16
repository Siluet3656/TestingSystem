using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestSystem.Migrations
{
    /// <inheritdoc />
    public partial class FixSubmissionObjective : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ObjectiveId",
                table: "Submissions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ObjectiveId",
                table: "Submissions");
        }
    }
}
