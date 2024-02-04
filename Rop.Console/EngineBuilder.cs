using Rop.Result.Object;

namespace Rop.Console;

internal static class EngineBuilder
{
    public static Result<Engine> CreateEngine() => new Engine();
    public static Result<Engine> CreateEngine(Engine engine) => engine;

}
