using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using Autofac;
using Core.Extensions;
using Core.Services;
using DataModel;
using Medical.CoreLayer.Gui;
using Medical.DatabaseCore.Services.Cache;
using Medical.DatabaseCore.Services.Classifiers;
using Microsoft.Windows.Controls.PropertyGrid.Attributes;

namespace Medical.AppLayer.Operator.Views
{
    /// <summary>
    /// Interaction logic for ZslOperatorView.xaml
    /// </summary>
    public partial class ZslOperatorView : UserControl
    {
        public ZslOperatorView()
        {
            InitializeComponent();
            Init();
            
        }
        private ICacheRepository _cacheRepository;

        private void Init()
        {
            SexComboBoxEdit.ItemsSource = new V005CacheItemsSource().GetValues();
            F008ComboBoxEdit.ItemsSource = new F008CacheItemsSource().GetValues();
            F011ComboBoxEdit.ItemsSource = new F011CaheItemsSource().GetValues();
            F010ComboBoxEdit.ItemsSource = new F010CacheTfItemsSource().GetValues();
            V005ReprComboBoxEdit.ItemsSource = new V005CacheItemsSource().GetValues();
            F010RegionComboBoxEdit.ItemsSource = new F010CacheItemsSource().GetValues();
            V010ComboBoxEdit.ItemsSource = new V010CacheItemsSource().GetValues();
            F005ComboBoxEdit.ItemsSource = F005ComboBoxEdit2.ItemsSource = new F005CacheItemsSource().GetValues();
            V002ServiceComboBoxEdit.ItemsSource = new V002CacheItemsSource().GetValues();
            V020MeventComboBoxEdit.ItemsSource = new V020CacheItemsSource().GetValues();
            V014ComboBoxEdit.ItemsSource = new V014CacheItemsSource().GetValues();
            V021ServiceComboBoxEdit.ItemsSource = new V021CacheItemsSource().GetValues();
            V021MeventComboBoxEdit.ItemsSource = new V021CacheItemsSource().GetValues();
            V027MeventComboBoxEdit.ItemsSource = new V027CacheItemsSource().GetValues();
            viewDirComboBoxEdit.ItemsSource = new V028CacheItemsSource().GetValues();
            metssComboBoxEdit.ItemsSource = new V029CacheItemsSource().GetValues();
            V002MeventComboBoxEdit.ItemsSource = new V002CacheItemsSource().GetValues();
            V006ComboBoxEdit.ItemsSource = new V006aItemsSource().GetValues();
            V008ComboBoxEdit.ItemsSource = new V008CacheItemsSource().GetValues();
            F014ComboBoxEdit.ItemsSource = new F014CacheItemsSource().GetValues();
            PcelComboBoxEdit.ItemsSource = new V025CacheItemsSource().GetValues();
            DnComboBoxEdit.ItemsSource = new DnItemsSource().GetValues();

            RegionalAttributeComboBoxEdit.ItemsSource = new RegionalAttributeCacheItemsSource().GetValues();
            Ds1tComboBoxEdit.ItemsSource = new N018CacheItemsSource().GetValues();
            DsOnkComboBoxEdit.ItemsSource = new DsOnktItemsSource().GetValues();
            
            StageDiseaseComboBoxEdit.ItemsSource = new N002CacheItemsSource().GetValues();
            OnkTComboBoxEdit.ItemsSource = new N003CacheItemsSource().GetValues();
            OnkNComboBoxEdit.ItemsSource = new N004CacheItemsSource().GetValues();
            OnkMComboBoxEdit.ItemsSource = new N005CacheItemsSource().GetValues();
            PrConsComboBoxEdit.ItemsSource = new N019CacheItemsSource().GetValues();
            ServicesOnkTypeComboBoxEdit.ItemsSource = new N013CacheItemsSource().GetValues();
            HirTypeComboBoxEdit.ItemsSource = new N014CacheItemsSource().GetValues();
            LekTypeLComboBoxEdit.ItemsSource = new N015CacheItemsSource().GetValues();
            LekTypeVComboBoxEdit.ItemsSource = new N016CacheItemsSource().GetValues();
            LuchTypeComboBoxEdit.ItemsSource = new N017CacheItemsSource().GetValues();

            CodeShColumn.DataContext = new V024CacheItemsSource().GetValues();
            RegNumColumn.DataContext = new N020CacheItemsSource().GetValues();
            DiagTipColumn.DataContext = new DiagTypeItemsSource().GetValues();
            DiagCode7Column.DataContext = new N007CacheItemsSource().GetValues();
            DiagCode10Column.DataContext = new N010CacheItemsSource().GetValues();
            DiagRslt8Column.DataContext = new N008CacheItemsSource().GetValues();
            DiagRslt11Column.DataContext = new N011CacheItemsSource().GetValues();

            Type.DataContext = new DsItemsSource().GetValues();
        }

    }
}
