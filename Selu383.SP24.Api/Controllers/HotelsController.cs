using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Selu383.SP24.Api.Data;
using Selu383.SP24.Api.Features.Hotels;
using Selu383.SP24.Api.Features;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Security.Principal;

namespace Selu383.SP24.Api.Controllers;

[Route("api/hotels")]
[ApiController]
public class HotelsController : ControllerBase
{
    private readonly DbSet<Hotel> hotels;
    private readonly DataContext dataContext;

    public HotelsController(DataContext dataContext)
    {
        this.dataContext = dataContext;
        hotels = dataContext.Set<Hotel>();
    }

    [HttpGet]
    public IQueryable<HotelDto> GetAllHotels()
    {
        return GetHotelDtos(hotels);
    }

    [HttpGet]
    [Route("{id}")]
    public ActionResult<HotelDto> GetHotelById(int id)
    {
        var result = GetHotelDtos(hotels.Where(x => x.Id == id)).FirstOrDefault();
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public ActionResult<HotelDto> CreateHotel(HotelDto dto)
    {
        if (IsInvalid(dto))
        {
            return BadRequest();
        }

        var hotel = new Hotel
        {
            Name = dto.Name,
            Address = dto.Address,
            Manager = dataContext.Users.Find(dto.ManagerId)

        };

        hotels.Add(hotel);

        dataContext.SaveChanges();

        dto.Id = hotel.Id;



        return CreatedAtAction(nameof(GetHotelById), new { id = dto.Id }, dto);
    }

    public static string GetUserId(IPrincipal user)
    {
        var claim = ((ClaimsIdentity)user.Identity).FindFirst(ClaimTypes.NameIdentifier);
        return claim == null ? null : claim.Value;
    }

    public static string GetUserRoles(IPrincipal user)
    {
        var claim = ((ClaimsIdentity)user.Identity).FindFirst(ClaimTypes.Role);
        return claim == null ? null : claim.Value;
    }

    [HttpPut]
    [Route("{id}")]
    [Authorize]
    public ActionResult<HotelDto> UpdateHotel(int id, HotelDto dto)
    {
        if (IsInvalid(dto))
        {
            return BadRequest();
        }

        var hotel = hotels.FirstOrDefault(x => x.Id == id);
        if (hotel == null)
        {
            return NotFound();
        }


        if (!(dto.ManagerId.ToString() == GetUserId(User)) && !(GetUserRoles(User) == "Admin"))
        {
            return Forbid();
        }

        hotel.Name = dto.Name;
        hotel.Address = dto.Address;
        hotel.Manager = dataContext.Users.Find(dto.ManagerId);

        dataContext.SaveChanges();

        dto.Id = hotel.Id;
        dto.Name = hotel.Name;
        dto.Address = hotel.Address;
        dto.ManagerId = hotel.Manager.Id;

        return Ok(dto);
    }

    [HttpDelete]
    [Route("{id}")]
    [Authorize(Roles = "Admin")]
    public ActionResult DeleteHotel(int id)
    {
        var hotel = hotels.FirstOrDefault(x => x.Id == id);
        if (hotel == null)
        {
            return NotFound();
        }

        hotels.Remove(hotel);

        dataContext.SaveChanges();

        return Ok();
    }


    private static bool IsInvalid(HotelDto dto)
    {
        return string.IsNullOrWhiteSpace(dto.Name) ||
               dto.Name.Length > 120 ||
               string.IsNullOrWhiteSpace(dto.Address);
    }

    private static IQueryable<HotelDto> GetHotelDtos(IQueryable<Hotel> hotels)
    {
        return hotels
            .Select(x => new HotelDto
            {
                Id = x.Id,
                Name = x.Name,
                Address = x.Address,
                ManagerId = x.Manager.Id
            });
    }
}