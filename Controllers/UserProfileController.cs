

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BiancasBikes.Data;

namespace BiancasBikes.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserProfileController : ControllerBase
{
    private BiancasBikesDbContext _dbContext;

    public UserProfileController(BiancasBikesDbContext context)
    {
        _dbContext = context;
    }

//# "This method from the UserProfileController gets the data for all users. This data should only be available to Admin users (it will be used to terminate employees, hire new ones, as well as upgrading an employee to be an Admin). [Authorize(Roles = "Admin")] ensures that this resource will only be accessible to authenticated users that also have the Admin role associated with their user id. A logged in user that is not an Admin will receive a 403 (Forbidden) response when trying to access this resource."

    [HttpGet]
    [Authorize]
    public IActionResult Get()
    {
        return Ok(_dbContext.UserProfiles.ToList());
    }
}