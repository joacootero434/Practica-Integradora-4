using Microsoft.AspNetCore.Mvc;
using ProductosApi.Exceptions;
using ProductosApi.Models.DTOs.Requests;
using ProductosApi.Services.Interfaces;

namespace ProductosApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;

    public ProductsController(IProductService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_service.GetAllProducts());
    }

    [HttpGet("search")]
    public IActionResult Search([FromQuery] string name)
    {
        // Sin coincidencias: 200 con lista vacía, no 404.
        return Ok(_service.SearchProductsByName(name ?? string.Empty));
    }

    [HttpGet("stats")]
    public IActionResult GetStats()
    {
        return Ok(_service.GetStats());
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var product = _service.GetProductById(id);
        if (product == null)
            return NotFound();

        return Ok(product);
    }

    [HttpPost]
    public IActionResult Create([FromBody] ProductForCreateDto dto)
    {
        try
        {
            var created = _service.CreateProduct(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (DuplicateProductException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] ProductForUpdateDto dto)
    {
        if (_service.GetProductById(id) == null)
            return NotFound();

        try
        {
            _service.UpdateProduct(id, dto);
            return NoContent();
        }
        catch (DuplicateProductException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        if (_service.GetProductById(id) == null)
            return NotFound();

        _service.DeleteProduct(id);
        return NoContent();
    }
}