using System;
using System.Collections.Generic;
using System.Linq;

namespace autobid.Services;

internal static class MockUsers
{
    internal record User(string Username, string Password, bool Corporate);

    internal static readonly List<User> Items =
    [
        new("admin", "1234", true),
        new("jon", "pass", false),
    ];

    internal static User? Login(string username, string password) =>
        Items.FirstOrDefault(u =>
            string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase) &&
            u.Password == password);

    internal static bool TryCreate(string username, string password, bool corporate)
    {
        if (Items.Any(u => string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase)))
            return false;
        Items.Add(new User(username, password, corporate));
        return true;
    }
}
