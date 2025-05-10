namespace Bookify.Domain.Shared;

public record Money(decimal Amount, Currency Currency)
{
    //运算符重载，必须相同的Currency才能相加
    public static Money operator +(Money first, Money second)
    {
        if (first.Currency != second.Currency)
        {
            throw new InvalidOperationException("Currencies have to be equal");
        }

        return new Money(first.Amount + second.Amount, first.Currency);
    }

    //返回一个“空金额”，没有货币单位
    public static Money Zero()
    {
        return new Money(0, Currency.None);
    }

    //返回指定货币单位的 0 金额
    public static Money Zero(Currency currency)
    {
        return new Money(0, currency);
    }

    //查金额是否为零，且货币单位相同
    public bool IsZero()
    {
        return this == Zero(Currency);
    }
}
