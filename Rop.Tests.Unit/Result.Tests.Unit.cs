using Rop.Result.Object;
using Rop.Result.Object.Extensions;

namespace Rop.Tests.Unit;

public class ResultTestsUnit
{
    [Fact]
    public void ShouldBeErrorResultOnBadTryCatchSwitch()
    {
        //given
        var expectedMessage = "Not implemented";
        var expectedDomainError = DomainError.UnhandledException;
        var expectedErrorCode = "3";
        Result<bool> createResult = true;
        Func<Result<bool>,Result<bool>> action = (func)=>
        {
            throw new NotImplementedException(expectedMessage);
        };

        
        //when 
        var result = createResult.TryCatchSwitch(_ => action(_));

        //then
        Assert.False(result.IsSuccess);
        Assert.Equal(expectedMessage,result.Errors.First().Message);
        Assert.Equal(expectedErrorCode,result.Errors.First().Code);
        Assert.Equal(expectedDomainError,result.Errors.First().DomainError);
    }

    [Fact]
    public void ShouldBeSuccessResultOnTryCatchSwitch()
    {
        //given
        Result<bool> createResult = true;
        Func<Result<bool>,Result<bool>> action = (func)=>
        {
            return true;
        };

        
        //when 
        var result = createResult.TryCatchSwitch(_ => action(_));

        //then
        Assert.True(result.IsSuccess);
    }

     [Fact]
    public void ShouldBeErrorResultOnFalseBooleanSwitch()
    {
        //given        
        var expectedDomainError = DomainError.BooleanSwitchFailed;        
        Result<bool> createResult = true;
        Func<Result<bool>,bool> action = (func)=>
        {
            return false;
        };

        
        //when 
        var result = createResult.BooleanSwitch(_ => action(_));

        //then
        Assert.False(result.IsSuccess);        
        Assert.Equal(expectedDomainError,result.Errors.First().DomainError);
    }

    [Fact]
    public void ShouldBeSuccessResultOnTrueBooleanSwitch()
    {
        //given
        Result<bool> createResult = true;
        Func<Result<bool>,bool> action = (func)=>
        {
            return true;
        };

        
        //when 
        var result = createResult.BooleanSwitch(_ => action(_));

        //then
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void ShoulBeFalseOnNullValue()
    {
        //given
        Order? order = null;
        var expectedError = new Error { DomainError = DomainError.NullValue };

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
        Error error = new Error { DomainError = DomainError.NullValue };
        var expectedError = DomainError.NullValue;

        //when 
        Result<Order> result = error;

        //then
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.NotEmpty(result.Errors);
        Assert.True(result.Errors.Length == 1);
        Assert.IsType<Error>(result.Errors[0]);
        Assert.Equal(expectedError, result.Errors[0].DomainError);
    }

    [Fact]
    public void ShoulBeFalseOnErrorsValues()
    {
        //given
        Error errorFirst = new Error { DomainError = DomainError.NullValue };
        Error errorSecond = new Error { DomainError = DomainError.NullValue };
        Error[] errors = [errorFirst, errorSecond];
        var expectedError = DomainError.NullValue;

        //when 
        Result<Order> result = errors;

        //then
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.NotEmpty(result.Errors);
        Assert.True(result.Errors.Length == 2);
        Assert.IsType<Error[]>(result.Errors);
        Assert.IsType<Error>(result.Errors[0]);
        Assert.IsType<Error>(result.Errors[1]);
        Assert.Equal(expectedError, result.Errors[0].DomainError);
        Assert.Equal(expectedError, result.Errors[1].DomainError);
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
        Assert.IsType<Error[]>(result.Errors);
    }
    [Fact]
    public void ShoulBeOfGivenTypeOnNonNullValueOnSuccess()
    {
        //given
        Order orderBinding = new Order();
        var expectedOrder = orderBinding;

        //when 
        Result<Order> binding = orderBinding;
        var bind = binding
                    .Bind(o => GetBindingOrder(o));

        //then
        Assert.True(bind.IsSuccess);
        Assert.False(bind.IsFailure);
        Assert.Empty(bind.Errors);
        Assert.IsType<Order>(bind.Data);
        Assert.Equal(bind.Data, expectedOrder);
    }

    [Fact]
    public void ShoulBeFailureOnNullValueOnSuccess()
    {
        //given
        Order? orderBinding = default;
        var expectedDomainError = DomainError.NullValue;

        //when 
        Result<Order> binding = orderBinding;
        var bind = binding
                    .Bind(o => GetBindingOrder(o));

        //then
        Assert.False(bind.IsSuccess);
        Assert.True(bind.IsFailure);
        Assert.NotEmpty(bind.Errors);
        Assert.IsType<Error[]>(bind.Errors);
        Assert.True(bind.Errors.Length == 1);
        Assert.Equal(bind.Errors[0].DomainError, expectedDomainError);
    }

    [Fact]
    public void ShoulBeOfGivenTypeOnNonNullValueOnMap()
    {
        //given
        Order orderMap = new Order();
        var expectedOrder = orderMap;

        //when 
        Result<Order> mapping = orderMap;
        var bind = mapping
                    .Map(o => GetBindingOrderMap(o));

        //then
        Assert.True(bind.IsSuccess);
        Assert.False(bind.IsFailure);
        Assert.Empty(bind.Errors);
        Assert.IsType<Order>(bind.Data);
        Assert.Equal(bind.Data, expectedOrder);
    }
    [Fact]
    public void ShoulBeFailureOnNullValueOnMap()
    {
        //given
        Order? orderMap = default;
        var expectedDomainError = DomainError.NullValue;

        //when 
        Result<Order> mapping = orderMap;
        var bind = mapping
                    .Map(o => GetBindingOrderMap(o));

        //then
        Assert.False(bind.IsSuccess);
        Assert.True(bind.IsFailure);
        Assert.NotEmpty(bind.Errors);
        Assert.IsType<Error[]>(bind.Errors);
        Assert.True(bind.Errors.Length == 1);
        Assert.Equal(bind.Errors[0].DomainError, expectedDomainError);
    }
    [Fact]
    public void ShoulBeOfGivenTypeOnNonNullValueOnDoubleMap()
    {
        //given
        Order orderMap = new Order();
        var expectedOrder = orderMap;

        //when 
        Result<Order> mapping = orderMap;
        var bind = mapping
                    .DoubleMap(s => GetBindingOrderMap(s),f => GetBindingOrderMap(f));

        //then
        Assert.True(bind.IsSuccess);
        Assert.False(bind.IsFailure);
        Assert.Empty(bind.Errors);
        Assert.IsType<Order>(bind.Data);
        Assert.Equal(bind.Data, expectedOrder);
    }
    [Fact]
    public void ShoulBeFailureOnNullValueOnDoubleMap()
    {
        //given
        Order? orderMap = default;
        var expectedDomainError = DomainError.NullValue;

        //when 
        Result<Order> mapping = orderMap;
        var bind = mapping
                    .DoubleMap(s => GetBindingOrderMap(s),f => GetBindingOrderMap(f));

        //then
        Assert.False(bind.IsSuccess);
        Assert.True(bind.IsFailure);
        Assert.NotEmpty(bind.Errors);
        Assert.IsType<Error[]>(bind.Errors);
        Assert.True(bind.Errors.Length == 1);
        Assert.Equal(bind.Errors[0].DomainError, expectedDomainError);
    }

     [Fact]
    public void ShoulBeOfGivenTypeOnNonNullValueOnTee()
    {
        //given
        Order orderMap = new Order();
        var expectedOrder = orderMap;

        //when 
        Result<Order> tee = orderMap;
        var bind = tee
                    .Tee(t => OrderAction(t));

        //then
        Assert.True(bind.IsSuccess);
        Assert.False(bind.IsFailure);
        Assert.Empty(bind.Errors);
        Assert.IsType<Order>(bind.Data);
        Assert.Equal(bind.Data, expectedOrder);    
    }
    private static Action<Order> OrderAction = (order) => 
    {

    }; 
    private static Func<Order, Order> GetBindingOrderMap = (input) => input;
    private static Func<Order, Result<Order>> GetBindingOrder = (input) => input;
    private static Func<Order, bool> GetOrder = (order) => true;
    private static Func<Order, bool> GetOrderFalse = (order) => false;
    private static Func<Error[], bool> GetOrderFault = (order) => false;

}
