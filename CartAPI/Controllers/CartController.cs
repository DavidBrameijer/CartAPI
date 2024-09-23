using CartAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace CartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private static List<CartItem> items = new List<CartItem>()
        {
            new CartItem() { Id=1, Product="Chips", Price=3.00, Quantity=10 },
            new CartItem() { Id=2, Product="Candy", Price=4.00, Quantity=20 },
            new CartItem() { Id=3, Product="Soda", Price=2.50, Quantity=15 },
            new CartItem() { Id=4, Product="Pizza", Price=10.00, Quantity=5 }
        };
        private static int nextId = 5;

//        $ curl -X 'GET' \
//  'https://localhost:7286/api/Cart?prefix=c&pageSize=1&page=2' \
//  -H 'accept: */*'
//  % Total    % Received % Xferd Average Speed Time    Time Time  Current
//                                 Dload  Upload Total   Spent Left  Speed
//100    52    0    52    0     0   2413      0 --:--:-- --:--:-- --:--:--  2476[{"id":2,"product":"Candy","price":4,"quantity":20}]


        [HttpGet()]
        public IActionResult GetAll(double? maxPrice, string? prefix, int? pageSize, int? page)
        {
            List<CartItem> result = items;

            if (maxPrice != null)
            {
                result = result.Where(x => x.Price <= maxPrice).ToList();
            }

            if (prefix != null)
            {
                result = result.Where(x => x.Product.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (pageSize != null && page != null)
            {
                result = result.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value).ToList();
            }
            else if (pageSize != null)
            {
                result = result.Take(pageSize.Value).ToList();
            }

            return Ok(result);
        }


//        $ curl -X 'GET' \
//  'https://localhost:7286/api/Cart/3' \
//  -H 'accept: */*'
//  % Total    % Received % Xferd Average Speed Time    Time Time  Current
//                                 Dload  Upload Total   Spent Left  Speed
//100    51    0    51    0     0   2419      0 --:--:-- --:--:-- --:--:--  2550{"id":3,"product":"Soda","price":2.5,"quantity":15}

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            CartItem c = items.FirstOrDefault(i => i.Id == id);
            //not found
            if(c == null)
            {
                return NotFound();
            }
            //match
            return Ok(c);
        }

//        $ curl -X 'POST' \
//  'https://localhost:7286/api/Cart' \
//  -H 'accept: */*' \
//  -H 'Content-Type: application/json' \
//  -d '{
//  "id": 0,
//  "product": "Burger",
//  "price": 12.00,
//  "quantity": 7
//}'
//  % Total    % Received % Xferd Average Speed Time    Time Time  Current
//                                 Dload  Upload Total   Spent Left  Speed
//100   122    0    51  100    71   1871   2605 --:--:-- --:--:-- --:--:--  4518{"id":6,"product":"Burger","price":12,"quantity":7}

        [HttpPost()]
        public IActionResult AddCartItem([FromBody]CartItem newCartItem)
        {
            newCartItem.Id = nextId;
            items.Add(newCartItem);
            nextId++;
            return Created($"/api/CartItem/{newCartItem.Id}", newCartItem);
        }

//        $ curl -X 'PUT' \
//  'https://localhost:7286/api/Cart/3' \
//  -H 'accept: */*' \
//  -H 'Content-Type: application/json' \
//  -d '{
//  "id": 3,
//  "product": "Coffee",
//  "price": 3.00,
//  "quantity": 50
//}'
//  % Total    % Received % Xferd Average Speed Time    Time Time  Current
//                                 Dload  Upload Total   Spent Left  Speed
//100   122    0    51  100    71   2661   3705 --:--:-- --:--:-- --:--:--  6421{"id":3,"product":"Coffee","price":3,"quantity":50}

        [HttpPut("{id}")]
        public IActionResult UpdateCartItem(int id, [FromBody]CartItem updatedCartItem)
        {
            if(id != updatedCartItem.Id)
            {
                return BadRequest();
            }
            if(!items.Any(x => x.Id == id))
            {
                return NotFound();
            }
            int index = items.FindIndex(x => x.Id == id);
            items[index] = updatedCartItem;
            return Ok(updatedCartItem);
        }

//$ curl -X 'DELETE' \
//  'https://localhost:7286/api/Cart/1' \
//  -H 'accept: */*'
//  % Total    % Received % Xferd Average Speed Time    Time Time  Current
//                                 Dload  Upload Total   Spent Left  Speed
//  0     0    0     0    0     0      0      0 --:--:-- --:--:-- --:--:--     0


        [HttpDelete("{id}")]
        public IActionResult DeleteById(int id)
        {
            int index = items.FindIndex(x => x.Id == id);
            if(index == -1)
            {
                return NotFound("That item was not found.");
            }
            else
            {
                items.RemoveAt(index);
                return NoContent();
            }
        }
    }
}
