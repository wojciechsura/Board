using AutoMapper;
using Board.BusinessLogic.Services.Config;
using Board.BusinessLogic.Services.DatabaseBuilder;
using Board.BusinessLogic.Services.Document;
using Board.BusinessLogic.Services.EventBus;
using Board.BusinessLogic.Services.FilesystemBuilder;
using Board.BusinessLogic.Services.Paths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Lifetime;

namespace Board.BusinessLogic.Dependencies
{
    public static class Configuration
    {
        private static bool isConfigured = false;

        public static void Configure(IUnityContainer container)
        {
            if (isConfigured)
                return;
            isConfigured = true;

            // Register services
            container.RegisterType<IEventBus, EventBus>(new ContainerControlledLifetimeManager());
            container.RegisterType<IPathService, PathService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IFilesystemBuilderService, FilesystemBuilderService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IDatabaseBuilderService, DatabaseBuilderService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IDocumentFactory, DocumentFactory>(new ContainerControlledLifetimeManager());
            container.RegisterType<IConfigurationService, ConfigurationService>(new ContainerControlledLifetimeManager());

            // Register mapper
            var mapperConfig = new MapperConfiguration(config =>
            {
                config.AddMaps("Board.BusinessLogic");
            });
            var mapper = mapperConfig.CreateMapper();
            container.RegisterInstance<IMapper>(mapper);
        }
    }
}
