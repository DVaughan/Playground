using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Playground
{
	public static class FormattedStringUtility
	{
		const string specialCharacters = @"([?.])";
		static readonly Regex specialCharactersRegex = new Regex(specialCharacters, RegexOptions.Compiled);

		const string placeHolderPattern = @"(?<All>\{(?<ID>\d+(:\w\w)?)\})";
		static readonly Regex placeHolderRegex = new Regex(placeHolderPattern, RegexOptions.Compiled);

		/// <summary>
		/// Determines if a string that has had string replacements was generated using the specified format.
		/// For example:
		///		"Hi John, nice to meet you."
		/// could have been created using
		///		"Hi {0}, nice to meet {1}."
		/// </summary>
		/// <param name="withReplacements">A string may have resulted from calling string.Format on <see cref="withPlaceHolders"/>.
		/// For example:
		///		"Hi John, nice to meet you."</param>
		/// <param name="withPlaceHolders">A string containing placeholder.
		/// For example:
		///		"Hi {0}, nice to meet {1}."</param>
		/// <param name="placeholders">Contains the format placeholders and their values.
		/// For the format "Hi {0}, nice to meet {1}.",
		/// the dictionary will contain the key value pair '0'->'John' and '1'->'you'.</param>
		public static bool CouldHaveBeenCreatedUsingFormat(this string withReplacements, 
														string withPlaceHolders, 
														out IDictionary<string, string> placeholders)
		{
			// Based on this Stack Overflow answer: Regular expression with placeholders
			// https://stackoverflow.com/questions/28655642/regular-expression-with-placeholders/28656147#28656147

			MatchCollection placeHolderMatches = placeHolderRegex.Matches(withPlaceHolders);
			
			Dictionary<string, (string id, string all)> placeHolderKeys = new Dictionary<string, (string, string)>();
			int counter = 0;

			foreach (Match match in placeHolderMatches)
			{
				string id = match.Groups["ID"].Value;
				string all = match.Groups["All"].Value;
				// Regex group name cannot begin with a digit.
				placeHolderKeys["p" + counter++] = (id, all);
			}

			Dictionary<string, string> dictionary = new Dictionary<string, string>();

			string pattern = withPlaceHolders;
			
			pattern = specialCharactersRegex.Replace(pattern, @"\$1");

			foreach (KeyValuePair<string, (string content, string all)> pair in placeHolderKeys)
			{
				pattern = pattern.Replace(pair.Value.all, $@"(?<{pair.Key}>.+)");
			}

			pattern = pattern.Replace(" ", @"\s");
			
			Regex withReplacementsRegex = new Regex(pattern);
			Match wholeMatch = withReplacementsRegex.Match(withReplacements);

			placeholders = dictionary;

			if (wholeMatch.Success)
			{
				foreach (Group match in wholeMatch.Groups)
				{
					if (!placeHolderKeys.TryGetValue(match.Name, out (string id, string all) tuple))
					{
						continue;
					}

					dictionary[tuple.id] = match.Value;
				}

				return true;
			}

			return false;
		}
	}
}
