using System;
using autobid.Domain.Users;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Identity.Client;

namespace autobid.API.RequestBinders;

public class UserRequestBinder : IModelBinder
{
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        string typeName = bindingContext.ValueProvider.GetValue("vehicleType").FirstValue ?? "";
        uint id = uint.Parse(bindingContext.ValueProvider.GetValue("id").FirstValue ?? "0"); 
        string username = bindingContext.ValueProvider.GetValue("username").FirstValue 
            ?? throw new ArgumentNullException();
        string passwordHash = bindingContext.ValueProvider.GetValue("passwordHash").FirstValue 
            ?? throw new ArgumentNullException();
        decimal credit = decimal.Parse(bindingContext.ValueProvider.GetValue("credit").FirstValue ?? "0");
        decimal balance = decimal.Parse(bindingContext.ValueProvider.GetValue("balance").FirstValue ?? "0");
        
        User user = typeName switch
        {
            CorporateCustomer.TypeName => 
                new CorporateCustomer(id, username, passwordHash, 
                    bindingContext.ValueProvider.GetValue("cvr").FirstValue ?? throw new ArgumentNullException(),
                    credit, balance),
            PrivateCustomer.TypeName =>
                new PrivateCustomer(id, username, passwordHash, 
                bindingContext.ValueProvider.GetValue("cpr").FirstValue ?? throw new ArgumentNullException(),
                balance),
            _ => throw new ArgumentException("incorrect type")
        };

        bindingContext.Result = ModelBindingResult.Success(user);
        await Task.CompletedTask;
    }
}
