using System.Text.Json;
using Rop.Console;
using Rop.Result.Object.Extensions;

var car = CarBuilder.CreateCar()    
    .Tee(Console.WriteLine)
    .Apply(EngineBuilder.CreateEngine(Engine.Hybrid),CarBuilder.AddEngine)
    .Tee(Console.WriteLine)
    .Apply(ModelBuilder.CreateModel(Model.Citroën),CarBuilder.AddModel)
    .Tee(Console.WriteLine)
    .Apply(WheelsBuilder.CreateWheels(Wheels.Four),CarBuilder.AddWheels)
    .Tee(Console.WriteLine);


Console.WriteLine(JsonSerializer.Serialize(car,new JsonSerializerOptions{WriteIndented = true}));