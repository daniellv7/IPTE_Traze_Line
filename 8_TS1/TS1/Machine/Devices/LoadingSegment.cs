using Ipte.Machine.Configuration;
using System;
using System.Threading;
using Ipte.Machine.Config;

namespace Ipte.Machine.Devices
{
    public class LoadingSegment : SimpleSegment
    {
        public LoadingSegment(string zoneId, string laneId)
            : base(zoneId, laneId)
        {
        }

        public override void Load()
        {
            Thread.Sleep(5000);
            Log("New product appeared");
            Product = new Panel()
            {
                IsFailed = false,
                SerialNumber = Guid.NewGuid().ToString(),
                RecipePath = Recipe.ShortNameToPath(Settings.ActiveRecipe)
            };

            var recipe = RecipeBuffer.GetRecipe(Product.RecipePath);
            for (int i = 0; i < recipe.Settings.ModuleCount; i++)
            {
                Product.Modules.Add(new Module()
                {
                    IsFailed = false,
                    SerialNumber = Guid.NewGuid().ToString(),
                });
            }

            OnItemTransferIn(Product.SerialNumber, LaneId);
        }
    }
}
