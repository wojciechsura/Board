using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace Board.Dependencies
{
    public static class Container
    {
        private static Lazy<UnityContainer> container = new Lazy<UnityContainer>(() =>
        {
            var container = new UnityContainer();

            // Optional: enable container diagnostics to resolve
            // injection problems
            //
            // container.AddExtension(new Diagnostic());

            return container;
        });

        public static IUnityContainer Instance
        {
            get
            {
                return container.Value;
            }
        }
    }
}
