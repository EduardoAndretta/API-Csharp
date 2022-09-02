using System;

namespace Catalog.Dtos
{
    //////////////////////////////////////////////////////////////////////////////////////////|
    //|
    //| DTO (Data transfer object)
    //|
    //| DTO is an object that carries data between preocesses.
    //|
    //| In short, the DTO is used for make the code more clean and organized. Also, the
    //| performace get better even have thousand requests in the API in same time.

    public record ItemDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public decimal Price { get; init; }
        public DateTimeOffset CreatedDate { get; init; }
    }
}