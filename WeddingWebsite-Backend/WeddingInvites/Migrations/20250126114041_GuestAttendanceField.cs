using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeddingInvites.Migrations
{
    /// <inheritdoc />
    public partial class GuestAttendanceField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Attending",
                schema: "dbo",
                table: "Guest",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Attending",
                schema: "dbo",
                table: "Guest");
        }
    }
}
