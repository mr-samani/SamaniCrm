using Hangfire;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Infrastructure.Notifications;

public abstract class LoginNotification
{
    public static Task SendLoginNotification(string username)
    {
        // کد ارسال نوتیفیکیشن یا لاگ
        Console.WriteLine($"User {username} logged in successfully!");
        return Task.CompletedTask;
    }

    public static Task SendLoginFailureNotification(string username)
    {
        // کد ارسال نوتیفیکیشن یا لاگ
        Console.WriteLine($"User {username} failed to log in.");
        return Task.CompletedTask;
    }
}

