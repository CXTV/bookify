using Bookify.Application.Abstractions.Email;
using Bookify.Domain.Bookings;
using Bookify.Domain.Bookings.Events;
using Bookify.Domain.Users;
using MediatR;

namespace Bookify.Application.Bookings.ReserveBooking;

/// 处理 BookingReservedDomainEvent 事件
internal sealed class BookingReservedDomainEventHandler : INotificationHandler<BookingReservedDomainEvent>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IEmailService _emailService;
    private readonly IUserRepository _userRepository;

    public BookingReservedDomainEventHandler(
        IEmailService emailService,
        IUserRepository userRepository,
        IBookingRepository bookingRepository)
    {
        _emailService = emailService;
        _userRepository = userRepository;
        _bookingRepository = bookingRepository;
    }


    public async Task Handle(BookingReservedDomainEvent notification, CancellationToken cancellationToken)
    {
        //通过事件里传来的 BookingId 从数据库查出预订信息。
        Booking? booking = await _bookingRepository.GetByIdAsync(notification.BookingId, cancellationToken);

        if (booking is null)
        {
            return;
        }

        //根据 booking.UserId 查出用户信息。
        User? user = await _userRepository.GetByIdAsync(booking.UserId, cancellationToken);

        if (user is null)
        {
            return;
        }

        //通过用户信息发送邮件。(可以是支付/消息队列/存储服务)
        await _emailService.SendAsync(
            user.Email,
            "Booking reserved!",
            "You have 10 minutes to confirm this booking");
    }
}
