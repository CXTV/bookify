using Bookify.Application.Abstractions.Messaging;
using Bookify.Application.Caching;

namespace Bookify.Application.Bookings.GetBooking;

//public sealed record GetBookingQuery(Guid BookingId) : IQuery<BookingResponse>;


public sealed record GetBookingQuery(Guid BookingId) : ICachedQuery<BookingResponse>
{
    public string CacheKey => $"GetBooking-{BookingId}";
    public TimeSpan? Expiration { get; }
}


