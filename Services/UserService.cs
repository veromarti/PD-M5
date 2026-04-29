using SportComplex.Data;
using SportComplex.Models;
using SportComplex.Responses;

namespace SportComplex.Services;

public class UserService
{
    private readonly MySqlDbContext _context;

    public UserService(MySqlDbContext context)
    {
        _context = context;
    }

    public ServiceResponse<IEnumerable<User>> GetAllUsers()
    {
        var users = _context.users.ToList();
        return new ServiceResponse<IEnumerable<User>>() 
            { 
                Success = true, 
                Data = users 
            };
    }

    public ServiceResponse<User> GetUserById(int id)
    {
        var user = _context.users.FirstOrDefault(u => u.Id == id);
        if (user != null){
            return new ServiceResponse<User>() 
                { 
                    Success = true, 
                    Data = user, 
                    Message = "User found" 
                };
        }

        return new ServiceResponse<User>() 
            { 
                Success = false, 
                Data = null, 
                Message = "User not found" 
            };
    }

    public ServiceResponse<User> SaveUser(User user)
    {
        try
        {
            var documentExists = _context.users.FirstOrDefault(u => u.Document == user.Document);
            if (documentExists != null)
            {
                return new ServiceResponse<User>() 
                    { 
                        Success = false, 
                        Data = user, 
                        Message = "User already exists" 
                    };
            }

            var emailExists = _context.users.FirstOrDefault(u => u.Email == user.Email);
            if (emailExists != null)
            {
                return new ServiceResponse<User>() 
                    { 
                        Success = false, 
                        Data = user, 
                        Message = "User already exists" 
                    };
            }    

            _context.users.Add(user);
            _context.SaveChanges();
            return new ServiceResponse<User>() 
                { 
                    Success = true, 
                    Data = user, 
                    Message = "User created successfully" };
        }
        catch (Exception ex)
        {
            return new ServiceResponse<User>()
            {
                Success = false, 
                Data = user, 
                Message = $"User registration failed: {ex.Message}"
            };
        }
    }

    public ServiceResponse<User> UpdateUser(User user)
    {
        try
        {
            var userDb = _context.users.Find(user.Id);
            
            if (userDb == null){
                return new ServiceResponse<User>()
                {
                    Success = false, 
                    Data = null, 
                    Message = "User not found"
                };
            }

            var documentExists = _context.users.FirstOrDefault(u => u.Document == user.Document && u.Id != user.Id);
            if (documentExists != null){    
                return new ServiceResponse<User>()
                {
                    Success = false, 
                    Data = user, 
                    Message = "User not found"
                };
            }
            
            var emailExists = _context.users.FirstOrDefault(u => u.Email == user.Email && u.Id != user.Id);
            if (emailExists != null){
                return new ServiceResponse<User>()
                {
                    Success = false, 
                    Data = user, 
                    Message = "User already exists"
                };
            }

            userDb.Name = user.Name;
            userDb.Document = user.Document;
            userDb.Phone = user.Phone;
            userDb.Email = user.Email;
            _context.SaveChanges();
            return new ServiceResponse<User>()
            {
                Success = true, 
                Data = user, 
                Message = "User updated successfully"
            };
        }
        catch (Exception ex)
        {
            return new ServiceResponse<User>()
            {
                Success = false, 
                Data = user, 
                Message = $"User update failed: {ex.Message}"
            };
        }
    }

    public ServiceResponse<User> DeleteUser(User user)
    {
        try
        {
            var userDb = _context.users.Find(user.Id);
            if (userDb == null){
                return new ServiceResponse<User>()
                {
                    Success = false, 
                    Data = user, 
                    Message = "User not found"
                };
            }

            _context.users.Remove(userDb);
            _context.SaveChanges();
            return new ServiceResponse<User>()
            {
                Success = true, 
                Data = user, 
                Message = "User removed  successfully"
            };
        }
        catch (Exception ex)
        {
            return new ServiceResponse<User>()
            {
                Success = false, 
                Data = user, 
                Message = $"User deletion failed: {ex.Message}"
            };
        }
    }
}