using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HorsesForCourses.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate22 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CoachId",
                table: "Courses",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Courses_CoachId",
                table: "Courses",
                column: "CoachId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Coaches_CoachId",
                table: "Courses",
                column: "CoachId",
                principalTable: "Coaches",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Coaches_CoachId",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_CoachId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "CoachId",
                table: "Courses");
        }
    }
}
