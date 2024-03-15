using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarAdApp.Data;
using CarAdApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CarAdApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CarsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Cars
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Car>>> GetCars()
        {
            return await _context.Cars.ToListAsync();
        }

        // GET: api/Cars/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Car>> GetCar(int id)
        {
            var car = await _context.Cars.FindAsync(id);

            if (car == null)
            {
                return NotFound();
            }

            return car;
        }

        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<Car>>> FilterCars(
            [FromQuery] string? brand = null, 
            [FromQuery] string? model = null, 
            [FromQuery] double? minEngineVolume = null,
            [FromQuery] double? maxEngineVolume = null, 
            [FromQuery] int? minMileage = null,
            [FromQuery] int? maxMileage = null, 
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null, 
            [FromQuery] string? color = null,
            [FromQuery] BanType? banType = null, 
            [FromQuery] FuelType? fuelType = null,
            [FromQuery] Gearbox? gearbox = null,
            [FromQuery] int? minYear = null,
            [FromQuery] int? maxYear = null)
        {
            var query = _context.Cars.AsQueryable();

            if (!string.IsNullOrEmpty(brand))
            {
                query = query.Where(c => c.Brand == brand);
            }
            if (!string.IsNullOrEmpty(model))
            {
                query = query.Where(c => c.Model == model);
            }
            if (minEngineVolume.HasValue)
            {
                query = query.Where(c => c.EngineVolume >= minEngineVolume.Value);
            }
            if (maxEngineVolume.HasValue)
            {
                query = query.Where(c => c.EngineVolume <= maxEngineVolume.Value);
            }
            if (minMileage.HasValue)
            {
                query = query.Where(c => c.Mileage >= minMileage.Value);
            }
            if (maxMileage.HasValue)
            {
                query = query.Where(c => c.Mileage <= maxMileage.Value);
            }
            if (minPrice.HasValue)
            {
                query = query.Where(c => c.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(c => c.Price <= maxPrice.Value);
            }
            if (!string.IsNullOrEmpty(color))
            {
                query = query.Where(c => c.Color == color);
            }
            if (banType.HasValue)
            {
                query = query.Where(c => c.BanType == banType.Value);
            }
            if (fuelType.HasValue)
            {
                query = query.Where(c => c.FuelType == fuelType.Value);
            }
            if (gearbox.HasValue)
            {
                query = query.Where(c => c.Gearbox == gearbox.Value);
            }
            if (minYear.HasValue)
            {
                query = query.Where(c => c.ProductionYear >= minYear.Value);
            }
            if (maxYear.HasValue)
            {
                query = query.Where(c => c.ProductionYear <= maxYear.Value);
            }

            List<Car> cars = await query.ToListAsync();
            return cars;
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCar(int id, CarUpdateDto carUpdate)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (car.UserId != userId)
            {
                return Forbid();
            }

            UpdateCarFields(car, carUpdate);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CarExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        // POST: api/Cars
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Car>> PostCar([FromBody] Car car)
        {
            _context.Cars.Add(car);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCar), new { id = car.CarId }, car);
        }

        private bool CarExists(int id)
        {
            return _context.Cars.Any(e => e.CarId == id);
        }
        
        private void UpdateCarFields(Car car, CarUpdateDto carUpdate)
        {
            if (carUpdate.Brand != null) car.Brand = carUpdate.Brand;
            if (carUpdate.Model != null) car.Model = carUpdate.Model;
            if (carUpdate.ProductionYear.HasValue) car.ProductionYear = carUpdate.ProductionYear.Value;
            if (carUpdate.EngineVolume.HasValue) car.EngineVolume = carUpdate.EngineVolume.Value;
            if (carUpdate.Mileage.HasValue) car.Mileage = carUpdate.Mileage.Value;
            if (carUpdate.Price.HasValue) car.Price = carUpdate.Price.Value;
            if (carUpdate.Color != null) car.Color = carUpdate.Color;
            if (carUpdate.Gearbox.HasValue) car.Gearbox = carUpdate.Gearbox.Value;
            if (carUpdate.BanType.HasValue) car.BanType = carUpdate.BanType.Value;
            if (carUpdate.FuelType.HasValue) car.FuelType = carUpdate.FuelType.Value;
            if (carUpdate.PictureUrl != null) car.PictureUrl = carUpdate.PictureUrl;
        }
    }
}
