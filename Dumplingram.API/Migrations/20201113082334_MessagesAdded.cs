using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Dumplingram.API.Migrations
{
    public partial class MessagesAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*migrationBuilder.DropForeignKey(
                name: "FK_Follow_Users_FolloweeId",
                table: "Follow");

            migrationBuilder.DropForeignKey(
                name: "FK_Follow_Users_FollowerId",
                table: "Follow");

            migrationBuilder.DropForeignKey(
                name: "FK_PhotoLikes_Photo_PhotoId",
                table: "PhotoLikes");

            migrationBuilder.DropForeignKey(
                name: "FK_PhotoLikes_Users_UserId",
                table: "PhotoLikes");*/

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SenderId = table.Column<int>(nullable: false),
                    SenderUsername = table.Column<string>(nullable: true),
                    RecipientId = table.Column<int>(nullable: false),
                    RecipientUsername = table.Column<string>(nullable: true),
                    Content = table.Column<string>(nullable: true),
                    DateRead = table.Column<DateTime>(nullable: true),
                    MessageSent = table.Column<DateTime>(nullable: false),
                    SenderDeleted = table.Column<bool>(nullable: false),
                    RecipientDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Messages_Users_RecipientId",
                        column: x => x.RecipientId,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messages_Users_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_RecipientId",
                table: "Messages",
                column: "RecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderId",
                table: "Messages",
                column: "SenderId");

            /*migrationBuilder.AddForeignKey(
                name: "FK_Follow_Users_FolloweeId",
                table: "Follow",
                column: "FolloweeId",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Follow_Users_FollowerId",
                table: "Follow",
                column: "FollowerId",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PhotoLikes_Photo_PhotoId",
                table: "PhotoLikes",
                column: "PhotoId",
                principalTable: "Photo",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PhotoLikes_Users_UserId",
                table: "PhotoLikes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);*/
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            /*migrationBuilder.DropForeignKey(
                name: "FK_Follow_Users_FolloweeId",
                table: "Follow");

            migrationBuilder.DropForeignKey(
                name: "FK_Follow_Users_FollowerId",
                table: "Follow");

            migrationBuilder.DropForeignKey(
                name: "FK_PhotoLikes_Photo_PhotoId",
                table: "PhotoLikes");

            migrationBuilder.DropForeignKey(
                name: "FK_PhotoLikes_Users_UserId",
                table: "PhotoLikes");*/

            migrationBuilder.DropTable(
                name: "Messages");

            /*migrationBuilder.AddForeignKey(
                name: "FK_Follow_Users_FolloweeId",
                table: "Follow",
                column: "FolloweeId",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Follow_Users_FollowerId",
                table: "Follow",
                column: "FollowerId",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhotoLikes_Photo_PhotoId",
                table: "PhotoLikes",
                column: "PhotoId",
                principalTable: "Photo",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhotoLikes_Users_UserId",
                table: "PhotoLikes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);*/
        }
    }
}
