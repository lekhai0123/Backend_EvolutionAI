using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_Evolution.Migrations
{
    /// <inheritdoc />
    public partial class AddIsApprovedToDish : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Dishes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Dishes");
        }
    }
}
