using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace PlantsDatabase
{
    public class AutoFilteredComboBox : ComboBox
    {
        private bool _isFilterActive;
        private int _silenceEvents;
        private ICollectionView _view;

        static AutoFilteredComboBox()
        {
            IsTextSearchEnabledProperty.OverrideMetadata(typeof(AutoFilteredComboBox),
                new FrameworkPropertyMetadata(true));
            IsEditableProperty.OverrideMetadata(typeof(AutoFilteredComboBox), new FrameworkPropertyMetadata(true));
            DisplayMemberPathProperty.OverrideMetadata(typeof(AutoFilteredComboBox),
                new FrameworkPropertyMetadata(null, DisplayMemberPathChanged));
        }

        public AutoFilteredComboBox()
        {
            Unloaded += CleanupOnUnload;

            var textProperty = DependencyPropertyDescriptor.FromProperty(
                TextProperty, typeof(AutoFilteredComboBox));
            textProperty.AddValueChanged(this, OnTextChanged);

            RegisterIsCaseSensitiveChangeNotification();
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);

            if (ItemsSource != null && DropDownOnFocus)
            {
                IsDropDownOpen = true;
            }
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            if (oldValue != null)
            {
                var oldView = CollectionViewSource.GetDefaultView(oldValue);
                if (!(oldView is BindingListCollectionView))
                {
                    oldView.Filter -= FilterPredicate;
                }
            }

            if (newValue != null)
            {
                _view = CollectionViewSource.GetDefaultView(newValue);
                if (!(_view is BindingListCollectionView))
                {
                    _view.Filter += FilterPredicate;
                }
            }

            base.OnItemsSourceChanged(oldValue, newValue);
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            if (!IsTextSearchEnabled && _silenceEvents == 0)
            {
                RefreshFilter();

                if (Text.Length > 0)
                {
                    foreach (var item in CollectionViewSource.GetDefaultView(ItemsSource))
                    {
                        var itemText = GetItemText(item);
                        int text = itemText.Length, prefix = Text.Length;
                        SelectedItem = item;

                        _silenceEvents++;
                        EditableTextBox.Text = itemText;
                        EditableTextBox.Select(prefix, text - prefix);
                        _silenceEvents--;
                        break;
                    }
                }
            }
        }

        private static void DisplayMemberPathChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var self = (AutoFilteredComboBox) o;
            self.CoerceValue(SearchTextPathProperty);
        }

        private void CleanupOnUnload(object sender, RoutedEventArgs e)
        {
            Unloaded -= CleanupOnUnload;

            if (_view != null)
            {
                _view.Filter -= FilterPredicate;
            }

            if (EditableTextBox != null)
            {
                EditableTextBox.SelectionChanged -= EditableTextBox_SelectionChanged;
                EditableTextBox.PreviewKeyUp -= EditableTextBox_PreviewKeyUp;
            }

            DependencyPropertyDescriptor.FromProperty(TextProperty, typeof(AutoFilteredComboBox))
                .RemoveValueChanged(this, OnTextChanged);
            DependencyPropertyDescriptor.FromProperty(IsCaseSensitiveProperty, typeof(AutoFilteredComboBox))
                .RemoveValueChanged(this, OnIsCaseSensitiveChanged);
        }

        public static readonly DependencyProperty IsCaseSensitiveProperty =
            DependencyProperty.Register("IsCaseSensitive", typeof(bool), typeof(AutoFilteredComboBox),
                new UIPropertyMetadata(false));

        [Description("The way the combo box treats the case sensitivity of typed text.")]
        [Category("AutoFiltered ComboBox")]
        [DefaultValue(true)]
        public bool IsCaseSensitive
        {
            [DebuggerStepThrough] get { return (bool) GetValue(IsCaseSensitiveProperty); }
            [DebuggerStepThrough] set { SetValue(IsCaseSensitiveProperty, value); }
        }

        protected virtual void OnIsCaseSensitiveChanged(object sender, EventArgs e)
        {
            if (IsCaseSensitive)
                IsTextSearchEnabled = false;

            RefreshFilter();
        }

        private void RegisterIsCaseSensitiveChangeNotification()
        {
            DependencyPropertyDescriptor.FromProperty(IsCaseSensitiveProperty, typeof(AutoFilteredComboBox))
                .AddValueChanged(
                    this, OnIsCaseSensitiveChanged);
        }

        public static readonly DependencyProperty DropDownOnFocusProperty =
            DependencyProperty.Register("DropDownOnFocus", typeof(bool), typeof(AutoFilteredComboBox),
                new UIPropertyMetadata(true));

        [Description("The way the combo box behaves when it receives focus.")]
        [Category("AutoFiltered ComboBox")]
        [DefaultValue(true)]
        public bool DropDownOnFocus
        {
            [DebuggerStepThrough] get { return (bool) GetValue(DropDownOnFocusProperty); }
            [DebuggerStepThrough] set { SetValue(DropDownOnFocusProperty, value); }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (!_isFilterActive &&
                ((e.Key >= Key.A && e.Key <= Key.Z) ||
                 (e.Key > Key.OemSemicolon && e.Key < Key.Oem102)))
            {
                _isFilterActive = true;
            }
            else if (_isFilterActive && e.Key == Key.Return)
            {
                var selectedValue = SelectedValue;
                ClearFilter();
                e.Handled = true;
                IsDropDownOpen = false;
                EditableTextBox.SelectAll();
                SelectedValue = selectedValue;
                return;
            }

            base.OnPreviewKeyDown(e);
        }

        protected override void OnDropDownClosed(EventArgs e)
        {
            base.OnDropDownClosed(e);
            ClearFilter();
        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnLostKeyboardFocus(e);
            ClearFilter();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (EditableTextBox != null)
            {
                EditableTextBox.SelectionChanged += EditableTextBox_SelectionChanged;
                EditableTextBox.PreviewKeyUp += EditableTextBox_PreviewKeyUp;
            }
        }

        private void EditableTextBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (_isFilterActive)
            {
                if (e.Key == Key.Down)
                {
                    _isFilterActive = false;
                    e.Handled = true;
                    SelectedIndex = -1;
                    SelectedIndex = 0;
                }
                else if (e.Key == Key.Up)
                {
                    _isFilterActive = false;
                    e.Handled = true;
                    SelectedIndex = -1;
                    SelectedIndex = Items.Count - 1;
                }
            }
        }

        protected TextBox EditableTextBox => (TextBox) Template.FindName("PART_EditableTextBox", this);

        private int _start, _length;

        private void EditableTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (_silenceEvents != 0 || !_isFilterActive) return;

            _silenceEvents++;
            var newStart = ((TextBox) e.OriginalSource).SelectionStart;
            var newLength = ((TextBox) e.OriginalSource).SelectionLength;

            if (newStart != _start || newLength != _length)
            {
                _start = newStart;
                _length = newLength;
                RefreshFilter();
            }

            _silenceEvents--;
        }

        public string SearchTextPath
        {
            get { return (string) GetValue(SearchTextPathProperty); }
            set { SetValue(SearchTextPathProperty, value); }
        }

        public static readonly DependencyProperty SearchTextPathProperty =
            DependencyProperty.Register("SearchTextPath", typeof(string), typeof(AutoFilteredComboBox),
                new FrameworkPropertyMetadata(null, null, CoerceSearchTextPath));

        private static object CoerceSearchTextPath(DependencyObject o, object baseValue)
        {
            var self = (AutoFilteredComboBox) o;
            return baseValue ?? self.DisplayMemberPath;
        }

        private void ClearFilter()
        {
            _isFilterActive = false;
            RefreshFilter();
        }

        private void RefreshFilter()
        {
            if (ItemsSource == null) return;

            var view = CollectionViewSource.GetDefaultView(ItemsSource);
            if (view is BindingListCollectionView)
            {
                var blcv = (BindingListCollectionView) view;
                if (!blcv.CanCustomFilter) return;

                if (string.IsNullOrEmpty(FilterPrefix))
                {
                    blcv.CustomFilter = string.Empty;
                }
                else
                {
                    var currItem = SelectedItem;

                    blcv.CustomFilter = "//" + FilterPrefix;

                    if (currItem != null)
                    {
                        blcv.MoveCurrentTo(currItem);
                    }
                }
            }
            else
            {
                view.Refresh();
            }
        }

        private bool FilterPredicate(object value)
        {
            if (value == null)
                return false;

            if (Text.Length == 0 || !_isFilterActive)
                return true;

            var prefix = Text;

            if (_length > 0 && _start + _length == Text.Length)
            {
                prefix = prefix.Substring(0, _start);
            }

            return GetItemText(value).ToLower().Contains(prefix.ToLower());
        }

        private string FilterPrefix
        {
            get
            {
                if (Text.Length == 0)
                    return string.Empty;

                var prefix = Text;

                if (_length > 0 && _start + _length == Text.Length)
                {
                    prefix = prefix.Substring(0, _start);
                }

                return prefix;
            }
        }

        private string GetItemText(object item)
        {
            if (string.IsNullOrEmpty(SearchTextPath))
            {
                return item.ToString();
            }

            var t = item.GetType();
            var objValue = t.GetProperty(SearchTextPath).GetValue(item, null);

            var strValue = string.Empty;
            if (objValue != null) strValue = objValue.ToString();

            return strValue;
        }
    }
}