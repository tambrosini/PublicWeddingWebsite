using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeddingInvites.Migrations
{
    /// <inheritdoc />
    public partial class SingleRsvpValidation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RsvpCompleted",
                schema: "dbo",
                table: "Invite",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RsvpCompleted",
                schema: "dbo",
                table: "Invite");
        }
    }
}
