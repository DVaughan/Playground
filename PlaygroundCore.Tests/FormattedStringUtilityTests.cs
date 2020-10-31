using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Playground
{
	public class FormattedStringUtilityTests
	{
		[Fact]
		public void Contains_ShouldReturnTrue()
		{
			string withReplacements = "Hi John, nice to meet you.";
			string withPlaceholders = "Hi {0}, nice to meet {1}.";

			withReplacements.CouldHaveBeenCreatedUsingFormat(withPlaceholders, out IDictionary<string, string> placeholders).Should().BeTrue();
			placeholders.Count.Should().Be(2);
			placeholders["0"].Should().Be("John");
			placeholders["1"].Should().Be("you");
		}

		[Fact]
		public void Contains_ShouldReturnFalse()
		{
			string withReplacements = "Hi John, nice to meet you.";
			string withPlaceholders = "Hi {0}, nice to meet.";

			withReplacements.CouldHaveBeenCreatedUsingFormat(withPlaceholders, out _).Should().BeFalse();
		}

		[Fact]
		public void Contains_ShouldReturnTrueIfTheSame()
		{
			string withReplacements = "Hi John, nice to meet you.";
			withReplacements.CouldHaveBeenCreatedUsingFormat(withReplacements, out IDictionary<string, string> placeholders1).Should().BeTrue();
			placeholders1.Should().NotBeNull();
			placeholders1.Count.Should().Be(0);

			string withPlaceholders = "Hi {0}, nice to meet.";
			withPlaceholders.CouldHaveBeenCreatedUsingFormat(withPlaceholders, out IDictionary<string, string> placeholders2).Should().BeTrue();
			placeholders2.Should().NotBeNull();
			placeholders2.Count.Should().Be(1);
		}
	}
}
