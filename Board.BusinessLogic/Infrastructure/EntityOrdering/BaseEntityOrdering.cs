#define DEBUG_REORDERING

using Board.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.BusinessLogic.Infrastructure.EntityOrdering
{
    /// <summary>
    /// Handles persisting item order in the database.
    /// </summary>
    /// <remarks>
    /// If items are not soft-deletable, implementations are straightforward.
    /// This class can also work with soft-deletable items, with the following
    /// required changes to the implementations:
    /// 
    /// <list type="bullet">
    ///     <item><code>GetLastOrderValue</code> must return last order value regardless of soft-deletion;</item>
    ///     <item><code>GetFirstOrderValue</code> must return first order value regardless of soft-deletion;</item>
    ///     <item><code>GetModelCount</code> must return count of <strong>not deleted</strong> items;</item>
    ///     <item><code>GetOrderedModels</code> must return items regardless of soft-deletion;</item>
    ///     <item><code>GetModelWithSuccessor</code> must return (index)-th <strong>not deleted</strong> item and its immediate successor (regardless of soft-deletion)</item>
    ///     <item><code>UpdateItems</code> should update all items, regardless of soft-deletion.</item>
    /// </list>
    /// 
    /// Such implementation will also update softly-deleted items' positions, so
    /// that if they are undeleted, they will return to (more or less) their
    /// previous location.
    /// </remarks>
    /// <typeparam name="TModel"></typeparam>
    public abstract class BaseEntityOrdering<TModel>
        where TModel : IOrderedModel
    {
        private const long ORDER_STEP = 1024;
        private const int REORDER_BATCH = 50;

        /// <summary>Returns order value of last item ( = greatest order value)</summary>
        protected abstract long GetLastOrderValue(int groupId);
        /// <summary>Returns order value of first item ( = smallest order value)</summary>
        protected abstract long GetFirstOrderValue(int groupId);
        /// <summary>Returns count of ordered items</summary>
        protected abstract int GetModelCount(int groupId);
        /// <summary>Returns a list of models ordered by the order field</summary>
        protected abstract List<TModel> GetOrderedModels(int groupId, int skip, int take);
        /// <summary>Returns model at index <paramref name="index"/> and its immediate successor.</summary>
        protected abstract (TModel indexModel, TModel nextModel) GetModelWithSuccessor(int groupId, int index);
        /// <summary>Persists changes made to given list of models.</summary>
        protected abstract void UpdateItems(int groupId, List<TModel> updatedItems);

        /// <summary>
        /// Sets order field of given model. If neccessary, reorders existing items
        /// so that new order value can be properly generated.
        /// </summary>        
        public void SetNewOrder(TModel model, int desiredPosition, int groupId)
        {
            #if DEBUG_REORDERING
            System.Diagnostics.Debug.WriteLine($"*** Start reordering, class: {this.GetType().Name}, group {groupId}, model ID: {model.Id}, desired position: {desiredPosition}");
            #endif

            // New first item - trivial
            if (desiredPosition == 0)
            {
                long? firstOrder = GetFirstOrderValue(groupId);

                if (firstOrder != null)
                {
                    firstOrder -= ORDER_STEP;
                    model.Order = firstOrder.Value;
                }
                else
                {
                    firstOrder = 0;
                    model.Order = firstOrder.Value;
                }

                #if DEBUG_REORDERING
                System.Diagnostics.Debug.WriteLine($"Desired position is zero, new order: {firstOrder}");
                #endif

                return;
            }

            var count = GetModelCount(groupId);

            // New last item - trivial
            if (desiredPosition >= count)
            {
                long? lastOrder = GetLastOrderValue(groupId);
                if (lastOrder != null)
                {
                    lastOrder += ORDER_STEP;
                    model.Order = lastOrder.Value;
                }
                else
                {
                    lastOrder = 0;
                    model.Order = lastOrder.Value;
                }

                #if DEBUG_REORDERING
                System.Diagnostics.Debug.WriteLine($"Desired position is after last, new order: {lastOrder}");
                #endif

                return;
            }

            #if DEBUG_REORDERING
            System.Diagnostics.Debug.WriteLine($"Desired position is middle, picking two neighbors");
#endif

            // Pick two items, between which new one is being inserted
            (TModel indexModel, TModel successorModel) = GetModelWithSuccessor(groupId, desiredPosition - 1);

            #if DEBUG_REORDERING
            System.Diagnostics.Debug.WriteLine($"Neighbor orders are: {indexModel.Order} and {successorModel.Order}");
            #endif

            long midOrder = (indexModel.Order + successorModel.Order) / 2;

            // There is place in between two items
            if (midOrder > indexModel.Order && midOrder < successorModel.Order)
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
