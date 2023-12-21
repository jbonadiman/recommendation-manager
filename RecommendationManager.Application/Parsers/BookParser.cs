using System.Text.RegularExpressions;
using RecommendationManager.Application.Interfaces;
using RecommendationManager.Domain;

namespace RecommendationManager.Application.Parsers;

public class BookParser : IBookParser
{
    private readonly INotificationService _notificationService;

    private static readonly Regex editionPattern =
        new(@"(?:\b[eE]dition\b|\dE)", RegexOptions.Compiled);

    private static readonly Regex editionParsePattern =
        new(
            @"[-, ]*[(]?(\d{0,2}\w*) ?[eE](?:dition)?\)?$",
            RegexOptions.Compiled);

    private static readonly Dictionary<string, string> EditionTranslation = new()
    {
        // editions exceptions
        { "portable", string.Empty },
        { "pocket", string.Empty },

        // ordinal editions
        { "1st", "1" },
        { "2nd", "2" },
        { "3rd", "3" },
        { "4th", "4" },
        { "5th", "5" },
        { "6th", "6" },
        { "7th", "7" },
        { "8th", "8" },
        { "9th", "9" },
        { "10th", "10" },
        { "11th", "11" },
        { "12th", "12" },
        { "13th", "13" },
        { "14th", "14" },
        { "15th", "15" },
        { "16th", "16" },
        { "17th", "17" },
        { "18th", "18" },
        { "19th", "19" },
        { "20th", "20" },

        // written editions
        { "first", "1" },
        { "second", "2" },
        { "third", "3" },
        { "fourth", "4" },
        { "fifth", "5" },
        { "sixth", "6" },
        { "seventh", "7" },
        { "eighth", "8" },
        { "ninth", "9" },
        { "tenth", "10" },
        { "eleventh", "11" },
        { "twelfth", "12" },
        { "thirteenth", "13" },
        { "fourteenth", "14" },
        { "fifteenth", "15" },
        { "sixteenth", "16" },
        { "seventeenth", "17" },
        { "eighteenth", "18" },
        { "nineteenth", "19" },
        { "twentieth", "20" }
    };

    public BookParser(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async IAsyncEnumerable<Book> ParseBundle(BookBundle bundle)
    {
        if (string.IsNullOrWhiteSpace(bundle.Name))
        {
            throw new ArgumentNullException(nameof(bundle));
        }

        foreach (var bundleItem in bundle.Items)
        {
            if (string.IsNullOrWhiteSpace(bundleItem))
            {
                continue;
            }

            if (!editionPattern.IsMatch(bundleItem))
            {
                yield return new Book
                {
                    Title = bundleItem,
                    Edition = string.Empty,
                    Source = bundle.Name
                };
                continue;
            }

            var editionMatch = editionParsePattern.Match(bundleItem);
            if (!editionMatch.Success)
            {
                await _notificationService
                    .SendAsync($"couldn't match edition '{bundleItem}'");
                continue;
            }

            var name = bundleItem.Replace(
                editionMatch.Groups[0].Value,
                string.Empty);

            var edition = editionMatch.Groups[1].Value;

            var hasTranslation = EditionTranslation.TryGetValue(
                edition.ToLowerInvariant(), out var translatedEdition);

            yield return new Book
            {
                Title = name,
                Edition = hasTranslation ? translatedEdition : edition,
                Source = bundle.Name
            };
        }
    }
}
