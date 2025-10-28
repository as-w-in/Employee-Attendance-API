using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_demo.Migrations
{
    /// <inheritdoc />
    public partial class FixDepartmentSpelling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Departmet",
                table: "Employees",
                newName: "Department");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Department",
                table: "Employees",
                newName: "Departmet");
        }
    }
}
