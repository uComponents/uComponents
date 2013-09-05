// --------------------------------------------------------------------------------------------------------------------
// <summary>
// 30.01.2013 - Created [Ove Andersen]
// 05.09.2013 - Updated to use custom collection implementation [Ove Andersen]
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace uComponents.DataTypes.DataTypeGrid.Model
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Xml;

    using umbraco.MacroEngines;

    /// <summary>
    /// Represents rows in a DataTypeGrid
    /// </summary>
    [ComVisible(false)]
    [DebuggerDisplay("Count = {Count}")]
    [Serializable]
    public class GridRowCollection : IList<GridRow>, IList
    {
        /// <summary>
        /// The synchronization locker
        /// </summary>
        private readonly object syncRoot = new object();

        /// <summary>
        /// The inner collection
        /// </summary>
        private readonly Collection<GridRow> items = new Collection<GridRow>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GridRowCollection" /> class.
        /// </summary>
        public GridRowCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GridRowCollection" /> class.
        /// </summary>
        /// <param name="xml">The XML.</param>
        public GridRowCollection(string xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);

            var items = doc.DocumentElement;

            if (items != null && items.HasChildNodes)
            {
                foreach (XmlNode item in items.ChildNodes)
                {
                    if (item.Attributes != null)
                    {
                        var id = int.Parse(item.Attributes["id"].Value);

                        var row = new GridRow(id)
                                      {
                                          SortOrder = int.Parse(item.Attributes["sortOrder"].Value)
                                      };

                        foreach (XmlNode node in item.ChildNodes)
                        {
                            if (node.Attributes != null)
                            {
                                var cell = new GridCell()
                                {
                                    Alias = node.Name,
                                    Name = node.Attributes["nodeName"].Value,
                                    DataType = int.Parse(node.Attributes["nodeType"].Value),
                                    Value = node.InnerText
                                };

                                row.Add(cell);
                            }
                        }

                        this.Add(row);
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GridRowCollection"/> class.
        /// </summary>
        /// <param name="list">The list.</param>
        public GridRowCollection(IEnumerable<GridRow> list)
        {
            foreach (var item in list)
            {
                this.Add(item);
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <value>The count.</value>
        /// <returns>The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
        public int Count
        {
            get
            {
                return this.items.Count;
            }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.
        /// </summary>
        /// <value>The asynchronous root.</value>
        /// <returns>An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.</returns>
        public object SyncRoot
        {
            get
            {
                return this.syncRoot;
            }
        }

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe).
        /// </summary>
        /// <value><c>true</c> if this instance is synchronized; otherwise, <c>false</c>.</value>
        /// <returns>true if access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe); otherwise, false.</returns>
        public bool IsSynchronized
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </summary>
        /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
        /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only; otherwise, false.</returns>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.IList" /> has a fixed size.
        /// </summary>
        /// <value><c>true</c> if this instance is fixed size; otherwise, <c>false</c>.</value>
        /// <returns>true if the <see cref="T:System.Collections.IList" /> has a fixed size; otherwise, false.</returns>
        public bool IsFixedSize
        {
            get
            {
                return false;
            }   
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <returns>
        /// The element at the specified index.
        /// </returns>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception>
        public GridRow this[int index]
        {
            get
            {
                return this.items[index];
            }

            set
            {
                lock (this.syncRoot)
                {
                    this.items[index] = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <returns>
        /// The element at the specified index.
        /// </returns>
        /// <param name="index">The zero-based index of the element to get or set. </param>
        object IList.this[int index]
        {
            get
            {
                return this[index];
            }

            set
            {
                var r = value as GridRow;

                if (r == null)
                {
                    throw new ArgumentException("Wrong value type", "value");
                }

                this[index] = r;
            }
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="IEnumerable" /> to <see cref="GridRowCollection" />.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator GridRowCollection(List<GridRow> list)
        {
            return new GridRowCollection(list);
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <exception cref="System.ArgumentException">An item with the same id has already been added.;item</exception>
        public void Add(GridRow item)
        {
            // Throw error if id is not unique
            if (this.items.Any(x => x.Id == item.Id))
            {
                throw new ArgumentException("An item with the same id has already been added.", "item");
            }

            lock (this.syncRoot)
            {
                this.items.Add(item);
            }
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.IList"/>.
        /// </summary>
        /// <returns>
        /// The position into which the new element was inserted, or -1 to indicate that the item was not inserted into the collection,
        /// </returns>
        /// <param name="value">The object to add to the <see cref="T:System.Collections.IList"/>. </param>
        public int Add(object value)
        {
            var i = value as GridRow;

            if (i == null)
            {
                throw new ArgumentException("Wrong value type", "value");
            }

            this.Add(i);

            return this.Count - 1;
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        public void Clear()
        {
            lock (this.syncRoot)
            {
                this.items.Clear();
            }
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false.</returns>
        public bool Contains(GridRow item)
        {
            return this.items.Contains(item);
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.IList" /> contains a specific value.
        /// </summary>
        /// <param name="value">The object to locate in the <see cref="T:System.Collections.IList" />.</param>
        /// <returns>true if the <see cref="T:System.Object" /> is found in the <see cref="T:System.Collections.IList" />; otherwise, false.</returns>
        /// <exception cref="System.ArgumentException">Wrong value type;value</exception>
        public bool Contains(object value)
        {
            var i = value as GridRow;

            if (i == null)
            {
                throw new ArgumentException("Wrong value type", "value");
            }

            return this.Contains(i);
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        public void CopyTo(GridRow[] array, int arrayIndex)
        {
            this.items.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.ICollection" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="System.ArgumentException">Invalid array type;array</exception>
        public void CopyTo(Array array, int index)
        {
            var a = array as GridRow[];

            if (a == null)
            {
                throw new ArgumentException("Invalid array type", "array");
            }

            this.CopyTo(a, index);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<GridRow> GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1" />.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1" />.</param>
        /// <returns>The index of <paramref name="item" /> if found in the list; otherwise, -1.</returns>
        public int IndexOf(GridRow item)
        {
            return this.items.IndexOf(item);
        }
        
        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.IList" />.
        /// </summary>
        /// <param name="value">The object to locate in the <see cref="T:System.Collections.IList" />.</param>
        /// <returns>The index of <paramref name="value" /> if found in the list; otherwise, -1.</returns>
        /// <exception cref="System.ArgumentException">Wrong value type;value</exception>
        public int IndexOf(object value)
        {
            var i = value as GridRow;

            if (i == null)
            {
                throw new ArgumentException("Wrong value type", "value");
            }

            return this.IndexOf(i);
        }

        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1" /> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1" />.</param>
        /// <exception cref="System.ArgumentException">An item with the same id has already been added.;item</exception>
        public void Insert(int index, GridRow item)
        {
            // Throw error if id is not unique
            if (this.items.Any(x => x.Id == item.Id))
            {
                throw new ArgumentException("An item with the same id has already been added.", "item");
            }

            lock (this.syncRoot)
            {
                this.items.Insert(index, item);
            }
        }
        
        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.IList" /> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="value" /> should be inserted.</param>
        /// <param name="value">The object to insert into the <see cref="T:System.Collections.IList" />.</param>
        /// <exception cref="System.ArgumentException">Wrong value type;value</exception>
        public void Insert(int index, object value)
        {
            var i = value as GridRow;

            if (i == null)
            {
                throw new ArgumentException("Wrong value type", "value");
            }

            this.Insert(index, i);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
        public bool Remove(GridRow item)
        {
            lock (this.syncRoot)
            {
                return this.items.Remove(item);
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.IList" />.
        /// </summary>
        /// <param name="value">The object to remove from the <see cref="T:System.Collections.IList" />.</param>
        /// <exception cref="System.ArgumentException">Wrong value type;value</exception>
        public void Remove(object value)
        {
            var i = value as GridRow;

            if (i == null)
            {
                throw new ArgumentException("Wrong value type", "value");
            }

            this.Remove(i);
        }

        /// <summary>
        /// Removes the <see cref="T:System.Collections.Generic.IList`1" /> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        void IList<GridRow>.RemoveAt(int index)
        {
            lock (this.syncRoot)
            {
                this.items.RemoveAt(index);
            }
        }

        /// <summary>
        /// Removes the <see cref="T:System.Collections.IList"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove. </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.IList"/>. </exception>
        void IList.RemoveAt(int index)
        {
            lock (this.syncRoot)
            {
                this.items.RemoveAt(index);
            }
        }

        /// <summary>
        /// Converts the collection to a <see cref="DynamicXml"/> object.
        /// </summary>
        /// <returns>The collection as dynamic xml.</returns>
        public DynamicXml AsDynamicXml()
        {
            var xml = "<items>";

            // Convert all rows
            foreach (var item in this)
            {
                xml += item.AsDynamicXml().ToXml();
            }

            xml += "</items>";

            return new DynamicXml(xml);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return this.AsDynamicXml().ToXml();
        }
    }
}