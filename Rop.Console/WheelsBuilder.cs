using Rop.Result.Object;

namespace Rop.Console;

internal static class WheelsBuilder
{
    public static Result<Wheels> CreateWheels() => new Wheels();
    public static Result<Wheels> CreateWheels(Wheels wheels) => wheels;
}
