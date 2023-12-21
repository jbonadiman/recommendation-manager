using System.Text.Json;
using AutoMapper;
using Microsoft.Extensions.Logging;
using RecommendationManager.Application.Exceptions;
using RecommendationManager.Application.Interfaces;
using RecommendationManager.Application.Models.Books;
using RecommendationManager.Domain;

namespace RecommendationManager.Application.Services;

public class BookService : IBookService
{
    private readonly ILogger<BookService> _logger;
    private readonly IBookRepository _bookRepository;
    private readonly IMapper _mapper;
    private readonly IBookParser _bookParser;

    public BookService(
        IBookRepository bookRepository,
        IBookParser bookParser,
        IMapper mapper,
        ILogger<BookService> logger)
    {
        _bookRepository = bookRepository;
        _bookParser = bookParser;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<Book>> GetAllAsync()
    {
        return await _bookRepository.GetAll();
    }


    public async Task SaveBundleAsync(SaveBundleRequest saveRequest)
    {
        _logger.LogDebug(
            "received save request: '{Request}'",
            JsonSerializer.Serialize(saveRequest));

        var mappedBundle = _mapper.Map<BookBundle>(saveRequest);
        _logger.LogDebug(
            "mapped '{Type}' to '{OtherType}'",
            saveRequest.GetType().Name,
            mappedBundle.GetType().Name);

        var createRequests = _bookParser
            .ParseBundle(mappedBundle)
            .Select(_mapper.Map<CreateRequest>);
        _logger.LogDebug("parser generator ready, processing books...");

        await foreach (var request in createRequests)
        {
            await AddBookAsync(request);
        }

        _logger.LogInformation("processed '{Count}' bundle items",
            saveRequest.Items.Count());
    }

    private async Task AddBookAsync(CreateRequest model)
    {
        try
        {
            _logger.LogDebug(
                "searching for book '{Title}' in the database",
                model.Title);
            var foundBook = await _bookRepository.SearchByTitle(model.Title!);

            _logger.LogDebug(
                "book '{Title}' already tracked",
                model.Title);

            if (string.Compare(
                    model.Edition,
                    foundBook.Edition,
                    StringComparison.OrdinalIgnoreCase) > 0) // edition is newer
            {
                _logger.LogDebug(
                    "book's edition is newer, updating info...");

                await UpdateEditionAndSourceAsync(
                    foundBook.Id!, new UpdateRequest
                    {
                        Edition = model.Edition,
                        Source = model.Source
                    });
            }

            return;
        }
        catch (AppException)
        {
            _logger.LogDebug(
                "couldn't find book '{}' in the database, adding...",
                model.Title);
            // book not found
        }

        // map model to new user object
        var book = _mapper.Map<Book>(model);

        // save book
        await _bookRepository.Create(book);
    }

    private async Task UpdateEditionAndSourceAsync(string id, UpdateRequest model)
    {
        _logger.LogDebug(
            "retrieving book with id '{Id}' from the database",
            id);
        var book = await _bookRepository.GetById(id);

        // copy model props to user
        _mapper.Map(model, book);

        // save book
        await _bookRepository.Update(book);
    }

    private async Task DeleteAsync(string id)
    {
        await _bookRepository.Delete(id);
    }
}
