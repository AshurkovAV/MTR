namespace Medical.AppLayer.Managers
{
    public class OperatorManager 
	{
        /*private readonly FastCreateEvent _control;
        public int MedicalAccountId { get; set; }
        public int PatientId { get; set; }
        public bool IsError { get; set; }
        public bool IsSrz { get; set; }
        public bool _isDirty = true;
        public FastEditorViewModel Model { get; set; }

        public override object Control
        {
            get
            {
                return _control;
            }
        }

        

        public override void Load()
        {
           
        }

        public override void Save()
        {
            var result = Model.InsertOrUpdateEvent();
            _isDirty = false;
        }

       

        public override bool IsDirty
        {
            get { return _isDirty; }
        }

        public EditorSDView()
		{
            _control = new FastCreateEvent();
            TitleName = "Редактор";
		}

        public EditorSDView InitFromPatient(int patientId)
        {
            Model = _control.InitFromPatient(patientId);
            TitleName = string.Format("Пациент {0}", _control.PatientSummary);
            PatientId = patientId;
            return this;
        }

        public EditorSDView InitFromAccount(int accountId)
        {
            Model = _control.InitFromAccountId(accountId);
            TitleName = string.Format("Счет {0}", _control.AccountSummary);
            MedicalAccountId = accountId;
            return this;
        }

        public EditorSDView InitFromAccountOnlyError(int accountId)
        {
            Model = _control.InitFromAccountIdOnlyError(accountId);
            TitleName = string.Format("Счет {0}(ошибки)", _control.AccountSummary);
            MedicalAccountId = accountId;
            IsError = true;
            return this;
        }

        public EditorSDView InitFromAccountOnlySrz(int accountId)
        {
            Model = _control.InitFromAccountIdOnlySrz(accountId);
            TitleName = string.Format("Счет {0}(СРЗ)", _control.AccountSummary);
            MedicalAccountId = accountId;
            IsSrz = true;
            return this;
        }

        public EditorSDView InitFromAccountMo(int accountId)
        {
            Model = _control.InitFromAccountIdMo(accountId);
            TitleName = string.Format("Счет МО {0}", _control.AccountSummary);
            MedicalAccountId = accountId;

            return this;
        }

        public EditorSDView InitFromAccountMoOnlyError(int accountId)
        {
            Model = _control.InitFromAccountIdMoOnlyError(accountId);
            TitleName = string.Format("Счет MO {0}(ошибки)", _control.AccountSummary);
            MedicalAccountId = accountId;
            IsError = true;
            return this;
        }

        public override void Dispose()
        {
            
        }*/

	}
}
