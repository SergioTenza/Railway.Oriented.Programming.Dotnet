using Rop.Result.Object;

namespace Rop.Tests.Unit;

public class ResultTestsUnit
{
    [Fact]
    public void ShoulBeFalseOnNullValue()
    {
        //given
        Order order = null;
        var expectedError = Error.NullValue;

        //when 
        Result<Order> result = order;

        //then
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.NotEmpty(result.Errors);
        Assert.True(result.Errors.Length == 1);
        Assert.Equal(expectedError, result.Errors[0]);
    }

    [Fact]
    public void ShoulBeFalseOnErrorValue()
    {
        //given
        Error error = Error.NullValue;
        var expectedError = Error.NullValue;

        //when 
        Result<Order> result = error;

        //then
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.NotEmpty(result.Errors);
        Assert.True(result.Errors.Length == 1);
        Assert.Equal(expectedError, result.Errors[0]);
    }

    [Fact]
    public void ShoulBeFalseOnErrorsValues()
    {
        //given
        Error errorFirst = Error.NullValue;
        Error errorSecond = Error.NullValue;
        Error[] errors = [errorFirst, errorSecond];
        var expectedError = Error.NullValue;

        //when 
        Result<Order> result = errors;

        //then
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.NotEmpty(result.Errors);
        Assert.True(result.Errors.Length == 2);
        Assert.Equal(expectedError, result.Errors[0]);
        Assert.Equal(expectedError, result.Errors[1]);
    }

    [Fact]
    public void ShoulBeTrueOnNonNullValue()
    {
        //given
        Order order = new Order();

        //when 
        Result<Order> result = order;

        //then
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Empty(result.Errors);
    }
    [Fact]
    public void ShoulBeOfGivenTypeOnNonNullValueMatch()
    {
        //given
        Order order = new Order();
        Error[] errors = [];

        //when 
        Result<Order> result = order;
        var matched = result.Match<bool>(
            success => GetOrder(order),
            failure => GetOrderFault(errors)
        );

        //then
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Empty(result.Errors);
        Assert.IsType<bool>(matched);
        Assert.True(matched);
    }
    private static Func<Order,bool> GetOrder = (order) => true;
    private static Func<Order,bool> GetOrderFalse = (order) => false;
    private static Func<Error[],bool> GetOrderFault= (order) => false;

}
