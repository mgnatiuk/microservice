using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Extensions;
using Play.Catalog.Service.Entities;
using Play.Common;

namespace Play.Catalog.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<Item> _itemsRepository;
        //private static int requestCounter = 0;

        public ItemsController(IRepository<Item> itemsRepository)
        {
            _itemsRepository = itemsRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetAsync()
        {

            // requestCounter++;

            // Console.WriteLine($"Request {requestCounter}: Starting...");

            // if (requestCounter <= 2)
            // {
            //     Console.WriteLine($"Request {requestCounter}: Delaying...");
            //     await Task.Delay(TimeSpan.FromSeconds(10));
            // }

            // if (requestCounter <= 4)
            // {
            //     Console.WriteLine($"Request {requestCounter}: 500 (Internal Server Error).");
            //     return StatusCode(500);
            // }

            // Console.WriteLine($"Request {requestCounter}: 200 (OK).");

            var items = (await _itemsRepository.GetAllAsync())
                .Select(x => x.AsDto());

            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id)
        {
            var item = await _itemsRepository.GetAsync(id);

            if (item is null)
                return NotFound();

            return item.AsDto();
        }

        [HttpPost]
        public async Task<ActionResult<ItemDto>> PostAsync([FromBody] CreateItemDto dto)
        {
            var item = new Item
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await _itemsRepository.CreateAsync(item);

            return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> PutAsync(Guid id, [FromBody] UpdateItemDto dto)
        {
            var existingItem = await _itemsRepository.GetAsync(id);

            if (existingItem is null)
                return NotFound();

            existingItem.Name = dto.Name;
            existingItem.Description = dto.Description;
            existingItem.Price = dto.Price;

            await _itemsRepository.UpdateAsync(existingItem);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var item = await _itemsRepository.GetAsync(id);

            if (item is null)
                return NotFound();

            await _itemsRepository.RemoveAsync(item.Id);

            return NoContent();
        }
    }
}