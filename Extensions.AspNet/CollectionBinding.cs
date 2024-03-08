// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Econolite.Ode.Extensions.AspNet;

public static class ExtensionsClass
{
    public static void AddCommaSeparatedGuidCollectionParsing(this MvcOptions options)
    {
        options.ModelBinderProviders.Insert(0, new EnumerableGuidBinderProvider());
    }
}

public class EnumerableGuidBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        return context.Metadata.ModelType == typeof(IEnumerable<Guid>) ? new EnumerableGuidBinder() : null;
    }
}

public class EnumerableGuidBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext.ModelType == typeof(IEnumerable<Guid>) && bindingContext.HttpContext.Request.Query.ContainsKey(bindingContext.FieldName))
        {
            var result = bindingContext.HttpContext.Request.Query[bindingContext.FieldName].ToString();

            bindingContext.Result = ModelBindingResult.Success(result.Split(",").Select(Guid.Parse));
        }

        return Task.CompletedTask;
    }
}
