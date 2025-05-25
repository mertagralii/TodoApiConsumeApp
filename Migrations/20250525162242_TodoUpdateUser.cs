using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoApiConsumeApp.Migrations
{
    /// <inheritdoc />
    public partial class TodoUpdateUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Todos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TodoId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_TodoId",
                table: "AspNetUsers",
                column: "TodoId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Todos_TodoId",
                table: "AspNetUsers",
                column: "TodoId",
                principalTable: "Todos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Todos_TodoId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_TodoId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Todos");

            migrationBuilder.DropColumn(
                name: "TodoId",
                table: "AspNetUsers");
        }
    }
}
