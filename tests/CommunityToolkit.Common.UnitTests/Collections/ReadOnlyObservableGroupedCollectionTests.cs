// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Common.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Collections
{
    [TestClass]
    public class ReadOnlyObservableGroupedCollectionTests
    {
        [TestCategory("Collections")]
        [TestMethod]
        public void Ctor_WithEmptySource_ShoudInitializeObject()
        {
            var source = new ObservableGroupedCollection<string, int>();
            var readOnlyGroup = new ReadOnlyObservableGroupedCollection<string, int>(source);

            Assert.AreEqual(readOnlyGroup.Count, 0);
            CollectionAssert.AreEqual(readOnlyGroup, Array.Empty<int>());
        }

        [TestCategory("Collections")]
        [TestMethod]
        public void Ctor_WithObservableGroupedCollection_ShoudInitializeObject()
        {
            var groups = new List<IGrouping<string, int>>
            {
                new IntGroup("A", new[] { 1, 3, 5 }),
                new IntGroup("B", new[] { 2, 4, 6 }),
            };
            var source = new ObservableGroupedCollection<string, int>(groups);
            var readOnlyGroup = new ReadOnlyObservableGroupedCollection<string, int>(source);

            Assert.AreEqual(readOnlyGroup.Count, 2);

            Assert.AreEqual(readOnlyGroup.ElementAt(0).Key, "A");
            CollectionAssert.AreEquivalent(readOnlyGroup.ElementAt(0), new[] { 1, 3, 5 });

            Assert.AreEqual(readOnlyGroup.ElementAt(1).Key, "B");
            CollectionAssert.AreEquivalent(readOnlyGroup.ElementAt(1), new[] { 2, 4, 6 });
        }

        [TestCategory("Collections")]
        [TestMethod]
        public void Ctor_WithListOfIGroupingSource_ShoudInitializeObject()
        {
            var source = new List<IGrouping<string, int>>
            {
                new IntGroup("A", new[] { 1, 3, 5 }),
                new IntGroup("B", new[] { 2, 4, 6 }),
            };
            var readOnlyGroup = new ReadOnlyObservableGroupedCollection<string, int>(source);

            Assert.AreEqual(readOnlyGroup.Count, 2);

            Assert.AreEqual(readOnlyGroup.ElementAt(0).Key, "A");
            CollectionAssert.AreEquivalent(readOnlyGroup.ElementAt(0), new[] { 1, 3, 5 });

            Assert.AreEqual(readOnlyGroup.ElementAt(1).Key, "B");
            CollectionAssert.AreEquivalent(readOnlyGroup.ElementAt(1), new[] { 2, 4, 6 });
        }

        [TestCategory("Collections")]
        [TestMethod]
        public void Ctor_WithListOfReadOnlyObservableGroupSource_ShoudInitializeObject()
        {
            var source = new List<ReadOnlyObservableGroup<string, int>>
            {
                new ReadOnlyObservableGroup<string, int>("A", new[] { 1, 3, 5 }),
                new ReadOnlyObservableGroup<string, int>("B", new[] { 2, 4, 6 }),
            };
            var readOnlyGroup = new ReadOnlyObservableGroupedCollection<string, int>(source);

            Assert.AreEqual(readOnlyGroup.Count, 2);

            Assert.AreEqual(readOnlyGroup.ElementAt(0).Key, "A");
            CollectionAssert.AreEqual(readOnlyGroup.ElementAt(0), new[] { 1, 3, 5 });

            Assert.AreEqual(readOnlyGroup.ElementAt(1).Key, "B");
            CollectionAssert.AreEqual(readOnlyGroup.ElementAt(1), new[] { 2, 4, 6 });
        }

        [TestCategory("Collections")]
        [TestMethod]
        public void IListImplementation_Properties_ShoudReturnExpectedValues()
        {
            var groups = new List<IGrouping<string, int>>
            {
                new IntGroup("A", new[] { 1, 3, 5 }),
                new IntGroup("B", new[] { 2, 4, 6 }),
            };
            var source = new ObservableGroupedCollection<string, int>(groups);
            var readOnlyGroup = new ReadOnlyObservableGroupedCollection<string, int>(source);
            var list = (IList)readOnlyGroup;

            Assert.AreEqual(list.Count, 2);

            var group0 = (ReadOnlyObservableGroup<string, int>)list[0];

            Assert.AreEqual(group0.Key, "A");
            CollectionAssert.AreEqual(group0, new[] { 1, 3, 5 });

            var group1 = (ReadOnlyObservableGroup<string, int>)list[1];

            Assert.AreEqual(group1.Key, "B");
            CollectionAssert.AreEqual(group1, new[] { 2, 4, 6 });

            Assert.IsNotNull(list.SyncRoot);
            Assert.IsTrue(list.IsFixedSize);
            Assert.IsTrue(list.IsReadOnly);
            Assert.IsFalse(list.IsSynchronized);
        }

        [TestCategory("Collections")]
        [TestMethod]
        public void IListImplementation_MutableMethods_ShoudThrow()
        {
            var groups = new List<IGrouping<string, int>>
            {
                new IntGroup("A", new[] { 1, 3, 5 }),
                new IntGroup("B", new[] { 2, 4, 6 }),
            };
            var source = new ObservableGroupedCollection<string, int>(groups);
            var readOnlyGroup = new ReadOnlyObservableGroupedCollection<string, int>(source);
            var list = (IList)readOnlyGroup;

            var testGroup = new ReadOnlyObservableGroup<string, int>("test", new ObservableCollection<int>());

            Assert.ThrowsException<NotSupportedException>(() => list.Add(testGroup));
            Assert.ThrowsException<NotSupportedException>(() => list.Clear());
            Assert.ThrowsException<NotSupportedException>(() => list.Insert(2, testGroup));
            Assert.ThrowsException<NotSupportedException>(() => list.Remove(testGroup));
            Assert.ThrowsException<NotSupportedException>(() => list.RemoveAt(2));
            Assert.ThrowsException<NotSupportedException>(() => list[2] = testGroup);

            var array = new object[5];

            // This line should not throw
            list.CopyTo(array, 0);
        }

        [TestCategory("Collections")]
        [DataTestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        public void IListImplementation_IndexOf_ShoudReturnExpectedValue(int groupIndex)
        {
            var groups = new List<IGrouping<string, int>>
            {
                new IntGroup("A", new[] { 1, 3, 5 }),
                new IntGroup("B", new[] { 2, 4, 6 }),
                new IntGroup("C", new[] { 7, 8, 9 }),
            };
            var source = new ObservableGroupedCollection<string, int>(groups);
            var readOnlyGroup = new ReadOnlyObservableGroupedCollection<string, int>(source);
            var list = (IList)readOnlyGroup;

            var groupToSearch = groupIndex >= 0 ? list[groupIndex] : null;

            var index = list.IndexOf(groupToSearch);

            Assert.AreEqual(index, groupIndex);
        }

        [TestCategory("Collections")]
        [DataTestMethod]
        [DataRow(-1, false)]
        [DataRow(0, true)]
        [DataRow(1, true)]
        public void IListImplementation_Contains_ShoudReturnExpectedValue(int groupIndex, bool expectedResult)
        {
            var groups = new List<IGrouping<string, int>>
            {
                new IntGroup("A", new[] { 1, 3, 5 }),
                new IntGroup("B", new[] { 2, 4, 6 }),
            };
            var source = new ObservableGroupedCollection<string, int>(groups);
            var readOnlyGroup = new ReadOnlyObservableGroupedCollection<string, int>(source);
            var list = (IList)readOnlyGroup;

            var groupToSearch = groupIndex >= 0 ? list[groupIndex] : null;

            var result = list.Contains(groupToSearch);

            Assert.AreEqual(result, expectedResult);
        }

        [TestCategory("Collections")]
        [DataTestMethod]
        [DataRow(0, 0)]
        [DataRow(3, 3)]
        public void AddGroupInSource_ShouldAddGroup(int sourceInitialItemsCount, int expectedInsertionIndex)
        {
            NotifyCollectionChangedEventArgs collectionChangedEventArgs = null;
            var collectionChangedEventsCount = 0;
            var isCountPropertyChangedEventRaised = false;
            var itemsList = new[] { 1, 2, 3 };
            var source = new ObservableGroupedCollection<string, int>();
            for (var i = 0; i < sourceInitialItemsCount; i++)
            {
                source.Add(new ObservableGroup<string, int>($"group {i}", Enumerable.Empty<int>()));
            }

            var readOnlyGroup = new ReadOnlyObservableGroupedCollection<string, int>(source);
            ((INotifyCollectionChanged)readOnlyGroup).CollectionChanged += (s, e) =>
            {
                collectionChangedEventArgs = e;
                collectionChangedEventsCount++;
            };
            ((INotifyPropertyChanged)readOnlyGroup).PropertyChanged += (s, e) => isCountPropertyChangedEventRaised = isCountPropertyChangedEventRaised || e.PropertyName == nameof(readOnlyGroup.Count);

            source.Add(new ObservableGroup<string, int>("Add", itemsList));

            var expectedReadOnlyGroupCount = sourceInitialItemsCount + 1;

            Assert.AreEqual(readOnlyGroup.Count, expectedReadOnlyGroupCount);

            Assert.AreEqual(readOnlyGroup.Last().Key, "Add");
            CollectionAssert.AreEquivalent(readOnlyGroup.Last(), itemsList);

            Assert.IsTrue(isCountPropertyChangedEventRaised);
            Assert.IsNotNull(collectionChangedEventArgs);
            Assert.AreEqual(collectionChangedEventsCount, 1);

            bool isAddEventValid = IsAddEventValid(collectionChangedEventArgs, itemsList, expectedInsertionIndex);

            Assert.IsTrue(isAddEventValid);
        }

        [TestCategory("Collections")]
        [DataTestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        public void InsertGroupInSource_ShouldAddGroup(int insertionIndex)
        {
            NotifyCollectionChangedEventArgs collectionChangedEventArgs = null;
            var collectionChangedEventsCount = 0;
            var isCountPropertyChangedEventRaised = false;
            var itemsList = new[] { 1, 2, 3 };
            var source = new ObservableGroupedCollection<string, int>
            {
                new ObservableGroup<string, int>("Group0", new[] { 10, 20, 30 }),
                new ObservableGroup<string, int>("Group1", new[] { 40, 50, 60 })
            };
            var readOnlyGroup = new ReadOnlyObservableGroupedCollection<string, int>(source);
            ((INotifyCollectionChanged)readOnlyGroup).CollectionChanged += (s, e) =>
            {
                collectionChangedEventArgs = e;
                collectionChangedEventsCount++;
            };
            ((INotifyPropertyChanged)readOnlyGroup).PropertyChanged += (s, e) => isCountPropertyChangedEventRaised = isCountPropertyChangedEventRaised || e.PropertyName == nameof(readOnlyGroup.Count);

            source.Insert(insertionIndex, new ObservableGroup<string, int>("Add", itemsList));

            Assert.AreEqual(readOnlyGroup.Count, 3);

            Assert.AreEqual(readOnlyGroup.ElementAt(insertionIndex).Key, "Add");
            CollectionAssert.AreEquivalent(readOnlyGroup.ElementAt(insertionIndex), itemsList);

            Assert.IsTrue(isCountPropertyChangedEventRaised);
            Assert.IsNotNull(collectionChangedEventArgs);
            Assert.AreEqual(collectionChangedEventsCount, 1);

            bool isAddEventValid = IsAddEventValid(collectionChangedEventArgs, itemsList, addIndex: insertionIndex);

            Assert.IsTrue(isAddEventValid);
        }

        [TestCategory("Collections")]
        [TestMethod]
        public void RemoveGroupInSource_ShoudRemoveGroup()
        {
            NotifyCollectionChangedEventArgs collectionChangedEventArgs = null;
            var collectionChangedEventsCount = 0;
            var isCountPropertyChangedEventRaised = false;
            var aItemsList = new[] { 1, 2, 3 };
            var bItemsList = new[] { 2, 4, 6 };
            var groups = new List<IGrouping<string, int>>
            {
                new IntGroup("A", aItemsList),
                new IntGroup("B", bItemsList),
            };
            var source = new ObservableGroupedCollection<string, int>(groups);
            var readOnlyGroup = new ReadOnlyObservableGroupedCollection<string, int>(source);
            ((INotifyCollectionChanged)readOnlyGroup).CollectionChanged += (s, e) =>
            {
                collectionChangedEventArgs = e;
                collectionChangedEventsCount++;
            };
            ((INotifyPropertyChanged)readOnlyGroup).PropertyChanged += (s, e) => isCountPropertyChangedEventRaised = isCountPropertyChangedEventRaised || e.PropertyName == nameof(readOnlyGroup.Count);

            source.RemoveAt(1);

            Assert.AreEqual(readOnlyGroup.Count, 1);

            Assert.AreEqual(readOnlyGroup.ElementAt(0).Key, "A");
            CollectionAssert.AreEquivalent(readOnlyGroup.ElementAt(0), aItemsList);

            Assert.IsTrue(isCountPropertyChangedEventRaised);
            Assert.IsNotNull(collectionChangedEventArgs);
            Assert.AreEqual(collectionChangedEventsCount, 1);

            bool isRemoveEventValid = IsRemoveEventValid(collectionChangedEventArgs, bItemsList, 1);

            Assert.IsTrue(isRemoveEventValid);
        }

        [TestCategory("Collections")]
        [DataTestMethod]
        [DataRow(1, 0)]
        [DataRow(0, 1)]
        public void MoveGroupInSource_ShoudMoveGroup(int oldIndex, int newIndex)
        {
            NotifyCollectionChangedEventArgs collectionChangedEventArgs = null;
            var collectionChangedEventsCount = 0;
            var isCountPropertyChangedEventRaised = false;
            var aItemsList = new[] { 1, 2, 3 };
            var bItemsList = new[] { 2, 4, 6 };
            var groups = new List<IGrouping<string, int>>
            {
                new IntGroup("A", aItemsList),
                new IntGroup("B", bItemsList),
            };
            var source = new ObservableGroupedCollection<string, int>(groups);
            var readOnlyGroup = new ReadOnlyObservableGroupedCollection<string, int>(source);
            ((INotifyCollectionChanged)readOnlyGroup).CollectionChanged += (s, e) =>
            {
                collectionChangedEventArgs = e;
                collectionChangedEventsCount++;
            };
            ((INotifyPropertyChanged)readOnlyGroup).PropertyChanged += (s, e) => isCountPropertyChangedEventRaised = isCountPropertyChangedEventRaised || e.PropertyName == nameof(readOnlyGroup.Count);

            source.Move(oldIndex, newIndex);

            Assert.AreEqual(readOnlyGroup.Count, 2);

            Assert.AreEqual(readOnlyGroup.ElementAt(0).Key, "B");
            CollectionAssert.AreEquivalent(readOnlyGroup.ElementAt(0), bItemsList);

            Assert.AreEqual(readOnlyGroup.ElementAt(1).Key, "A");
            CollectionAssert.AreEquivalent(readOnlyGroup.ElementAt(1), aItemsList);

            Assert.IsFalse(isCountPropertyChangedEventRaised);
            Assert.IsNotNull(collectionChangedEventArgs);
            Assert.AreEqual(collectionChangedEventsCount, 1);

            bool isMoveEventValid = IsMoveEventValid(collectionChangedEventArgs, groups[oldIndex], oldIndex, newIndex);

            Assert.IsTrue(isMoveEventValid);
        }

        [TestCategory("Collections")]
        [TestMethod]
        public void ClearSource_ShoudClear()
        {
            NotifyCollectionChangedEventArgs collectionChangedEventArgs = null;
            var collectionChangedEventsCount = 0;
            var isCountPropertyChangedEventRaised = false;
            var aItemsList = new[] { 1, 2, 3 };
            var bItemsList = new[] { 2, 4, 6 };
            var groups = new List<IGrouping<string, int>>
            {
                new IntGroup("A", aItemsList),
                new IntGroup("B", bItemsList),
            };
            var source = new ObservableGroupedCollection<string, int>(groups);
            var readOnlyGroup = new ReadOnlyObservableGroupedCollection<string, int>(source);
            ((INotifyCollectionChanged)readOnlyGroup).CollectionChanged += (s, e) =>
            {
                collectionChangedEventArgs = e;
                collectionChangedEventsCount++;
            };
            ((INotifyPropertyChanged)readOnlyGroup).PropertyChanged += (s, e) => isCountPropertyChangedEventRaised = isCountPropertyChangedEventRaised || e.PropertyName == nameof(readOnlyGroup.Count);

            source.Clear();

            Assert.AreEqual(readOnlyGroup.Count, 0);

            Assert.IsTrue(isCountPropertyChangedEventRaised);
            Assert.IsNotNull(collectionChangedEventArgs);
            Assert.AreEqual(collectionChangedEventsCount, 1);

            bool isResetEventValid = IsResetEventValid(collectionChangedEventArgs);

            Assert.IsTrue(isResetEventValid);
        }

        [TestCategory("Collections")]
        [TestMethod]
        public void ReplaceGroupInSource_ShoudReplaceGroup()
        {
            NotifyCollectionChangedEventArgs collectionChangedEventArgs = null;
            var collectionChangedEventsCount = 0;
            var isCountPropertyChangedEventRaised = false;
            var aItemsList = new[] { 1, 2, 3 };
            var bItemsList = new[] { 2, 4, 6 };
            var cItemsList = new[] { 7, 8, 9 };
            var groups = new List<IGrouping<string, int>>
            {
                new IntGroup("A", aItemsList),
                new IntGroup("B", bItemsList),
            };
            var source = new ObservableGroupedCollection<string, int>(groups);
            var readOnlyGroup = new ReadOnlyObservableGroupedCollection<string, int>(source);
            ((INotifyCollectionChanged)readOnlyGroup).CollectionChanged += (s, e) =>
            {
                collectionChangedEventArgs = e;
                collectionChangedEventsCount++;
            };
            ((INotifyPropertyChanged)readOnlyGroup).PropertyChanged += (s, e) => isCountPropertyChangedEventRaised = isCountPropertyChangedEventRaised || e.PropertyName == nameof(readOnlyGroup.Count);

            source[0] = new ObservableGroup<string, int>("C", cItemsList);

            Assert.AreEqual(readOnlyGroup.Count, 2);

            Assert.AreEqual(readOnlyGroup.ElementAt(0).Key, "C");
            CollectionAssert.AreEquivalent(readOnlyGroup.ElementAt(0), cItemsList);

            Assert.AreEqual(readOnlyGroup.ElementAt(1).Key, "B");
            CollectionAssert.AreEquivalent(readOnlyGroup.ElementAt(1), bItemsList);

            Assert.IsFalse(isCountPropertyChangedEventRaised);
            Assert.IsNotNull(collectionChangedEventArgs);
            Assert.AreEqual(collectionChangedEventsCount, 1);

            bool isReplaceEventValid = IsReplaceEventValid(collectionChangedEventArgs, aItemsList, cItemsList);

            Assert.IsTrue(isReplaceEventValid);
        }

        private static bool IsAddEventValid(NotifyCollectionChangedEventArgs args, IEnumerable<int> expectedGroupItems, int addIndex)
        {
            var newItems = args.NewItems?.Cast<IEnumerable<int>>();
            return args.Action == NotifyCollectionChangedAction.Add &&
                    args.NewStartingIndex == addIndex &&
                    args.OldItems == null &&
                    newItems?.Count() == 1 &&
                    Enumerable.SequenceEqual(newItems.ElementAt(0), expectedGroupItems);
        }

        private static bool IsRemoveEventValid(NotifyCollectionChangedEventArgs args, IEnumerable<int> expectedGroupItems, int oldIndex)
        {
            var oldItems = args.OldItems?.Cast<IEnumerable<int>>();
            return args.Action == NotifyCollectionChangedAction.Remove &&
                    args.NewItems == null &&
                    args.OldStartingIndex == oldIndex &&
                    oldItems?.Count() == 1 &&
                    Enumerable.SequenceEqual(oldItems.ElementAt(0), expectedGroupItems);
        }

        private static bool IsMoveEventValid(NotifyCollectionChangedEventArgs args, IEnumerable<int> expectedGroupItems, int oldIndex, int newIndex)
        {
            var oldItems = args.OldItems?.Cast<IEnumerable<int>>();
            var newItems = args.NewItems?.Cast<IEnumerable<int>>();
            return args.Action == NotifyCollectionChangedAction.Move &&
                    args.OldStartingIndex == oldIndex &&
                    args.NewStartingIndex == newIndex &&
                    oldItems?.Count() == 1 &&
                    Enumerable.SequenceEqual(oldItems.ElementAt(0), expectedGroupItems) &&
                    newItems?.Count() == 1 &&
                    Enumerable.SequenceEqual(newItems.ElementAt(0), expectedGroupItems);
        }

        private static bool IsReplaceEventValid(NotifyCollectionChangedEventArgs args, IEnumerable<int> expectedRemovedItems, IEnumerable<int> expectedAddItems)
        {
            var oldItems = args.OldItems?.Cast<IEnumerable<int>>();
            var newItems = args.NewItems?.Cast<IEnumerable<int>>();
            return args.Action == NotifyCollectionChangedAction.Replace &&
                    oldItems?.Count() == 1 &&
                    Enumerable.SequenceEqual(oldItems.ElementAt(0), expectedRemovedItems) &&
                    newItems?.Count() == 1 &&
                    Enumerable.SequenceEqual(newItems.ElementAt(0), expectedAddItems);
        }

        private static bool IsResetEventValid(NotifyCollectionChangedEventArgs args) => args.Action == NotifyCollectionChangedAction.Reset && args.NewItems == null && args.OldItems == null;
    }
}