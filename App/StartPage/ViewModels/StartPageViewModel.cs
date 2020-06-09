using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Autofac;
using Autofac.Core;
using Core.Extensions;
using Core.Helpers;
using Core.Infrastructure;
using Core.Services;
using DevExpress.Xpf.LayoutControl;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Medical.CoreLayer.Attributes;
using Medical.CoreLayer.Resource;

namespace Medical.AppLayer.StartPage.ViewModels
{

    public class StartPageViewModel : ViewModelBase, IHash
    {
        private readonly ObservableCollection<Tile> _tileItemsList;
        public ObservableCollection<RelayCommand> TileCommandsList { get; set; }
        public ObservableCollection<Tile> TileItemsList { get { return _tileItemsList; } }
        private RelayCommand _saveLayoutCommand;

        #region IContextCommandContainer
        public ObservableCollection<IContextCommand> ContextCommands { get; set; }
        public string PageName { get; set; }
        #endregion

        #region IHash
        public string Hash
        {
            get { return typeof(StartPageViewModel).FullName; }
        }
        #endregion
        public StartPageViewModel()
        {
            bool first = true;
            _tileItemsList = new ObservableCollection<Tile>();
            TileCommandsList = new ObservableCollection<RelayCommand>();

            using (var scope = Di.I.BeginLifetimeScope())
            {
                var l = scope.ComponentRegistry.Registrations.SelectMany(x => x.Services)
                    .OfType<IServiceWithType>()
                    .Select(x => x.ServiceType.GetMembers())
                    .SelectMany(x => x)
                    .Select(r => new
                    {
                        attributes = r.GetCustomAttributes(true), 
                        info = r
                    })  
                    .Select(r => r).ToList();

                    var t = new List<Tuple<StartPageCommandAttribute, MemberInfo>>();
                    foreach (var content in l)
                    {
                        content.attributes
                            .Where(r=>r.GetType() == typeof(StartPageCommandAttribute))
                            .ForEachAction(r => t.Add(Tuple.Create(r as StartPageCommandAttribute,content.info)));
                    }

                    t.OrderBy(r => r.Item1.Order)
                    .GroupBy(r => r.Item1.Order)
                    .ToList()
                    .ForEach(r =>
                    {
                        r.ToList().ForEach(s =>
                        {
                            var tile = CreateItem(s.Item1, s.Item2);

                            _tileItemsList.Add(tile);
                            if (first)
                            {
                                first = false;
                                FlowLayoutControl.SetIsFlowBreak(tile, true);
                            }
                        });
                        first = true;
                    });
                }
        }

        public void RunCommand(object data)
        {
            var tuple = data as dynamic;
            if (tuple == null)
            {
                return;
            }
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var obj = scope.Resolve(tuple.Item1 as Type);
                tuple.Item2.Invoke(obj, null);
            }
        }

        private Tile CreateItem(StartPageCommandAttribute attribute, MemberInfo info)
        {
            SolidColorBrush brush = null;
            var image = new Image
                            {
                                Source = ImageSourceHelper.GetImageSource<IResourceCatalog>(attribute.ImageSource),
                                Stretch = Stretch.None
                            };

            ICommand command = new RelayCommand<object>(RunCommand);

            var color = ColorConverter.ConvertFromString(attribute.Color);
            if(color!=null)
            {
                brush = new SolidColorBrush((Color)color);
            }

            var size = TileSize.ExtraSmall;
            switch (attribute.Size)
            {
                case 0:
                    size = TileSize.ExtraSmall;
                    break;
                case 1:
                    size = TileSize.Small;
                    break;
                case 2:
                    size = TileSize.Large;
                    break;
                case 3:
                    size = TileSize.ExtraLarge;
                    break;
            }
            var item = new Tile
            {
                Header = attribute.Caption,
                Size = size,
                Background = brush,
                Command = command,
                Content = image,
                FontWeight = FontWeights.Bold,
                CommandParameter = Tuple.Create(attribute.Class, info) 
            };

            return item;
        }

        public ICommand SaveLayoutCommand
        {
            get { return _saveLayoutCommand ?? (_saveLayoutCommand = new RelayCommand(SaveLayout)); }
        }

        private void SaveLayout()
        {
            
        }
    }

}
