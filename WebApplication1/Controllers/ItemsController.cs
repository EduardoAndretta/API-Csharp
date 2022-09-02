using Catalog.Dtos;
using Catalog.Entities;
using Catalog.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.Controllers
{
    [ApiController]
    [Route("items")] //Route of controller
    public class ItemsController : ControllerBase
    {
        private readonly IItemsRepository repository;

        public ItemsController(IItemsRepository repository)
        {
            this.repository = repository;
        }
   
        //////////////////////////////////////////////////////////////////////////////////////////|
        //|
        //| IEnumerable -  Is similar to ForEach. (´responsible for list the items´)
        //|
        //| Task -> Void (In this case, the Task is used for allows the assincronisn of methods)
        //|
        //| The method .AsDto() are responsible for change the original item to DtoItem.
        //|
        //| Await to Async - Assincronisn of methods.
        //|

        // GET /items
        [HttpGet]
        public async Task<IEnumerable<ItemDto>> GetItemsAsync()
        {
            var items = (await repository.GetItemsAsync())
                        .Select(item => item.AsDto());

            return items;
        }

        //////////////////////////////////////////////////////////////////////////////////////////|
        //|
        //| The parameter of GetItemAsync in the underneath method as get in Link (/{id}).
        //|
        //| If not exists the requested id, the API return 'NotFound 404'
        //| 
        //| All items are returned in Dto format (Similar to original Item)
        //|

        // GET /items/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetItemAsync(Guid id)
        {
            var item = await repository.GetItemAsync(id);

            if (item is null)
            {
                return NotFound();
            }

            return item.AsDto();
        }

        //////////////////////////////////////////////////////////////////////////////////////////|
        //|
        //| The parameters of CreateItemAsync in the underneath method as get in JSON text format.
        //| (Postman / Insomnia)

        // POST /items
        [HttpPost]
        public async Task<ActionResult<ItemDto>> CreateItemAsync(CreateItemDto itemDto)
        {
            Item item = new()
            {
                Id = Guid.NewGuid(),
                Name = itemDto.Name,
                Price = itemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await repository.CreateItemAsync(item);
            
            return CreatedAtAction(nameof(GetItemAsync), new { id = item.Id }, item.AsDto());
        }

        //////////////////////////////////////////////////////////////////////////////////////////|
        //|
        //| The difference of PUT and PATCH HTTP methods
        //|
        //| * PUT is a method of modifying resource where the client sends data that updates the
        //| entire resource. (Complete Change of resource)
        //|
        //| * PATCH is a method of modifying resources where the client sends partial data that is
        //| to be updated without modifying the entire data. (Partial change of resource)
        //|


        // PUT /items/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateItemAsync(Guid id, UpdateItemDto itemDto)
        {
            var existingItem = await repository.GetItemAsync(id);

            if(existingItem is null)
            {
                return NotFound();
            }

            Item updatedItem = existingItem with
            {
                Name = itemDto.Name,
                Price = itemDto.Price
            };

            await repository .UpdateItemAsync(updatedItem);

            return NoContent();
        }

        // DELETE /Items/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteItemAsync(Guid id)
        {
            var existingItem = await repository.GetItemAsync(id);

            if (existingItem is null)
            {
                return NotFound();
            }

            await repository.DeleteItemAsync(id);

            return NoContent();
        }
    }
}