using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace SimpleService.Controllers;

[ApiController]
[Route("/")]
public class EntitiesController : ControllerBase
{
    private readonly ILogger<EntitiesController> _logger;
    private readonly IMemoryCache _cache;
    private readonly IConfiguration _config;

    public EntitiesController(IMemoryCache cache, IConfiguration config, ILogger<EntitiesController> logger)
    {
        _logger = logger;
        _cache = cache;
        _config = config;
    }

    [HttpPost("insert")]
    public ActionResult Insert(Entity entity)
    {
        _logger?.LogInformation($"[{Request.Method}] {Request.GetDisplayUrl()}");

        if (entity == null)
            return BadRequest("Input object is null.");

        if (entity.Id == Guid.Empty)
            return BadRequest("Input object's id is empty.");

        if (_cache.TryGetValue(entity.Id, out _))
            return BadRequest("An object with the same id already exists.");

        var expirationMinutes = _config.GetValue<double>("Storage:ExpirationMins", 5);

        var expiration = expirationMinutes <= 0
            ? DateTimeOffset.MaxValue
            : DateTimeOffset.Now.AddMinutes(expirationMinutes);

        _ = _cache.Set(entity.Id, entity, new MemoryCacheEntryOptions()
        { 
            AbsoluteExpiration = expiration,
        });

        return Ok();
    }

    [HttpGet("get")]
    public ActionResult<Entity> Get(Guid id)
    {
        _logger?.LogInformation($"[{Request.Method}] {Request.GetDisplayUrl()} body {id}");

        if (id == Guid.Empty)
            return BadRequest("Input object's id is empty.");
        
        if (!_cache.TryGetValue(id, out var entity))
            return NotFound();

        return new ObjectResult(entity);
    }
}