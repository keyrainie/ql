using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using Newegg.Oversea.Silverlight.Controls.Components;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Text;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.Common;
using Newegg.Oversea.Silverlight.Utilities.Resources;
using ECCentral.Portal.Basic.Components.Facades;

namespace ECCentral.Portal.Basic.Components.UserControls.Language
{
    public partial class UCMultiLanguageMaintain : UserControl
    {
        public IDialog Dialog { get; set; }
        public int SysNo { get; set; }
        public string BizEntityType { get; set; }
        public string MappingTable { get; set; }

        private MultiLanguageMaintainFacade facade;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SysNo">系统编号</param>
        /// <param name="BizEntityType">BizEntityType</param>
        public UCMultiLanguageMaintain(int SysNo, string BizEntityType)
        {
            InitializeComponent();
            this.SysNo = SysNo;
            this.BizEntityType = BizEntityType;
            Loaded += new RoutedEventHandler(UCMultiLanguageMaintain_Loaded);
        }

        private void UCMultiLanguageMaintain_Loaded(object sender, RoutedEventArgs e)
        {
            facade = new MultiLanguageMaintainFacade(CPApplication.Current.CurrentPage);
            MultiLanguageDataContract multiLanguageData = new MultiLanguageDataContract();
            multiLanguageData.SysNo = SysNo;
            multiLanguageData.BizEntityType = BizEntityType;

            facade.QueryMultiLanguageBizEntity(multiLanguageData, (s, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                List<MultiLanguageBizEntity> multiLanguageBizEntities = args.Result;

                MappingTable = multiLanguageBizEntities[0].MappingTable;

                for (int i = 0; i < multiLanguageBizEntities.Count; i++)
                {
                    LanguageExpander expander = new LanguageExpander(multiLanguageBizEntities[i], i, facade);
                    LayoutRoot.Children.Add(expander);
                }

            });

        }

        private void CloseDialog(DialogResultType dialogResult)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.DialogResult = dialogResult;
                Dialog.Close();
            }
        }
    }


    #region 自定义的语言控件
    public class LanguageExpander : Expander
    {
        public Grid rootGrid;
        private MultiLanguageBizEntity languageBizEntity;
        public int rowCount = 0;
        public List<TextBlock> TbLabelsList;
        public List<TextBox> TxtValueList;
        private MultiLanguageMaintainFacade facade;

        public LanguageExpander(MultiLanguageBizEntity entity, int index, MultiLanguageMaintainFacade _facade)
        {
            TbLabelsList = new List<TextBlock>();
            TxtValueList = new List<TextBox>();

            TextBlock tbLabel;
            TextBox txtValue;

            languageBizEntity = entity;
            this.facade = _facade;
            this.IsExpanded = true;
            this.Header = entity.LanguageName;
            Grid.SetRow(this, index);

            rootGrid = new Grid();
            rootGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            rootGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            rootGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(560) });
            rootGrid.Margin = new Thickness(20, 5, 20, 5);

            #region 根据属性创建TextBlock+TextBox
            for (int i = 0; i < entity.PropertyItemList.Count; i++)
            {
                rootGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                var item = entity.PropertyItemList[i];

                if (item.InputType.Equals("S"))
                {
                    txtValue = new TextBox()
                    {
                        Tag = item.Field,
                        MaxLength = item.MaxLength
                    };
                }
                else
                {
                    txtValue = new TextBox()
                    {
                        AcceptsReturn = true,
                        VerticalScrollBarVisibility = ScrollBarVisibility.Visible,
                        TextWrapping = TextWrapping.Wrap,
                        Tag = item.Field,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        MaxLength = item.MaxLength,
                        Height = 80
                    };
                }
                if (String.IsNullOrWhiteSpace(item.Value))
                {
                    txtValue.Text = String.Empty;
                }
                else
                {
                    txtValue.Text = item.Value;
                }

                tbLabel = new TextBlock()
                {
                    Text = item.Label,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Tag = item.IsRequired,
                    Height = 20
                };

                if (item.IsRequired.Equals("Y"))
                {
                    tbLabel.Text = "*" + tbLabel.Text;
                }

                Grid.SetRow(tbLabel, i);
                Grid.SetRow(txtValue, i);

                Grid.SetColumn(tbLabel, 0);
                Grid.SetColumn(txtValue, 2);

                rootGrid.Children.Add(tbLabel);
                TbLabelsList.Add(tbLabel);
                rootGrid.Children.Add(txtValue);
                TxtValueList.Add(txtValue);
                rowCount++;
            }

            #endregion

            #region 创建保存按钮
            rootGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            Button saveButton = new Button();
            saveButton.Click += new RoutedEventHandler(btnSave_Click);
            saveButton.HorizontalAlignment = HorizontalAlignment.Right;
            saveButton.VerticalAlignment = VerticalAlignment.Top;
            saveButton.Height = 20;
            saveButton.Width = 60;
            saveButton.Content = "保存";
            Grid.SetRow(saveButton, rowCount);
            Grid.SetColumn(saveButton, 3);
            rootGrid.Children.Add(saveButton);
            #endregion

            this.Content = rootGrid;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < rowCount; i++)
            {
                if (TbLabelsList[i].Tag.Equals("Y") && string.IsNullOrEmpty(TxtValueList[i].Text.Trim()))
                {
                    MessageBox.Show("请填完必填信息再保存！必填信息用*号标出。");
                    return;
                }
            }

            facade.SetMultiLanguageBizEntity(ConverLanguageExpanderToMultiLanguageBizEntity(), (s, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                MessageBox.Show("多语言设置成功！");
            });
        }

        /// <summary>
        /// 将自定义的语言控件转化成多语言实体类MultiLanguageBizEntity
        /// </summary>
        /// <returns></returns>
        private MultiLanguageBizEntity ConverLanguageExpanderToMultiLanguageBizEntity()
        {
            MultiLanguageBizEntity m = new MultiLanguageBizEntity();
            m.PropertyItemList = new List<PropertyItem>();
            m.SysNo = languageBizEntity.SysNo;
            m.BizEntityType = languageBizEntity.BizEntityType;
            m.MappingTable = languageBizEntity.MappingTable;
            m.LanguageCode = languageBizEntity.LanguageCode;
            for (int i = 0; i < rowCount; i++)
            {
                PropertyItem item = new PropertyItem();
                item.Label = TbLabelsList[i].Text;
                item.Value = TxtValueList[i].Text;
                item.Field = TxtValueList[i].Tag.ToString().Trim();
                item.InputType = TxtValueList[i].TextWrapping == TextWrapping.Wrap ? "M" : "S";
                m.PropertyItemList.Add(item);
            }
            return m;
        }

    }
    #endregion
}