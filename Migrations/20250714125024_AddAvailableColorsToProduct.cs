using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SHEIN_Clone_MVC.Migrations
{
    /// <inheritdoc />
    public partial class AddAvailableColorsToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvailableColors",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableColors",
                table: "Products");
        }
    }
}
