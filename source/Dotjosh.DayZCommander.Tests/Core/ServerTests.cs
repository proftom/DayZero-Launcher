using System;
using Dotjosh.DayZCommander.App.Core;
using NUnit.Framework;

namespace Dotjosh.DayZCommander.Tests.Core
{
    [TestFixture]
    public class When_getting_server_time
    {
        [Test]
        public void Single_digit_offsets_parse_correctly()
        {
            var server = new Server(null, 0);
            server.Settings["hostname"] = @"DayZ - [GMT+2] dayzmod.com - hosted by LegoDeCom & Hulk";
            Assert.That(server.ServerTime.Value.Hour, Is.EqualTo(DateTime.UtcNow.AddHours(+2).Hour));
        }

        [Test]
        public void Double_digit_offsets_parse_correctly()
        {
            var server = new Server(null, 0);
            server.Settings["hostname"] = @"DayZ - [GMT+12] dayzmod.com - hosted by LegoDeCom & Hulk";
            Assert.That(server.ServerTime.Value.Hour, Is.EqualTo(DateTime.UtcNow.AddHours(+12).Hour));
        }
    }
}
