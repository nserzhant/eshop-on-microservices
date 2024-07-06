namespace EShop.Ordering.Infrastructure.Services;
public class DateTimeService : IDateTimeService
{
    public DateTime GetCurrentDateTime() => DateTime.Now;
}
