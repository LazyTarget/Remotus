﻿using System;
using System.Web.Http;
using FullCtrl.API.Interfaces;
using FullCtrl.API.v1.Models;
using FullCtrl.Base;

namespace FullCtrl.API.v1.Controllers
{
    public abstract class BaseController : ApiController
    {
        protected Worker Worker => new Worker();
        protected IProcessHelper ProcessHelper => new ProcessHelper();
        
        protected BaseController()
        {
            
        }


        protected ResponseBase<TResult> CreateResponse<TResult>(TResult result = default(TResult))
        {
            var serverRootUri = new Uri(Request.RequestUri.GetLeftPart(UriPartial.Authority));

            var response = new ResponseBase<TResult>();
            response.Links["self"] = LinkFromUri(Request.RequestUri);
            response.Links["root"] = LinkFromUri(new Uri(serverRootUri, "api/v1"));

            response.Result = result;
            return response;
        } 


        protected ILink LinkFromUri(Uri uri)
        {
            var link = new Link
            {
                Href = uri.AbsoluteUri,
                Relative = uri.AbsolutePath,
            };
            return link;
        }

    }
}
