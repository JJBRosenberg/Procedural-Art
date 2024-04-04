using System.Text.RegularExpressions;
using UnityEngine;

/**
 * This defines a regex attribute [RegexAttribute(pattern, message, options)] or [Regex(...)] in short.
 * The attribute tag to class matching is Unity-behind-the-scenes-magic.
 */
public class RegexAttribute : PropertyAttribute
{
	public readonly string pattern;
	public readonly string helpMessage;
	public readonly RegexOptions options;

	public RegexAttribute(string pattern, string helpMessage, RegexOptions options = RegexOptions.None)
	{
		this.pattern = pattern;
		this.helpMessage = helpMessage;
	}
}