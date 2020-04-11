using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CommonsLib_FLL.Adapters
{
    /// <summary>
    /// Simple Base List Adapter to handle a set of items.
    /// </summary>
    /// <typeparam name="TItem">Item type.</typeparam>
    public abstract class BaseListAdapter<TItem>
         where TItem : class, new()
    {
        public ListView InternalListView { get; }
        public List<TItem> InternalList { get; }
        
        private bool _isInitialized = false;

        
        /// <summary>
        /// Item Click Event.
        /// </summary>
        public event Action<TItem>? OnItemClicked;

        public event Action<TItem>? OnItemDoubleClicked;

        /// <summary>
        /// Set String Array with ListView Columns Name.
        /// </summary>
        /// <returns>String Array.</returns>
        public abstract string[] Names { get; }

        /// <summary>
        /// Set String Array with ListView Items Data.
        /// </summary>
        /// <param name="entity">Instance to extract the values.</param>
        /// <returns>String Array.</returns>
        public abstract string[] GetValues(TItem entity);

        /// <summary>
        /// Set int Array for Columns Width (width percentage for every column).
        /// </summary>
        /// <returns>int Array.</returns>
        public abstract int[] ColWidths { get; }

        
        public BaseListAdapter(List<TItem> list, ListView listView)
        {
            this.InternalList = list;
            this.InternalListView = listView;
            Init();
        }

        /// <summary>
        /// Initialize Components
        /// </summary>
        private void Init()
        {
            if (_isInitialized)
                return;
            _isInitialized = true;

            InternalListView.View = View.Details;
            InternalListView.GridLines = true;
            InternalListView.FullRowSelect = true;
            InternalListView.MultiSelect = false;

            InternalListView.ItemSelectionChanged -= AdapterListView_ItemSelectionHandler;
            InternalListView.ItemSelectionChanged += AdapterListView_ItemSelectionHandler;

            InternalListView.DoubleClick -= AdapterListView_DoubleClickHandler;
            InternalListView.DoubleClick += AdapterListView_DoubleClickHandler;
        }

        /// <summary>
        /// Load Data (items) in Internal ListView.
        /// </summary>
        public void ReloadData()
        {
            InternalListView.Clear();
            var arrayWidth = ColWidths;
            var indexArray = 0;
            var widthListView = (int)(InternalListView.Width * 0.96);

            //set Columns with width
            foreach (var col in Names)
            {
                InternalListView.Columns.Add(col);
                InternalListView.Columns[indexArray].Width = (int)((arrayWidth[indexArray] * widthListView) / 100.0);
                indexArray++;
            }

            //Set Items
            foreach (var entity in InternalList)
            {
                InternalListView.Items.Add(new ListViewItem(this.GetValues(entity)));
            }
        }

        /// <summary>
        /// Catch Click Item Event.
        /// </summary>
        private void AdapterListView_ItemSelectionHandler(object sender, ListViewItemSelectionChangedEventArgs args)
        {
            if (!args.IsSelected) return;
            var index = args.ItemIndex;
            var item = InternalList[index];
            OnItemClicked?.Invoke(item);
        }
        
        /// <summary>
        /// Catch double click event
        /// </summary>
        private void AdapterListView_DoubleClickHandler(object sender, EventArgs e)
        {
            var selectedItem = SelectedItem;
            if (selectedItem == null)
                return;
            OnItemDoubleClicked?.Invoke(selectedItem);
        }

        /// <summary>
        /// Gets the currently selected item, or null if there is no item selected.
        /// </summary>
        /// <returns>Selected Item.</returns>
        public TItem? SelectedItem
        {
            get
            {
                var selectedIndices = InternalListView.SelectedIndices;
                if (selectedIndices.Count == 0)
                    return null;
                var selectedIndex = selectedIndices[0];
                return InternalList[selectedIndex];
            }
        }

    }

}