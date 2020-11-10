using Microsoft.EntityFrameworkCore.Migrations;

namespace Dumplingram.API.Migrations
{
    public partial class PhotoLikesAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PhotoLikes",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    PhotoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhotoLikes", x => new { x.UserId, x.PhotoId });
                    table.ForeignKey(
                        name: "FK_PhotoLikes_Photo_PhotoId",
                        column: x => x.PhotoId,
                        principalTable: "Photo",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhotoLikes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PhotoLikes_PhotoId",
                table: "PhotoLikes",
                column: "PhotoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PhotoLikes");
        }
    }
}
