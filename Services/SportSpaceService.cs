using Microsoft.EntityFrameworkCore;
using SportComplex.Data;
using SportComplex.Models;
using SportComplex.Responses;

namespace SportComplex.Services;

public class SportSpaceService
{
    private readonly MySqlDbContext _context;

    public SportSpaceService(MySqlDbContext context)
    {
        _context = context;
    }

    public ServiceResponse<IEnumerable<SportSpace>> GetAllSpaces()
    {
        var spaces = _context.sport_spaces.ToList();
        return new ServiceResponse<IEnumerable<SportSpace>>() 
            { 
                Success = true, 
                Data = spaces 
            };
    }

    public ServiceResponse<IEnumerable<SportSpace>> GetSpacesByType(string type)
    {
        var spaces = _context.sport_spaces
            .Where(s => s.Type == type)
            .ToList();
        return new ServiceResponse<IEnumerable<SportSpace>>()
        {
            Success = true, 
            Data = spaces
        };
    }

    public ServiceResponse<SportSpace> GetSpaceById(int id)
    {
        var space = _context.sport_spaces.FirstOrDefault(s => s.Id == id);
        if (space != null)
        {
            return new ServiceResponse<SportSpace>()
            {
                Success = true, 
                Data = space, 
                Message = "Sport Complex found"
            };
        }
        return new ServiceResponse<SportSpace>()
        {
            Success = false, 
            Data = null, 
            Message = "Sport Complex not found"
        };
    }

    public ServiceResponse<SportSpace> SaveSpace(SportSpace space)
    {
        try
        {
            var nameExists = _context.sport_spaces.FirstOrDefault(s => s.Name == space.Name);
            if (nameExists != null){
                return new ServiceResponse<SportSpace>()
                {
                    Success = false, 
                    Data = space, 
                    Message = "Sport Complex already exists"
                };
            }
            _context.sport_spaces.Add(space);
            _context.SaveChanges();
            return new ServiceResponse<SportSpace>() 
                { 
                    Success = true, 
                    Data = space, 
                    Message = "Sport Complex created successfully" };
        }
        catch (Exception ex)
        {
            return new ServiceResponse<SportSpace>()
            {
                Success = false, 
                Data = space, 
                Message = $"Sport Complex registration failed: {ex.Message}"
            };
        }
    }

    public ServiceResponse<SportSpace> UpdateSpace(SportSpace space)
    {
        try
        {
            var spaceDb = _context.sport_spaces.Find(space.Id);
            if (spaceDb == null){
                return new ServiceResponse<SportSpace>()
                {
                    Success = false, 
                    Data = space, 
                    Message = "Sport Complex not found"
                };
            }

            var nameExists = _context.sport_spaces.FirstOrDefault(s => s.Name == space.Name && s.Id != space.Id);
            if (nameExists != null){
                return new ServiceResponse<SportSpace>()
                {
                    Success = false, 
                    Data = space, 
                    Message = "Sport Complex already exists"
                };
            }

            spaceDb.Name = space.Name;
            spaceDb.Type = space.Type;
            spaceDb.Capacity = space.Capacity;
            _context.SaveChanges();
            return new ServiceResponse<SportSpace>()
            {
                Success = true, 
                Data = space, 
                Message = "Sport Complex updated successfully" 
            };
        }
        catch (Exception ex)
        {
            return new ServiceResponse<SportSpace>()
            {
                Success = false, 
                Data = space, 
                Message = $"Sport Complex update failed: {ex.Message}"
            };
        }
    }

    public ServiceResponse<SportSpace> DeleteSpace(SportSpace space)
    {
        try
        {
            var spaceDb = _context.sport_spaces.Find(space.Id);
            if (spaceDb == null)
            {
                return new ServiceResponse<SportSpace>()
                {
                    Success = false, 
                    Data = space, 
                    Message = "Sport Complex not found"
                };
            }

            _context.sport_spaces.Remove(spaceDb);
            _context.SaveChanges();
            return new ServiceResponse<SportSpace>()
            {
                Success = true, 
                Data = space, 
                Message = "Sport Complex removed successfully"
            };
        }
        catch (Exception ex)
        {
            return new ServiceResponse<SportSpace>()
            {
                Success = false, 
                Data = space, 
                Message = $"Sport complex deletion failed: {ex.Message}"
            };
        }
    }

    public List<string> GetSpaceTypes()
    {
        return _context.sport_spaces
            .Where(s => s.Type != null)
            .Select(s => s.Type!)
            .Distinct()
            .OrderBy(t => t)
            .ToList();
    }
    
    public ServiceResponse<IEnumerable<SportSpace>> GetFilteredSpaces(string? type)
    {
        return string.IsNullOrEmpty(type)
            ? GetAllSpaces()
            : GetSpacesByType(type);
    }
}