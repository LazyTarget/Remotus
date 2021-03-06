﻿using System;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Lux;
using Lux.Interfaces;

namespace Remotus.Base
{
    public static class Extensions
    {
        public static TProp TryGetProp<TObj, TProp>(this TObj obj, Expression<Func<TObj, TProp>> lamda)
        {
            try
            {
                var value = lamda.Compile().Invoke(obj);
                return value;
            }
            catch (Exception ex)
            {
                return default(TProp);
            }
        }


        public static IParameter<TValue> GetOrDefault<TValue>(this IParameterCollection collection, string parameterName)
        {
            IParameter parameter = null;
            if (collection?.ContainsKey(parameterName) ?? false)
                parameter = collection[parameterName];
            var result = Parameter<TValue>.Create(parameter);
            return result;
        }


        public static TValue GetParamValue<TValue>(this IParameterCollection collection, string parameterName)
        {
            var result = default(TValue);
            if (string.IsNullOrEmpty(parameterName))
            {
                throw new ArgumentNullException(nameof(parameterName), "Invalid parameter name");
            }

            try
            {
                if (collection != null && collection.ContainsKey(parameterName))
                {
                    var param = collection[parameterName];
                    if (param != null)
                    {
                        IConverter converter = new Converter();
                        result = converter.Convert<TValue>(param.Value);
                    }
                    else
                    {

                    }
                }
                else
                {

                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }


        public static IParameter SetParamValue(this IParameterCollection collection, string parameterName, object value)
        {
            IParameter result = null;
            if (string.IsNullOrEmpty(parameterName))
            {
                throw new ArgumentNullException(nameof(parameterName), "Invalid parameter name");
            }

            try
            {
                if (collection != null && collection.ContainsKey(parameterName))
                {
                    result = collection[parameterName];
                    if (result != null)
                    {
                        var type = value?.GetType();
                        if (type != null && result.Type != null)
                        {
                            if (type != result.Type)
                            {
                                IConverter converter = new Converter();
                                value = converter.Convert(value, result.Type);
                            }
                        }

                        result.Value = value;
                    }
                    else
                    {

                    }
                }
                else
                {

                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }


        public static bool IsBase64String(this string s)
        {
            s = s.Trim();
            var res = (s.Length % 4 == 0) && Regex.IsMatch(s, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
            return res;
        }


        public static void ThrowIfNullResponse(this IResponseBase response)
        {
            if (response == null)
                throw new Exception("Request resulted in a null response");
        }

        public static void ThrowIfNullResult(this IResponseBase response)
        {
            if (response?.Result == null)
                throw new Exception("Request resulted in a null response result");
        }

        public static void ThrowIfError(this IResponseBase response)
        {
            if (response?.Error != null)
                response.Error.Throw();
        }

        public static void EnsureSuccess(this IResponseBase response, string errorMessage = null)
        {
            try
            {
                response.ThrowIfError();
                response.ThrowIfNullResponse();
                response.ThrowIfNullResult();
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(errorMessage))
                    throw new Exception(errorMessage, ex);
                throw;
            }
        }


    }
}