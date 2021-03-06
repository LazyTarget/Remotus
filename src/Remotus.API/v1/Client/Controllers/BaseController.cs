﻿using System;
using System.Threading.Tasks;
using System.Web.Http;
using Lux;
using Lux.Interfaces;
using Remotus.API.v1.Models;
using Remotus.Base;

namespace Remotus.API.v1.Client.Controllers
{
    [Obsolete]
    [ControllerCategory("Client")]
    public abstract class BaseController : ApiController, IBaseController
    {
        protected IConverter Converter = new Converter();
        protected Worker Worker => new Worker();
        protected IProcessHelper ProcessHelper => new ProcessHelper();
        protected IPowershellHelper PowershellHelper => new PowershellHelper(ProcessHelper);
        protected IInputHelper InputHelper => new InputHelper();
        protected IWinServiceHelper WinServiceHelper => new WinServiceHelper();

        protected BaseController()
        {
            
        }


        protected IClientInfo LoadClientInfo()
        {
            var clientInfo = new ClientInfo();
            clientInfo.ClientID = Configuration.Properties["InstanceID"].ToString();

            var rootAddress = (Uri) Configuration.Properties["ApiRootAddress"];
            clientInfo.ApiAddress = new Uri(rootAddress, "/v1/");
            return clientInfo;
        }


        [Obsolete]
        public async Task<IFunctionResult> ExecuteFunction(IFunction function, IFunctionArguments arguments)
        {
            try
            {
                IExecutionContext context = new ExecutionContext
                {
                    ClientInfo = LoadClientInfo(),
                    Logger = new TraceLogger(),
                    Remotus = new FullCtrlAPI(),
                };

                var result = await function.Execute(context, arguments);
                return result;
            }
            catch (Exception ex)
            {
                var result = new FunctionResult();
                result.Arguments = arguments;
                result.Error = DefaultError.FromException(ex);
                return result;
            }
        }


        [Obsolete]
        public ResponseBase<TResult> CreateFunctionResponse<TResult>(IFunctionResult functionResult)
        {
            ResponseBase<TResult> response = null;
            if (functionResult == null)
            {
                response = ResponseBase.Create<TResult>(Request);
            }
            else if (functionResult.Error != null)
            {
                response = ResponseBase.CreateError<TResult>(Request, functionResult.Error);
            }
            else
            {
                var result = Converter.Convert<TResult>(functionResult.Result);
                response = ResponseBase.Create<TResult>(Request, result);
            }
            return response;
        }


        [Obsolete]
        public ResponseBase<TResult> CreateResponse<TResult>(TResult result = default(TResult), IError error = null)
        {
            var response = ResponseBase.Create<TResult>(Request, result, error);
            return response;
        }


        [Obsolete]
        public ResponseBase<object> CreateError(IError error)
        {
            var response = CreateError<object>(error);
            return response;
        }

        [Obsolete]
        public ResponseBase<TResult> CreateError<TResult>(IError error)
        {
            var response = ResponseBase.Create<TResult>(Request);
            response.Error = error;
            return response;
        }



    }
}
