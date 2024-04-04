using UnityEngine;
using System.Text.RegularExpressions;

/**
 * Regexp example, for more info about regular expressions see:
 * https://docs.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-language-quick-reference
 * https://docs.microsoft.com/en-us/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format?redirectedfrom=MSDN
 * 
 * Disclaimer: I am horrible at regular expressions, direct all your questions about them elsewhere ;).
 */
public class RegexExample : MonoBehaviour
{
	//In case you use the Attribute postfix for your attribute classname, you can leave Attribute out
	[RegexAttribute(@"^(?:\d{1,3}\.){3}\d{1,3}$", "Invalid IP address!\nExample: '127.0.0.1'", RegexOptions.None)]
	public string serverAddress = "192.168.0.1";

	[RegexAttribute(@"(.+)(@)(.+)(\.)(.+)$", "Invalid email address!\nExample: 'a@b.com'", RegexOptions.IgnoreCase)]
	public string emailAddress = "a@b.com";
}
