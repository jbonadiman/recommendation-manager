using AutoMapper;
using RecommendationManager.Application.Models.Books;
using RecommendationManager.Domain;

namespace RecommendationManager.Application.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // SaveBundleRequest -> BookBundle
        CreateMap<SaveBundleRequest, BookBundle>();

        // Book -> CreateRequest
        CreateMap<Book, CreateRequest>();

        // CreateRequest -> Book
        CreateMap<CreateRequest, Book>()
            .AfterMap((_, dest) => dest.Id = Guid.NewGuid().ToString());

        // UpdateRequest -> Book
        CreateMap<UpdateRequest, Book>()
            .ForAllMembers(x =>
                x.Condition(
                    (_, _, prop) =>
                    {
                        // ignore both null & empty string properties
                        if (prop is null)
                        {
                            return false;
                        }

                        return prop.GetType() != typeof(string) ||
                               !string.IsNullOrEmpty((string)prop);
                    }
                ));
    }
}
