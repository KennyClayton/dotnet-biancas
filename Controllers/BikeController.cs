using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BiancasBikes.Data;
using Microsoft.EntityFrameworkCore;
using BiancasBikes.Models;


namespace BiancasBikes.Controllers;

[ApiController]
[Route("api/[controller]")]
//# above is a route attribute on the controller. This tells the framework what route segment should be associated with all of the endpoints in the controller. In this case, "api/[controller]" tells the framework to use the first part of the controller name ("Bike") to create the route. So all of the endpoints in this controller will have URLs that start with "/api/bike" (it is case insensitive)
public class BikeController : ControllerBase //? BikeController inherits from ControllerBase I think
{
    private BiancasBikesDbContext _dbContext;

    public BikeController(BiancasBikesDbContext context)
    {
        _dbContext = context;
    }

    [HttpGet] //# This is a decoration. This is a HttpGet attribute to mark it as a GET endpoint, but is technically unnecessary as GET is the default.
    //[Authorize] //# we can comment this out to test postman without having to log in
    public IActionResult Get() //# this Get method is an endpoint to get all bikes
    {
        return Ok(_dbContext.Bikes.Include(b => b.Owner).ToList()); //# The Ok method that gets called inside Get will create an HTTP response with a status of 200, as well as the data that's passed in.
                                                                    //above, we updated the reference to dbCOntext.Bikes to include the owner. We did this by dot notating the include method and specifying where bike owner is found 
    }
    
    //^ Added this endpoint below to get bikes by id
    [HttpGet("{id}")]
    [Authorize]
    public IActionResult GetById(int id)
    {
        Bike bike = _dbContext
            .Bikes
            .Include(b => b.Owner)
            .Include(b => b.BikeType)
            .Include(b => b.WorkOrders)
            .SingleOrDefault(b => b.Id == id);

        if (bike == null)
        {
            return NotFound();
        }

        return Ok(bike);
    }
    
    //^ Added this endpoint below to get the count/total number of bikes in the garage
    [HttpGet("inventory")]
    [Authorize]
    public IActionResult Inventory()
    {
        int inventory = _dbContext
        .Bikes
        .Where(b => b.WorkOrders.Any(wo => wo.DateCompleted == null))
        .Count();

        return Ok(inventory);
    }
}

//# What is a controller? "a controller is a class". " A controller class contains methods that are the handlers for the endpoints of the API. In web APIs the controllers inherit many of the properties and methods we will use from the ControllerBase class."
//&---------------------------------------------------
//# attributes - This controller has a number of attributes on it and on the methods
//&---------------------------------------------------
//# decorate - all controllers get decorated with the ApiController attribute AND inherit ControllerBase
//&---------------------------------------------------
//# inherit - all controllers inherit ControllerBase
//&---------------------------------------------------
//# A controller contains all of the endpoints for a specific resource. In this case, all endpoints related to the Bikes resource. Usually you will have one controller for each resource available in the API.
//&---------------------------------------------------
//# attributes - The BikeController class also has a Route attribute. This tells the framework what route segment should be associated with all of the endpoints in the controller. In this case, "api/[controller]" tells the framework to use the first part of the controller name ("Bike") to create the route. So all of the endpoints in this controller will have URLs that start with "/api/bike" (it is case insensitive)
//&---------------------------------------------------
//# Finally, the Get method is an endpoint. The Ok method that gets called inside Get will create an HTTP response with a status of 200, as well as the data that's passed in. It is decorated with the HttpGet attribute to mark it as a GET endpoint, but is technically unnecessary as GET is the default. Authorize will be covered in the next chapter.
//&---------------------------------------------------
//# "The Ids for the Identity Framework tables are Guids, not ints. A Guid (Global Unique Identifier) can be generated with Guid.NewGuid(). You will need to do this when you create your own data to seed."