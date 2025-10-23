
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using AndroidX.Core.App;

namespace Tk.App;

public class AndroidNotificationService(AndroidNotificationServiceOpts opts)
    : INotificationService
{
    static int MessageId                  = 0;
    static int PendingActivityRequestCode = 0;


    readonly AndroidNotificationServiceOpts Opts = opts;

    readonly AlarmScheduler                 AlarmScheduler = new(opts.AppContext);

    readonly ILogger Logger = MainApplication.BuildLogger();


    static readonly bool ImmutableSupported = Build.VERSION.SdkInt >= BuildVersionCodes.S;
    static readonly bool ChannelsSupported  = Build.VERSION.SdkInt >= BuildVersionCodes.O;

    static PendingIntentFlags UpdateFlags { get => 
        ImmutableSupported ? PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable
                           : PendingIntentFlags.UpdateCurrent
    ;}

    static PendingIntentFlags CancelFlags { get =>
        ImmutableSupported ? PendingIntentFlags.CancelCurrent | PendingIntentFlags.Immutable
                           : PendingIntentFlags.CancelCurrent
    ;}
        

    public void SendNotification(string title, string message, NotificationChannelType channel, DateTime? notifyTime = null) {
        Logger.LogInformation("send notif");

        CreateNotificationChannel(channel);

        if (notifyTime == null) {
            Show(title, message, channel);
            return;
        }

        Logger.LogInformation("schedule notif");

        AlarmScheduler.Schedule(new () {
            ScheduledTime = notifyTime ?? throw new Exception("y tho"),
            // ScheduledTime = notifyTime,
            Message       = message,
            Title         = title,
        });
    }

    public void Show(string title, string message, NotificationChannelType channel) {

        Logger.LogInformation("show notif");

        var pendingIntent = GetPendingIntent(
            title, 
            message, 
            UpdateFlags,
            PendingIntent.GetActivity
        );


        var builder       = new NotificationCompat.Builder(Opts.AppContext, channel.GetAttr().ChannelId)
             .SetContentIntent(pendingIntent)
            ?.SetContentTitle (title)
            ?.SetContentText  (message)
            ?.SetLargeIcon    (BitmapFactory.DecodeResource(
                Opts.AppContext.Resources,
                Opts.LargeIcon
            ))
            ?.SetSmallIcon    (Opts.SmallIcon)

            ?? throw Panic("Unable to build notification")

        ;

        var notification = builder.Build();
        Opts.CompatManager.Notify(MessageId++, notification);
    }

    private void CreateNotificationChannel(NotificationChannelType channelType) {

        if (!ChannelsSupported) {
            return;
        }

        var attr = channelType.GetAttr();

        var channel = new NotificationChannel(
            attr.ChannelId, 
            attr.DisplayName, 
            NotificationImportance.Default
        ) {
            Description = attr.ChannelDesc
        };

        Opts.NotifManager.CreateNotificationChannel(channel);
    }


    private Exception Panic(string message) =>
        new(message)
    ;

    private PendingIntent GetPendingIntent(
        string             title,
        string             message,
        PendingIntentFlags intentFlags,

        Func<Context?, int, Intent, PendingIntentFlags, PendingIntent?> getIntent
    ) {

        Intent intent = new (Opts.AppContext, Opts.MainActivity);
        
        intent.PutExtra("Title",   title);
        intent.PutExtra("Message", message);
        intent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTop);

        return getIntent(
            Opts.AppContext,
            PendingActivityRequestCode++,
            intent,
            intentFlags
        )
            ?? throw Panic("Unable to get pending intent")
        ;

    }
}


public class NotificationEventArgs() 
    : EventArgs
{
    public required string Title   { get; set; }
    public required string Message { get; set; }
}

public class AndroidNotificationServiceOpts {
    public required NotificationManagerCompat CompatManager { get; set; }
    public required NotificationManager       NotifManager  { get; set; }
    public required Type                      MainActivity  { get; set; }
    public required Context                   AppContext    { get; set; }

    public required int                       SmallIcon     { get; set; }
    public required int                       LargeIcon     { get; set; }
}


