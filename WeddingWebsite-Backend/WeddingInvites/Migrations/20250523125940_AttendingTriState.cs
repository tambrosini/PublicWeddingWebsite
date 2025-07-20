using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeddingInvites.Migrations
{
    /// <inheritdoc />
    public partial class AttendingTriState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Attending",
                schema: "dbo",
                table: "Guest",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Attending",
                schema: "dbo",
                table: "Guest",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);
        }
    }
}
