using Bookify.Domain.Shared;

namespace Bookify.Domain.Bookings;

public record PricingDetails(
    Money PriceForPeriod, //租金费用
    Money CleaningFee, //清洁费用
    Money AmenitiesUpCharge, //附加费用，例如停车费
    Money TotalPrice); //总费用
