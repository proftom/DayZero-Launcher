using zombiesnu.DayZeroLauncher.App.Core;
using NUnit.Framework;

// ReSharper disable CheckNamespace
namespace zombiesnu.DayZeroLauncher.Tests.Core.ServerInfoTests
{
	// ReSharper disable InconsistentNaming
	[TestFixture]
	public class When_getting_recruit_server_info : ServerInfoTest
	{
		public override ServerInfoTestParameters GetServerInfoTestParameters()
		{
			return new ServerInfoTestParameters { Difficulty = ServerDifficulty.Recruit };
		}

		[Test]
		public void Armor_is_on()
		{
			Assert.That(ServerInfo.Armor.Enabled, Is.True);
			Assert.That(ServerInfo.Armor.Confirmed, Is.False);
		}

		[Test]
		public void ThirdPerson_is_on()
		{
			Assert.That(ServerInfo.ThirdPerson.Enabled, Is.True);
			Assert.That(ServerInfo.ThirdPerson.Confirmed, Is.False);
		}

		[Test]
		public void Tracers_are_on()
		{
			Assert.That(ServerInfo.Tracers.Enabled, Is.True);
			Assert.That(ServerInfo.Tracers.Confirmed, Is.False);
		}

		[Test]
		public void Nameplates_are_on()
		{
			Assert.That(ServerInfo.Nameplates.Enabled, Is.True);
			Assert.That(ServerInfo.Nameplates.Confirmed, Is.False);
		}

		[Test]
		public void Crosshairs_are_on()
		{
			Assert.That(ServerInfo.Crosshairs.Enabled, Is.True);
			Assert.That(ServerInfo.Crosshairs.Confirmed, Is.False);
		}

		[Test]
		public void DeathMessages_are_on()
		{
			Assert.That(ServerInfo.DeathMessages.Enabled, Is.True);
			Assert.That(ServerInfo.DeathMessages.Confirmed, Is.False);
		}

		[Test]
		public void Scores_are_on()
		{
			Assert.That(ServerInfo.Scores.Enabled, Is.True);
			Assert.That(ServerInfo.Scores.Confirmed, Is.False);
		}
	}

	[TestFixture]
	public class When_getting_regular_server_info : ServerInfoTest
	{
		public override ServerInfoTestParameters GetServerInfoTestParameters()
		{
			return new ServerInfoTestParameters { Difficulty = ServerDifficulty.Regular };
		}

		[Test]
		public void Armor_is_on()
		{
			Assert.That(ServerInfo.Armor.Enabled, Is.True);
			Assert.That(ServerInfo.Armor.Confirmed, Is.False);
		}

		[Test]
		public void ThirdPerson_is_on()
		{
			Assert.That(ServerInfo.ThirdPerson.Enabled, Is.True);
			Assert.That(ServerInfo.ThirdPerson.Confirmed, Is.False);
		}

		[Test]
		public void Tracers_are_on()
		{
			Assert.That(ServerInfo.Tracers.Enabled, Is.False);
			Assert.That(ServerInfo.Tracers.Confirmed, Is.False);
		}

		[Test]
		public void Nameplates_are_on()
		{
			Assert.That(ServerInfo.Nameplates.Enabled, Is.True);
			Assert.That(ServerInfo.Nameplates.Confirmed, Is.False);
		}

		[Test]
		public void Crosshairs_are_on()
		{
			Assert.That(ServerInfo.Crosshairs.Enabled, Is.True);
			Assert.That(ServerInfo.Crosshairs.Confirmed, Is.False);
		}

		[Test]
		public void DeathMessages_are_on()
		{
			Assert.That(ServerInfo.DeathMessages.Enabled, Is.True);
			Assert.That(ServerInfo.DeathMessages.Confirmed, Is.False);
		}

		[Test]
		public void Scores_are_on()
		{
			Assert.That(ServerInfo.Scores.Enabled, Is.True);
			Assert.That(ServerInfo.Scores.Confirmed, Is.False);
		}
	}

	[TestFixture]
	public class When_getting_veteran_server_info : ServerInfoTest
	{
		public override ServerInfoTestParameters GetServerInfoTestParameters()
		{
			return new ServerInfoTestParameters { Difficulty = ServerDifficulty.Veteran };
		}

		[Test]
		public void Armor_is_on()
		{
			Assert.That(ServerInfo.Armor.Enabled, Is.False);
			Assert.That(ServerInfo.Armor.Confirmed, Is.True);
		}

		[Test]
		public void ThirdPerson_is_on()
		{
			Assert.That(ServerInfo.ThirdPerson.Enabled, Is.True);
			Assert.That(ServerInfo.ThirdPerson.Confirmed, Is.False);
		}

		[Test]
		public void Tracers_are_on()
		{
			Assert.That(ServerInfo.Tracers.Enabled, Is.False);
			Assert.That(ServerInfo.Tracers.Confirmed, Is.False);
		}

		[Test]
		public void Nameplates_are_on()
		{
			Assert.That(ServerInfo.Nameplates.Enabled, Is.False);
			Assert.That(ServerInfo.Nameplates.Confirmed, Is.True);
		}

		[Test]
		public void Crosshairs_are_on()
		{
			Assert.That(ServerInfo.Crosshairs.Enabled, Is.False);
			Assert.That(ServerInfo.Crosshairs.Confirmed, Is.False);
		}

		[Test]
		public void DeathMessages_are_on()
		{
			Assert.That(ServerInfo.DeathMessages.Enabled, Is.True);
			Assert.That(ServerInfo.DeathMessages.Confirmed, Is.False);
		}

		[Test]
		public void Scores_are_on()
		{
			Assert.That(ServerInfo.Scores.Enabled, Is.True);
			Assert.That(ServerInfo.Scores.Confirmed, Is.False);
		}
	}

	[TestFixture]
	public class When_getting_expert_server_info : ServerInfoTest
	{
		public override ServerInfoTestParameters GetServerInfoTestParameters()
		{
			return new ServerInfoTestParameters { Difficulty = ServerDifficulty.Expert };
		}

		[Test]
		public void Armor_is_on()
		{
			Assert.That(ServerInfo.Armor.Enabled, Is.False);
			Assert.That(ServerInfo.Armor.Confirmed, Is.True);
		}

		[Test]
		public void ThirdPerson_is_on()
		{
			Assert.That(ServerInfo.ThirdPerson.Enabled, Is.False);
			Assert.That(ServerInfo.ThirdPerson.Confirmed, Is.True);
		}

		[Test]
		public void Tracers_are_on()
		{
			Assert.That(ServerInfo.Tracers.Enabled, Is.False);
			Assert.That(ServerInfo.Tracers.Confirmed, Is.True);
		}

		[Test]
		public void Nameplates_are_on()
		{
			Assert.That(ServerInfo.Nameplates.Enabled, Is.False);
			Assert.That(ServerInfo.Nameplates.Confirmed, Is.True);
		}

		[Test]
		public void Crosshairs_are_on()
		{
			Assert.That(ServerInfo.Crosshairs.Enabled, Is.False);
			Assert.That(ServerInfo.Crosshairs.Confirmed, Is.True);
		}

		[Test]
		public void DeathMessages_are_on()
		{
			Assert.That(ServerInfo.DeathMessages.Enabled, Is.False);
			Assert.That(ServerInfo.DeathMessages.Confirmed, Is.False);
		}

		[Test]
		public void Scores_are_on()
		{
			Assert.That(ServerInfo.Scores.Enabled, Is.True);
			Assert.That(ServerInfo.Scores.Confirmed, Is.False);
		}
	}

	public abstract class ServerInfoTest
	{
		public ServerInfo ServerInfo { get; private set; }
		
		public abstract ServerInfoTestParameters GetServerInfoTestParameters();

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			var parameters = GetServerInfoTestParameters();
			ServerInfo = new ServerInfo(parameters.Difficulty, parameters.ServerName);
		}
	}

	public class ServerInfoTestParameters
	{
		public ServerDifficulty? Difficulty { get; set; }
		public string ServerName { get; set; }
	}
}