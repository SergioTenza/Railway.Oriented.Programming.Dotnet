using System.Text.Json;
using Rop.Console;
using Rop.Result.Object.Extensions;

var car = CarBuilder.CreateCar()    
    .Tee(Console.WriteLine)
    .Apply(Engine.Hybrid,CarBuilder.AddEngine)
    .Tee(Console.WriteLine)
    .Apply(Model.Citroën,CarBuilder.AddModel)
    .Tee(Console.WriteLine)
    .Apply(Wheels.Four,CarBuilder.AddWheels)
    .Tee(Console.WriteLine);


var options = new JsonSerializerOptions{WriteIndented = true};
Console.WriteLine(JsonSerializer.Serialize(car,options));