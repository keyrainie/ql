using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace Newegg.Oversea.Silverlight.Controls
{
    public class StepChangingEventArgs:EventArgs
    {
        public Step OldStep {get;private set;}
        public Step NewStep {get;private set;}

        public StepChangingEventArgs(Step oldStep,Step newStep)
        {
             this.OldStep = oldStep;
             this.NewStep = NewStep;
        }
    }

    public class WizardControl:ContentControl
    {
        public static readonly DependencyProperty StepsProperty;

        private LinkedList<Step> History { get; set; }
        private string OriginalTitle { get; set;}
        private LinkedListNode<Step> Current { get; set; }
        public ObservableCollection<Step> Steps
        {
            get
            {
                return this.GetValue(StepsProperty) as ObservableCollection<Step>;
            }
            set
            {
                 this.SetValue(StepsProperty, value);
            }
        }

        public event EventHandler<StepChangingEventArgs> StepChanging;

        static WizardControl()
        {
            StepsProperty = DependencyProperty.Register("Steps", typeof(ObservableCollection<Step>), 
                typeof(WizardControl), new PropertyMetadata(null, (sender, e) => {
                    WizardControl control = sender as WizardControl;
                    ObservableCollection<Step> items = control.Steps;

                    if (items == null)
                    {
                        items = new ObservableCollection<Step>();
                        items.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler((o, args) =>
                        {
                            if ((args.Action == NotifyCollectionChangedAction.Add ||
                             args.Action == NotifyCollectionChangedAction.Replace) &&
                                args.NewItems != null)
                            {
                                foreach (Step item in args.NewItems)
                                {
                                    if (item.Content is IPage)
                                    {
                                        throw new ArgumentException("The usercontrol can not implement 'IPage' interface.");
                                    }
                                }
                            }
                        });
                    }

                    if (e.NewValue != null && e.NewValue != items)
                    {
                        foreach (Step item in e.NewValue as ObservableCollection<Step>)
                        {
                            items.Add(item);
                        }
                    }
                }));
        }

        public WizardControl():base()
        {
            this.Steps = new ObservableCollection<Step>();
            History = new LinkedList<Step>();   
        }


        public Step Next()
        {
            Step current = null;

            if (this.Current != null)
            {
                bool isExistNext = (this.Current.Next != null);
                if (isExistNext)
                {
                    current = this.Current.Next.Value;
                }
                else
                {
                    current = this.Current.Value;
                }

                current = this.GoToStep(this.Steps.IndexOf(current) + (isExistNext ? 0 : 1), !isExistNext);
                if (isExistNext)
                {
                    this.Current = this.Current.Next;
                }
            }
            else if(this.Steps.Count > 0)
            {
                current = GoToStep(0);
            }
            return current;
        }

        public Step Previous()
        {
            Step current = null;

            if (this.Current != null && this.Current.Previous != null)
            {
                current = this.Current.Previous.Value;
                GoToStep(this.Steps.IndexOf(current), false);
                this.Current = this.Current.Previous;
            }

            return current;
        }

        public Step GoToStep(int index)
        {
            return GoToStep(index, true);
        }

        private Step GoToStep(int index, bool isRecordToHistory)
        {
            //if (this.Current == null)
            //{
            //    this.OriginalTitle = string.IsNullOrEmpty(CPApplication.Current.CurrentPage.Title) ? string.Empty : CPApplication.Current.CurrentPage.Title;
            //}

            Step current = null;

            if (index >= 0 && index < this.Steps.Count)
            {
                Step previousItem = null;
                if (this.Current != null)
                {
                    previousItem = this.Current.Value;
                }
                current = this.Steps[index];
                if (isRecordToHistory)
                {
                    if (this.Current != null && this.Current.Next != null)
                    {
                        while (this.History.Last != this.Current)
                        {
                            this.History.RemoveLast();
                        }
                    }
                    this.Current = new LinkedListNode<Step>(current);
                    this.History.AddLast(this.Current);
                }

                OnStepChanging(this, new StepChangingEventArgs(previousItem, current));
                this.Content = current;

                //    if(CPApplication.Current.CurrentPage is PageBase)
                //        (CPApplication.Current.CurrentPage as PageBase).Title = string.IsNullOrEmpty(current.Title) ? this.OriginalTitle : current.Title;
            }

            return current;
        }

        protected virtual void OnStepChanging(object sender,StepChangingEventArgs e)
        {
            if (this.StepChanging != null)
            {
                this.StepChanging(sender, e);
            }
        }
       
    }

    public class Step : ContentControl
    {
        public static readonly DependencyProperty TitleProperty;

        public string Title
        {
            get
            {
                return this.GetValue(TitleProperty) as string;
            }

            set
            {
                this.SetValue(TitleProperty, value);
            }
        }

        static Step()
        {
            TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(Step),null);
        }

        public Step()
            : base()
        { }

        public Step(string title, object content):this()
        {
            this.Title = title;
            this.Content = content;
        }
    }

}
