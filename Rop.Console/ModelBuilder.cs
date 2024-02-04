using Rop.Result.Object;

namespace Rop.Console;

internal static class ModelBuilder
{
    public static Result<Model> CreateModel() => new Model();
    public static Result<Model> CreateModel(Model model) => model;
}
