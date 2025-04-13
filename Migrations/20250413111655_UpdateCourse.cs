using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentEnrollmentSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_ProgramStudies_ProgramStudyId",
                table: "Courses");

            migrationBuilder.AlterColumn<int>(
                name: "ProgramStudyId",
                table: "Courses",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_ProgramStudies_ProgramStudyId",
                table: "Courses",
                column: "ProgramStudyId",
                principalTable: "ProgramStudies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_ProgramStudies_ProgramStudyId",
                table: "Courses");

            migrationBuilder.AlterColumn<int>(
                name: "ProgramStudyId",
                table: "Courses",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_ProgramStudies_ProgramStudyId",
                table: "Courses",
                column: "ProgramStudyId",
                principalTable: "ProgramStudies",
                principalColumn: "Id");
        }
    }
}
