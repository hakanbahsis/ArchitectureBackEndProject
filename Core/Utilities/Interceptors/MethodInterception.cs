using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;

namespace Core.Utilities.Interceptors
{
    public class MethodInterception:MethodInterceptionBaseAttribute
    {
        protected virtual void OnBefore(IInvocation invocation) { }
        protected virtual void OnAfter(IInvocation invocation) { }
        protected virtual void OnException(IInvocation invocation, Exception e) { }
        protected virtual void OnSucces(IInvocation invocation) { }

        public override void Intercept(IInvocation invocation)
        {
            var isSucccess = true;
            OnBefore(invocation);
            try
            {
                invocation.Proceed();
            }
            catch (Exception e)
            {
                isSucccess = false;
                OnException(invocation, e);
                throw;
            }
            finally
            {
                if (isSucccess)
                {
                    OnSucces(invocation);
                }
            }
            OnAfter(invocation);
        }
    }
}
