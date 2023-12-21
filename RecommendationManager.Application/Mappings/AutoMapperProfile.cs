
using System;
using AutoMapper;
using Function.Entities;
using Function.Models.Books;

namespace Function.Utils;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // Book -> CreateRequest
        CreateMap<Book, CreateRequest>();

        // CreateRequest -> Book
        CreateMap<CreateRequest, Book>()
            .AfterMap((_, dest) => dest.Id = Guid.NewGuid().ToString());

        // UpdateRequest -> Book
        CreateMap<UpdateRequest, Book>()
            .ForAllMembers(x => x.Condition(
                (src, dest, prop) =>
                {
                    // ignore both null & empty string properties
                    if (prop is null) return false;
                    if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;

                    return true;
                }
            ));
    }
}
