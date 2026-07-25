using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SHEIN_Clone_MVC.Migrations
{
    /// <inheritdoc />
    public partial class AddIsPrimaryToProductImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPrimary",
                table: "ProductImages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPrimary",
                table: "ProductImages");
        }
    }
}
