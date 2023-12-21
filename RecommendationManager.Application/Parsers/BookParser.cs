namespace RecommendationManager.Application.Parsers;

public class BookParser
{
    private static readonly Regex editionPattern =
        new(@"(?:\b[eE]dition\b|\dE)", RegexOptions.Compiled);
    private static readonly Regex editionParsePattern =
        new(@"[-, ]*[(]?(\d{0,2}\w*) ?[eE](?:dition)?\)?$", RegexOptions.Compiled);

    private static readonly Dictionary<string, string> EditionTranslation = new()
    {
        // editions exceptions
        { "portable", string.Empty },
        { "pocket", string.Empty },

        // ordinal editions
        {"1st", "1"},
        {"2nd", "2"},
        {"3rd", "3"},
        {"4th", "4"},
        {"5th", "5"},
        {"6th", "6"},
        {"7th", "7"},
        {"8th", "8"},
        {"9th", "9"},
        {"10th", "10"},
        {"11th", "11"},
        {"12th", "12"},
        {"13th", "13"},
        {"14th", "14"},
        {"15th", "15"},
        {"16th", "16"},
        {"17th", "17"},
        {"18th", "18"},
        {"19th", "19"},
        {"20th", "20"},

        // written editions
        {"first", "1"},
        {"second", "2"},
        {"third", "3"},
        {"fourth", "4"},
        {"fifth", "5"},
        {"sixth", "6"},
        {"seventh", "7"},
        {"eighth", "8"},
        {"ninth", "9"},
        {"tenth", "10"},
        {"eleventh", "11"},
        {"twelfth", "12"},
        {"thirteenth", "13"},
        {"fourteenth", "14"},
        {"fifteenth", "15"},
        {"sixteenth", "16"},
        {"seventeenth", "17"},
        {"eighteenth", "18"},
        {"nineteenth", "19"},
        {"twentieth", "20"},
    };


    public static Book FromBundle(string bundleName, string bundleItem)
    {
        if (string.IsNullOrWhiteSpace(bundleItem))
        {
            throw new ArgumentNullException(nameof(bundleItem));
        }

        if (editionPattern.IsMatch(bundleItem))
        {
            var editionMatch = editionParsePattern.Match(bundleItem);
            if (!editionMatch.Success)
            {
                // Notify, new pattern emerged
                throw new Exception("Failed to parse edition");
            }

            var name = bundleItem.Replace(editionMatch.Groups[0].Value, string.Empty);
            var edition = editionMatch.Groups[1].Value;

            var hasTranslation = EditionTranslation.TryGetValue(
                edition.ToLowerInvariant(), out var translatedEdition);

            return new Book
            {
                Title = name,
                Edition = hasTranslation ? translatedEdition : edition,
                Source = bundleName
            };
        }

        return new Book
        {
            Title = bundleItem,
            Edition = string.Empty,
            Source = bundleName
        };
    }
}
