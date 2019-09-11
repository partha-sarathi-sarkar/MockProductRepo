using Microsoft.EntityFrameworkCore.Migrations;

namespace Product.API.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateSequence(
                name: "product_id_seq",
                schema: "public");

            migrationBuilder.CreateTable(
                name: "Products",
                schema: "public",
                columns: table => new
                {
                    ProductID = table.Column<int>(nullable: false, defaultValueSql: "nextval('public.product_id_seq'::regclass)"),
                    ProductName = table.Column<string>(maxLength: 200, nullable: false),
                    ProductDescription = table.Column<string>(maxLength: 500, nullable: true),
                    Price = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products",
                schema: "public");

            migrationBuilder.DropSequence(
                name: "product_id_seq",
                schema: "public");
        }
    }
}
