using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace GvGRank_Server.Migrations
{
	public partial class InitialCreate : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "Players",
				columns: table => new
				{
					Id = table.Column<int>(nullable: false)
						.Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
					Name = table.Column<string>(maxLength: 20, nullable: false),
					Shitlo = table.Column<int>(nullable: false),
					Role = table.Column<int>(nullable: false),
					Wins = table.Column<int>(nullable: false),
					Losses = table.Column<int>(nullable: false),
					Active = table.Column<bool>(nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Players", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "Users",
				columns: table => new
				{
					Id = table.Column<int>(nullable: false)
						.Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
					Ip = table.Column<string>(maxLength: 64, nullable: false),
					VoteLimit = table.Column<int>(nullable: false),
					AntiTamper = table.Column<int>(nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Users", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "Votes",
				columns: table => new
				{
					Id = table.Column<int>(nullable: false)
						.Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
					Date = table.Column<DateTime>(nullable: false),
					UserId = table.Column<int>(nullable: false),
					WinId = table.Column<int>(nullable: false),
					LoseId = table.Column<int>(nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Votes", x => x.Id);
				});

			migrationBuilder.InsertData(
				table: "Players",
				columns: new[] { "Id", "Active", "Losses", "Name", "Role", "Shitlo", "Wins" },
				values: new object[,]
				{
					{ 1, true, 0, "Alan", 1, 0, 0 },
					{ 103, true, 0, "Izzy", 3, 0, 0 },
					{ 102, true, 0, "I Let You Live Long", 3, 0, 0 },
					{ 101, true, 0, "Holye", 3, 0, 0 },
					{ 100, true, 0, "Goddie", 3, 0, 0 },
					{ 99, true, 0, "Fate", 3, 0, 0 },
					{ 98, true, 0, "Eraziel", 3, 0, 0 },
					{ 97, true, 0, "Duchesse", 3, 0, 0 },
					{ 96, true, 0, "DMP", 3, 0, 0 },
					{ 95, true, 0, "Demon", 3, 0, 0 },
					{ 94, true, 0, "Daenerys Del Dragon", 3, 0, 0 },
					{ 93, true, 0, "Chunin", 3, 0, 0 },
					{ 92, true, 0, "Chamalee", 3, 0, 0 },
					{ 91, true, 0, "Boring", 3, 0, 0 },
					{ 90, true, 0, "Blastoise", 3, 0, 0 },
					{ 89, true, 0, "Azin", 3, 0, 0 },
					{ 88, true, 0, "Amnell", 3, 0, 0 },
					{ 87, true, 0, "Ali", 3, 0, 0 },
					{ 73, true, 0, "Rudi", 2, 0, 0 },
					{ 74, true, 0, "Saints", 2, 0, 0 },
					{ 75, true, 0, "Shisha", 2, 0, 0 },
					{ 76, true, 0, "Spartan (USA)", 2, 0, 0 },
					{ 77, true, 0, "Sven", 2, 0, 0 },
					{ 78, true, 0, "Tequila", 2, 0, 0 },
					{ 104, true, 0, "Jacke", 3, 0, 0 },
					{ 79, true, 0, "TJ", 2, 0, 0 },
					{ 81, true, 0, "Vesto", 2, 0, 0 },
					{ 82, true, 0, "Yamamoto", 2, 0, 0 },
					{ 83, true, 0, "Yoko", 2, 0, 0 },
					{ 84, true, 0, "Zynkh", 2, 0, 0 },
					{ 85, true, 0, "Accolade", 3, 0, 0 },
					{ 86, true, 0, "Akemi", 3, 0, 0 },
					{ 80, true, 0, "Trunkz", 2, 0, 0 },
					{ 72, true, 0, "Resi", 2, 0, 0 },
					{ 105, true, 0, "Java", 3, 0, 0 },
					{ 107, true, 0, "Karl Monky", 3, 0, 0 },
					{ 138, true, 0, "Willy", 3, 0, 0 },
					{ 137, true, 0, "Walky", 3, 0, 0 },
					{ 136, true, 0, "Virti", 3, 0, 0 },
					{ 135, true, 0, "Strng Jpn", 3, 0, 0 },
					{ 134, true, 0, "Stark", 3, 0, 0 },
					{ 133, true, 0, "Spartan (FR)", 3, 0, 0 },
					{ 132, true, 0, "Smoki", 3, 0, 0 },
					{ 131, true, 0, "Sistrens", 3, 0, 0 },
					{ 130, true, 0, "Sebbe", 3, 0, 0 },
					{ 129, true, 0, "Santana", 3, 0, 0 },
					{ 128, true, 0, "Robert", 3, 0, 0 },
					{ 127, true, 0, "Remede", 3, 0, 0 },
					{ 126, true, 0, "Rainy", 3, 0, 0 },
					{ 125, true, 0, "Purif", 3, 0, 0 },
					{ 124, true, 0, "Puma Girly", 3, 0, 0 },
					{ 123, true, 0, "Pfefferkuchen", 3, 0, 0 },
					{ 122, true, 0, "Oln", 3, 0, 0 },
					{ 108, true, 0, "Kasperov", 3, 0, 0 },
					{ 109, true, 0, "Killing", 3, 0, 0 },
					{ 110, true, 0, "Lao", 3, 0, 0 },
					{ 111, true, 0, "Leila Of Langlar", 3, 0, 0 },
					{ 112, true, 0, "Luke", 3, 0, 0 },
					{ 113, true, 0, "Lynie", 3, 0, 0 },
					{ 106, true, 0, "Jonas", 3, 0, 0 },
					{ 114, true, 0, "Marley", 3, 0, 0 },
					{ 116, true, 0, "Moo", 3, 0, 0 },
					{ 117, true, 0, "Nameless", 3, 0, 0 },
					{ 118, true, 0, "Ne Baktin Canim", 3, 0, 0 },
					{ 119, true, 0, "Newty", 3, 0, 0 },
					{ 120, true, 0, "Noya/Martin", 3, 0, 0 },
					{ 121, true, 0, "Nutella/Pasco", 3, 0, 0 },
					{ 115, true, 0, "Math", 3, 0, 0 },
					{ 71, true, 0, "Raffy", 2, 0, 0 },
					{ 70, true, 0, "Ra", 2, 0, 0 },
					{ 69, true, 0, "Nick", 2, 0, 0 },
					{ 32, true, 0, "Sync", 1, 0, 0 },
					{ 31, true, 0, "Slam", 1, 0, 0 },
					{ 30, true, 0, "Skittles", 1, 0, 0 },
					{ 29, true, 0, "Roken", 1, 0, 0 },
					{ 28, true, 0, "Portu", 1, 0, 0 },
					{ 27, true, 0, "Paul Yang", 1, 0, 0 },
					{ 26, true, 0, "Pally", 1, 0, 0 },
					{ 25, true, 0, "No Chance", 1, 0, 0 },
					{ 24, true, 0, "Nihal", 1, 0, 0 },
					{ 23, true, 0, "Monky", 1, 0, 0 },
					{ 22, true, 0, "Messy", 1, 0, 0 },
					{ 21, true, 0, "Matze", 1, 0, 0 },
					{ 20, true, 0, "Kai", 1, 0, 0 },
					{ 19, true, 0, "Jake", 1, 0, 0 },
					{ 18, true, 0, "Izzo", 1, 0, 0 },
					{ 17, true, 0, "Ice", 1, 0, 0 },
					{ 16, true, 0, "Hemo", 1, 0, 0 },
					{ 2, true, 0, "Andi", 1, 0, 0 },
					{ 3, true, 0, "Battle Clog", 1, 0, 0 },
					{ 4, true, 0, "Bounty", 1, 0, 0 },
					{ 5, true, 0, "Butters", 1, 0, 0 },
					{ 6, true, 0, "Chrona", 1, 0, 0 },
					{ 7, true, 0, "Dervish Xen", 1, 0, 0 },
					{ 33, true, 0, "Takida", 1, 0, 0 },
					{ 8, true, 0, "Dome", 1, 0, 0 },
					{ 10, true, 0, "Endo", 1, 0, 0 },
					{ 11, true, 0, "Exile", 1, 0, 0 },
					{ 12, true, 0, "Fedex", 1, 0, 0 },
					{ 13, true, 0, "Gatos", 1, 0, 0 },
					{ 14, true, 0, "Gurerro Sauron", 1, 0, 0 },
					{ 15, true, 0, "Hash", 1, 0, 0 },
					{ 9, true, 0, "Ego", 1, 0, 0 },
					{ 34, true, 0, "Teddy", 1, 0, 0 },
					{ 35, true, 0, "Zurrie", 1, 0, 0 },
					{ 36, true, 0, "ANT Philip", 2, 0, 0 },
					{ 55, true, 0, "Honk", 2, 0, 0 },
					{ 56, true, 0, "Honors", 2, 0, 0 },
					{ 57, true, 0, "Jorn", 2, 0, 0 },
					{ 58, true, 0, "Kam", 2, 0, 0 },
					{ 59, true, 0, "KO", 2, 0, 0 },
					{ 60, true, 0, "Kobe", 2, 0, 0 },
					{ 54, true, 0, "Hadshi", 2, 0, 0 },
					{ 61, true, 0, "Lisek", 2, 0, 0 },
					{ 63, true, 0, "Luv", 2, 0, 0 },
					{ 64, true, 0, "Maga", 2, 0, 0 },
					{ 65, true, 0, "Mario", 2, 0, 0 },
					{ 66, true, 0, "Maverick", 2, 0, 0 },
					{ 67, true, 0, "Miru", 2, 0, 0 },
					{ 68, true, 0, "Motoko", 2, 0, 0 },
					{ 62, true, 0, "Lua Kavanuh", 2, 0, 0 },
					{ 139, true, 0, "Yoshi", 3, 0, 0 },
					{ 53, true, 0, "Godly/VERYNICE", 2, 0, 0 },
					{ 51, true, 0, "Francis The Earth", 2, 0, 0 },
					{ 37, true, 0, "Arclite", 2, 0, 0 },
					{ 38, true, 0, "Aspect", 2, 0, 0 },
					{ 39, true, 0, "Beware", 2, 0, 0 },
					{ 40, true, 0, "Brent", 2, 0, 0 },
					{ 41, true, 0, "Candyboy", 2, 0, 0 },
					{ 42, true, 0, "Chijo", 2, 0, 0 },
					{ 52, true, 0, "Giggler", 2, 0, 0 },
					{ 43, true, 0, "Chrizzo", 2, 0, 0 },
					{ 45, true, 0, "Domi/Far Cry", 2, 0, 0 },
					{ 46, true, 0, "Dopos", 2, 0, 0 },
					{ 47, true, 0, "Enso", 2, 0, 0 },
					{ 48, true, 0, "Ep Loves My Babies", 2, 0, 0 },
					{ 49, true, 0, "Epica", 2, 0, 0 },
					{ 50, true, 0, "Fluxy", 2, 0, 0 },
					{ 44, true, 0, "Crimy", 2, 0, 0 },
					{ 140, true, 0, "Zuvit", 3, 0, 0 }
				});
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "Players");

			migrationBuilder.DropTable(
				name: "Users");

			migrationBuilder.DropTable(
				name: "Votes");
		}
	}
}
