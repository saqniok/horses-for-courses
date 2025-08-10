using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HorsesForCourses.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddMappings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Coaches_CoachId",
                table: "Courses");

            migrationBuilder.DropTable(
                name: "TimeSlot");

            migrationBuilder.RenameColumn(
                name: "_requiredSkills",
                table: "Courses",
                newName: "RequiredSkills");

            migrationBuilder.AddColumn<int>(
                name: "AssignedCoachId",
                table: "Courses",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CourseSchedule",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Day = table.Column<string>(type: "TEXT", nullable: false),
                    Start = table.Column<int>(type: "INTEGER", nullable: false),
                    End = table.Column<int>(type: "INTEGER", nullable: false),
                    CourseId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseSchedule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseSchedule_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Coaches_Email",
                table: "Coaches",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseSchedule_CourseId",
                table: "CourseSchedule",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Coaches_CoachId",
                table: "Courses",
                column: "CoachId",
                principalTable: "Coaches",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Coaches_CoachId",
                table: "Courses");

            migrationBuilder.DropTable(
                name: "CourseSchedule");

            migrationBuilder.DropIndex(
                name: "IX_Coaches_Email",
                table: "Coaches");

            migrationBuilder.DropColumn(
                name: "AssignedCoachId",
                table: "Courses");

            migrationBuilder.RenameColumn(
                name: "RequiredSkills",
                table: "Courses",
                newName: "_requiredSkills");

            migrationBuilder.CreateTable(
                name: "TimeSlot",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CourseId = table.Column<int>(type: "INTEGER", nullable: false),
                    Day = table.Column<string>(type: "TEXT", nullable: false),
                    End = table.Column<int>(type: "INTEGER", nullable: false),
                    Start = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeSlot", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeSlot_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TimeSlot_CourseId",
                table: "TimeSlot",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Coaches_CoachId",
                table: "Courses",
                column: "CoachId",
                principalTable: "Coaches",
                principalColumn: "Id");
        }
    }
}
