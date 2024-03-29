﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using StoreProject.Core.Domain.Base;
using StoreProject.Core.Domain.Logger;

namespace StoreProject.API.ActionFilters
{
    public class ValidateProductCategoryExistsAttribute : IAsyncActionFilter
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        public ValidateProductCategoryExistsAttribute(IRepositoryManager repository,
            ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var trackChanges = context.HttpContext.Request.Method.Equals("PUT");
            var id = (Guid)context.ActionArguments["id"];
            var productCategory = await _repository.ProductCategory.GetProductCategoryById(id, trackChanges);
            if (productCategory == null)
            {
                _logger.LogInfo($"Product category with id: {id} doesn't exist in the database.");
                context.Result = new NotFoundResult();
            }
            else
            {
                context.HttpContext.Items.Add("category", productCategory);
                await next();
            }
        }
    }
}
