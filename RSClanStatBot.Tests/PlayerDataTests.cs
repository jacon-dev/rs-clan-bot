using FakeItEasy;
using NUnit.Framework.Internal;
using RSClanStatBot.ClanStatistics.Converters;
using RSClanStatBot.Interface.Services;

namespace RSClanStatBot.Tests
{
    public class PlayerDataTests
    {
        private IHelperService fakeHelperService;
        private PlayerDataToCappingStatisticConverter sut;
        private const string FakePlayerName = "FakePlayer";

        [SetUp]
        public void Setup()
        {
            fakeHelperService = A.Fake<IHelperService>();
            sut = new PlayerDataToCappingStatisticConverter(fakeHelperService);
        }

        [Test]
        public async Task A_player_has_capped_after_the_plot_refresh_shows_HasCapped_as_true()
        {
            var jsonContent = await new StreamReader("fake-player-api-response.json").ReadToEndAsync();

            A.CallTo(() => fakeHelperService.GetLastPlotRefreshDate())
                .Returns(new DateTime(2025, 4, 20));
            
            var result = sut.Convert(jsonContent);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.HasCapped, Is.True);
            Assert.That(result.PlayerName, Is.EqualTo(FakePlayerName));
            Assert.That(result.HasErrored, Is.False);
        }

        [Test]
        public async Task A_player_has_capped_before_the_plot_refresh_shows_HasCapped_as_false()
        {
            var jsonContent = await new StreamReader("fake-player-api-response.json").ReadToEndAsync();

            A.CallTo(() => fakeHelperService.GetLastPlotRefreshDate())
                .Returns(new DateTime(2025, 4, 30));

            var result = sut.Convert(jsonContent);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.HasCapped, Is.False);
            Assert.That(result.PlayerName, Is.EqualTo(FakePlayerName));
            Assert.That(result.HasErrored, Is.False);
        }

        [Test]
        public async Task A_player_with_private_profile_should_return_null_response()
        {
            var jsonContent = await new StreamReader("fake-player-api-response-private.json").ReadToEndAsync();

            var result = sut.Convert(jsonContent);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.HasCapped, Is.False);
            Assert.That(result.PlayerName, Is.EqualTo(FakePlayerName));
            Assert.That(result.HasErrored, Is.False);
            Assert.That(result.IsPrivate, Is.True);
        }

        [Test]
        public async Task A_player_has_not_capped_shows_HasCapped_as_false()
        {
            var jsonContent = await new StreamReader("fake-player-api-response-no-cap.json").ReadToEndAsync();

            var result = sut.Convert(jsonContent);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.HasCapped, Is.False);
            Assert.That(result.PlayerName, Is.EqualTo(FakePlayerName));
            Assert.That(result.HasErrored, Is.False);
        }

        [Test]
        public async Task A_player_that_has_errored_data_shows_HasErrored_as_true()
        {
            var jsonContent = await new StreamReader("fake-player-api-response-error.json").ReadToEndAsync();

            var result = sut.Convert(null);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.HasCapped, Is.False);
            Assert.That(result.PlayerName, Is.Null);
            Assert.That(result.HasErrored, Is.True);
        }

        [Test]
        public void A_player_that_has_null_data_shows_HasErrored_as_true()
        {
            var result = sut.Convert(null);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.HasCapped, Is.False);
            Assert.That(result.PlayerName, Is.Null);
            Assert.That(result.HasErrored, Is.True);
        }
    }
}