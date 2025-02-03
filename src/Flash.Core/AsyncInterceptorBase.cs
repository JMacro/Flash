using Castle.DynamicProxy;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Flash.Core
{
    public abstract class AsyncInterceptorBase : IInterceptor
    {
        public AsyncInterceptorBase()
        {
        }

        public void Intercept(IInvocation invocation)
        {
            BeforeProceed(invocation);
            invocation.Proceed();
            if (IsAsyncMethod(invocation.MethodInvocationTarget))
            {
                invocation.ReturnValue = InterceptAsync((dynamic)invocation.ReturnValue, invocation);
            }
            else
            {
                AfterProceedSync(invocation);
            }
        }

        private bool CheckMethodReturnTypeIsTaskType(MethodInfo method)
        {
            var methodReturnType = method.ReturnType;
            if (methodReturnType.IsGenericType)
            {
                if (methodReturnType.GetGenericTypeDefinition() == typeof(Task<>) ||
                    methodReturnType.GetGenericTypeDefinition() == typeof(ValueTask<>))
                    return true;
            }
            else
            {
                if (methodReturnType == typeof(Task))
                    return true;
            }
            return false;
        }

        private bool IsAsyncMethod(MethodInfo method)
        {
            bool isDefAsync = Attribute.IsDefined(method, typeof(AsyncStateMachineAttribute), false);
            bool isTaskType = CheckMethodReturnTypeIsTaskType(method);
            bool isAsync = isDefAsync && isTaskType;

            return isAsync;
        }

        public object ProceedAsyncResult { get; internal set; }


        private async Task InterceptAsync(Task task, IInvocation invocation)
        {
            await task.ConfigureAwait(false);
            await AfterProceedAsync(invocation, false);
        }

        private async Task<TResult> InterceptAsync<TResult>(Task<TResult> task, IInvocation invocation)
        {
            ProceedAsyncResult = await task.ConfigureAwait(false);
            await AfterProceedAsync(invocation, true);
            return (TResult)ProceedAsyncResult;
        }

        private async ValueTask<TResult> InterceptAsync<TResult>(ValueTask<TResult> task, IInvocation invocation)
        {
            ProceedAsyncResult = await task.ConfigureAwait(false);
            await AfterProceedAsync(invocation, true);
            return (TResult)ProceedAsyncResult;
        }

        /// <summary>
        /// 执行方法之前
        /// </summary>
        /// <param name="invocation"></param>
        protected virtual void BeforeProceed(IInvocation invocation) { }
        /// <summary>
        /// 执行方法（同步）之后
        /// </summary>
        /// <param name="invocation"></param>
        protected virtual void AfterProceedSync(IInvocation invocation) { }
        /// <summary>
        /// 执行方法（异步）之后
        /// </summary>
        /// <param name="invocation"></param>
        /// <param name="hasAsynResult"></param>
        /// <returns></returns>
        protected virtual Task AfterProceedAsync(IInvocation invocation, bool hasAsynResult)
        {
            return Task.CompletedTask;
        }
    }
}
