using System;
using Medical.DatabaseCore.EntityDataModel;

namespace Medical.DatabaseCore.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<EntityMedicineContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(EntityMedicineContext context)
        {
            //��������!
            //��������� ��� ������ ����������� �������� (����������� ��������), �.�. ��� ���������� �������� ������ ���������
            context.globalProcessingType.AddOrUpdate(
                p => p.ProcessingTypeId,
                new globalProcessingType
                {
                    ProcessingTypeId = 1,
                    Code = 1,
                    Name = "Build-In"
                },
                new globalProcessingType
                {
                    ProcessingTypeId = 2,
                    Code = 2,
                    Name = "SQL"
                },
                new globalProcessingType
                {
                    ProcessingTypeId = 3,
                    Code = 3,
                    Name = "C#"
                });

            context.localRole.AddOrUpdate(
                p => p.RoleID,
                new localRole
                {
                    RoleID = 1,
                    Name = "�������������"
                },
                new localRole
                {
                    RoleID = 2,
                    Name = "����� ������������������ ��������"
                },
                new localRole
                {
                    RoleID = 3,
                    Name = "������������� �����"
                });

            context.F014.AddOrUpdate(
                p => p.Id,
                new F014
                {
                    Id = 1000,
                    Kod = "1000",
                    Osn = "6.15.1",
                    Naim = "���������� ������������ ������� ����������� ������������ (������)",
                    Comments = "���������� ������������ ������� ����������� ������������ (������)",
                    DATEBEG = new DateTime(2014,01,01)
                },
                new F014
                {
                    Id = 1001,
                    Kod = "1001",
                    Osn = "6.15.2",
                    Naim = "���������� ������������ ������� ����������� ������������ (������)",
                    Comments = "���������� ������������ ������� ����������� ������������ (������)",
                    DATEBEG = new DateTime(2014, 01, 01)
                },
                new F014
                {
                    Id = 1002,
                    Kod = "1002",
                    Osn = "4.6.3",
                    Naim = "�������������� ��� ����, ������ ��������� ����������� ������ (������)",
                    Comments = "�������������� ��� ����, ������ ��������� ����������� ������ (������)",
                    DATEBEG = new DateTime(2014, 01, 01)
                },
                new F014
                {
                    Id = 1003,
                    Kod = "1003",
                    Osn = "4.4.1",
                    Naim = "C������� �� ������� ��������� ������ ���������� �������� � ���������� (������)",
                    Comments = "C������� �� ������� ��������� ������ ���������� �������� � ���������� (������)",
                    DATEBEG = new DateTime(2014, 01, 01)
                },
                new F014
                {
                    Id = 1004,
                    Kod = "1004",
                    Osn = "4.2.1",
                    Naim = "������� ���������� ��������� ����������� ������������ (������)",
                    Comments = "������� ���������� ��������� ����������� ������������ (������)",
                    DATEBEG = new DateTime(2014, 01, 01)
                },
                new F014
                {
                    Id = 1005,
                    Kod = "1005",
                    Osn = "4.2.2",
                    Naim = "������� ���������� ��������� ����������� ������������ (������)",
                    Comments = "������� ���������� ��������� ����������� ������������ (������)",
                    DATEBEG = new DateTime(2014, 01, 01)
                },
                new F014
                {
                    Id = 1006,
                    Kod = "1006",
                    Osn = "3.9.1",
                    Naim = "����������� �� �������������� ���������� ������ ������� �� ���� ����������� ����������� (������)",
                    Comments = "����������� �� �������������� ���������� ������ ������� �� ���� ����������� ����������� (������)",
                    DATEBEG = new DateTime(2014, 01, 01)
                },
                new F014
                {
                    Id = 1007,
                    Kod = "1007",
                    Osn = "3.9.2",
                    Naim = "����������� �� �������������� ������������� �� ������ �� ��� ������� � ����������� ������ ���������� (����� 30%) (������)",
                    Comments = "����������� �� �������������� ������������� �� ������ �� ��� ������� � ����������� ������ ���������� (����� 30%) (������)",
                    DATEBEG = new DateTime(2014, 01, 01)
                },
                new F014
                {
                    Id = 1008,
                    Kod = "1008",
                    Osn = "3.13.1",
                    Naim = "����������� ������� ����������� ������������ � ���������������������� ��������� 2 ���������  (������)",
                    Comments = "����������� ������� ����������� ������������ � ���������������������� ��������� 2 ���������  (������)",
                    DATEBEG = new DateTime(2014, 01, 01)
                },
                new F014
                {
                    Id = 1009,
                    Kod = "1009",
                    Osn = "3.13.2",
                    Naim = "����������� ������� ����������� ������������ � ���������������������� ��������� 3 ��������� (������)",
                    Comments = "����������� ������� ����������� ������������ � ���������������������� ��������� 3 ��������� (������)",
                    DATEBEG = new DateTime(2014, 01, 01)
                },
                new F014
                {
                    Id = 1010,
                    Kod = "1010",
                    Osn = "3.1.1",
                    Naim = "������������� ��������, ������������ ��������� ����� (������)",
                    Comments = "������������� ��������, ������������ ��������� ����� (������)",
                    DATEBEG = new DateTime(2014, 01, 01)
                },
                new F014
                {
                    Id = 1011,
                    Kod = "1011",
                    Osn = "3.1.2",
                    Naim = "���������� ��������� ����� (������)",
                    Comments = "���������� ��������� ����� (������)",
                    DATEBEG = new DateTime(2014, 01, 01)
                });

            context.globalEqmaOutcome.AddOrUpdate(
               p => p.Id,
               new globalEqmaOutcome
               {
                   Id = 1,
                   OutcomeName = "�������������"
               },
               new globalEqmaOutcome
               {
                   Id = 2,
                   OutcomeName = "���������"
               },
               new globalEqmaOutcome
               {
                   Id = 3,
                   OutcomeName = "��� �������"
               },
               new globalEqmaOutcome
               {
                   Id = 4,
                   OutcomeName = "���������"
               },
               new globalEqmaOutcome
               {
                   Id = 5,
                   OutcomeName = "������"
               },
               new globalEqmaOutcome
               {
                   Id = 6,
                   OutcomeName = "����������� ����"
               },
               new globalEqmaOutcome
               {
                   Id = 7,
                   OutcomeName = "���������(���������) �� �������������� (����)"
               },
               new globalEqmaOutcome
               {
                   Id = 8,
                   OutcomeName = "������"
               });
        }
    }
}
