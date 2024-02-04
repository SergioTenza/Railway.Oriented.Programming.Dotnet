using Rop.Result.Object;

namespace Rop.Console;

internal static class CarBuilder
{
    internal static Result<Car> CreateCar() => new Car();

    internal static Result<Car> AddEngine(Result<Car> car,Result<Engine> engine)
    {
        if(car.IsSuccess && engine.IsSuccess)        
        {
            var response =  car.Data with {Engine = engine.Data };
            return response;
        } 
        List<Error> errors = [];
        if(car.IsFailure)
        {
            errors.AddRange(car.Errors);
        } 
        if(engine.IsFailure)
        {
            errors.AddRange(engine.Errors);
        } 
        return errors.ToArray();
    }

    internal static Result<Car> AddModel(Result<Car> car,Result<Model> model)
    {
        if(car.IsSuccess && model.IsSuccess)        
        {
            var response =  car.Data with {Model = model.Data };
            return response;
        } 
        List<Error> errors = [];
        if(car.IsFailure)
        {
            errors.AddRange(car.Errors);
        } 
        if(model.IsFailure)
        {
            errors.AddRange(model.Errors);
        } 
        return errors.ToArray();
    }

    internal static Result<Car> AddWheels(Result<Car> car,Result<Wheels> wheels)
    {
        if(car.IsSuccess && wheels.IsSuccess)        
        {
            var response =  car.Data with {Wheels = wheels.Data };
            return response;
        } 
        List<Error> errors = [];
        if(car.IsFailure)
        {
            errors.AddRange(car.Errors);
        } 
        if(wheels.IsFailure)
        {
            errors.AddRange(wheels.Errors);
        } 
        return errors.ToArray();
    }
}
