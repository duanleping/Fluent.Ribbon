﻿namespace Fluent.Collections
{
    using System;
    using System.Collections.Specialized;

    /// <summary>
    /// asdfasdfasdf
    /// </summary>
    public class CollectionSyncHelperWithLogicalTreeSupport<TItem>
    {
        /// <summary>
        /// asdfasdf
        /// </summary>
        public CollectionSyncHelperWithLogicalTreeSupport(ItemCollectionWithLogicalTreeSupport<TItem> source, ItemCollectionWithLogicalTreeSupport<TItem> target)
        {
            this.Source = source ?? throw new ArgumentNullException(nameof(source));
            this.Target = target ?? throw new ArgumentNullException(nameof(target));

            this.Source.CollectionChanged += this.SourceOnCollectionChanged;
        }

        /// <summary>
        /// The source collection.
        /// </summary>
        public ItemCollectionWithLogicalTreeSupport<TItem> Source { get; }

        /// <summary>
        /// The target collection.
        /// </summary>
        public ItemCollectionWithLogicalTreeSupport<TItem> Target { get; }

        /// <summary>
        /// asdf
        /// </summary>
        public void TransferItemsToTarget()
        {
            this.Target.Clear();

            this.Source.ReleaseLogicalOwnership();

            foreach (var item in this.Source)
            {
                this.Target.Add(item);
            }
        }

        /// <summary>
        /// asdf
        /// </summary>
        public void TransferOwnershipToSource()
        {
            this.Source.AquireLogicalOwnership();
            this.Target.ReleaseLogicalOwnership();
        }

        private void SourceOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (var i = 0; i < e.NewItems.Count; i++)
                    {
                        var item = (TItem)e.NewItems[i];
                        this.Source.Parent.RemoveLogicalChild(item);
                        this.Target.Insert(e.NewStartingIndex + i, item);
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        this.Source.Parent.RemoveLogicalChild(item);
                        this.Target.Remove((TItem)item);
                    }

                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (var item in e.OldItems)
                    {
                        this.Source.Parent.RemoveLogicalChild(item);
                        this.Target.Remove((TItem)item);
                    }

                    foreach (var item in e.NewItems)
                    {
                        this.Source.Parent.RemoveLogicalChild(item);
                        this.Target.Add((TItem)item);
                    }

                    break;

                case NotifyCollectionChangedAction.Reset:
                    this.Target.Clear();

                    foreach (var item in this.Source)
                    {
                        this.Source.Parent.AddLogicalChild(item);
                        this.Target.Add(item);
                    }

                    break;
            }
        }
    }
}