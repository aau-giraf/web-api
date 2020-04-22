using Microsoft.EntityFrameworkCore.Migrations;

namespace GirafRest.Migrations
{
    public partial class MakeScreenNameInAspnetusersRequired : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE aspnetusers SET DisplayName = UserName WHERE DisplayName IS NULL");
            
            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "AspNetUsers",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "AspNetUsers",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
