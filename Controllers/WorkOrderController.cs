//$ What does this model do?
    //& This file holds all of the methods for HTTP requests regarding work orders
    //& This model exists to define what methods and what data will be retrieved

using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using BiancasBikes.Data;
using BiancasBikes.Models;

//#"Because work orders are their own resource in our data model, they should get their own controller to contain the methods for getting those resources."
//^ Curriculum provided this entire model so we have a way to control the endpoints related to work orders
//# This controller, like the others, inherits from ControllerBase and has the ApiController attribute.
//# The route for this controller will be /api/workorder.
//# the WorkOrderController constructor is injecting an instance of the BiancasBikesDbContext class to use to access the database.
//# There is one endpoint in the class, GetIncompleteWorkOrders.
//# The query in the method uses OrderBy and ThenByDescending to order the work orders first by when they were created, so that the oldest appear first. Then they are further sorted by whether an employee has been assigned to them or not. If the work order does not have a UserProfileId, it will appear before one that does.
//# notice that we had to use Include twice for Bike. Once, to be able to call ThenInclude for Owner, and a second time to be able to call ThenInclude for BikeType.

namespace BiancasBikes.Controllers;

[ApiController]
[Route("api/[controller]")] //# this is an API controller attribute. "This attribute indicates that this controller handles requests for routes that start with "api/workorder,"
public class WorkOrderController : ControllerBase //# ControllerBase is the base class from which WorkOrderController inherits ... Where did ControllerBase come from? It "is a base class provided by ASP.NET Core for building controller classes in web applications, especially for building RESTful APIs using ASP.NET Core MVC.
{
    private BiancasBikesDbContext _dbContext; //? is this an instance of the database?

    public WorkOrderController(BiancasBikesDbContext context)
    {
        _dbContext = context; //* injecting "context" here gives us access to the database.
        //# Dependency Injection: In the constructor of WorkOrderController, you inject an instance of BiancasBikesDbContext (ie - the word context after the equals sign). This demonstrates dependency injection, a common practice in ASP.NET Core. Dependency injection allows you to access services (in this case, a database context) that your controller needs to perform its actions.
    }

    [HttpGet("incomplete")] //# decorate - This is an attirbute also, but it is a decoration for this action method. This tells the below GetIncompleteWorkOrders method to respond to HTTP GET requests with a url ending in /incomplete...the first part of the url is defined in the base route, and this "incomplete" is what we want at the end
    [Authorize] //!remember to uncomment when done testing
    //# this above is another attirbute that decorates the below action method. It " Requires that the client making the request must be authenticated and authorized to access this action method."
    public IActionResult GetIncompleteWorkOrders() //# This GetIncompleteWorkOrders is a method. "These action methods are responsible for handling incoming HTTP requests and returning HTTP responses. By inheriting from ControllerBase, you gain access to various helper methods and properties for managing these requests and responses."
    {
        return Ok(_dbContext.WorkOrders
        .Include(wo => wo.Bike) //* had to call (include) first time here to be able to call ThenInclude for Owner
        .ThenInclude(b => b.Owner)
        .Include(wo => wo.Bike) //* had to call (include) bike a second time here to call ThenInclude for BikeType
        .ThenInclude(b => b.BikeType)
        .Include(wo => wo.UserProfile)
        .Where(wo => wo.DateCompleted == null) //# after all properties of our work order objects are attached here for the HTTP response, we can filter only matching objects where there is no date completed...meaning we have a list of all objects, but then filtered through them with this .Where method to return only objects still in the garage
        .OrderBy(wo => wo.DateInitiated)
        .ThenByDescending(wo => wo.UserProfileId == null).ToList());
    }

//^ "This endpoint will map to a POST request with the url /api/workorder."
    [HttpPost]
    [Authorize]
    public IActionResult CreateWorkOrder(WorkOrder workOrder)
    {
        workOrder.DateInitiated = DateTime.Now;
        _dbContext.WorkOrders.Add(workOrder);
        _dbContext.SaveChanges();
        return Created($"/api/workorder/{workOrder.Id}", workOrder);
    }



//^ ENDPOINT - this endpoint is an update to an individual work order
[HttpPut("{id}")]
[Authorize]
public IActionResult UpdateWorkOrder(WorkOrder workOrder, int id)
{
    WorkOrder workOrderToUpdate = _dbContext.WorkOrders.SingleOrDefault(wo => wo.Id == id);
    if (workOrderToUpdate == null)
    {
        return NotFound();
    }
    else if (id != workOrder.Id)
    {
        return BadRequest();
    }

    //These are the only properties that we want to make editable
    workOrderToUpdate.Description = workOrder.Description;
    workOrderToUpdate.UserProfileId = workOrder.UserProfileId;
    workOrderToUpdate.BikeId = workOrder.BikeId;

    _dbContext.SaveChanges();

    return NoContent();
}

//^ ENDPOINT - this endpoint is used to instruct the server what to do when an http PUT request to the server comes in at ".../complete": COMPLETE an individual work order
//~ The meat of this code finds a single work order and changes its DateCompleted property to datetime.now, then saves that to the database
    //* "The role of this endpoint is to handle the PUT request when the user clicks the "Mark as Complete" button for a specific work order."
        //$ so when the "Mark as Complete" button is clicked and the http request is sent, this is where we tell the http PUT request the details of what exactly to update/PUT
            //! WHAT IS NOT HAPPENING: This is NOT navigating the user anywhere. This is not sending a request for another page. This is not updating the page. This is just a communication to the server to update the DateCompleted property of a specific work order within the work orders table. 
[HttpPut("{id}/complete")]
[Authorize]
public IActionResult CompleteWorkOrder(WorkOrder workOrder, int id) // this is the action method that handles incoming PUT requests.
{
    WorkOrder workOrderToComplete = _dbContext.WorkOrders.SingleOrDefault(wo => wo.Id == id); //find the work order by id
    if (workOrderToComplete == null) // if the provided work order id is not found/does not exist, return not found message
    {
        return NotFound();
    }
    else if (id != workOrderToComplete.Id) // if the id does not match the id we requested, return a bad request
    {
        return BadRequest();
    }
    workOrderToComplete.DateCompleted = DateTime.Now; //otherwise, update the date completed property to now

    _dbContext.SaveChanges();

    return NoContent();
}


//^ ENDPOINT = this endpoint is used to delete a work order from the list of work orders 
[HttpDelete("{id}")]
[Authorize]
public IActionResult DeleteWorkOrder(int id)
{
    // Find the work order by its ID
    WorkOrder workOrderToDelete = _dbContext.WorkOrders.SingleOrDefault(wo => wo.Id == id);
    
    if (workOrderToDelete == null)
    {
        // If the work order with the specified ID does not exist, return a "Not Found" response
        return NotFound();
    }
    
    // Remove the work order from the database
    _dbContext.WorkOrders.Remove(workOrderToDelete);
    _dbContext.SaveChanges();
    
    // Return a "No Content" response to indicate successful deletion
    return NoContent();
}




}