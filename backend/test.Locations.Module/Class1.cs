using Abp.AspNetCore;
using Abp.Dependency;
using Abp.Modules;
using Abp.AutoMapper;
using Abp.AspNetCore;
using Abp.AspNetCore.Configuration;
using Shesha.Modules;
using Shesha;
using Shesha.Web.FormsDesigner;
using System.Reflection;

namespace test.Locations.Module
{
    [DependsOn(
    typeof(SheshaCoreModule),
    typeof(AbpAspNetCoreModule)
)]
    public class LocationModule : SheshaModule
    {
        public override SheshaModuleInfo ModuleInfo => new SheshaModuleInfo("Location")
        {
            
            Name = "Location Module",
            FriendlyName = "Module Exteninds Sites to locations - Property, Building, Floor, locations",
            Publisher = "Boxfusion"
        };

        public override async Task<bool> InitializeConfigurationAsync()
        {
            // Import any configuration embeded as resources in this assembly on application start-up.
            return await ImportConfigurationAsync();
        }

        public override void Initialize()
        {
            var thisAssembly = Assembly.GetExecutingAssembly();

            // Register IoC services
            IocManager.RegisterAssemblyByConvention(thisAssembly);

            // Scan the assembly for classes which inherit from AutoMapper.Profile
            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                cfg => cfg.AddMaps(thisAssembly)
                );
        }


        public override void PreInitialize()
        {
            var thisAssembly = Assembly.GetExecutingAssembly();

            // Create Controllers for all AppServices classes in the assembly
            Configuration.Modules.AbpAspNetCore().CreateControllersForAppServices(
                typeof(SheshaFormsDesignerModule).Assembly,
                moduleName: "Location",      // Specifies the module name to use for the controller route
                useConventionalHttpVerbs: true);
        }
    }
}
