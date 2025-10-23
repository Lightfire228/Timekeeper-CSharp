using Android.Content;
using Tk.App.Models;

namespace Tk.App;

public interface IAlarmScheduler {

    void Schedule(AlarmItem item);
    void Cancel  (AlarmItem item);
}


public class AlarmScheduler(Context context)
        : IAlarmScheduler
{
    private readonly Context      _Context      = context;
    private readonly AlarmManager _AlarmManager = (AlarmManager)context.GetSystemService(Context.AlarmService)!;
    private readonly ILogger      _Logger       = MainApplication.BuildLogger();



    public void Schedule(AlarmItem item) {
        var intent = new Intent(_Context, typeof(AlarmRecevier));
        intent.SetAction(TkIntents.TK_REMINDER);
        intent.PutExtra(AlarmRecevier.EXTRA_MESSAGE, item.Message);
        intent.PutExtra(AlarmRecevier.EXTRA_TITLE,   item.Title);

        if (!OperatingSystem.IsAndroidVersionAtLeast(23)) {
            throw new Exception();
        }

        long millis = (item.ScheduledTime.Ticks - DateTime.UnixEpoch.Ticks) / TimeSpan.TicksPerMillisecond;

        _AlarmManager.SetExactAndAllowWhileIdle(
            AlarmType.RtcWakeup,
            millis,
            PendingIntent.GetBroadcast(
                _Context,
                item.GetHashCode(),
                intent,
                PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable
            )!
        );
    }

    public void Cancel(AlarmItem item) {

        if (!OperatingSystem.IsAndroidVersionAtLeast(23)) {
            throw new Exception();
        }

        _AlarmManager.Cancel(PendingIntent.GetBroadcast(
            _Context,
            item.GetHashCode(),
            new Intent(),
            PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable
        )!);
    }
}
