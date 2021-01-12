using GvGRank_Server.Entities;
using GvGRank_Server.Enums;
using Microsoft.EntityFrameworkCore;

namespace GvGRank_Server.Context
{
	public class VoteDbContext : DbContext
	{
		public DbSet<Player> Players { get; set; }
		public DbSet<Vote> Votes { get; set; }
		public DbSet<User> Users { get; set; }

		public VoteDbContext(DbContextOptions<VoteDbContext> options)
			: base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelbuilder)
		{
			string[] activeFronts = {
				"Alan",
				"Andi",
				"Battle Clog",
				"Bounty",
				"Butters",
				"Chrona",
				"Dervish Xen",
				"Dome",
				"Ego",
				"Endo",
				"Exile",
				"Fedex",
				"Gatos",
				"Gurerro Sauron",
				"Hash",
				"Hemo",
				"Ice",
				"Izzo",
				"Jake",
				"Kai",
				"Matze",
				"Messy",
				"Monky",
				"Nihal",
				"No Chance",
				"Pally",
				"Paul Yang",
				"Portu",
				"Roken",
				"Skittles",
				"Slam",
				"Sync",
				"Takida",
				"Teddy",
				"Zurrie",
			};
			string[] activeMids = {
				"ANT Philip",
				"Arclite",
				"Aspect",
				"Beware",
				"Brent",
				"Candyboy",
				"Chijo",
				"Chrizzo",
				"Crimy",
				"Domi/Far Cry",
				"Dopos",
				"Enso",
				"Ep Loves My Babies",
				"Epica",
				"Fluxy",
				"Francis The Earth",
				"Giggler",
				"Godly/VERYNICE",
				"Hadshi",
				"Honk",
				"Honors",
				"Jorn",
				"Kam",
				"KO",
				"Kobe",
				"Lisek",
				"Lua Kavanuh",
				"Luv",
				"Maga",
				"Mario",
				"Maverick",
				"Miru",
				"Motoko",
				"Nick",
				"Ra",
				"Raffy",
				"Resi",
				"Rudi",
				"Saints",
				"Shisha",
				"Spartan (USA)",
				"Sven",
				"Tequila",
				"TJ",
				"Trunkz",
				"Vesto",
				"Yamamoto",
				"Yoko",
				"Zynkh",
			};

			string[] activeBacks = {
				"Accolade",
				"Akemi",
				"Ali",
				"Amnell",
				"Azin",
				"Blastoise",
				"Boring",
				"Chamalee",
				"Chunin",
				"Daenerys Del Dragon",
				"Demon",
				"DMP",
				"Duchesse",
				"Eraziel",
				"Fate",
				"Goddie",
				"Holye",
				"I Let You Live Long",
				"Izzy",
				"Jacke",
				"Java",
				"Jonas",
				"Karl Monky",
				"Kasperov",
				"Killing",
				"Lao",
				"Leila Of Langlar",
				"Luke",
				"Lynie",
				"Marley",
				"Math",
				"Moo",
				"Nameless",
				"Ne Baktin Canim",
				"Newty",
				"Noya/Martin",
				"Nutella/Pasco",
				"Oln",
				"Pfefferkuchen",
				"Puma Girly",
				"Purif",
				"Rainy",
				"Remede",
				"Robert",
				"Santana",
				"Sebbe",
				"Sistrens",
				"Smoki",
				"Spartan (FR)",
				"Stark",
				"Strng Jpn",
				"Virti",
				"Walky",
				"Willy",
				"Yoshi",
				"Zuvit",
			};

			string[] inactiveFronts = {
			};
			string[] inactiveMids = {
			};
			string[] inactiveBacks = {
			};

			int counter = 1;

			foreach (string playerName in activeFronts)
				modelbuilder.Entity<Player>().HasData(new Player() { Id = counter++, Name = playerName, Active = true, Role = Role.Front });
			foreach (string playerName in activeMids)
				modelbuilder.Entity<Player>().HasData(new Player() { Id = counter++, Name = playerName, Active = true, Role = Role.Mid });
			foreach (string playerName in activeBacks)
				modelbuilder.Entity<Player>().HasData(new Player() { Id = counter++, Name = playerName, Active = true, Role = Role.Back });
			foreach (string playerName in inactiveFronts)
				modelbuilder.Entity<Player>().HasData(new Player() { Id = counter++, Name = playerName, Active = false, Role = Role.Front });
			foreach (string playerName in inactiveMids)
				modelbuilder.Entity<Player>().HasData(new Player() { Id = counter++, Name = playerName, Active = false, Role = Role.Mid });
			foreach (string playerName in inactiveBacks)
				modelbuilder.Entity<Player>().HasData(new Player() { Id = counter++, Name = playerName, Active = false, Role = Role.Back });
		}
	}
}