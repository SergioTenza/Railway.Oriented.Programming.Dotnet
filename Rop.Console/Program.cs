using Rop.Console;
using Rop.Result.Object.Extensions;

var car = CarBuilder.CreateCar()    
    .Apply(c => c.AddEngine(EngineBuilder.CreateEngine(Engine.Hybrid)));

