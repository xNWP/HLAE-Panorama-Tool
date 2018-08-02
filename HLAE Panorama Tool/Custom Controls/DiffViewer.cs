using HLAE_Panorama_Tool.Core.DiffGenerators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HLAE_Panorama_Tool.Custom_Controls
{
    public class DiffViewer : Control
    {
        static DiffViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DiffViewer), new FrameworkPropertyMetadata(typeof(DiffViewer)));
        }

        #region Properties
        public static readonly DependencyProperty InfoContentProperty =
            DependencyProperty.Register("InfoContent", typeof(string), typeof(DiffViewer));

        /// <summary>
        /// Gets or sets the Info Label Content.
        /// </summary>
        /// <value>The content.</value>
        [Description("Gets or sets the Info Label Content."), Category("Common")]
        public string InfoContent
        {
            get { return (string)GetValue(InfoContentProperty); }
            set { SetValue(InfoContentProperty, value); }
        }

        public static readonly DependencyProperty InfoWidthProperty =
            DependencyProperty.Register("InfoWidth", typeof(double), typeof(DiffViewer)
                , new PropertyMetadata(50.0));

        /// <summary>
        /// Gets or sets the Info Label Width.
        /// </summary>
        /// <value>The width.</value>
        [Description("Gets or Sets the Info Label Width"), Category("Layout")]
        public double InfoWidth
        {
            get { return (double)GetValue(InfoWidthProperty); }
            set { SetValue(InfoWidthProperty, value); }
        }

        public static readonly DependencyProperty DiffSourceContentProperty =
            DependencyProperty.Register("DiffSourceContent", typeof(string), typeof(DiffViewer));

        /// <summary>
        /// Gets or sets the Diff Source Content.
        /// </summary>
        /// <value>The content.</value>
        [Description("Gets or sets the Diff Source Content."), Category("Common")]
        public string DiffSourceContent
        {
            get { return (string)GetValue(DiffSourceContentProperty); }
            set { SetValue(DiffSourceContentProperty, value); }
        }

        public static readonly DependencyProperty DiffDestinationContentProperty =
            DependencyProperty.Register("DiffDestinationContent", typeof(string), typeof(DiffViewer));

        /// <summary>
        /// Gets or sets the Diff Destination Content.
        /// </summary>
        /// <value>The content.</value>
        [Description("Gets or sets the Diff Destination Content."), Category("Common")]
        public string DiffDestinationContent
        {
            get { return (string)GetValue(DiffDestinationContentProperty); }
            set { SetValue(DiffDestinationContentProperty, value); }
        }

        public new static readonly DependencyProperty BorderBrushProperty =
            DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(DiffViewer),
                new PropertyMetadata(Brushes.Black));

        /// <summary>
        /// Gets or sets the border brush for Diff sections.
        /// </summary>
        /// <value>The brush to use.</value>
        [Description("Gets or sets the border brush for Diff sections."), Category("Brush")]
        public new Brush BorderBrush
        {
            get { return (Brush)GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }

        public new static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register("BorderThickness", typeof(Thickness), typeof(DiffViewer),
                new PropertyMetadata(new Thickness(1)));

        /// <summary>
        /// Gets or sets the border thickness for Diff sections.
        /// </summary>
        /// <value>The thickness. ( ͡° ͜ʖ ͡°)</value>
        [Description("Gets or sets the border thickness for Diff sections."), Category("Appearance")]
        public new Thickness BorderThickness
        {
            get { return (Thickness)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

        public static readonly DependencyProperty InfoBackgroundProperty =
            DependencyProperty.Register("InfoBackground", typeof(Brush), typeof(DiffViewer),
                new PropertyMetadata(Brushes.DimGray));

        /// <summary>
        /// Gets or sets the info section background.
        /// </summary>
        /// <value>The InfoBackground Brush.</value>
        [Description("Gets or sets the info section background."), Category("Brushes")]
        public Brush InfoBackground
        {
            get { return (Brush)GetValue(InfoBackgroundProperty); }
            set { SetValue(InfoBackgroundProperty, value); }
        }

        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(DiffViewer),
                new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets whether or not the diff has been "enabled" (typically used for merges).
        /// </summary>
        /// <value>The value, true or false.</value>
        [Description("Gets or sets whether or not the diff has been \"enabled\" (typically used for merges)."), Category("Common")]
        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Difference that the DiffViewer displays.
        /// Configuration of the DiffViewer will change depending on the type of DifferenceObject.
        /// </summary>
        public Difference Difference
        {
            get { return this.difference; }
            set { this.difference = value; OnDifferenceChanged(); }
        }

        public static readonly DependencyProperty DiffSourceVisibilityProperty =
            DependencyProperty.Register("DiffSourceVisibility", typeof(Visibility), typeof(DiffViewer),
                new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Gets or sets the visibility of the Diff Source Content.
        /// </summary>
        /// <value>The value, Visibility.</value>
        [Description("Gets or sets the visibility property of the Diff Source Content."), Category("Common")]
        public Visibility DiffSourceVisibility
        {
            get { return (Visibility)GetValue(DiffSourceVisibilityProperty); }
            set { SetValue(DiffSourceVisibilityProperty, value); }
        }

        public static readonly DependencyProperty DiffDestinationVisibilityProperty =
            DependencyProperty.Register("DiffDestinationVisibility", typeof(Visibility), typeof(DiffViewer),
                new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Gets or sets the visibility of the Diff Destination Content.
        /// </summary>
        /// <value>The value, Visibility.</value>
        [Description("Gets or sets the visibility property of the Diff Destination Content."), Category("Common")]
        public Visibility DiffDestinationVisibility
        {
            get { return (Visibility)GetValue(DiffDestinationVisibilityProperty); }
            set { SetValue(DiffDestinationVisibilityProperty, value); }
        }

        public static readonly DependencyProperty InfoLabelVisibilityProperty =
            DependencyProperty.Register("InfoLabelVisibility", typeof(Visibility), typeof(DiffViewer),
                new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Gets or sets the visibility of the Info Label.
        /// </summary>
        /// <value>The value, Visibility.</value>
        [Description("Gets or sets the visibility property of the Info Label."), Category("Common")]
        public Visibility InfoLabelVisibility
        {
            get { return (Visibility)GetValue(InfoLabelVisibilityProperty); }
            set { SetValue(InfoLabelVisibilityProperty, value); }
        }
        #endregion

        #region Events
        public static RoutedEvent ClickEvent =
            EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DiffViewer));

        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        protected virtual void OnClick()
        {
            RoutedEventArgs args = new RoutedEventArgs(ClickEvent, this);
            RaiseEvent(args);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            OnClick();
        }

        protected virtual void OnDifferenceChanged()
        {
            if (difference == null)
                throw new NullReferenceException("NullReferenceException: diffViewer difference object not initialized.");

            if (this.difference.Mode == DifferenceMode.Modification)
            {
                this.DiffSourceContent = this.difference.NewData;
                this.DiffDestinationContent = this.difference.OldData;
                this.InfoContent = "<< Modification >>\n" + this.difference.Class;
            }
            else if (this.difference.Mode == DifferenceMode.Insertion)
            {
                this.DiffSourceContent = this.difference.NewData;
                this.InfoContent = "<< Insertion >>\n" + this.difference.Class;
            }
            else if (this.difference.Mode == DifferenceMode.Deletion)
            {
                this.DiffDestinationContent = this.difference.OldData;
                this.InfoContent = "<< Deletion >>\n" + this.difference.Class;
            }
            else
                throw new InvalidEnumArgumentException(String.Format("Invalid DifferenceMode enum : {0} : {1}", this.difference.Mode.ToString()
                    , this.difference.Mode.ToString("X")));
        }
        #endregion

        #region Properties
        private Difference difference;
        #endregion
    }
}
