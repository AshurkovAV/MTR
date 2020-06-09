<Query Kind="Expression">
  <Connection>
    <ID>b1996b57-fd46-492c-bef8-8d58f766e8f6</ID>
    <Persist>true</Persist>
    <Server>.\SQLEXPRESS</Server>
    <Database>medicine_ins</Database>
    <ShowServer>true</ShowServer>
  </Connection>
</Query>

FactPatients.Where(p=>p.AccountId == 18).Where(p=>
(p.Newborn!= null && p.Newborn != "0") && 
p.PersonalIdFactPerson.PName.ToLower() != "нет" &&
p.PersonalIdFactPerson.Surname.ToLower() != "нет" &&
p.PersonalIdFactPerson.Patronymic.ToLower() != "нет").Select(p=>p.PatientId)