using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace autobid.API.RequestBinders;

public class UserRequestBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        throw new NotImplementedException();
    }
}
