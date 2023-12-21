using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Function.Entities;
using Function.Exceptions;
using Function.Models.Books;
using Function.Repositories;
using Function.Utils;

namespace Function.Services;

public interface IBookService
{
    Task SaveBundleAsync(SaveBookRequest saveRequest);
    Task<IEnumerable<Book>> GetAllAsync();
}

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IMapper _mapper;

    public BookService(
        IBookRepository bookRepository,
        IMapper mapper)
    {
        _bookRepository = bookRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<Book>> GetAllAsync() =>
        await _bookRepository.GetAll();


    public async Task SaveBundleAsync(SaveBookRequest saveRequest)
    {
        if (saveRequest is null)
        {
            throw new ArgumentNullException(nameof(saveRequest));
        }

        if (saveRequest.BundleName is null)
        {
            throw new ArgumentNullException(nameof(saveRequest.BundleName));
        }

        if (saveRequest.Items is null)
        {
            throw new ArgumentNullException(nameof(saveRequest.Items));
        }

        foreach (var item in saveRequest.Items)
        {
            // parse name, edition
            var book = BookParser.FromBundle(saveRequest.BundleName, item);

            var request = _mapper.Map<CreateRequest>(book);

            await AddBookAsync(request);
        }
    }

    private async Task<Book> GetByIdAsync(string id)
    {
        Book book = await _bookRepository.GetById(id)
            ?? throw new KeyNotFoundException("book not found");
        return book;
    }

    private async Task AddBookAsync(CreateRequest model)
    {
        try
        {
            var foundBook = await _bookRepository.SearchByTitle(model.Title!);

            if (string.Compare(model.Edition, foundBook.Edition, StringComparison.OrdinalIgnoreCase) > 0)
            {
                await UpdateEditionAndSourceAsync(foundBook.Id!, new UpdateRequest
                {
                    Edition = model.Edition,
                    Source = model.Source
                });
            }

            return;
        }
        catch (AppException)
        {
            // book not found
        }

        // map model to new user object
        var book = _mapper.Map<Book>(model);

        // save book
        await _bookRepository.Create(book);
    }

    private async Task UpdateEditionAndSourceAsync(string id, UpdateRequest model)
    {
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
