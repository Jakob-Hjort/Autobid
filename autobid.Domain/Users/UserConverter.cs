using System;
using System.Text.Json.Nodes;

namespace autobid.Domain.Users;

public class UserConverter
{
    public static User Convert(Dictionary<string, JsonNode?> sellerData, string userType)
    {
        if (!sellerData.TryGetValue("id", out var sellerIdJson) ||
            !sellerData.TryGetValue("username", out var sellerNameJson) ||
            !sellerData.TryGetValue("passwordHash", out var sellerPasswordHashJson) ||
            !sellerData.TryGetValue("balance", out var sellerBalanceJson) ||
            !sellerData.TryGetValue("cvr", out var _) &&
            !sellerData.TryGetValue("cpr", out var _))
        {
            throw new ArgumentException("Invalid seller data");
        }
        
        uint sellerId = sellerIdJson?.GetValue<uint>() ?? 0;
        string sellerName = sellerNameJson?.GetValue<string>() ?? "";
        string sellerPasswordHash = sellerPasswordHashJson?.GetValue<string>() ?? "";
        decimal sellerBalance = sellerBalanceJson?.GetValue<decimal>() ?? 0;

        return userType switch
        {
            "Private" => new PrivateCustomer(sellerId, sellerName, sellerPasswordHash,
                sellerData["cpr"]?.GetValue<string>() ?? "", sellerBalance),
            "Corporate" => new CorporateCustomer(sellerId, sellerName, sellerPasswordHash,
                sellerData["cvr"]?.GetValue<string>() ?? "", sellerBalance),
            _ => throw new ArgumentException("Invalid seller type")
        };
    }
}
