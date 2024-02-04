using Rop.Result.Object;

namespace Rop.Console;

internal static class CarBuilder
{
    internal static Result<Car> CreateCar() => new Car();

    internal static Result<Car> AddEngine(this Result<Car> car,Func<Result<Engine>> engine)
    {
        System.Console.WriteLine("Enters AddEngine");
        var createdEngine = engine();        
        if(createdEngine.IsSuccess)        
        {
            var response =  car.Data with {Engine = createdEngine.Data };
            System.Console.WriteLine(response);
            System.Console.WriteLine("Exits AddEngine");
            return response;
        }  
        System.Console.WriteLine("Exits Errored AddEngine");
        return createdEngine.Errors;
    }
    internal static Func<Result<Car>,Model,Result<Car>> AddModel = (car,model) => 
        car.IsSuccess 
            ? car.Data with {Model = model }
            : car.Errors;
    internal static Func<Result<Car>,Wheels,Result<Car>> AddWheels = (car,wheels) =>
        car.IsSuccess 
            ? car.Data with {Wheels = wheels }
            : car.Errors; 

}
