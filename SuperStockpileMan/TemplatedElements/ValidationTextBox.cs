using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SuperStockpileMan.TemplatedElements
{
    [TemplatePart(Name = "TextBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "TextBlock", Type = typeof(TextBlock))]
    public sealed partial class ValidationTextBox : Control
    {
        private TextBox? textBox;
        private TextBlock? textBlock;

        public ValidationTextBox()
        {
            DefaultStyleKey = typeof(ValidationTextBox);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            textBox = (TextBox)GetTemplateChild("TextBox");
            textBlock = (TextBlock)GetTemplateChild("TextBlock");

            textBox.TextChanged += TextBox_TextChanged;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Text = ((TextBox)sender).Text;
            ValidateText();
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public Brush PlaceholderForeground
        {
            get => (Brush)GetValue(PlaceholderForegroundProperty);
            set => SetValue(PlaceholderForegroundProperty, value);
        }

        public string PlaceholderText
        {
            get => (string)GetValue(PlaceholderTextProperty);
            set => SetValue(PlaceholderTextProperty, value);
        }

        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public string PropertyName
        {
            get => (string)GetValue(PropertyNameProperty);
            set => SetValue(PropertyNameProperty, value);
        }

        public INotifyDataErrorInfo? ValidationErrorInfo
        {
            get => GetValue(ValidationErrorInfoProperty) as INotifyDataErrorInfo;
            set => SetValue(ValidationErrorInfoProperty, value);
        }

        public static DependencyProperty TextProperty { get; } = DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(ValidationTextBox),
            new(default(string)));

        public static DependencyProperty PlaceholderForegroundProperty { get; } = DependencyProperty.Register(
            nameof(PlaceholderForeground),
            typeof(Brush),
            typeof(ValidationTextBox),
            new(default(Brush)));

        public static DependencyProperty PlaceholderTextProperty { get; } = DependencyProperty.Register(
            nameof(PlaceholderText),
            typeof(string),
            typeof(ValidationTextBox),
            new(default(string)));

        public static DependencyProperty HeaderProperty { get; } = DependencyProperty.Register(
            nameof(Header),
            typeof(string),
            typeof(ValidationTextBox),
            new(default(string)));

        public static DependencyProperty PropertyNameProperty { get; } = DependencyProperty.Register(
            nameof(PropertyName),
            typeof(string),
            typeof(ValidationTextBox),
            new(default(string), OnPropertyNamePropertyChanged));

        public static DependencyProperty ValidationErrorInfoProperty { get; } = DependencyProperty.Register(
            nameof(ValidationErrorInfo),
            typeof(INotifyDataErrorInfo),
            typeof(ValidationTextBox),
            new(default(INotifyDataErrorInfo), OnValidationErrorInfoPropertyChanged));

        private static void OnPropertyNamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ValidationTextBox)d).ValidateText();
        }

        private static void OnValidationErrorInfoPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ValidationTextBox)d).ValidateText();
            ((INotifyDataErrorInfo)e.OldValue).ErrorsChanged -= ((ValidationTextBox)d).ValidationTextBox_ErrorsChanged;
            ((INotifyDataErrorInfo)e.NewValue).ErrorsChanged += ((ValidationTextBox)d).ValidationTextBox_ErrorsChanged;
        }

        private void ValidationTextBox_ErrorsChanged(object? sender, DataErrorsChangedEventArgs e)
        {
            ValidateText();
        }

        private void ValidateText()
        {
            if (textBox is null ||
                textBlock is null ||
                ValidationErrorInfo is null)
                return;

            ValidationResult? result = ValidationErrorInfo.GetErrors(PropertyName).OfType<ValidationResult>().FirstOrDefault();
            if (result == null)
            {
                textBlock.Text = string.Empty;
                if (App.Current.Resources["SystemFillColorSuccessBrush"] is Brush brush)
                {
                    textBox.BorderBrush = brush;
                }
            }
            else
            {
                textBlock.Text = result.ErrorMessage;
                if (App.Current.Resources["SystemFillColorCriticalBrush"] is Brush brush)
                    textBox.BorderBrush = brush;
            }
        }
    }
}
