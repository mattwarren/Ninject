using System;
using System.Reflection;
using Xunit;

namespace Ninject.Tests.Integration
{
    public class DynamicMethodInjectionFactory_Invalid_IL
    {
        [Fact]
        public void InvalidProgramExceptionDueToInvalidIL()
        {
            //See https://github.com/ninject/Ninject/issues/175
            var kernel = new StandardKernel();

            // Setup the regular Binding
            kernel.Bind<ILogging>().To<MockLogging>();
            // Setup the Func<T> ToMethod() Binding (that causes the error)
            kernel.Bind<Func<ILogging>>().ToMethod(ctx => () => new MockLogging());

            // This works
            var logger = kernel.Get<ILogging>();
            logger.Debug("Via ILogging: SUCCESSFULLY Setup the Ninject Kernel");
            Console.WriteLine();

            // This blows up!!!
            var getLogger = kernel.Get<Func<ILogging>>();
            getLogger().Debug("Via Func<ILogging>: SUCCESSFULLY Setup the Ninject Kernel");
            Console.WriteLine();
        }

        [Fact]
        public void NoInvalidProgramExceptionInReflectionBasedInjectionMode()
        {
            INinjectSettings settings = new NinjectSettings
            {
                UseReflectionBasedInjection = true,     // disable code generation for partial trust
            };
            var kernel = new StandardKernel(settings);

            // Setup the regular Binding
            kernel.Bind<ILogging>().To<MockLogging>();
            // Setup the Func<T> ToMethod() Binding (that causes the error)
            kernel.Bind<Func<ILogging>>().ToMethod(ctx => () => new MockLogging());
            
            // This works
            var logger = kernel.Get<ILogging>();
            logger.Debug("Via ILogging: SUCCESSFULLY Setup the Ninject Kernel");
            Console.WriteLine();

            // This always works (in Reflection-based Injection mode!!!
            var getLogger = kernel.Get<Func<ILogging>>();
            getLogger().Debug("Via Func<ILogging>: SUCCESSFULLY Setup the Ninject Kernel");
            Console.WriteLine();
        }
    }

    public interface ILogging
    {
        void Debug(string message);
    }

    public class MockLogging : ILogging
    {
        public void Debug(string message)
        {
            Console.WriteLine("Mock Debug: [{0}]", message);
        }
    }
}

