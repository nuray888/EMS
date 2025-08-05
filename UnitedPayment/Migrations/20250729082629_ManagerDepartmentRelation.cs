using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UnitedPayment.Migrations
{
    /// <inheritdoc />
    public partial class ManagerDepartmentRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "Managers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Managers_DepartmentId",
                table: "Managers",
                column: "DepartmentId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Managers_Department_DepartmentId",
                table: "Managers",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Managers_Department_DepartmentId",
                table: "Managers");

            migrationBuilder.DropIndex(
                name: "IX_Managers_DepartmentId",
                table: "Managers");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Managers");
        }
    }
}
