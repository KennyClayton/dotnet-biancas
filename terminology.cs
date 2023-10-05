//~ Authentication - proving you are who you say you are.
    //authenticated or not to get into the building, stadium, house, etc

//~ Authorization - proving you are allowed to do what you're trying to do.
    //authorized or not to do this thing inside the building, stadium, house, etc
    //authenticated into the stadium, unauthorized to play in the game because you are not on the roster

//~ What is a controller? "a controller is a class". " A controller class contains methods that are the handlers for the endpoints of the API. In web APIs the controllers inherit many of the properties and methods we will use from the ControllerBase class."

//~ Depency injection - 

//~ Constructor - "A constructor is a method-like member of a class that provides extra logic for setting up the class to be ready to use. Constructors always have the same name as the class, are public, and don't have a return type." example:
// public class Dog
// {
//     public Dog(string breed, string name, int age, bool hasShots) //~ this is the constructor?
//     {
//         Breed = breed;
//         Name = name;
//         Age = age;
//         HasShots = hasShots;
//     }
//     public string Breed { get; set; }
//     public string Name { get; set; }
//     public int Age { get; set; }
//     public bool HasShots { get; set; }
// }

// You make a new Dog with the constructor like this:

// Dog spot = new Dog("Beagle", "Spot", 2, true);
// Console.WriteLine(spot.Name);
// //output: "Spot"

//~ Follow this dependency injection pattern when creating new controllers for your APIs.
//~ The AuthController has two dependencies:

// private BiancasBikesDbContext _dbContext;
// private UserManager<IdentityUser> _userManager;

// public AuthController(BiancasBikesDbContext context, UserManager<IdentityUser> userManager)
// {
//     _dbContext = context;
//     _userManager = userManager;
// }

//~ Dependency Injection: Dependency injection allows you to access services (in this case, a database context) that your controller needs to perform its actions.