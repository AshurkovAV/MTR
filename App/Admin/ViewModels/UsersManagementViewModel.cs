using System.Windows;
using System.Windows.Input;
using Autofac;
using BLToolkit.Mapping;
using Core.Extensions;
using Core.Infrastructure;
using Core.Services;
using DataModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Medical.AppLayer.Editors;
using Medical.AppLayer.Linq;
using Medical.AppLayer.Managers;
using Medical.AppLayer.Services;
using Medical.CoreLayer.PropertyGrid;
using Medical.CoreLayer.Service;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Admin.ViewModels
{
    public class UsersManagementViewModel : ViewModelBase, IHash
    {
        private readonly IMedicineRepository _repository;
        private readonly INotifyManager _notifyManager;
        private readonly IAdminService _adminService;
        private readonly IMessageService _messageService;

        #region IHash

        public string Hash
        {
            get { return typeof (UsersManagementViewModel).FullName; }
        }

        #endregion

        private PropertyGridViewModel<EditUserViewModel> _editUserModel;
        private localUser _currentRow;
        private PLinqUserList _userListSource;
        
        private RelayCommand _editUserCommand;
        private RelayCommand _addUserCommand;
        private RelayCommand _deleteUserCommand;
        

        public PropertyGridViewModel<EditUserViewModel> EditUserModel
        {
            get { return _editUserModel; }
            set { _editUserModel = value; RaisePropertyChanged(() => EditUserModel); }
        }

        public localUser CurrentRow
        {
            get { return _currentRow; }
            set
            {
                _currentRow = value;
                RaisePropertyChanged(() => CurrentRow);
            }
        }

        public PLinqUserList UserListSource
        {
            get { return _userListSource; }
            set { _userListSource = value; RaisePropertyChanged(() => UserListSource); }
        }

        public UsersManagementViewModel(PLinqUserList userList, 
            IMedicineRepository repository, 
            INotifyManager notifyManager,
            IAdminService adminService,
            IMessageService messageService)
        {
            _repository = repository;
            _notifyManager = notifyManager;
            _adminService = adminService;
            _messageService = messageService;

            UserListSource = userList;
        }

        public ICommand DeleteUserCommand => _deleteUserCommand ?? (_deleteUserCommand = new RelayCommand(DeleteUser, CanDeleteUser));
        private bool CanDeleteUser()
        {
            return CurrentRow.IsNotNull();
        }

        private void DeleteUser()
        {
            if (_messageService.AskQuestion("Вы действительно хотите удалить пользователя?","Внимание"))
            {
                var deleteResult = _repository.DeleteUser(CurrentRow.UserID);
                if (deleteResult.Success)
                {
                    Reload();
                    _notifyManager.ShowNotify("Пользователь успешно удален");
                }
            }
        }

        public ICommand AddUserCommand => _addUserCommand ?? (_addUserCommand = new RelayCommand(AddUser));
        private void AddUser()
        {
            var model = new PropertyGridViewModel<EditUserViewModel>(new EditUserViewModel());
            var view = new CommonWindow(Application.Current.MainWindow, 800, 600, model);
            model.OkCallback = () =>
            {
                var tmp = Map.ObjectToObject<localUser>(model.SelectedObject.Classifier);
                var result = _adminService.CreateUser(tmp);
                if (result.Success)
                {
                    Reload();
                    view.Close();
                    _notifyManager.ShowNotify("Пользователь успешно добавлен.");
                }
            };

            view.ShowDialog();
        }

        public ICommand EditUserCommand => _editUserCommand ?? (_editUserCommand = new RelayCommand(EditUser, CanEditUser));

        private bool CanEditUser()
        {
            return true;
        }

        private void EditUser()
        {
            var copy = Map.ObjectToObject<localUser>(CurrentRow);
            var model = new PropertyGridViewModel<EditUserViewModel>(new EditUserViewModel(copy));
            var view = new CommonWindow(Application.Current.MainWindow, 800, 600, model);

            model.OkCallback = () =>
            {
                var tmp = Map.ObjectToObject<localUser>(model.SelectedObject.Classifier);
                if (CurrentRow.Pass != tmp.Pass)
                {
                    var genResult = _adminService.GenerateNewPassword(tmp.Pass);
                    if (genResult.Success)
                    {
                        tmp.Pass = genResult.Data.Item1;
                        tmp.Salt = genResult.Data.Item2;
                    }
                }
                var result = _repository.Update(tmp);
                if (result.Success)
                {
                    var map = model.SelectedObject.MapObject<localUser>(CurrentRow);
                    CurrentRow.Update(map.Affected);
                    _notifyManager.ShowNotify("Данные пользователя успешно обновлены.");
                }
                view.Close();
            };

            view.ShowDialog();
        }

        private void Reload()
        {
            UserListSource = Di.I.Resolve<PLinqUserList>();
        }
    }
}
