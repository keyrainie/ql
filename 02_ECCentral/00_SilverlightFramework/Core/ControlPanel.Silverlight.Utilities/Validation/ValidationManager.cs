using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.ServiceModel.DomainServices.Client;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Newegg.Oversea.Silverlight.Utilities.Resources;

namespace Newegg.Oversea.Silverlight.Utilities.Validation
{
    internal sealed class InternalValidator
    {      
        private List<Control> ErrorControls{get; set;}
        private List<Control> Controls { get; set; }

        internal InternalValidator()
        {
            ErrorControls = new List<Control>();
            Controls = new List<Control>();
        }

        internal bool ValidateObject(ComplexObject entity)
        {
            ValidationContext vc = new ValidationContext(entity, null, null);

            ICollection<ValidationResult> validationResults = new List<ValidationResult>();

            if (Validator.TryValidateObject(entity, vc, validationResults, true) == false)
            {
                foreach (ValidationResult error in validationResults)
                {
                    entity.ValidationErrors.Add(error);
                }
                return entity.ValidationErrors.Count == 0;
            }
            return true;
        }

        internal bool Validate(FrameworkElement container)
        {            
            ErrorControls.Clear();
            Controls.Clear();
            
            //1.找到所有需要验证的控件
            ForEachElement(container, p =>
            {
                var control = p as Control;
                var textBox = p as TextBox;
                if (control != null && control.IsEnabled && control.Visibility == Visibility.Visible)
                {
                    Controls.Add(control);
                    control.BindingValidationError -= new EventHandler<ValidationErrorEventArgs>(control_BindingValidationError);
                    control.BindingValidationError += new EventHandler<ValidationErrorEventArgs>(control_BindingValidationError);

                    //如果TextBox为只读，则不应该验证
                    if (textBox != null && textBox.IsReadOnly)
                    {
                        control.BindingValidationError -= new EventHandler<ValidationErrorEventArgs>(control_BindingValidationError);
                        Controls.Remove(control);
                    }
                }
            });

            Validate();

            //2.去触发所有验证；
            //if (ErrorControls.Count == 0)
            //{
            //    ValidateObject(entity);
            //}

            //3.定位到第一个控件
            if (ErrorControls.Count > 0)
            {
                ErrorControls.Sort((x, y) =>
                {
                    return x.TabIndex - y.TabIndex;
                });

                var firstControl = ErrorControls[0];
                ScrollToElement(firstControl);
                firstControl.Dispatcher.BeginInvoke(() =>
                {
                    //Ryan:当disable或者readonly的情况下不能进行聚焦，也不应该聚焦，聚焦也没有用，反而会因为事件冒泡影响RootVisual的KeyDown事件，从而影响快捷键
                    if (firstControl.IsEnabled)
                    {
                        var control = firstControl as TextBox;
                        if (control != null)
                        {
                            if (!control.IsReadOnly)
                            {
                                firstControl.Focus();
                            }
                        }
                        else
                        {
                            firstControl.Focus();
                        }
                    }
                });
            }

            //if (ErrorControls.Count == 0)
            //{
            //    if (entity.HasValidationErrors)
            //    {
            //        entity.ValidationErrors.Clear();
            //    }
            //}

            return ErrorControls.Count == 0;

        }

        void control_BindingValidationError(object sender, ValidationErrorEventArgs e)
        {           
            var control = e.OriginalSource as Control;

            if (e.Action == ValidationErrorEventAction.Removed && ErrorControls.Contains(control))
            {
                ErrorControls.Remove(control);
            }
            else if (e.Action == ValidationErrorEventAction.Added && !ErrorControls.Contains(control) && control.IsEnabled && control.Visibility == Visibility.Visible)
            {
                ErrorControls.Add(control);
            }
        }

        private void Validate()
        {
            var propertyName = "";
            Controls.ForEach(p =>
            {
                 FrameworkElement element = (FrameworkElement)p;

                if (p.GetType().Name == "TextBox")
                {
                    propertyName = "Text";
                }
                else if (p.GetType().Name == "Combox")
                {
                    propertyName = "SelectedValue";
                }
                else if (p.GetType().Name == "ComboBox")
                {
                    propertyName = "SelectedValue";
                }
                else if (p.GetType().Name == "DatePicker")
                {
                    propertyName = "SelectedDate";
                }
                else if (p.GetType().Name == "DateRange")
                {
                    propertyName = "SelectedRangeType";
                }
                else if (p.GetType().Name == "TimePicker")
                {
                    propertyName = "Value";
                }
                else if (p.GetType().Name == "WaterMarkTextBox")
                {
                    propertyName = "Text";
                }
                else if (p.GetType().Name == "PasswordBox")
                {
                    propertyName = "Password";
                }

                var field = element.GetType().GetFields(BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.Public)
                  .Where(q => q.FieldType == typeof(DependencyProperty) && q.Name == (propertyName + "Property"))
                  .FirstOrDefault();

                if (p.GetType().Name == "DatePicker")
                {
                    if (ValidateDateRange(p, element, field))
                    {
                        if (field != null)
                        {
                            var be = element.GetBindingExpression((DependencyProperty)field.GetValue(null));
                            if (be != null)
                            {
                                be.UpdateSource();
                            }
                        }
                    }
                }
                else
                {
                    if (field != null)
                    {
                        var be = element.GetBindingExpression((DependencyProperty)field.GetValue(null));
                        if (be != null)
                        {
                            be.UpdateSource();
                        }
                    }
                }
            });
        }

        private bool ValidateDateRange(Control p, FrameworkElement element,FieldInfo field)
        {
            DateTime? currentDate = p.GetType().GetProperty("SelectedDate").GetValue(p, null) as DateTime?;
            var entity = element.DataContext as ComplexObject;

            string maxDateFrom = RestrictDatePickerAttached.GetMaxDate(element);
            string minDateFrom = RestrictDatePickerAttached.GetMinDate(element);

            if (entity != null && currentDate != null && (currentDate < DateTime.Parse(minDateFrom) || currentDate > DateTime.Parse(maxDateFrom)))
            {
                var be = element.GetBindingExpression((DependencyProperty)field.GetValue(null));
                if (be != null && be.ParentBinding != null && be.ParentBinding.Path != null)
                {
                    if (p.Tag == null)
                    {
                        p.Tag = new ValidationResult(string.Format(ValidationResource.ValidationMessage_InvalidDate, minDateFrom, maxDateFrom), new string[] { be.ParentBinding.Path.Path });
                        entity.ValidationErrors.Add(p.Tag as ValidationResult);
                    }
                    else if (!entity.ValidationErrors.Contains(p.Tag as ValidationResult))
                    {
                        entity.ValidationErrors.Add(p.Tag as ValidationResult);
                    }
                }
                if (!ErrorControls.Contains(p))
                {
                    ErrorControls.Add(element as Control);
                }
                return false;
            }
            else if (entity != null)
            {
                if (p.Tag != null && entity.ValidationErrors.Contains(p.Tag as ValidationResult))
                {
                    entity.ValidationErrors.Remove(p.Tag as ValidationResult);
                    p.Tag = null;
                }
                if (ErrorControls.Contains(p))
                {
                    ErrorControls.Remove(element as Control);
                }
            }
            return true;
        }

        private static ScrollViewer GetScrollViewer(UIElement element)
        {
            DependencyObject parent = null;

            if (element != null)
            {
                parent = element;
                while ((parent = VisualTreeHelper.GetParent(parent)) != null)
                {
                    if (parent is ScrollViewer)
                    {
                        break;
                    }
                }
            }

            return parent as ScrollViewer;
        }

        private static void ScrollToElement(UIElement element)
        {
            ScrollViewer scrollViewer;
            if (element != null && (scrollViewer = GetScrollViewer(element)) != null)
            {
                var transform = element.TransformToVisual(scrollViewer);
                Point topPositionInScrollViewer = new Point();
                try
                {
                    topPositionInScrollViewer = element.TransformToVisual(scrollViewer).Transform(new Point());
                }
                catch
                {
                    //Do Nothing.
                    //这段代码是为了避免当验证容器已经不再visual tree中去做定位操作的话，就会出现异常；
                    //这里直接返回就可以了，不用做任何操作；
                    return;
                }
                FrameworkElement f = element as FrameworkElement;
                Point bottomPositionInScrollViewer = transform.Transform(new Point(0, f.ActualHeight));

                //如果Element在可视范围内，则不应进行滚动
                if (topPositionInScrollViewer.Y < 0)
                {
                    scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + topPositionInScrollViewer.Y - 10);
                }
                else if (bottomPositionInScrollViewer.Y > scrollViewer.ViewportHeight)
                {
                    scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + bottomPositionInScrollViewer.Y - scrollViewer.ViewportHeight + 10);
                }                
            }
        }

        private static void ForEachElement(DependencyObject root, Action<DependencyObject> action)
        {
            if (root != null)
            {
                int childCount = VisualTreeHelper.GetChildrenCount(root);
                for (int i = 0; i < childCount; i++)
                {
                    var obj = VisualTreeHelper.GetChild(root, i);

                    action(obj);

                    ForEachElement(obj, action);
                }
            }
        }
    }

    public static class ValidationManager
    {
        public static bool Validate(FrameworkElement container)
        {
            return new InternalValidator().Validate(container);
        }
    }
}
