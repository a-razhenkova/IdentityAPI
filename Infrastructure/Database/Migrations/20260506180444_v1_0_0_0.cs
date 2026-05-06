using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class v1_0_0_0 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "client",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    name = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false),
                    key = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false),
                    secret = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false),
                    wrong_login_attempts_counter = table.Column<int>(type: "int", nullable: false),
                    is_internal = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_client", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "document",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    sign_timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    checksum = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_document", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    public_id = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false),
                    username = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false),
                    role = table.Column<int>(type: "int", nullable: false),
                    otp_secret = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_verified = table.Column<bool>(type: "bit", nullable: false),
                    registration_timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "client_right",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    client_id = table.Column<long>(type: "bigint", nullable: false),
                    can_notify = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_client_right", x => x.id);
                    table.ForeignKey(
                        name: "FK_client_right_client_client_id",
                        column: x => x.client_id,
                        principalTable: "client",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "client_status",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    client_id = table.Column<long>(type: "bigint", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    reason = table.Column<int>(type: "int", nullable: false),
                    note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_client_status", x => x.id);
                    table.ForeignKey(
                        name: "FK_client_status_client_client_id",
                        column: x => x.client_id,
                        principalTable: "client",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "subscription",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    create_timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    expiration_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    contract_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscription", x => x.id);
                    table.ForeignKey(
                        name: "FK_subscription_document_contract_id",
                        column: x => x.contract_id,
                        principalTable: "document",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "login",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    wrong_login_attempts_counter = table.Column<int>(type: "int", nullable: false),
                    last_login_timestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    last_login_ip_address = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_login", x => x.id);
                    table.ForeignKey(
                        name: "FK_login_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_password",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    password = table.Column<string>(type: "varchar(256)", unicode: false, maxLength: 256, nullable: false),
                    secret = table.Column<string>(type: "varchar(128)", unicode: false, maxLength: 128, nullable: false),
                    last_changed_timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_password", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_password_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_status",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    reason = table.Column<int>(type: "int", nullable: false),
                    note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_status", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_status_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "client_subscription",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    client_id = table.Column<long>(type: "bigint", nullable: false),
                    subscription_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_client_subscription", x => x.id);
                    table.ForeignKey(
                        name: "FK_client_subscription_client_client_id",
                        column: x => x.client_id,
                        principalTable: "client",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_client_subscription_subscription_subscription_id",
                        column: x => x.subscription_id,
                        principalTable: "subscription",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_client_key",
                table: "client",
                column: "key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_client_right_client_id",
                table: "client_right",
                column: "client_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_client_status_client_id",
                table: "client_status",
                column: "client_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_client_subscription_client_id",
                table: "client_subscription",
                column: "client_id");

            migrationBuilder.CreateIndex(
                name: "IX_client_subscription_subscription_id",
                table: "client_subscription",
                column: "subscription_id");

            migrationBuilder.CreateIndex(
                name: "IX_login_user_id",
                table: "login",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_subscription_contract_id",
                table: "subscription",
                column: "contract_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_public_id",
                table: "user",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_username",
                table: "user",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_password_user_id",
                table: "user_password",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_status_user_id",
                table: "user_status",
                column: "user_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "client_right");

            migrationBuilder.DropTable(
                name: "client_status");

            migrationBuilder.DropTable(
                name: "client_subscription");

            migrationBuilder.DropTable(
                name: "login");

            migrationBuilder.DropTable(
                name: "user_password");

            migrationBuilder.DropTable(
                name: "user_status");

            migrationBuilder.DropTable(
                name: "client");

            migrationBuilder.DropTable(
                name: "subscription");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.DropTable(
                name: "document");
        }
    }
}
