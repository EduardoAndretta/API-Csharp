using System;

namespace Catalog.Entities
{
    //////////////////////////////////////////////////////////////////////////////////////////|
    //|
    //| * Item record
    //|
    //| This resource make a class immutable and readonly. Also, enable others methods for use
    //| with this object and enable the 'with' expression for create objects more easy.
    //|
    //|
    //| * Init-only properties - This resource allow us to assing values in camps or propeties 
    //| in the creation moment. Constructor in not necessary.
    //|
    //| var customer = new Customer("Antônio","antonioandretta@outlook.com");
    //|
    //| customer.Name = "Eduardo" (This return a error. customer are initialized because init propieties) 
    //| 'for edit this object, you need to create a new Customer'
    //|

    public record Item
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public decimal Price { get; init; }
        public DateTimeOffset CreatedDate { get; init; }
    }
}
