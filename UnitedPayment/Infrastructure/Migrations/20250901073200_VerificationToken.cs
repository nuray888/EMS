using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UnitedPayment.Migrations
{
    /// <inheritdoc />
    public partial class VerificationToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VerificationToken",
                table: "Employees",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VerificationToken",
                table: "Employees");
        }
    }
}
