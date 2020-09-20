using Caliburn.Micro;
using CoreNotes.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace CoreNotes
{
    /// <summary>
    /// 
    /// </summary>
    public class Bootstrapper : BootstrapperBase
    {
        /// <summary>
        /// 
        /// </summary>
        private SimpleContainer _container = new SimpleContainer();
        
        /// <summary>
        /// 
        /// </summary>
        public Bootstrapper()
        {
            Initialize();
        }

        /// <summary>
        /// Creates a new DI container, retrieves all classes who's name contains 'ViewModel'
        /// </summary>
        protected override void Configure()
        {
            _container.Instance(_container);
            _container
                .Singleton<IWindowManager, WindowManager>()
                .Singleton<IEventAggregator, EventAggregator>();

            GetType().Assembly.GetTypes()
                .Where(type => type.IsClass)
                .Where(type => type.Name.EndsWith("ViewModel"))
                .ToList()
                .ForEach(viewModelType => _container.RegisterPerRequest(
                    viewModelType, viewModelType.ToString(), viewModelType));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<CoreNotesViewModel>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }
    }
}
