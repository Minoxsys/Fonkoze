using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MvcContrib.TestHelper.MockFactories;
using System.Reflection;
using System.Linq.Expressions;

namespace Tests.Utils
{

    /// <summary>
    /// Creates mock objects using Moq.
    /// </summary>
    internal class MoqFactory : IMockFactory
    {
        private static readonly Type _mockOpenType;
        private static readonly Exception _loadException;

        /// <summary>
        /// Grabs references to static types.
        /// </summary>
        static MoqFactory()
        {
            try
            {
                _mockOpenType = typeof(Moq.Mock<>);

                if (_mockOpenType == null)
                {
                    throw new InvalidOperationException("Unable to find Type Moq.Mock<T> in assembly " + _mockOpenType.Assembly.Location);
                }
            }
            catch (Exception ex)
            {
                _loadException = ex;
            }
        }

        /// <summary>
        /// Creates a new factory.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if Moq isn't available.</exception>
        public MoqFactory()
        {
            if (_mockOpenType == null)
            {
                throw new InvalidOperationException("Unable to create a proxy for Moq.", _loadException);
            }
        }

        /// <summary>
        /// Creates a new dynamic mock.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IMockProxy<T> DynamicMock<T>() where T : class
        {
            return new MoqProxy<T>(_mockOpenType.MakeGenericType(typeof(T)));
        }
    }

    /// <summary>
    /// Runtime proxy for a Moq proxy. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class MoqProxy<T> : IMockProxy<T>
    {
        private readonly Type _mockType;
        private readonly PropertyInfo _objectProperty;
        private readonly object _instance;

        /// <summary>
        /// Gets the object. 
        /// </summary>
        public T Object
        {
            get
            {
                return (T)_objectProperty.GetValue(_instance, null);

            }
        }

        /// <summary>
        /// Creates a new proxy. 
        /// </summary>
        /// <param name="mockType"></param>
        public MoqProxy(Type mockType)
        {
            _mockType = mockType;
            _instance = Activator.CreateInstance(_mockType);
            _objectProperty = mockType.GetProperty("Object", _mockType);
        }

        private MethodInfo GetSetupMethod<TResult>()
        {
            var openSetupMethod = _mockType.GetMethods().First(m => m.IsGenericMethod && m.Name == "Setup");
            return openSetupMethod.MakeGenericMethod(typeof(TResult));
        }

        /// <summary>
        /// Sets up the specified return value for the specified method call or property. 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression"></param>
        /// <param name="result"></param>
        public void ReturnFor<TResult>(Expression<Func<T, TResult>> expression, TResult result)
        {
            var setupMethod = GetSetupMethod<TResult>();
            var setup = setupMethod.Invoke(_instance, new object[] { expression });
            var returnsMethod = setup.GetType().GetMethod("Returns", new[] { typeof(TResult) });
            returnsMethod.Invoke(setup, new object[] { result });
        }

        /// <summary>
        /// Sets up a callback function for the specified method call or property.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression"></param>
        /// <param name="callback"></param>
        public void CallbackFor<TResult>(Expression<Func<T, TResult>> expression, Func<TResult> callback)
        {
            var setupMethod = GetSetupMethod<TResult>();
            var setup = setupMethod.Invoke(_instance, new object[] { expression });
            var returnsMethod = setup.GetType().GetMethod("Returns", new[] { typeof(Func<TResult>) });
            returnsMethod.Invoke(setup, new object[] { callback });
        }

        /// <summary>
        /// Sets up normal get/set property behavior. 
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="expression"></param>
        public void SetupProperty<TProperty>(Expression<Func<T, TProperty>> expression)
        {
            var openSetupMethod = _mockType.GetMethods().First(m => m.Name == "SetupProperty" && m.GetParameters().Length == 1);
            var setupMethod = openSetupMethod.MakeGenericMethod(typeof(TProperty));
            setupMethod.Invoke(_instance, new object[] { expression });
        }
    }
}
