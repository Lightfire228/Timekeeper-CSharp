using Android.Content;
using AndroidX.Core.App;

namespace Tk.App;


[BroadcastReceiver(Enabled = true, Label = "Local Notifications Broadcast Receiver")]
public class AlarmRecevier
    : BroadcastReceiver
{

    public const string EXTRA_TITLE   = "EXTRA_TITLE";
    public const string EXTRA_MESSAGE = "EXTRA_MESSAGE";

    private static readonly ILogger Logger = MainApplication.BuildLogger();

    public override void OnReceive(Context? context, Intent? intent) {
        try {
            TryOnReceive(context, intent);
        }
        catch (Exception e) {
            Logger.LogError("Uncaught exception in alarm receiver: {e}", e);
            throw;
        }
    }

    public void TryOnReceive(Context? context, Intent? intent) {

        Logger.LogInformation("on receive alarm");

        if (context == null || intent == null) {
            Logger.LogInformation("null context or intent");
            return;
        }

        if (intent.Action != TkIntents.TK_REMINDER) {
            Logger.LogInformation("not a reminder action");
            return;
        }

        var message = intent.GetStringExtra(EXTRA_MESSAGE);
        var title   = intent.GetStringExtra(EXTRA_TITLE);

        if (message == null || title == null) {
            return;
        }


        Logger.LogInformation("compat manager");
        var compatManager = NotificationManagerCompat.From(context)!;

        Logger.LogInformation("notif manager");
        var notifManager  = (NotificationManager)context!.GetSystemService(Context.NotificationService)!;

        Logger.LogInformation("send notif");
        var service = new AndroidNotificationService(new() {
            CompatManager = compatManager,
            NotifManager  = notifManager,
            MainActivity  = typeof(MainActivity),
            AppContext    = context,
            SmallIcon     = Resource.Drawable.appicon,
            LargeIcon     = Resource.Drawable.appicon,
        });

        Logger.LogInformation("send notif");
        service.Show(title, message, NotificationChannelType.Default);
    }

} 
