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
            //Внимание!
            //Добавлять тут только константные значения (справочники например), т.к. при применении миграции данные обновятся
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
                    Name = "Администратор"
                },
                new localRole
                {
                    RoleID = 2,
                    Name = "Отдел межтерриториальных расчетов"
                },
                new localRole
                {
                    RoleID = 3,
                    Name = "Экономический отдел"
                });

            context.F014.AddOrUpdate(
                p => p.Id,
                new F014
                {
                    Id = 1000,
                    Kod = "1000",
                    Osn = "6.15.1",
                    Naim = "Оформление ненадлежащим образом медицинской документации (Регион)",
                    Comments = "Оформление ненадлежащим образом медицинской документации (Регион)",
                    DATEBEG = new DateTime(2014,01,01)
                },
                new F014
                {
                    Id = 1001,
                    Kod = "1001",
                    Osn = "6.15.2",
                    Naim = "Оформление ненадлежащим образом медицинской документации (Регион)",
                    Comments = "Оформление ненадлежащим образом медицинской документации (Регион)",
                    DATEBEG = new DateTime(2014, 01, 01)
                },
                new F014
                {
                    Id = 1002,
                    Kod = "1002",
                    Osn = "4.6.3",
                    Naim = "несоответствие КСГ виду, объему оказанной медицинской помощи (Регион)",
                    Comments = "несоответствие КСГ виду, объему оказанной медицинской помощи (Регион)",
                    DATEBEG = new DateTime(2014, 01, 01)
                },
                new F014
                {
                    Id = 1003,
                    Kod = "1003",
                    Osn = "4.4.1",
                    Naim = "Cокрытия МО случаев нарушения режима пребывания пациента в стационаре (Регион)",
                    Comments = "Cокрытия МО случаев нарушения режима пребывания пациента в стационаре (Регион)",
                    DATEBEG = new DateTime(2014, 01, 01)
                },
                new F014
                {
                    Id = 1004,
                    Kod = "1004",
                    Osn = "4.2.1",
                    Naim = "Дефекты оформления первичной медицинской документации (Регион)",
                    Comments = "Дефекты оформления первичной медицинской документации (Регион)",
                    DATEBEG = new DateTime(2014, 01, 01)
                },
                new F014
                {
                    Id = 1005,
                    Kod = "1005",
                    Osn = "4.2.2",
                    Naim = "Дефекты оформления первичной медицинской документации (Регион)",
                    Comments = "Дефекты оформления первичной медицинской документации (Регион)",
                    DATEBEG = new DateTime(2014, 01, 01)
                },
                new F014
                {
                    Id = 1006,
                    Kod = "1006",
                    Osn = "3.9.1",
                    Naim = "Последствия за необоснованное увеличение сроков лечения по вине медицинской организации (Регион)",
                    Comments = "Последствия за необоснованное увеличение сроков лечения по вине медицинской организации (Регион)",
                    DATEBEG = new DateTime(2014, 01, 01)
                },
                new F014
                {
                    Id = 1007,
                    Kod = "1007",
                    Osn = "3.9.2",
                    Naim = "Последствия за необоснованное представление на оплату по КСГ случаев с укороченным сроком пребывания (более 30%) (Регион)",
                    Comments = "Последствия за необоснованное представление на оплату по КСГ случаев с укороченным сроком пребывания (более 30%) (Регион)",
                    DATEBEG = new DateTime(2014, 01, 01)
                },
                new F014
                {
                    Id = 1008,
                    Kod = "1008",
                    Osn = "3.13.1",
                    Naim = "Недопущение наличия расхождений клинического и патологоанатомического диагнозов 2 категории  (Регион)",
                    Comments = "Недопущение наличия расхождений клинического и патологоанатомического диагнозов 2 категории  (Регион)",
                    DATEBEG = new DateTime(2014, 01, 01)
                },
                new F014
                {
                    Id = 1009,
                    Kod = "1009",
                    Osn = "3.13.2",
                    Naim = "Недопущение наличия расхождений клинического и патологоанатомического диагнозов 3 категории (Регион)",
                    Comments = "Недопущение наличия расхождений клинического и патологоанатомического диагнозов 3 категории (Регион)",
                    DATEBEG = new DateTime(2014, 01, 01)
                },
                new F014
                {
                    Id = 1010,
                    Kod = "1010",
                    Osn = "3.1.1",
                    Naim = "Неразглашение сведений, составляющих врачебную тайну (Регион)",
                    Comments = "Неразглашение сведений, составляющих врачебную тайну (Регион)",
                    DATEBEG = new DateTime(2014, 01, 01)
                },
                new F014
                {
                    Id = 1011,
                    Kod = "1011",
                    Osn = "3.1.2",
                    Naim = "Соблюдение врачебной тайны (Регион)",
                    Comments = "Соблюдение врачебной тайны (Регион)",
                    DATEBEG = new DateTime(2014, 01, 01)
                });

            context.globalEqmaOutcome.AddOrUpdate(
               p => p.Id,
               new globalEqmaOutcome
               {
                   Id = 1,
                   OutcomeName = "выздоровление"
               },
               new globalEqmaOutcome
               {
                   Id = 2,
                   OutcomeName = "улучшение"
               },
               new globalEqmaOutcome
               {
                   Id = 3,
                   OutcomeName = "без перемен"
               },
               new globalEqmaOutcome
               {
                   Id = 4,
                   OutcomeName = "ухудшение"
               },
               new globalEqmaOutcome
               {
                   Id = 5,
                   OutcomeName = "смерть"
               },
               new globalEqmaOutcome
               {
                   Id = 6,
                   OutcomeName = "самовольный уход"
               },
               new globalEqmaOutcome
               {
                   Id = 7,
                   OutcomeName = "переведен(направлен) на госпитализацию (куда)"
               },
               new globalEqmaOutcome
               {
                   Id = 8,
                   OutcomeName = "другое"
               });
        }
    }
}
