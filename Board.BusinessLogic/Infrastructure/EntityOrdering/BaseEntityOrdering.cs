#define DEBUG_REORDERING

using Board.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.BusinessLogic.Infrastructure.EntityOrdering
{
    public abstract class BaseEntityOrdering<TModel>
        where TModel : IOrderedModel
    {
        private const long ORDER_STEP = 1024;
        private const int REORDER_BATCH = 50;

        protected abstract long GetLastOrderValue(int groupId);
        protected abstract long GetFirstOrderValue(int groupId);
        protected abstract int GetModelCount(int groupId);
        protected abstract List<TModel> GetOrderedModels(int groupId, int skip, int take);
        protected abstract void UpdateItems(int groupId, List<TModel> updatedItems);

        public void SetNewOrder(TModel model, int desiredPosition, int groupId)
        {
            #if DEBUG_REORDERING
            System.Diagnostics.Debug.WriteLine($"*** Start reordering, class: {this.GetType().Name}, group {groupId}, model ID: {model.Id}, desired position: {desiredPosition}");
            #endif

            // New first item - trivial
            if (desiredPosition == 0)
            {
                long firstOrder = GetFirstOrderValue(groupId);
                firstOrder -= ORDER_STEP;
                model.Order = firstOrder;

                #if DEBUG_REORDERING
                System.Diagnostics.Debug.WriteLine($"Desired position is zero, new order: {firstOrder}");
                #endif

                return;
            }

            var count = GetModelCount(groupId);

            // New last item - trivial
            if (desiredPosition >= count)
            {
                long lastOrder = GetLastOrderValue(groupId);
                lastOrder += ORDER_STEP;
                model.Order = lastOrder;

                #if DEBUG_REORDERING
                System.Diagnostics.Debug.WriteLine($"Desired position is after last, new order: {lastOrder}");
                #endif

                return;
            }

            #if DEBUG_REORDERING
            System.Diagnostics.Debug.WriteLine($"Desired position is middle, picking two neighbors");
            #endif

            // Pick two items, between which new one is being inserted
            List<TModel> surroundingItems = GetOrderedModels(groupId, desiredPosition - 1, 2);
            if (surroundingItems.Count != 2 || surroundingItems[0] is null || surroundingItems[1] is null)
                throw new InvalidOperationException("Fatal error: expected exactly two non-null items returned");

            #if DEBUG_REORDERING
            System.Diagnostics.Debug.WriteLine($"Neighbor orders are: {surroundingItems[0].Order} and {surroundingItems[1].Order}");
            #endif

            long midOrder = (surroundingItems[0].Order + surroundingItems[1].Order) / 2;

            // There is place in between two items
            if (midOrder > surroundingItems[0].Order && midOrder < surroundingItems[1].Order)
            {
                #if DEBUG_REORDERING
                System.Diagnostics.Debug.WriteLine($"Mid order will be used: {midOrder}");
                #endif

                model.Order = midOrder;
                return;
            }

            // If there is no place in between two items, reordering is required

            #if DEBUG_REORDERING
            System.Diagnostics.Debug.WriteLine($"No mid order can be used, reordering...");
            #endif

            int taken = desiredPosition + 1;
            long newOrder = 0;
            List<TModel> updatedItems = GetOrderedModels(groupId, 0, taken);

            // First, reorder all items up to one prior desired position

            for (int i = 0; i < updatedItems.Count - 1; i++)
            {
                #if DEBUG_REORDERING
                System.Diagnostics.Debug.WriteLine($"Reordering item {updatedItems[i].Id} from {updatedItems[i].Order} to {newOrder}");
                #endif

                updatedItems[i].Order = newOrder;
                newOrder += ORDER_STEP;
            }

            #if DEBUG_REORDERING
            System.Diagnostics.Debug.WriteLine($"New item's order will be {newOrder}, now continuing reordering...");
            #endif

            // Here's a place for our new model
            model.Order = newOrder;
            newOrder += ORDER_STEP;

            // And we need to update order of one post desired position too
            #if DEBUG_REORDERING
            System.Diagnostics.Debug.WriteLine($"Reordering item {updatedItems[^1].Id} from {updatedItems[^1].Order} to {newOrder}");
            #endif

            updatedItems[^1].Order = newOrder;
            newOrder += ORDER_STEP;

            // Now we have reorder next items, but only if it is necessary
            bool stillReordering = true;

            #if DEBUG_REORDERING
            System.Diagnostics.Debug.WriteLine($"Now continuing reordering...");
            #endif

            do
            {
                List<TModel> batch = GetOrderedModels(groupId, taken, REORDER_BATCH);
                taken += REORDER_BATCH;

                if (batch.Count == 0)
                {
                    // No more items to reorder

                    #if DEBUG_REORDERING
                    System.Diagnostics.Debug.WriteLine($"No more items to reorder...");
                    #endif

                    stillReordering = false;
                }
                else
                {
                    for (int i = 0; i < batch.Count; i++)
                    {
                        if (batch[i].Order >= newOrder)
                        {
                            // We can stop reordering here (orders are already equal or greater than one we're trying to set

                            #if DEBUG_REORDERING
                            System.Diagnostics.Debug.WriteLine($"Reached item with Id {batch[i].Id} with order {batch[i].Order}, which is greater or equal than {newOrder}, so reordering can finish now.");
                            #endif

                            stillReordering = false;
                            break;
                        }
                        else
                        {
                            #if DEBUG_REORDERING
                            System.Diagnostics.Debug.WriteLine($"Reordering item {batch[i].Id} from {batch[i].Order} to {newOrder}");
                            #endif

                            batch[i].Order = newOrder;
                            newOrder += ORDER_STEP;
                            updatedItems.Add(batch[i]);
                        }
                    }
                }
            }
            while (stillReordering);

            #if DEBUG_REORDERING
            System.Diagnostics.Debug.WriteLine($"Now persisting changes to {updatedItems.Count} item(s)...");
            #endif

            // We need to persist new orders
            UpdateItems(groupId, updatedItems);

            return;
        }
    }
}
