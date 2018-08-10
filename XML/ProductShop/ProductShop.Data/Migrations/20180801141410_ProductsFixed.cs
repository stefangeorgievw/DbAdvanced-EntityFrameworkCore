using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductShop.Data.Migrations
{
    public partial class ProductsFixed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Products",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 3,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Products",
                maxLength: 3,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
